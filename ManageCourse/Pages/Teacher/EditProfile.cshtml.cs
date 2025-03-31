using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ManageCourse.Pages.Teacher
{
    [Authorize(Roles = "Teacher")]
    public class EditProfileModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public EditProfileModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User Teacher { get; set; }

        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login/Login");

            Teacher = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (Teacher == null) return RedirectToPage("/Login/Login");

            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login/Login");

            var teacher = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (teacher == null) return RedirectToPage("/Login/Login");

            teacher.FullName = Request.Form["FullName"];
            teacher.PhoneNumber = Request.Form["PhoneNumber"];
            teacher.Hometown = Request.Form["Hometown"];

            _context.SaveChanges();
            SuccessMessage = "Cập nhật thông tin thành công!";

            Teacher = teacher;

            return Page();
        }
    }
}
