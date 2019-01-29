// Save this to a file using filename: PSBook-7-GUIForm.cs
using System;
using System.Windows.Forms;

namespace PSBook.Chapter7
{
    public sealed class PSGUIForm : Form
    {
        public PSGUIForm()
        {
            InitializeComponent();
        }

        #region Public interfaces

        public TextBox OutputTextBox
        {
            get { return outputTextBox; }
        }

        public TextBox InputTextBox
        {
            get { return inputTextBox; }
        }

        public Button InvokeButton
        {
            get { return invokeBtn; }
        }

        public void SetInvokeButtonState(bool isEnabled)
        {
            invokeBtn.Enabled = isEnabled;
            inputTextBox.Focus();
        }

        public void SetOutputTextBoxContent(string text)
        {
            outputTextBox.Clear();
            outputTextBox.AppendText(text);
        }

        public delegate void SetInvokeButtonStateDelegate(bool isEnabled);
        public delegate void SetOutputTextBoxContentDelegate(string text);

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.invokeBtn = new System.Windows.Forms.Button();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // outputTextBox
            // 
            this.outputTextBox.BackColor = System.Drawing.Color.White;
            this.outputTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputTextBox.Location = new System.Drawing.Point(8, 41);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTextBox.Size = new System.Drawing.Size(365, 272);
            this.outputTextBox.TabIndex = 2;
            this.outputTextBox.WordWrap = false;
            // 
            // invokeBtn
            // 
            this.invokeBtn.Location = new System.Drawing.Point(298, 12);
            this.invokeBtn.Name = "invokeBtn";
            this.invokeBtn.Size = new System.Drawing.Size(75, 23);
            this.invokeBtn.TabIndex = 1;
            this.invokeBtn.Text = "Invoke";
            this.invokeBtn.UseCompatibleTextRendering = true;
            this.invokeBtn.UseVisualStyleBackColor = true;
            // 
            // inputTextBox
            // 
            this.inputTextBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputTextBox.Location = new System.Drawing.Point(8, 12);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(284, 21);
            this.inputTextBox.TabIndex = 0;
            // 
            // PSGUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 325);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.invokeBtn);
            this.Controls.Add(this.outputTextBox);
            this.Name = "PSGUIForm";
            this.Text = "PSBook Chapter 7";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button invokeBtn;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.ComponentModel.IContainer components = null;
    }
}
