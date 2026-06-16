-- Run this script AFTER setup.sql to seed the admin user with the correct bcrypt hash.
-- The hash below corresponds to password: Admin@123
-- Generated with BCrypt.Net cost factor 11.

USE StudentRecordDB;
GO

-- Remove placeholder admin if exists
DELETE FROM Users WHERE Username = 'admin';

-- Insert admin with real BCrypt hash for 'Admin@123'
INSERT INTO Users (Username, PasswordHash, Role)
VALUES (
    'admin',
    '$2a$11$rBnNhl3SHmFuQbsHAKN/u.5mvLy2a/EK5dCBOCLMqwNFkUZpxY6aq',
    'Invigilator'
);

-- Sample student data (no linked accounts)
IF NOT EXISTS (SELECT 1 FROM Students WHERE RollNumber = '10001')
BEGIN
    INSERT INTO Students (RollNumber, Name, Maths, Physics, Chemistry, English, Programming)
    VALUES
        ('10001', 'Alice Johnson',   92, 88, 95, 85, 90),
        ('10002', 'Bob Smith',       72, 65, 70, 80, 68),
        ('10003', 'Carol Williams',  55, 60, 58, 62, 70),
        ('10004', 'David Brown',     40, 45, 38, 55, 42),
        ('10005', 'Eva Martinez',    98, 95, 97, 92, 99);
END
GO

PRINT 'Seed data inserted successfully.';
PRINT 'Admin login: admin / Admin@123';
GO
