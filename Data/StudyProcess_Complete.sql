-- =============================================
-- DATABASE: StudyProcess - LMS (Learning Management System)
-- Phiên bản: Hoàn chỉnh cho Giảng viên
-- Chỉ có: Khóa học, Bài học, Bài tập (không có Quiz/Đồ án)
-- =============================================

-- 1. TẠO DATABASE
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'StudyProcess')
BEGIN
    ALTER DATABASE StudyProcess SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE StudyProcess;
END
GO

CREATE DATABASE StudyProcess;
GO

USE StudyProcess;
GO

-- =============================================
-- 2. TẠO CÁC BẢNG CƠ BẢN
-- =============================================

-- Bảng 1: Accounts (Quản lý đăng nhập & Phân quyền)
CREATE TABLE Accounts (
    AccountID VARCHAR(50) PRIMARY KEY,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Role NVARCHAR(20) CHECK (Role IN ('Admin', 'Teacher', 'Student')),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng 2: Users (Hồ sơ cá nhân)
CREATE TABLE Users (
    UserID VARCHAR(50) PRIMARY KEY,
    AccountID VARCHAR(50) UNIQUE NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20),
    AvatarUrl NVARCHAR(255),
    DateOfBirth DATE,
    Address NVARCHAR(255),
    FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID) ON DELETE CASCADE
);

-- Bảng 3: Categories (Danh mục khóa học)
CREATE TABLE Categories (
    CategoryID VARCHAR(50) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);

-- Bảng 4: Courses (Khóa học)
CREATE TABLE Courses (
    CourseID VARCHAR(50) PRIMARY KEY,
    CourseName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    ImageCover NVARCHAR(255),
    TotalLessons INT DEFAULT 0,
    CategoryID VARCHAR(50),
    TeacherID VARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Pending',
    
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (TeacherID) REFERENCES Users(UserID)
);

-- Bảng 5: Sections (Chương trong khóa học)
CREATE TABLE Sections (
    SectionID VARCHAR(50) PRIMARY KEY,
    CourseID VARCHAR(50) NOT NULL,
    SectionTitle NVARCHAR(200) NOT NULL,
    SectionOrder INT NOT NULL,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE
);

-- Bảng 6: Lessons (Bài học trong chương)
CREATE TABLE Lessons (
    LessonID VARCHAR(50) PRIMARY KEY,
    CourseID VARCHAR(50),
    SectionID VARCHAR(50),
    LessonTitle NVARCHAR(200) NOT NULL,
    LessonOrder INT,
    Content NVARCHAR(MAX),
    VideoUrl NVARCHAR(255),
    AttachmentUrl NVARCHAR(255),
    
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
    FOREIGN KEY (SectionID) REFERENCES Sections(SectionID) ON DELETE SET NULL
);

