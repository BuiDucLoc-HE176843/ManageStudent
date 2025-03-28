using ManageCourse.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ManageCourse.Pages.Teacher
{
    public class DashboardModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public string TeacherName { get; set; } = "Gi�o vi�n";

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

            var teacher = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (teacher != null)
            {
                TeacherName = teacher.FullName;
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login/Login");
        }
    }
}
