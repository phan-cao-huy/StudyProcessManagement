using StudyProcessManagement.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace StudyProcessManagement.Business
{
    public class StudentInfoService
    {
        private readonly DataProcessDAL _db = new DataProcessDAL();

        public StudentInfoModel LoginAndGetStudentInfo(string email, string passwordHash)
        {
            string sql = @"
                SELECT u.UserID, u.FullName, acc.Email, u.PhoneNumber,
                       u.AvatarUrl, u.DateOfBirth, u.Address
                FROM Accounts acc
                INNER JOIN Users u ON acc.AccountID = u.AccountID
                WHERE acc.Email = @Email
                  AND acc.PasswordHash = @PasswordHash
                  AND acc.Role = 'Student'
            ";

            var parameters = new Dictionary<string, object>()
            {
                {"@Email", email },
                {"@PasswordHash", passwordHash }
            };

            DataTable dt = _db.ReadData(sql, parameters);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new StudentInfoModel
            {
                UserID = row["UserID"].ToString(),
                FullName = row["FullName"].ToString(),
                Email = row["Email"].ToString(),
                PhoneNumber = row["PhoneNumber"].ToString(),
                AvatarUrl = row["AvatarUrl"] == DBNull.Value ? null : row["AvatarUrl"].ToString(),
                Address = row["Address"].ToString(),
                DateOfBirth = row["DateOfBirth"] == DBNull.Value ? null : (DateTime?)row["DateOfBirth"]
            };
        }
    }
}
