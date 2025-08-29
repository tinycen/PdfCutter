using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using PdfiumViewer;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace PdfCutter
{
    public partial class Form1 : Form
    {
        private string? currentPdfPath;
        private PdfiumViewer.PdfDocument? pdfViewerDoc;
        private Image? currentPageImage;
        private Rectangle selectionRectangle;
        private Point? selectionStart;
        private bool isSelecting = false;
        private int currentPage = 1;
        private Bitmap? cutPreviewImage = null;
        private float lastPrintScale = 1.0f;
        private PrinterSettings printerSettings;
        private ToolStripComboBox printerComboBox = null!;
        private ToolStripComboBox paperSizeComboBox = null!;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            printerSettings = PrinterSettings.Load();
            InitializePrinterControls();
        }

        private void InitializePrinterControls()
        {
            // Create printer selection combobox
            printerComboBox = new ToolStripComboBox();
            printerComboBox.Width = 200; // 保持原宽度
            printerComboBox.ToolTipText = "选择打印机";
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printerComboBox.Items.Add(printer);
            }

            // Create paper size combobox
            paperSizeComboBox = new ToolStripComboBox();
            paperSizeComboBox.AutoSize = false; // 关键设置
            paperSizeComboBox.Width = 200; // 增加到500
            paperSizeComboBox.ToolTipText = "选择纸张尺寸";

            // Add controls to mainToolStrip at the beginning
            mainToolStrip.Items.Insert(0, new ToolStripLabel("打印机: "));
            mainToolStrip.Items.Insert(1, printerComboBox);
            mainToolStrip.Items.Insert(2, new ToolStripSeparator());
            mainToolStrip.Items.Insert(3, new ToolStripLabel("纸张: "));
            mainToolStrip.Items.Insert(4, paperSizeComboBox);
            mainToolStrip.Items.Insert(5, new ToolStripSeparator());

            // Restore last used printer
            if (!string.IsNullOrEmpty(printerSettings.LastUsedPrinter))
            {
                int index = printerComboBox.Items.IndexOf(printerSettings.LastUsedPrinter);
                if (index >= 0)
                {
                    printerComboBox.SelectedIndex = index;
                }
            }

            printerComboBox.SelectedIndexChanged += PrinterComboBox_SelectedIndexChanged;
            paperSizeComboBox.SelectedIndexChanged += PaperSizeComboBox_SelectedIndexChanged;

            // Initialize paper sizes for default/saved printer
            if (printerComboBox.SelectedItem != null)
            {
                UpdatePaperSizes(printerComboBox.SelectedItem.ToString()!);
            }
        }

        private void PrinterComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (printerComboBox.SelectedItem != null)
            {
                string selectedPrinter = printerComboBox.SelectedItem.ToString()!;
                UpdatePaperSizes(selectedPrinter);
                printerSettings.LastUsedPrinter = selectedPrinter;
                printerSettings.Save();
            }
        }

        private void PaperSizeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (paperSizeComboBox.SelectedItem != null)
            {
                printerSettings.LastUsedPaperName = paperSizeComboBox.SelectedItem.ToString();
                printerSettings.Save();
            }
        }

        private void UpdatePaperSizes(string printerName)
        {
            paperSizeComboBox.Items.Clear();
            var printSettings = new System.Drawing.Printing.PrinterSettings();
            printSettings.PrinterName = printerName;

            foreach (System.Drawing.Printing.PaperSize paperSize in printSettings.PaperSizes)
            {
                paperSizeComboBox.Items.Add(paperSize.PaperName);
            }

            // Restore last used paper size
            if (!string.IsNullOrEmpty(printerSettings.LastUsedPaperName))
            {
                int index = paperSizeComboBox.Items.IndexOf(printerSettings.LastUsedPaperName);
                if (index >= 0)
                {
                    paperSizeComboBox.SelectedIndex = index;
                    return;
                }
            }

            // Default to A4 if available, otherwise select first paper size
            int a4Index = paperSizeComboBox.Items.IndexOf("A4");
            paperSizeComboBox.SelectedIndex = a4Index >= 0 ? a4Index : 0;
        }

        private void btnOpenPdf_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentPdfPath = openFileDialog.FileName;
                    LoadPdfPage(1);
                }
            }
        }

        private void LoadPdfPage(int pageNumber)
        {
            if (currentPdfPath == null) return;
            try
            {
                if (pdfViewerDoc != null)
                {
                    pdfViewerDoc.Dispose();
                    pdfViewerDoc = null;
                }
                if (currentPageImage != null)
                {
                    currentPageImage.Dispose();
                    currentPageImage = null;
                }
                pdfViewerDoc = PdfiumViewer.PdfDocument.Load(currentPdfPath);
                currentPage = pageNumber;
                var size = pdfViewerDoc.PageSizes[pageNumber - 1];
                int dpi = 200;
                int renderWidth = (int)(size.Width * dpi / 72.0);
                int renderHeight = (int)(size.Height * dpi / 72.0);
                currentPageImage = pdfViewerDoc.Render(pageNumber - 1, renderWidth, renderHeight, dpi, dpi, false);
                cutPreviewImage?.Dispose();
                cutPreviewImage = null;
                pictureBoxMain.Image = null;
                pictureBoxMain.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (cutPreviewImage != null)
            {
                e.Graphics.DrawImage(cutPreviewImage, 0, 0, pictureBoxMain.Width, pictureBoxMain.Height);
                return;
            }

            if (currentPageImage != null)
            {
                float scale = Math.Min(
                    (float)pictureBoxMain.Width / currentPageImage.Width,
                    (float)pictureBoxMain.Height / currentPageImage.Height
                );

                int width = (int)(currentPageImage.Width * scale);
                int height = (int)(currentPageImage.Height * scale);
                int x = (pictureBoxMain.Width - width) / 2;
                int y = (pictureBoxMain.Height - height) / 2;

                e.Graphics.DrawImage(currentPageImage, x, y, width, height);

                if (isSelecting)
                {
                    using (Pen pen = new Pen(Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, selectionRectangle);
                    }
                }
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && currentPageImage != null)
            {
                isSelecting = true;
                selectionStart = e.Location;
                selectionRectangle = new Rectangle(e.Location, new Size(0, 0));
                pictureBoxMain.Invalidate();
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting && selectionStart.HasValue)
            {
                int x = Math.Min(selectionStart.Value.X, e.X);
                int y = Math.Min(selectionStart.Value.Y, e.Y);
                int width = Math.Abs(e.X - selectionStart.Value.X);
                int height = Math.Abs(e.Y - selectionStart.Value.Y);

                selectionRectangle = new Rectangle(x, y, width, height);
                pictureBoxMain.Invalidate();
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSelecting = false;
                if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
                {
                    DetectNonWhiteArea();
                }
            }
        }

        private Rectangle ConvertToImageCoordinates(Rectangle screenRect)
        {
            if (currentPageImage == null) return Rectangle.Empty;

            float scale = Math.Min(
                (float)pictureBoxMain.Width / currentPageImage.Width,
                (float)pictureBoxMain.Height / currentPageImage.Height
            );

            int imageWidth = (int)(currentPageImage.Width * scale);
            int imageHeight = (int)(currentPageImage.Height * scale);
            int imageX = (pictureBoxMain.Width - imageWidth) / 2;
            int imageY = (pictureBoxMain.Height - imageHeight) / 2;

            int x = (int)((screenRect.X - imageX) / scale);
            int y = (int)((screenRect.Y - imageY) / scale);
            int width = (int)(screenRect.Width / scale);
            int height = (int)(screenRect.Height / scale);

            return new Rectangle(x, y, width, height);
        }

        private void DetectNonWhiteArea()
        {
            if (currentPageImage == null) return;

            Rectangle imageRect = ConvertToImageCoordinates(selectionRectangle);
            if (imageRect.IsEmpty) return;

            // Ensure the rectangle is within the image bounds
            imageRect.Intersect(new Rectangle(0, 0, currentPageImage.Width, currentPageImage.Height));
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            if (currentPageImage == null || selectionRectangle.IsEmpty)
            {
                MessageBox.Show("请先选择区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Rectangle imageRect = ConvertToImageCoordinates(selectionRectangle);
            Bitmap srcBmp = new Bitmap(currentPageImage);
            cutPreviewImage?.Dispose();
            cutPreviewImage = new Bitmap(imageRect.Width, imageRect.Height);
            using (Graphics g = Graphics.FromImage(cutPreviewImage))
            {
                g.DrawImage(srcBmp, new Rectangle(0, 0, imageRect.Width, imageRect.Height), imageRect, GraphicsUnit.Pixel);
            }
            srcBmp.Dispose();
            // 用裁剪后的图片刷新预览
            pictureBoxMain.Image = cutPreviewImage;
            pictureBoxMain.Invalidate();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cutPreviewImage == null)
            {
                MessageBox.Show("请先裁剪区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG图片 (*.png)|*.png|JPEG图片 (*.jpg)|*.jpg|BMP图片 (*.bmp)|*.bmp|所有文件 (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.FileName = "cut_preview";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImageFormat format = ImageFormat.Png; // 默认PNG格式
                        string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();

                        // 根据文件扩展名选择保存格式
                        switch (extension)
                        {
                            case ".jpg":
                            case ".jpeg":
                                format = ImageFormat.Jpeg;
                                break;
                            case ".bmp":
                                format = ImageFormat.Bmp;
                                break;
                        }

                        cutPreviewImage.Save(saveFileDialog.FileName, format);
                        MessageBox.Show("保存成功！", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存出错: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SetupPrintDocument(PrintDocument printDoc, Image imageToPrint)
        {
            // 使用选择的打印机
            if (!string.IsNullOrEmpty(printerSettings.LastUsedPrinter))
            {
                printDoc.PrinterSettings.PrinterName = printerSettings.LastUsedPrinter;
            }

            // 设置纸张大小
            if (!string.IsNullOrEmpty(printerSettings.LastUsedPaperName))
            {
                foreach (System.Drawing.Printing.PaperSize paperSize in printDoc.PrinterSettings.PaperSizes)
                {
                    if (paperSize.PaperName == printerSettings.LastUsedPaperName)
                    {
                        printDoc.DefaultPageSettings.PaperSize = paperSize;
                        break;
                    }
                }
            }

            // 设置默认页边距为 1（单位：百分之一英寸）
            printDoc.DefaultPageSettings.Margins = new Margins(1, 1, 1, 1);

            // 计算图片和纸张的纵横比
            double imageAspectRatio = (double)imageToPrint.Width / imageToPrint.Height;
            double paperAspectRatio = (double)printDoc.DefaultPageSettings.PaperSize.Width / printDoc.DefaultPageSettings.PaperSize.Height;
            bool shouldBeLandscape = imageAspectRatio > 1;

            // 设置方向
            printDoc.DefaultPageSettings.Landscape = shouldBeLandscape;
            printerSettings.Landscape = shouldBeLandscape;
            printerSettings.Save();

            printDoc.PrintPage += (s, ev) =>
            {
                Rectangle marginBounds = ev.MarginBounds;
                float scale = Math.Min((float)marginBounds.Width / imageToPrint.Width, (float)marginBounds.Height / imageToPrint.Height);
                int printWidth = (int)(imageToPrint.Width * scale);
                int printHeight = (int)(imageToPrint.Height * scale);
                int x = marginBounds.X + (marginBounds.Width - printWidth) / 2;
                int y = marginBounds.Y + (marginBounds.Height - printHeight) / 2;
                lastPrintScale = scale;
                ev.Graphics!.DrawImage(imageToPrint, x, y, printWidth, printHeight);
            };
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Image? imageToPrint = cutPreviewImage ?? currentPageImage;
            if (imageToPrint == null)
            {
                MessageBox.Show("没有可打印的内容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (PrintDialog printDialog = new PrintDialog())
            {
                PrintDocument printDoc = new PrintDocument();
                SetupPrintDocument(printDoc, imageToPrint);
                printDialog.Document = printDoc;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        printDoc.Print();
                        MessageBox.Show($"打印缩放比例: {lastPrintScale:F2}", "打印信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"打印出错: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            Image? imageToPrint = cutPreviewImage ?? currentPageImage;
            if (imageToPrint == null)
            {
                MessageBox.Show("没有可预览的内容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Form previewForm = new Form())
            using (PrintPreviewControl previewControl = new PrintPreviewControl())
            using (PrintDocument printDoc = new PrintDocument())
            {
                // 设置预览窗口
                previewForm.Text = "打印预览";
                previewForm.WindowState = FormWindowState.Maximized;

                // 创建工具栏
                ToolStrip toolStrip = new ToolStrip();
                ToolStripButton pageSetupButton = new ToolStripButton("页面设置");
                ToolStripButton printerButton = new ToolStripButton("打印机设置");
                ToolStripButton printButton = new ToolStripButton("打印");
                toolStrip.Items.AddRange(new ToolStripItem[] { pageSetupButton, printerButton, printButton });

                // 设置预览控件
                previewControl.Document = printDoc;
                previewControl.UseAntiAlias = true;
                previewControl.Dock = DockStyle.Fill;

                // 添加控件到窗口
                previewForm.Controls.Add(previewControl);
                previewForm.Controls.Add(toolStrip);

                // 设置打印文档
                SetupPrintDocument(printDoc, imageToPrint);

                // 页面设置按钮事件
                pageSetupButton.Click += (s, ev) =>
                {
                    using (PageSetupDialog pageSetupDialog = new PageSetupDialog())
                    {
                        pageSetupDialog.Document = printDoc;
                        if (pageSetupDialog.ShowDialog(previewForm) == DialogResult.OK)
                        {
                            previewControl.InvalidatePreview();
                        }
                    }
                };

                // 打印机设置按钮事件
                printerButton.Click += (s, ev) =>
                {
                    using (PrintDialog printDialog = new PrintDialog())
                    {
                        printDialog.Document = printDoc;
                        if (printDialog.ShowDialog(previewForm) == DialogResult.OK)
                        {
                            previewControl.InvalidatePreview();
                        }
                    }
                };

                // 打印按钮事件
                printButton.Click += (s, ev) =>
                {
                    try
                    {
                        printDoc.Print();
                        MessageBox.Show($"打印缩放比例: {lastPrintScale:F2}", "打印信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        previewForm.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"打印出错: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                // 显示预览窗口
                previewForm.ShowDialog();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pdfViewerDoc != null)
            {
                pdfViewerDoc.Dispose();
            }

            if (currentPageImage != null)
            {
                currentPageImage.Dispose();
            }
        }

        private void btnAutoDetect_Click(object sender, EventArgs e)
        {
            if (currentPageImage == null) return;
            var bmp = new Bitmap(currentPageImage);

            // 确保为 32bppArgb，便于高效扫描
            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                var converted = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(converted))
                {
                    g.DrawImageUnscaled(bmp, 0, 0);
                }
                bmp.Dispose();
                bmp = converted;
            }

            int left = bmp.Width, top = bmp.Height, right = -1, bottom = -1;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                int stride = data.Stride;
                int width = bmp.Width;
                int height = bmp.Height;
                int byteCount = stride * height;
                byte[] buffer = new byte[byteCount];
                System.Runtime.InteropServices.Marshal.Copy(data.Scan0, buffer, 0, byteCount);

                const byte threshold = 240;
                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int idx = rowOffset + (x << 2); // x * 4
                        byte b = buffer[idx + 0];
                        byte g = buffer[idx + 1];
                        byte r = buffer[idx + 2];
                        byte a = buffer[idx + 3];

                        // 透明像素忽略；非白阈值检测
                        if (a != 0 && (r < threshold || g < threshold || b < threshold))
                        {
                            if (x < left) left = x;
                            if (y < top) top = y;
                            if (x > right) right = x;
                            if (y > bottom) bottom = y;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            if (right >= 0 && bottom >= 0 && left < right && top < bottom)
            {
                float scale = Math.Min((float)pictureBoxMain.Width / bmp.Width, (float)pictureBoxMain.Height / bmp.Height);
                int imageWidth = (int)(bmp.Width * scale);
                int imageHeight = (int)(bmp.Height * scale);
                int imageX = (pictureBoxMain.Width - imageWidth) / 2;
                int imageY = (pictureBoxMain.Height - imageHeight) / 2;
                selectionRectangle = new Rectangle(
                    (int)(left * scale) + imageX,
                    (int)(top * scale) + imageY,
                    (int)((right - left) * scale),
                    (int)((bottom - top) * scale)
                );
                isSelecting = true;
                pictureBoxMain.Invalidate();
            }
            bmp.Dispose();
        }

        private void mainToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
