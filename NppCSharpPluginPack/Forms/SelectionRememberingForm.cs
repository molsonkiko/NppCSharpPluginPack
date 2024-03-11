using NppDemo.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kbg.NppPluginNET;
using System.IO;

namespace NppDemo.Forms
{
    public partial class SelectionRememberingForm : FormBase
    {
        public DarkModeTestForm darkModeTestForm;

        public SelectionRememberingForm() : base(false, true)
        {
            InitializeComponent();
            darkModeTestForm = null;
        }

        private void CopySelectionsToStartEndsButton_Click(object sender, EventArgs e)
        {
            string startEnds = SelectionManager.StartEndListToString(SelectionManager.GetSelectedRanges());
            Clipboard.SetText(startEnds);
        }

        private void SetSelectionsFromStartEndsButton_Click(object sender, EventArgs e)
        {
            var startEndMatches = Regex.Matches(SelectionStartEndsBox.Text, @"\d+,\d+");
            if (startEndMatches.Count == 0)
            {
                MessageBox.Show("Expected a space-separated list of two comma-separated numbers, like \"1,2  3,4   5,6\"\r\n" +
                                "but no comma-separated number pairs were found",
                    "Couldn't find comma-separated numbers",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var startEndList = new List<string>(startEndMatches.Count);
            foreach (Match m in startEndMatches)
            {
                startEndList.Add(m.Value);
            }
            SelectionManager.SetSelectionsFromStartEnds(startEndList);
        }

        private void SaveCurrentSelectionsToFileButton_Click(object sender, EventArgs e)
        {
            Npp.CreateConfigSubDirectoryIfNotExists();
            var savedSelectionsFilePath = Path.Combine(Main.PluginConfigDirectory, "SavedSelections.txt");

            string startEnds = SelectionManager.StartEndListToString(SelectionManager.GetSelectedRanges(), "\r\n");
            using (var writer = new StreamWriter(savedSelectionsFilePath, false, Encoding.UTF8))
            {
                writer.Write(startEnds);
                writer.Flush();
            }
        }

        private void LoadSelectionsFromFileButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Main.PluginConfigDirectory))
            {
                MessageBox.Show("No selections were previously saved to file.", "No saved selections",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var savedSelectionsFileInfo = new FileInfo(Path.Combine(Main.PluginConfigDirectory, "SavedSelections.txt"));
            string savedSelections;
            using (StreamReader reader = savedSelectionsFileInfo.OpenText())
            {
                savedSelections = reader.ReadToEnd();
            }
            SelectionStartEndsBox.Text = savedSelections;
        }

        private void OpenDarkModeTestFormButton_Click(object sender, EventArgs e)
        {
            if (darkModeTestForm == null || darkModeTestForm.IsDisposed)
                darkModeTestForm = new DarkModeTestForm(this);
            darkModeTestForm.GrabFocus();
        }

        private void SelectionRememberingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (darkModeTestForm != null && !darkModeTestForm.IsDisposed)
                darkModeTestForm.Close();
        }

        private void SelectionRememberingForm_KeyDown(object sender, KeyEventArgs e)
        {
            NppFormHelper.GenericKeyDownHandler(sender, e);
        }

        private void SelectionRememberingForm_KeyUp(object sender, KeyEventArgs e)
        {
            NppFormHelper.GenericKeyUpHandler(this, sender, e, false);
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            NppFormHelper.TextBoxKeyPressHandler(sender, e);
        }
    }
}
