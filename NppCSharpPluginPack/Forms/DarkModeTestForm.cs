using System.Windows.Forms;
using Kbg.NppPluginNET;
using NppDemo.Utils;

namespace NppDemo.Forms
{
    public partial class DarkModeTestForm : Form
    {
        private SelectionRememberingForm selectionRememberingForm;

        public DarkModeTestForm(SelectionRememberingForm selectionRememberingForm)
        {
            InitializeComponent();
            NppFormHelper.RegisterFormIfModeless(this, false);
            this.selectionRememberingForm = selectionRememberingForm;
            selectionRememberingForm.AddOwnedForm(this);
            FormStyle.ApplyStyle(this, Main.settings.use_npp_styling);
            comboBox1.SelectedIndex = 0;
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            row.Cells[0].Value = "Value1";
            row.Cells[1].Value = "Should look pretty";
            row.Cells[2].Value = "Value3";
            dataGridView1.Rows.Add(row);
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = !linkLabel1.LinkVisited;
        }

        private void DarkModeTestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (selectionRememberingForm != null && !selectionRememberingForm.IsDisposed)
                selectionRememberingForm.RemoveOwnedForm(this);
        }

        public void GrabFocus()
        {
            Show();
            textBox1.Focus();
        }

        private void ShowPopupDialogButton_Click(object sender, System.EventArgs e)
        {
            using (var popupDialog = new PopupDialog())
                popupDialog.ShowDialog();
        }
    }
}