-- Bảng 7: Enrollments (Đăng ký học & Tiến độ)
CREATE TABLE Enrollments (
    EnrollmentID VARCHAR(50) PRIMARY KEY,
    StudentID VARCHAR(50),
    CourseID VARCHAR(50),
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    ProgressPercent INT DEFAULT 0,
    CompletedLessons INT DEFAULT 0,
    AverageScore DECIMAL(4,2) DEFAULT 0,
    Status NVARCHAR(50) DEFAULT 'Learning',
    
    FOREIGN KEY (StudentID) REFERENCES Users(UserID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);

-- Bảng 8: Assignments (Bài tập)
CREATE TABLE Assignments (
    AssignmentID VARCHAR(50) PRIMARY KEY,
    CourseID VARCHAR(50),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    AttachmentUrl NVARCHAR(255),
    AssignedDate DATETIME DEFAULT GETDATE(),
    DueDate DATETIME,
    MaxScore DECIMAL(4,2) DEFAULT 10.0,
    
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);

-- Bảng 9: Submissions (Bài nộp & Chấm điểm)
CREATE TABLE Submissions (
    SubmissionID VARCHAR(50) PRIMARY KEY,
    AssignmentID VARCHAR(50),
    StudentID VARCHAR(50),
    SubmissionDate DATETIME DEFAULT GETDATE(),
    FileUrl NVARCHAR(255),
    StudentNote NVARCHAR(MAX),
    Score DECIMAL(4,2),
    TeacherFeedback NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT 'Submitted',
    
    FOREIGN KEY (AssignmentID) REFERENCES Assignments(AssignmentID),
    FOREIGN KEY (StudentID) REFERENCES Users(UserID)
);

-- Bảng 10: ActivityLogs (Lịch sử hoạt động)
CREATE TABLE ActivityLogs (
    LogID VARCHAR(50) PRIMARY KEY,
    UserID VARCHAR(50) NOT NULL,
    ActivityType NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    RelatedID VARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);
GO

-- =============================================
-- 3. TẠO INDEX ĐỂ TỐI ƯU HIỆU NĂNG
-- =============================================

CREATE INDEX IX_Users_AccountID ON Users(AccountID);
CREATE INDEX IX_Courses_TeacherID ON Courses(TeacherID);
CREATE INDEX IX_Courses_CategoryID ON Courses(CategoryID);
CREATE INDEX IX_Sections_CourseID ON Sections(CourseID);
CREATE INDEX IX_Lessons_CourseID ON Lessons(CourseID);
CREATE INDEX IX_Lessons_SectionID ON Lessons(SectionID);
CREATE INDEX IX_Enrollments_StudentID ON Enrollments(StudentID);
CREATE INDEX IX_Enrollments_CourseID ON Enrollments(CourseID);
CREATE INDEX IX_Assignments_CourseID ON Assignments(CourseID);
CREATE INDEX IX_Submissions_AssignmentID ON Submissions(AssignmentID);
CREATE INDEX IX_Submissions_StudentID ON Submissions(StudentID);
CREATE INDEX IX_ActivityLogs_UserID ON ActivityLogs(UserID);
GO

-- =============================================
-- 4. THÊM DỮ LIỆU MẪU
-- =============================================

-- 4.1. Accounts
INSERT INTO Accounts (AccountID, Email, PasswordHash, Role) VALUES 
('ACC001', 'admin@lms.com', '123', 'Admin'),
('ACC002', 'longnv@lms.com', '123', 'Teacher'),
('ACC003', 'hoatt@lms.com', '123', 'Teacher'),
('ACC004', 'annguyen@lms.com', '123', 'Student'),
('ACC005', 'binhtr@lms.com', '123', 'Student'),
('ACC006', 'cuongle@lms.com', '123', 'Student');

-- 4.2. Users
INSERT INTO Users (UserID, AccountID, FullName, PhoneNumber, AvatarUrl) VALUES 
('USR001', 'ACC001', N'Quản trị viên', '0900000000', 'admin.png'),
('USR002', 'ACC002', N'TS. Nguyễn Văn Long', '0912345678', 'teacher1.png'),
('USR003', 'ACC003', N'ThS. Trần Thị Hoa', '0987654321', 'teacher2.png'),
('USR004', 'ACC004', N'Nguyễn Văn An', '0999888777', 'student1.png'),
('USR005', 'ACC005', N'Trần Thị Bình', '0988777666', 'student2.png'),
('USR006', 'ACC006', N'Lê Văn Cường', '0977666555', 'student3.png');

-- 4.3. Categories
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES 
('CAT001', N'Công nghệ thông tin', N'Lập trình, Database, AI...'),
('CAT002', N'Ngoại ngữ', N'Tiếng Anh, Tiếng Nhật...'),
('CAT003', N'Thiết kế đồ họa', N'Photoshop, Illustrator...');

-- 4.4. Courses
INSERT INTO Courses (CourseID, CourseName, Description, TotalLessons, CategoryID, TeacherID, Status) VALUES
('CRS001', N'Lập trình Web với React', N'Học ReactJS từ cơ bản đến nâng cao, xây dựng ứng dụng web hiện đại', 12, 'CAT001', 'USR002', 'Approved'),
('CRS002', N'Python cơ bản', N'Nhập môn lập trình Python cho người mới bắt đầu', 20, 'CAT001', 'USR003', 'Approved'),
('CRS003', N'Tiếng Anh Giao Tiếp', N'Tự tin giao tiếp tiếng Anh trong công việc và cuộc sống', 30, 'CAT002', 'USR002', 'Approved');

-- 4.5. Sections
INSERT INTO Sections (SectionID, CourseID, SectionTitle, SectionOrder, Description) VALUES
('SEC001', 'CRS001', N'Chương 1: Giới thiệu React', 1, N'Tìm hiểu về React, JSX và Component cơ bản'),
('SEC002', 'CRS001', N'Chương 2: Hooks trong React', 2, N'useState, useEffect và các hooks phổ biến'),
('SEC003', 'CRS001', N'Chương 3: React Router', 3, N'Định tuyến và navigation trong React'),
('SEC004', 'CRS002', N'Chương 1: Python cơ bản', 1, N'Biến, kiểu dữ liệu, cấu trúc điều khiển'),
('SEC005', 'CRS002', N'Chương 2: Functions & Modules', 2, N'Hàm, module và package trong Python'),
('SEC006', 'CRS003', N'Chương 1: Phát âm cơ bản', 1, N'Ngữ âm và phát âm tiếng Anh');

-- 4.6. Lessons
INSERT INTO Lessons (LessonID, CourseID, SectionID, LessonTitle, LessonOrder, Content, VideoUrl) VALUES
-- React Course
('LES001', 'CRS001', 'SEC001', N'Bài 1: React là gì?', 1, N'Giới thiệu về React và JSX', 'https://youtube.com/react-intro'),
('LES002', 'CRS001', 'SEC001', N'Bài 2: JSX và Components', 2, N'Cách viết JSX và tạo component', 'https://youtube.com/jsx-components'),
('LES003', 'CRS001', 'SEC001', N'Bài 3: Props và State', 3, N'Truyền dữ liệu với Props và quản lý State', 'https://youtube.com/props-state'),
('LES004', 'CRS001', 'SEC002', N'Bài 1: useState Hook', 1, N'Sử dụng useState để quản lý state', 'https://youtube.com/usestate'),
('LES005', 'CRS001', 'SEC002', N'Bài 2: useEffect Hook', 2, N'Side effects với useEffect', 'https://youtube.com/useeffect'),
('LES006', 'CRS001', 'SEC003', N'Bài 1: Routing cơ bản', 1, N'Cài đặt và sử dụng React Router', 'https://youtube.com/routing'),
-- Python Course
('LES007', 'CRS002', 'SEC004', N'Bài 1: Biến và kiểu dữ liệu', 1, N'Khai báo biến, các kiểu dữ liệu cơ bản', 'https://youtube.com/python-variables'),
('LES008', 'CRS002', 'SEC004', N'Bài 2: Vòng lặp và điều kiện', 2, N'If-else, for, while loops', 'https://youtube.com/python-loops'),
('LES009', 'CRS002', 'SEC005', N'Bài 1: Hàm trong Python', 1, N'Định nghĩa và sử dụng hàm', 'https://youtube.com/python-functions');

-- 4.7. Enrollments
INSERT INTO Enrollments (EnrollmentID, StudentID, CourseID, ProgressPercent, CompletedLessons, AverageScore, Status) VALUES
('ENR001', 'USR004', 'CRS001', 75, 9, 8.5, 'Learning'),
('ENR002', 'USR004', 'CRS002', 50, 10, 9.0, 'Learning'),
('ENR003', 'USR005', 'CRS001', 100, 12, 9.2, 'Completed'),
('ENR004', 'USR006', 'CRS001', 45, 5, 7.8, 'Learning');

-- 4.8. Assignments
INSERT INTO Assignments (AssignmentID, CourseID, Title, Description, DueDate, MaxScore) VALUES
('ASM001', 'CRS001', N'Bài tập 1: Tạo Component trong React', N'Tạo một component hiển thị thông tin cá nhân với props', '2025-11-20', 10.0),
('ASM002', 'CRS002', N'Bài tập 1: Viết hàm xử lý chuỗi', N'Viết các hàm xử lý chuỗi: đảo ngược, đếm từ, chuyển in hoa', '2025-11-25', 10.0),
('ASM003', 'CRS001', N'Bài tập 2: useState Hook', N'Xây dựng counter app sử dụng useState', '2025-11-27', 10.0);

-- 4.9. Submissions
INSERT INTO Submissions (SubmissionID, AssignmentID, StudentID, FileUrl, Score, TeacherFeedback, Status) VALUES
('SUB001', 'ASM001', 'USR004', 'react_bai1.zip', 9.5, N'Code sạch, cấu trúc tốt. Component được tổ chức rõ ràng.', 'Graded'),
('SUB002', 'ASM001', 'USR005', 'react_bai1_binh.zip', 10.0, N'Xuất sắc! Props được sử dụng hiệu quả.', 'Graded'),
('SUB003', 'ASM002', 'USR004', 'python_bai1.zip', NULL, NULL, 'Submitted'),
('SUB004', 'ASM001', 'USR006', 'react_cuong.zip', 7.5, N'Cần chú ý về naming conventions và comments.', 'Graded');

-- 4.10. ActivityLogs
INSERT INTO ActivityLogs (LogID, UserID, ActivityType, Description, RelatedID) VALUES
('LOG001', 'USR002', N'Tạo khóa học', N'Tạo khóa học "Lập trình Web với React"', 'CRS001'),
('LOG002', 'USR002', N'Chấm bài', N'Chấm bài tập "Tạo Component trong React" cho học viên Nguyễn Văn An', 'SUB001'),
('LOG003', 'USR003', N'Tạo bài tập', N'Giao bài tập "Viết hàm xử lý chuỗi"', 'ASM002'),
('LOG004', 'USR002', N'Thêm bài học', N'Thêm bài học "useState Hook" vào khóa React', 'LES004'),
('LOG005', 'USR002', N'Chấm bài', N'Chấm bài tập cho học viên Trần Thị Bình', 'SUB002');
GO

-- =============================================
-- 5. TẠO STORED PROCEDURES
-- =============================================

-- Procedure: Lấy danh sách khóa học của giảng viên
CREATE PROCEDURE sp_GetTeacherCourses
    @TeacherID VARCHAR(50)
AS
BEGIN
    SELECT 
        c.CourseID,
        c.CourseName,
        c.Description,
        c.TotalLessons,
        c.Status,
        c.ImageCover,
        cat.CategoryName,
        COUNT(DISTINCT e.EnrollmentID) AS TotalStudents,
        c.CreatedAt
    FROM Courses c
    LEFT JOIN Categories cat ON c.CategoryID = cat.CategoryID
    LEFT JOIN Enrollments e ON c.CourseID = e.CourseID
    WHERE c.TeacherID = @TeacherID
    GROUP BY c.CourseID, c.CourseName, c.Description, c.TotalLessons, 
             c.Status, c.ImageCover, cat.CategoryName, c.CreatedAt
    ORDER BY c.CreatedAt DESC;
END;
GO

-- Procedure: Lấy cấu trúc khóa học (Sections + Lessons)
CREATE PROCEDURE sp_GetCourseStructure
    @CourseID VARCHAR(50)
AS
BEGIN
    -- Lấy thông tin khóa học
    SELECT 
        c.CourseID,
        c.CourseName,
        c.Description,
        c.TotalLessons,
        u.FullName AS TeacherName
    FROM Courses c
    INNER JOIN Users u ON c.TeacherID = u.UserID
    WHERE c.CourseID = @CourseID;

    -- Lấy danh sách chương
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

    -- Lấy danh sách bài học
    SELECT 
        l.LessonID,
        l.SectionID,
        l.LessonTitle,
        l.LessonOrder,
        l.Content,
        l.VideoUrl,
        l.AttachmentUrl
    FROM Lessons l
    INNER JOIN Sections s ON l.SectionID = s.SectionID
    WHERE s.CourseID = @CourseID
    ORDER BY s.SectionOrder, l.LessonOrder;
END;
GO

-- Procedure: Lấy danh sách bài nộp cần chấm
CREATE PROCEDURE sp_GetPendingSubmissions
    @TeacherID VARCHAR(50)
AS
BEGIN
    SELECT 
        s.SubmissionID,
        s.SubmissionDate,
        u.FullName AS StudentName,
        acc.Email AS StudentEmail,
        a.Title AS AssignmentTitle,
        a.AssignmentID,
        c.CourseName,
        c.CourseID,
        s.FileUrl,
        s.StudentNote,
        a.MaxScore,
        a.DueDate,
        CASE 
            WHEN s.SubmissionDate > a.DueDate THEN N'Nộp trễ'
            ELSE N'Đúng hạn'
        END AS SubmissionStatus
    FROM Submissions s
    INNER JOIN Assignments a ON s.AssignmentID = a.AssignmentID
    INNER JOIN Courses c ON a.CourseID = c.CourseID
    INNER JOIN Users u ON s.StudentID = u.UserID
    INNER JOIN Accounts acc ON u.AccountID = acc.AccountID
    WHERE c.TeacherID = @TeacherID
      AND s.Status = 'Submitted'
      AND s.Score IS NULL
    ORDER BY s.SubmissionDate ASC;
END;
GO

-- Procedure: Lấy tất cả bài nộp (đã chấm + chưa chấm)
CREATE PROCEDURE sp_GetAllSubmissions
    @TeacherID VARCHAR(50),
    @CourseID VARCHAR(50) = NULL,
    @StatusFilter NVARCHAR(50) = NULL
AS
BEGIN
    SELECT 
        s.SubmissionID,
        s.SubmissionDate,
        u.FullName AS StudentName,
        acc.Email AS StudentEmail,
        a.Title AS AssignmentTitle,
        a.AssignmentID,
        c.CourseName,
        c.CourseID,
        s.FileUrl,
        s.StudentNote,
        s.Score,
        s.TeacherFeedback,
        s.Status,
        a.MaxScore,
        a.DueDate,
        CASE 
            WHEN s.SubmissionDate > a.DueDate THEN N'Nộp trễ'
            ELSE N'Đúng hạn'
        END AS SubmissionStatus
    FROM Submissions s
    INNER JOIN Assignments a ON s.AssignmentID = a.AssignmentID
    INNER JOIN Courses c ON a.CourseID = c.CourseID
    INNER JOIN Users u ON s.StudentID = u.UserID
    INNER JOIN Accounts acc ON u.AccountID = acc.AccountID
    WHERE c.TeacherID = @TeacherID
      AND (@CourseID IS NULL OR c.CourseID = @CourseID)
      AND (@StatusFilter IS NULL OR 
           (@StatusFilter = N'Chưa chấm' AND s.Score IS NULL) OR
           (@StatusFilter = N'Đã chấm' AND s.Score IS NOT NULL) OR
           (@StatusFilter = N'Nộp trễ' AND s.SubmissionDate > a.DueDate))
    ORDER BY s.SubmissionDate DESC;
END;
GO

-- Procedure: Lấy danh sách học viên trong khóa học
CREATE PROCEDURE sp_GetCourseStudents
    @CourseID VARCHAR(50)
AS
BEGIN
    SELECT 
        u.UserID,
        u.FullName,
        acc.Email,
        u.PhoneNumber,
        e.EnrollmentDate,
        e.ProgressPercent,
        e.CompletedLessons,
        e.AverageScore,
        e.Status,
        c.TotalLessons
    FROM Enrollments e
    INNER JOIN Users u ON e.StudentID = u.UserID
    INNER JOIN Accounts acc ON u.AccountID = acc.AccountID
    INNER JOIN Courses c ON e.CourseID = c.CourseID
    WHERE e.CourseID = @CourseID
    ORDER BY e.EnrollmentDate DESC;
END;
GO

-- Procedure: Lấy thống kê tổng quan của giảng viên (Dashboard)
CREATE PROCEDURE sp_GetTeacherDashboard
    @TeacherID VARCHAR(50)
AS
BEGIN
    -- Tổng số khóa học
    SELECT COUNT(*) AS TotalCourses
    FROM Courses
    WHERE TeacherID = @TeacherID;

    -- Tổng số học viên
    SELECT COUNT(DISTINCT e.StudentID) AS TotalStudents
    FROM Enrollments e
    INNER JOIN Courses c ON e.CourseID = c.CourseID
    WHERE c.TeacherID = @TeacherID;

    -- Số bài cần chấm
    SELECT COUNT(*) AS PendingSubmissions
    FROM Submissions s
    INNER JOIN Assignments a ON s.AssignmentID = a.AssignmentID
    INNER JOIN Courses c ON a.CourseID = c.CourseID
    WHERE c.TeacherID = @TeacherID
      AND s.Status = 'Submitted'
      AND s.Score IS NULL;

    -- Hoạt động gần đây (10 hoạt động)
    SELECT TOP 10
        LogID,
        ActivityType,
        Description,
        CreatedAt
    FROM ActivityLogs
    WHERE UserID = @TeacherID
    ORDER BY CreatedAt DESC;
END;
GO

-- Procedure: Lấy danh sách bài tập của khóa học
CREATE PROCEDURE sp_GetCourseAssignments
    @CourseID VARCHAR(50)
AS
BEGIN
    SELECT 
        a.AssignmentID,
        a.Title,
        a.Description,
        a.AttachmentUrl,
        a.AssignedDate,
        a.DueDate,
        a.MaxScore,
        COUNT(s.SubmissionID) AS TotalSubmissions,
        SUM(CASE WHEN s.Score IS NULL THEN 1 ELSE 0 END) AS PendingGrading,
        CASE 
            WHEN GETDATE() > a.DueDate THEN N'Đã đóng'
            WHEN GETDATE() < a.AssignedDate THEN N'Chưa mở'
            ELSE N'Đang mở'
        END AS Status
    FROM Assignments a
    LEFT JOIN Submissions s ON a.AssignmentID = s.AssignmentID
    WHERE a.CourseID = @CourseID
    GROUP BY a.AssignmentID, a.Title, a.Description, a.AttachmentUrl, 
             a.AssignedDate, a.DueDate, a.MaxScore
    ORDER BY a.AssignedDate DESC;
END;
GO

-- Procedure: Chấm điểm bài nộp
CREATE PROCEDURE sp_GradeSubmission
    @SubmissionID VARCHAR(50),
    @Score DECIMAL(4,2),
    @TeacherFeedback NVARCHAR(MAX)
AS
BEGIN
    UPDATE Submissions
    SET Score = @Score,
        TeacherFeedback = @TeacherFeedback,
        Status = 'Graded'
    WHERE SubmissionID = @SubmissionID;

    -- Cập nhật điểm trung bình của học viên
    UPDATE e
    SET e.AverageScore = (
        SELECT AVG(s.Score)
        FROM Submissions s
        INNER JOIN Assignments a ON s.AssignmentID = a.AssignmentID
        WHERE s.StudentID = e.StudentID 
          AND a.CourseID = e.CourseID
          AND s.Score IS NOT NULL
    )
    FROM Enrollments e
    INNER JOIN Submissions sub ON e.StudentID = sub.StudentID
    WHERE sub.SubmissionID = @SubmissionID;
END;
GO

-- =============================================
-- 6. TẠO VIEWS
-- =============================================

-- View: Tổng quan khóa học
CREATE VIEW vw_CourseOverview AS
SELECT 
    c.CourseID,
    c.CourseName,
    c.Description,
    c.Status,
    cat.CategoryName,
    u.FullName AS TeacherName,
    c.TotalLessons,
    COUNT(DISTINCT e.StudentID) AS TotalStudents,
    COUNT(DISTINCT s.SectionID) AS TotalSections,
    c.CreatedAt
FROM Courses c
LEFT JOIN Categories cat ON c.CategoryID = cat.CategoryID
LEFT JOIN Users u ON c.TeacherID = u.UserID
LEFT JOIN Enrollments e ON c.CourseID = e.CourseID
LEFT JOIN Sections s ON c.CourseID = s.CourseID
GROUP BY c.CourseID, c.CourseName, c.Description, c.Status, 
         cat.CategoryName, u.FullName, c.TotalLessons, c.CreatedAt;
GO

-- View: Tiến độ học viên
CREATE VIEW vw_StudentProgress AS
SELECT 
    e.EnrollmentID,
    u.FullName AS StudentName,
    acc.Email,
    c.CourseName,
    e.EnrollmentDate,
    e.ProgressPercent,
    e.CompletedLessons,
    c.TotalLessons,
    e.AverageScore,
    e.Status
FROM Enrollments e
INNER JOIN Users u ON e.StudentID = u.UserID
INNER JOIN Accounts acc ON u.AccountID = acc.AccountID
INNER JOIN Courses c ON e.CourseID = c.CourseID;
GO

PRINT '========================================';
PRINT 'DATABASE StudyProcess ĐÃ TẠO THÀNH CÔNG!';
PRINT '========================================';
PRINT '';
PRINT 'Các bảng đã tạo:';
PRINT '  1. Accounts - Tài khoản đăng nhập';
PRINT '  2. Users - Thông tin người dùng';
PRINT '  3. Categories - Danh mục khóa học';
PRINT '  4. Courses - Khóa học';
PRINT '  5. Sections - Chương trong khóa học';
PRINT '  6. Lessons - Bài học';
PRINT '  7. Enrollments - Đăng ký học';
PRINT '  8. Assignments - Bài tập';
PRINT '  9. Submissions - Bài nộp';
PRINT ' 10. ActivityLogs - Lịch sử hoạt động';
PRINT '';
PRINT 'Stored Procedures:';
PRINT '  - sp_GetTeacherCourses';
PRINT '  - sp_GetCourseStructure';
PRINT '  - sp_GetPendingSubmissions';
PRINT '  - sp_GetAllSubmissions';
PRINT '  - sp_GetCourseStudents';
PRINT '  - sp_GetTeacherDashboard';
PRINT '  - sp_GetCourseAssignments';
PRINT '  - sp_GradeSubmission';
PRINT '';
PRINT 'Views:';
PRINT '  - vw_CourseOverview';
PRINT '  - vw_StudentProgress';
PRINT '';
PRINT 'Tài khoản mẫu:';
PRINT '  Teacher: longnv@lms.com / 123';
PRINT '  Teacher: hoatt@lms.com / 123';
PRINT '  Student: annguyen@lms.com / 123';
GO
