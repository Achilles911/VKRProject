using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication2.Data;
using WebApplication2.Data.Models;

namespace WebApplication2.Pages.Another
{
    public class CreateWorkerModel : PageModel
    {
        private readonly ApplicationContext _context;

        public CreateWorkerModel(ApplicationContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string FullName, string Email, string Password, string PhoneNumber, string Role)
        {
            var maxId = _context.Users.Max(a => (int?)a.Id) ?? 0;
            var newId = maxId + 1;

            var user = new Users
            {
                Id = newId,
                FullName = FullName,
                Email = Email,
                Password = Password,
                PhoneNumber = PhoneNumber,
                Role = Role
                
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Another/Workers");
        }
    }
}
