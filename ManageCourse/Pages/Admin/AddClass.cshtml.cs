using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace ManageCourse.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AddClassModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public AddClassModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string ClassName { get; set; } = string.Empty;
        [BindProperty]
        public int CourseId { get; set; }
        [BindProperty]
        public int TeacherId { get; set; }
        [BindProperty]
        public byte Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public List<Course> Courses { get; set; } = new();
        public List<User> Teachers { get; set; } = new();

        public void OnGet()
        {
            Courses = _context.Courses.ToList();
            Teachers = _context.Users.Where(u => u.Role == "Teacher").ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Courses = _context.Courses.ToList();
                Teachers = _context.Users.Where(u => u.Role == "Teacher").ToList();
                return Page();
            }

            if (_context.Classes.Any(c => c.ClassName == ClassName))
            {
                ErrorMessage = "Class name already exists!";
                Courses = _context.Courses.ToList();
                Teachers = _context.Users.Where(u => u.Role == "Teacher").ToList();
                return Page();
            }

            var newClass = new Class
            {
                ClassName = ClassName,
                CourseId = CourseId,
                TeacherId = TeacherId,
                Status = Status
            };

            _context.Classes.Add(newClass);
            _context.SaveChanges();

            Message = "Class added successfully!";
            Courses = _context.Courses.ToList();
            Teachers = _context.Users.Where(u => u.Role == "Teacher").ToList();
            return Page();
        }
    }
}
