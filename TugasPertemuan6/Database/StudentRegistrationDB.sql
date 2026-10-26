-- =====================================================================
-- Hands-On Lab 1 — Membangun Database Student Registration dengan SQL Server
-- Seluruh script praktikum (LAB 09 - LAB 24 + Mini Assignment)
-- Server: (localdb)\MSSQLLocalDB / .\SQLEXPRESS (Windows Authentication)
-- Script aman dijalankan ulang (tidak membuat data ganda).
-- =====================================================================

-- LAB 09 — Membuat Database
IF DB_ID('StudentRegistrationDB') IS NULL
    CREATE DATABASE StudentRegistrationDB;
GO

-- LAB 10 — Menggunakan Database
USE StudentRegistrationDB;
GO

SELECT DB_NAME() AS CurrentDatabase;
GO

-- LAB 11 — Membuat Tabel Programs
IF OBJECT_ID('dbo.Programs', 'U') IS NULL
CREATE TABLE Programs
(
    ProgramId INT IDENTITY(1,1)
        CONSTRAINT PK_Programs PRIMARY KEY,

    ProgramCode VARCHAR(20) NOT NULL
        CONSTRAINT UQ_Programs_ProgramCode UNIQUE,

    ProgramName VARCHAR(100) NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Programs_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Programs_CreatedAt
        DEFAULT SYSDATETIME()
);
GO

-- LAB 12 — Memasukkan Data Program Studi
IF NOT EXISTS (SELECT 1 FROM Programs)
INSERT INTO Programs
(
    ProgramCode,
    ProgramName
)
VALUES
('TI',  'Teknik Informatika'),
('SI',  'Sistem Informasi'),
('MNJ', 'Manajemen'),
('TE',  'Teknik Elektro'),
('DKV', 'Desain Komunikasi Visual');
GO

-- LAB 13 — Melihat Data Programs
SELECT *
FROM Programs;
GO

-- LAB 14 — Membuat Tabel Students
IF OBJECT_ID('dbo.Students', 'U') IS NULL
CREATE TABLE Students
(
    StudentId INT IDENTITY(1,1)
        CONSTRAINT PK_Students PRIMARY KEY,

    NIM VARCHAR(20) NOT NULL
        CONSTRAINT UQ_Students_NIM UNIQUE,

    Name VARCHAR(100) NOT NULL,

    ProgramId INT NOT NULL,

    BirthDate DATE NULL,

    Address VARCHAR(250) NULL,

    PhoneNumber VARCHAR(20) NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Students_CreatedAt
        DEFAULT SYSDATETIME(),

    UpdatedAt DATETIME2 NULL,

    CONSTRAINT FK_Students_Programs
        FOREIGN KEY (ProgramId)
        REFERENCES Programs(ProgramId)
);
GO

-- LAB 16 — Memasukkan Data Mahasiswa
IF NOT EXISTS (SELECT 1 FROM Students)
INSERT INTO Students
(
    NIM,
    Name,
    ProgramId,
    BirthDate,
    Address,
    PhoneNumber
)
VALUES
(
    '20231001',
    'Ahmad Rizki',
    1,
    '2003-03-12',
    'Jl. Merdeka No. 123, Surabaya',
    '081234567890'
),
(
    '20231002',
    'Siti Aisyah',
    2,
    '2004-07-21',
    'Jl. Diponegoro No. 45, Surabaya',
    '082233445566'
),
(
    '20231003',
    'Budi Santoso',
    1,
    '2003-11-05',
    'Jl. Ahmad Yani No. 10, Surabaya',
    '081355667788'
),
(
    '20231004',
    'Citra Lestari',
    3,
    '2004-01-18',
    'Jl. Darmo No. 20, Surabaya',
    '089900112233'
),
(
    '20231005',
    'Dimas Prayoga',
    4,
    '2003-09-27',
    'Jl. Raya ITS, Surabaya',
    '081288776655'
);
GO

-- LAB 17 — Menampilkan Data Students
SELECT *
FROM Students;
GO

