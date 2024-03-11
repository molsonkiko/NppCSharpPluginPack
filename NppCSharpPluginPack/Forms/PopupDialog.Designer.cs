namespace NppDemo.Forms
{
    partial class PopupDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                NppFormHelper.UnregisterFormIfModeless(this, true);
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.Title = new System.Windows.Forms.Label();
            this.ComboBox1EnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.ComboBox1 = new System.Windows.Forms.ComboBox();
            this.TextBox1Label = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.ComboBox1Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(43, 78);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(133, 22);
            this.TextBox1.TabIndex = 0;
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(105, 28);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(127, 22);
            this.Title.TabIndex = 1;
            this.Title.Text = "Popup dialog";
            // 
            // ComboBox1EnabledCheckBox
            // 
            this.ComboBox1EnabledCheckBox.AutoSize = true;
            this.ComboBox1EnabledCheckBox.Location = new System.Drawing.Point(43, 120);
            this.ComboBox1EnabledCheckBox.Name = "ComboBox1EnabledCheckBox";
            this.ComboBox1EnabledCheckBox.Size = new System.Drawing.Size(156, 20);
            this.ComboBox1EnabledCheckBox.TabIndex = 2;
            this.ComboBox1EnabledCheckBox.Text = "Enable ComboBox1?";
            this.ComboBox1EnabledCheckBox.UseVisualStyleBackColor = true;
            this.ComboBox1EnabledCheckBox.CheckedChanged += new System.EventHandler(this.ComboBox1EnabledCheckBox_CheckedChanged);
            // 
            // ComboBox1
            // 
            this.ComboBox1.FormattingEnabled = true;
            this.ComboBox1.Items.AddRange(new object[] {
            "item1",
            "item2"});
            this.ComboBox1.Location = new System.Drawing.Point(43, 164);
            this.ComboBox1.Name = "ComboBox1";
            this.ComboBox1.Size = new System.Drawing.Size(121, 24);
            this.ComboBox1.TabIndex = 3;
            // 
            // TextBox1Label
            // 
            this.TextBox1Label.AutoSize = true;
            this.TextBox1Label.Location = new System.Drawing.Point(197, 78);
            this.TextBox1Label.Name = "TextBox1Label";
            this.TextBox1Label.Size = new System.Drawing.Size(63, 16);
            this.TextBox1Label.TabIndex = 4;
            this.TextBox1Label.Text = "TextBox1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(128, 225);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ComboBox1Label
            // 
            this.ComboBox1Label.AutoSize = true;
            this.ComboBox1Label.Location = new System.Drawing.Point(188, 164);
            this.ComboBox1Label.Name = "ComboBox1Label";
            this.ComboBox1Label.Size = new System.Drawing.Size(81, 16);
            this.ComboBox1Label.TabIndex = 6;
            this.ComboBox1Label.Text = "ComboBox1";
            // 
            // PopupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 295);
            this.Controls.Add(this.ComboBox1Label);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TextBox1Label);
            this.Controls.Add(this.ComboBox1);
            this.Controls.Add(this.ComboBox1EnabledCheckBox);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.TextBox1);
            this.Name = "PopupDialog";
            this.Text = "PopupDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBox1;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.CheckBox ComboBox1EnabledCheckBox;
        private System.Windows.Forms.ComboBox ComboBox1;
        private System.Windows.Forms.Label TextBox1Label;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label ComboBox1Label;
    }
}