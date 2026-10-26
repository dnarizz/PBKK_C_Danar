# Mengecek koneksi ke SQL Server dan menampilkan isi database StudentRegistrationDB.
# Jalankan lewat cek-database.bat (klik dua kali).

$connectionString = 'Server=(localdb)\MSSQLLocalDB;Database=StudentRegistrationDB;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5'

function Show-Query($conn, $title, $sql) {
    Write-Host ''
    Write-Host "=== $title ===" -ForegroundColor Cyan
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $table = New-Object System.Data.DataTable
    $table.Load($cmd.ExecuteReader())
    $table | Format-Table -AutoSize | Out-String -Width 220 | Write-Host
}

try {
    $conn = New-Object System.Data.SqlClient.SqlConnection $connectionString
    $conn.Open()

    Show-Query $conn 'KONEKSI SQL SERVER' "SELECT @@SERVERNAME AS Server,
                                                 CAST(SERVERPROPERTY('ProductVersion') AS varchar(20)) AS Versi,
                                                 CAST(SERVERPROPERTY('Edition') AS varchar(100)) AS Edisi,
                                                 DB_NAME() AS CurrentDatabase"
    Write-Host 'Status: TERHUBUNG' -ForegroundColor Green

    Show-Query $conn 'TABEL DI StudentRegistrationDB' "SELECT TABLE_SCHEMA + '.' + TABLE_NAME AS [Table]
                                                       FROM INFORMATION_SCHEMA.TABLES
                                                       WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME"

    Show-Query $conn 'SELECT * FROM Programs' "SELECT ProgramId, ProgramCode, ProgramName, IsActive,
                                                      CONVERT(varchar(19), CreatedAt, 120) AS CreatedAt
                                               FROM Programs"

    Show-Query $conn 'SELECT * FROM Students' "SELECT StudentId, NIM, Name, ProgramId,
                                                      CONVERT(varchar(10), BirthDate, 23) AS BirthDate,
                                                      Address, PhoneNumber,
                                                      CONVERT(varchar(19), CreatedAt, 120) AS CreatedAt, UpdatedAt
                                               FROM Students"

    Show-Query $conn 'JOIN Students + Programs (ORDER BY Name)' "SELECT s.NIM, s.Name, p.ProgramName,
                                                                        CONVERT(varchar(10), s.BirthDate, 23) AS BirthDate,
                                                                        s.PhoneNumber
                                                                 FROM Students s
                                                                 INNER JOIN Programs p ON s.ProgramId = p.ProgramId
                                                                 ORDER BY s.Name"

    $cmd = $conn.CreateCommand()
    $cmd.CommandText = 'SELECT COUNT(*) FROM Students'
    Write-Host ("Total Mahasiswa: " + $cmd.ExecuteScalar()) -ForegroundColor Yellow

    $conn.Close()
}
catch {
    Write-Host ''
    Write-Host 'Status: GAGAL TERHUBUNG' -ForegroundColor Red
    Write-Host $_.Exception.Message
}
