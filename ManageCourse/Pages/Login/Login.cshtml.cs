using ManageCourse.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

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

        public async Task<IActionResult> OnPost()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.PasswordHash == Password);

            if (user == null)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            // Tạo các Claims cho người dùng
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Đăng nhập người dùng
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Lưu ID người dùng vào session
            HttpContext.Session.SetInt32("UserId", user.UserId);

            // Chuyển hướng tới trang tương ứng với vai trò
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
