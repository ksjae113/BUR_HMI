namespace BUR_INS_HMI
{
    partial class Form6
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
            dataGridView1 = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            btnRefresh = new Button();
            dtStart = new DateTimePicker();
            dtEnd = new DateTimePicker();
            label3 = new Label();
            label4 = new Label();
            cbChannel = new ComboBox();
            txtValue = new TextBox();
            btnFilter = new Button();
            chkUseDate = new CheckBox();
            filter_pan = new Panel();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            filter_pan.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 72);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(1784, 839);
            dataGridView1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1011, 13);
            label1.Name = "label1";
            label1.Size = new Size(17, 15);
            label1.TabIndex = 1;
            label1.Text = "id";
            label1.Visible = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1005, 39);
            label2.Name = "label2";
            label2.Size = new Size(37, 15);
            label2.TabIndex = 2;
            label2.Text = "name";
            label2.Visible = false;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(1034, 10);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 3;
            textBox1.Visible = false;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(1093, 39);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 23);
            textBox2.TabIndex = 4;
            textBox2.Visible = false;
            // 
            // button1
            // 
            button1.Location = new Point(1140, 10);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "추가";
            button1.UseVisualStyleBackColor = true;
            button1.Visible = false;
            // 
            // button2
            // 
            button2.Location = new Point(1199, 39);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 6;
            button2.Text = "조회";
            button2.UseVisualStyleBackColor = true;
            button2.Visible = false;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(678, 10);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(75, 23);
            btnRefresh.TabIndex = 7;
            btnRefresh.Text = "초기화";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // dtStart
            // 
            dtStart.Location = new Point(3, 46);
            dtStart.Name = "dtStart";
            dtStart.Size = new Size(200, 23);
            dtStart.TabIndex = 8;
            // 
            // dtEnd
            // 
            dtEnd.Location = new Point(209, 46);
            dtEnd.Name = "dtEnd";
            dtEnd.Size = new Size(200, 23);
            dtEnd.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 29);
            label3.Name = "label3";
            label3.Size = new Size(43, 15);
            label3.TabIndex = 10;
            label3.Text = "시작일";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(209, 29);
            label4.Name = "label4";
            label4.Size = new Size(43, 15);
            label4.TabIndex = 11;
            label4.Text = "종료일";
            // 
            // cbChannel
            // 
            cbChannel.FormattingEnabled = true;
            cbChannel.Location = new Point(436, 46);
            cbChannel.Name = "cbChannel";
            cbChannel.Size = new Size(121, 23);
            cbChannel.TabIndex = 12;
            cbChannel.Text = "검색 항목";
            // 
            // txtValue
            // 
            txtValue.Location = new Point(563, 46);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(100, 23);
            txtValue.TabIndex = 13;
            txtValue.Text = "검색 값";
            // 
            // btnFilter
            // 
            btnFilter.Location = new Point(678, 43);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(75, 23);
            btnFilter.TabIndex = 14;
            btnFilter.Text = "조회하기";
            btnFilter.UseVisualStyleBackColor = true;
            btnFilter.Click += btnFilter_Click;
            // 
            // chkUseDate
            // 
            chkUseDate.AutoSize = true;
            chkUseDate.Location = new Point(3, 5);
            chkUseDate.Name = "chkUseDate";
            chkUseDate.Size = new Size(106, 19);
            chkUseDate.TabIndex = 15;
            chkUseDate.Text = "날짜 사용 여부";
            chkUseDate.UseVisualStyleBackColor = true;
            // 
            // filter_pan
            // 
            filter_pan.Controls.Add(label3);
            filter_pan.Controls.Add(button2);
            filter_pan.Controls.Add(chkUseDate);
            filter_pan.Controls.Add(textBox2);
            filter_pan.Controls.Add(button1);
            filter_pan.Controls.Add(label2);
            filter_pan.Controls.Add(btnRefresh);
            filter_pan.Controls.Add(btnFilter);
            filter_pan.Controls.Add(textBox1);
            filter_pan.Controls.Add(dtStart);
            filter_pan.Controls.Add(txtValue);
            filter_pan.Controls.Add(label1);
            filter_pan.Controls.Add(dtEnd);
            filter_pan.Controls.Add(cbChannel);
            filter_pan.Controls.Add(label4);
            filter_pan.Dock = DockStyle.Top;
            filter_pan.Location = new Point(0, 0);
            filter_pan.Name = "filter_pan";
            filter_pan.Size = new Size(1784, 72);
            filter_pan.TabIndex = 16;
            // 
            // Form6
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1784, 911);
            Controls.Add(dataGridView1);
            Controls.Add(filter_pan);
            Name = "Form6";
            Text = "History";
            Load += Form6_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            filter_pan.ResumeLayout(false);
            filter_pan.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button1;
        private Button button2;
        private Button btnRefresh;
        private DateTimePicker dtStart;
        private DateTimePicker dtEnd;
        private Label label3;
        private Label label4;
        private ComboBox cbChannel;
        private TextBox txtValue;
        private Button btnFilter;
        private CheckBox chkUseDate;
        private Panel filter_pan;
    }
}