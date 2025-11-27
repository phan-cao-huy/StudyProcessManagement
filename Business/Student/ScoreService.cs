using StudyProcessManagement.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using StudyProcessManagement.Business; // Phải thêm using này nếu DataProcessDAL ở namespace Business

namespace StudyProcessManagement.Business.Student
{
    public class ScoreService
    {
        // Sử dụng DataProcessDAL (Generic DAL) để truy cập DB
        private readonly DataProcessDAL _db = new DataProcessDAL();

        private const string SQL_COURSE_SUMMARY_FN =
            "SELECT CourseID, KhoaHoc, SoBaiDaCham, DiemTB, DiemCaoNhat, DiemThapNhat " +
            "FROM dbo.fn_StudentCourseScoreSummary(@StudentID)";


        // ------------------------------------------------------------------------------------------------
        // 1. GetCourseSummary: Sử dụng _db.ReadData cho truy vấn hàm SQL
        // ------------------------------------------------------------------------------------------------
        public List<CourseSummary> GetCourseSummary(string studentId)
        {
            List<CourseSummary> summaries = new List<CourseSummary>();

            // 1. CHUYỂN ĐỔI VÀ KIỂM TRA ID (Logic nghiệp vụ)
            if (!int.TryParse(studentId, out int studentIntId))
            {
                // Không hiển thị MessageBox, chỉ trả về rỗng nếu ở tầng Service
                return summaries;
            }

            // 2. Thiết lập tham số
            var parameters = new Dictionary<string, object> { { "@StudentID", studentIntId } };
            DataTable dt = null;

            try
            {
                // 3. GỌI DAL: Sử dụng ReadData của DataProcessDAL để thực thi hàm SQL
                dt = _db.ReadData(SQL_COURSE_SUMMARY_FN, parameters);
            }
            catch (Exception ex)
            {
                // Bắt lỗi từ DAL
                MessageBox.Show("Lỗi truy vấn cơ sở dữ liệu khi lấy dữ liệu tổng kết khóa học: " + ex.Message, "Lỗi DAL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return summaries;
            }

            // 4. Ánh xạ (Mapping) DataTable sang List<CourseSummary> (Logic nghiệp vụ)
            if (dt != null)
            {
                foreach (DataRow reader in dt.Rows)
                {
                    summaries.Add(new CourseSummary
                    {
                        CourseID = reader["CourseID"].ToString(),
                        KhoaHoc = reader["KhoaHoc"].ToString(),

                        // Xử lý DBNull an toàn
                        SoBaiDaCham = reader["SoBaiDaCham"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SoBaiDaCham"]),
                        DiemTB = reader["DiemTB"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DiemTB"]),
                        DiemCaoNhat = reader["DiemCaoNhat"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DiemCaoNhat"]),
                        DiemThapNhat = reader["DiemThapNhat"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DiemThapNhat"])
                    });
                }
            }
            return summaries;
        }

        // ------------------------------------------------------------------------------------------------
        // 2. GetOverallStats: Sử dụng _db.ExecuteStoredProcedure cho Stored Procedure
        // ------------------------------------------------------------------------------------------------
        public StudentOverallStats GetOverallStats(string studentId)
        {
            StudentOverallStats stats = new StudentOverallStats();

            // 1. CHUYỂN ĐỔI VÀ KIỂM TRA ID (Logic nghiệp vụ)
            if (!int.TryParse(studentId, out int studentIntId))
            {
                MessageBox.Show("Lỗi: ID sinh viên không hợp lệ. (Phải là số nguyên)", "Lỗi Dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return stats;
            }

            string spName = "sp_GetStudentScoreSummary";

            // 2. Thiết lập tham số
            var parameters = new Dictionary<string, object> { { "@StudentID", studentIntId } };
            DataTable dt = null;

            try
            {
                // 3. GỌI DAL: Sử dụng ExecuteStoredProcedure của DataProcessDAL
                dt = _db.ExecuteStoredProcedure(spName, parameters);
            }
            catch (Exception ex)
            {
                // Bắt lỗi từ DAL
                MessageBox.Show("Lỗi truy vấn cơ sở dữ liệu khi lấy dữ liệu tổng quan điểm số: " + ex.Message, "Lỗi DAL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return stats;
            }

            // 4. Ánh xạ (Mapping) DataTable sang StudentOverallStats (Logic nghiệp vụ)
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow reader = dt.Rows[0];
                stats.DiemTBTong = reader["DiemTBTong"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DiemTBTong"]);
                stats.DiemCaoNhat = reader["DiemCaoNhat"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DiemCaoNhat"]);
                stats.SoBaiDaCham = reader["SoBaiDaCham"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SoBaiDaCham"]);
                stats.XepLoai = reader["XepLoai"] == DBNull.Value ? "N/A" : reader["XepLoai"].ToString();
            }

            return stats;
        }
    }
}