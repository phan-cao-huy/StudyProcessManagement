using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using StudyProcessManagement.Business.Admin;
using StudyProcessManagement.Models;
using StudyProcessManagement.Views.Admin.Teacher.TeacherMangement; 
using StudyProcessManagement.Views.Admin.User;
using OfficeOpenXml; 
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
namespace StudyProcessManagement.Views.Admin.Student.StudentManagement
{
    public partial class StudentManagement : Form
    {
        // Khởi tạo Service
        private StudentService studentService = new StudentService();
        private List<Users> _currentStudentList;
        public StudentManagement()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(1324, 673);

            // Tải dữ liệu ngay khi mở form
            LoadStudentData();
        }

        private void StudentManagement_Load(object sender, EventArgs e)
        {
            // Không cần làm gì thêm
        }

        // --- HÀM TẢI DỮ LIỆU ---
        private void LoadStudentData()
        {
            try
            {
                dataGridViewUsers.Rows.Clear();
                string keyword = txtSearch.Text.Trim();
                if (txtSearch.ForeColor == Color.Gray)
                {
                    keyword = "";
                }

                // Gọi Service
                List<Users> list = studentService.GetAllStudents(keyword);
                _currentStudentList = list;
                if (keyword == "Tìm kiếm học viên...") keyword = "";

                foreach (var u in list)
                {
                    int index = dataGridViewUsers.Rows.Add(
                        u.UserID,
                        u.FullName,
                        u.Email,
                        u.Role,
                        u.StatusText
                    );

                    // Tô màu trạng thái
                    if (u.IsActive)
                        dataGridViewUsers.Rows[index].Cells["colStatus"].Style.ForeColor = Color.Green;
                    else
                        dataGridViewUsers.Rows[index].Cells["colStatus"].Style.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // --- 1. NÚT THÊM HỌC VIÊN ---
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            UserDetailForm form = new UserDetailForm(null, false);

            // Thiết lập mặc định: Role là Student và Khóa lại không cho sửa
            form.cboRole.SelectedItem = "Student";
            form.cboRole.Enabled = false;

            if (form.ShowDialog() == DialogResult.OK) LoadStudentData();
        }

        // --- 2. XỬ LÝ SỰ KIỆN TRÊN BẢNG (XÓA / SỬA / XEM) ---
        private void dataGridViewUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Bỏ qua header
            if (e.RowIndex < 0 || e.RowIndex >= dataGridViewUsers.Rows.Count) return;

            DataGridViewRow row = dataGridViewUsers.Rows[e.RowIndex];
            string userId = row.Cells[0].Value?.ToString();
            string userName = row.Cells[1].Value?.ToString();

            if (string.IsNullOrEmpty(userId)) return;

            // XÓA
            if (dataGridViewUsers.Columns[e.ColumnIndex].Name == "colDelete")
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa học viên '{userName}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (studentService.DeleteStudent(userId))
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo");
                        LoadStudentData();
                    }
                    else MessageBox.Show("Xóa thất bại!", "Lỗi");
                }
            }
            // SỬA
            else if (dataGridViewUsers.Columns[e.ColumnIndex].Name == "colEdit")
            {
                OpenDetailForm(userId);
            }
            // XEM CHI TIẾT
            else if (dataGridViewUsers.Columns[e.ColumnIndex].Name == "colView")
            {
                UserDetailForm form = new UserDetailForm(userId, true);
                form.ShowDialog();
            }
        }

        // Hàm phụ trợ mở form sửa
        private void OpenDetailForm(string userId)
        {
            UserDetailForm form = new UserDetailForm(userId, false);
            form.cboRole.Enabled = false; // Khóa role khi sửa luôn (Student thì mãi là Student)
            if (form.ShowDialog() == DialogResult.OK) LoadStudentData();
        }

        // --- 3. TÌM KIẾM ---
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text != "Tìm kiếm học viên...") LoadStudentData();
        }

        private void lblSearchIcon_Click(object sender, EventArgs e)
        {
            LoadStudentData();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadStudentData();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        // --- 4. PLACEHOLDER UI ---
        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Tìm kiếm học viên...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }
        private void ExportToExcelInterop()
        {
            if (_currentStudentList == null || _currentStudentList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu học viên để xuất ra Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 1. Dùng SaveFileDialog để lấy đường dẫn và tên file từ người dùng
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                Title = "Lưu danh sách Học viên",
                FileName = $"DanhSachHocVien_{DateTime.Now:yyyyMMdd}.xlsx" // Tên file mặc định
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return; // Người dùng hủy bỏ
                }

                // --- BẮT ĐẦU XUẤT EXCEL ---

                Excel.Application excelApp = null;
                Excel.Workbook workbook = null;
                Excel.Worksheet worksheet = null;
                Excel.Range range = null;

                try
                {
                    excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    workbook = excelApp.Workbooks.Add(Type.Missing);
                    worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                    int excelRow = 1;
                    int excelCol = 1;

                    // 2. Ghi Header (Tiêu đề cột)
                    // (Đang dùng header cố định vì xuất từ List<Users> đã lọc)
                    worksheet.Cells[excelRow, 1] = "Mã học viên";
                    worksheet.Cells[excelRow, 2] = "Họ và tên";
                    worksheet.Cells[excelRow, 3] = "Email";
                    worksheet.Cells[excelRow, 4] = "Vai trò";
                    worksheet.Cells[excelRow, 5] = "Trạng thái";

                    // Định dạng tiêu đề
                    worksheet.Range["A1", "E1"].Font.Bold = true;

                    // 3. Ghi Dữ liệu từ List<Users> đã được lọc
                    excelRow++; // Bắt đầu từ hàng 2

                    foreach (var user in _currentStudentList)
                    {
                        worksheet.Cells[excelRow, 1] = user.UserID;
                        worksheet.Cells[excelRow, 2] = user.FullName;
                        worksheet.Cells[excelRow, 3] = user.Email;
                        worksheet.Cells[excelRow, 4] = user.Role;
                        worksheet.Cells[excelRow, 5] = user.StatusText;

                        // Tô màu trạng thái (Tùy chọn)
                        if (user.IsActive)
                        {
                            worksheet.Cells[excelRow, 5].Font.Color = ColorTranslator.ToOle(Color.Green);
                        }
                        else
                        {
                            worksheet.Cells[excelRow, 5].Font.Color = ColorTranslator.ToOle(Color.Red);
                        }

                        excelRow++;
                    }

                    // 4. Định dạng và LƯU FILE SỬ DỤNG ĐƯỜNG DẪN CỦA SaveFileDialog
                    range = worksheet.UsedRange;
                    range.Columns.AutoFit();

                    // Sử dụng sfd.FileName là tên và đường dẫn file đã chọn
                    workbook.SaveAs(
                        sfd.FileName,
                        Excel.XlFileFormat.xlOpenXMLWorkbook,
                        Type.Missing,
                        Type.Missing,
                        false,
                        false,
                        Excel.XlSaveAsAccessMode.xlNoChange,
                        Type.Missing,
                        Type.Missing,
                        Type.Missing,
                        Type.Missing,
                        Type.Missing
                    );

                    // Đóng workbook sau khi lưu và giải phóng tài nguyên
                    workbook.Close(false);
                    excelApp.Quit();

                    MessageBox.Show($"Xuất Excel thành công!\nĐã lưu tại: {sfd.FileName}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi, đảm bảo Excel được đóng và giải phóng
                    if (workbook != null) workbook.Close(false);
                    if (excelApp != null) excelApp.Quit();
                    MessageBox.Show("Lỗi khi xuất file Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Luôn giải phóng các đối tượng COM
                    ReleaseObject(worksheet);
                    ReleaseObject(workbook);
                    ReleaseObject(excelApp);
                }
            }
        }
        // Hàm hỗ trợ giải phóng đối tượng COM
        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Tìm kiếm học viên...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tính năng đang phát triển...");
        }

        // Chuyển trang sang Teacher (nếu cần)
        private void btnMenuTeachers_Click(object sender, EventArgs e)
        {
            var teacher = new TeacherMangement();
            this.Hide();
            teacher.Show();
        }

        private void btnExportExcel_Click_1(object sender, EventArgs e)
        {
            ExportToExcelInterop();
        }
    }
}