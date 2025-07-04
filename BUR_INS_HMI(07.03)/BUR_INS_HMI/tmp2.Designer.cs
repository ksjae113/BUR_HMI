namespace BUR_INS_HMI
{
    partial class tmp2
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
            roll_panel = new Panel();
            ampare_btn = new PictureBox();
            numericUpDown1 = new NumericUpDown();
            pic_position3 = new PictureBox();
            pic_position2 = new PictureBox();
            pic_position1 = new PictureBox();
            position3_lbl = new Label();
            position2_lbl = new Label();
            position1_lbl = new Label();
            runstop_cap_panel = new Panel();
            screencap_btn = new PictureBox();
            run_btn = new PictureBox();
            runstop_btn = new PictureBox();
            temp_btn = new PictureBox();
            record_btn = new PictureBox();
            info_panel = new Panel();
            roll_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ampare_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pic_position3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pic_position2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pic_position1).BeginInit();
            runstop_cap_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)screencap_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)run_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)runstop_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)temp_btn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)record_btn).BeginInit();
            SuspendLayout();
            // 
            // roll_panel
            // 
            roll_panel.BackColor = SystemColors.ActiveCaptionText;
            roll_panel.Controls.Add(ampare_btn);
            roll_panel.Controls.Add(numericUpDown1);
            roll_panel.Controls.Add(pic_position3);
            roll_panel.Controls.Add(pic_position2);
            roll_panel.Controls.Add(pic_position1);
            roll_panel.Controls.Add(position3_lbl);
            roll_panel.Controls.Add(position2_lbl);
            roll_panel.Controls.Add(position1_lbl);
            roll_panel.Controls.Add(runstop_cap_panel);
            roll_panel.Controls.Add(temp_btn);
            roll_panel.Controls.Add(record_btn);
            roll_panel.Dock = DockStyle.Top;
            roll_panel.Location = new Point(0, 0);
            roll_panel.Name = "roll_panel";
            roll_panel.Size = new Size(1389, 250);
            roll_panel.TabIndex = 1;
            // 
            // ampare_btn
            // 
            ampare_btn.Image = Properties.Resources.amp_rec_btn;
            ampare_btn.Location = new Point(243, 73);
            ampare_btn.Name = "ampare_btn";
            ampare_btn.Size = new Size(100, 50);
            ampare_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            ampare_btn.TabIndex = 4;
            ampare_btn.TabStop = false;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDown1.Location = new Point(106, 145);
            numericUpDown1.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(59, 23);
            numericUpDown1.TabIndex = 35;
            numericUpDown1.TextAlign = HorizontalAlignment.Center;
            // 
            // pic_position3
            // 
            pic_position3.Image = Properties.Resources.sen_stat_none;
            pic_position3.Location = new Point(174, 196);
            pic_position3.Name = "pic_position3";
            pic_position3.Size = new Size(40, 40);
            pic_position3.SizeMode = PictureBoxSizeMode.StretchImage;
            pic_position3.TabIndex = 34;
            pic_position3.TabStop = false;
            // 
            // pic_position2
            // 
            pic_position2.Image = Properties.Resources.sen_stat_none;
            pic_position2.Location = new Point(174, 141);
            pic_position2.Name = "pic_position2";
            pic_position2.Size = new Size(40, 40);
            pic_position2.SizeMode = PictureBoxSizeMode.StretchImage;
            pic_position2.TabIndex = 33;
            pic_position2.TabStop = false;
            // 
            // pic_position1
            // 
            pic_position1.Image = Properties.Resources.sen_stat_none;
            pic_position1.Location = new Point(174, 83);
            pic_position1.Name = "pic_position1";
            pic_position1.Size = new Size(40, 40);
            pic_position1.SizeMode = PictureBoxSizeMode.StretchImage;
            pic_position1.TabIndex = 32;
            pic_position1.TabStop = false;
            // 
            // position3_lbl
            // 
            position3_lbl.AutoSize = true;
            position3_lbl.Font = new Font("맑은 고딕", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            position3_lbl.ForeColor = SystemColors.Control;
            position3_lbl.Location = new Point(-3, 196);
            position3_lbl.Name = "position3_lbl";
            position3_lbl.Size = new Size(134, 37);
            position3_lbl.TabIndex = 29;
            position3_lbl.Text = "교정 종료";
            // 
            // position2_lbl
            // 
            position2_lbl.AutoSize = true;
            position2_lbl.Font = new Font("맑은 고딕", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            position2_lbl.ForeColor = SystemColors.Control;
            position2_lbl.Location = new Point(-3, 141);
            position2_lbl.Name = "position2_lbl";
            position2_lbl.Size = new Size(107, 37);
            position2_lbl.TabIndex = 28;
            position2_lbl.Text = "교정 중";
            // 
            // position1_lbl
            // 
            position1_lbl.AutoSize = true;
            position1_lbl.Font = new Font("맑은 고딕", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            position1_lbl.ForeColor = SystemColors.Control;
            position1_lbl.Location = new Point(-3, 83);
            position1_lbl.Name = "position1_lbl";
            position1_lbl.Size = new Size(188, 37);
            position1_lbl.TabIndex = 27;
            position1_lbl.Text = "평탄도계 통과";
            // 
            // runstop_cap_panel
            // 
            runstop_cap_panel.Controls.Add(screencap_btn);
            runstop_cap_panel.Controls.Add(run_btn);
            runstop_cap_panel.Controls.Add(runstop_btn);
            runstop_cap_panel.Dock = DockStyle.Top;
            runstop_cap_panel.Location = new Point(0, 0);
            runstop_cap_panel.Name = "runstop_cap_panel";
            runstop_cap_panel.Size = new Size(1389, 57);
            runstop_cap_panel.TabIndex = 26;
            // 
            // screencap_btn
            // 
            screencap_btn.Image = Properties.Resources.screencapture_btn;
            screencap_btn.Location = new Point(281, 3);
            screencap_btn.Name = "screencap_btn";
            screencap_btn.Size = new Size(100, 50);
            screencap_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            screencap_btn.TabIndex = 3;
            screencap_btn.TabStop = false;
            // 
            // run_btn
            // 
            run_btn.Image = Properties.Resources.run_btn;
            run_btn.Location = new Point(4, 3);
            run_btn.Name = "run_btn";
            run_btn.Size = new Size(100, 50);
            run_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            run_btn.TabIndex = 2;
            run_btn.TabStop = false;
            // 
            // runstop_btn
            // 
            runstop_btn.Image = Properties.Resources.runstop_btn;
            runstop_btn.Location = new Point(142, 3);
            runstop_btn.Name = "runstop_btn";
            runstop_btn.Size = new Size(100, 50);
            runstop_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            runstop_btn.TabIndex = 1;
            runstop_btn.TabStop = false;
            // 
            // temp_btn
            // 
            temp_btn.Image = Properties.Resources.sensor_btn_black;
            temp_btn.Location = new Point(243, 129);
            temp_btn.Name = "temp_btn";
            temp_btn.Size = new Size(145, 60);
            temp_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            temp_btn.TabIndex = 2;
            temp_btn.TabStop = false;
            // 
            // record_btn
            // 
            record_btn.Image = Properties.Resources.record_btn_black;
            record_btn.Location = new Point(243, 188);
            record_btn.Name = "record_btn";
            record_btn.Size = new Size(145, 60);
            record_btn.SizeMode = PictureBoxSizeMode.StretchImage;
            record_btn.TabIndex = 5;
            record_btn.TabStop = false;
            // 
            // info_panel
            // 
            info_panel.BackColor = SystemColors.ActiveCaptionText;
            info_panel.Dock = DockStyle.Fill;
            info_panel.Location = new Point(0, 250);
            info_panel.Name = "info_panel";
            info_panel.Size = new Size(1389, 553);
            info_panel.TabIndex = 2;
            // 
            // tmp2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1389, 803);
            Controls.Add(info_panel);
            Controls.Add(roll_panel);
            Name = "tmp2";
            Text = "tmp2";
            roll_panel.ResumeLayout(false);
            roll_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ampare_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pic_position3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pic_position2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pic_position1).EndInit();
            runstop_cap_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)screencap_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)run_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)runstop_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)temp_btn).EndInit();
            ((System.ComponentModel.ISupportInitialize)record_btn).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel roll_panel;
        private PictureBox ampare_btn;
        private NumericUpDown numericUpDown1;
        private PictureBox pic_position3;
        private PictureBox pic_position2;
        private PictureBox pic_position1;
        private Label position3_lbl;
        private Label position2_lbl;
        private Label position1_lbl;
        private Panel runstop_cap_panel;
        private PictureBox screencap_btn;
        private PictureBox run_btn;
        private PictureBox runstop_btn;
        private PictureBox temp_btn;
        private PictureBox record_btn;
        private Panel info_panel;
    }
}