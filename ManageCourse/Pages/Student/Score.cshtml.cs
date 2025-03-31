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
    public class ScoreModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ScoreModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<GradeInfo> Grades { get; set; } = new List<GradeInfo>();

        public async Task<IActionResult> OnGetAsync(int classId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Login");
            }

            // Lấy điểm của học sinh trong lớp đó
            Grades = await _context.Grades
                .Where(g => g.StudentId == userId && g.ClassId == classId)
                .Select(g => new GradeInfo
                {
                    CourseName = g.Class.Course.CourseName,
                    Score1 = g.Score1,
                    Score2 = g.Score2,
                    Score3 = g.Score3,
                    Score4 = g.Score4,
                    Score5 = g.Score5,
                    FinalScore = g.FinalScore
                })
                .ToListAsync();

            return Page();
        }

        public class GradeInfo
        {
            public string CourseName { get; set; }
            public double? Score1 { get; set; }
            public double? Score2 { get; set; }
            public double? Score3 { get; set; }
            public double? Score4 { get; set; }
            public double? Score5 { get; set; }
            public double? FinalScore { get; set; }
        }
    }
}
