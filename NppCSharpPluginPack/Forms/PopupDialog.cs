using NppDemo.Utils;
using Kbg.NppPluginNET;
using System.Windows.Forms;

namespace NppDemo.Forms
{
    public partial class PopupDialog : Form
    {
        public PopupDialog()
        {
            InitializeComponent();
            NppFormHelper.RegisterFormIfModeless(this, true);
            FormStyle.ApplyStyle(this, Main.settings.use_npp_styling);
            ComboBox1.Enabled = ComboBox1EnabledCheckBox.Checked;
        }

        private void ComboBox1EnabledCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            ComboBox1.Enabled = ComboBox1EnabledCheckBox.Checked;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            string msg = ComboBox1.Enabled
                ? $"ComboBox1 selected value = {ComboBox1.Text}"
                : "ComboBox1 is disabled";
            MessageBox.Show(msg);
        }

        /// <summary>
        /// suppress the default response to the Tab key
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData.HasFlag(Keys.Tab)) // this covers Tab with or without modifiers
                return true;
            return base.ProcessDialogKey(keyData);
        }

        private void PopupDialog_KeyUp(object sender, KeyEventArgs e)
        {
            NppFormHelper.GenericKeyUpHandler(this, sender, e, true);
        }
    }
}
