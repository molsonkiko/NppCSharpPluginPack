using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppDemo.Utils;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NppDemo.Forms
{
    public partial class FormBase : Form
    {
        /// <summary>
        /// if true, this blocks the parent application until closed.<br></br>
        /// THIS IS ONLY TRUE OF POP-UP DIALOGS.
        /// </summary>
        public bool IsModal { get; private set; }
        /// <summary>
        /// if true, this form's default appearance is docked (attached) to the left, right, bottom, or top of the Notepad++ window.
        /// </summary>
        public bool IsDocking { get; private set; }
        /// <summary>
        /// indicates whether the form became visible for the first time<br></br>
        /// this is an unprincipled hack to deal with weirdness surrounding the opening of docking forms<br></br>
        /// since the Load and Shown events are suppressed on docking form startup.
        /// </summary>
        private bool IsLoaded = false;

        private static Win32.WindowLongGetter _wndLongGetter;
        private static Win32.WindowLongSetter _wndLongSetter;

        /// <summary>
        /// superclass of all forms in the application.<br></br>
        /// Implements many useful handlers, and deals with some weird behaviors induced by interoperating with Notepad++.
        /// </summary>
        /// <param name="isModal">if true, this blocks the parent application until closed. THIS IS ONLY TRUE OF POP-UP DIALOGS</param>
        /// <param name="isDocking">if true, this form's default appearance is docked (attached) to the left, right, bottom, or top of the Notepad++ window.</param>
        public FormBase(bool isModal, bool isDocking)
        {
            InitializeComponent();
            IsModal = isModal;
            IsDocking = isDocking;
            NppFormHelper.RegisterFormIfModeless(this, isModal);
            if (IsDocking)
            {
                if (Marshal.SizeOf(typeof(IntPtr)) == 8) // we are 64-bit
                {
                    _wndLongGetter = Win32.GetWindowLongPtr;
                    _wndLongSetter = Win32.SetWindowLongPtr;
                }
                else // we are 32-bit
                {
                    _wndLongGetter = Win32.GetWindowLong;
                    _wndLongSetter = Win32.SetWindowLong;
                }
            }
        }

        /// <summary>
        /// this is called every time the form's visibility changes,
        /// but it only does anything once, before the form is loaded for the first time.<br></br>
        /// This adds KeyUp, KeyDown, and KeyPress event handlers to all controls according to the recommendations in NppFormHelper.<br></br>
        /// It also styles the form using FormStyle.ApplyStyle
        /// </summary>
        private void FormBase_VisibleChanged(object sender, EventArgs e)
        {
            if (IsLoaded || !Visible)
                return;
            IsLoaded = true;
            // we can't put this in the base constructor
            //     because it must be called *after* the subclass constructor adds all child controls
            //     and the base constructor must be called first (that's just how C# works)
            AddKeyUpDownPressHandlers(this);
            Translator.TranslateForm(this);
            FormStyle.ApplyStyle(this, Main.settings.use_npp_styling);
        }

        /// <summary>
        /// This adds KeyUp, KeyDown, and KeyPress event handlers to all controls according to the recommendations in NppFormHelper.
        /// </summary>
        /// <param name="ctrl"></param>
        private void AddKeyUpDownPressHandlers(Control ctrl = null)
        {
            if (ctrl is null)
                ctrl = this;
            ctrl.KeyUp += (sender, e) => NppFormHelper.GenericKeyUpHandler(this, sender, e, IsModal);
            if (ctrl is TextBox tb)
                tb.KeyPress += NppFormHelper.TextBoxKeyPressHandler;
            else
                ctrl.KeyDown += NppFormHelper.GenericKeyDownHandler;
            if (ctrl.HasChildren)
            {
                foreach (Control child in ctrl.Controls)
                    AddKeyUpDownPressHandlers(child);
            }
        }

        [Obsolete("Designer only", true)]
        public FormBase()
        {
            // this only exists to make the Visual Studio Windows Forms designer happy
        }

        private void FormBase_KeyUp(object sender, KeyEventArgs e)
        {
            NppFormHelper.GenericKeyUpHandler(this, sender, e, IsModal);
        }

        private void FormBase_KeyDown(object sender, KeyEventArgs e)
        {
            NppFormHelper.GenericKeyDownHandler(sender, e);
        }

        /// <summary>
        /// suppress the default response to the Tab key
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData.HasFlag(Keys.Tab)) // this covers Tab with or without modifiers
                return true;
            return base.ProcessDialogKey(keyData);
        }


        /// <summary>
        /// this fixes a bug where Notepad++ can hang in the following situation:<br></br>
        /// 1. you are in a docking form<br></br>
        /// 2. you click a button that is a child of another control (e.g., a GroupBox)<br></br>
        /// 3. that button would cause a new form to appear<br></br>
        /// see https://github.com/BdR76/CSVLint/pull/88/commits
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (IsDocking)
            {
                switch (m.Msg)
                {
                case Win32.WM_NOTIFY:
                    var nmdr = (Win32.TagNMHDR)Marshal.PtrToStructure(m.LParam, typeof(Win32.TagNMHDR));
                    if (nmdr.hwndFrom == PluginBase.nppData._nppHandle)
                    {
                        switch ((DockMgrMsg)(nmdr.code & 0xFFFFU))
                        {
                        case DockMgrMsg.DMN_DOCK:   // we are being docked
                            break;
                        case DockMgrMsg.DMN_FLOAT:  // we are being _un_docked
                            RemoveControlParent(this);
                            break;
                        case DockMgrMsg.DMN_CLOSE:  // we are being closed
                            break;
                        }
                    }
                    break;
                }
            }
            base.WndProc(ref m);
        }

        private void RemoveControlParent(Control parent)
        {
            if (parent.HasChildren)
            {
                long extAttrs = (long)_wndLongGetter(parent.Handle, Win32.GWL_EXSTYLE);
                if (Win32.WS_EX_CONTROLPARENT == (extAttrs & Win32.WS_EX_CONTROLPARENT))
                {
                    _wndLongSetter(parent.Handle, Win32.GWL_EXSTYLE, new IntPtr(extAttrs & ~Win32.WS_EX_CONTROLPARENT));
                }
                foreach (Control c in parent.Controls)
                {
                    RemoveControlParent(c);
                }
            }
        }
    }
}
