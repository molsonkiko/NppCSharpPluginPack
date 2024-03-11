namespace NppDemo.Forms
{
    partial class SelectionRememberingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectionRememberingForm));
            this.CopySelectionsToStartEndsButton = new System.Windows.Forms.Button();
            this.SelectionRememberingFormTitle = new System.Windows.Forms.Label();
            this.SelectionStartEndsBox = new System.Windows.Forms.TextBox();
            this.SelectionStartEndsBoxLabel = new System.Windows.Forms.Label();
            this.SetSelectionsFromStartEndsButton = new System.Windows.Forms.Button();
            this.SaveCurrentSelectionsToFileButton = new System.Windows.Forms.Button();
            this.LoadSelectionsFromFileButton = new System.Windows.Forms.Button();
            this.OpenDarkModeTestFormButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CopySelectionsToStartEndsButton
            // 
            this.CopySelectionsToStartEndsButton.Location = new System.Drawing.Point(50, 74);
            this.CopySelectionsToStartEndsButton.Name = "CopySelectionsToStartEndsButton";
            this.CopySelectionsToStartEndsButton.Size = new System.Drawing.Size(236, 54);
            this.CopySelectionsToStartEndsButton.TabIndex = 0;
            this.CopySelectionsToStartEndsButton.Text = "Copy current selections to clipboard as list of comma-separated numbers";
            this.CopySelectionsToStartEndsButton.UseVisualStyleBackColor = true;
            this.CopySelectionsToStartEndsButton.Click += new System.EventHandler(this.CopySelectionsToStartEndsButton_Click);
            // 
            // SelectionRememberingFormTitle
            // 
            this.SelectionRememberingFormTitle.AutoSize = true;
            this.SelectionRememberingFormTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectionRememberingFormTitle.Location = new System.Drawing.Point(31, 22);
            this.SelectionRememberingFormTitle.Name = "SelectionRememberingFormTitle";
            this.SelectionRememberingFormTitle.Size = new System.Drawing.Size(273, 22);
            this.SelectionRememberingFormTitle.TabIndex = 1;
            this.SelectionRememberingFormTitle.Text = "Remember and set selections";
            // 
            // SelectionStartEndsBox
            // 
            this.SelectionStartEndsBox.Location = new System.Drawing.Point(35, 165);
            this.SelectionStartEndsBox.Multiline = true;
            this.SelectionStartEndsBox.Name = "SelectionStartEndsBox";
            this.SelectionStartEndsBox.Size = new System.Drawing.Size(118, 94);
            this.SelectionStartEndsBox.TabIndex = 2;
            // 
            // SelectionStartEndsBoxLabel
            // 
            this.SelectionStartEndsBoxLabel.AutoSize = true;
            this.SelectionStartEndsBoxLabel.Location = new System.Drawing.Point(171, 163);
            this.SelectionStartEndsBoxLabel.Name = "SelectionStartEndsBoxLabel";
            this.SelectionStartEndsBoxLabel.Size = new System.Drawing.Size(148, 96);
            this.SelectionStartEndsBoxLabel.TabIndex = 3;
            this.SelectionStartEndsBoxLabel.Text = "Enter starts and ends\r\n(0 is start of document)\r\nof regions to select\r\nas space-s" +
    "eparated list\r\nof comma-separated\r\nnumbers";
            // 
            // SetSelectionsFromStartEndsButton
            // 
            this.SetSelectionsFromStartEndsButton.Location = new System.Drawing.Point(50, 291);
            this.SetSelectionsFromStartEndsButton.Name = "SetSelectionsFromStartEndsButton";
            this.SetSelectionsFromStartEndsButton.Size = new System.Drawing.Size(236, 51);
            this.SetSelectionsFromStartEndsButton.TabIndex = 4;
            this.SetSelectionsFromStartEndsButton.Text = "Select all regions in the text box above";
            this.SetSelectionsFromStartEndsButton.UseVisualStyleBackColor = true;
            this.SetSelectionsFromStartEndsButton.Click += new System.EventHandler(this.SetSelectionsFromStartEndsButton_Click);
            // 
            // SaveCurrentSelectionsToFileButton
            // 
            this.SaveCurrentSelectionsToFileButton.Location = new System.Drawing.Point(50, 371);
            this.SaveCurrentSelectionsToFileButton.Name = "SaveCurrentSelectionsToFileButton";
            this.SaveCurrentSelectionsToFileButton.Size = new System.Drawing.Size(236, 23);
            this.SaveCurrentSelectionsToFileButton.TabIndex = 5;
            this.SaveCurrentSelectionsToFileButton.Text = "Save current selections to file";
            this.SaveCurrentSelectionsToFileButton.UseVisualStyleBackColor = true;
            this.SaveCurrentSelectionsToFileButton.Click += new System.EventHandler(this.SaveCurrentSelectionsToFileButton_Click);
            // 
            // LoadSelectionsFromFileButton
            // 
            this.LoadSelectionsFromFileButton.Location = new System.Drawing.Point(50, 420);
            this.LoadSelectionsFromFileButton.Name = "LoadSelectionsFromFileButton";
            this.LoadSelectionsFromFileButton.Size = new System.Drawing.Size(236, 28);
            this.LoadSelectionsFromFileButton.TabIndex = 6;
            this.LoadSelectionsFromFileButton.Text = "Load selections from config file";
            this.LoadSelectionsFromFileButton.UseVisualStyleBackColor = true;
            this.LoadSelectionsFromFileButton.Click += new System.EventHandler(this.LoadSelectionsFromFileButton_Click);
            // 
            // OpenDarkModeTestFormButton
            // 
            this.OpenDarkModeTestFormButton.Location = new System.Drawing.Point(38, 21);
            this.OpenDarkModeTestFormButton.Name = "OpenDarkModeTestFormButton";
            this.OpenDarkModeTestFormButton.Size = new System.Drawing.Size(236, 23);
            this.OpenDarkModeTestFormButton.TabIndex = 7;
            this.OpenDarkModeTestFormButton.Text = "Open dark mode test form";
            this.OpenDarkModeTestFormButton.UseVisualStyleBackColor = true;
            this.OpenDarkModeTestFormButton.Click += new System.EventHandler(this.OpenDarkModeTestFormButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.OpenDarkModeTestFormButton);
            this.groupBox1.Location = new System.Drawing.Point(13, 454);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 59);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // SelectionRememberingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 541);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CopySelectionsToStartEndsButton);
            this.Controls.Add(this.SelectionStartEndsBoxLabel);
            this.Controls.Add(this.SelectionStartEndsBox);
            this.Controls.Add(this.SetSelectionsFromStartEndsButton);
            this.Controls.Add(this.SaveCurrentSelectionsToFileButton);
            this.Controls.Add(this.LoadSelectionsFromFileButton);
            this.Controls.Add(this.SelectionRememberingFormTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectionRememberingForm";
            this.Text = "Remember and set selections";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectionRememberingForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label SelectionRememberingFormTitle;
        private System.Windows.Forms.Label SelectionStartEndsBoxLabel;
        public System.Windows.Forms.Button CopySelectionsToStartEndsButton;
        public System.Windows.Forms.TextBox SelectionStartEndsBox;
        public System.Windows.Forms.Button SetSelectionsFromStartEndsButton;
        public System.Windows.Forms.Button SaveCurrentSelectionsToFileButton;
        public System.Windows.Forms.Button LoadSelectionsFromFileButton;
        private System.Windows.Forms.Button OpenDarkModeTestFormButton;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}