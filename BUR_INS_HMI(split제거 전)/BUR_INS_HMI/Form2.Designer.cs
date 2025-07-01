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
            password_lbl = new Label();
            close_btn = new PictureBox();
            password_panel = new Panel();
            new_pass_lbl = new Label();
            new_pass_name = new Label();
            numpad_panel = new Panel();
            numpad_check = new Button();
            numpad_clear = new Button();
            numpad_delete = new Button();
            numpad0 = new Button();
            numpad9 = new Button();
            numpad8 = new Button();
            numpad7 = new Button();
            numpad6 = new Button();
            numpad5 = new Button();
            numpad4 = new Button();
            numpad3 = new Button();
            numpad2 = new Button();
            numpad1 = new Button();
            ((System.ComponentModel.ISupportInitialize)close_btn).BeginInit();
            password_panel.SuspendLayout();
            numpad_panel.SuspendLayout();
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
            admin_name.Text = "관리자 로그인";
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
            login_name.Text = "관리번호";
            login_name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // password_lbl
            // 
            password_lbl.BackColor = SystemColors.Control;
            password_lbl.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            password_lbl.Location = new Point(475, 12);
            password_lbl.Name = "password_lbl";
            password_lbl.Size = new Size(400, 60);
            password_lbl.TabIndex = 2;
            // 
            // close_btn
            // 
            close_btn.Anchor = AnchorStyles.Right;
            close_btn.Image = Properties.Resources.close_btn;
            close_btn.Location = new Point(1000, 652);
            close_btn.Name = "close_btn";
            close_btn.Size = new Size(184, 81);
            close_btn.SizeMode = PictureBoxSizeMode.Zoom;
            close_btn.TabIndex = 3;
            close_btn.TabStop = false;
            close_btn.Click += close_btn_Click;
            // 
            // password_panel
            // 
            password_panel.Controls.Add(new_pass_lbl);
            password_panel.Controls.Add(new_pass_name);
            password_panel.Controls.Add(login_name);
            password_panel.Controls.Add(password_lbl);
            password_panel.Dock = DockStyle.Top;
            password_panel.Location = new Point(0, 50);
            password_panel.Name = "password_panel";
            password_panel.Size = new Size(1184, 83);
            password_panel.TabIndex = 4;
            // 
            // new_pass_lbl
            // 
            new_pass_lbl.BackColor = SystemColors.Control;
            new_pass_lbl.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            new_pass_lbl.Location = new Point(561, 12);
            new_pass_lbl.Name = "new_pass_lbl";
            new_pass_lbl.Size = new Size(314, 60);
            new_pass_lbl.TabIndex = 13;
            new_pass_lbl.Visible = false;
            // 
            // new_pass_name
            // 
            new_pass_name.BackColor = SystemColors.ControlDark;
            new_pass_name.BorderStyle = BorderStyle.Fixed3D;
            new_pass_name.Font = new Font("맑은 고딕", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            new_pass_name.ForeColor = SystemColors.ActiveCaptionText;
            new_pass_name.Location = new Point(275, 12);
            new_pass_name.Name = "new_pass_name";
            new_pass_name.Size = new Size(280, 60);
            new_pass_name.TabIndex = 14;
            new_pass_name.Text = "새 관리번호";
            new_pass_name.TextAlign = ContentAlignment.MiddleCenter;
            new_pass_name.Visible = false;
            // 
            // numpad_panel
            // 
            numpad_panel.BackColor = SystemColors.ControlDarkDark;
            numpad_panel.Controls.Add(numpad_check);
            numpad_panel.Controls.Add(numpad_clear);
            numpad_panel.Controls.Add(numpad_delete);
            numpad_panel.Controls.Add(numpad0);
            numpad_panel.Controls.Add(numpad9);
            numpad_panel.Controls.Add(numpad8);
            numpad_panel.Controls.Add(numpad7);
            numpad_panel.Controls.Add(numpad6);
            numpad_panel.Controls.Add(numpad5);
            numpad_panel.Controls.Add(numpad4);
            numpad_panel.Controls.Add(numpad3);
            numpad_panel.Controls.Add(numpad2);
            numpad_panel.Controls.Add(numpad1);
            numpad_panel.Location = new Point(275, 200);
            numpad_panel.Name = "numpad_panel";
            numpad_panel.Size = new Size(600, 600);
            numpad_panel.TabIndex = 5;
            // 
            // numpad_check
            // 
            numpad_check.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad_check.Location = new Point(431, 455);
            numpad_check.Name = "numpad_check";
            numpad_check.Size = new Size(140, 70);
            numpad_check.TabIndex = 11;
            numpad_check.Text = "확인";
            numpad_check.UseVisualStyleBackColor = true;
            numpad_check.Click += numpad_check_Click;
            // 
            // numpad_clear
            // 
            numpad_clear.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad_clear.Location = new Point(431, 175);
            numpad_clear.Name = "numpad_clear";
            numpad_clear.Size = new Size(140, 70);
            numpad_clear.TabIndex = 10;
            numpad_clear.Text = "CLR";
            numpad_clear.UseVisualStyleBackColor = true;
            numpad_clear.Click += numpad_clear_Click;
            // 
            // numpad_delete
            // 
            numpad_delete.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad_delete.Location = new Point(431, 35);
            numpad_delete.Name = "numpad_delete";
            numpad_delete.Size = new Size(140, 70);
            numpad_delete.TabIndex = 6;
            numpad_delete.Text = "DEL";
            numpad_delete.UseVisualStyleBackColor = true;
            numpad_delete.Click += numpad_delete_Click;
            // 
            // numpad0
            // 
            numpad0.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad0.Location = new Point(160, 440);
            numpad0.Name = "numpad0";
            numpad0.Size = new Size(100, 100);
            numpad0.TabIndex = 9;
            numpad0.Text = "0";
            numpad0.UseVisualStyleBackColor = true;
            numpad0.Click += numpad0_Click;
            // 
            // numpad9
            // 
            numpad9.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad9.Location = new Point(300, 300);
            numpad9.Name = "numpad9";
            numpad9.Size = new Size(100, 100);
            numpad9.TabIndex = 8;
            numpad9.Text = "9";
            numpad9.UseVisualStyleBackColor = true;
            numpad9.Click += numpad9_Click;
            // 
            // numpad8
            // 
            numpad8.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad8.Location = new Point(160, 300);
            numpad8.Name = "numpad8";
            numpad8.Size = new Size(100, 100);
            numpad8.TabIndex = 7;
            numpad8.Text = "8";
            numpad8.UseVisualStyleBackColor = true;
            numpad8.Click += numpad8_Click;
            // 
            // numpad7
            // 
            numpad7.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad7.Location = new Point(20, 300);
            numpad7.Name = "numpad7";
            numpad7.Size = new Size(100, 100);
            numpad7.TabIndex = 6;
            numpad7.Text = "7";
            numpad7.UseVisualStyleBackColor = true;
            numpad7.Click += numpad7_Click;
            // 
            // numpad6
            // 
            numpad6.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad6.Location = new Point(300, 160);
            numpad6.Name = "numpad6";
            numpad6.Size = new Size(100, 100);
            numpad6.TabIndex = 5;
            numpad6.Text = "6";
            numpad6.UseVisualStyleBackColor = true;
            numpad6.Click += numpad6_Click;
            // 
            // numpad5
            // 
            numpad5.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad5.Location = new Point(160, 160);
            numpad5.Name = "numpad5";
            numpad5.Size = new Size(100, 100);
            numpad5.TabIndex = 4;
            numpad5.Text = "5";
            numpad5.UseVisualStyleBackColor = true;
            numpad5.Click += numpad5_Click;
            // 
            // numpad4
            // 
            numpad4.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad4.Location = new Point(20, 160);
            numpad4.Name = "numpad4";
            numpad4.Size = new Size(100, 100);
            numpad4.TabIndex = 3;
            numpad4.Text = "4";
            numpad4.UseVisualStyleBackColor = true;
            numpad4.Click += numpad4_Click;
            // 
            // numpad3
            // 
            numpad3.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad3.Location = new Point(300, 20);
            numpad3.Name = "numpad3";
            numpad3.Size = new Size(100, 100);
            numpad3.TabIndex = 2;
            numpad3.Text = "3";
            numpad3.UseVisualStyleBackColor = true;
            numpad3.Click += numpad3_Click;
            // 
            // numpad2
            // 
            numpad2.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad2.Location = new Point(160, 20);
            numpad2.Name = "numpad2";
            numpad2.Size = new Size(100, 100);
            numpad2.TabIndex = 1;
            numpad2.Text = "2";
            numpad2.UseVisualStyleBackColor = true;
            numpad2.Click += numpad2_Click;
            // 
            // numpad1
            // 
            numpad1.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad1.Location = new Point(20, 20);
            numpad1.Name = "numpad1";
            numpad1.Size = new Size(100, 100);
            numpad1.TabIndex = 0;
            numpad1.Text = "1";
            numpad1.UseVisualStyleBackColor = true;
            numpad1.Click += numpad1_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1184, 861);
            Controls.Add(numpad_panel);
            Controls.Add(password_panel);
            Controls.Add(close_btn);
            Controls.Add(admin_name);
            Name = "Form2";
            Text = "Admin_Login";
            ((System.ComponentModel.ISupportInitialize)close_btn).EndInit();
            password_panel.ResumeLayout(false);
            numpad_panel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label admin_name;
        private Label login_name;
        private Label password_lbl;
        private PictureBox close_btn;
        private Panel password_panel;
        private Panel numpad_panel;
        private Button numpad1;
        private Button numpad0;
        private Button numpad9;
        private Button numpad8;
        private Button numpad7;
        private Button numpad6;
        private Button numpad5;
        private Button numpad4;
        private Button numpad3;
        private Button numpad2;
        private Button numpad_clear;
        private Button numpad_delete;
        private Button numpad_check;
        private Label new_pass_lbl;
        private Label new_pass_name;
    }
}