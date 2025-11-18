using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudyProcessManagement.Views.Teacher.Controls
{
    public class ContentControl : UserControl
    {
        private Panel headerPanel;
        private Label lblTitle;
        private Label lblBreadcrumb;
        private ComboBox cboCourse;
        private Button btnAddSection;
        private Panel mainContentPanel;
        private Panel treeContainerPanel;
        private TreeView tvContent;
        private Panel detailPanel;
        private Label lblDetailTitle;
        private Label lblLessonName;
        private TextBox txtLessonName;
        private Label lblLessonDesc;
        private RichTextBox txtLessonDescription;
        private Label lblVideoUrl;
        private TextBox txtVideoUrl;
        private Label lblMaterials;
        private TextBox txtMaterials;
        private Panel buttonPanel;
        private Button btnSave;
        private Button btnAddLesson;
        private Button btnDelete;

        public ContentControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.headerPanel = new Panel();
            this.lblTitle = new Label();
            this.lblBreadcrumb = new Label();
            this.cboCourse = new ComboBox();
            this.btnAddSection = new Button();
            this.mainContentPanel = new Panel();
            this.treeContainerPanel = new Panel();
            this.tvContent = new TreeView();
            this.detailPanel = new Panel();
            this.lblDetailTitle = new Label();
            this.lblLessonName = new Label();
            this.txtLessonName = new TextBox();
            this.lblLessonDesc = new Label();
            this.txtLessonDescription = new RichTextBox();
            this.lblVideoUrl = new Label();
            this.txtVideoUrl = new TextBox();
            this.lblMaterials = new Label();
            this.txtMaterials = new TextBox();
            this.buttonPanel = new Panel();
            this.btnSave = new Button();
            this.btnAddLesson = new Button();
            this.btnDelete = new Button();

            this.SuspendLayout();

            // Main Control
            this.BackColor = Color.FromArgb(248, 248, 248);
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(30, 20, 30, 20);

            // headerPanel
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Height = 130;
            this.headerPanel.BackColor = Color.Transparent;

            // lblTitle
            this.lblTitle.Text = "Quản lý Nội dung";
            this.lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(40, 40, 40);
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new Point(0, 5);

            // lblBreadcrumb
            this.lblBreadcrumb.Text = "Trang chủ / Nội dung / Quản lý bài học";
            this.lblBreadcrumb.Font = new Font("Segoe UI", 9F);
            this.lblBreadcrumb.ForeColor = Color.FromArgb(120, 120, 120);
            this.lblBreadcrumb.AutoSize = true;
            this.lblBreadcrumb.Location = new Point(0, 40);

            // cboCourse
            this.cboCourse.Size = new Size(280, 38);
            this.cboCourse.Font = new Font("Segoe UI", 10F);
            this.cboCourse.Location = new Point(0, 75);
            this.cboCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboCourse.Items.AddRange(new object[] {
                "Lập trình Web với React",
                "Python cơ bản",
                "JavaScript nâng cao",
                "Data Science với Python"
            });
            this.cboCourse.SelectedIndex = 0;

            // btnAddSection
            this.btnAddSection.Text = "➕ Thêm chương";
            this.btnAddSection.Size = new Size(160, 40);
            this.btnAddSection.Location = new Point(300, 75);
            this.btnAddSection.BackColor = Color.FromArgb(76, 175, 80);
            this.btnAddSection.ForeColor = Color.White;
            this.btnAddSection.FlatStyle = FlatStyle.Flat;
            this.btnAddSection.FlatAppearance.BorderSize = 0;
            this.btnAddSection.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnAddSection.Cursor = Cursors.Hand;

            this.btnAddSection.MouseEnter += (s, e) => this.btnAddSection.BackColor = Color.FromArgb(56, 142, 60);
            this.btnAddSection.MouseLeave += (s, e) => this.btnAddSection.BackColor = Color.FromArgb(76, 175, 80);
            this.btnAddSection.Click += (s, e) => MessageBox.Show("Thêm chương mới");

            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.lblBreadcrumb);
            this.headerPanel.Controls.Add(this.cboCourse);
            this.headerPanel.Controls.Add(this.btnAddSection);

            // mainContentPanel
            this.mainContentPanel.Dock = DockStyle.Fill;
            this.mainContentPanel.BackColor = Color.Transparent;

            // treeContainerPanel - Dùng Dock Left chiếm 50%
            this.treeContainerPanel.Dock = DockStyle.Left;
            this.treeContainerPanel.Width = (this.Width - 60) / 2; // 50% minus padding
            this.treeContainerPanel.BackColor = Color.White;
            this.treeContainerPanel.Padding = new Padding(15);
            this.treeContainerPanel.Margin = new Padding(0, 0, 8, 0);

            // Resize handler để treePanel luôn chiếm 50%
            this.Resize += (s, e) =>
            {
                this.treeContainerPanel.Width = (this.Width - 60) / 2;
            };

            this.treeContainerPanel.Paint += (s, e) =>
            {
                // Draw rounded rectangle border
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 8;
                    var rect = new Rectangle(0, 0, this.treeContainerPanel.Width - 1, this.treeContainerPanel.Height - 1);
                    path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                    path.CloseFigure();

                    using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            // tvContent - TreeView với styling đẹp hơn
            this.tvContent.Dock = DockStyle.Fill;
            this.tvContent.Font = new Font("Segoe UI", 11F);
            this.tvContent.BackColor = Color.White;
            this.tvContent.BorderStyle = BorderStyle.None;
            this.tvContent.ItemHeight = 38;
            this.tvContent.Indent = 25;
            this.tvContent.ShowLines = true;
            this.tvContent.ShowPlusMinus = true;
            this.tvContent.ShowRootLines = true;
            this.tvContent.FullRowSelect = true;
            this.tvContent.HideSelection = false;

            // Custom colors for TreeView
            this.tvContent.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.tvContent.DrawNode += (sender, e) =>
            {
                // Background color
                Color bgColor = Color.White;
                if ((e.State & TreeNodeStates.Selected) != 0)
                {
                    bgColor = Color.FromArgb(227, 242, 253);
                }
                else if ((e.State & TreeNodeStates.Hot) != 0)
                {
                    bgColor = Color.FromArgb(245, 245, 245);
                }

                using (var brush = new SolidBrush(bgColor))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                // Text color based on level
                Color textColor;
                Font textFont;

                if (e.Node.Level == 0) // Course node
                {
                    textColor = Color.FromArgb(33, 150, 243);
                    textFont = new Font("Segoe UI", 12F, FontStyle.Bold);
                }
                else if (e.Node.Level == 1) // Section node
                {
                    textColor = Color.FromArgb(76, 175, 80);
                    textFont = new Font("Segoe UI", 11F, FontStyle.Bold);
                }
                else // Lesson node
                {
                    textColor = Color.FromArgb(100, 100, 100);
                    textFont = new Font("Segoe UI", 10.5F);
                }

                // Draw text
                TextRenderer.DrawText(e.Graphics, e.Node.Text, textFont, e.Bounds, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            };

            // Add sample data with better structure
            var courseNode = new TreeNode("📚 Lập trình Web với React");

            var section1 = new TreeNode("📖 Chương 1: Giới thiệu React");
            section1.Nodes.Add(new TreeNode("📄 Bài 1: Tổng quan về React"));
            section1.Nodes.Add(new TreeNode("📄 Bài 2: Cài đặt môi trường"));
            section1.Nodes.Add(new TreeNode("📄 Bài 3: Component đầu tiên"));
            section1.Nodes.Add(new TreeNode("📄 Bài 4: JSX và Virtual DOM"));

            var section2 = new TreeNode("📖 Chương 2: State và Props");
            section2.Nodes.Add(new TreeNode("📄 Bài 5: Hiểu về State"));
            section2.Nodes.Add(new TreeNode("📄 Bài 6: Props là gì?"));
            section2.Nodes.Add(new TreeNode("📄 Bài 7: Props validation"));
            section2.Nodes.Add(new TreeNode("📄 Bài 8: State vs Props"));

            var section3 = new TreeNode("📖 Chương 3: Lifecycle và Hooks");
            section3.Nodes.Add(new TreeNode("📄 Bài 9: Component Lifecycle"));
            section3.Nodes.Add(new TreeNode("📄 Bài 10: useState Hook"));
            section3.Nodes.Add(new TreeNode("📄 Bài 11: useEffect Hook"));
            section3.Nodes.Add(new TreeNode("📄 Bài 12: Custom Hooks"));

            courseNode.Nodes.Add(section1);
            courseNode.Nodes.Add(section2);
            courseNode.Nodes.Add(section3);

            this.tvContent.Nodes.Add(courseNode);
            this.tvContent.ExpandAll();

            this.tvContent.AfterSelect += (s, e) =>
            {
                if (e.Node.Level == 2) // Lesson selected
                {
                    this.txtLessonName.Text = e.Node.Text.Replace("📄 ", "");
                    this.txtLessonDescription.Text = "Mô tả chi tiết của bài học...";
                }
            };

            this.treeContainerPanel.Controls.Add(this.tvContent);

            // detailPanel - Dùng Dock Fill để chiếm phần còn lại
            this.detailPanel.Dock = DockStyle.Fill;
            this.detailPanel.BackColor = Color.White;
            this.detailPanel.Padding = new Padding(25);
            this.detailPanel.AutoScroll = true;
            this.detailPanel.Margin = new Padding(15, 0, 0, 0);

            this.detailPanel.Paint += (s, e) =>
            {
                // Draw rounded rectangle border
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 8;
                    var rect = new Rectangle(0, 0, this.detailPanel.Width - 1, this.detailPanel.Height - 1);
                    path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                    path.CloseFigure();

                    using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            // lblDetailTitle
            this.lblDetailTitle.AutoSize = true;
            this.lblDetailTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblDetailTitle.ForeColor = Color.FromArgb(40, 40, 40);
            this.lblDetailTitle.Text = "Chi tiết bài học";
            this.lblDetailTitle.Location = new Point(25, 25);

            // lblLessonName
            this.lblLessonName.AutoSize = true;
            this.lblLessonName.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.lblLessonName.ForeColor = Color.FromArgb(60, 60, 60);
            this.lblLessonName.Text = "📝 Tên bài học:";
            this.lblLessonName.Location = new Point(25, 80);

            // txtLessonName
            this.txtLessonName.Location = new Point(25, 110);
            this.txtLessonName.Font = new Font("Segoe UI", 11F);
            this.txtLessonName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtLessonName.BorderStyle = BorderStyle.FixedSingle;
            this.txtLessonName.Width = this.detailPanel.Width - 50;

            // lblLessonDesc
            this.lblLessonDesc.AutoSize = true;
            this.lblLessonDesc.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.lblLessonDesc.ForeColor = Color.FromArgb(60, 60, 60);
            this.lblLessonDesc.Text = "📋 Mô tả nội dung:";
            this.lblLessonDesc.Location = new Point(25, 160);

            // txtLessonDescription
            this.txtLessonDescription.Location = new Point(25, 190);
            this.txtLessonDescription.Height = 150;
            this.txtLessonDescription.Font = new Font("Segoe UI", 10.5F);
            this.txtLessonDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtLessonDescription.BorderStyle = BorderStyle.FixedSingle;
            this.txtLessonDescription.Width = this.detailPanel.Width - 50;

            // lblVideoUrl
            this.lblVideoUrl.AutoSize = true;
            this.lblVideoUrl.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.lblVideoUrl.ForeColor = Color.FromArgb(60, 60, 60);
            this.lblVideoUrl.Text = "🎥 Link video bài giảng:";
            this.lblVideoUrl.Location = new Point(25, 360);

            // txtVideoUrl
            this.txtVideoUrl.Location = new Point(25, 390);
            this.txtVideoUrl.Font = new Font("Segoe UI", 11F);
            this.txtVideoUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtVideoUrl.BorderStyle = BorderStyle.FixedSingle;
            this.txtVideoUrl.Text = "https://youtube.com/...";
            this.txtVideoUrl.Width = this.detailPanel.Width - 50;

            // lblMaterials
            this.lblMaterials.AutoSize = true;
            this.lblMaterials.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.lblMaterials.ForeColor = Color.FromArgb(60, 60, 60);
            this.lblMaterials.Text = "📎 Tài liệu đính kèm:";
            this.lblMaterials.Location = new Point(25, 440);

            // txtMaterials
            this.txtMaterials.Location = new Point(25, 470);
            this.txtMaterials.Font = new Font("Segoe UI", 11F);
            this.txtMaterials.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtMaterials.BorderStyle = BorderStyle.FixedSingle;
            this.txtMaterials.Text = "Chọn file...";
            this.txtMaterials.ReadOnly = true;
            this.txtMaterials.Cursor = Cursors.Hand;
            this.txtMaterials.Width = this.detailPanel.Width - 50;
            this.txtMaterials.Click += (s, e) => MessageBox.Show("Chọn file tài liệu");

            // buttonPanel
            this.buttonPanel.Location = new Point(25, 530);
            this.buttonPanel.Size = new Size(500, 45);
            this.buttonPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // btnSave
            this.btnSave.Text = "💾 Lưu thay đổi";
            this.btnSave.Size = new Size(150, 40);
            this.btnSave.Location = new Point(0, 0);
            this.btnSave.BackColor = Color.FromArgb(76, 175, 80);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnSave.Cursor = Cursors.Hand;

            this.btnSave.MouseEnter += (s, e) => this.btnSave.BackColor = Color.FromArgb(56, 142, 60);
            this.btnSave.MouseLeave += (s, e) => this.btnSave.BackColor = Color.FromArgb(76, 175, 80);
            this.btnSave.Click += (s, e) => MessageBox.Show("Đã lưu thay đổi!");

            // btnAddLesson
            this.btnAddLesson.Text = "➕ Thêm bài học";
            this.btnAddLesson.Size = new Size(150, 40);
            this.btnAddLesson.Location = new Point(165, 0);
            this.btnAddLesson.BackColor = Color.FromArgb(33, 150, 243);
            this.btnAddLesson.ForeColor = Color.White;
            this.btnAddLesson.FlatStyle = FlatStyle.Flat;
            this.btnAddLesson.FlatAppearance.BorderSize = 0;
            this.btnAddLesson.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnAddLesson.Cursor = Cursors.Hand;

            this.btnAddLesson.MouseEnter += (s, e) => this.btnAddLesson.BackColor = Color.FromArgb(25, 118, 210);
            this.btnAddLesson.MouseLeave += (s, e) => this.btnAddLesson.BackColor = Color.FromArgb(33, 150, 243);
            this.btnAddLesson.Click += (s, e) => MessageBox.Show("Thêm bài học mới");

            // btnDelete
            this.btnDelete.Text = "🗑️ Xóa";
            this.btnDelete.Size = new Size(120, 40);
            this.btnDelete.Location = new Point(330, 0);
            this.btnDelete.BackColor = Color.FromArgb(244, 67, 54);
            this.btnDelete.ForeColor = Color.White;
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnDelete.Cursor = Cursors.Hand;

            this.btnDelete.MouseEnter += (s, e) => this.btnDelete.BackColor = Color.FromArgb(229, 57, 53);
            this.btnDelete.MouseLeave += (s, e) => this.btnDelete.BackColor = Color.FromArgb(244, 67, 54);
            this.btnDelete.Click += (s, e) =>
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa bài học này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Đã xóa bài học!");
                }
            };

            this.buttonPanel.Controls.Add(this.btnSave);
            this.buttonPanel.Controls.Add(this.btnAddLesson);
            this.buttonPanel.Controls.Add(this.btnDelete);

            this.detailPanel.Controls.Add(this.lblDetailTitle);
            this.detailPanel.Controls.Add(this.lblLessonName);
            this.detailPanel.Controls.Add(this.txtLessonName);
            this.detailPanel.Controls.Add(this.lblLessonDesc);
            this.detailPanel.Controls.Add(this.txtLessonDescription);
            this.detailPanel.Controls.Add(this.lblVideoUrl);
            this.detailPanel.Controls.Add(this.txtVideoUrl);
            this.detailPanel.Controls.Add(this.lblMaterials);
            this.detailPanel.Controls.Add(this.txtMaterials);
            this.detailPanel.Controls.Add(this.buttonPanel);

            this.mainContentPanel.Controls.Add(this.detailPanel);
            this.mainContentPanel.Controls.Add(this.treeContainerPanel);

            this.Controls.Add(this.mainContentPanel);
            this.Controls.Add(this.headerPanel);

            this.ResumeLayout(false);
        }
    }
}
