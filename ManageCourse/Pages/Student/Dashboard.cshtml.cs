using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ManageCourse.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class DashboardModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public string StudentName { get; set; } = "Sinh viên";

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

            var student = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (student != null)
            {
                StudentName = student.FullName;
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login/Logout");
        }
    }
}
