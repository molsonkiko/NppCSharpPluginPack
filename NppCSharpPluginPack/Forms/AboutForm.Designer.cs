namespace NppDemo.Forms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.Title = new System.Windows.Forms.Label();
            this.GitHubLink = new System.Windows.Forms.LinkLabel();
            this.Description = new System.Windows.Forms.Label();
            this.DebugInfoLabel = new System.Windows.Forms.Label();
            this.ThanksWowLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(78, 21);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(272, 20);
            this.Title.TabIndex = 0;
            this.Title.Text = "NppCSharpPluginPack vX.Y.Z.A";
            // 
            // GitHubLink
            // 
            this.GitHubLink.AutoSize = true;
            this.GitHubLink.LinkArea = new System.Windows.Forms.LinkArea(38, 36);
            this.GitHubLink.Location = new System.Drawing.Point(48, 145);
            this.GitHubLink.Name = "GitHubLink";
            this.GitHubLink.Size = new System.Drawing.Size(238, 35);
            this.GitHubLink.TabIndex = 1;
            this.GitHubLink.TabStop = true;
            this.GitHubLink.Text = "Add a link to your plugin repo here:\r\nhttps://github.com/yourName/yourRepo";
            this.GitHubLink.UseCompatibleTextRendering = true;
            this.GitHubLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GitHubLink_LinkClicked);
            // 
            // Description
            // 
            this.Description.AutoSize = true;
            this.Description.Location = new System.Drawing.Point(45, 66);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(305, 16);
            this.Description.TabIndex = 3;
            this.Description.Text = "A demo for how to make a Notepad++ plugin in C#.";
            // 
            // DebugInfoLabel
            // 
            this.DebugInfoLabel.AutoSize = true;
            this.DebugInfoLabel.Location = new System.Drawing.Point(45, 203);
            this.DebugInfoLabel.Name = "DebugInfoLabel";
            this.DebugInfoLabel.Size = new System.Drawing.Size(333, 32);
            this.DebugInfoLabel.TabIndex = 4;
            this.DebugInfoLabel.Text = "Notepad++ version: X.Y.Z. For more info about your\r\ninstallation, go to ? -> Debu" +
    "g Info on the main status bar.";
            // 
            // ThanksWowLinkLabel
            // 
            this.ThanksWowLinkLabel.AutoSize = true;
            this.ThanksWowLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(228, 234);
            this.ThanksWowLinkLabel.LinkColor = System.Drawing.Color.Black;
            this.ThanksWowLinkLabel.Location = new System.Drawing.Point(45, 257);
            this.ThanksWowLinkLabel.Name = "ThanksWowLinkLabel";
            this.ThanksWowLinkLabel.Size = new System.Drawing.Size(336, 108);
            this.ThanksWowLinkLabel.TabIndex = 5;
            this.ThanksWowLinkLabel.TabStop = true;
            this.ThanksWowLinkLabel.Text = resources.GetString("ThanksWowLinkLabel.Text");
            this.ThanksWowLinkLabel.UseCompatibleTextRendering = true;
            this.ThanksWowLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ThanksWowLink_LinkClicked);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 389);
            this.Controls.Add(this.ThanksWowLinkLabel);
            this.Controls.Add(this.DebugInfoLabel);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.GitHubLink);
            this.Controls.Add(this.Title);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AboutForm";
            this.Text = "About NppCSharpPluginPack";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.LinkLabel GitHubLink;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.Label DebugInfoLabel;
        private System.Windows.Forms.LinkLabel ThanksWowLinkLabel;
    }
}