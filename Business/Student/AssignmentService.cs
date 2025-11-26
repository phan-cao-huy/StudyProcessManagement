using StudyProcessManagement.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using StudyProcessManagement.Business; // Đảm bảo DataProcessDAL nằm trong namespace này

namespace StudyProcessManagement.Business.Student
{
    // Class Service (Lớp nghiệp vụ)
    public class AssignmentService
    {
        private readonly DataProcessDAL _db = new DataProcessDAL();
        private const string SP_GET_ASSIGNMENTS = "sp_GetStudentAssignments";
        private const string SP_INSERT_SUBMISSION = "sp_InsertStudentSubmission";


        // ------------------------------------------------------------------------------------------------
        // 1. Phương thức GetStudentAssignments (Đã sửa để sử dụng DataProcessDAL)
        // ------------------------------------------------------------------------------------------------
        public List<AssignmentViewModel> GetStudentAssignments(string studentId)
        {
            List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();

            if (!int.TryParse(studentId, out int studentIntId))
            {
                MessageBox.Show("ID sinh viên không hợp lệ.", "Lỗi Dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return assignments;
            }

            var parameters = new Dictionary<string, object>
            {
                { "@StudentID", studentIntId }
            };

            DataTable dt = null;
            try
            {
                // GỌI DAL: Lớp DAL (DataProcessDAL) lo phần kết nối và truy vấn DB
                dt = _db.ExecuteStoredProcedure(SP_GET_ASSIGNMENTS, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi truy vấn cơ sở dữ liệu khi lấy danh sách bài tập: " + ex.Message, "Lỗi DAL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return assignments;
            }

            // Ánh xạ (Mapping) DataTable sang List<AssignmentViewModel>
            if (dt != null)
            {
                foreach (DataRow reader in dt.Rows)
                {
                    try
                    {
                        assignments.Add(new AssignmentViewModel
                        {
                            AssignmentID = reader["AssignmentID"] is DBNull ? -1 : Convert.ToInt32(reader["AssignmentID"]),
                            TenBaiTap = reader["TenBaiTap"] is DBNull ? "" : reader["TenBaiTap"].ToString(),
                            MoTa = reader["MoTa"] is DBNull ? "" : reader["MoTa"].ToString(),
                            NgayGiao = reader["NgayGiao"] is DBNull ? DateTime.MinValue : (DateTime)reader["NgayGiao"],
                            HanNop = reader["HanNop"] is DBNull ? DateTime.MaxValue : (DateTime)reader["HanNop"],
                            DiemToiDa = reader["DiemToiDa"] is DBNull ? 0m : Convert.ToDecimal(reader["DiemToiDa"]),
                            TaiLieuDinhKem = reader["TaiLieuDinhKem"] is DBNull ? null : reader["TaiLieuDinhKem"].ToString(),
                            CourseID = reader["CourseID"] is DBNull ? -1 : Convert.ToInt32(reader["CourseID"]),
                            TenKhoaHoc = reader["TenKhoaHoc"] is DBNull ? "" : reader["TenKhoaHoc"].ToString(),
                            TenGiangVien = reader["TenGiangVien"] is DBNull ? "" : reader["TenGiangVien"].ToString(),
                            SubmissionID = reader["SubmissionID"] is DBNull ? (int?)null : Convert.ToInt32(reader["SubmissionID"]),
                            DiemDat = reader["DiemDat"] is DBNull ? (decimal?)null : Convert.ToDecimal(reader["DiemDat"]),
                            NgayNop = reader["NgayNop"] is DBNull ? (DateTime?)null : (DateTime)reader["NgayNop"],
                            FileBaiNop = reader["FileBaiNop"] is DBNull ? null : reader["FileBaiNop"].ToString(),
                            GhiChuSinhVien = reader["GhiChuSinhVien"] is DBNull ? null : reader["GhiChuSinhVien"].ToString(),
                            NhanXetGiaoVien = reader["NhanXetGiaoVien"] is DBNull ? null : reader["NhanXetGiaoVien"].ToString(),
                            TrangThai = reader["TrangThai"] is DBNull ? "" : reader["TrangThai"].ToString(),
                            StatusColor = reader["StatusColor"] is DBNull ? "" : reader["StatusColor"].ToString(),
                            ThoiGianConLai = reader["ThoiGianConLai"] is DBNull ? "" : reader["ThoiGianConLai"].ToString(),
                            SoNgayConLai = reader["SoNgayConLai"] is DBNull ? 0 : Convert.ToInt32(reader["SoNgayConLai"]),
                            SoGioConLai = reader["SoGioConLai"] is DBNull ? 0 : Convert.ToInt32(reader["SoGioConLai"]),
                            DaNopTre = reader["DaNopTre"] is DBNull ? false : Convert.ToInt32(reader["DaNopTre"]) == 1,
                            DaHoanThanh = reader["DaHoanThanh"] is DBNull ? false : Convert.ToInt32(reader["DaHoanThanh"]) == 1,
                            PhanTramDiem = reader["PhanTramDiem"] is DBNull ? (decimal?)null : Convert.ToDecimal(reader["PhanTramDiem"]),
                            NgayGiaoFormat = reader["NgayGiaoFormat"] is DBNull ? "" : reader["NgayGiaoFormat"].ToString(),
                            HanNopFormat = reader["HanNopFormat"] is DBNull ? "" : reader["HanNopFormat"].ToString(),
                            NgayNopFormat = reader["NgayNopFormat"] is DBNull ? null : reader["NgayNopFormat"].ToString(),
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi ép kiểu dữ liệu khi đọc danh sách bài tập: {ex.Message}", "Lỗi Ánh Xạ Dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }
            return assignments;
        }


        // ------------------------------------------------------------------------------------------------
        // 2. Phương thức InsertSubmission (Đã sửa để sử dụng DataProcessDAL)
        // ------------------------------------------------------------------------------------------------
        public int InsertSubmission(int assignmentId, string studentId, string fileUrl, string studentNote)
        {
            // 1. Kiểm tra ID sinh viên (Logic nghiệp vụ/Service Layer)
            if (!int.TryParse(studentId, out int studentIntId))
            {
                MessageBox.Show("ID sinh viên không hợp lệ. (Phải là số nguyên)", "Lỗi Dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            int submissionId = 0;

            // 2. Định nghĩa tham số cho DAL
            var parameters = new Dictionary<string, object>
            {
                { "@AssignmentID", assignmentId },
                { "@StudentID", studentIntId },
                { "@FileUrl", fileUrl },
                // Xử lý giá trị NULL an toàn cho StudentNote
                { "@StudentNote", string.IsNullOrEmpty(studentNote) ? (object)DBNull.Value : studentNote }
            };

            DataTable dt = null;
            try
            {
                // 3. GỌI DAL để thực thi Stored Procedure.
                // DataProcessDAL chịu trách nhiệm thực thi câu lệnh SQL và quản lý kết nối.
                dt = _db.ExecuteStoredProcedure(SP_INSERT_SUBMISSION, parameters);

                // 4. Trích xuất SubmissionID từ DataTable
                // Giả định Stored Procedure trả về một DataTable chứa SubmissionID
                if (dt != null && dt.Rows.Count > 0)
                {
                    // Lấy giá trị cột đầu tiên (hoặc cột có tên 'SubmissionID')
                    object result = dt.Rows[0][0]; // Lấy cột đầu tiên (index 0)

                    if (dt.Columns.Contains("SubmissionID")) // Nếu SP trả về cột có tên rõ ràng
                    {
                        result = dt.Rows[0]["SubmissionID"];
                    }

                    if (result != null && result != DBNull.Value)
                    {
                        submissionId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                // 5. Bắt lỗi từ DAL (Lỗi truy vấn SQL)
                MessageBox.Show("Lỗi truy vấn cơ sở dữ liệu khi nộp bài: " + ex.Message, "Lỗi Nộp bài (DAL)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            // Trả về SubmissionID: > 0 nếu thành công, 0 nếu thất bại
            return submissionId;
        }
    }
}