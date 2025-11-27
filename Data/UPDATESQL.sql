/* ======================
   BẢNG TÀI KHOẢN & NGƯỜI DÙNG
   ====================== */
-- ============================================

ALTER DATABASE StudyProcess
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

DROP DATABASE StudyProcess;

-- Tạo database
CREATE DATABASE [StudyProcess];
GO

USE [StudyProcess];
GO

CREATE TABLE Accounts (
    AccountID      VARCHAR(50)   NOT NULL PRIMARY KEY,
    Email          VARCHAR(100)  NOT NULL,
    PasswordHash   VARCHAR(255)  NOT NULL,
    Role           NVARCHAR(20)  NULL,      -- Student / Teacher / Admin
    IsActive       BIT           NOT NULL DEFAULT 1,
    CreatedAt      DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Users (
    UserID        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AccountID     VARCHAR(50)   NOT NULL,
    FullName      NVARCHAR(100) NOT NULL,
    PhoneNumber   VARCHAR(20)   NULL,
    AvatarUrl     NVARCHAR(255) NULL,
    DateOfBirth   DATE          NULL,
    Address       NVARCHAR(255) NULL,
    
    CONSTRAINT FK_Users_Accounts
        FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID)
);
GO

/* ======================
   DANH MỤC & KHÓA HỌC
   ====================== */

CREATE TABLE Categories (
    CategoryID    INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CategoryName  NVARCHAR(100) NOT NULL,
    Description   NVARCHAR(255) NULL
);
GO

-- ✅ QUAN TRỌNG: TeacherID là VARCHAR(50) và tham chiếu đến Accounts.AccountID
CREATE TABLE Courses (
    CourseID      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CourseName    NVARCHAR(200) NOT NULL,
    Description   NVARCHAR(MAX) NULL,
    ImageCover    NVARCHAR(255) NULL,
    TotalLessons  INT           NOT NULL DEFAULT 0,
    CategoryID    INT           NOT NULL,
    TeacherID     VARCHAR(50)   NULL,              -- ✅ Đã sửa: VARCHAR(50)
    CreatedAt     DATETIME      NOT NULL DEFAULT GETDATE(),
    Status        NVARCHAR(50)  NULL,             -- Active / Draft / Archived

    CONSTRAINT FK_Courses_Categories
        FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT FK_Courses_Accounts_Teacher
        FOREIGN KEY (TeacherID)  REFERENCES Accounts(AccountID)  -- ✅ Đã sửa
);
GO

/* ======================
   SECTIONS & LESSONS
   ====================== */

CREATE TABLE Sections (
    SectionID     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CourseID      INT           NOT NULL,
    SectionTitle  NVARCHAR(200) NOT NULL,
    SectionOrder  INT           NULL,
    Description   NVARCHAR(MAX) NULL,
    CreatedAt     DATETIME      NULL,

    CONSTRAINT FK_Sections_Courses
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);
GO

CREATE TABLE Lessons (
    LessonID      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CourseID      INT           NOT NULL,
    LessonTitle   NVARCHAR(200) NOT NULL,
    LessonOrder   INT           NULL,
    Content       NVARCHAR(MAX) NULL,
    VideoUrl      NVARCHAR(500) NULL,
    AttachmentUrl NVARCHAR(500) NULL,
    SectionID     INT           NULL,

    CONSTRAINT FK_Lessons_Courses
        FOREIGN KEY (CourseID)  REFERENCES Courses(CourseID),
    CONSTRAINT FK_Lessons_Sections
        FOREIGN KEY (SectionID) REFERENCES Sections(SectionID)
);
GO

/* ======================
   BÀI TẬP, GHI DANH, NỘP BÀI
   ====================== */

CREATE TABLE Assignments (
    AssignmentID  INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CourseID      INT           NOT NULL,
    Title         NVARCHAR(200) NOT NULL,
    Description   NVARCHAR(MAX) NULL,
    AssignedDate  DATETIME      NULL,
    DueDate       DATETIME      NULL,
    MaxScore      DECIMAL(5,2)  NULL,
    AttachmentPath NVARCHAR(500) NULL,

    CONSTRAINT FK_Assignments_Courses
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);
GO

CREATE TABLE Enrollments (
    EnrollmentID     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    StudentID        INT           NOT NULL,          -- Users.UserID (role = Student)
    CourseID         INT           NOT NULL,
    EnrollmentDate   DATETIME      NOT NULL DEFAULT GETDATE(),
    ProgressPercent  INT           NULL,
    CompletedLessons INT           NULL,
    Status           NVARCHAR(50)  NULL,
    AverageScore     DECIMAL(5,2)  NULL,

    CONSTRAINT FK_Enrollments_Users
        FOREIGN KEY (StudentID) REFERENCES Users(UserID),
    CONSTRAINT FK_Enrollments_Courses
        FOREIGN KEY (CourseID)  REFERENCES Courses(CourseID)
);
GO

CREATE TABLE Submissions (
    SubmissionID    INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AssignmentID    INT           NOT NULL,
    StudentID       INT           NOT NULL,
    SubmissionDate  DATETIME      NULL,
    FileUrl         NVARCHAR(500) NULL,
    StudentNote     NVARCHAR(MAX) NULL,
    Score           DECIMAL(5,2)  NULL,
    TeacherFeedback NVARCHAR(MAX) NULL,
    Status          NVARCHAR(50)  NULL,

    CONSTRAINT FK_Submissions_Assignments
        FOREIGN KEY (AssignmentID) REFERENCES Assignments(AssignmentID),
    CONSTRAINT FK_Submissions_Users
        FOREIGN KEY (StudentID)    REFERENCES Users(UserID)
);
GO

