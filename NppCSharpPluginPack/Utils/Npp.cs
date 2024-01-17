using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;

/// <summary>
/// miscellaneous useful things like a connector to Notepad++
/// </summary>
namespace NppDemo.Utils
{
    /// <summary>
    /// contains connectors to Scintilla (editor) and Notepad++ (notepad)
    /// </summary>
    public class Npp
    {
        /// <summary>
        /// connector to Scintilla
        /// </summary>
        public static IScintillaGateway editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
        /// <summary>
        /// connector to Notepad++
        /// </summary>
        public static INotepadPPGateway notepad = new NotepadPPGateway();

        /// <summary>
        /// this should only be instantiated once in your entire project
        /// </summary>
        public static Random random = new Random();

        public static readonly int[] nppVersion = notepad.GetNppVersion();

        public static readonly string nppVersionStr = NppVersionString(true);

        public static readonly bool nppVersionAtLeast8 = nppVersion[0] >= 8;

        /// <summary>
        /// append text to current doc, then append newline and move cursor
        /// </summary>
        /// <param name="inp"></param>
        public static void AddLine(string inp)
        {
            editor.AppendText(Encoding.UTF8.GetByteCount(inp), inp);
            editor.AppendText(Environment.NewLine.Length, Environment.NewLine);
        }

        public enum PathType
        {
            FULL_CURRENT_PATH,
            FILE_NAME,
            DIRECTORY
        }

        /// <summary>
        /// input is one of 'p', 'd', 'f'<br></br>
        /// if 'p', get full path to current file (default)<br></br>
        /// if 'd', get directory of current file<br></br>
        /// if 'f', get filename of current file
        /// </summary>
        /// <param name="which"></param>
        /// <returns></returns>
        public static string GetCurrentPath(PathType which = PathType.FULL_CURRENT_PATH)
        {
            NppMsg msg = NppMsg.NPPM_GETFULLCURRENTPATH;
            switch (which)
            {
                case PathType.FULL_CURRENT_PATH: break;
                case PathType.DIRECTORY: msg = NppMsg.NPPM_GETCURRENTDIRECTORY; break;
                case PathType.FILE_NAME: msg = NppMsg.NPPM_GETFILENAME; break;
                default: throw new ArgumentException("GetCurrentPath argument must be member of PathType enum");
            }

            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)msg, 0, path);

