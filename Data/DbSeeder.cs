using Microsoft.Data.SqlClient;
using System.Data;

namespace StudentRecordSystem.Data;

/// <summary>
/// Ensures the default admin account exists on app startup.
/// </summary>
public static class DbSeeder
{
    public static void Seed(IConfiguration config)
    {
        var connStr = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connStr)) return;

        try
        {
            using var conn = new SqlConnection(connStr);
            conn.Open();

            // Check if admin already exists
            using var check = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Username = 'admin'", conn);
            var count = (int)check.ExecuteScalar()!;
            if (count > 0) return;

            // Create admin with hashed password
            var hash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 11);
            using var insert = new SqlCommand("sp_CreateUser", conn) { CommandType = CommandType.StoredProcedure };
            insert.Parameters.AddWithValue("@Username", "admin");
            insert.Parameters.AddWithValue("@PasswordHash", hash);
            insert.Parameters.AddWithValue("@Role", "Invigilator");
            insert.ExecuteScalar();

            // Seed sample students
            SeedSampleStudents(conn);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbSeeder] Warning: {ex.Message}");
        }
    }

    private static void SeedSampleStudents(SqlConnection conn)
    {
        using var chk = new SqlCommand("SELECT COUNT(1) FROM Students", conn);
        var count = (int)chk.ExecuteScalar()!;
        if (count > 0) return;

        var students = new[]
        {
            ("Alice Johnson",   92, 88, 95, 85, 90),
            ("Bob Smith",       72, 65, 70, 80, 68),
            ("Carol Williams",  55, 60, 58, 62, 70),
            ("David Brown",     40, 45, 38, 55, 42),
            ("Eva Martinez",    98, 95, 97, 92, 99),
        };

        foreach (var (name, m, p, c, e, pr) in students)
        {
            using var cmd = new SqlCommand("sp_CreateStudent", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Maths", m);
            cmd.Parameters.AddWithValue("@Physics", p);
            cmd.Parameters.AddWithValue("@Chemistry", c);
            cmd.Parameters.AddWithValue("@English", e);
            cmd.Parameters.AddWithValue("@Programming", pr);
            cmd.Parameters.AddWithValue("@UserId", DBNull.Value);
            var rollParam = new SqlParameter("@RollNumber", System.Data.SqlDbType.Char, 5) { Direction = System.Data.ParameterDirection.Output };
            cmd.Parameters.Add(rollParam);
            cmd.ExecuteNonQuery();
        }
    }
}