/* ======================
   NHẬT KÝ HOẠT ĐỘNG
   ====================== */

CREATE TABLE ActivityLogs (
    LogID               INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID              INT           NOT NULL,
    ActivityType        NVARCHAR(100) NOT NULL,
    ActivityDescription NVARCHAR(MAX) NULL,
    RelatedEntityType   NVARCHAR(50)  NULL,
    RelatedEntityID     INT           NULL,
    CreatedAt           DATETIME      NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_ActivityLogs_Users
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

PRINT '✅ Đã tạo tất cả các bảng thành công!';
GO

/* ============================================
   STORED PROCEDURES
   ============================================ */

-- ============================================
-- 1. sp_GetCourseStructure
-- ============================================
IF OBJECT_ID('sp_GetCourseStructure', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetCourseStructure
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

-- ============================================
-- 2. sp_GetTeacherCourses
-- ✅ Đã sửa: @TeacherID là VARCHAR(50)
-- ============================================
IF OBJECT_ID('sp_GetTeacherCourses', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTeacherCourses
GO

CREATE PROCEDURE sp_GetTeacherCourses
    @TeacherID VARCHAR(50)  -- ✅ Đã sửa
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

-- ============================================
-- 3. sp_GetAllSubmissions
-- ✅ Đã sửa: @TeacherID là VARCHAR(50)
-- ============================================
IF OBJECT_ID('sp_GetAllSubmissions', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllSubmissions
GO

CREATE PROCEDURE sp_GetAllSubmissions
    @TeacherID VARCHAR(50),  -- ✅ Đã sửa
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

-- ============================================
-- 4. spGradeSubmission
-- ✅ Đã sửa: SET NOCOUNT OFF để trả về rows affected
-- ============================================
IF OBJECT_ID('spGradeSubmission', 'P') IS NOT NULL
    DROP PROCEDURE spGradeSubmission
GO

CREATE PROCEDURE spGradeSubmission
    @SubmissionID INT,
    @Score DECIMAL(5,2),
    @TeacherFeedback NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT OFF;  -- ✅ Quan trọng: Để trả về số rows affected
    
    UPDATE Submissions
    SET Score = @Score,
        TeacherFeedback = @TeacherFeedback
    WHERE SubmissionID = @SubmissionID;
    
    -- Trả về số rows affected
    SELECT @@ROWCOUNT AS AffectedRows;
END;
GO

PRINT '✅ Đã tạo tất cả stored procedures thành công!';
PRINT '';
GO

/* ============================================
   DỮ LIỆU MẪU ĐẦY ĐỦ
   ============================================ */

PRINT '========================================';
PRINT 'Bắt đầu tạo dữ liệu mẫu...';
PRINT '========================================';
GO

-- 1. TẠO TÀI KHOẢN
INSERT INTO Accounts (AccountID, Email, PasswordHash, Role, IsActive, CreatedAt)
VALUES 
    ('USR001', 'admin@lms.com', '123456', 'Admin', 1, GETDATE()),
    ('USR002', 'teacher1@lms.com', '123456', 'Teacher', 1, GETDATE()),
    ('USR003', 'teacher2@lms.com', 'hashed_password_teacher2', 'Teacher', 1, GETDATE()),
    ('STU001', 'student001@lms.com', '45678910', 'Student', 1, GETDATE()),
    ('STU002', 'student002@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU003', 'student003@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU004', 'student004@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU005', 'student005@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU006', 'student006@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU007', 'student007@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU008', 'student008@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU009', 'student009@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU010', 'student010@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU011', 'student011@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU012', 'student012@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU013', 'student013@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU014', 'student014@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU015', 'student015@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU016', 'student016@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU017', 'student017@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU018', 'student018@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU019', 'student019@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU020', 'student020@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU021', 'student021@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU022', 'student022@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU023', 'student023@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU024', 'student024@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU025', 'student025@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU026', 'student026@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU027', 'student027@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU028', 'student028@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU029', 'student029@lms.com', 'hashed_password', 'Student', 1, GETDATE()),
    ('STU030', 'student030@lms.com', 'hashed_password', 'Student', 1, GETDATE());
GO
                
               
-- 2. TẠO USERS
INSERT INTO Users (AccountID, FullName, PhoneNumber, DateOfBirth, Address)
VALUES 
    ('USR001', N'Quản trị viên hệ thống', '0900000000', '1990-01-01', N'Hà Nội'),
    ('USR002', N'Nguyễn Văn A', '0901234567', '1985-03-15', N'Hồ Chí Minh'),
    ('USR003', N'Trần Thị B', '0912345678', '1988-07-20', N'Đà Nẵng'),
    ('STU001', N'Lê Minh Tuấn', '0901111111', '2000-01-15', N'Hà Nội'),
    ('STU002', N'Phạm Thu Hương', '0902222222', '2001-02-20', N'TP.HCM'),
    ('STU003', N'Hoàng Văn Nam', '0903333333', '2000-03-10', N'Đà Nẵng'),
    ('STU004', N'Nguyễn Thị Mai', '0904444444', '2001-04-05', N'Hải Phòng'),
    ('STU005', N'Trần Đức Anh', '0905555555', '2000-05-25', N'Cần Thơ'),
    ('STU006', N'Võ Thị Lan', '0906666666', '2001-06-30', N'Huế'),
    ('STU007', N'Đặng Minh Quân', '0907777777', '2000-07-12', N'Hà Nội'),
    ('STU008', N'Bùi Thu Trang', '0908888888', '2001-08-18', N'TP.HCM'),
    ('STU009', N'Phan Văn Hải', '0909999999', '2000-09-22', N'Nha Trang'),
    ('STU010', N'Lý Thị Hoa', '0910101010', '2001-10-28', N'Vũng Tàu'),
    ('STU011', N'Đinh Văn Tùng', '0911111111', '2000-11-08', N'Hà Nội'),
    ('STU012', N'Mai Thu Hà', '0912121212', '2001-12-14', N'TP.HCM'),
    ('STU013', N'Dương Minh Khang', '0913131313', '2000-01-20', N'Đà Nẵng'),
    ('STU014', N'Vũ Thị Ngọc', '0914141414', '2001-02-25', N'Hải Phòng'),
    ('STU015', N'Trương Văn Đạt', '0915151515', '2000-03-30', N'Cần Thơ'),
    ('STU016', N'Cao Thị Linh', '0916161616', '2001-04-10', N'Huế'),
    ('STU017', N'Lâm Minh Tâm', '0917171717', '2000-05-15', N'Hà Nội'),
    ('STU018', N'Hồ Thu Hiền', '0918181818', '2001-06-20', N'TP.HCM'),
    ('STU019', N'Đỗ Văn Long', '0919191919', '2000-07-25', N'Đà Nẵng'),
    ('STU020', N'Ngô Thị Phương', '0920202020', '2001-08-30', N'Nha Trang'),
    ('STU021', N'Tạ Văn Bình', '0921212121', '2000-09-05', N'Hà Nội'),
    ('STU022', N'Châu Thị Kim', '0922222222', '2001-10-10', N'TP.HCM'),
    ('STU023', N'Lưu Minh Đức', '0923232323', '2000-11-15', N'Đà Nẵng'),
    ('STU024', N'Phùng Thị Yến', '0924242424', '2001-12-20', N'Hải Phòng'),
    ('STU025', N'Khúc Văn Huy', '0925252525', '2000-01-25', N'Cần Thơ'),
    ('STU026', N'La Thị Nga', '0926262626', '2001-02-28', N'Huế'),
    ('STU027', N'Từ Minh Nhật', '0927272727', '2000-03-05', N'Hà Nội'),
    ('STU028', N'Ông Thị Tâm', '0928282828', '2001-04-15', N'TP.HCM'),
    ('STU029', N'Tô Văn Phong', '0929292929', '2000-05-20', N'Đà Nẵng'),
    ('STU030', N'Lạc Thị Thủy', '0930303030', '2001-06-25', N'Nha Trang');
GO

-- 3. TẠO CATEGORIES
INSERT INTO Categories (CategoryName, Description)
VALUES 
    (N'Lập trình', N'Các khóa học về lập trình và phát triển phần mềm'),
    (N'Thiết kế', N'Các khóa học về thiết kế đồ họa và UI/UX'),
    (N'Marketing', N'Các khóa học về marketing và kinh doanh'),
    (N'Ngoại ngữ', N'Các khóa học ngoại ngữ');
GO

-- 4. TẠO COURSES
INSERT INTO Courses (CourseName, Description, CategoryID, TeacherID, Status, TotalLessons, CreatedAt)
VALUES 
    (N'Lập trình C# cơ bản', N'Khóa học C# từ cơ bản đến nâng cao, phù hợp cho người mới bắt đầu', 1, 'USR002', N'Active', 24, DATEADD(DAY, -60, GETDATE())),
    (N'Lập trình Web với ASP.NET Core', N'Xây dựng ứng dụng web hiện đại với ASP.NET Core MVC', 1, 'USR002', N'Active', 30, DATEADD(DAY, -45, GETDATE())),
    (N'Thiết kế UI/UX chuyên nghiệp', N'Học thiết kế giao diện người dùng và trải nghiệm người dùng', 2, 'USR003', N'Active', 20, DATEADD(DAY, -30, GETDATE()));
GO
select * from Courses;
-- 5. TẠO SECTIONS
INSERT INTO Sections (CourseID, SectionTitle, SectionOrder, Description, CreatedAt)
VALUES 
    -- Khóa 1: C#
    (1, N'Giới thiệu về C#', 1, N'Tổng quan về ngôn ngữ lập trình C# và .NET Framework', GETDATE()),
    (1, N'Cú pháp cơ bản', 2, N'Các cú pháp cơ bản trong C#: biến, kiểu dữ liệu, toán tử', GETDATE()),
    (1, N'Cấu trúc điều khiển', 3, N'If-else, switch-case, vòng lặp', GETDATE()),
    (1, N'Lập trình hướng đối tượng', 4, N'Class, Object, Inheritance, Polymorphism', GETDATE()),
    (1, N'Xử lý ngoại lệ và File', 5, N'Exception handling và làm việc với file', GETDATE()),
    (1, N'Collection và LINQ', 6, N'List, Dictionary, LINQ queries', GETDATE()),
    -- Khóa 2: ASP.NET
    (2, N'Cài đặt môi trường', 1, N'Cài đặt Visual Studio và .NET SDK', GETDATE()),
    (2, N'MVC Pattern', 2, N'Hiểu về Model-View-Controller', GETDATE()),
    (2, N'Routing và Controllers', 3, N'Xử lý routing và tạo controllers', GETDATE()),
    (2, N'Views và Razor', 4, N'Tạo giao diện với Razor syntax', GETDATE()),
    (2, N'Entity Framework Core', 5, N'Làm việc với database sử dụng EF Core', GETDATE()),
    (2, N'Authentication & Authorization', 6, N'Xác thực và phân quyền người dùng', GETDATE()),
    -- Khóa 3: UI/UX
    (3, N'Nguyên tắc thiết kế', 1, N'Các nguyên tắc cơ bản trong thiết kế UI/UX', GETDATE()),
    (3, N'Nghiên cứu người dùng', 2, N'Phương pháp nghiên cứu và phân tích người dùng', GETDATE()),
    (3, N'Wireframe và Prototype', 3, N'Tạo wireframe và prototype với Figma', GETDATE()),
    (3, N'Design System', 4, N'Xây dựng hệ thống thiết kế nhất quán', GETDATE()),
    (3, N'Usability Testing', 5, N'Kiểm tra tính khả dụng của sản phẩm', GETDATE());
GO

-- 6. TẠO LESSONS (74 lessons total)
INSERT INTO Lessons (CourseID, SectionID, LessonTitle, LessonOrder, Content, VideoUrl)
VALUES 
    -- Section 1: Giới thiệu về C# (4 lessons)
    (1, 1, N'C# là gì?', 1, N'Giới thiệu về ngôn ngữ C# và lịch sử phát triển', 'https://youtube.com/video1'),
    (1, 1, N'Cài đặt Visual Studio', 2, N'Hướng dẫn cài đặt môi trường phát triển', 'https://youtube.com/video2'),
    (1, 1, N'.NET Framework vs .NET Core', 3, N'So sánh giữa .NET Framework và .NET Core', 'https://youtube.com/video3'),
    (1, 1, N'Chương trình Hello World', 4, N'Viết chương trình C# đầu tiên', 'https://youtube.com/video4'),
    
    -- Section 2: Cú pháp cơ bản (4 lessons)
    (1, 2, N'Biến và hằng số', 1, N'Cách khai báo và sử dụng biến, hằng số', 'https://youtube.com/video5'),
    (1, 2, N'Kiểu dữ liệu', 2, N'Các kiểu dữ liệu cơ bản: int, string, bool...', 'https://youtube.com/video6'),
    (1, 2, N'Toán tử', 3, N'Toán tử số học, logic, so sánh', 'https://youtube.com/video7'),
    (1, 2, N'Chuyển đổi kiểu dữ liệu', 4, N'Type casting và conversion', 'https://youtube.com/video8'),
    
    -- Section 3: Cấu trúc điều khiển (4 lessons)
    (1, 3, N'Câu lệnh If-Else', 1, N'Cấu trúc điều kiện if-else', 'https://youtube.com/video9'),
    (1, 3, N'Switch-Case', 2, N'Sử dụng switch-case', 'https://youtube.com/video10'),
    (1, 3, N'Vòng lặp For', 3, N'Vòng lặp for và nested loop', 'https://youtube.com/video11'),
    (1, 3, N'Vòng lặp While', 4, N'While và do-while loop', 'https://youtube.com/video12'),
    
    -- Section 4: OOP (4 lessons)
    (1, 4, N'Class và Object', 1, N'Khái niệm class và object', 'https://youtube.com/video13'),
    (1, 4, N'Properties và Methods', 2, N'Thuộc tính và phương thức', 'https://youtube.com/video14'),
    (1, 4, N'Constructor', 3, N'Hàm khởi tạo trong C#', 'https://youtube.com/video15'),
    (1, 4, N'Inheritance', 4, N'Kế thừa trong OOP', 'https://youtube.com/video16'),
    
    -- Section 5: Exception & File (4 lessons)
    (1, 5, N'Try-Catch', 1, N'Xử lý ngoại lệ với try-catch', 'https://youtube.com/video17'),
    (1, 5, N'Đọc File', 2, N'Đọc dữ liệu từ file text', 'https://youtube.com/video18'),
    (1, 5, N'Ghi File', 3, N'Ghi dữ liệu vào file', 'https://youtube.com/video19'),
    (1, 5, N'Using Statement', 4, N'Quản lý tài nguyên với using', 'https://youtube.com/video20'),
    
    -- Section 6: Collection & LINQ (4 lessons)
    (1, 6, N'List Collection', 1, N'Làm việc với List<T>', 'https://youtube.com/video21'),
    (1, 6, N'Dictionary', 2, N'Key-Value pair với Dictionary', 'https://youtube.com/video22'),
    (1, 6, N'LINQ Queries', 3, N'Truy vấn dữ liệu với LINQ', 'https://youtube.com/video23'),
    (1, 6, N'Lambda Expression', 4, N'Biểu thức Lambda trong C#', 'https://youtube.com/video24'),
    
    -- Section 7-12: ASP.NET (30 lessons)
    (2, 7, N'Cài đặt .NET SDK', 1, N'Download và cài đặt .NET SDK', 'https://youtube.com/asp1'),
    (2, 7, N'Visual Studio 2022', 2, N'Cài đặt và cấu hình VS 2022', 'https://youtube.com/asp2'),
    (2, 7, N'Tạo project đầu tiên', 3, N'Khởi tạo ASP.NET Core MVC project', 'https://youtube.com/asp3'),
    (2, 7, N'Cấu trúc project', 4, N'Tìm hiểu cấu trúc thư mục', 'https://youtube.com/asp4'),
    (2, 7, N'Chạy ứng dụng', 5, N'Build và run ứng dụng web', 'https://youtube.com/asp5'),
    (2, 8, N'MVC là gì?', 1, N'Giới thiệu mô hình MVC', 'https://youtube.com/asp6'),
    (2, 8, N'Model', 2, N'Tạo và sử dụng Model', 'https://youtube.com/asp7'),
    (2, 8, N'View', 3, N'Tạo View với Razor', 'https://youtube.com/asp8'),
    (2, 8, N'Controller', 4, N'Xử lý logic với Controller', 'https://youtube.com/asp9'),
    (2, 8, N'Data Flow', 5, N'Luồng dữ liệu trong MVC', 'https://youtube.com/asp10'),
    (2, 9, N'Route Configuration', 1, N'Cấu hình routing', 'https://youtube.com/asp11'),
    (2, 9, N'Action Methods', 2, N'Các phương thức action', 'https://youtube.com/asp12'),
    (2, 9, N'Parameters', 3, N'Truyền tham số cho action', 'https://youtube.com/asp13'),
    (2, 9, N'Attribute Routing', 4, N'Routing với attributes', 'https://youtube.com/asp14'),
    (2, 9, N'ActionResult Types', 5, N'Các kiểu trả về của action', 'https://youtube.com/asp15'),
    (2, 10, N'Razor Syntax', 1, N'Cú pháp Razor cơ bản', 'https://youtube.com/asp16'),
    (2, 10, N'Layout Pages', 2, N'Sử dụng Layout', 'https://youtube.com/asp17'),
    (2, 10, N'Partial Views', 3, N'Tạo Partial View', 'https://youtube.com/asp18'),
    (2, 10, N'ViewData & ViewBag', 4, N'Truyền dữ liệu sang View', 'https://youtube.com/asp19'),
    (2, 10, N'Tag Helpers', 5, N'Sử dụng Tag Helpers', 'https://youtube.com/asp20'),
    (2, 11, N'EF Core Overview', 1, N'Giới thiệu Entity Framework Core', 'https://youtube.com/asp21'),
    (2, 11, N'DbContext', 2, N'Tạo DbContext class', 'https://youtube.com/asp22'),
    (2, 11, N'Code First', 3, N'Code First approach', 'https://youtube.com/asp23'),
    (2, 11, N'Migrations', 4, N'Database migrations', 'https://youtube.com/asp24'),
    (2, 11, N'CRUD Operations', 5, N'Thao tác CRUD với EF Core', 'https://youtube.com/asp25'),
    (2, 12, N'Identity Setup', 1, N'Cài đặt ASP.NET Identity', 'https://youtube.com/asp26'),
    (2, 12, N'User Registration', 2, N'Đăng ký người dùng', 'https://youtube.com/asp27'),
    (2, 12, N'Login & Logout', 3, N'Đăng nhập và đăng xuất', 'https://youtube.com/asp28'),
    (2, 12, N'Role Management', 4, N'Quản lý vai trò', 'https://youtube.com/asp29'),
    (2, 12, N'Authorization', 5, N'Phân quyền truy cập', 'https://youtube.com/asp30'),
    
    -- Section 13-17: UI/UX (20 lessons)
    (3, 13, N'Nguyên tắc đơn giản', 1, N'Thiết kế đơn giản và rõ ràng', 'https://youtube.com/uiux1'),
    (3, 13, N'Hierarchy', 2, N'Tạo thứ bậc thông tin', 'https://youtube.com/uiux2'),
    (3, 13, N'Màu sắc', 3, N'Lý thuyết màu sắc trong thiết kế', 'https://youtube.com/uiux3'),
    (3, 13, N'Typography', 4, N'Nghệ thuật chữ viết', 'https://youtube.com/uiux4'),
    (3, 14, N'User Interview', 1, N'Phỏng vấn người dùng', 'https://youtube.com/uiux5'),
    (3, 14, N'User Persona', 2, N'Tạo user persona', 'https://youtube.com/uiux6'),
    (3, 14, N'User Journey Map', 3, N'Bản đồ hành trình người dùng', 'https://youtube.com/uiux7'),
    (3, 14, N'Analytics', 4, N'Phân tích dữ liệu người dùng', 'https://youtube.com/uiux8'),
    (3, 15, N'Low-fidelity Wireframe', 1, N'Wireframe độ trung thực thấp', 'https://youtube.com/uiux9'),
    (3, 15, N'High-fidelity Wireframe', 2, N'Wireframe độ trung thực cao', 'https://youtube.com/uiux10'),
    (3, 15, N'Figma Basics', 3, N'Cơ bản về Figma', 'https://youtube.com/uiux11'),
    (3, 15, N'Interactive Prototype', 4, N'Tạo prototype tương tác', 'https://youtube.com/uiux12'),
    (3, 16, N'Component Library', 1, N'Thư viện component', 'https://youtube.com/uiux13'),
    (3, 16, N'Style Guide', 2, N'Hướng dẫn phong cách', 'https://youtube.com/uiux14'),
    (3, 16, N'Design Tokens', 3, N'Tokens trong thiết kế', 'https://youtube.com/uiux15'),
    (3, 16, N'Documentation', 4, N'Tài liệu hóa design system', 'https://youtube.com/uiux16'),
    (3, 17, N'Test Planning', 1, N'Lập kế hoạch kiểm tra', 'https://youtube.com/uiux17'),
    (3, 17, N'A/B Testing', 2, N'Kiểm tra A/B', 'https://youtube.com/uiux18'),
    (3, 17, N'Heuristic Evaluation', 3, N'Đánh giá heuristic', 'https://youtube.com/uiux19'),
    (3, 17, N'Iteration', 4, N'Cải tiến liên tục', 'https://youtube.com/uiux20');
GO

-- 7. TẠO ENROLLMENTS
INSERT INTO Enrollments (StudentID, CourseID, EnrollmentDate, ProgressPercent, Status)
SELECT UserID, 1, DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 50), GETDATE()), 
       (ABS(CHECKSUM(NEWID()) % 100)), N'Active'
FROM Users 
WHERE AccountID IN ('STU001','STU002','STU003','STU004','STU005','STU006','STU007','STU008','STU009','STU010',
                    'STU011','STU012','STU013','STU014','STU015','STU016','STU017','STU018','STU019','STU020');

INSERT INTO Enrollments (StudentID, CourseID, EnrollmentDate, ProgressPercent, Status)
SELECT UserID, 2, DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 40), GETDATE()), 
       (ABS(CHECKSUM(NEWID()) % 100)), N'Active'
