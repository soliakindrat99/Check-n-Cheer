using System.ComponentModel.DataAnnotations;

namespace Check_n_Cheer.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Email is not passed!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is not passed!")]
        public string Password { get; set; }
        public string Role { get; set; } = "Student";
    }
    public enum Role
    {
        Admin,
        Student,
        Teacher
    }
}