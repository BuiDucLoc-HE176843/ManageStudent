using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace ManageCourse.Pages.Admin
{
    public class ManageOtherProfileModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ManageOtherProfileModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public IQueryable<User> Other { get; set; }
        public string SearchName { get; set; }
        public string SearchHometown { get; set; }
        public string SearchPhoneNumber { get; set; }
        public string SearchEmail { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;

        public string Action { get; set; }

        public async Task OnGetAsync(string action, string searchName, string searchHometown, string searchPhoneNumber, string searchEmail, int pageNumber = 1)
        {
            var query = _context.Users.Where(u => u.Role == "student");

            if (action == "giaovien")
            {
                query = _context.Users.Where(u => u.Role == "teacher");
            }

            // Áp dụng các bộ lọc tìm kiếm
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(u => u.FullName.Contains(searchName));
            }
            if (!string.IsNullOrEmpty(searchHometown))
            {
                query = query.Where(u => u.Hometown.Contains(searchHometown));
            }
            if (!string.IsNullOrEmpty(searchPhoneNumber))
            {
                query = query.Where(u => u.PhoneNumber.Contains(searchPhoneNumber));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                query = query.Where(u => u.Email.Contains(searchEmail));
            }

            Action = action;

            // Phân trang
            Other = query.Skip((pageNumber - 1) * PageSize).Take(PageSize);
            var totalUsers = await query.CountAsync();
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalUsers / PageSize);
            ViewData["CurrentPage"] = pageNumber;

            // Lưu các tham số tìm kiếm vào ViewData để sử dụng trong form tìm kiếm
            SearchName = searchName;
            SearchHometown = searchHometown;
            SearchPhoneNumber = searchPhoneNumber;
            SearchEmail = searchEmail;
        }

        public async Task<IActionResult> OnPostDeleteAsync(int userId, string action)
        {
            var user = await _context.Users
                .Include(u => u.Enrollments)
                .Include(u => u.Grades)
                .Include(u => u.StudentAttendances)
                .Include(u => u.NotificationReceivers)
                .Include(u => u.NotificationSenders)
                .Include(u => u.Classes)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Xóa dữ liệu liên quan đến user nếu là Student
            if (user.Role == "Student")
            {
                _context.Enrollments.RemoveRange(user.Enrollments);
                _context.Grades.RemoveRange(user.Grades);
                _context.StudentAttendances.RemoveRange(user.StudentAttendances);
                _context.Notifications.RemoveRange(user.NotificationReceivers);
                _context.Notifications.RemoveRange(user.NotificationSenders);
            }
            // Xóa dữ liệu liên quan đến user nếu là Teacher
            else if (user.Role == "Teacher")
            {
                _context.Classes.RemoveRange(user.Classes);

                var allStudentEnrollments = _context.Enrollments.Where(e => e.Class.TeacherId == userId).ToList();
                _context.Enrollments.RemoveRange(allStudentEnrollments);

                var allStudentGrades = _context.Grades.Where(g => g.Class.TeacherId == userId).ToList();
                _context.Grades.RemoveRange(allStudentGrades);

                var allStudentAttendances = _context.StudentAttendances.Where(sa => sa.Class.TeacherId == userId).ToList();
                _context.StudentAttendances.RemoveRange(allStudentAttendances);

                var allNotifications = _context.Notifications.Where(n => n.SenderId == userId).ToList();
                _context.Notifications.RemoveRange(allNotifications);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Redirect lại trang hiện tại với action đã được truyền lại
            return RedirectToPage(new { action = action });
        }

    }
}

