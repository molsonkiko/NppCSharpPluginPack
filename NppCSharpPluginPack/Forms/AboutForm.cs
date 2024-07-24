using System.Windows.Forms;
using NppDemo.Utils;
using Kbg.NppPluginNET;

namespace NppDemo.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            NppFormHelper.RegisterFormIfModeless(this, true);
            Translator.TranslateForm(this);
            FormStyle.ApplyStyle(this, Main.settings.use_npp_styling);
            ThanksWowLinkLabel.LinkColor = ThanksWowLinkLabel.ForeColor; // hidden!
            Title.Text = Title.Text.Replace("X.Y.Z.A", Npp.AssemblyVersionString());
            DebugInfoLabel.Text = DebugInfoLabel.Text.Replace("X.Y.Z", Npp.nppVersionStr);
        }

        private void GitHubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Main.OpenUrlInWebBrowser(Main.PluginRepository);
        }

        /// <summary>
        /// maybe do something with this link?
        /// </summary>
        private void ThanksWowLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }

        /// <summary>
        /// Escape key exits the form.
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
