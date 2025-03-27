using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageCourse.Pages.Login
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ForgotPasswordModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string PhoneNumber { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        public bool IsValidUser { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            IsValidUser = false;
            ErrorMessage = string.Empty;
        }

        public void OnPostCheckUser()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.PhoneNumber == PhoneNumber);

            if (user == null)
            {
                ErrorMessage = "Invalid information";
                IsValidUser = false;
            }
            else
            {
                IsValidUser = true;
            }
        }

        public IActionResult OnPostResetPassword()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.PhoneNumber == PhoneNumber);

            if (user == null)
            {
                ErrorMessage = "Invalid information";
                IsValidUser = false;
                return Page();
            }

            user.PasswordHash = NewPassword;
            _context.SaveChanges();

            return RedirectToPage("/Login/Login");
        }
    }
}
