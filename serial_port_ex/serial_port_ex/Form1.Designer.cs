namespace serial_port_ex
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
            txtSend = new TextBox();
            txtReceive = new TextBox();
            btnConnect = new Button();
            btnSend = new Button();
            txtAddress = new TextBox();
            txtValue = new TextBox();
            SuspendLayout();
            // 
            // txtSend
            // 
            txtSend.Location = new Point(12, 12);
            txtSend.Multiline = true;
            txtSend.Name = "txtSend";
            txtSend.Size = new Size(474, 176);
            txtSend.TabIndex = 0;
            // 
            // txtReceive
            // 
            txtReceive.Location = new Point(12, 223);
            txtReceive.Multiline = true;
            txtReceive.Name = "txtReceive";
            txtReceive.Size = new Size(474, 215);
            txtReceive.TabIndex = 1;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(622, 31);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(140, 55);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "btnconnect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(622, 152);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(140, 73);
            btnSend.TabIndex = 3;
            btnSend.Text = "btnsend";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(12, 194);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(97, 23);
            txtAddress.TabIndex = 4;
            // 
            // txtValue
            // 
            txtValue.Location = new Point(385, 194);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(101, 23);
            txtValue.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtValue);
            Controls.Add(txtAddress);
            Controls.Add(btnSend);
            Controls.Add(btnConnect);
            Controls.Add(txtReceive);
            Controls.Add(txtSend);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSend;
        private TextBox txtReceive;
        private Button btnConnect;
        private Button btnSend;
        private TextBox txtAddress;
        private TextBox txtValue;
    }
}