FROM Users 
WHERE AccountID IN ('STU005','STU006','STU007','STU008','STU009','STU010','STU011','STU012','STU013','STU014',
                    'STU015','STU021','STU022','STU023','STU024');

INSERT INTO Enrollments (StudentID, CourseID, EnrollmentDate, ProgressPercent, Status)
SELECT UserID, 3, DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 25), GETDATE()), 
       (ABS(CHECKSUM(NEWID()) % 100)), N'Active'
FROM Users 
WHERE AccountID IN ('STU010','STU011','STU012','STU013','STU014','STU015','STU016','STU017','STU018','STU019',
                    'STU020','STU025','STU026','STU027','STU028','STU029','STU030','STU003');

-- ============================================
-- CẬP NHẬT TRẠNG THÁI ENROLLMENTS
-- ============================================

-- B1: mặc định coi tất cả là đang học
UPDATE Enrollments
SET Status = N'Learning'
WHERE Status IS NULL OR Status = N'Active';

-- B2: một số bạn đã hoàn thành (ví dụ: tiến độ >= 80%)
UPDATE Enrollments
SET Status = N'Completed'
WHERE ProgressPercent >= 80;

-- B3: một số bạn tạm dừng (ví dụ: tiến độ <= 20% và đăng ký đã lâu)
UPDATE Enrollments
SET Status = N'Suspended'
WHERE ProgressPercent <= 20
  AND DATEDIFF(DAY, EnrollmentDate, GETDATE()) > 30;

