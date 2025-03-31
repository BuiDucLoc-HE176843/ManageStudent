using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ManageCourse.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UpdateOtherProfileModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public UpdateOtherProfileModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User Other { get; set; }

        public string SuccessMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            // Lấy thông tin sinh viên theo UserId
            Other = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (Other == null)
            {
                return RedirectToPage("/Admin/ManageStudents");
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (Other == null)
            {
                return RedirectToPage("/Admin/ManageStudents");
            }

            var existingStudent = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (existingStudent == null)
            {
                return RedirectToPage("/Admin/ManageStudents");
            }

            existingStudent.FullName = Other.FullName;
            existingStudent.Hometown = Other.Hometown;
            existingStudent.PhoneNumber = Other.PhoneNumber;

            _context.SaveChanges();

            Other = existingStudent;
            SuccessMessage = "Cập nhật thông tin thành công!";
            return Page();
        }
    }
}

