using ManageCourse.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Student
{
    public class AttendanceModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public AttendanceModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        public List<AttendanceInfo> Attendances { get; set; } = new List<AttendanceInfo>();
        public double AbsencePercentage { get; set; }
        public string AbsenceMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int classId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            // Danh sách Session từ 1 đến 20
            var allSessions = Enumerable.Range(1, 20).ToList();

            // Lấy thông tin điểm danh từ cơ sở dữ liệu
            var attendanceData = await _context.StudentAttendances
                .Where(a => a.StudentId == userId && a.ClassId == classId)
                .Select(a => new { a.SessionNumber, a.IsPresent })
                .ToListAsync();

            // Lập danh sách AttendanceInfo cho 20 buổi học
            int absentCount = 0;
            foreach (var session in allSessions)
            {
                var attendance = attendanceData.FirstOrDefault(a => a.SessionNumber == session);

                if (attendance != null)
                {
                    // Nếu có thông tin điểm danh
                    Attendances.Add(new AttendanceInfo
                    {
                        SessionNumber = session,
                        IsPresent = attendance.IsPresent == 1 ? "Present" : "Absent"
                    });

                    // Nếu vắng mặt, tăng số buổi vắng mặt
                    if (attendance.IsPresent == 0)
                    {
                        absentCount++;
                    }
                }
                else
                {
                    // Nếu không có thông tin điểm danh, hiển thị "Not Yet"
                    Attendances.Add(new AttendanceInfo
                    {
                        SessionNumber = session,
                        IsPresent = "Not Yet"
                    });
                }
            }

            // Tính tỷ lệ nghỉ
            AbsencePercentage = (double)absentCount / 20 * 100;

            // Kiểm tra tỷ lệ nghỉ
            if (AbsencePercentage > 20)
            {
                AbsenceMessage = "Bạn đã nghỉ quá 20% và sẽ không được qua môn này.";
            }
            else
            {
                AbsenceMessage = $"Tỷ lệ nghỉ: {AbsencePercentage:F2}%";
            }

            return Page();
        }

        public class AttendanceInfo
        {
            public int SessionNumber { get; set; }
            public string IsPresent { get; set; }
        }
    }
}