-- LAB 18 — Menggabungkan Dua Tabel
SELECT
    s.StudentId,
    s.NIM,
    s.Name,
    p.ProgramCode,
    p.ProgramName,
    s.BirthDate,
    s.PhoneNumber
FROM Students s
INNER JOIN Programs p
    ON s.ProgramId = p.ProgramId;
GO

-- LAB 19 — Pengujian Foreign Key
-- ProgramId 99 tidak ada di tabel Programs, sehingga harus ditolak (error 547).
BEGIN TRY
    INSERT INTO Students
    (
        NIM,
        Name,
        ProgramId
    )
    VALUES
    (
        '20231099',
        'Test Student',
        99
    );
END TRY
BEGIN CATCH
    SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
END CATCH;
GO

-- LAB 20 — Pengujian UNIQUE NIM
-- NIM 20231001 sudah ada, sehingga harus ditolak (error 2627).
BEGIN TRY
    INSERT INTO Students
    (
        NIM,
        Name,
        ProgramId
    )
    VALUES
    (
        '20231001',
        'Mahasiswa Duplikat',
        1
    );
END TRY
BEGIN CATCH
    SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
END CATCH;
GO

-- LAB 21 - LAB 23 — Melihat struktur database, kolom, dan relationship
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Students'
ORDER BY ORDINAL_POSITION;

SELECT
    fk.name AS ForeignKey,
    OBJECT_NAME(fk.parent_object_id) AS ChildTable,
    OBJECT_NAME(fk.referenced_object_id) AS ParentTable
FROM sys.foreign_keys fk;
GO

-- LAB 24 — Query Challenge
-- Challenge 1 — Tampilkan seluruh mahasiswa
SELECT *
FROM Students;

-- Challenge 2 — Tampilkan mahasiswa Teknik Informatika
SELECT
    s.NIM,
    s.Name,
    p.ProgramName
FROM Students s
INNER JOIN Programs p
    ON s.ProgramId = p.ProgramId
WHERE p.ProgramCode = 'TI';

-- Challenge 3 — Hitung jumlah mahasiswa
SELECT COUNT(*) AS TotalMahasiswa
FROM Students;

-- Challenge 4 — Hitung mahasiswa berdasarkan program studi
SELECT
    p.ProgramName,
    COUNT(s.StudentId) AS JumlahMahasiswa
FROM Programs p
LEFT JOIN Students s
    ON p.ProgramId = s.ProgramId
GROUP BY
    p.ProgramId,
    p.ProgramName;
GO

-- Mini Assignment — 3 mahasiswa baru (1 TI, 1 SI, 1 TE)
IF NOT EXISTS (SELECT 1 FROM Students WHERE NIM = '20231006')
INSERT INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber)
SELECT '20231006', 'Reza Mahendra', ProgramId, '2004-04-15',
       'Jl. Keputih Tegal No. 12, Surabaya', '081377889911'
FROM Programs WHERE ProgramCode = 'TI';

IF NOT EXISTS (SELECT 1 FROM Students WHERE NIM = '20231007')
INSERT INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber)
SELECT '20231007', 'Nabila Putri', ProgramId, '2003-08-22',
       'Jl. Mulyosari Utara No. 24, Surabaya', '082144556688'
FROM Programs WHERE ProgramCode = 'SI';

IF NOT EXISTS (SELECT 1 FROM Students WHERE NIM = '20231008')
INSERT INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber)
SELECT '20231008', 'Bagas Pratama', ProgramId, '2004-12-03',
       'Jl. Kertajaya Indah No. 55, Surabaya', '085233221199'
FROM Programs WHERE ProgramCode = 'TE';
GO

SELECT
    s.NIM,
    s.Name,
    p.ProgramName,
    s.BirthDate,
    s.PhoneNumber
FROM Students s
INNER JOIN Programs p
    ON s.ProgramId = p.ProgramId
ORDER BY s.Name;
GO
