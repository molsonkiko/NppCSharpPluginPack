using NppDemo.PluginInfrastructure;
using Kbg.NppPluginNET;
using System.Windows.Forms;

namespace NppDemo.Utils
{
    /// <summary>
    /// This class listens to messages that are not broadcast by the plugins manager (e.g., Notepad++ internal messages).<br></br>
    /// The original idea comes from Peter Frentrup's NppMenuSearch plugin (https://github.com/search?q=repo%3Apeter-frentrup%2FNppMenuSearch%20NppListener&type=code)
    /// </summary>
    public class NppListener : NativeWindow
    {
        /// <summary>
        /// the NPPM_INTERNAL_RELOADNATIVELANG message is fired twice every time we change the native language preference.
        /// I don't know why, but we should ignore every other one of those messages.
        /// </summary>
        private static bool reloadNativeLangToggle = false;

        protected override void WndProc(ref Message m)
        {
            if (!Main.isShuttingDown)
            {
                switch (m.Msg)
                {
                case (int)Resource.NPPM_INTERNAL_RELOADNATIVELANG:
                    base.WndProc(ref m);
                    if (reloadNativeLangToggle)
                        Translator.ResetTranslations(false);
                    reloadNativeLangToggle = !reloadNativeLangToggle;
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }
}
