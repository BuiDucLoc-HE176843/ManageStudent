using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageCourse.Pages.Teacher
{
    [Authorize(Roles = "Teacher")]
    public class NotificationModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public NotificationModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<User> Students { get; set; }
        public int ClassId { get; set; }
        public string Message { get; set; }

        public string SuccessMessage => TempData["SuccessMessage"] as string;

        public async Task OnGetAsync(int classId)
        {
            ClassId = classId;

            // Lấy danh sách sinh viên của lớp
            Students = await _context.Enrollments
                .Where(e => e.ClassId == classId)
                .Select(e => e.Student)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int classId, string message, int[] selectedStudents)
        {
            if (selectedStudents != null && selectedStudents.Length > 0)
            {
                string classNane = _context.Classes.FirstOrDefault(x => x.ClassId == classId).ClassName;

                int teacherId = HttpContext.Session.GetInt32("UserId") ?? 0;

                if (teacherId > 0)
                {
                    foreach (var studentId in selectedStudents)
                    {
                        var notification = new Notification
                        {
                            SenderId = teacherId,
                            ReceiverId = studentId,
                            Message = "["+classNane+"] " + message,
                            CreatedAt = DateTime.Now
                        };

                        _context.Notifications.Add(notification);
                    }

                    await _context.SaveChangesAsync();

                    // Lưu vào TempData để hiển thị sau khi redirect
                    TempData["SuccessMessage"] = "Bạn đã gửi thông báo thành công đến sinh viên.";
                }
            }

            return RedirectToPage("/Teacher/Notification", new { classId });
        }
    }
}
