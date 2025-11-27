-- ============================================
-- DATABASE: StudyProcess
-- Ngày tạo: 27/11/2025
-- Mô tả: Hệ thống quản lý học tập - LMS
-- ============================================

-- Xóa database cũ nếu tồn tại (CHÚ Ý: sẽ mất toàn bộ dữ liệu)
-- ALTER DATABASE StudyProcess SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
-- DROP DATABASE StudyProcess;

-- Tạo database mới
CREATE DATABASE [StudyProcess];
GO

USE [StudyProcess];
GO

/* ==============================================
   PHẦN 1: TẠO CÁC BẢNG (TABLES)
   ============================================== */

-- ============================================
-- Bảng Accounts: Tài khoản đăng nhập
-- ============================================
CREATE TABLE Accounts (
    AccountID VARCHAR(50) NOT NULL PRIMARY KEY,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NULL, -- Student / Teacher / Admin
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================
-- Bảng Users: Thông tin người dùng
-- ============================================
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AccountID VARCHAR(50) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20) NULL,
    AvatarUrl NVARCHAR(255) NULL,
    DateOfBirth DATE NULL,
    Address NVARCHAR(255) NULL,
    CONSTRAINT FK_Users_Accounts 
        FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID)
);
GO

-- ============================================
-- Bảng Categories: Danh mục khóa học
-- ============================================
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL
);
GO

-- ============================================
-- Bảng Courses: Khóa học
-- ============================================
CREATE TABLE Courses (
    CourseID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CourseName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ImageCover NVARCHAR(255) NULL,
    TotalLessons INT NOT NULL DEFAULT 0,
    CategoryID INT NOT NULL,
    TeacherID VARCHAR(50) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) NULL, -- Active / Draft / Archived
    CONSTRAINT FK_Courses_Categories 
        FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT FK_Courses_Accounts_Teacher 
        FOREIGN KEY (TeacherID) REFERENCES Accounts(AccountID)
);
GO

-- ============================================
-- Bảng Sections: Chương học
-- ============================================
CREATE TABLE Sections (
    SectionID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CourseID INT NOT NULL,
    SectionTitle NVARCHAR(200) NOT NULL,
    SectionOrder INT NULL,
    Description NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NULL,
    CONSTRAINT FK_Sections_Courses 
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);
GO

-- ============================================
-- Bảng Lessons: Bài học
-- ============================================
CREATE TABLE Lessons (
    LessonID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CourseID INT NOT NULL,
    LessonTitle NVARCHAR(200) NOT NULL,
    LessonOrder INT NULL,
    Content NVARCHAR(MAX) NULL,
    VideoUrl NVARCHAR(500) NULL,
    AttachmentUrl NVARCHAR(500) NULL,
    SectionID INT NULL,
    CONSTRAINT FK_Lessons_Courses 
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
    CONSTRAINT FK_Lessons_Sections 
        FOREIGN KEY (SectionID) REFERENCES Sections(SectionID)
);
GO

-- ============================================
-- Bảng Assignments: Bài tập
-- ============================================
CREATE TABLE Assignments (
    AssignmentID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CourseID INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    AssignedDate DATETIME NULL,
    DueDate DATETIME NULL,
    MaxScore DECIMAL(5,2) NULL,
    AttachmentPath NVARCHAR(500) NULL,
    CONSTRAINT FK_Assignments_Courses 
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);
GO

-- ============================================
-- Bảng Enrollments: Đăng ký học
-- ============================================
CREATE TABLE Enrollments (
    EnrollmentID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    StudentID INT NOT NULL, -- Users.UserID (role = Student)
    CourseID INT NOT NULL,
    EnrollmentDate DATETIME NOT NULL DEFAULT GETDATE(),
    ProgressPercent INT NULL,
    CompletedLessons INT NULL,
    Status NVARCHAR(50) NULL,
    AverageScore DECIMAL(5,2) NULL,
    CONSTRAINT FK_Enrollments_Users 
        FOREIGN KEY (StudentID) REFERENCES Users(UserID),
    CONSTRAINT FK_Enrollments_Courses 
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);
GO

