using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCourse.Models;

namespace ManageCourse.Pages.Student
{
    public class ClassModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ClassModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<ClassViewModel> Classes { get; set; } = new();

        public async Task OnGetAsync()
        {
            Classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .Select(c => new ClassViewModel
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    CourseId = c.CourseId,
                    CourseName = c.Course.CourseName, // Lấy tên khóa học
                    TeacherId = c.TeacherId,
                    TeacherName = c.Teacher.FullName, // Lấy tên giáo viên
                    Status = c.Status == 1 ? "Đang diễn ra" : "Đã kết thúc"
                }).ToListAsync();
        }
    }

    public class ClassViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
