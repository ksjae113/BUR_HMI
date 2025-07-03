namespace BUR_INS_HMI
{
    partial class Form4
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
            unit_lbl = new Label();
            editer_btn_close = new Button();
            editer_btn_check = new Button();
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
            ((System.ComponentModel.ISupportInitialize)save_btn).BeginInit();
            SuspendLayout();
            // 
            // unit_lbl
            // 
            unit_lbl.AutoSize = true;
            unit_lbl.BackColor = Color.Yellow;
            unit_lbl.Font = new Font("맑은 고딕", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            unit_lbl.Location = new Point(931, 0);
            unit_lbl.Name = "unit_lbl";
            unit_lbl.Size = new Size(64, 40);
            unit_lbl.TabIndex = 1;
            unit_lbl.Text = "mA";
            unit_lbl.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // editer_btn_close
            // 
            editer_btn_close.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            editer_btn_close.Location = new Point(863, 625);
            editer_btn_close.Name = "editer_btn_close";
            editer_btn_close.Size = new Size(120, 60);
            editer_btn_close.TabIndex = 16;
            editer_btn_close.Text = "닫기";
            editer_btn_close.UseVisualStyleBackColor = true;
            editer_btn_close.Click += editer_btn_close_Click;
            // 
            // editer_btn_check
            // 
            editer_btn_check.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            editer_btn_check.Location = new Point(863, 552);
            editer_btn_check.Name = "editer_btn_check";
            editer_btn_check.Size = new Size(120, 60);
            editer_btn_check.TabIndex = 17;
            editer_btn_check.Text = "확인";
            editer_btn_check.UseVisualStyleBackColor = true;
            editer_btn_check.Click += editer_btn_check_Click;
            // 
            // tar_temp
            // 
            tar_temp.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            tar_temp.Location = new Point(706, 70);
            tar_temp.Multiline = true;
            tar_temp.Name = "tar_temp";
            tar_temp.Size = new Size(100, 30);
            tar_temp.TabIndex = 113;
            // 
            // label13
            // 
            label13.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label13.ForeColor = SystemColors.Control;
            label13.Location = new Point(706, 9);
            label13.Name = "label13";
            label13.Size = new Size(105, 60);
            label13.TabIndex = 112;
            label13.Text = "기준 온도\r\n(ºC)";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pos2val
            // 
            pos2val.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            pos2val.Location = new Point(454, 152);
            pos2val.Multiline = true;
            pos2val.Name = "pos2val";
            pos2val.Size = new Size(100, 30);
            pos2val.TabIndex = 111;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label12.ForeColor = SystemColors.Control;
            label12.Location = new Point(305, 150);
            label12.Name = "label12";
            label12.Size = new Size(130, 30);
            label12.TabIndex = 110;
            label12.Text = "RPM 교정값";
            // 
            // textBox12
            // 
            textBox12.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            textBox12.Location = new Point(454, 87);
            textBox12.Multiline = true;
            textBox12.Name = "textBox12";
            textBox12.Size = new Size(100, 30);
            textBox12.TabIndex = 109;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label11.ForeColor = SystemColors.Control;
            label11.Location = new Point(305, 87);
            label11.Name = "label11";
            label11.Size = new Size(133, 30);
            label11.TabIndex = 108;
            label11.Text = "ERR_RANGE";
            // 
            // save_btn
            // 
            save_btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            save_btn.Image = Properties.Resources.save1;
            save_btn.Location = new Point(1068, 675);
            save_btn.Name = "save_btn";
            save_btn.Size = new Size(100, 50);
            save_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            save_btn.TabIndex = 107;
            save_btn.TabStop = false;
            // 
            // target_amp_lbl
            // 
            target_amp_lbl.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            target_amp_lbl.ForeColor = SystemColors.Control;
            target_amp_lbl.Location = new Point(106, 9);
            target_amp_lbl.Name = "target_amp_lbl";
            target_amp_lbl.Size = new Size(105, 60);
            target_amp_lbl.TabIndex = 106;
            target_amp_lbl.Text = "기준 전류\r\n(mA)";
            target_amp_lbl.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // textBox10
            // 
            textBox10.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox10.Location = new Point(111, 618);
            textBox10.Multiline = true;
            textBox10.Name = "textBox10";
            textBox10.Size = new Size(100, 30);
            textBox10.TabIndex = 105;
            // 
            // textBox9
            // 
            textBox9.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox9.Location = new Point(111, 554);
            textBox9.Multiline = true;
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(100, 30);
            textBox9.TabIndex = 104;
            // 
            // textBox8
            // 
            textBox8.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox8.Location = new Point(111, 480);
            textBox8.Multiline = true;
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(100, 30);
            textBox8.TabIndex = 103;
            // 
            // textBox7
            // 
            textBox7.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox7.Location = new Point(111, 410);
            textBox7.Multiline = true;
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(100, 30);
            textBox7.TabIndex = 102;
            // 
            // textBox6
            // 
            textBox6.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox6.Location = new Point(111, 346);
            textBox6.Multiline = true;
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(100, 30);
            textBox6.TabIndex = 101;
            // 
            // textBox5
            // 
            textBox5.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox5.Location = new Point(111, 293);
            textBox5.Multiline = true;
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(100, 30);
            textBox5.TabIndex = 100;
            // 
            // textBox4
            // 
            textBox4.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox4.Location = new Point(111, 239);
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(100, 30);
            textBox4.TabIndex = 99;
            // 
            // textBox3
            // 
            textBox3.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox3.Location = new Point(111, 184);
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(100, 30);
            textBox3.TabIndex = 98;
            // 
            // textBox2
            // 
            textBox2.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox2.Location = new Point(111, 125);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 30);
            textBox2.TabIndex = 97;
            // 
            // textBox11
            // 
            textBox11.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold);
            textBox11.Location = new Point(111, 70);
            textBox11.Multiline = true;
            textBox11.Name = "textBox11";
            textBox11.Size = new Size(100, 30);
            textBox11.TabIndex = 96;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label10.ForeColor = SystemColors.Control;
            label10.Location = new Point(8, 618);
            label10.Name = "label10";
            label10.Size = new Size(86, 30);
            label10.TabIndex = 95;
            label10.Text = "Line 10";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label9.ForeColor = SystemColors.Control;
            label9.Location = new Point(8, 554);
            label9.Name = "label9";
            label9.Size = new Size(86, 30);
            label9.TabIndex = 94;
            label9.Text = "Line 09";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label8.ForeColor = SystemColors.Control;
            label8.Location = new Point(8, 480);
            label8.Name = "label8";
            label8.Size = new Size(86, 30);
            label8.TabIndex = 93;
            label8.Text = "Line 08";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label7.ForeColor = SystemColors.Control;
            label7.Location = new Point(8, 410);
            label7.Name = "label7";
            label7.Size = new Size(86, 30);
            label7.TabIndex = 92;
            label7.Text = "Line 07";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label6.ForeColor = SystemColors.Control;
            label6.Location = new Point(8, 346);
            label6.Name = "label6";
            label6.Size = new Size(86, 30);
            label6.TabIndex = 91;
            label6.Text = "Line 06";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label5.ForeColor = SystemColors.Control;
            label5.Location = new Point(8, 293);
            label5.Name = "label5";
            label5.Size = new Size(86, 30);
            label5.TabIndex = 90;
            label5.Text = "Line 05";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label4.ForeColor = SystemColors.Control;
            label4.Location = new Point(8, 239);
            label4.Name = "label4";
            label4.Size = new Size(86, 30);
            label4.TabIndex = 89;
            label4.Text = "Line 04";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label3.ForeColor = SystemColors.Control;
            label3.Location = new Point(8, 184);
            label3.Name = "label3";
            label3.Size = new Size(86, 30);
            label3.TabIndex = 88;
            label3.Text = "Line 03";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label2.ForeColor = SystemColors.Control;
            label2.Location = new Point(8, 125);
            label2.Name = "label2";
            label2.Size = new Size(86, 30);
            label2.TabIndex = 87;
            label2.Text = "Line 02";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(8, 70);
            label1.Name = "label1";
            label1.Size = new Size(86, 30);
            label1.TabIndex = 86;
            label1.Text = "Line 01";
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(995, 697);
            Controls.Add(tar_temp);
            Controls.Add(label13);
            Controls.Add(pos2val);
            Controls.Add(label12);
            Controls.Add(textBox12);
            Controls.Add(label11);
            Controls.Add(save_btn);
            Controls.Add(target_amp_lbl);
            Controls.Add(textBox10);
            Controls.Add(textBox9);
            Controls.Add(textBox8);
            Controls.Add(textBox7);
            Controls.Add(textBox6);
            Controls.Add(textBox5);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox11);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(editer_btn_check);
            Controls.Add(editer_btn_close);
            Controls.Add(unit_lbl);
            Name = "Form4";
            Text = "Editer";
            ((System.ComponentModel.ISupportInitialize)save_btn).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button editer_btn_close;
        private Button editer_btn_check;
        internal Label unit_lbl;
        private TextBox tar_temp;
        private Label label13;
        private TextBox pos2val;
        private Label label12;
        private TextBox textBox12;
        private Label label11;
        private PictureBox save_btn;
        private Label target_amp_lbl;
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
    }
}