            return path.ToString();
        }

        /// <summary>
        /// Get the file type for a file path (no period)<br></br>
        /// Default path is the currently open file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FileExtension(string path = null)
        {
            if (path == null)
                path = GetCurrentPath(Npp.PathType.FILE_NAME);
            StringBuilder sb = new StringBuilder();
            for (int ii = path.Length - 1; ii >= 0; ii--)
            {
                char c = path[ii];
                if (c == '.') break;
                sb.Append(c);
            }
            // the chars were added in the wrong direction, so reverse them
            return sb.ToString().Slice("::-1");
        }

        /// <summary>
        /// Trying to copy an empty string or null to the clipboard raises an error.<br></br>
        /// This shows a message box if the user tries to do that.
        /// </summary>
        /// <param name="text"></param>
        public static void TryCopyToClipboard(string text)
        {
            if (text == null || text.Length == 0)
            {
                MessageBox.Show("Couldn't find anything to copy to the clipboard",
                    "Nothing to copy to clipboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            Clipboard.SetText(text);
        }

        public static string AssemblyVersionString()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            while (version.EndsWith(".0"))
                version = version.Substring(0, version.Length - 2);
#if DEBUG
            return $"{version} Debug";
#else
            return version;
#endif // DEBUG
        }

        public static void CreateConfigSubDirectoryIfNotExists()
        {
            var ConfigDirInfo = new DirectoryInfo(Main.PluginConfigDirectory);
            if (!ConfigDirInfo.Exists)
                ConfigDirInfo.Create();
        }

        /// <summary>
        /// get all text starting at position start in the current document
        /// and ending at position end in the current document
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string GetSlice(int start, int end)
        {
            int len = end - start;
            IntPtr rangePtr = editor.GetRangePointer(start, len);
            string ansi = Marshal.PtrToStringAnsi(rangePtr, len);
            // TODO: figure out a way to do this that involves less memcopy for non-ASCII
            if (ansi.Any(c => c >= 128))
                return Encoding.UTF8.GetString(Encoding.Default.GetBytes(ansi));
            return ansi;
        }

        private static readonly string[] newlines = new string[] { "\r\n", "\r", "\n" };

        /// <summary>0: CRLF, 1: CR, 2: LF<br></br>
        /// Anything less than 0 or greater than 2: LF</summary>
        public static string GetEndOfLineString(int eolType)
        {
            if (eolType < 0 || eolType >= 3)
                return "\n";
            return newlines[eolType];
        }

        private static string NppVersionString(bool include32bitVs64bit)
        {
            int[] nppVer = notepad.GetNppVersion();
            string nppVerStr = $"{nppVer[0]}.{nppVer[1]}.{nppVer[2]}";
            return include32bitVs64bit ? $"{nppVerStr} {IntPtr.Size * 8}bit" : nppVerStr;
        }

        /// <summary>
        /// appends the JSON representation of char c to a StringBuilder.<br></br>
        /// for most characters, this just means appending the character itself, but for example '\n' would become "\\n", '\t' would become "\\t",<br></br>
        /// and most other chars less than 32 would be appended as "\\u00{char value in hex}" (e.g., '\x14' becomes "\\u0014")
        /// </summary>
        public static void CharToSb(StringBuilder sb, char c)
        {
            switch (c)
            {
            case '\\': sb.Append("\\\\"); break;
            case '"': sb.Append("\\\""); break;
            case '\x01': sb.Append("\\u0001"); break;
            case '\x02': sb.Append("\\u0002"); break;
            case '\x03': sb.Append("\\u0003"); break;
            case '\x04': sb.Append("\\u0004"); break;
            case '\x05': sb.Append("\\u0005"); break;
            case '\x06': sb.Append("\\u0006"); break;
            case '\x07': sb.Append("\\u0007"); break;
            case '\x08': sb.Append("\\b"); break;
            case '\x09': sb.Append("\\t"); break;
            case '\x0A': sb.Append("\\n"); break;
            case '\x0B': sb.Append("\\v"); break;
            case '\x0C': sb.Append("\\f"); break;
            case '\x0D': sb.Append("\\r"); break;
            case '\x0E': sb.Append("\\u000E"); break;
            case '\x0F': sb.Append("\\u000F"); break;
            case '\x10': sb.Append("\\u0010"); break;
            case '\x11': sb.Append("\\u0011"); break;
            case '\x12': sb.Append("\\u0012"); break;
            case '\x13': sb.Append("\\u0013"); break;
            case '\x14': sb.Append("\\u0014"); break;
            case '\x15': sb.Append("\\u0015"); break;
            case '\x16': sb.Append("\\u0016"); break;
            case '\x17': sb.Append("\\u0017"); break;
            case '\x18': sb.Append("\\u0018"); break;
            case '\x19': sb.Append("\\u0019"); break;
            case '\x1A': sb.Append("\\u001A"); break;
            case '\x1B': sb.Append("\\u001B"); break;
            case '\x1C': sb.Append("\\u001C"); break;
            case '\x1D': sb.Append("\\u001D"); break;
            case '\x1E': sb.Append("\\u001E"); break;
            case '\x1F': sb.Append("\\u001F"); break;
            default: sb.Append(c); break;
            }
        }

        /// <summary>
        /// the string representation of a JSON string
        /// if not quoted, this will not have the enclosing quotes a JSON string normally has
        /// </summary>
        public static string StrToString(string s, bool quoted)
        {
            int slen = s.Length;
            int ii = 0;
            for (; ii < slen; ii++)
            {
                char c = s[ii];
                if (c < 32 || c == '\\' || c == '"')
                    break;
            }
            if (ii == slen)
                return quoted ? $"\"{s}\"" : s;
            var sb = new StringBuilder();
            if (quoted)
                sb.Append('"');
            if (ii > 0)
            {
                ii--;
                sb.Append(s, 0, ii);
            }
            for (; ii < slen; ii++)
                CharToSb(sb, s[ii]);
            if (quoted)
                sb.Append('"');
            return sb.ToString();
        }

        /// <summary>
        /// Based on the value of askUser, do one of three things:<br></br>
        /// DONT_DO_DONT_ASK: return false (don't do the thing)<br></br>
        /// ASK_BEFORE_DOING:<br></br>
        /// 1. show a Yes/No message box with text messageBoxText and caption messageBoxCaption.<br></br>
        /// 2. if and only if the user clicks Yes, return true.<br></br>
        /// DO_WITHOUT_ASKING: return true (do the thing without asking)
        /// </summary>
        /// <param name="askUser">whether to ask user</param>
        /// <param name="messageBoxText">text of message box (if and only if askUser = ASK_BEFORE_DOING</param>
        /// <param name="messageBoxCaption">caption of message box (if and only if askUser = ASK_BEFORE_DOING</param>
        /// <returns></returns>
        public static bool AskBeforeDoingSomething(AskUserWhetherToDoThing askUser, string messageBoxText, string messageBoxCaption)
        {
            switch (askUser)
            {
            case AskUserWhetherToDoThing.DONT_DO_DONT_ASK:
                return false;
            case AskUserWhetherToDoThing.ASK_BEFORE_DOING:
                return MessageBox.Show(messageBoxText,
                    messageBoxCaption,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    ) != DialogResult.No;
            case AskUserWhetherToDoThing.DO_WITHOUT_ASKING:
            default:
                break;
            }
            return true;
        }
    }

    public enum AskUserWhetherToDoThing
    {
        /// <summary>
        /// don't do the thing, and don't prompt the user either
        /// </summary>
        DONT_DO_DONT_ASK,
        /// <summary>
        /// prompt the user to ask whether to do it
        /// </summary>
        ASK_BEFORE_DOING,
        /// <summary>
        /// do it without prompting the user
        /// </summary>
        DO_WITHOUT_ASKING,
    }
}
