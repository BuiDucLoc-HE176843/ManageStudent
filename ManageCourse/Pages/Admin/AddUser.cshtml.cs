using ManageCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AddUserModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public AddUserModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User NewUser { get; set; } = new User();

        public void OnGet(string actionAdd)
        {
            if (actionAdd == "hocsinh")
            {
                NewUser.Role = "Student";
            }
            else if (actionAdd == "giaovien")
            {
                NewUser.Role = "Teacher";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra User có tồn tại không
            if (_context.Users.Any(u => u.Email == NewUser.Email))
            {
                ModelState.AddModelError("NewUser.Email", "Email already exists.");
            }

            if (_context.Users.Any(u => u.PhoneNumber == NewUser.PhoneNumber))
            {
                ModelState.AddModelError("NewUser.PhoneNumber", "Phone number already exists.");
            }

            // Nếu có lỗi, quay lại trang và hiển thị lỗi
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Lưu user vào database nếu không có lỗi
            _context.Users.Add(NewUser);
            await _context.SaveChangesAsync();

            // Lưu thông báo thành công
            TempData["SuccessMessage"] = "User added successfully!";

            // Redirect với actionAdd để tránh lỗi thiếu tham số
            return RedirectToPage(new { actionAdd = NewUser.Role == "Student" ? "hocsinh" : "giaovien" });
        }
    }
}