-- ============================================
-- Bảng Submissions: Bài nộp
-- ============================================
CREATE TABLE Submissions (
    SubmissionID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AssignmentID INT NOT NULL,
    StudentID INT NOT NULL,
    SubmissionDate DATETIME NULL,
    FileUrl NVARCHAR(500) NULL,
    StudentNote NVARCHAR(MAX) NULL,
    Score DECIMAL(5,2) NULL,
    TeacherFeedback NVARCHAR(MAX) NULL,
    Status NVARCHAR(50) NULL,
    CONSTRAINT FK_Submissions_Assignments 
        FOREIGN KEY (AssignmentID) REFERENCES Assignments(AssignmentID),
    CONSTRAINT FK_Submissions_Users 
        FOREIGN KEY (StudentID) REFERENCES Users(UserID)
);
GO

-- ============================================
-- Bảng ActivityLogs: Nhật ký hoạt động
-- ============================================
CREATE TABLE ActivityLogs (
    LogID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID INT NOT NULL,
    ActivityType NVARCHAR(100) NOT NULL,
    ActivityDescription NVARCHAR(MAX) NULL,
    RelatedEntityType NVARCHAR(50) NULL,
    RelatedEntityID INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_ActivityLogs_Users 
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- ============================================
-- Bảng LessonProgress: Tiến trình bài học
-- ============================================
CREATE TABLE LessonProgress (
    ProgressID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    LessonID INT NOT NULL,
    IsCompleted BIT DEFAULT 0,
    CompletedDate DATETIME NULL,
    CONSTRAINT FK_LessonProgress_Students 
        FOREIGN KEY (StudentID) REFERENCES Users(UserID),
    CONSTRAINT FK_LessonProgress_Lessons 
        FOREIGN KEY (LessonID) REFERENCES Lessons(LessonID)
);
GO

PRINT '✅ Đã tạo tất cả các bảng thành công!';
PRINT '';
GO

/* ==============================================
   PHẦN 2: CẬP NHẬT CẤU TRÚC BẢNG (HỖ TRỢ UPLOAD)
   ============================================== */

-- Thêm cột lưu file đính kèm dạng binary cho Lessons
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Lessons') AND name = 'AttachmentData')
BEGIN
    ALTER TABLE Lessons
    ADD AttachmentData VARBINARY(MAX) NULL,
        AttachmentName NVARCHAR(255) NULL;
    PRINT '✅ Đã thêm cột AttachmentData vào bảng Lessons.';
END
GO

-- Thêm cột lưu file đính kèm dạng binary cho Assignments
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assignments') AND name = 'AttachmentData')
BEGIN
    ALTER TABLE Assignments
    ADD AttachmentData VARBINARY(MAX) NULL,
        AttachmentName NVARCHAR(255) NULL;
    PRINT '✅ Đã thêm cột AttachmentData vào bảng Assignments.';
END
GO

-- Thêm cột lưu file nộp bài dạng binary cho Submissions
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Submissions') AND name = 'FileData')
BEGIN
    ALTER TABLE Submissions
    ADD FileData VARBINARY(MAX) NULL,
        FileName NVARCHAR(255) NULL;
    PRINT '✅ Đã thêm cột FileData vào bảng Submissions.';
END
GO

/* ==============================================
   PHẦN 3: CẤU HÌNH CASCADE DELETE
   ============================================== */

-- Cấu hình cascade cho Users
ALTER TABLE Users DROP CONSTRAINT FK_Users_Accounts;
GO
ALTER TABLE Users ADD CONSTRAINT FK_Users_Accounts
    FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID) ON DELETE CASCADE;
GO

-- Cấu hình cascade cho Sections
ALTER TABLE Sections DROP CONSTRAINT FK_Sections_Courses;
GO
ALTER TABLE Sections ADD CONSTRAINT FK_Sections_Courses
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE;
GO

-- Cấu hình cascade cho Lessons
ALTER TABLE Lessons DROP CONSTRAINT FK_Lessons_Sections;
GO
ALTER TABLE Lessons ADD CONSTRAINT FK_Lessons_Sections
    FOREIGN KEY (SectionID) REFERENCES Sections(SectionID) ON DELETE CASCADE;
