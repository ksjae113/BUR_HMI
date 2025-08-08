namespace Serial_gpt
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
            checkBoxDO = new CheckBox();
            btnOpenPort = new Button();
            btnClosePort = new Button();
            btnWriteDO = new Button();
            btnReadDI = new Button();
            txtPortName = new ComboBox();
            txtLog = new TextBox();
            txtReceive = new TextBox();
            txtWriteValue = new TextBox();
            SuspendLayout();
            // 
            // checkBoxDO
            // 
            checkBoxDO.AutoSize = true;
            checkBoxDO.Location = new Point(400, 13);
            checkBoxDO.Name = "checkBoxDO";
            checkBoxDO.Size = new Size(84, 19);
            checkBoxDO.TabIndex = 0;
            checkBoxDO.Text = "checkBox1";
            checkBoxDO.UseVisualStyleBackColor = true;
            checkBoxDO.CheckedChanged += checkBoxDO_CheckedChanged;
            // 
            // btnOpenPort
            // 
            btnOpenPort.Location = new Point(1256, 27);
            btnOpenPort.Name = "btnOpenPort";
            btnOpenPort.Size = new Size(79, 31);
            btnOpenPort.TabIndex = 1;
            btnOpenPort.Text = "open";
            btnOpenPort.UseVisualStyleBackColor = true;
            btnOpenPort.Click += btnOpenPort_Click;
            // 
            // btnClosePort
            // 
            btnClosePort.Location = new Point(1256, 77);
            btnClosePort.Name = "btnClosePort";
            btnClosePort.Size = new Size(79, 31);
            btnClosePort.TabIndex = 2;
            btnClosePort.Text = "close";
            btnClosePort.UseVisualStyleBackColor = true;
            btnClosePort.Click += btnClosePort_Click;
            // 
            // btnWriteDO
            // 
            btnWriteDO.Location = new Point(902, 36);
            btnWriteDO.Name = "btnWriteDO";
            btnWriteDO.Size = new Size(79, 31);
            btnWriteDO.TabIndex = 3;
            btnWriteDO.Text = "writeDO";
            btnWriteDO.UseVisualStyleBackColor = true;
            btnWriteDO.Click += btnWriteDO_Click;
            // 
            // btnReadDI
            // 
            btnReadDI.Location = new Point(902, 154);
            btnReadDI.Name = "btnReadDI";
            btnReadDI.Size = new Size(79, 31);
            btnReadDI.TabIndex = 4;
            btnReadDI.Text = "readDI";
            btnReadDI.UseVisualStyleBackColor = true;
            btnReadDI.Click += btnReadDI_Click;
            // 
            // txtPortName
            // 
            txtPortName.FormattingEnabled = true;
            txtPortName.Location = new Point(11, 11);
            txtPortName.Name = "txtPortName";
            txtPortName.Size = new Size(75, 23);
            txtPortName.TabIndex = 5;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(12, 390);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(647, 279);
            txtLog.TabIndex = 6;
            // 
            // txtReceive
            // 
            txtReceive.Location = new Point(686, 390);
            txtReceive.Multiline = true;
            txtReceive.Name = "txtReceive";
            txtReceive.Size = new Size(649, 279);
            txtReceive.TabIndex = 7;
            // 
            // txtWriteValue
            // 
            txtWriteValue.Location = new Point(11, 63);
            txtWriteValue.Multiline = true;
            txtWriteValue.Name = "txtWriteValue";
            txtWriteValue.Size = new Size(649, 283);
            txtWriteValue.TabIndex = 8;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1359, 681);
            Controls.Add(txtWriteValue);
            Controls.Add(txtReceive);
            Controls.Add(txtLog);
            Controls.Add(txtPortName);
            Controls.Add(btnReadDI);
            Controls.Add(btnWriteDO);
            Controls.Add(btnClosePort);
            Controls.Add(btnOpenPort);
            Controls.Add(checkBoxDO);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBoxDO;
        private Button btnOpenPort;
        private Button btnClosePort;
        private Button btnWriteDO;
        private Button btnReadDI;
        private ComboBox txtPortName;
        private TextBox txtLog;
        private TextBox txtReceive;
        private TextBox txtWriteValue;
    }
}
