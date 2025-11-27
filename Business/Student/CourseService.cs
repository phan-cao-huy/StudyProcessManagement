using StudyProcessManagement.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using StudyProcessManagement.Business; // Import namespace chứa DataProcessDAL

namespace StudyProcessManagement.Business.Student
{
    public class CourseService
    {
        // Sử dụng DataProcessDAL (Generic DAL) để truy cập DB
        private readonly DataProcessDAL _db = new DataProcessDAL();

        public List<StudentCourseViewModel> GetMyCourses(string studentId)
        {
            List<StudentCourseViewModel> courses = new List<StudentCourseViewModel>();

            // 1. Chuyển đổi ID sang INT (Logic nghiệp vụ)
            if (!int.TryParse(studentId, out int studentIntId))
            {
                MessageBox.Show("ID sinh viên không hợp lệ.", "Lỗi Dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return courses;
            }

            string spName = "sp_GetMyCoursesWithProgress";

            // 2. Định nghĩa tham số cho Stored Procedure
            var parameters = new Dictionary<string, object>
            {
                { "@StudentID", studentIntId }
            };

            DataTable dt = null;

            try
            {
                // 3. GỌI DAL: Sử dụng ExecuteStoredProcedure của DataProcessDAL
                // Lớp DAL (DataProcessDAL) chịu trách nhiệm mở/đóng kết nối và thực thi truy vấn.
                dt = _db.ExecuteStoredProcedure(spName, parameters);
            }
            catch (Exception ex)
            {
                // Bắt lỗi từ DAL
                MessageBox.Show("Lỗi truy vấn SQL khi lấy danh sách khóa học: " + ex.Message, "Lỗi DAL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return courses;
            }

            // 4. Ánh xạ (Mapping) DataTable sang List<StudentCourseViewModel> (Giữ nguyên logic ánh xạ)
            if (dt != null)
            {
                foreach (DataRow reader in dt.Rows)
                {
                    try
                    {
                        courses.Add(new StudentCourseViewModel
                        {
                            // THÔNG TIN KHÓA HỌC
                            CourseID = reader["CourseID"].ToString(),
                            TenKhoaHoc = reader["TenKhoaHoc"].ToString(),

                            // XỬ LÝ NULL STRING
                            AnhBia = reader["AnhBia"] == DBNull.Value ? null : reader["AnhBia"].ToString(),
                            MoTa = reader["MoTa"] == DBNull.Value ? null : reader["MoTa"].ToString(),

                            // DANH MỤC (Giả định không NULL)
                            DanhMuc = reader["DanhMuc"].ToString(),
                            CategoryID = reader["CategoryID"].ToString(),

                            // GIẢNG VIÊN
                            TenGiangVien = reader["TenGiangVien"].ToString(),
                            AnhGiangVien = reader["AnhGiangVien"] == DBNull.Value ? null : reader["AnhGiangVien"].ToString(),

                            // TIẾN ĐỘ (Giả định không NULL hoặc đã xử lý ISNULL trong SP)
                            TienDoHoc = Convert.ToInt32(reader["TienDoHoc"]),
                            SoBaiHoanThanh = Convert.ToInt32(reader["SoBaiHoanThanh"]),
                            TongSoBaiHoc = Convert.ToInt32(reader["TongSoBaiHoc"]),
                            SoBaiConLai = Convert.ToInt32(reader["SoBaiConLai"]),

                            // TIẾN ĐỘ TEXT (Giả định không NULL)
                            TienDoText = reader["TienDoText"].ToString(),
                            PhanTramText = reader["PhanTramText"].ToString(),
                            ProgressColor = reader["ProgressColor"].ToString(),

                            // TRẠNG THÁI & ĐIỂM
                            TrangThai = reader["TrangThai"] == DBNull.Value ? null : reader["TrangThai"].ToString(),
                            TrangThaiText = reader["TrangThaiText"].ToString(),

                            // XỬ LÝ DATETIME NULL CHECK
                            NgayDangKy = reader["NgayDangKy"] == DBNull.Value
                                             ? (DateTime?)null
                                             : (DateTime)reader["NgayDangKy"],

                            // DiemTrungBinh (Giả định không NULL hoặc đã xử lý ISNULL trong SP)
                            DiemTrungBinh = Convert.ToDecimal(reader["DiemTrungBinh"])
                        });
                    }
                    catch (Exception ex)
                    {
                        // Bắt lỗi ép kiểu dữ liệu
                        MessageBox.Show("Lỗi ánh xạ dữ liệu khi đọc danh sách khóa học: " + ex.Message, "Lỗi Ánh Xạ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }
            return courses;
        }
    }
}