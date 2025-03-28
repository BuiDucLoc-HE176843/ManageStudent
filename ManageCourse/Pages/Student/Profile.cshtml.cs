using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ManageCourse.Pages.Student
{
    public class ProfileModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ProfileModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User Student { get; set; }

        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login/Login");

            Student = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (Student == null) return RedirectToPage("/Login/Login");

            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login/Login");

            var student = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (student == null) return RedirectToPage("/Login/Login");

            student.FullName = Request.Form["FullName"];
            student.Hometown = Request.Form["Hometown"];
            student.PhoneNumber = Request.Form["PhoneNumber"];

            _context.SaveChanges();
            SuccessMessage = "Cập nhật thông tin thành công!";

            Student = student;

            return Page();
        }
    }
}
