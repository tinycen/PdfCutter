namespace PdfCutter
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBoxMain;
        private System.Windows.Forms.ToolStripButton btnOpenPdf;
        private System.Windows.Forms.ToolStripButton btnAutoDetect;
        private System.Windows.Forms.ToolStripButton btnCut;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripButton btnPrintPreview;
        private System.Windows.Forms.ToolStrip mainToolStrip;

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
            components = new System.ComponentModel.Container();
            mainToolStrip = new ToolStrip();
            pictureBoxMain = new PictureBox();
            btnOpenPdf = new ToolStripButton();
            btnAutoDetect = new ToolStripButton();
            btnCut = new ToolStripButton();
            btnSave = new ToolStripButton();
            btnPrint = new ToolStripButton();
            btnPrintPreview = new ToolStripButton();

            // ToolStrip
            mainToolStrip.Location = new Point(0, 0);
            mainToolStrip.Name = "mainToolStrip";
            mainToolStrip.Size = new Size(800, 25);
            mainToolStrip.TabIndex = 0;
            mainToolStrip.Text = "mainToolStrip";

            // PictureBox
            pictureBoxMain.Dock = DockStyle.Fill;
            pictureBoxMain.Location = new Point(0, 25);
            pictureBoxMain.Name = "pictureBoxMain";
            pictureBoxMain.Size = new Size(800, 425);
            pictureBoxMain.TabIndex = 1;
            pictureBoxMain.TabStop = false;
            pictureBoxMain.Paint += pictureBox_Paint;
            pictureBoxMain.MouseDown += pictureBox_MouseDown;
            pictureBoxMain.MouseMove += pictureBox_MouseMove;
            pictureBoxMain.MouseUp += pictureBox_MouseUp;
            pictureBoxMain.SizeMode = PictureBoxSizeMode.Zoom;

            // Buttons
            btnOpenPdf.Text = "打开PDF";
            btnOpenPdf.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnOpenPdf.Click += (s, e) => btnOpenPdf_Click(s, e);

            btnAutoDetect.Text = "自动检测";
            btnAutoDetect.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnAutoDetect.Click += (s, e) => btnAutoDetect_Click(s, e);

            btnCut.Text = "裁剪";
            btnCut.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnCut.Click += (s, e) => btnCut_Click(s, e);

            btnSave.Text = "保存";
            btnSave.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnSave.Click += (s, e) => btnSave_Click(s, e);

            btnPrint.Text = "打印";
            btnPrint.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnPrint.Click += (s, e) => btnPrint_Click(s, e);

            btnPrintPreview.Text = "打印预览";
            btnPrintPreview.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnPrintPreview.Click += (s, e) => btnPrintPreview_Click(s, e);

            // Add buttons to ToolStrip
            mainToolStrip.Items.Add(btnOpenPdf);
            mainToolStrip.Items.Add(btnAutoDetect);
            mainToolStrip.Items.Add(btnCut);
            mainToolStrip.Items.Add(btnSave);
            mainToolStrip.Items.Add(btnPrint);
            mainToolStrip.Items.Add(btnPrintPreview);

            // Form
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBoxMain);
            Controls.Add(mainToolStrip);
            Name = "Form1";
            Text = "PDF裁剪工具";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
