using System.Windows.Forms;
using ExampleDependency;

namespace NppDemo.Forms
{
    public partial class PopupDialog : FormBase
    {
        public PopupDialog() : base(true, false)
        {
            InitializeComponent();
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
            var exampleClassMember = new ExampleClass(msg);
            MessageBox.Show(exampleClassMember.ToString());
        }
    }
}
