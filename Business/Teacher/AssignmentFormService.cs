using System;
using System.Data;
using System.Collections.Generic;

namespace StudyProcessManagement.Business.Teacher
{
    public class AssignmentFormService
    {
        private DataProcess dataProcess;

        public AssignmentFormService()
        {
            dataProcess = new DataProcess();
        }

        public DataTable GetTeacherCourses(string teacherID)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@TeacherID", teacherID }
            };
            return dataProcess.ExecuteStoredProcedure("sp_GetTeacherCourses", parameters);
        }

        public DataTable GetAssignmentDetails(int assignmentID)
        {
            string query = @"
                SELECT AssignmentID,
                       CourseID,
                       Title,
                       Description,
                       AssignedDate,
                       DueDate,
                       MaxScore,
                       AttachmentData,
                       AttachmentName
                FROM Assignments
                WHERE AssignmentID = @ID";

            var parameters = new Dictionary<string, object>
            {
                { "@ID", assignmentID }
            };
            return dataProcess.ReadData(query, parameters);
        }

        public int CreateAssignment(int courseID, string title, string description,
    DateTime assignedDate, DateTime dueDate, decimal maxScore,
    byte[] attachmentData, string attachmentName)
        {
            string sql = @"INSERT INTO Assignments 
        (CourseID, Title, Description, AssignedDate, DueDate, MaxScore, AttachmentData, AttachmentName)
        VALUES (@CourseID, @Title, @Description, @AssignedDate, @DueDate, @MaxScore, @AttachmentData, @AttachmentName);
        SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
    {
        { "@CourseID", courseID },
        { "@Title", title },
        { "@Description", description ?? "" },
        { "@AssignedDate", assignedDate },
        { "@DueDate", dueDate },
        { "@MaxScore", maxScore },
        { "@AttachmentData", (object)attachmentData ?? DBNull.Value },
        { "@AttachmentName", (object)attachmentName ?? DBNull.Value }
    };

            DataTable dt = dataProcess.ReadData(sql, parameters);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0][0]) : 0;
        }
        /// <summary>
        /// Tạo bài tập với file đính kèm
        /// </summary>


        /// <summary>
        /// Lấy file đính kèm của bài tập
        /// </summary>
        public (byte[] FileData, string FileName) GetAssignmentAttachment(int assignmentID)
        {
            string sql = "SELECT AttachmentData, AttachmentName FROM Assignments WHERE AssignmentID = @AssignmentID";

            var parameters = new Dictionary<string, object>
    {
        { "@AssignmentID", assignmentID }
    };

            DataTable dt = dataProcess.ReadData(sql, parameters);

            if (dt.Rows.Count > 0 && dt.Rows[0]["AttachmentData"] != DBNull.Value)
            {
                byte[] fileData = (byte[])dt.Rows[0]["AttachmentData"];
                string fileName = dt.Rows[0]["AttachmentName"]?.ToString() ?? "attachment";
                return (fileData, fileName);
            }

            return (null, null);
        }
        public bool UpdateAssignment(int assignmentID, int courseID, string title, string description,
    DateTime assignedDate, DateTime dueDate, decimal maxScore,
    byte[] attachmentData, string attachmentName)
        {
            // ✅ FIX: Nếu KHÔNG có file mới, chỉ update thông tin khác, GIỮ NGUYÊN file cũ
            string sql;
            Dictionary<string, object> parameters;

            if (attachmentData != null && attachmentData.Length > 0)
            {
                // CÓ file mới → Update cả file
                sql = @"UPDATE Assignments
            SET CourseID = @CourseID,
                Title = @Title,
                Description = @Description,
                AssignedDate = @AssignedDate,
                DueDate = @DueDate,
                MaxScore = @MaxScore,
                AttachmentData = @AttachmentData,
                AttachmentName = @AttachmentName
            WHERE AssignmentID = @AssignmentID";

                parameters = new Dictionary<string, object>
        {
            { "@AssignmentID", assignmentID },
            { "@CourseID", courseID },
            { "@Title", title },
            { "@Description", description ?? "" },
            { "@AssignedDate", assignedDate },
            { "@DueDate", dueDate },
            { "@MaxScore", maxScore },
            { "@AttachmentData", attachmentData },
            { "@AttachmentName", attachmentName }
        };
            }
            else
            {
                // KHÔNG có file mới → Chỉ update thông tin, giữ nguyên file cũ
                sql = @"UPDATE Assignments
            SET CourseID = @CourseID,
                Title = @Title,
                Description = @Description,
                AssignedDate = @AssignedDate,
                DueDate = @DueDate,
                MaxScore = @MaxScore
            WHERE AssignmentID = @AssignmentID";

                parameters = new Dictionary<string, object>
        {
            { "@AssignmentID", assignmentID },
            { "@CourseID", courseID },
            { "@Title", title },
            { "@Description", description ?? "" },
            { "@AssignedDate", assignedDate },
            { "@DueDate", dueDate },
            { "@MaxScore", maxScore }
        };
            }

            return dataProcess.ChangeData(sql, parameters);
        }

    }
}