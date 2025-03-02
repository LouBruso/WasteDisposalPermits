namespace WasteDisposalPermits
{
    partial class SubmitButton
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
            this.Submit_label = new System.Windows.Forms.Label();
            this.Submit_textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Submit_label
            // 
            this.Submit_label.AutoSize = true;
            this.Submit_label.Location = new System.Drawing.Point(154, 68);
            this.Submit_label.Name = "Submit_label";
            this.Submit_label.Size = new System.Drawing.Size(96, 13);
            this.Submit_label.TabIndex = 1;
            this.Submit_label.Text = "PropertyID or other";
            // 
            // Submit_textBox
            // 
            this.Submit_textBox.Location = new System.Drawing.Point(157, 93);
            this.Submit_textBox.Name = "Submit_textBox";
            this.Submit_textBox.Size = new System.Drawing.Size(120, 20);
            this.Submit_textBox.TabIndex = 2;
            this.Submit_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler (TextBoxKeyUp);            
            // 
            // SubmitButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 174);
            this.Controls.Add(this.Submit_textBox);
            this.Controls.Add(this.Submit_label);
            this.Name = "SubmitButton";
            this.Text = "SubmitButton";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Submit_label;
        private System.Windows.Forms.TextBox Submit_textBox;
    }
}