using StudyProcessManagement.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using StudyProcessManagement.Business; // Import namespace chứa DataProcessDAL (nếu cần)

namespace StudyProcessManagement.Business.Student
{
    public class ContentService
    {
        // 1. Khai báo và sử dụng DAL
        private readonly DataProcessDAL _db = new DataProcessDAL();

        public List<StudentContent> GetAllResources(string studentId)
        {
            List<StudentContent> resources = new List<StudentContent>();
            string spName = "sp_GetAllResourcesForStudent";

            // 2. Định nghĩa tham số cho Stored Procedure
            var parameters = new Dictionary<string, object>
            {
                // Truyền StudentID trực tiếp
                { "@StudentID", studentId }
            };

            DataTable dt = null;

            try
            {
                // 3. GỌI DAL: Thực thi Stored Procedure và nhận DataTable
                dt = _db.ExecuteStoredProcedure(spName, parameters);
            }
            catch (Exception ex)
            {
                // Bắt lỗi từ DAL (Lỗi SQL hoặc lỗi kết nối)
                MessageBox.Show("Lỗi truy vấn SQL khi lấy tài nguyên: " + ex.Message, "Lỗi DAL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return resources;
            }

            // 4. Ánh xạ (Mapping) DataTable sang List<StudentContent>
            if (dt != null)
            {
                // Lặp qua các hàng trong DataTable (DataRow)
                foreach (DataRow reader in dt.Rows)
                {
                    try
                    {
                        resources.Add(new StudentContent
                        {
                            // === Thông tin Tài nguyên Cơ bản ===
                            LoaiTaiNguyen = reader["LoaiTaiNguyen"].ToString(),
                            ResourceID = Convert.ToInt32(reader["ResourceID"]), // Giả định không NULL
                            TenTaiNguyen = reader["TenTaiNguyen"].ToString(),
                            MoTa = reader["MoTa"] == DBNull.Value ? null : reader["MoTa"].ToString(),

                            // === Thông tin Liên kết (Links) ===
                            LinkVideo = reader["LinkVideo"] == DBNull.Value ? null : reader["LinkVideo"].ToString(),
                            LinkTaiLieu = reader["LinkTaiLieu"] == DBNull.Value ? null : reader["LinkTaiLieu"].ToString(),
                            LoaiChiTiet = reader["LoaiChiTiet"].ToString(),

                            // === Thông tin Khóa học và Chương ===
                            CourseID = Convert.ToInt32(reader["CourseID"]), // Giả định không NULL
                            TenKhoaHoc = reader["TenKhoaHoc"].ToString(),
                            AnhKhoaHoc = reader["AnhKhoaHoc"] == DBNull.Value ? null : reader["AnhKhoaHoc"].ToString(),

                            // SectionID là kiểu int? (Nullable int)
                            SectionID = reader["SectionID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["SectionID"]),

                            TenChuong = reader["TenChuong"].ToString(),
                            TenGiangVien = reader["TenGiangVien"].ToString(),

                            // === Trạng thái và Sắp xếp ===
                            TrangThai = reader["TrangThai"].ToString(),

                            // NgayDang là kiểu DateTime? (Nullable DateTime)
                            NgayDang = reader["NgayDang"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["NgayDang"],

                            ThuTuChuong = Convert.ToInt32(reader["ThuTuChuong"]), // Giả định không NULL
                            ThuTu = Convert.ToInt32(reader["ThuTu"])             // Giả định không NULL
                        });
                    }
                    catch (Exception ex)
                    {
                        // Bắt lỗi Ép kiểu Dữ liệu khi ánh xạ từ DataRow sang StudentContent
                        MessageBox.Show("Lỗi Ép Kiểu Dữ liệu trong ContentService: " + ex.Message, "Lỗi Ánh Xạ Dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break; // Dừng vòng lặp
                    }
                }
            }

            return resources;
        }
    }
}