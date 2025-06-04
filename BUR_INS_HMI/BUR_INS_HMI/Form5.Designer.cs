namespace BUR_INS_HMI
{
    partial class Form5
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
            DB_panel = new Panel();
            dataGridView1 = new DataGridView();
            DB_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // DB_panel
            // 
            DB_panel.BackColor = SystemColors.ActiveCaptionText;
            DB_panel.Controls.Add(dataGridView1);
            DB_panel.Dock = DockStyle.Fill;
            DB_panel.Location = new Point(0, 0);
            DB_panel.Name = "DB_panel";
            DB_panel.Size = new Size(1123, 610);
            DB_panel.TabIndex = 0;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 12);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(1099, 586);
            dataGridView1.TabIndex = 0;
            // 
            // Form5
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1123, 610);
            Controls.Add(DB_panel);
            Name = "Form5";
            Text = "Form5";
            DB_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel DB_panel;
        private DataGridView dataGridView1;
    }
}