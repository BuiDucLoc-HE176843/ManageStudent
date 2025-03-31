using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class TeacherNoticeModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public TeacherNoticeModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<User> Teachers { get; set; }
        public string SuccessMessage => TempData["SuccessMessage"] as string;

        public async Task OnGetAsync()
        {
            // Lấy danh sách tất cả giáo viên
            Teachers = await _context.Users
                .Where(u => u.Role == "Teacher")
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string message, int[] selectedTeachers)
        {
            if (selectedTeachers != null && selectedTeachers.Length > 0)
            {
                int adminId = HttpContext.Session.GetInt32("UserId") ?? 0;

                if (adminId > 0)
                {
                    foreach (var teacherId in selectedTeachers)
                    {
                        var notification = new Notification
                        {
                            SenderId = adminId,
                            ReceiverId = teacherId,
                            Message = "[Thông báo từ Admin] " + message,
                            CreatedAt = DateTime.Now
                        };

                        _context.Notifications.Add(notification);
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Bạn đã gửi thông báo thành công đến giáo viên.";
                }
            }

            return RedirectToPage("/Admin/TeacherNotice");
        }
    }
}
