using System;
using System.Collections.Generic;
using System.Text;

namespace Check_n_Cheer.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid? CurrentTestId { get; set; } = null;
        public List<Test> Tests { get; set; }
        public List<TestResult> TestResults { get; set; }
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