using Microsoft.Data.SqlClient;
using StudentRecordSystem.Models;
using System.Data;

namespace StudentRecordSystem.Data;

public class StudentRepository
{
    private readonly string _connectionString;

    public StudentRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    private SqlConnection GetConnection() => new SqlConnection(_connectionString);

    // ── Students ──────────────────────────────────────────────

    public List<Student> GetAllStudents()
    {
        var list = new List<Student>();
        using var conn = GetConnection();
        using var cmd = new SqlCommand("sp_GetAllStudents", conn) { CommandType = CommandType.StoredProcedure };
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) list.Add(MapStudent(reader));
        return list;
    }

    public Student? GetStudentByRoll(string rollNumber)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("sp_GetStudentByRoll", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@RollNumber", rollNumber);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapStudent(reader) : null;
    }

    public Student? GetStudentByUserId(int userId)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("sp_GetStudentByUserId", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UserId", userId);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapStudent(reader) : null;
    }

    /// <returns>Generated roll number</returns>
    public string CreateStudent(string name, int maths, int physics, int chemistry, int english, int programming, int? userId = null)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("sp_CreateStudent", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@Maths", maths);
        cmd.Parameters.AddWithValue("@Physics", physics);
        cmd.Parameters.AddWithValue("@Chemistry", chemistry);
        cmd.Parameters.AddWithValue("@English", english);
        cmd.Parameters.AddWithValue("@Programming", programming);
        cmd.Parameters.AddWithValue("@UserId", userId.HasValue ? (object)userId.Value : DBNull.Value);

        var rollParam = new SqlParameter("@RollNumber", SqlDbType.Char, 5)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(rollParam);

        conn.Open();
        cmd.ExecuteNonQuery();
        return rollParam.Value?.ToString() ?? string.Empty;
    }

    public bool UpdateMarks(string rollNumber, int maths, int physics, int chemistry, int english, int programming)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("sp_UpdateStudentMarks", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@RollNumber", rollNumber);
        cmd.Parameters.AddWithValue("@Maths", maths);
        cmd.Parameters.AddWithValue("@Physics", physics);
        cmd.Parameters.AddWithValue("@Chemistry", chemistry);
        cmd.Parameters.AddWithValue("@English", english);
        cmd.Parameters.AddWithValue("@Programming", programming);
        conn.Open();
        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result) > 0;
    }

    public bool DeleteStudent(string rollNumber)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("sp_DeleteStudent", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@RollNumber", rollNumber);
        conn.Open();
        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result) > 0;
    }

    // ── Users ─────────────────────────────────────────────────

    public User? GetUserByUsername(string username)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("sp_GetUserByUsername", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Username", username);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new User
        {
            UserId = (int)reader["UserId"],
            Username = reader["Username"].ToString()!,
            PasswordHash = reader["PasswordHash"].ToString()!,
            Role = reader["Role"].ToString()!,
            IsActive = (bool)reader["IsActive"]
        };
    }

    public int CreateUser(string username, string passwordHash, string role)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand("sp_CreateUser", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
        cmd.Parameters.AddWithValue("@Role", role);
        conn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // ── Mapping ───────────────────────────────────────────────

    private static Student MapStudent(SqlDataReader r) => new()
    {
        StudentId   = (int)r["StudentId"],
        RollNumber  = r["RollNumber"].ToString()!.Trim(),
        Name        = r["Name"].ToString()!,
        Maths       = (int)r["Maths"],
        Physics     = (int)r["Physics"],
        Chemistry   = (int)r["Chemistry"],
        English     = (int)r["English"],
        Programming = (int)r["Programming"],
        IsActive    = (bool)r["IsActive"],
        CreatedAt   = (DateTime)r["CreatedAt"],
        ModifiedAt  = r["ModifiedAt"] as DateTime?
    };
}
