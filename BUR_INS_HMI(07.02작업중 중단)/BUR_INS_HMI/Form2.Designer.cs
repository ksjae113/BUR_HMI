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
            textBox1 = new TextBox();
            numpad_check = new Button();
            edit_panel = new Panel();
            tar_temp = new TextBox();
            label13 = new Label();
            pos2val = new TextBox();
            label12 = new Label();
            textBox12 = new TextBox();
            label11 = new Label();
            save_btn = new PictureBox();
            target_amp_lbl = new Label();
            textBox10 = new TextBox();
            textBox9 = new TextBox();
            textBox8 = new TextBox();
            textBox7 = new TextBox();
            textBox6 = new TextBox();
            textBox5 = new TextBox();
            textBox4 = new TextBox();
            textBox3 = new TextBox();
            textBox2 = new TextBox();
            textBox11 = new TextBox();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)close_btn).BeginInit();
            password_panel.SuspendLayout();
            edit_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)save_btn).BeginInit();
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
            login_name.Location = new Point(201, 12);
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
            close_btn.Location = new Point(1041, 10);
            close_btn.Name = "close_btn";
            close_btn.Size = new Size(140, 64);
            close_btn.SizeMode = PictureBoxSizeMode.Zoom;
            close_btn.TabIndex = 3;
            close_btn.TabStop = false;
            close_btn.Click += close_btn_Click;
            // 
            // password_panel
            // 
            password_panel.Controls.Add(textBox1);
            password_panel.Controls.Add(close_btn);
            password_panel.Controls.Add(numpad_check);
            password_panel.Controls.Add(login_name);
            password_panel.Dock = DockStyle.Top;
            password_panel.Location = new Point(0, 50);
            password_panel.Name = "password_panel";
            password_panel.Size = new Size(1184, 83);
            password_panel.TabIndex = 4;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("맑은 고딕", 36F, FontStyle.Regular, GraphicsUnit.Point, 129);
            textBox1.Location = new Point(401, 12);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.PasswordChar = '*';
            textBox1.Size = new Size(400, 62);
            textBox1.TabIndex = 0;
            textBox1.KeyDown += textBox1_KeyDown;
            // 
            // numpad_check
            // 
            numpad_check.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            numpad_check.Location = new Point(807, 10);
            numpad_check.Name = "numpad_check";
            numpad_check.Size = new Size(140, 64);
            numpad_check.TabIndex = 1;
            numpad_check.Text = "확인";
            numpad_check.UseVisualStyleBackColor = true;
            numpad_check.Click += numpad_check_Click;
            // 
            // edit_panel
            // 
            edit_panel.Controls.Add(tar_temp);
            edit_panel.Controls.Add(label13);
            edit_panel.Controls.Add(pos2val);
            edit_panel.Controls.Add(label12);
            edit_panel.Controls.Add(textBox12);
            edit_panel.Controls.Add(label11);
            edit_panel.Controls.Add(save_btn);
            edit_panel.Controls.Add(target_amp_lbl);
            edit_panel.Controls.Add(textBox10);
            edit_panel.Controls.Add(textBox9);
            edit_panel.Controls.Add(textBox8);
            edit_panel.Controls.Add(textBox7);
            edit_panel.Controls.Add(textBox6);
            edit_panel.Controls.Add(textBox5);
            edit_panel.Controls.Add(textBox4);
            edit_panel.Controls.Add(textBox3);
            edit_panel.Controls.Add(textBox2);
            edit_panel.Controls.Add(textBox11);
            edit_panel.Controls.Add(label10);
            edit_panel.Controls.Add(label9);
            edit_panel.Controls.Add(label8);
            edit_panel.Controls.Add(label7);
            edit_panel.Controls.Add(label6);
            edit_panel.Controls.Add(label5);
            edit_panel.Controls.Add(label4);
            edit_panel.Controls.Add(label3);
            edit_panel.Controls.Add(label2);
            edit_panel.Controls.Add(label1);
            edit_panel.Dock = DockStyle.Fill;
            edit_panel.Location = new Point(0, 133);
            edit_panel.Name = "edit_panel";
            edit_panel.Size = new Size(1184, 728);
            edit_panel.TabIndex = 5;
            edit_panel.Visible = false;
            // 
            // tar_temp
            // 
            tar_temp.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            tar_temp.Location = new Point(710, 61);
            tar_temp.Multiline = true;
            tar_temp.Name = "tar_temp";
            tar_temp.Size = new Size(100, 30);
            tar_temp.TabIndex = 85;
            // 
            // label13
            // 
            label13.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label13.ForeColor = SystemColors.Control;
            label13.Location = new Point(710, 0);
            label13.Name = "label13";
            label13.Size = new Size(105, 60);
            label13.TabIndex = 84;
            label13.Text = "기준 온도\r\n(ºC)";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pos2val
            // 
            pos2val.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            pos2val.Location = new Point(458, 143);
            pos2val.Multiline = true;
            pos2val.Name = "pos2val";
            pos2val.Size = new Size(100, 30);
            pos2val.TabIndex = 83;
            pos2val.TextChanged += pos2val_TextChanged;
            pos2val.KeyDown += AmpTextBox_KeyDown;
            pos2val.Leave += AmpTextBox_Leave;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label12.ForeColor = SystemColors.Control;
            label12.Location = new Point(309, 141);
            label12.Name = "label12";
            label12.Size = new Size(130, 30);
            label12.TabIndex = 82;
            label12.Text = "RPM 교정값";
            // 
            // textBox12
            // 
            textBox12.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            textBox12.Location = new Point(458, 78);
            textBox12.Multiline = true;
            textBox12.Name = "textBox12";
            textBox12.Size = new Size(100, 30);
            textBox12.TabIndex = 81;
            textBox12.KeyDown += AmpTextBox_KeyDown;
            textBox12.Leave += AmpTextBox_Leave;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label11.ForeColor = SystemColors.Control;
            label11.Location = new Point(309, 78);
            label11.Name = "label11";
            label11.Size = new Size(133, 30);
            label11.TabIndex = 80;
            label11.Text = "ERR_RANGE";
            // 
            // save_btn
            // 
            save_btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            save_btn.Image = Properties.Resources.save1;
            save_btn.Location = new Point(1072, 666);
            save_btn.Name = "save_btn";
            save_btn.Size = new Size(100, 50);
            save_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            save_btn.TabIndex = 79;
            save_btn.TabStop = false;
            save_btn.Click += save_btn_Click;
            // 
            // target_amp_lbl
            // 
            target_amp_lbl.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            target_amp_lbl.ForeColor = SystemColors.Control;
            target_amp_lbl.Location = new Point(110, 0);
            target_amp_lbl.Name = "target_amp_lbl";
            target_amp_lbl.Size = new Size(105, 60);
            target_amp_lbl.TabIndex = 78;
            target_amp_lbl.Text = "기준 전류\r\n(mA)";
            target_amp_lbl.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // textBox10
            // 
            textBox10.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox10.Location = new Point(115, 609);
            textBox10.Multiline = true;
            textBox10.Name = "textBox10";
            textBox10.Size = new Size(100, 30);
            textBox10.TabIndex = 77;
            textBox10.KeyDown += AmpTextBox_KeyDown;
            textBox10.Leave += AmpTextBox_Leave;
            // 
            // textBox9
            // 
            textBox9.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox9.Location = new Point(115, 545);
            textBox9.Multiline = true;
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(100, 30);
            textBox9.TabIndex = 76;
            textBox9.KeyDown += AmpTextBox_KeyDown;
            textBox9.Leave += AmpTextBox_Leave;
            // 
            // textBox8
            // 
            textBox8.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox8.Location = new Point(115, 471);
            textBox8.Multiline = true;
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(100, 30);
            textBox8.TabIndex = 75;
            textBox8.KeyDown += AmpTextBox_KeyDown;
            textBox8.Leave += AmpTextBox_Leave;
            // 
            // textBox7
            // 
            textBox7.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox7.Location = new Point(115, 401);
            textBox7.Multiline = true;
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(100, 30);
            textBox7.TabIndex = 74;
            textBox7.KeyDown += AmpTextBox_KeyDown;
            textBox7.Leave += AmpTextBox_Leave;
            // 
            // textBox6
            // 
            textBox6.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox6.Location = new Point(115, 337);
            textBox6.Multiline = true;
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(100, 30);
            textBox6.TabIndex = 73;
            textBox6.KeyDown += AmpTextBox_KeyDown;
            textBox6.Leave += AmpTextBox_Leave;
            // 
            // textBox5
            // 
            textBox5.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox5.Location = new Point(115, 284);
            textBox5.Multiline = true;
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(100, 30);
            textBox5.TabIndex = 72;
            textBox5.KeyDown += AmpTextBox_KeyDown;
            textBox5.Leave += AmpTextBox_Leave;
            // 
            // textBox4
            // 
            textBox4.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox4.Location = new Point(115, 230);
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(100, 30);
            textBox4.TabIndex = 71;
            textBox4.KeyDown += AmpTextBox_KeyDown;
            textBox4.Leave += AmpTextBox_Leave;
            // 
            // textBox3
            // 
            textBox3.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox3.Location = new Point(115, 175);
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(100, 30);
            textBox3.TabIndex = 70;
            textBox3.KeyDown += AmpTextBox_KeyDown;
            textBox3.Leave += AmpTextBox_Leave;
            // 
            // textBox2
            // 
            textBox2.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox2.Location = new Point(115, 116);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 30);
            textBox2.TabIndex = 69;
            textBox2.KeyDown += AmpTextBox_KeyDown;
            textBox2.Leave += AmpTextBox_Leave;
            // 
            // textBox11
            // 
            textBox11.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox11.Location = new Point(115, 61);
            textBox11.Multiline = true;
            textBox11.Name = "textBox11";
            textBox11.Size = new Size(100, 30);
            textBox11.TabIndex = 68;
            textBox11.KeyDown += AmpTextBox_KeyDown;
            textBox11.Leave += AmpTextBox_Leave;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label10.ForeColor = SystemColors.Control;
            label10.Location = new Point(12, 609);
            label10.Name = "label10";
            label10.Size = new Size(86, 30);
            label10.TabIndex = 67;
            label10.Text = "Line 10";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label9.ForeColor = SystemColors.Control;
            label9.Location = new Point(12, 545);
            label9.Name = "label9";
            label9.Size = new Size(86, 30);
            label9.TabIndex = 66;
            label9.Text = "Line 09";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label8.ForeColor = SystemColors.Control;
            label8.Location = new Point(12, 471);
            label8.Name = "label8";
            label8.Size = new Size(86, 30);
            label8.TabIndex = 65;
            label8.Text = "Line 08";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label7.ForeColor = SystemColors.Control;
            label7.Location = new Point(12, 401);
            label7.Name = "label7";
            label7.Size = new Size(86, 30);
            label7.TabIndex = 64;
            label7.Text = "Line 07";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label6.ForeColor = SystemColors.Control;
            label6.Location = new Point(12, 337);
            label6.Name = "label6";
            label6.Size = new Size(86, 30);
            label6.TabIndex = 63;
            label6.Text = "Line 06";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label5.ForeColor = SystemColors.Control;
            label5.Location = new Point(12, 284);
            label5.Name = "label5";
            label5.Size = new Size(86, 30);
            label5.TabIndex = 62;
            label5.Text = "Line 05";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label4.ForeColor = SystemColors.Control;
            label4.Location = new Point(12, 230);
            label4.Name = "label4";
            label4.Size = new Size(86, 30);
            label4.TabIndex = 61;
            label4.Text = "Line 04";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label3.ForeColor = SystemColors.Control;
            label3.Location = new Point(12, 175);
            label3.Name = "label3";
            label3.Size = new Size(86, 30);
            label3.TabIndex = 60;
            label3.Text = "Line 03";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label2.ForeColor = SystemColors.Control;
            label2.Location = new Point(12, 116);
            label2.Name = "label2";
            label2.Size = new Size(86, 30);
            label2.TabIndex = 59;
            label2.Text = "Line 02";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(12, 61);
            label1.Name = "label1";
            label1.Size = new Size(86, 30);
            label1.TabIndex = 58;
            label1.Text = "Line 01";
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1184, 861);
            Controls.Add(edit_panel);
            Controls.Add(password_panel);
            Controls.Add(admin_name);
            Name = "Form2";
            Text = "Admin_Login";
            ((System.ComponentModel.ISupportInitialize)close_btn).EndInit();
            password_panel.ResumeLayout(false);
            password_panel.PerformLayout();
            edit_panel.ResumeLayout(false);
            edit_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)save_btn).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label admin_name;
        private Label login_name;
        private PictureBox close_btn;
        private Panel password_panel;
        private Button numpad_check;
        private TextBox textBox1;
        private TextBox textBox10;
        private TextBox textBox9;
        private TextBox textBox8;
        private TextBox textBox7;
        private TextBox textBox6;
        private TextBox textBox5;
        private TextBox textBox4;
        private TextBox textBox3;
        private TextBox textBox2;
        private TextBox textBox11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label target_amp_lbl;
        private PictureBox save_btn;
        private TextBox textBox12;
        private Label label11;
        internal Panel edit_panel;
        private TextBox pos2val;
        private Label label12;
        private TextBox tar_temp;
        private Label label13;
    }
}