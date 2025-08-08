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
            password_panel = new Panel();
            new_pass = new Label();
            now_pass = new Label();
            textBox1 = new TextBox();
            change_btn = new Button();
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
            // password_panel
            // 
            password_panel.Controls.Add(new_pass);
            password_panel.Controls.Add(now_pass);
            password_panel.Controls.Add(textBox1);
            password_panel.Controls.Add(login_name);
            password_panel.Location = new Point(0, 111);
            password_panel.Name = "password_panel";
            password_panel.Size = new Size(1184, 83);
            password_panel.TabIndex = 4;
            // 
            // new_pass
            // 
            new_pass.BackColor = SystemColors.ControlDark;
            new_pass.BorderStyle = BorderStyle.Fixed3D;
            new_pass.Font = new Font("맑은 고딕", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            new_pass.ForeColor = SystemColors.ActiveCaptionText;
            new_pass.Location = new Point(179, 12);
            new_pass.Name = "new_pass";
            new_pass.Size = new Size(290, 60);
            new_pass.TabIndex = 7;
            new_pass.Text = "변경할 패스워드";
            new_pass.TextAlign = ContentAlignment.MiddleCenter;
            new_pass.Visible = false;
            // 
            // now_pass
            // 
            now_pass.BackColor = SystemColors.ControlDark;
            now_pass.BorderStyle = BorderStyle.Fixed3D;
            now_pass.Font = new Font("맑은 고딕", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            now_pass.ForeColor = SystemColors.ActiveCaptionText;
            now_pass.Location = new Point(179, 12);
            now_pass.Name = "now_pass";
            now_pass.Size = new Size(290, 60);
            now_pass.TabIndex = 6;
            now_pass.Text = "현재 패스워드";
            now_pass.TextAlign = ContentAlignment.MiddleCenter;
            now_pass.Visible = false;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("맑은 고딕", 24F, FontStyle.Bold, GraphicsUnit.Point, 129);
            textBox1.Location = new Point(475, 12);
            textBox1.MaxLength = 8;
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.PasswordChar = '*';
            textBox1.Size = new Size(400, 60);
            textBox1.TabIndex = 6;
            textBox1.KeyDown += textBox1_KeyDown;
            // 
            // change_btn
            // 
            change_btn.AutoSize = true;
            change_btn.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            change_btn.Location = new Point(1065, 12);
            change_btn.Name = "change_btn";
            change_btn.Size = new Size(107, 70);
            change_btn.TabIndex = 5;
            change_btn.Text = "패스워드\r\n변경";
            change_btn.UseVisualStyleBackColor = true;
            change_btn.Click += change_btn_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1184, 316);
            Controls.Add(change_btn);
            Controls.Add(password_panel);
            Controls.Add(admin_name);
            Name = "Form2";
            Text = "Admin_Login";
            password_panel.ResumeLayout(false);
            password_panel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label admin_name;
        private Label login_name;
        private Panel password_panel;
        private TextBox textBox1;
        private Button change_btn;
        private Label now_pass;
        private Label new_pass;
    }
}