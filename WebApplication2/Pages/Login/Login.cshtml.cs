using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebApplication2.Data;

namespace WebApplication2.Pages.Login
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserRepository _userRepository;

        [BindProperty]
        public Credential Credential { get; set; }

        public LoginModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _userRepository.GetUserByCredentials(Credential.UserName, Credential.Password);

            if (user != null)
            {
                
                return RedirectToPage("/Main/Menu");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return Page();
            }
        }
        public IActionResult OnGet(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            return Page();
        }

    }
    public class Credential
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }


}