GO

-- 8. TẠO ASSIGNMENTS
INSERT INTO Assignments (CourseID, Title, Description, AssignedDate, DueDate, MaxScore)
VALUES 
    (1, N'Bài tập 1: Hello World', N'Viết chương trình in ra "Hello World" bằng C#', DATEADD(DAY, -50, GETDATE()), DATEADD(DAY, -43, GETDATE()), 10.00),
    (1, N'Bài tập 2: Tính toán cơ bản', N'Viết chương trình tính tổng, hiệu, tích, thương', DATEADD(DAY, -40, GETDATE()), DATEADD(DAY, -33, GETDATE()), 15.00),
    (1, N'Bài tập 3: Vòng lặp', N'In ra các số từ 1 đến 100 sử dụng vòng lặp', DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -23, GETDATE()), 20.00),
    (1, N'Bài tập 4: Class và Object', N'Tạo class Student với các thuộc tính và phương thức', DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -13, GETDATE()), 25.00),
    (1, N'Bài tập 5: LINQ', N'Sử dụng LINQ để truy vấn danh sách', DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -3, GETDATE()), 30.00),
    (2, N'Bài tập 1: Tạo Controller', N'Tạo HomeController với các action cơ bản', DATEADD(DAY, -35, GETDATE()), DATEADD(DAY, -28, GETDATE()), 20.00),
    (2, N'Bài tập 2: Razor Views', N'Tạo View hiển thị danh sách sản phẩm', DATEADD(DAY, -25, GETDATE()), DATEADD(DAY, -18, GETDATE()), 25.00),
    (2, N'Bài tập 3: Entity Framework', N'Tạo Model và DbContext để kết nối database', DATEADD(DAY, -15, GETDATE()), DATEADD(DAY, -8, GETDATE()), 30.00),
    (2, N'Bài tập 4: CRUD Operations', N'Xây dựng chức năng CRUD hoàn chỉnh', DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, 2, GETDATE()), 35.00),
    (3, N'Bài tập 1: User Persona', N'Tạo 3 user persona cho một ứng dụng di động', DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -13, GETDATE()), 20.00),
    (3, N'Bài tập 2: Wireframe', N'Thiết kế wireframe cho trang chủ website', DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -3, GETDATE()), 30.00),
    (3, N'Bài tập 3: Prototype', N'Tạo prototype tương tác với Figma', DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, 2, GETDATE()), 40.00);
