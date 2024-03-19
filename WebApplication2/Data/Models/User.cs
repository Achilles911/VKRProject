using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Data.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public decimal? PhoneNumber { get; set; }

        public string Role { get; set; }
    }
}
