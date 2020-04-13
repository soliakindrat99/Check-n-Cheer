using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Check_n_Cheer.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Email is not passed!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is not passed!")]
        public string Password{ get; set; }
        public void SetPassword(string password)
        {
            Password = Crypto.Hash(password);
        }
        public bool CheckPassword(string password)
        {
            return string.Equals(Password, Crypto.Hash(password));
        }
        public string Role { get; set; } = "Student";
    }
    public static class Crypto
    {
        public static string Hash(string value)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(value)));
        }
    }
}