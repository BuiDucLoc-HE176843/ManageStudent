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
    public class StudentNoticeModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public StudentNoticeModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<User> Students { get; set; }
        public string SuccessMessage => TempData["SuccessMessage"] as string;

        public async Task OnGetAsync()
        {
            // Lấy danh sách tất cả học sinh
            Students = await _context.Users
                .Where(u => u.Role == "Student")
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string message, int[] selectedStudents)
        {
            if (selectedStudents != null && selectedStudents.Length > 0)
            {
                int adminId = HttpContext.Session.GetInt32("UserId") ?? 0;

                if (adminId > 0)
                {
                    foreach (var studentId in selectedStudents)
                    {
                        var notification = new Notification
                        {
                            SenderId = adminId,
                            ReceiverId = studentId,
                            Message = "[Thông báo từ Admin] " + message,
                            CreatedAt = DateTime.Now
                        };

                        _context.Notifications.Add(notification);
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Bạn đã gửi thông báo thành công đến học sinh.";
                }
            }

            return RedirectToPage("/Admin/StudentNotice");
        }
    }
}
