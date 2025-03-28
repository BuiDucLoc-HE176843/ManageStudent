using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Teacher
{
    public class ScoreModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ScoreModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public Class Class { get; set; }
        public IList<Grade> Grades { get; set; }

        public async Task<IActionResult> OnGetAsync(int classId)
        {
            // Lấy lớp học với các sinh viên đã đăng ký
            Class = await _context.Classes
                .Include(c => c.Grades)
                .ThenInclude(g => g.Student)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (Class == null)
            {
                return NotFound();
            }

            // Lấy điểm của các sinh viên trong lớp này
            Grades = Class.Grades.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int classId)
        {
            // Cập nhật điểm của các sinh viên trong lớp
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Lấy lớp học và điểm của các sinh viên
            var gradesToUpdate = await _context.Grades
                .Where(g => g.ClassId == classId)
                .ToListAsync();

            foreach (var grade in gradesToUpdate)
            {
                var score1 = Convert.ToDouble(Request.Form[$"Score1_{grade.StudentId}"]);
                var score2 = Convert.ToDouble(Request.Form[$"Score2_{grade.StudentId}"]);
                var score3 = Convert.ToDouble(Request.Form[$"Score3_{grade.StudentId}"]);
                var score4 = Convert.ToDouble(Request.Form[$"Score4_{grade.StudentId}"]);
                var score5 = Convert.ToDouble(Request.Form[$"Score5_{grade.StudentId}"]);

                // Cập nhật điểm cho sinh viên
                grade.Score1 = score1;
                grade.Score2 = score2;
                grade.Score3 = score3;
                grade.Score4 = score4;
                grade.Score5 = score5;

                // Tính điểm cuối cùng và làm tròn đến 2 chữ số sau dấu thập phân
                double finalScore = (score1 + score2 + score3 + score4 + score5) / 5;
                grade.FinalScore = Math.Round(finalScore, 2); // Làm tròn đến 2 chữ số sau dấu thập phân
            }

            // Lưu lại các thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return RedirectToPage("/Teacher/Score", new { classId = classId });
        }
    }
}
