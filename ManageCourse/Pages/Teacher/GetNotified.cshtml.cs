using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageCourse.Pages.Teacher
{
    [Authorize(Roles = "Teacher")]
    public class GetNotifiedModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public GetNotifiedModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<Notification> UserNotifications { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Login"); // Chuyển hướng về trang đăng nhập nếu chưa đăng nhập
            }

            UserNotifications = await _context.Notifications
                .Where(n => n.ReceiverId == userId)
                .Include(n => n.Sender)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Page();
        }
    }
}
