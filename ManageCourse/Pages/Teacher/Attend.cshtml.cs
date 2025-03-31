using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageCourse.Pages.Teacher
{
    [Authorize(Roles = "Teacher")]
    public class AttendModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public AttendModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public Class Class { get; set; }
        public List<int> SessionNumbers { get; set; } = new List<int>();

        public async Task OnGetAsync(int classId)
        {
            // Lấy thông tin lớp học theo classId
            Class = await _context.Classes
                .Include(c => c.StudentAttendances)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (Class != null)
            {
                SessionNumbers = Enumerable.Range(1, 20).ToList();
            }
        }
    }
}