GO

-- Cấu hình cascade cho Enrollments
ALTER TABLE Enrollments DROP CONSTRAINT FK_Enrollments_Users;
ALTER TABLE Enrollments DROP CONSTRAINT FK_Enrollments_Courses;
GO
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Users
    FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE;
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Courses
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE;
GO

-- Cấu hình cascade cho Assignments
ALTER TABLE Assignments DROP CONSTRAINT FK_Assignments_Courses;
GO
ALTER TABLE Assignments ADD CONSTRAINT FK_Assignments_Courses
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE;
GO

-- Cấu hình cascade cho Submissions
ALTER TABLE Submissions DROP CONSTRAINT FK_Submissions_Assignments;
ALTER TABLE Submissions DROP CONSTRAINT FK_Submissions_Users;
GO
ALTER TABLE Submissions ADD CONSTRAINT FK_Submissions_Assignments
    FOREIGN KEY (AssignmentID) REFERENCES Assignments(AssignmentID) ON DELETE CASCADE;
ALTER TABLE Submissions ADD CONSTRAINT FK_Submissions_Users
    FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE;
GO

-- Cấu hình cascade cho ActivityLogs
ALTER TABLE ActivityLogs DROP CONSTRAINT FK_ActivityLogs_Users;
GO
ALTER TABLE ActivityLogs ADD CONSTRAINT FK_ActivityLogs_Users
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE;
GO

-- Cấu hình cascade cho LessonProgress
ALTER TABLE LessonProgress DROP CONSTRAINT FK_LessonProgress_Students;
ALTER TABLE LessonProgress DROP CONSTRAINT FK_LessonProgress_Lessons;
GO
ALTER TABLE LessonProgress ADD CONSTRAINT FK_LessonProgress_Students
    FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE;
ALTER TABLE LessonProgress ADD CONSTRAINT FK_LessonProgress_Lessons
    FOREIGN KEY (LessonID) REFERENCES Lessons(LessonID) ON DELETE CASCADE;
GO

PRINT '✅ Đã cấu hình CASCADE DELETE cho tất cả các bảng!';
PRINT '';
GO

/* ==============================================
   PHẦN 4: FUNCTIONS (HÀM HỖ TRỢ)
   ============================================== */

-- ============================================
-- Function: fn_StudentCourseScoreSummary
-- Mục đích: Tính điểm tổng kết theo từng khóa học
-- ============================================
IF OBJECT_ID('fn_StudentCourseScoreSummary', 'IF') IS NOT NULL
    DROP FUNCTION fn_StudentCourseScoreSummary;
GO

CREATE FUNCTION fn_StudentCourseScoreSummary
(
    @StudentID INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        c.CourseID,
        c.CourseName AS KhoaHoc,
        COUNT(s.SubmissionID) AS SoBaiDaCham,
        ISNULL(AVG(s.Score * 1.0), 0) AS DiemTB,
        ISNULL(MAX(s.Score), 0) AS DiemCaoNhat,
        ISNULL(MIN(s.Score), 0) AS DiemThapNhat
    FROM Enrollments e
    INNER JOIN Courses c ON e.CourseID = c.CourseID
    LEFT JOIN Assignments a ON c.CourseID = a.CourseID
    LEFT JOIN Submissions s ON a.AssignmentID = s.AssignmentID AND s.StudentID = e.StudentID
    WHERE e.StudentID = @StudentID AND s.Score IS NOT NULL
    GROUP BY c.CourseID, c.CourseName
);
GO

PRINT '✅ Đã tạo Function [fn_StudentCourseScoreSummary].';
GO

/* ==============================================
   PHẦN 5: STORED PROCEDURES
   ============================================== */

