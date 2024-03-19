using System.Linq;
using WebApplication2.Data.Models;

namespace WebApplication2.Data
{
    public class UserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Users GetUserByCredentials(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == username && u.Password == password);
        }
    }
}

