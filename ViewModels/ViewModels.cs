using System.ComponentModel.DataAnnotations;

namespace StudentRecordSystem.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public class CreateStudentViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(30, ErrorMessage = "Name cannot exceed 30 characters")]
    [Display(Name = "Student Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Maths marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    [Display(Name = "Maths")]
    public int Maths { get; set; }

    [Required(ErrorMessage = "Physics marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    [Display(Name = "Physics")]
    public int Physics { get; set; }

    [Required(ErrorMessage = "Chemistry marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    [Display(Name = "Chemistry")]
    public int Chemistry { get; set; }

    [Required(ErrorMessage = "English marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    [Display(Name = "English")]
    public int English { get; set; }

    [Required(ErrorMessage = "Programming marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    [Display(Name = "Programming")]
    public int Programming { get; set; }

    // Optional: create a linked user account
    [MaxLength(50)]
    [Display(Name = "Student Username (optional)")]
    public string? Username { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Student Password (optional)")]
    public string? Password { get; set; }
}

public class EditMarksViewModel
{
    [Required]
    public string RollNumber { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Maths marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    public int Maths { get; set; }

    [Required(ErrorMessage = "Physics marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    public int Physics { get; set; }

    [Required(ErrorMessage = "Chemistry marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    public int Chemistry { get; set; }

    [Required(ErrorMessage = "English marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    public int English { get; set; }

    [Required(ErrorMessage = "Programming marks are required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    public int Programming { get; set; }
}
