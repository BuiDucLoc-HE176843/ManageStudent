﻿using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Admin
{
    public class ManageStudentModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ManageStudentModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public Class ClassItem { get; set; }
        public List<User> EnrolledStudents { get; set; }
        public List<User> AvailableStudents { get; set; }
        public string SuccessMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int classId)
        {
            ClassItem = await _context.Classes
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (ClassItem == null)
            {
                return NotFound();
            }

            EnrolledStudents = ClassItem.Enrollments.Select(e => e.Student).ToList();
            AvailableStudents = await _context.Users
                .Where(u => u.Role == "Student" && !EnrolledStudents.Contains(u))
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveStudentAsync(int studentId, int classId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.ClassId == classId);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                SuccessMessage = "Xóa sinh viên khỏi lớp thành công!";
            }

            return await OnGetAsync(classId);
        }

        public async Task<IActionResult> OnPostAddStudentAsync(int studentId, int classId)
        {
            // Lấy thông tin lớp hiện tại
            var currentClass = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (currentClass == null)
            {
                return NotFound();
            }

            // Kiểm tra sinh viên có học lớp nào khác của cùng CourseId và Status = 1 hay không
            bool isEnrolledInOtherClass = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId &&
                               e.Class.CourseId == currentClass.CourseId &&
                               e.Class.Status == 1);

            if (isEnrolledInOtherClass)
            {
                ModelState.AddModelError(string.Empty, "Học sinh đã trong lớp khác học môn này.");
                return await OnGetAsync(classId);
            }

            // Nếu không có lớp nào vi phạm, tiến hành thêm sinh viên
            bool exists = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.ClassId == classId);

            if (!exists)
            {
                _context.Enrollments.Add(new Enrollment
                {
                    StudentId = studentId,
                    ClassId = classId
                });

                await _context.SaveChangesAsync();
                SuccessMessage = "Thêm sinh viên vào lớp thành công!";
            }

            return await OnGetAsync(classId);
        }
    }
}
