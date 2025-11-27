using System;
using System.Data;
using System.IO;  // THÊM DÒNG NÀY
using System.Windows.Forms;
using StudyProcessManagement.Business.Teacher;

namespace StudyProcessManagement.Views.Teacher.Forms
{
    public partial class AssignmentForm : Form
    {
        private AssignmentFormService assignmentFormService;
        private string currentTeacherID = "USR002";
        private int assignmentID;
        private bool isEditMode = false;

        // THÊM 2 biến lưu file
        private byte[] selectedFileData = null;
        private string selectedFileName = null;

        public AssignmentForm()
        {
            InitializeComponent();
            assignmentFormService = new AssignmentFormService();
            this.Text = "Tạo bài tập mới";
            isEditMode = false;
            dtpAssignedDate.Value = DateTime.Now;
            dtpDueDate.Value = DateTime.Now.AddDays(7);
            numMaxScore.Maximum = 10;
            numMaxScore.Minimum = 0;
            LoadCourses();
        }

        public AssignmentForm(int assignmentID)
        {
            InitializeComponent();
            assignmentFormService = new AssignmentFormService();
            this.assignmentID = assignmentID;
            this.Text = "Chỉnh sửa bài tập";
            isEditMode = true;
            numMaxScore.Maximum = 10;
            numMaxScore.Minimum = 0;
            LoadCourses();
            LoadAssignment(assignmentID);
        }

        private void LoadCourses()
        {
            DataTable dt = assignmentFormService.GetTeacherCourses(currentTeacherID);
            cboCourse.DataSource = dt;
            cboCourse.DisplayMember = "CourseName";
            cboCourse.ValueMember = "CourseID";
        }

        private void LoadAssignment(int id)
        {
            DataTable dt = assignmentFormService.GetAssignmentDetails(id);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                cboCourse.SelectedValue = Convert.ToInt32(row["CourseID"]);
                txtTitle.Text = row["Title"].ToString();
                txtDescription.Text = row["Description"].ToString();
                dtpAssignedDate.Value = Convert.ToDateTime(row["AssignedDate"]);
                dtpDueDate.Value = Convert.ToDateTime(row["DueDate"]);
                numMaxScore.Value = Convert.ToDecimal(row["MaxScore"]);

                // Load file đính kèm nếu có
                if (row["AttachmentName"] != DBNull.Value &&
    !string.IsNullOrEmpty(row["AttachmentName"].ToString()))
                {
                    selectedFileName = row["AttachmentName"].ToString();
                    txtAttachment.Text = selectedFileName;

                    if (row["AttachmentData"] != DBNull.Value)
                        selectedFileData = (byte[])row["AttachmentData"];
                    else
                        selectedFileData = null;
                }
                else
                {
                    txtAttachment.Text = "";
                    selectedFileData = null;
                    selectedFileName = null;
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin bài tập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        // SỬA HÀM NÀY
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Tất cả file|*.*|PDF|*.pdf|Word|*.docx;*.doc";
            ofd.Title = "Chọn file đính kèm";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Kiểm tra kích thước file (giới hạn 10MB)
                    long fileSize = new FileInfo(ofd.FileName).Length;
                    if (fileSize > 10 * 1024 * 1024)
                    {
                        MessageBox.Show("File không được vượt quá 10MB!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Đọc file thành byte[]
                    selectedFileData = File.ReadAllBytes(ofd.FileName);
                    selectedFileName = Path.GetFileName(ofd.FileName);
                    txtAttachment.Text = selectedFileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đọc file: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    selectedFileData = null;
                    selectedFileName = null;
                }
            }
        }

        // SỬA HÀM NÀY
        private void BtnSave_Click(object sender, EventArgs e)
        {
            int courseID = Convert.ToInt32(cboCourse.SelectedValue);
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            DateTime assignedDate = dtpAssignedDate.Value;
            DateTime dueDate = dtpDueDate.Value;
            decimal maxScore = numMaxScore.Value;

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Tên bài tập không được trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate xác nhận
            DialogResult confirm;
            if (isEditMode)
            {
                confirm = MessageBox.Show("Bạn có chắc chắn muốn sửa bài tập?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {
                confirm = MessageBox.Show("Bạn có chắc chắn muốn tạo bài tập mới?", "Xác nhận tạo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            if (confirm == DialogResult.No)
                return;

            try
            {
                bool result = false;
                if (isEditMode)
                {
                    result = assignmentFormService.UpdateAssignment(
                        assignmentID, courseID, title, description, assignedDate, dueDate, maxScore,
                        selectedFileData, selectedFileName  // Truyền file data
                    );
                }
                else
                {
                    result = assignmentFormService.CreateAssignment(
                        courseID, title, description, assignedDate, dueDate, maxScore,
                        selectedFileData, selectedFileName  // Truyền file data
                    ) > 0;
                }

                if (result)
                {
                    MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lưu thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}