GO

-- 9. TẠO SUBMISSIONS
DECLARE @StudentID INT, @AssignmentID INT, @Score DECIMAL(5,2), @IsLate BIT, @IsGraded BIT;
DECLARE @Counter INT = 1;
DECLARE @SubmissionDate DATETIME, @DueDate DATETIME;

WHILE @Counter <= 80
BEGIN
    SELECT TOP 1 
        @StudentID = e.StudentID, 
        @AssignmentID = a.AssignmentID,
        @DueDate = a.DueDate
    FROM Assignments a
    INNER JOIN Enrollments e ON a.CourseID = e.CourseID
    WHERE NOT EXISTS (
        SELECT 1 FROM Submissions s 
        WHERE s.AssignmentID = a.AssignmentID AND s.StudentID = e.StudentID
    )
    ORDER BY NEWID();
    
    IF @StudentID IS NOT NULL AND @AssignmentID IS NOT NULL
    BEGIN
        SET @IsLate = CASE WHEN ABS(CHECKSUM(NEWID())) % 100 < 20 THEN 1 ELSE 0 END;
        SET @IsGraded = CASE WHEN ABS(CHECKSUM(NEWID())) % 100 < 70 THEN 1 ELSE 0 END;
        SET @Score = CASE WHEN @IsGraded = 1 THEN (5 + (ABS(CHECKSUM(NEWID())) % 6) + (ABS(CHECKSUM(NEWID())) % 10) * 0.1) ELSE NULL END;
        
        SET @SubmissionDate = CASE 
            WHEN @IsLate = 1 THEN DATEADD(DAY, 2, @DueDate)
            ELSE DATEADD(DAY, -1, @DueDate)
        END;
        
        INSERT INTO Submissions (AssignmentID, StudentID, SubmissionDate, FileUrl, StudentNote, Score, TeacherFeedback, Status)
        VALUES (
            @AssignmentID,
            @StudentID,
            @SubmissionDate,
            'C:\Submissions\file_' + CAST(@Counter AS VARCHAR) + '.zip',
            N'Em đã hoàn thành bài tập. Mong thầy xem xét.',
            @Score,
            CASE 
                WHEN @IsGraded = 1 THEN N'Bài làm tốt. Cần chú ý thêm về phần...'
                ELSE NULL
            END,
            CASE 
                WHEN @IsGraded = 1 THEN N'Đã chấm'
                WHEN @IsLate = 1 THEN N'Nộp trễ'
                ELSE N'Chưa chấm'
            END
        );
    END
    
    SET @Counter = @Counter + 1;
