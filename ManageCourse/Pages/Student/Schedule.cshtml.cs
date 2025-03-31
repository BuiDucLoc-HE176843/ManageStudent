using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class ScheduleModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ScheduleModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<Schedule> Schedules { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var studentId = HttpContext.Session.GetInt32("UserId");

            if (studentId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập trước khi xem lịch học!";
                return RedirectToPage("/Login/Login");
            }

            // Lấy lịch học của sinh viên
            Schedules = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.Class)
                .SelectMany(c => c.Schedules)
                .Include(s => s.Class)
                .ThenInclude(c => c.Teacher)
                .Where(c => c.Class.Status != 2)
                .ToListAsync();

            // Kiểm tra nếu không có lịch học
            if (Schedules == null || !Schedules.Any())
            {
                TempData["ErrorMessage"] = "Không có lịch học!";
            }

            return Page();
        }

    }
}
