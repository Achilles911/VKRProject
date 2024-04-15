using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Data.Models;

namespace WebApplication2.Pages.Another
{
    public class WorkersModel : PageModel
    {
        private readonly IApplicationContext _context;

        public WorkersModel(IApplicationContext context)
        {
            _context = context;
        }

        public IList<Users> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
            return Page();
        }
    }
}