END
GO
ALTER TABLE Users DROP CONSTRAINT FK_Users_Accounts;
GO

-- 2. Tạo lại ràng buộc mới có thêm tính năng "ON DELETE CASCADE"
ALTER TABLE Users ADD CONSTRAINT FK_Users_Accounts
FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID)
ON DELETE CASCADE;

ALTER TABLE Enrollments DROP CONSTRAINT FK_Enrollments_Users;
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Users
    FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE;
GO

-- 2. Cho phép xóa User -> tự động xóa Submissions (Bài nộp)
ALTER TABLE Submissions DROP CONSTRAINT FK_Submissions_Users;
ALTER TABLE Submissions ADD CONSTRAINT FK_Submissions_Users
    FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE;
GO

-- 3. Cho phép xóa User -> tự động xóa ActivityLogs (Nhật ký)
ALTER TABLE ActivityLogs DROP CONSTRAINT FK_ActivityLogs_Users;
ALTER TABLE ActivityLogs ADD CONSTRAINT FK_ActivityLogs_Users
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE;
GO
PRINT '';
PRINT '========================================';
PRINT 'HOÀN TẤT TẠO DATABASE!';
PRINT '========================================';
PRINT '✅ Database Structure:';
PRINT '   - 10 Tables';
PRINT '   - 4 Stored Procedures';
PRINT '';
PRINT '✅ Sample Data:';
PRINT '   - 33 Accounts (3 Teachers + 30 Students)';
PRINT '   - 33 Users';
PRINT '   - 4 Categories';
PRINT '   - 3 Courses';
PRINT '   - 17 Sections';
PRINT '   - 74 Lessons';
PRINT '   - 53 Enrollments';
PRINT '   - 12 Assignments';
PRINT '   - ~80 Submissions';
PRINT '';
PRINT '🎉 Database StudyProcess đã sẵn sàng!';
PRINT '🎉 Có thể test ứng dụng WinForms ngay!';
GO
ALTER TABLE Users DROP CONSTRAINT FK_Users_Accounts;
GO

