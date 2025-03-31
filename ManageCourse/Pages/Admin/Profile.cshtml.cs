using ManageCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ManageCourse.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ProfileModel : PageModel
    {
        private readonly LearningManagementSystemContext _context;

        public ProfileModel(LearningManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User Admin { get; set; }

        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login/Login");

            Admin = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (Admin == null) return RedirectToPage("/Login/Login");

            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login/Login");

            var admin = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (admin == null) return RedirectToPage("/Login/Login");

            admin.FullName = Request.Form["FullName"];
            admin.Hometown = Request.Form["Hometown"];
            admin.PhoneNumber = Request.Form["PhoneNumber"];

            _context.SaveChanges();
            SuccessMessage = "Cập nhật thông tin thành công!";

            Admin = admin;

            return Page();
        }
    }
}
