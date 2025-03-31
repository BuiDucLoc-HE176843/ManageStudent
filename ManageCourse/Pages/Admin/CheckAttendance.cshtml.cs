using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageCourse.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CheckAttendanceModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public CheckAttendanceModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public Class Class { get; set; }
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public int SessionNumber { get; set; }

        [BindProperty]
        public Dictionary<int, byte?> Attendance { get; set; } = new Dictionary<int, byte?>();

        public async Task OnGetAsync(int classId, int sessionNumber)
        {
            // Lấy thông tin lớp học và các enrollment
            Class = await _context.Classes
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                        .ThenInclude(s => s.StudentAttendances)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            SessionNumber = sessionNumber;

            if (Class != null)
            {
                Enrollments = Class.Enrollments.ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync(int classId, int sessionNumber)
        {
            try
            {
                // Lấy thông tin lớp học và các enrollment
                Class = await _context.Classes
                    .Include(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                            .ThenInclude(s => s.StudentAttendances)
                    .FirstOrDefaultAsync(c => c.ClassId == classId);

                if (Class != null)
                {
                    bool allAttended = true;

                    foreach (var enrollment in Class.Enrollments)
                    {
                        // Kiểm tra trạng thái điểm danh
                        byte? isPresent = Attendance.ContainsKey(enrollment.StudentId) ? Attendance[enrollment.StudentId] : null;

                        if (isPresent == null)
                        {
                            allAttended = false; // Nếu có sinh viên chưa điểm danh, set allAttended = false
                        }

                        // Cập nhật hoặc tạo mới bản ghi điểm danh
                        var studentAttendance = await _context.StudentAttendances
                            .FirstOrDefaultAsync(sa => sa.ClassId == classId && sa.SessionNumber == sessionNumber && sa.StudentId == enrollment.StudentId);

                        if (studentAttendance == null)
                        {
                            studentAttendance = new StudentAttendance
                            {
                                ClassId = classId,
                                SessionNumber = sessionNumber,
                                StudentId = enrollment.StudentId,
                            };
                            _context.StudentAttendances.Add(studentAttendance);
                        }

                        studentAttendance.IsPresent = isPresent;
                    }

                    // Nếu có sinh viên chưa điểm danh, hiển thị thông báo
                    if (!allAttended)
                    {
                        TempData["ErrorMessage"] = "Chưa điểm danh đủ. Vui lòng điểm danh tất cả sinh viên.";
                    }
                    else
                    {
                        // Nếu không có lỗi, lưu và hiển thị thông báo thành công
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Điểm danh đã được lưu thành công!";
                    }
                }
                else
                {
                    // Nếu không tìm thấy lớp học, lưu thông báo lỗi
                    TempData["ErrorMessage"] = "Lớp học không tồn tại.";
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi trong quá trình lưu, lưu thông báo lỗi
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToPage("/Admin/CheckAttendance", new { classId = classId, sessionNumber = sessionNumber });
        }
    }
}