-- 2. Tạo lại ràng buộc mới có thêm tính năng "ON DELETE CASCADE"
ALTER TABLE Users ADD CONSTRAINT FK_Users_Accounts
FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID)
ON DELETE CASCADE;

ALTER TABLE Enrollments DROP CONSTRAINT FK_Enrollments_Users;
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Users
    FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE;
GO

-- 2. Cho phép xóa User -> tự động xóa Submissions (Bài nộp)
ALTER TABLE Submissions DROP CONSTRAINT FK_Submissions_Users;
ALTER TABLE Submissions ADD CONSTRAINT FK_Submissions_Users
    FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE;
GO

-- 3. Cho phép xóa User -> tự động xóa ActivityLogs (Nhật ký)
ALTER TABLE ActivityLogs DROP CONSTRAINT FK_ActivityLogs_Users;
ALTER TABLE ActivityLogs ADD CONSTRAINT FK_ActivityLogs_Users
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE;
GO
-- 1. Xử lý bảng Sections (Chương) - Nguyên nhân lỗi hiện tại
ALTER TABLE Sections DROP CONSTRAINT FK_Sections_Courses;
ALTER TABLE Sections ADD CONSTRAINT FK_Sections_Courses
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE;
GO

-- 2. Xử lý bảng Enrollments (Học viên đăng ký) - Tránh lỗi tiếp theo
ALTER TABLE Enrollments DROP CONSTRAINT FK_Enrollments_Courses;
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Courses
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE;
GO

