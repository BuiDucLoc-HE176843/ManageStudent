using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ScheduleModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ScheduleModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<Schedule> Schedules { get; set; } = new();
        public List<Class> Classes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Schedules = await _context.Schedules
                .Include(s => s.Class)
                .ThenInclude(c => c.Teacher) // Lấy tên giáo viên
                .Where(c => c.Class.Status !=2)
                .ToListAsync();

            Classes = await _context.Classes
                .Include(c => c.Teacher)// Lấy thông tin giáo viên
                .Where(c => c.Status != 2)
                .ToListAsync();

            return Page();
        }

        // Xử lý thêm lịch học mới với các kiểm tra
        public async Task<IActionResult> OnPostAddScheduleAsync(int classId, int dayOfWeek, byte session)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem lịch học đã tồn tại chưa
                var existingSchedule = await _context.Schedules
                    .FirstOrDefaultAsync(s => s.ClassId == classId && s.DayOfWeek == dayOfWeek && s.Session == session);

                if (existingSchedule != null)
                {
                    TempData["ErrorMessage"] = "Lịch học đã tồn tại!";
                    return RedirectToPage();
                }

                // Lấy tất cả các sinh viên trong lớp và kiểm tra lịch học của họ
                var studentsInClass = await _context.Enrollments
                    .Where(e => e.ClassId == classId)
                    .Select(e => e.StudentId)
                    .ToListAsync();

                foreach (var studentId in studentsInClass)
                {
                    var conflictingSchedules = await _context.Enrollments
                        .Where(e => e.StudentId == studentId && e.ClassId != classId)  // Lọc các lớp mà sinh viên đã đăng ký, ngoại trừ lớp hiện tại
                        .Select(e => e.Class)
                        .Where(c => c.Schedules
                            .Any(s => s.DayOfWeek == dayOfWeek && s.Session == session))  // Kiểm tra xem lớp đã đăng ký có lịch trùng không
                        .ToListAsync();

                    if (conflictingSchedules.Any())
                    {
                        TempData["ErrorMessage"] = "Lịch học của sinh viên trùng nhau!";
                        return RedirectToPage();
                    }
                }

                // Lấy giáo viên của lớp hiện tại
                var teacherId = _context.Classes
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.TeacherId)
                    .FirstOrDefault();

                if (teacherId != 0)
                {
                    // Kiểm tra xem giáo viên có lớp trùng lịch không
                    var conflictingTeacherSchedules = await _context.Classes
                        .Where(c => c.TeacherId == teacherId)
                        .Select(c => c.Schedules)
                        .Where(s => s.Any(sch => sch.DayOfWeek == dayOfWeek && sch.Session == session))
                        .ToListAsync();

                    if (conflictingTeacherSchedules.Any())
                    {
                        TempData["ErrorMessage"] = "Lịch dạy của giáo viên trùng nhau!";
                        return RedirectToPage();
                    }
                }

                // Thêm lịch học mới nếu không có sự xung đột
                var newSchedule = new Schedule
                {
                    ClassId = classId,
                    DayOfWeek = dayOfWeek,
                    Session = session
                };

                _context.Schedules.Add(newSchedule);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm lịch học thành công!";
                return RedirectToPage(); // Điều hướng lại về trang hiện tại để hiển thị thông báo
            }

            // Nếu có lỗi, giữ lại trang và thông báo lỗi
            TempData["ErrorMessage"] = "Thêm lịch học thất bại!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteScheduleAsync(int scheduleId)
        {
            // Tìm lịch học cần xóa
            var scheduleToDelete = await _context.Schedules
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (scheduleToDelete != null)
            {
                // Xóa lịch học
                _context.Schedules.Remove(scheduleToDelete);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lịch học đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch học để xóa!";
            }

            return RedirectToPage(); // Điều hướng lại về trang hiện tại để hiển thị thông báo
        }

    }
}
