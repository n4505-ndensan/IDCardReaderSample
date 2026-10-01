namespace IDCardReaderSample
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            readStatusLabel = new Label();
            resultLabel = new Label();
            SuspendLayout();
            // 
            // readStatusLabel
            // 
            readStatusLabel.AutoSize = true;
            readStatusLabel.Location = new Point(21, 23);
            readStatusLabel.Name = "readStatusLabel";
            readStatusLabel.Size = new Size(55, 15);
            readStatusLabel.TabIndex = 2;
            readStatusLabel.Text = "初期化中";
            // 
            // resultLabel
            // 
            resultLabel.AutoSize = true;
            resultLabel.Font = new Font("Yu Gothic UI", 40F, FontStyle.Bold);
            resultLabel.Location = new Point(33, 60);
            resultLabel.Name = "resultLabel";
            resultLabel.Size = new Size(150, 72);
            resultLabel.TabIndex = 3;
            resultLabel.Text = "0000";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(235, 162);
            Controls.Add(resultLabel);
            Controls.Add(readStatusLabel);
            Name = "Form1";
            Text = "社員証の読み取り";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label readStatusLabel;
        private Label resultLabel;
    }
}
