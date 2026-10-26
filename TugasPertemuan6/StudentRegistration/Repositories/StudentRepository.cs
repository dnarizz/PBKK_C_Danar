using Microsoft.Data.SqlClient;
using StudentRegistration.Models;

namespace StudentRegistration.Repositories;

public class StudentRepository
{
    private readonly string _connectionString =
        @"Server=(localdb)\MSSQLLocalDB;
          Database=StudentRegistrationDB;
          Trusted_Connection=True;
          TrustServerCertificate=True;";

    public List<Student> GetAll()
    {
        var students = new List<Student>();

        const string sql = """
            SELECT
                s.StudentId,
                s.NIM,
                s.Name,
                s.ProgramId,
                p.ProgramName,
                s.BirthDate,
                s.Address,
                s.PhoneNumber,
                s.CreatedAt,
                s.UpdatedAt
            FROM Students s
            INNER JOIN Programs p
                ON s.ProgramId = p.ProgramId
            ORDER BY s.StudentId;
            """;

        using var connection =
            new SqlConnection(_connectionString);

        using var command =
            new SqlCommand(sql, connection);

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            students.Add(new Student
            {
                StudentId = reader.GetInt32(
                    reader.GetOrdinal("StudentId")),

                NIM = reader.GetString(
                    reader.GetOrdinal("NIM")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                ProgramId = reader.GetInt32(
                    reader.GetOrdinal("ProgramId")),

                ProgramName = reader.GetString(
                    reader.GetOrdinal("ProgramName")),

                BirthDate = reader.IsDBNull(
                    reader.GetOrdinal("BirthDate"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("BirthDate")),

                Address = reader.IsDBNull(
                    reader.GetOrdinal("Address"))
                    ? string.Empty
                    : reader.GetString(
                        reader.GetOrdinal("Address")),

                PhoneNumber = reader.IsDBNull(
                    reader.GetOrdinal("PhoneNumber"))
                    ? string.Empty
                    : reader.GetString(
                        reader.GetOrdinal("PhoneNumber")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt")),

                UpdatedAt = reader.IsDBNull(
                    reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("UpdatedAt"))
            });
        }

        return students;
    }

    // Mini Challenge 3 — Total Mahasiswa (SELECT COUNT(*) FROM Students)
    public int GetTotal()
    {
        const string sql = "SELECT COUNT(*) FROM Students;";

        using var connection =
            new SqlConnection(_connectionString);

        using var command =
            new SqlCommand(sql, connection);

        connection.Open();

        return (int)command.ExecuteScalar();
    }
}
