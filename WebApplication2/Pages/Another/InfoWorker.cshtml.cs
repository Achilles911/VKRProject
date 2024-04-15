using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Data.Models;

namespace WebApplication2.Pages.Another
{
    public class InfoWorkerModel : PageModel
    {
        private readonly IApplicationContext _context;

        public InfoWorkerModel(IApplicationContext context)
        {
            _context = context;
        }
        public Users User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await _context.FindUserAsync(id);

            if (User == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, string FullName, string Email, string Password, string PhoneNumber, string Role)
        {
            var User = await _context.FindUserAsync(id);

            if (User == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(FullName))
            {
                User.FullName = FullName;
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                User.Email = Email;
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                User.Password = Password;
            }

            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                User.PhoneNumber = PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(Role))
            {
                User.Role = Role;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Another/Workers");
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int id)
        {
            var userToDelete = await _context.FindUserAsync(id);

            if (userToDelete == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Another/Workers");
        }
    }
}