-- 3. Xử lý bảng Assignments (Bài tập) - Tránh lỗi tiếp theo
ALTER TABLE Assignments DROP CONSTRAINT FK_Assignments_Courses;
ALTER TABLE Assignments ADD CONSTRAINT FK_Assignments_Courses
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE;
GO

-- 4. Xử lý bảng Lessons (Bài học) - Xóa Chương thì xóa luôn Bài
ALTER TABLE Lessons DROP CONSTRAINT FK_Lessons_Sections;
ALTER TABLE Lessons ADD CONSTRAINT FK_Lessons_Sections
    FOREIGN KEY (SectionID) REFERENCES Sections(SectionID) ON DELETE CASCADE;
GO
USE StudyProcess;
GO

-- Gỡ bỏ ràng buộc cũ gây lỗi
ALTER TABLE Submissions DROP CONSTRAINT FK_Submissions_Assignments;
GO

-- Thêm ràng buộc mới có tính năng TỰ ĐỘNG XÓA (Cascade)
ALTER TABLE Submissions ADD CONSTRAINT FK_Submissions_Assignments
    FOREIGN KEY (AssignmentID) REFERENCES Assignments(AssignmentID) ON DELETE CASCADE;

-- =============================================================
-- 1. CẬP NHẬT CẤU TRÚC BẢNG (Hỗ trợ Upload & Tiến độ)
-- =============================================================

-- Bảng Lessons: Thêm cột lưu tài liệu đính kèm (Binary)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Lessons') AND name = 'AttachmentData')
BEGIN
    ALTER TABLE Lessons
    ADD AttachmentData VARBINARY(MAX) NULL,
        AttachmentName NVARCHAR(255) NULL;
    PRINT '✅ Đã thêm cột AttachmentData vào bảng Lessons.';
END
GO

-- Bảng Assignments: Thêm cột lưu file đề bài (Binary)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assignments') AND name = 'AttachmentData')
BEGIN
    ALTER TABLE Assignments
    ADD AttachmentData VARBINARY(MAX) NULL,
        AttachmentName NVARCHAR(255) NULL;
    PRINT '✅ Đã thêm cột AttachmentData vào bảng Assignments.';
END
GO

-- Bảng Submissions: Thêm cột lưu bài làm sinh viên (Binary)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Submissions') AND name = 'FileData')
BEGIN
    ALTER TABLE Submissions
    ADD FileData VARBINARY(MAX) NULL,
        FileName NVARCHAR(255) NULL;
    PRINT '✅ Đã thêm cột FileData vào bảng Submissions.';
END
GO

-- Bảng LessonProgress: Lưu trạng thái hoàn thành từng bài học
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('LessonProgress') AND type in (N'U'))
BEGIN
    CREATE TABLE LessonProgress (
        ProgressID INT IDENTITY(1,1) PRIMARY KEY,
        StudentID INT NOT NULL,
        LessonID INT NOT NULL,
        IsCompleted BIT DEFAULT 0,
        CompletedDate DATETIME NULL,
        FOREIGN KEY (StudentID) REFERENCES Users(UserID) ON DELETE CASCADE,
        FOREIGN KEY (LessonID) REFERENCES Lessons(LessonID) ON DELETE CASCADE
    );
    PRINT '✅ Đã tạo bảng LessonProgress.';
END
GO

-- =============================================================
-- 2. FUNCTION HỖ TRỢ (Tạo trước vì được dùng trong SP)
-- =============================================================

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

-- =============================================================
-- 3. STORED PROCEDURES CHO STUDENT
-- =============================================================

-- 3.1. sp_GetAllResourcesForStudent
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
    INNER JOIN Sections s ON l.SectionID = s.SectionID
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
PRINT '✅ Đã tạo Procedure [sp_GetAllResourcesForStudent].';
GO

-- 3.2. sp_GetStudentAssignments
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
            ELSE 
                ISNULL(CAST(DATEDIFF(DAY, GETDATE(), a.DueDate) AS NVARCHAR) + N' ngày', N'Hôm nay')
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
PRINT '✅ Đã tạo Procedure [sp_GetStudentAssignments].';
GO

-- 3.3. sp_GetMyCoursesWithProgress
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
        (c.TotalLessons - ISNULL(e.CompletedLessons, 0)) AS SoBaiConLai,
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
PRINT '✅ Đã tạo Procedure [sp_GetMyCoursesWithProgress].';
GO

-- 3.4. sp_GetStudentScoreSummary
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
        ISNULL(MAX(sub_summary.MaxScore), 0) AS DiemCaoNhat,
        ISNULL(SUM(sub_summary.CountSubmission), 0) AS SoBaiDaCham,
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
    ) sub_summary ON e.StudentID = sub_summary.StudentID
    WHERE e.StudentID = @StudentID;
END;
GO
PRINT '✅ Đã tạo Procedure [sp_GetStudentScoreSummary].';
GO

PRINT '=============================================================';
PRINT '🎉 ĐÃ CẬP NHẬT THÀNH CÔNG CÁC THỦ TỤC CHO SINH VIÊN!';
PRINT '=============================================================';
