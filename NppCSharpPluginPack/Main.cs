// NPP plugin platform for .Net v0.91.57 by Kasper B. Graversen etc.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppDemo.Utils;
using NppDemo.Forms;
using NppDemo.Tests;
using System.Linq;
using PluginNetResources = NppDemo.Properties.Resources;
using static Kbg.NppPluginNET.PluginInfrastructure.Win32;
using System.Threading;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace Kbg.NppPluginNET
{
    class Main
    {
        #region " Fields "
        internal const int UNDO_BUFFER_SIZE = 64;
        internal const string PluginName = "CSharpPluginPack";
        public static readonly string PluginConfigDirectory = Path.Combine(Npp.notepad.GetConfigDirectory(), PluginName);
        public const string PluginRepository = "https://github.com/molsonkiko/NppCSharpPluginPack";
        // general stuff things
        static Icon dockingFormIcon = null;
        private static readonly string sessionFilePath = Path.Combine(PluginConfigDirectory, "savedNppSession.xml");
        private static List<(string filepath, DateTime time, bool opened)> filesOpenedClosed = new List<(string filepath, DateTime time, bool opened)>();
        public static Settings settings = new Settings();
        public static bool bufferFinishedOpening;
        public static string activeFname = null;
        public static bool isDocTypeHTML = false;
        // forms
        public static SelectionRememberingForm selectionRememberingForm = null;
        static internal int IdAboutForm = -1;
        static internal int IdSelectionRememberingForm = -1;
        static internal int IdCloseHtmlTag = -1;
        #endregion

        #region " Startup/CleanUp "

        static internal void CommandMenuInit()
        {
            // Initialization of your plugin commands

            // with function :
            // SetCommand(int index,                            // zero based number to indicate the order of command
            //            string commandName,                   // the command name that you want to see in plugin menu
            //            NppFuncItemDelegate functionPointer,  // the symbol of function (function pointer) associated with this command. The body should be defined below. See Step 4.
            //            ShortcutKey *shortcut,                // optional. Define a shortcut to trigger this command
            //            bool check0nInit                      // optional. Make this menu item be checked visually
            //            );
            
            // the "&" before the "D" means that D is an accelerator key for selecting this option 
            PluginBase.SetCommand(0, "&Documentation", Docs);
            // the "&" before the "b" means that B is an accelerator key for selecting this option 
            PluginBase.SetCommand(1, "A&bout", ShowAboutForm); IdAboutForm = 1;
            PluginBase.SetCommand(2, "&Settings", OpenSettings);
            PluginBase.SetCommand(3, "Selection &Remembering Form", OpenSelectionRememberingForm); IdSelectionRememberingForm = 3;
            PluginBase.SetCommand(4, "Run &tests", TestRunner.RunAll);
            
            // this inserts a separator
            PluginBase.SetCommand(5, "---", null);
            PluginBase.SetCommand(6, "Use NanInf class for -inf, inf, nan!!", PrintNanInf);
            PluginBase.SetCommand(7, "Hello Notepad++", HelloFX);
            PluginBase.SetCommand(8, "What is Notepad++?", WhatIsNpp);

            PluginBase.SetCommand(9, "---", null);
            PluginBase.SetCommand(10, "Current &Full Path", InsertCurrentFullFilePath);
            PluginBase.SetCommand(11, "Current Directory", InsertCurrentDirectory);

            PluginBase.SetCommand(12, "---", null);
            
            PluginBase.SetCommand(13, "Close HTML/&XML tag automatically", CheckInsertHtmlCloseTag,
                new ShortcutKey(true, true, true, Keys.X), // this adds a keyboard shortcut for Ctrl+Alt+Shift+X
                settings.close_html_tag // this may check the plugin menu item on startup depending on settings
                ); IdCloseHtmlTag = 13;

            PluginBase.SetCommand(14, "---", null);
            PluginBase.SetCommand(15, "Get File Names Demo", GetFileNamesDemo);
            PluginBase.SetCommand(16, "Get Session File Names Demo", GetSessionFileNamesDemo);
            PluginBase.SetCommand(17, "Show files opened and closed this session", ShowFilesOpenedAndClosedThisSession);
            PluginBase.SetCommand(18, "Save Current Session Demo", SaveCurrentSessionDemo);
            PluginBase.SetCommand(19, "Print Scroll and Row Information", PrintScrollInformation);
            PluginBase.SetCommand(20, "Open a pop-up dialog", OpenPopupDialog);

        }

        static internal void SetToolBarIcons()
        {
            string iconsToUseChars = settings.toolbar_icons.ToLower();
            var iconInfo = new (Bitmap bmp, Icon icon, Icon iconDarkMode, int id, char representingChar)[]
            {
                (PluginNetResources.about_form_toolbar_bmp, PluginNetResources.about_form_toolbar, PluginNetResources.about_form_toolbar_darkmode, IdAboutForm, 'a'),
                (PluginNetResources.selection_remembering_form_toolbar_bmp, PluginNetResources.selection_remembering_form_toolbar, PluginNetResources.selection_remembering_form_toolbar_darkmode, IdSelectionRememberingForm, 's'),
                (PluginNetResources.close_html_tag_toolbar_bmp, PluginNetResources.close_html_tag_toolbar, PluginNetResources.close_html_tag_toolbar_darkmode, IdCloseHtmlTag, 'h'),
            }
                .Where(x => iconsToUseChars.IndexOf(x.representingChar) >= 0)
                .OrderBy(x => iconsToUseChars.IndexOf(x.representingChar));
            // order the icons according to their order in settings.toolbar_icons, and exclude those without their representing char listed

            foreach ((Bitmap bmp, Icon icon, Icon iconDarkMode, int funcId, char representingChar) in iconInfo)
            {
                // create struct
                toolbarIcons tbIcons = new toolbarIcons();

                // add bmp's and icons
                tbIcons.hToolbarBmp = bmp.GetHbitmap();
                tbIcons.hToolbarIcon = icon.Handle;
                tbIcons.hToolbarIconDarkMode = iconDarkMode.Handle;

                // convert to c++ pointer
                IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
                Marshal.StructureToPtr(tbIcons, pTbIcons, false);

                // call Notepad++ api to add icons
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE,
                    PluginBase._funcItems.Items[funcId]._cmdID, pTbIcons);

                // release pointer
                Marshal.FreeHGlobal(pTbIcons);
            }
        }

        public static void OnNotification(ScNotification notification)
        {
            uint code = notification.Header.Code;
            // This method is invoked whenever something is happening in notepad++
            // use eg. as
            // if (code == (uint)NppMsg.NPPN_xxx)
            // { ... }
            // or
            //
            // if (code == (uint)SciMsg.SCNxxx)
            // { ... }
            //// changing tabs
            switch (code)
            {
            // when a file starts opening (but before it is fully loaded)
            case (uint)NppMsg.NPPN_FILEBEFOREOPEN:
                bufferFinishedOpening = false;
                break;
            // when a file is finished opening
            case (uint)NppMsg.NPPN_BUFFERACTIVATED:
                bufferFinishedOpening = true;
                // When a new buffer is activated, we need to reset the connector to the Scintilla editing component.
                // This is usually unnecessary, but if there are multiple instances or multiple views,
                // we need to track which of the currently visible buffers are actually being edited.
                Npp.editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
                DoesCurrentLexerSupportCloseHtmlTag();
                // track when it was opened
                IntPtr bufferOpenedId = notification.Header.IdFrom;
                activeFname = Npp.notepad.GetFilePath(bufferOpenedId);
                filesOpenedClosed.Add((activeFname, DateTime.Now, true));
                return;
            // when the lexer language changed, re-check whether this is a document where we close HTML tags.
            case (uint)NppMsg.NPPN_LANGCHANGED:
                DoesCurrentLexerSupportCloseHtmlTag();
                break;
            // when closing a file
            case (uint)NppMsg.NPPN_FILEBEFORECLOSE:
                IntPtr bufferClosedId = notification.Header.IdFrom;
                string bufferClosedPath = Npp.notepad.GetFilePath(bufferClosedId);
                filesOpenedClosed.Add((bufferClosedPath, DateTime.Now, false));
                return;
            // the editor color scheme changed, so update form colors
            case (uint)NppMsg.NPPN_WORDSTYLESUPDATED:
                RestyleEverything();
                return;
            case (uint)SciMsg.SCN_CHARADDED:
                DoInsertHtmlCloseTag(notification.Character);
                break;
                //if (code > int.MaxValue) // windows messages
                //{
                //    int wm = -(int)code;
                //    }
                //}
            }
        }

        static internal void PluginCleanUp()
        {
            // dispose of any forms
            if (selectionRememberingForm != null && !selectionRememberingForm.IsDisposed)
            {
                selectionRememberingForm.Close();
                selectionRememberingForm.Dispose();
            }
        }
        #endregion

        #region " Menu functions "

        /// <summary>
        /// open GitHub repo with the web browser
        /// </summary>
        public static void Docs()
        {
            try
            {
                var ps = new ProcessStartInfo(PluginRepository)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),
                    "Could not open documentation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static void PrintScrollInformation()
        {
            ScrollInfo scrollInfo = Npp.editor.GetScrollInfo(ScrollInfoMask.SIF_RANGE | ScrollInfoMask.SIF_TRACKPOS | ScrollInfoMask.SIF_PAGE, ScrollInfoBar.SB_VERT);
            var scrollRatio = (double)scrollInfo.nTrackPos / (scrollInfo.nMax - scrollInfo.nPage);
            var scrollPercentage = Math.Min(scrollRatio, 1) * 100;
            Npp.editor.ReplaceSel($@"The maximum row in the current document was {scrollInfo.nMax + 1}.
A maximum of {scrollInfo.nPage} rows is visible at a time.
The current scroll ratio is {Math.Round(scrollPercentage, 2)}%.
");
        }

        /// <summary>
        /// open a new file, write a hello world type message, then play with the zoom
        /// </summary>
        static void HelloFX()
        {
            Npp.notepad.FileNew();
            Npp.editor.SetText("Hello, Notepad++...from.NET!");
            var rest = Npp.editor.GetLine(0);
            Npp.editor.SetText(rest + rest + rest);
            new Thread(CallbackHelloFX).Start();
        }

        static void CallbackHelloFX()
        {
            int currentZoomLevel = Npp.editor.GetZoom();
            int i = currentZoomLevel;
            for (int j = 0; j < 4; j++)
            {
                for (; i >= -10; i--)
                {
                    Npp.editor.SetZoom(i);
                    Thread.Sleep(30);
                }
                Thread.Sleep(100);
                for (; i <= 20; i++)
                {
                    Thread.Sleep(30);
                    Npp.editor.SetZoom(i);
                }
                Thread.Sleep(100);
            }
            for (; i >= currentZoomLevel; i--)
            {
                Thread.Sleep(30);
                Npp.editor.SetZoom(i);
            }
        }

        /// <summary>
        /// open a new buffer and slowly write out the text of text2display in the WhatIsNpp method above.
        /// </summary>
        static void WhatIsNpp()
        {
            string text2display = "Notepad++ is a free (as in \"free speech\" and also as in \"free beer\") " +
                "source code editor and Notepad replacement that supports several languages.\n" +
                "Running in the MS Windows environment, its use is governed by GPL License.\n\n" +
                "Based on a powerful editing component Scintilla, Notepad++ is written in C++ and " +
                "uses pure Win32 API and STL which ensures a higher execution speed and smaller program size.\n" +
                "By optimizing as many routines as possible without losing user friendliness, Notepad++ is trying " +
                "to reduce the world carbon dioxide emissions. When using less CPU power, the PC can throttle down " +
                "and reduce power consumption, resulting in a greener environment.";
            new Thread(new ParameterizedThreadStart(CallbackWhatIsNpp)).Start(text2display);
        }

        static void CallbackWhatIsNpp(object data)
        {
            string text2display = (string)data;
            Npp.notepad.FileNew();
            string newFileName = Npp.notepad.GetCurrentFilePath();

            Random srand = new Random(DateTime.Now.Millisecond);
            int rangeMin = 0;
            int rangeMax = 250;
            for (int i = 0; i < text2display.Length; i++)
            {
                Thread.Sleep(srand.Next(rangeMin, rangeMax) + 30);
                if (Npp.notepad.GetCurrentFilePath() != newFileName)
                    break;
                Npp.editor.AppendTextAndMoveCursor(text2display[i].ToString());
            }
        }

        static void InsertCurrentFullFilePath()
        {
            Npp.editor.ReplaceSel(Npp.GetCurrentPath(Npp.PathType.FULL_CURRENT_PATH));
        }

        static void InsertCurrentDirectory()
        {
            Npp.editor.ReplaceSel(Npp.GetCurrentPath(Npp.PathType.DIRECTORY));
        }

        /// <summary>
        /// toggle whether or not to autocomplete HTML/XML tags
        /// </summary>
        static void CheckInsertHtmlCloseTag()
        {
            bool doCloseTag = settings.close_html_tag;
            PluginBase.CheckMenuItemToggle(IdCloseHtmlTag, ref doCloseTag);
            settings.close_html_tag = doCloseTag;
            settings.SaveToIniFile();
        }

        static internal void DoesCurrentLexerSupportCloseHtmlTag()
        {
            LangType docType = LangType.L_TEXT;
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTLANGTYPE, 0, ref docType);
            isDocTypeHTML = (docType == LangType.L_HTML || docType == LangType.L_XML || docType == LangType.L_PHP);
        }

        static readonly Regex htmlTagNameRegex = new Regex(@"[\._\-:\w]", RegexOptions.Compiled);


        static internal void DoInsertHtmlCloseTag(char newChar)
        {
            if (!(isDocTypeHTML && settings.close_html_tag && newChar == '>'))
                return;

            int bufCapacity = 512;
            var pos = Npp.editor.GetCurrentPos();
            int currentPos = pos;
            int beginPos = currentPos - (bufCapacity - 1);
            int startPos = (beginPos > 0) ? beginPos : 0;
            int size = currentPos - startPos;

            if (size < 3)
                return;

            using (TextRange tr = new TextRange(startPos, currentPos, bufCapacity))
            {
                Npp.editor.GetTextRange(tr);
                string buf = tr.lpstrText;

                if (buf[size - 2] == '/')
                    return;

                int pCur = size - 2;
                while ((pCur > 0) && (buf[pCur] != '<') && (buf[pCur] != '>'))
                    pCur--;

                if (buf[pCur] == '<')
                {
                    pCur++;

                    var insertString = new StringBuilder("</");

                    while (htmlTagNameRegex.IsMatch(buf[pCur].ToString()))
                    {
                        insertString.Append(buf[pCur]);
                        pCur++;
                    }
                    insertString.Append('>');

                    if (insertString.Length > 3)
                    {
                        Npp.editor.BeginUndoAction();
                        Npp.editor.ReplaceSel(insertString.ToString());
                        Npp.editor.SetSel(pos, pos);
                        Npp.editor.EndUndoAction();
                    }
                }
            }
        }

        static void GetFileNamesDemo()
        {
            int nbFile = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, 0);
            MessageBox.Show(nbFile.ToString(), "Number of opened files:");

            using (ClikeStringArray cStrArray = new ClikeStringArray(nbFile, Win32.MAX_PATH))
            {
                if (Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMES, cStrArray.NativePointer, nbFile) != IntPtr.Zero)
                    foreach (string file in cStrArray.ManagedStringsUnicode) MessageBox.Show(file);
            }
        }
        static void GetSessionFileNamesDemo()
        {
            if (!Directory.Exists(PluginConfigDirectory) || !File.Exists(sessionFilePath))
            {
                MessageBox.Show($"No valid session file at path \"{sessionFilePath}\" in order to point to a valid session file",
                    "No valid session file", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int nbFile = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNBSESSIONFILES, 0, sessionFilePath);

            if (nbFile < 1)
            {
                MessageBox.Show($"No valid session file at path \"{sessionFilePath}\" in order to point to a valid session file",
                    "No valid session file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show($"Number of session files: {nbFile}");

            using (ClikeStringArray cStrArray = new ClikeStringArray(nbFile, Win32.MAX_PATH))
            {
                if (Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETSESSIONFILES, cStrArray.NativePointer, sessionFilePath) != IntPtr.Zero)
                    foreach (string file in cStrArray.ManagedStringsUnicode) MessageBox.Show(file);
            }
        }
        static void SaveCurrentSessionDemo()
        {
            Npp.CreateConfigSubDirectoryIfNotExists();
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SAVECURRENTSESSION, 0, sessionFilePath);
            if (!string.IsNullOrWhiteSpace(sessionFilePath))
                MessageBox.Show($"Saved Session File to: \"{sessionFilePath}\"",
                    "saved session file",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static void PrintNanInf()
        {
            bool neginf_correct = double.IsNegativeInfinity(NanInf.neginf);
            bool inf_correct = double.IsPositiveInfinity(NanInf.inf);
            bool nan_correct = double.IsNaN(NanInf.nan);
            string naninf = $@"-infinity == NanInf.neginf: {neginf_correct}
infinity == NanInf.inf: {inf_correct}
NaN == NanInf.nan: {nan_correct}
If you want these constants in your plugin, you can find them in the NanInf class in PluginInfrastructure.
DO NOT USE double.PositiveInfinity, double.NegativeInfinity, or double.NaN.
You will get a compiler error if you do.";
            Npp.notepad.FileNew();
            Npp.editor.AppendTextAndMoveCursor(naninf);
        }

        private static void ShowFilesOpenedAndClosedThisSession()
        {
            Npp.notepad.FileNew();
            var sb = new StringBuilder();
            sb.Append("Action\tFilename\tTime\r\n");
            foreach ((string filename, DateTime time, bool wasOpened) in filesOpenedClosed)
            {
                string formattedTime = time.ToString("yyyy-MM-dd HH:mm:ss");
                string openClose = wasOpened ? "open" : "close";
                sb.Append($"{filename}\t{formattedTime}\t{openClose}\r\n");
            }
            Npp.editor.SetText(sb.ToString());
        }

        //form opening stuff

        static void OpenSettings()
        {
            settings.ShowDialog();
        }

        /// <summary>
        /// Apply the appropriate styling
        /// (either generic control styling or Notepad++ styling as the case may be)
        /// to all forms.
        /// </summary>
        public static void RestyleEverything()
        {
            if (selectionRememberingForm != null && !selectionRememberingForm.IsDisposed)
                FormStyle.ApplyStyle(selectionRememberingForm, settings.use_npp_styling);
        }

        public static void OpenSelectionRememberingForm()
        {
            bool wasVisible = selectionRememberingForm != null && selectionRememberingForm.Visible;
            if (wasVisible)
                Npp.notepad.HideDockingForm(selectionRememberingForm);
            else if (selectionRememberingForm == null || selectionRememberingForm.IsDisposed)
            {
                selectionRememberingForm = new SelectionRememberingForm();
                DisplaySelectionRememberingForm(selectionRememberingForm);
            }
            else
            {
                Npp.notepad.ShowDockingForm(selectionRememberingForm);
            }
        }

        private static void DisplaySelectionRememberingForm(SelectionRememberingForm form)
        {
            using (Bitmap newBmp = new Bitmap(16, 16))
            {
                Graphics g = Graphics.FromImage(newBmp);
                ColorMap[] colorMap = new ColorMap[1];
                colorMap[0] = new ColorMap();
                colorMap[0].OldColor = Color.Fuchsia;
                colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap);
                //g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                dockingFormIcon = Icon.FromHandle(newBmp.GetHicon());
            }

            NppTbData _nppTbData = new NppTbData();
            _nppTbData.hClient = form.Handle;
            _nppTbData.pszName = form.Text;
            // the dlgDlg should be the index of funcItem where the current function pointer is in
            // this case is 15.. so the initial value of funcItem[15]._cmdID - not the updated internal one !
            _nppTbData.dlgID = IdSelectionRememberingForm;
            // dock on left
            _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_LEFT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
            _nppTbData.hIconTab = (uint)dockingFormIcon.Handle;
            _nppTbData.pszModuleName = PluginName;
            IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
            Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            Npp.notepad.ShowDockingForm(form);
        }

        static void ShowAboutForm()
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
            aboutForm.Focus();
        }

        static void OpenPopupDialog()
        {
            using (var popupForm = new PopupDialog())
                popupForm.ShowDialog();
        }
        #endregion
    }
}   
