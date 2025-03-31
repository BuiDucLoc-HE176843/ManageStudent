using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageCourse.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageScoreModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ManageScoreModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public Class Class { get; set; }
        public IList<Enrollment> Enrollments { get; set; }
        public IList<Grade> Grades { get; set; }

        public async Task<IActionResult> OnGetAsync(int classId)
        {
            // Lấy lớp học và danh sách sinh viên đã đăng ký
            Class = await _context.Classes
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (Class == null)
            {
                return NotFound();
            }

            Enrollments = Class.Enrollments.ToList();

            // Lấy điểm của sinh viên trong lớp
            Grades = await _context.Grades.Where(g => g.ClassId == classId).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int classId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Lấy danh sách sinh viên trong lớp
            var enrollments = await _context.Enrollments
                .Where(e => e.ClassId == classId)
                .ToListAsync();

            foreach (var enrollment in enrollments)
            {
                var grade = await _context.Grades
                    .FirstOrDefaultAsync(g => g.ClassId == classId && g.StudentId == enrollment.StudentId);

                // Nếu chưa có điểm, tạo mới
                if (grade == null)
                {
                    grade = new Grade
                    {
                        ClassId = classId,
                        StudentId = enrollment.StudentId
                    };
                    _context.Grades.Add(grade);
                }

                // Lấy điểm từ form
                grade.Score1 = GetScore($"Score1_{enrollment.StudentId}");
                grade.Score2 = GetScore($"Score2_{enrollment.StudentId}");
                grade.Score3 = GetScore($"Score3_{enrollment.StudentId}");
                grade.Score4 = GetScore($"Score4_{enrollment.StudentId}");
                grade.Score5 = GetScore($"Score5_{enrollment.StudentId}");

            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/ManageScore", new { classId });
        }

        private double? GetScore(string key)
        {
            if (Request.Form.TryGetValue(key, out var value) && double.TryParse(value, out var result))
            {
                return result;
            }
            return null;
        }
    }
}
