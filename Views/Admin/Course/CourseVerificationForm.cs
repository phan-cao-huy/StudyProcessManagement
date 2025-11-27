using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using StudyProcessManagement.Business.Admin;

namespace StudyProcessManagement.Views.Admin.Course
{
    public partial class CourseVerificationForm : Form
    {
        private ContentService contentService;
        private string _courseId;

        // Biến lưu trữ file đính kèm
        private byte[] currentAttachmentData = null;
        private string currentAttachmentName = "";

        // Constructor mặc định (Bắt buộc cho Designer)
        public CourseVerificationForm()
        {
            InitializeComponent();

            // CHẶN DESIGNER - Không làm gì thêm nếu đang trong chế độ thiết kế
            if (this.DesignMode) return;
        }

        // Constructor có tham số (Dùng khi chạy ứng dụng)
        public CourseVerificationForm(string courseId, string courseName) : this()
        {
            // CHẶN nếu đang trong Design Mode (an toàn kép)
            if (this.DesignMode) return;

            // Khởi tạo Service
            contentService = new ContentService();
            _courseId = courseId;

            // Cập nhật tiêu đề
            if (!string.IsNullOrEmpty(courseName))
            {
                lblTitle.Text = $"DUYỆT: {courseName.ToUpper()}";
            }

            // Load dữ liệu cây thư mục (Section/Lesson)
            if (!string.IsNullOrEmpty(_courseId))
            {
                LoadTreeData();
            }

            // Gán sự kiện
            // Lưu ý: Các sự kiện UI cơ bản thường đã được gán trong InitializeComponent() (Designer.cs)
            // Chỉ gán thủ công nếu Designer chưa gán hoặc bạn muốn override.

            // Đảm bảo sự kiện LinkClicked được gán
            lnkAttachment.LinkClicked += LnkAttachment_LinkClicked;
        }

        private void LoadTreeData()
        {
            try
            {
                tvContent.Nodes.Clear();
                // Gọi Service lấy danh sách Section
                DataTable dtSections = contentService.GetSections(_courseId);

                foreach (DataRow secRow in dtSections.Rows)
                {
                    string secId = secRow["SectionID"].ToString();
                    TreeNode secNode = new TreeNode($"📂 {secRow["SectionTitle"]}");
                    secNode.Tag = "SECTION"; // Đánh dấu node cha

                    // Gọi Service lấy danh sách Lesson cho Section này
                    DataTable dtLessons = contentService.GetLessons(secId);
                    foreach (DataRow lesRow in dtLessons.Rows)
                    {
                        TreeNode lesNode = new TreeNode($"📄 {lesRow["LessonTitle"]}");
                        lesNode.Tag = lesRow["LessonID"].ToString(); // Lưu LessonID vào Tag
                        secNode.Nodes.Add(lesNode);
                    }
                    tvContent.Nodes.Add(secNode);
                }
                tvContent.ExpandAll();
            }
            catch (Exception ex)
            {
                if (!this.DesignMode)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        // Xử lý khi chọn một Node trong TreeView
        private void TvContent_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Nếu chọn Section hoặc không có Tag -> Xóa chi tiết cũ
            if (e.Node.Tag == null || e.Node.Tag.ToString() == "SECTION")
            {
                ClearDetail();
                return;
            }

            // Nếu chọn Lesson -> Tải chi tiết
            string lessonId = e.Node.Tag.ToString();

            // Gọi Service lấy chi tiết bài học (bao gồm cả file đính kèm)
            DataRow lesson = contentService.GetLessonDetail(lessonId);

            if (lesson != null)
            {
                txtLessonName.Text = lesson["LessonTitle"].ToString();
                txtContent.Text = lesson["Content"].ToString();
                txtVideo.Text = lesson["VideoUrl"].ToString();

                // ✅ XỬ LÝ FILE ĐÍNH KÈM
                if (lesson["AttachmentName"] != DBNull.Value && !string.IsNullOrEmpty(lesson["AttachmentName"].ToString()))
                {
                    currentAttachmentName = lesson["AttachmentName"].ToString();

                    // Kiểm tra null trước khi ép kiểu
                    if (lesson["AttachmentData"] != DBNull.Value)
                    {
                        currentAttachmentData = (byte[])lesson["AttachmentData"];
                    }
                    else
                    {
                        currentAttachmentData = null;
                    }

                    // Hiển thị link tải file
                    lnkAttachment.Text = $"📎 {currentAttachmentName}";
                    lnkAttachment.Visible = true;
                    lblAttachment.Visible = true;
                }
                else
                {
                    // Không có file -> Ẩn link
                    currentAttachmentName = "";
                    currentAttachmentData = null;
                    lnkAttachment.Visible = false;
                    lblAttachment.Visible = false;
                }
            }
        }

        // Xử lý sự kiện click vào link file để tải về/mở
        private void LnkAttachment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (currentAttachmentData == null || currentAttachmentData.Length == 0)
            {
                MessageBox.Show("File rỗng hoặc lỗi dữ liệu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Tạo đường dẫn file tạm thời trong thư mục Temp của hệ thống
                string tempPath = Path.Combine(Path.GetTempPath(), currentAttachmentName);

                // Ghi mảng byte ra file
                File.WriteAllBytes(tempPath, currentAttachmentData);

                // Mở file bằng ứng dụng mặc định của Windows
                System.Diagnostics.Process.Start(tempPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xóa trắng các trường thông tin chi tiết
        private void ClearDetail()
        {
            txtLessonName.Clear();
            txtContent.Clear();
            txtVideo.Clear();
            lnkAttachment.Visible = false;
            lblAttachment.Visible = false;
            currentAttachmentData = null;
        }

        // Xử lý nút Duyệt
        private void BtnApprove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_courseId)) return;

            if (MessageBox.Show("Xác nhận DUYỆT khóa học này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Gọi Service duyệt khóa học
                if (contentService.ApproveCourse(_courseId))
                {
                    MessageBox.Show("Đã duyệt thành công!");
                    this.DialogResult = DialogResult.OK; // Trả về OK để form cha biết mà reload lại danh sách
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lỗi khi duyệt! Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Các event handler rỗng (có thể xóa nếu không dùng)
        private void label1_Click(object sender, EventArgs e) { }
        private void CourseVerificationForm_Load(object sender, EventArgs e) { }
    }
}