-- ============================================
-- SP: sp_GetCourseStructure
-- Mục đích: Lấy cấu trúc khóa học (sections + lessons)
-- ============================================
IF OBJECT_ID('sp_GetCourseStructure', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetCourseStructure;
GO

CREATE PROCEDURE sp_GetCourseStructure
    @CourseID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.SectionID,
        s.SectionTitle,
        s.SectionOrder,
        s.Description,
        COUNT(l.LessonID) AS TotalLessons
    FROM Sections s
    LEFT JOIN Lessons l ON s.SectionID = l.SectionID
    WHERE s.CourseID = @CourseID
    GROUP BY s.SectionID, s.SectionTitle, s.SectionOrder, s.Description
    ORDER BY s.SectionOrder;
END;
GO

PRINT '✅ Đã tạo Stored Procedure [sp_GetCourseStructure].';
GO

-- ============================================
-- SP: sp_GetTeacherCourses
-- Mục đích: Lấy danh sách khóa học của giáo viên
-- ============================================
IF OBJECT_ID('sp_GetTeacherCourses', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTeacherCourses;
GO

CREATE PROCEDURE sp_GetTeacherCourses
    @TeacherID VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.CourseID,
        c.CourseName,
        c.Description,
        c.CategoryID,
        cat.CategoryName,
        c.TeacherID,
        c.ImageCover,
        c.Status,
        c.TotalLessons,
        c.CreatedAt,
        COUNT(DISTINCT e.EnrollmentID) AS TotalStudents
    FROM Courses c
    LEFT JOIN Categories cat ON c.CategoryID = cat.CategoryID
    LEFT JOIN Enrollments e ON c.CourseID = e.CourseID
    WHERE c.TeacherID = @TeacherID
    GROUP BY c.CourseID, c.CourseName, c.Description, c.CategoryID,
             cat.CategoryName, c.TeacherID, c.ImageCover, c.Status,
             c.TotalLessons, c.CreatedAt
    ORDER BY c.CreatedAt DESC;
END;
GO

PRINT '✅ Đã tạo Stored Procedure [sp_GetTeacherCourses].';
GO

-- ============================================
-- SP: sp_GetAllSubmissions
-- Mục đích: Lấy danh sách bài nộp của giáo viên
-- ============================================
IF OBJECT_ID('sp_GetAllSubmissions', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllSubmissions;
GO

CREATE PROCEDURE sp_GetAllSubmissions
    @TeacherID VARCHAR(50),
    @CourseID INT = NULL,
    @StatusFilter NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.SubmissionID,
        u.FullName AS StudentName,
        a.Title AS AssignmentTitle,
        c.CourseName,
        s.SubmissionDate,
        a.DueDate,
        s.Score,
        a.MaxScore,
        s.FileUrl,
        s.StudentNote,
        s.TeacherFeedback,
        CASE
            WHEN s.Score IS NOT NULL THEN N'Đã chấm'
            WHEN s.SubmissionDate > a.DueDate THEN N'Nộp trễ'
            ELSE N'Chưa chấm'
        END AS Status
    FROM Submissions s
    INNER JOIN Assignments a ON s.AssignmentID = a.AssignmentID
    INNER JOIN Courses c ON a.CourseID = c.CourseID
    INNER JOIN Users u ON s.StudentID = u.UserID
    WHERE c.TeacherID = @TeacherID
        AND (@CourseID IS NULL OR c.CourseID = @CourseID)
        AND (@StatusFilter IS NULL 
             OR @StatusFilter = N'Tất cả'
             OR (
                 (@StatusFilter = N'Đã chấm' AND s.Score IS NOT NULL)
                 OR (@StatusFilter = N'Chưa chấm' AND s.Score IS NULL AND s.SubmissionDate <= a.DueDate)
                 OR (@StatusFilter = N'Nộp trễ' AND s.Score IS NULL AND s.SubmissionDate > a.DueDate)
             ))
    ORDER BY s.SubmissionDate DESC;
END;
GO

PRINT '✅ Đã tạo Stored Procedure [sp_GetAllSubmissions].';
GO

-- ============================================
-- SP: sp_GradeSubmission
-- Mục đích: Chấm điểm bài nộp
-- ============================================
IF OBJECT_ID('sp_GradeSubmission', 'P') IS NOT NULL
    DROP PROCEDURE sp_GradeSubmission;
GO

CREATE PROCEDURE sp_GradeSubmission
    @SubmissionID INT,
    @Score DECIMAL(5,2),
    @TeacherFeedback NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE Submissions
    SET Score = @Score,
        TeacherFeedback = @TeacherFeedback
    WHERE SubmissionID = @SubmissionID;

    SELECT @@ROWCOUNT AS AffectedRows;
END;
GO

PRINT '✅ Đã tạo Stored Procedure [sp_GradeSubmission].';
GO

-- ============================================
-- SP: sp_GetAllResourcesForStudent
-- Mục đích: Lấy tất cả tài nguyên của sinh viên
-- ============================================
IF OBJECT_ID('sp_GetAllResourcesForStudent', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllResourcesForStudent;
GO

CREATE PROCEDURE sp_GetAllResourcesForStudent
    @StudentID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy tài nguyên Bài học
    SELECT
        N'Lesson' AS LoaiTaiNguyen,
        l.LessonID AS ResourceID,
        l.LessonTitle AS TenTaiNguyen,
        l.Content AS MoTa,
        l.VideoUrl AS LinkVideo,
        l.AttachmentUrl AS LinkTaiLieu,
        N'Bài giảng' AS LoaiChiTiet,
        c.CourseID,
        c.CourseName AS TenKhoaHoc,
        c.ImageCover AS AnhKhoaHoc,
        s.SectionID,
        s.SectionTitle AS TenChuong,
        (SELECT FullName FROM Users WHERE AccountID = c.TeacherID) AS TenGiangVien,
        ISNULL(lp.IsCompleted, 0) AS TrangThaiRaw,
        CASE WHEN ISNULL(lp.IsCompleted, 0) = 1 THEN N'Hoàn thành' ELSE N'Chưa bắt đầu' END AS TrangThai,
        c.CreatedAt AS NgayDang,
        s.SectionOrder AS ThuTuChuong,
        l.LessonOrder AS ThuTu
    FROM Lessons l
    INNER JOIN Courses c ON l.CourseID = c.CourseID
    LEFT JOIN Sections s ON l.SectionID = s.SectionID
    INNER JOIN Enrollments e ON c.CourseID = e.CourseID
    LEFT JOIN LessonProgress lp ON l.LessonID = lp.LessonID AND lp.StudentID = @StudentID
    WHERE e.StudentID = @StudentID

    UNION ALL

    -- Lấy tài nguyên Bài tập
    SELECT
        N'Assignment' AS LoaiTaiNguyen,
        a.AssignmentID AS ResourceID,
        a.Title AS TenTaiNguyen,
        a.Description AS MoTa,
        NULL AS LinkVideo,
        a.AttachmentPath AS LinkTaiLieu,
        N'Đề bài' AS LoaiChiTiet,
        c.CourseID,
        c.CourseName AS TenKhoaHoc,
        c.ImageCover AS AnhKhoaHoc,
        NULL AS SectionID,
        NULL AS TenChuong,
        (SELECT FullName FROM Users WHERE AccountID = c.TeacherID) AS TenGiangVien,
        CASE WHEN sub.SubmissionID IS NOT NULL THEN 1 ELSE 0 END AS TrangThaiRaw,
        CASE
            WHEN sub.Score IS NOT NULL THEN N'Đã chấm'
            WHEN sub.SubmissionID IS NOT NULL THEN N'Đã nộp'
            WHEN a.DueDate < GETDATE() THEN N'Quá hạn'
            ELSE N'Chưa nộp'
        END AS TrangThai,
        a.AssignedDate AS NgayDang,
        999 AS ThuTuChuong,
        a.AssignmentID AS ThuTu
    FROM Assignments a
    INNER JOIN Courses c ON a.CourseID = c.CourseID
    INNER JOIN Enrollments e ON c.CourseID = e.CourseID
    LEFT JOIN Submissions sub ON a.AssignmentID = sub.AssignmentID AND sub.StudentID = @StudentID
    WHERE e.StudentID = @StudentID

    ORDER BY CourseID, ThuTuChuong, ThuTu;
END;
GO

PRINT '✅ Đã tạo Stored Procedure [sp_GetAllResourcesForStudent].';
GO

-- ============================================
-- SP: sp_GetStudentAssignments
-- Mục đích: Lấy danh sách bài tập của sinh viên
-- ============================================
IF OBJECT_ID('sp_GetStudentAssignments', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetStudentAssignments;
GO

CREATE PROCEDURE sp_GetStudentAssignments
    @StudentID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.AssignmentID,
        a.Title AS TenBaiTap,
        a.Description AS MoTa,
        a.AssignedDate AS NgayGiao,
        a.DueDate AS HanNop,
        a.MaxScore AS DiemToiDa,
        a.AttachmentPath AS TaiLieuDinhKem,
        c.CourseID,
        c.CourseName AS TenKhoaHoc,
        (SELECT FullName FROM Users WHERE AccountID = c.TeacherID) AS TenGiangVien,
        s.SubmissionID,
        s.Score AS DiemDat,
        s.SubmissionDate AS NgayNop,
        s.FileUrl AS FileBaiNop,
        s.StudentNote AS GhiChuSinhVien,
        s.TeacherFeedback AS NhanXetGiaoVien,
        -- Tính toán Trạng thái
        CASE
            WHEN s.Score IS NOT NULL THEN N'Đã chấm'
            WHEN s.SubmissionID IS NOT NULL THEN N'Chờ chấm'
            WHEN GETDATE() > a.DueDate THEN N'Quá hạn'
            ELSE N'Chưa nộp'
        END AS TrangThai,
        -- Màu sắc Status
        CASE
            WHEN s.Score IS NOT NULL THEN N'badge-success'
            WHEN GETDATE() > a.DueDate THEN N'badge-danger'
            WHEN s.SubmissionID IS NOT NULL THEN N'badge-warning'
            ELSE N'badge-primary'
        END AS StatusColor,
        -- Thời gian còn lại
        CASE
            WHEN s.SubmissionID IS NOT NULL THEN N'Đã nộp'
            WHEN GETDATE() > a.DueDate THEN N'Đã quá hạn'
            ELSE ISNULL(CAST(DATEDIFF(DAY, GETDATE(), a.DueDate) AS NVARCHAR) + N' ngày', N'Hôm nay')
        END AS ThoiGianConLai,
        DATEDIFF(DAY, GETDATE(), a.DueDate) AS SoNgayConLai,
        DATEDIFF(HOUR, GETDATE(), a.DueDate) AS SoGioConLai,
        CASE WHEN s.SubmissionID IS NOT NULL AND s.SubmissionDate > a.DueDate THEN 1 ELSE 0 END AS DaNopTre,
        CASE WHEN s.SubmissionID IS NOT NULL THEN 1 ELSE 0 END AS DaHoanThanh,
        ISNULL(s.Score, 0) * 100.0 / a.MaxScore AS PhanTramDiem,
        CONVERT(NVARCHAR, a.AssignedDate, 103) AS NgayGiaoFormat,
        CONVERT(NVARCHAR, a.DueDate, 103) AS HanNopFormat,
        CONVERT(NVARCHAR, s.SubmissionDate, 103) + ' ' + CONVERT(NVARCHAR, s.SubmissionDate, 108) AS NgayNopFormat
    FROM Assignments a
    INNER JOIN Courses c ON a.CourseID = c.CourseID
    INNER JOIN Enrollments e ON c.CourseID = e.CourseID AND e.StudentID = @StudentID
    LEFT JOIN Submissions s ON a.AssignmentID = s.AssignmentID AND s.StudentID = @StudentID
    ORDER BY a.DueDate;
END;
GO

PRINT '✅ Đã tạo Stored Procedure [sp_GetStudentAssignments].';
GO

-- ============================================
-- SP: sp_GetMyCoursesWithProgress
-- Mục đích: Lấy khóa học kèm tiến trình của sinh viên
-- ============================================
IF OBJECT_ID('sp_GetMyCoursesWithProgress', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetMyCoursesWithProgress;
GO

CREATE PROCEDURE sp_GetMyCoursesWithProgress
    @StudentID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.CourseID,
        c.CourseName AS TenKhoaHoc,
        c.ImageCover AS AnhBia,
        c.Description AS MoTa,
        cat.CategoryName AS DanhMuc,
        cat.CategoryID,
        (SELECT FullName FROM Users WHERE AccountID = c.TeacherID) AS TenGiangVien,
        (SELECT AvatarUrl FROM Users WHERE AccountID = c.TeacherID) AS AnhGiangVien,
        ISNULL(e.ProgressPercent, 0) AS TienDoHoc,
        ISNULL(e.CompletedLessons, 0) AS SoBaiHoanThanh,
        c.TotalLessons AS TongSoBaiHoc,
        c.TotalLessons - ISNULL(e.CompletedLessons, 0) AS SoBaiConLai,
        CAST(ISNULL(e.CompletedLessons, 0) AS NVARCHAR) + N'/' + CAST(c.TotalLessons AS NVARCHAR) + N' bài' AS TienDoText,
        CAST(ISNULL(e.ProgressPercent, 0) AS NVARCHAR) + N'%' AS PhanTramText,
        CASE
            WHEN ISNULL(e.ProgressPercent, 0) >= 80 THEN N'success'
            WHEN ISNULL(e.ProgressPercent, 0) >= 50 THEN N'warning'
            ELSE N'info'
        END AS ProgressColor,
        e.Status AS TrangThai,
        e.Status AS TrangThaiText,
        e.EnrollmentDate AS NgayDangKy,
        ISNULL(e.AverageScore, 0.0) AS DiemTrungBinh
    FROM Enrollments e
    INNER JOIN Courses c ON e.CourseID = c.CourseID
    INNER JOIN Categories cat ON c.CategoryID = cat.CategoryID
    WHERE e.StudentID = @StudentID
    ORDER BY e.EnrollmentDate DESC;
END;
GO

PRINT '✅ Đã tạo Stored Procedure [sp_GetMyCoursesWithProgress].';
GO

-- ============================================
-- SP: sp_GetStudentScoreSummary
-- Mục đích: Tính điểm tổng kết của sinh viên
-- ============================================
IF OBJECT_ID('sp_GetStudentScoreSummary', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetStudentScoreSummary;
GO

CREATE PROCEDURE sp_GetStudentScoreSummary
    @StudentID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ISNULL(AVG(e.AverageScore * 1.0), 0) AS DiemTBTong,
        ISNULL(MAX(subsummary.MaxScore), 0) AS DiemCaoNhat,
        ISNULL(SUM(subsummary.CountSubmission), 0) AS SoBaiDaCham,
        CASE
            WHEN AVG(e.AverageScore * 1.0) >= 8.5 THEN N'Giỏi'
            WHEN AVG(e.AverageScore * 1.0) >= 7.0 THEN N'Khá'
            WHEN AVG(e.AverageScore * 1.0) >= 5.0 THEN N'Trung bình'
            ELSE N'Kém'
        END AS XepLoai
    FROM Enrollments e
    LEFT JOIN (
        SELECT StudentID, MAX(Score) AS MaxScore, COUNT(SubmissionID) AS CountSubmission
        FROM Submissions
        WHERE Score IS NOT NULL
        GROUP BY StudentID
    ) subsummary ON e.StudentID = subsummary.StudentID
    WHERE e.StudentID = @StudentID;
END;
GO

PRINT '✅ Đã tạo Stored Procedure [sp_GetStudentScoreSummary].';
GO

PRINT '';
PRINT '========================================';
PRINT 'HOÀN TẤT TẠO CẤU TRÚC DATABASE!';
PRINT '========================================';
PRINT '✅ Database Structure:';
PRINT '   - 11 Tables';
PRINT '   - 1 Function';
PRINT '   - 8 Stored Procedures';
PRINT '';
PRINT '🎉 Database StudyProcess đã sẵn sàng!';
PRINT '🎉 Sẵn sàng import dữ liệu mẫu!';
GO
