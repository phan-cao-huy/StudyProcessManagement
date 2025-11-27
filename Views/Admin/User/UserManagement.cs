using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using StudyProcessManagement.Business.Admin;
using StudyProcessManagement.Models;
using StudyProcessManagement.Views.Admin.Student.StudentManagement;
using StudyProcessManagement.Views.Admin.Teacher.TeacherMangement;
using Excel = Microsoft.Office.Interop.Excel;
namespace StudyProcessManagement.Views.Admin.User
{
    public partial class UserManagement : Form
    {
        private UserService userService = new UserService();
        private List<Users> _currentUserList;
        public UserManagement()
        {
            InitializeComponent();
            // Set cứng kích thước form
            this.Size = new System.Drawing.Size(1324, 673);
        }

        private void UserManagement_Load(object sender, EventArgs e)
        {
            // Đăng ký sự kiện
            //this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            //this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            //this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged_1);
            //this.dataGridViewUsers.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUsers_CellContentClick_1);

            // Tải dữ liệu lên bảng
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                dataGridViewUsers.Rows.Clear();
                string keyword = txtSearch.Text.Trim();
                if (keyword == "Tìm kiếm người dùng...") keyword = "";

                List<Users> list = userService.GetAllUsers(keyword);
                _currentUserList = list;
                if (list.Count == 0)
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        MessageBox.Show($"Không tìm thấy người dùng nào với từ khóa: '{keyword}'",
                                        "Kết quả tìm kiếm",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    return;
                }
                foreach (var u in list)
                {
                    int index = dataGridViewUsers.Rows.Add(
                        u.UserID,
                        u.FullName,
                        u.Email,
                        u.Role,
                        u.StatusText
                    );

                    if (u.IsActive)
                        dataGridViewUsers.Rows[index].Cells["colStatus"].Style.ForeColor = Color.Green;
                    else
                        dataGridViewUsers.Rows[index].Cells["colStatus"].Style.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

   
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            OpenDetailForm(null);
        }

        private void dataGridViewUsers_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridViewUsers.Rows.Count) return;

            // 2. Lấy dòng hiện tại
            DataGridViewRow row = dataGridViewUsers.Rows[e.RowIndex];

            // 3. Lấy ID và Tên (DÙNG SỐ THỨ TỰ CỘT THAY VÌ TÊN)
    
            string userId = row.Cells[0].Value?.ToString();
            string userName = row.Cells[1].Value?.ToString();

            // Kiểm tra nếu ID null (dòng lỗi) thì bỏ qua
            if (string.IsNullOrEmpty(userId)) return;

            // --- XỬ LÝ NÚT XÓA ---
            // Kiểm tra xem có phải bấm vào cột nút Xóa không?
   
            if (dataGridViewUsers.Columns[e.ColumnIndex].Name == "colDelete")
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa user '{userName}' (ID: {userId})?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (userService.DeleteUser(userId))
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo");
                        LoadUserData();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            // --- XỬ LÝ NÚT SỬA ---
            else if (dataGridViewUsers.Columns[e.ColumnIndex].Name == "colEdit")
            {
               if(MessageBox.Show($"Bạn có chắc muốn sửa user '{userName}' (ID: {userId})?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    OpenDetailForm(userId);
                }
               else
                {
                    return;
                }
            }
            // --- XỬ LÝ NÚT XEM ---
            else if (dataGridViewUsers.Columns[e.ColumnIndex].Name == "colView")
            {
                UserDetailForm form = new UserDetailForm(userId, true);
                form.ShowDialog();
            }
        }

        // Hàm mở Form chi tiết (Dùng chung cho Thêm & Sửa)
        private void OpenDetailForm(string userId)
        {
            UserDetailForm form = new UserDetailForm(userId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadUserData();
            }
        }

        // Tìm kiếm
        private void txtSearch_TextChanged_1(object sender, EventArgs e)
        {

        }

        // Các hàm placeholder UI
        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Tìm kiếm người dùng...") { txtSearch.Text = ""; txtSearch.ForeColor = Color.Black; }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Tìm kiếm người dùng..."; txtSearch.ForeColor = Color.Gray; }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel đang phát triển...");
        }

        // Chuyển trang (Menu)
        private void btnMenuTeachers_Click(object sender, EventArgs e)
        {
            var teacher = new TeacherMangement();
            this.Hide();
            teacher.Show();
        }

        private void btnMenuStudents_Click(object sender, EventArgs e)
        {
            var studentForm = new StudentManagement();
            this.Hide();
            studentForm.Show();
        }

        private void lblSearchIcon_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadUserData();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
          
        }
        // --- PHẦN XUẤT EXCEL (Microsoft.Office.Interop.Excel) ---

        private void ExportToExcelInterop()
        {
            if (_currentUserList == null || _currentUserList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu người dùng để xuất ra Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 1. Dùng SaveFileDialog để lấy đường dẫn và tên file từ người dùng
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                Title = "Lưu danh sách Người dùng",
                FileName = $"DanhSachNguoiDung_{DateTime.Now:yyyyMMdd}.xlsx"
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

                try
                {
                    excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    workbook = excelApp.Workbooks.Add(Type.Missing);
                    worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                    int excelRow = 1;

                    // 2. Ghi Header (Tiêu đề cột)
                    worksheet.Cells[excelRow, 1] = "Mã người dùng";
                    worksheet.Cells[excelRow, 2] = "Họ và tên";
                    worksheet.Cells[excelRow, 3] = "Email";
                    worksheet.Cells[excelRow, 4] = "Vai trò";
                    worksheet.Cells[excelRow, 5] = "Trạng thái";

                    // Định dạng tiêu đề
                    worksheet.Range["A1", "E1"].Font.Bold = true;

                    // 3. Ghi Dữ liệu từ List<Users>
                    excelRow++; // Bắt đầu từ hàng 2

                    foreach (var user in _currentUserList)
                    {
                        worksheet.Cells[excelRow, 1] = user.UserID;
                        worksheet.Cells[excelRow, 2] = user.FullName;
                        worksheet.Cells[excelRow, 3] = user.Email;
                        worksheet.Cells[excelRow, 4] = user.Role;
                        worksheet.Cells[excelRow, 5] = user.StatusText;

                        // Tô màu trạng thái
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

                    // 4. Định dạng và LƯU FILE
                    worksheet.UsedRange.Columns.AutoFit();

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

                    // Đóng workbook và thoát Excel App
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

        private void btnExportExcel_Click_1(object sender, EventArgs e)
        {
            ExportToExcelInterop();
        }
    }
}