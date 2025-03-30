using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageCourse.Pages.Teacher
{
    public class ListClassModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ListClassModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public IList<Class> Classes { get; set; }
        public string Action { get; set; }

        public async Task OnGetAsync(string action)
        {
            // Lấy thông tin TeacherId từ session
            int? teacherId = HttpContext.Session.GetInt32("UserId");

            if (teacherId.HasValue)
            {
                // Lấy danh sách lớp học của giáo viên
                Classes = await _context.Classes
                    .Where(c => c.TeacherId == teacherId.Value)  // Sử dụng teacherId.Value
                    .Where(c => c.Status != 2)
                    .Include(c => c.Course)
                    .ToListAsync();
            }
            else
            {
                Classes = new List<Class>(); // Hoặc bạn có thể thông báo lỗi tùy ý
            }

            Action = action; // Nhận giá trị của action từ URL
        }
    }
}
