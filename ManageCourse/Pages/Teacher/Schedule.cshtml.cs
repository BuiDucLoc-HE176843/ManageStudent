using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageCourse.Pages.Teacher
{
    [Authorize(Roles = "Teacher")]
    public class ScheduleModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ScheduleModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<Schedule> Schedules { get; set; } = new List<Schedule>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy ID của giáo viên từ session
            var teacherId = HttpContext.Session.GetInt32("UserId");

            if (teacherId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin giáo viên!";
                return RedirectToPage("/Login/Login");
            }

            // Truy vấn các lịch dạy của giáo viên
            Schedules = await _context.Schedules
                .Include(s => s.Class)
                .ThenInclude(c => c.Teacher) // Lấy tên giáo viên
                .Where(s => s.Class.TeacherId == teacherId)
                .ToListAsync();

            // Nếu không có lịch học
            if (!Schedules.Any())
            {
                TempData["ErrorMessage"] = "Không có lịch dạy!";
            }

            return Page();
        }
    }
}
