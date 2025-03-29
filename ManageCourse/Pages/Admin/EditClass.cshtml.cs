using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Admin
{
    public class EditClassModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public EditClassModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class ClassItem { get; set; } = new();

        public List<User> Students { get; set; } = new();
        public List<User> AllStudents { get; set; } = new();
        public SelectList Teachers { get; set; } = new(new List<User>(), "UserId", "FullName");
        public SelectList Courses { get; set; } = new(new List<Course>(), "CourseId", "CourseName");
        public string SuccessMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int classId)
        {
            ClassItem = await _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Course)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (ClassItem == null)
            {
                return NotFound();
            }

            Students = ClassItem.Enrollments.Select(e => e.Student).ToList();
            AllStudents = await _context.Users.Where(u => u.Role == "Student").ToListAsync();
            Teachers = new SelectList(await _context.Users.Where(u => u.Role == "Teacher").ToListAsync(), "UserId", "FullName");
            Courses = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var classToUpdate = await _context.Classes.FindAsync(ClassItem.ClassId);
            if (classToUpdate == null)
            {
                return NotFound();
            }

            classToUpdate.ClassName = ClassItem.ClassName;
            classToUpdate.TeacherId = ClassItem.TeacherId;
            classToUpdate.CourseId = ClassItem.CourseId;
            classToUpdate.TotalSessions = ClassItem.TotalSessions;
            classToUpdate.Status = ClassItem.Status;

            await _context.SaveChangesAsync();
            SuccessMessage = "Cập nhật lớp học thành công!";

            // Load lại danh sách để hiển thị đầy đủ
            return await OnGetAsync(ClassItem.ClassId);
        }


        public async Task<IActionResult> OnPostRemoveStudentAsync(int studentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.ClassId == ClassItem.ClassId);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            SuccessMessage = "Xóa sinh viên thành công!";
            return await OnGetAsync(ClassItem.ClassId);
        }

        public async Task<IActionResult> OnPostAddStudentAsync(int newStudentId)
        {
            var exists = await _context.Enrollments
                .AnyAsync(e => e.StudentId == newStudentId && e.ClassId == ClassItem.ClassId);

            if (!exists)
            {
                var enrollment = new Enrollment { StudentId = newStudentId, ClassId = ClassItem.ClassId };
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                SuccessMessage = "Thêm sinh viên thành công!";
            }

            return await OnGetAsync(ClassItem.ClassId);
        }
    }
}
