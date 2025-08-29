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
        private iText.Kernel.Pdf.PdfDocument? pdfReaderDoc;
        private PdfiumViewer.PdfDocument? pdfViewerDoc;
        private Image? currentPageImage;
        private Rectangle selectionRectangle;
        private Point? selectionStart;
        private bool isSelecting = false;
        private int currentPage = 1;
        private Bitmap? cutPreviewImage = null;
        private float lastPrintScale = 1.0f; // 保存最近一次打印的缩放比例

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
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

        private void btnAutoDetect_Click(object sender, EventArgs e)
        {
            if (currentPageImage == null) return;
            var bmp = new Bitmap(currentPageImage);
            int left = bmp.Width, top = bmp.Height, right = 0, bottom = 0;
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    var color = bmp.GetPixel(x, y);
                    // 判断是否为非白色像素
                    if (color.R < 240 || color.G < 240 || color.B < 240)
                    {
                        if (x < left) left = x;
                        if (y < top) top = y;
                        if (x > right) right = x;
                        if (y > bottom) bottom = y;
                    }
                }
            }
            if (left < right && top < bottom)
            {
                float scale = Math.Min((float)pictureBox.Width / bmp.Width, (float)pictureBox.Height / bmp.Height);
                int imageWidth = (int)(bmp.Width * scale);
                int imageHeight = (int)(bmp.Height * scale);
                int imageX = (pictureBox.Width - imageWidth) / 2;
                int imageY = (pictureBox.Height - imageHeight) / 2;
                selectionRectangle = new Rectangle(
                    (int)(left * scale) + imageX,
                    (int)(top * scale) + imageY,
                    (int)((right - left) * scale),
                    (int)((bottom - top) * scale)
                );
                isSelecting = true; // 保证红色框线可视化
                pictureBox.Invalidate();
            }
            bmp.Dispose();
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
                // 提高渲染分辨率
                int dpi = 200;
                int renderWidth = (int)(size.Width * dpi / 72.0);
                int renderHeight = (int)(size.Height * dpi / 72.0);
                currentPageImage = pdfViewerDoc.Render(pageNumber - 1, renderWidth, renderHeight, dpi, dpi, false);
                cutPreviewImage?.Dispose();
                cutPreviewImage = null;
                pictureBox.Image = null;
                // 显示当前页面
                pictureBox.Invalidate();
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
                // 预览裁剪后的图片
                e.Graphics.DrawImage(cutPreviewImage, 0, 0, pictureBox.Width, pictureBox.Height);
                return;
            }

            if (currentPageImage != null)
            {
                // Calculate scaling to fit the image while maintaining aspect ratio
                float scale = Math.Min(
                    (float)pictureBox.Width / currentPageImage.Width,
                    (float)pictureBox.Height / currentPageImage.Height
                );

                int width = (int)(currentPageImage.Width * scale);
                int height = (int)(currentPageImage.Height * scale);
                int x = (pictureBox.Width - width) / 2;
                int y = (pictureBox.Height - height) / 2;

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
                pictureBox.Invalidate();
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
                pictureBox.Invalidate();
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

            // Calculate scaling and offset
            float scale = Math.Min(
                (float)pictureBox.Width / currentPageImage.Width,
                (float)pictureBox.Height / currentPageImage.Height
            );

            int imageWidth = (int)(currentPageImage.Width * scale);
            int imageHeight = (int)(currentPageImage.Height * scale);
            int imageX = (pictureBox.Width - imageWidth) / 2;
            int imageY = (pictureBox.Height - imageHeight) / 2;

            // Convert screen coordinates to image coordinates
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
            pictureBox.Image = cutPreviewImage;
            pictureBox.Invalidate();
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
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.FileName = "cut_preview.pdf";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var ms = new System.IO.MemoryStream())
                        {
                            cutPreviewImage.Save(ms, ImageFormat.Png);
                            ms.Position = 0;
                            using (var writer = new iText.Kernel.Pdf.PdfWriter(saveFileDialog.FileName))
                            {
                                var pdfDoc = new iText.Kernel.Pdf.PdfDocument(writer);
                                var doc = new iText.Layout.Document(pdfDoc);
                                var img = new iText.Layout.Element.Image(iText.IO.Image.ImageDataFactory.Create(ms.ToArray()));
                                img.SetAutoScale(true);
                                doc.Add(img);
                                doc.Close();
                                pdfDoc.Close();
                            }
                        }
                        MessageBox.Show("保存成功!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存出错: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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
                printDoc.PrintPage += (s, ev) =>
                {
                    Rectangle marginBounds = ev.MarginBounds;
                    float scale = Math.Min((float)marginBounds.Width / imageToPrint.Width, (float)marginBounds.Height / imageToPrint.Height);
                    lastPrintScale = scale; // 保存缩放比例
                    int printWidth = (int)(imageToPrint.Width * scale);
                    int printHeight = (int)(imageToPrint.Height * scale);
                    int x = marginBounds.X + (marginBounds.Width - printWidth) / 2;
                    int y = marginBounds.Y + (marginBounds.Height - printHeight) / 2;
                    ev.Graphics.DrawImage(imageToPrint, x, y, printWidth, printHeight);
                };
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
                // 设置预览窗口属性
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

                // 打印页面处理
                printDoc.PrintPage += (s, ev) =>
                {
                    Rectangle marginBounds = ev.MarginBounds;
                    float scale = Math.Min((float)marginBounds.Width / imageToPrint.Width, (float)marginBounds.Height / imageToPrint.Height);
                    lastPrintScale = scale;
                    int printWidth = (int)(imageToPrint.Width * scale);
                    int printHeight = (int)(imageToPrint.Height * scale);
                    int x = marginBounds.X + (marginBounds.Width - printWidth) / 2;
                    int y = marginBounds.Y + (marginBounds.Height - printHeight) / 2;
                    ev.Graphics.DrawImage(imageToPrint, x, y, printWidth, printHeight);
                };

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
    }
}
