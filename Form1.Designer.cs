namespace PdfCutter
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
            tableLayoutPanel1 = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnOpenPdf = new Button();
            btnAutoDetect = new Button();
            btnCut = new Button();
            btnSave = new Button();
            btnPrint = new Button();
            pictureBox = new PictureBox();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 0);
            tableLayoutPanel1.Controls.Add(pictureBox, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(700, 510);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnOpenPdf);
            flowLayoutPanel1.Controls.Add(btnAutoDetect);
            flowLayoutPanel1.Controls.Add(btnCut);
            flowLayoutPanel1.Controls.Add(btnSave);
            flowLayoutPanel1.Controls.Add(btnPrint);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(694, 28);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // btnOpenPdf
            // 
            btnOpenPdf.Location = new Point(3, 3);
            btnOpenPdf.Name = "btnOpenPdf";
            btnOpenPdf.Size = new Size(82, 25);
            btnOpenPdf.TabIndex = 0;
            btnOpenPdf.Text = "打开 PDF";
            btnOpenPdf.UseVisualStyleBackColor = true;
            btnOpenPdf.Click += btnOpenPdf_Click;
            // 
            // btnAutoDetect
            // 
            btnAutoDetect.Location = new Point(91, 3);
            btnAutoDetect.Name = "btnAutoDetect";
            btnAutoDetect.Size = new Size(122, 25);
            btnAutoDetect.TabIndex = 2;
            btnAutoDetect.Text = "自动 识别 区域";
            btnAutoDetect.UseVisualStyleBackColor = true;
            btnAutoDetect.Click += btnAutoDetect_Click;
            // 
            // btnCut
            // 
            btnCut.Location = new Point(219, 3);
            btnCut.Name = "btnCut";
            btnCut.Size = new Size(82, 25);
            btnCut.TabIndex = 1;
            btnCut.Text = "裁剪";
            btnCut.UseVisualStyleBackColor = true;
            btnCut.Click += btnCut_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(307, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(94, 29);
            btnSave.TabIndex = 3;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnPrint
            // 
            btnPrint.Location = new Point(407, 3);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(94, 29);
            btnPrint.TabIndex = 4;
            btnPrint.Text = "打印";
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Click += btnPrint_Click;
            // 
            // pictureBox
            // 
            pictureBox.BackColor = SystemColors.ControlLight;
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.Location = new Point(3, 37);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(694, 470);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex = 1;
            pictureBox.TabStop = false;
            pictureBox.Paint += pictureBox_Paint;
            pictureBox.MouseDown += pictureBox_MouseDown;
            pictureBox.MouseMove += pictureBox_MouseMove;
            pictureBox.MouseUp += pictureBox_MouseUp;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 510);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            Text = "PDF Cutter";
            tableLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnOpenPdf;
        private Button btnCut;
        private Button btnAutoDetect;
        private Button btnSave;
        private Button btnPrint;
        private PictureBox pictureBox;
    }
}
