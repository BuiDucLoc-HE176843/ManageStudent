using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ManageCourse.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public string AdminName { get; set; } = "Quản trị viên";

        public DashboardModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                Response.Redirect("/Login/Login");
                return;
            }

            var admin = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (admin != null)
            {
                AdminName = admin.FullName;
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login/Logout");
        }
    }
}
