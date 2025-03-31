using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class CourcesModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public List<ClassInfo> StudentClasses { get; set; } = new List<ClassInfo>();

        public CourcesModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Login");
            }

            StudentClasses = await _context.Enrollments
                .Where(e => e.StudentId == userId)
                .Include(e => e.Class)
                    .ThenInclude(c => c.Course)
                .Include(e => e.Class)
                    .ThenInclude(c => c.Teacher)
                .Select(e => new ClassInfo
                {
                    ClassId = e.Class.ClassId,
                    ClassName = e.Class.ClassName,
                    CourseName = e.Class.Course.CourseName,
                    TeacherName = e.Class.Teacher.FullName,
                    Status = e.Class.Status == 1 ? "Đang diễn ra" : "Đã kết thúc"
                })
                .ToListAsync();

            return Page();
        }

        public class ClassInfo
        {
            public int ClassId { get; set; }
            public string ClassName { get; set; }
            public string CourseName { get; set; }
            public string TeacherName { get; set; }
            public string Status { get; set; } = null!;
        }
    }
}