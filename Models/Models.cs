namespace StudentRecordSystem.Models;

public class Student
{
    public int StudentId { get; set; }
    public string RollNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Maths { get; set; }
    public int Physics { get; set; }
    public int Chemistry { get; set; }
    public int English { get; set; }
    public int Programming { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public int? UserId { get; set; }
    public int Total => Maths + Physics + Chemistry + English + Programming;
    public double Average => Total / 5.0;
    public string Grade => Average >= 90 ? "A+" : Average >= 80 ? "A" : Average >= 70 ? "B" : Average >= 60 ? "C" : Average >= 50 ? "D" : "F";
    public string GradeClass => Average >= 80 ? "success" : Average >= 60 ? "warning" : "danger";
}

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
