namespace BUR_INS_HMI
{
    partial class Form2
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
            admin_name = new Label();
            login_name = new Label();
            close_btn = new PictureBox();
            password_panel = new Panel();
            numpad_check = new Button();
            textBox1 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)close_btn).BeginInit();
            password_panel.SuspendLayout();
            SuspendLayout();
            // 
            // admin_name
            // 
            admin_name.BackColor = SystemColors.ActiveCaptionText;
            admin_name.Dock = DockStyle.Top;
            admin_name.Font = new Font("맑은 고딕", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            admin_name.ForeColor = SystemColors.Control;
            admin_name.Location = new Point(0, 0);
            admin_name.Name = "admin_name";
            admin_name.Size = new Size(1184, 50);
            admin_name.TabIndex = 0;
            admin_name.Text = "관리자 패스워드";
            admin_name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // login_name
            // 
            login_name.BackColor = SystemColors.ControlDark;
            login_name.BorderStyle = BorderStyle.Fixed3D;
            login_name.Font = new Font("맑은 고딕", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            login_name.ForeColor = SystemColors.ActiveCaptionText;
            login_name.Location = new Point(275, 12);
            login_name.Name = "login_name";
            login_name.Size = new Size(194, 60);
            login_name.TabIndex = 1;
            login_name.Text = "패스워드";
            login_name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // close_btn
            // 
            close_btn.Anchor = AnchorStyles.Right;
            close_btn.Image = Properties.Resources.close_btn;
            close_btn.Location = new Point(988, 139);
            close_btn.Name = "close_btn";
            close_btn.Size = new Size(184, 81);
            close_btn.SizeMode = PictureBoxSizeMode.Zoom;
            close_btn.TabIndex = 3;
            close_btn.TabStop = false;
            close_btn.Click += close_btn_Click;
            // 
            // password_panel
            // 
            password_panel.Controls.Add(textBox1);
            password_panel.Controls.Add(numpad_check);
            password_panel.Controls.Add(login_name);
            password_panel.Dock = DockStyle.Top;
            password_panel.Location = new Point(0, 50);
            password_panel.Name = "password_panel";
            password_panel.Size = new Size(1184, 83);
            password_panel.TabIndex = 4;
            // 
            // numpad_check
            // 
            numpad_check.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad_check.Location = new Point(881, 10);
            numpad_check.Name = "numpad_check";
            numpad_check.Size = new Size(140, 64);
            numpad_check.TabIndex = 11;
            numpad_check.Text = "확인";
            numpad_check.UseVisualStyleBackColor = true;
            numpad_check.Click += numpad_check_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(475, 12);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.PasswordChar = '*';
            textBox1.Size = new Size(400, 62);
            textBox1.TabIndex = 5;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1184, 861);
            Controls.Add(password_panel);
            Controls.Add(close_btn);
            Controls.Add(admin_name);
            Name = "Form2";
            Text = "Admin_Login";
            ((System.ComponentModel.ISupportInitialize)close_btn).EndInit();
            password_panel.ResumeLayout(false);
            password_panel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label admin_name;
        private Label login_name;
        private PictureBox close_btn;
        private Panel password_panel;
        private Button numpad_check;
        private TextBox textBox1;
    }
}