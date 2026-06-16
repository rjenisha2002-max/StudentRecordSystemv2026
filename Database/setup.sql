-- ============================================================
-- Student Record Management System - Database Setup Script
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StudentRecordDB')
BEGIN
    CREATE DATABASE StudentRecordDB;
END
GO

USE StudentRecordDB;
GO

-- ============================================================
-- TABLES
-- ============================================================

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        UserId      INT IDENTITY(1,1) PRIMARY KEY,
        Username    VARCHAR(50)  NOT NULL UNIQUE,
        PasswordHash VARCHAR(255) NOT NULL,
        Role        VARCHAR(20)  NOT NULL CHECK (Role IN ('Invigilator','Student')),
        IsActive    BIT          NOT NULL DEFAULT 1,
        CreatedAt   DATETIME     NOT NULL DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Students' AND xtype='U')
BEGIN
    CREATE TABLE Students (
        StudentId       INT IDENTITY(1,1) PRIMARY KEY,
        RollNumber      CHAR(5)      NOT NULL UNIQUE,
        Name            VARCHAR(30)  NOT NULL,
        Maths           INT          NOT NULL CHECK (Maths BETWEEN 1 AND 100),
        Physics         INT          NOT NULL CHECK (Physics BETWEEN 1 AND 100),
        Chemistry       INT          NOT NULL CHECK (Chemistry BETWEEN 1 AND 100),
        English         INT          NOT NULL CHECK (English BETWEEN 1 AND 100),
        Programming     INT          NOT NULL CHECK (Programming BETWEEN 1 AND 100),
        IsActive        BIT          NOT NULL DEFAULT 1,
        CreatedAt       DATETIME     NOT NULL DEFAULT GETDATE(),
        ModifiedAt      DATETIME     NULL,
        UserId          INT          NULL REFERENCES Users(UserId)
    );
END
GO

-- ============================================================
-- STORED PROCEDURES
-- ============================================================

-- sp_GetAllStudents
IF OBJECT_ID('sp_GetAllStudents','P') IS NOT NULL DROP PROCEDURE sp_GetAllStudents;
GO
CREATE PROCEDURE sp_GetAllStudents
AS
BEGIN
    SET NOCOUNT ON;
    SELECT StudentId, RollNumber, Name, Maths, Physics, Chemistry, English, Programming,
           IsActive, CreatedAt, ModifiedAt,
           (Maths + Physics + Chemistry + English + Programming) AS Total,
           CAST((Maths + Physics + Chemistry + English + Programming) AS FLOAT) / 5.0 AS Average
    FROM Students
    WHERE IsActive = 1
    ORDER BY RollNumber;
END
GO

-- sp_GetStudentByRoll
IF OBJECT_ID('sp_GetStudentByRoll','P') IS NOT NULL DROP PROCEDURE sp_GetStudentByRoll;
GO
CREATE PROCEDURE sp_GetStudentByRoll
    @RollNumber CHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT StudentId, RollNumber, Name, Maths, Physics, Chemistry, English, Programming,
           IsActive, CreatedAt, ModifiedAt,
           (Maths + Physics + Chemistry + English + Programming) AS Total,
           CAST((Maths + Physics + Chemistry + English + Programming) AS FLOAT) / 5.0 AS Average
    FROM Students
    WHERE RollNumber = @RollNumber AND IsActive = 1;
END
GO

-- sp_GetStudentByUserId
IF OBJECT_ID('sp_GetStudentByUserId','P') IS NOT NULL DROP PROCEDURE sp_GetStudentByUserId;
GO
CREATE PROCEDURE sp_GetStudentByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT StudentId, RollNumber, Name, Maths, Physics, Chemistry, English, Programming,
           IsActive, CreatedAt, ModifiedAt,
           (Maths + Physics + Chemistry + English + Programming) AS Total,
           CAST((Maths + Physics + Chemistry + English + Programming) AS FLOAT) / 5.0 AS Average
    FROM Students
    WHERE UserId = @UserId AND IsActive = 1;
END
GO

-- sp_CreateStudent
IF OBJECT_ID('sp_CreateStudent','P') IS NOT NULL DROP PROCEDURE sp_CreateStudent;
GO
CREATE PROCEDURE sp_CreateStudent
    @Name        VARCHAR(30),
    @Maths       INT,
    @Physics     INT,
    @Chemistry   INT,
    @English     INT,
    @Programming INT,
    @UserId      INT = NULL,
    @RollNumber  CHAR(5) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaxRoll INT;

    SELECT @MaxRoll = ISNULL(MAX(CAST(RollNumber AS INT)), 10000)
    FROM Students;

    SET @RollNumber = RIGHT('00000' + CAST(@MaxRoll + 1 AS VARCHAR(5)), 5);

    INSERT INTO Students (RollNumber, Name, Maths, Physics, Chemistry, English, Programming, UserId)
    VALUES (@RollNumber, @Name, @Maths, @Physics, @Chemistry, @English, @Programming, @UserId);

    SELECT SCOPE_IDENTITY() AS StudentId;
END
GO

-- sp_UpdateStudentMarks
IF OBJECT_ID('sp_UpdateStudentMarks','P') IS NOT NULL DROP PROCEDURE sp_UpdateStudentMarks;
GO
CREATE PROCEDURE sp_UpdateStudentMarks
    @RollNumber  CHAR(5),
    @Maths       INT,
    @Physics     INT,
    @Chemistry   INT,
    @English     INT,
    @Programming INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Students
    SET Maths = @Maths, Physics = @Physics, Chemistry = @Chemistry,
        English = @English, Programming = @Programming, ModifiedAt = GETDATE()
    WHERE RollNumber = @RollNumber AND IsActive = 1;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- sp_DeleteStudent (soft delete)
IF OBJECT_ID('sp_DeleteStudent','P') IS NOT NULL DROP PROCEDURE sp_DeleteStudent;
GO
CREATE PROCEDURE sp_DeleteStudent
    @RollNumber CHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Students SET IsActive = 0
    WHERE RollNumber = @RollNumber AND IsActive = 1;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- sp_GetUserByUsername
IF OBJECT_ID('sp_GetUserByUsername','P') IS NOT NULL DROP PROCEDURE sp_GetUserByUsername;
GO
CREATE PROCEDURE sp_GetUserByUsername
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Username, PasswordHash, Role, IsActive
    FROM Users
    WHERE Username = @Username AND IsActive = 1;
END
GO

-- sp_CreateUser
IF OBJECT_ID('sp_CreateUser','P') IS NOT NULL DROP PROCEDURE sp_CreateUser;
GO
CREATE PROCEDURE sp_CreateUser
    @Username     VARCHAR(50),
    @PasswordHash VARCHAR(255),
    @Role         VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        SELECT -1 AS UserId;
        RETURN;
    END
    INSERT INTO Users (Username, PasswordHash, Role)
    VALUES (@Username, @PasswordHash, @Role);
    SELECT SCOPE_IDENTITY() AS UserId;
END
GO

-- ============================================================
-- SEED DATA
-- ============================================================

-- Default invigilator: admin / Admin@123
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, Role)
    VALUES ('admin', '$2a$11$KMnY3Y3Y3Y3Y3Y3Y3Y3Y3uQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ', 'Invigilator');
END
GO

PRINT 'Database setup complete.';
GO
