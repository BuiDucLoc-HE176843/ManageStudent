using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageCourse.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public LoginModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.PasswordHash == Password);

            if (user == null)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);

            if (user.Role == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            if (user.Role == "Teacher")
            {
                return RedirectToPage("/Teacher/Dashboard");
            }

            return RedirectToPage("/Student/Dashboard");
        }
    }
}
