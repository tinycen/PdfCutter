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
            mainToolStrip = new ToolStrip();
            btnOpenPdf = new ToolStripButton();
            btnAutoDetect = new ToolStripButton();
            btnCut = new ToolStripButton();
            btnSave = new ToolStripButton();
            btnPrint = new ToolStripButton();
            btnPrintPreview = new ToolStripButton();
            pictureBoxMain = new PictureBox();
            mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMain).BeginInit();
            SuspendLayout();
            // 
            // mainToolStrip
            // 
            mainToolStrip.Items.AddRange(new ToolStripItem[] { btnOpenPdf, btnAutoDetect, btnCut, btnSave, btnPrint, btnPrintPreview });
            mainToolStrip.Location = new Point(0, 0);
            mainToolStrip.Name = "mainToolStrip";
            mainToolStrip.Size = new Size(800, 25);
            mainToolStrip.TabIndex = 0;
            mainToolStrip.Text = "mainToolStrip";
            mainToolStrip.ItemClicked += mainToolStrip_ItemClicked;
            // 
            // btnOpenPdf
            // 
            btnOpenPdf.Name = "btnOpenPdf";
            btnOpenPdf.Size = new Size(58, 22);
            btnOpenPdf.Text = "打开PDF";
            btnOpenPdf.Click += btnOpenPdf_Click;
            // 
            // btnAutoDetect
            // 
            btnAutoDetect.Name = "btnAutoDetect";
            btnAutoDetect.Size = new Size(60, 22);
            btnAutoDetect.Text = "自动检测";
            btnAutoDetect.Click += btnAutoDetect_Click;
            // 
            // btnCut
            // 
            btnCut.Name = "btnCut";
            btnCut.Size = new Size(36, 22);
            btnCut.Text = "裁剪";
            btnCut.Click += btnCut_Click;
            // 
            // btnSave
            // 
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(36, 22);
            btnSave.Text = "保存";
            btnSave.Click += btnSave_Click;
            // 
            // btnPrint
            // 
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(36, 22);
            btnPrint.Text = "打印";
            btnPrint.Click += btnPrint_Click;
            // 
            // btnPrintPreview
            // 
            btnPrintPreview.Name = "btnPrintPreview";
            btnPrintPreview.Size = new Size(60, 22);
            btnPrintPreview.Text = "打印预览";
            btnPrintPreview.Click += btnPrintPreview_Click;
            // 
            // pictureBoxMain
            // 
            pictureBoxMain.Dock = DockStyle.Fill;
            pictureBoxMain.Location = new Point(0, 25);
            pictureBoxMain.Name = "pictureBoxMain";
            pictureBoxMain.Size = new Size(800, 485);
            pictureBoxMain.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxMain.TabIndex = 1;
            pictureBoxMain.TabStop = false;
            pictureBoxMain.Paint += pictureBox_Paint;
            pictureBoxMain.MouseDown += pictureBox_MouseDown;
            pictureBoxMain.MouseMove += pictureBox_MouseMove;
            pictureBoxMain.MouseUp += pictureBox_MouseUp;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 510);
            Controls.Add(pictureBoxMain);
            Controls.Add(mainToolStrip);
            Name = "Form1";
            Text = "PDF裁剪工具";
            mainToolStrip.ResumeLayout(false);
            mainToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMain).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
