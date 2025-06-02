namespace modbus_gpt
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
            btnToggleDO_Click = new Button();
            btnReadDI = new Button();
            chkOutput = new CheckBox();
            lblDI1 = new Label();
            lblDI2 = new Label();
            btnTurnOn = new Button();
            btnTurnOff = new Button();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // btnToggleDO_Click
            // 
            btnToggleDO_Click.Location = new Point(643, 12);
            btnToggleDO_Click.Name = "btnToggleDO_Click";
            btnToggleDO_Click.Size = new Size(145, 78);
            btnToggleDO_Click.TabIndex = 0;
            btnToggleDO_Click.Text = "do제어";
            btnToggleDO_Click.UseVisualStyleBackColor = true;
            btnToggleDO_Click.Click += btnToggleDO_Click_Click;
            // 
            // btnReadDI
            // 
            btnReadDI.Location = new Point(643, 222);
            btnReadDI.Name = "btnReadDI";
            btnReadDI.Size = new Size(145, 78);
            btnReadDI.TabIndex = 1;
            btnReadDI.Text = "di읽기";
            btnReadDI.UseVisualStyleBackColor = true;
            btnReadDI.Click += btnReadDI_Click;
            // 
            // chkOutput
            // 
            chkOutput.AutoSize = true;
            chkOutput.Location = new Point(18, 11);
            chkOutput.Name = "chkOutput";
            chkOutput.Size = new Size(120, 19);
            chkOutput.TabIndex = 2;
            chkOutput.Text = "DO ON/OFF 상태";
            chkOutput.UseVisualStyleBackColor = true;
            // 
            // lblDI1
            // 
            lblDI1.BorderStyle = BorderStyle.FixedSingle;
            lblDI1.Location = new Point(12, 54);
            lblDI1.Name = "lblDI1";
            lblDI1.Size = new Size(585, 172);
            lblDI1.TabIndex = 3;
            lblDI1.Text = "\r\n";
            // 
            // lblDI2
            // 
            lblDI2.BorderStyle = BorderStyle.FixedSingle;
            lblDI2.Location = new Point(12, 248);
            lblDI2.Name = "lblDI2";
            lblDI2.Size = new Size(585, 193);
            lblDI2.TabIndex = 4;
            lblDI2.Text = "\r\n";
            // 
            // btnTurnOn
            // 
            btnTurnOn.Location = new Point(436, 8);
            btnTurnOn.Name = "btnTurnOn";
            btnTurnOn.Size = new Size(68, 22);
            btnTurnOn.TabIndex = 5;
            btnTurnOn.Text = "Turn ON";
            btnTurnOn.UseVisualStyleBackColor = true;
            btnTurnOn.Click += btnTurnOn_Click;
            // 
            // btnTurnOff
            // 
            btnTurnOff.Location = new Point(529, 8);
            btnTurnOff.Name = "btnTurnOff";
            btnTurnOff.Size = new Size(68, 22);
            btnTurnOff.TabIndex = 6;
            btnTurnOff.Text = "Turn OFF";
            btnTurnOff.UseVisualStyleBackColor = true;
            btnTurnOff.Click += btnTurnOff_Click;
            // 
            // lblStatus
            // 
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Location = new Point(132, 8);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(298, 35);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "\r\n";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblStatus);
            Controls.Add(btnTurnOff);
            Controls.Add(btnTurnOn);
            Controls.Add(lblDI2);
            Controls.Add(lblDI1);
            Controls.Add(chkOutput);
            Controls.Add(btnReadDI);
            Controls.Add(btnToggleDO_Click);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnToggleDO_Click;
        private Button btnReadDI;
        private CheckBox chkOutput;
        private Label lblDI1;
        private Label lblDI2;
        private Button btnTurnOn;
        private Button btnTurnOff;
        private Label lblStatus;
    }
}
