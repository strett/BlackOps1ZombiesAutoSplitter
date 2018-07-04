namespace ZombiesAutosplitter
{
    partial class FormMain
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
            this.RTB_log = new System.Windows.Forms.RichTextBox();
            this.LB_MenuState = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RTB_log
            // 
            this.RTB_log.Location = new System.Drawing.Point(502, 12);
            this.RTB_log.Name = "RTB_log";
            this.RTB_log.Size = new System.Drawing.Size(268, 426);
            this.RTB_log.TabIndex = 0;
            this.RTB_log.Text = "";
            // 
            // LB_MenuState
            // 
            this.LB_MenuState.AutoSize = true;
            this.LB_MenuState.Location = new System.Drawing.Point(12, 15);
            this.LB_MenuState.Name = "LB_MenuState";
            this.LB_MenuState.Size = new System.Drawing.Size(45, 13);
            this.LB_MenuState.TabIndex = 1;
            this.LB_MenuState.Text = "STATE:";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 450);
            this.Controls.Add(this.LB_MenuState);
            this.Controls.Add(this.RTB_log);
            this.Name = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox RTB_log;
        private System.Windows.Forms.Label LB_MenuState;
    }
}

