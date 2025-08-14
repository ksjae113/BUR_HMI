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
            editer_show = new Label();
            editer_btn_del = new Button();
            editer_btn_clr = new Button();
            editer_btn_close = new Button();
            editer_btn_check = new Button();
            editer = new TextBox();
            SuspendLayout();
            // 
            // unit_lbl
            // 
            unit_lbl.AutoSize = true;
            unit_lbl.BackColor = Color.Yellow;
            unit_lbl.Font = new Font("맑은 고딕", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            unit_lbl.Location = new Point(2, 9);
            unit_lbl.Name = "unit_lbl";
            unit_lbl.Size = new Size(64, 40);
            unit_lbl.TabIndex = 1;
            unit_lbl.Text = "mA";
            unit_lbl.TextAlign = ContentAlignment.MiddleCenter;
            unit_lbl.Visible = false;
            // 
            // editer_show
            // 
            editer_show.BackColor = Color.Yellow;
            editer_show.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            editer_show.Location = new Point(450, 292);
            editer_show.Name = "editer_show";
            editer_show.Size = new Size(122, 69);
            editer_show.TabIndex = 2;
            editer_show.Text = "00.0";
            editer_show.TextAlign = ContentAlignment.MiddleRight;
            // 
            // editer_btn_del
            // 
            editer_btn_del.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            editer_btn_del.Location = new Point(450, 110);
            editer_btn_del.Name = "editer_btn_del";
            editer_btn_del.Size = new Size(120, 60);
            editer_btn_del.TabIndex = 14;
            editer_btn_del.Text = "DEL";
            editer_btn_del.UseVisualStyleBackColor = true;
            // 
            // editer_btn_clr
            // 
            editer_btn_clr.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            editer_btn_clr.Location = new Point(450, 200);
            editer_btn_clr.Name = "editer_btn_clr";
            editer_btn_clr.Size = new Size(120, 60);
            editer_btn_clr.TabIndex = 15;
            editer_btn_clr.Text = "CLR";
            editer_btn_clr.UseVisualStyleBackColor = true;
            // 
            // editer_btn_close
            // 
            editer_btn_close.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            editer_btn_close.Location = new Point(450, 475);
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
            editer_btn_check.Location = new Point(450, 390);
            editer_btn_check.Name = "editer_btn_check";
            editer_btn_check.Size = new Size(120, 60);
            editer_btn_check.TabIndex = 17;
            editer_btn_check.Text = "확인";
            editer_btn_check.UseVisualStyleBackColor = true;
            editer_btn_check.Click += editer_btn_check_Click;
            // 
            // editer
            // 
            editer.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            editer.Location = new Point(12, 10);
            editer.Multiline = true;
            editer.Name = "editer";
            editer.Size = new Size(414, 70);
            editer.TabIndex = 18;
            editer.KeyDown += editer_KeyDown;
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(436, 88);
            Controls.Add(editer);
            Controls.Add(editer_btn_check);
            Controls.Add(editer_btn_close);
            Controls.Add(editer_btn_clr);
            Controls.Add(editer_btn_del);
            Controls.Add(editer_show);
            Controls.Add(unit_lbl);
            Name = "Form4";
            Text = "Editer";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label editer_show;
        private Button editer_btn_del;
        private Button editer_btn_clr;
        private Button editer_btn_close;
        private Button editer_btn_check;
        internal Label unit_lbl;
        private TextBox editer;
    }
}