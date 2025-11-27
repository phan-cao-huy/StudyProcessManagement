using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using StudyProcessManagement.Business.Admin;
using StudyProcessManagement.Models;
using StudyProcessManagement.Views.Admin.User;
using Excel = Microsoft.Office.Interop.Excel;
namespace StudyProcessManagement.Views.Admin.Teacher.TeacherMangement
{
    public partial class TeacherMangement : Form
    {
       
        private TeacherService teacherService = new TeacherService();
        private List<Users> _currentTeacherList;
        public TeacherMangement()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(1324, 673);

            // Tải dữ liệu ngay khi mở
            LoadTeacherData();
        }

        private void TeacherMangement_Load(object sender, EventArgs e)
        {
            
        }

        
        private void LoadTeacherData()
        {
            try
            {
                dataGridViewUsers.Rows.Clear();
                string keyword = txtSearch.Text.Trim();
                if (keyword == "Tìm kiếm người dùng...") keyword = "";

                // Gọi Service lấy danh sách Teacher
                List<Users> list = teacherService.GetAllTeachers(keyword);
                _currentTeacherList = list;
                // (Đoạn kiểm tra rỗng này có thể bỏ nếu muốn form luôn hiện trống khi ko có data)
                // if (list.Count == 0 && !string.IsNullOrEmpty(keyword)) { ... }

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
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // 1. NÚT THÊM MỚI
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            UserDetailForm form = new UserDetailForm(null, false);

           
            form.cboRole.SelectedItem = "Teacher";
            form.cboRole.Enabled = false;

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadTeacherData(); 
            }
        }
        private void dataGridViewUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0 || e.RowIndex >= dataGridViewUsers.Rows.Count) return;

            DataGridViewRow row = dataGridViewUsers.Rows[e.RowIndex];

            // Lấy ID và Tên (Dùng chỉ số cột 0 và 1 cho chắc ăn)
            string userId = row.Cells[0].Value?.ToString();
            string userName = row.Cells[1].Value?.ToString();

            if (string.IsNullOrEmpty(userId)) return;

            // --- XÓA ---
            if (dataGridViewUsers.Columns[e.ColumnIndex].Name == "colDelete")
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa giảng viên '{userName}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (teacherService.DeleteTeacher(userId)) // Gọi TeacherService
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo");
                        LoadTeacherData();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại!", "Lỗi");
                    }
                }
            }
         
            else if (dataGridViewUsers.Columns[e.ColumnIndex].Name == "colEdit")
            {
                OpenDetailForm(userId);
            }
           
            else if (dataGridViewUsers.Columns[e.ColumnIndex].Name == "colView")
            {
                UserDetailForm form = new UserDetailForm(userId, true);
                form.ShowDialog();
            }
        }

       
        private void OpenDetailForm(string userId)
        {
            UserDetailForm form = new UserDetailForm(userId, false);

            // Khi sửa cũng khóa Role lại luôn cho chắc
            form.cboRole.Enabled = false;

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadTeacherData();
            }
        }

       
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text != "Tìm kiếm người dùng...") LoadTeacherData();
        }

        private void lblSearchIcon_Click(object sender, EventArgs e)
        {
            LoadTeacherData();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadTeacherData();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        // Placeholder
        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Tìm kiếm người dùng...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Tìm kiếm người dùng...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e) {  }
        // --- PHẦN XUẤT EXCEL (Microsoft.Office.Interop.Excel) ---

        private void ExportToExcelInterop()
        {
            if (_currentTeacherList == null || _currentTeacherList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu giảng viên để xuất ra Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 1. Dùng SaveFileDialog để lấy đường dẫn và tên file từ người dùng
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                Title = "Lưu danh sách Giảng viên",
                FileName = $"DanhSachGiangVien_{DateTime.Now:yyyyMMdd}.xlsx"
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
                    worksheet.Cells[excelRow, 1] = "Mã giảng viên";
                    worksheet.Cells[excelRow, 2] = "Họ và tên";
                    worksheet.Cells[excelRow, 3] = "Email";
                    worksheet.Cells[excelRow, 4] = "Vai trò";
                    worksheet.Cells[excelRow, 5] = "Trạng thái";

                    // Định dạng tiêu đề
                    worksheet.Range["A1", "E1"].Font.Bold = true;

                    // 3. Ghi Dữ liệu từ List<Users> đã được lọc
                    excelRow++; // Bắt đầu từ hàng 2

                    foreach (var user in _currentTeacherList)
                    {
                        worksheet.Cells[excelRow, 1] = user.UserID;
                        worksheet.Cells[excelRow, 2] = user.FullName;
                        worksheet.Cells[excelRow, 3] = user.Email;
                        worksheet.Cells[excelRow, 4] = user.Role;
                        worksheet.Cells[excelRow, 5] = user.StatusText;

                        // Tô màu trạng thái (Dùng ColorTranslator cho Interop)
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