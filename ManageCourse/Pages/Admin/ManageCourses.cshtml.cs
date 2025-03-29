using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Admin
{
    public class ManageCoursesModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ManageCoursesModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<Course> Courses { get; set; } = new();
        public List<Class> Classes { get; set; } = new();

        [BindProperty]
        public string NewCourseName { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            Courses = await _context.Courses.ToListAsync();
            Classes = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Classes)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course != null)
            {
                var classIds = course.Classes.Select(c => c.ClassId).ToList();

                var enrollments = _context.Enrollments.Where(e => classIds.Contains(e.ClassId));
                var grades = _context.Grades.Where(g => classIds.Contains(g.ClassId));
                var attendance = _context.StudentAttendances.Where(a => classIds.Contains(a.ClassId));
                var classes = _context.Classes.Where(c => classIds.Contains(c.ClassId));

                _context.Enrollments.RemoveRange(enrollments);
                _context.Grades.RemoveRange(grades);
                _context.StudentAttendances.RemoveRange(attendance);
                _context.Classes.RemoveRange(classes);
                _context.Courses.Remove(course);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa môn học thành công!";
            }
            else
            {
                TempData["Error"] = "Môn học không tồn tại!";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteClassAsync(int classId)
        {
            var classToDelete = await _context.Classes
                .Include(c => c.Enrollments)
                .Include(c => c.Grades)
                .Include(c => c.StudentAttendances)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (classToDelete != null)
            {
                _context.Enrollments.RemoveRange(classToDelete.Enrollments);
                _context.Grades.RemoveRange(classToDelete.Grades);
                _context.StudentAttendances.RemoveRange(classToDelete.StudentAttendances);
                _context.Classes.Remove(classToDelete);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa lớp học thành công!";
            }
            else
            {
                TempData["Error"] = "Lớp học không tồn tại!";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddCourseAsync()
        {
            if (string.IsNullOrWhiteSpace(NewCourseName))
            {
                TempData["Error"] = "Tên môn học không hợp lệ!";
                return RedirectToPage();
            }

            NewCourseName = NewCourseName.Trim();

            var existingCourse = await _context.Courses
                .AnyAsync(c => c.CourseName == NewCourseName);

            if (existingCourse)
            {
                TempData["Error"] = "Môn học đã tồn tại!";
                return RedirectToPage();
            }

            try
            {
                var newCourse = new Course
                {
                    CourseName = NewCourseName
                };

                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm môn học thành công!";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = "Lỗi khi thêm môn học: " + ex.InnerException?.Message;
            }

            return RedirectToPage();
        }
    }
}