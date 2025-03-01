using ManageCoure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageCoure.Controllers
{
    public class AdminController : Controller
    {
        private readonly PrnassmContext _contextDAO;
        public AdminController(PrnassmContext context)
        {
            _contextDAO = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageStudent()
        {
            var listStudent = _contextDAO.Accounts
       .Where(st => st.Role == 1)
       .Select(st => new
       {
           st.Id,
           st.Name,
           st.Dob,
           st.Email,
           st.Phone,
           st.Address,
           st.Code,
           Classes = st.StudentClasses.Select(sc => sc.Class).ToList() 
       })
       .ToList();
            ViewBag.listStudent = listStudent;
            return View();
        }

        public IActionResult AddStudent()
        {
            var listClass = _contextDAO.Classes.ToList();
            ViewBag.listClass = listClass;
            return View();
        }

        [HttpPost]
        public IActionResult AddStudent(string Name, DateOnly Dob, string Phone, string Address, string Code, int ClassId)
        {
            var student = _contextDAO.Accounts.FirstOrDefault(st => st.Code == Code);
            if(student != null)
            {
                ViewBag.CodeExited = "Code is exited please input different code";
                return View();
            }
            var studentPhone = _contextDAO.Accounts.FirstOrDefault(st => st.Phone == Phone);
            if (studentPhone != null)
            {
                ViewBag.PhoneExit = "Phone is exited please input different Phone";
                return View();
            }
            Account ac = new Account
            {
                Name = Name,
                Dob = Dob,
                Phone = Phone,
                Address = Address,
                Code = Code,
                Role = 1
            };
            _contextDAO.Accounts.Add(ac);
            _contextDAO.SaveChanges();
            StudentClass st = new StudentClass
            {
                StudentId = ac.Id,
                ClassId = ClassId
            };
            _contextDAO.StudentClasses.Add(st);
            _contextDAO.SaveChanges();
            return RedirectToAction("ManageStudent");
        }

        public IActionResult EditStudent(int Id)
        {
            var student = _contextDAO.Accounts.FirstOrDefault(st => st.Id == Id);
            ViewBag.getInfor = student;
            var listClass = _contextDAO.Classes.ToList();
            ViewBag.listClass = listClass;
            return View();
        }

        public IActionResult SearchStudent(string Search)
        {
            var listStudent = _contextDAO.Accounts
                .Where(st => st.Role == 1 && st.Name.Contains(Search))
                .Select(st => new
                {
                    st.Id,
                    st.Name,
                    st.Dob,
                    st.Email,
                    st.Phone,
                    st.Address,
                    st.Code,
                    Classes = st.StudentClasses.Select(sc => sc.Class).ToList()
                })
                .ToList();

            ViewBag.listStudent = listStudent;
            return View("ManageStudent"); 
        }


        [HttpPost]
        public IActionResult EditStudent(int Id, string Code, string Name, DateOnly Dob, string Phone, string Address, int ClassId)
        {
            var student = _contextDAO.Accounts.FirstOrDefault(st => st.Id == Id);
            if (student == null)
            {
                return NotFound(); 
            }
            student.Code = Code;
            student.Name = Name;
            student.Dob = Dob;
            student.Phone = Phone;
            student.Address = Address;

            _contextDAO.SaveChanges(); 

            var studentClass = _contextDAO.StudentClasses.FirstOrDefault(sc => sc.StudentId == Id);

            if (studentClass != null)
            {
                studentClass.ClassId = ClassId;
            }
            else
            {
                studentClass = new StudentClass
                {
                    StudentId = Id,
                    ClassId = ClassId
                };
                _contextDAO.StudentClasses.Add(studentClass);
            }

            _contextDAO.SaveChanges(); 

            return RedirectToAction("ManageStudent");
        }

        public IActionResult RemoveStudent(int accountId)
        {
            try
            {
                var student = _contextDAO.Accounts.FirstOrDefault(st => st.Id == accountId);

                if (student == null)
                {
                    TempData["ErrorMessage"] = "Student not found.";
                    return RedirectToAction("ManageStudent");
                }

                _contextDAO.Accounts.Remove(student);
                _contextDAO.SaveChanges();

                TempData["SuccessMessage"] = "Student removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Delete fail because student is exited in a class ";
            }

            return RedirectToAction("ManageStudent");
        }

        public IActionResult AddStudentInClass()
        {
            var students = _contextDAO.Accounts.Where(ac => ac.Role == 1);
            var Classes = _contextDAO.Classes.ToList();
            ViewBag.Students = students;
            ViewBag.Classes = Classes;
            return View();
        }

        [HttpPost]
        public IActionResult AddStudentInClass(int classId, int studentId)
        {
            var checkClass = _contextDAO.StudentClasses
                .FirstOrDefault(sc => sc.StudentId == studentId && sc.ClassId == classId);

            if (checkClass != null) 
            {
                ViewBag.sExisted = "Student already exists in the class";

                ViewBag.Students = _contextDAO.Accounts.Where(ac => ac.Role == 1).ToList();
                ViewBag.Classes = _contextDAO.Classes.ToList();

                return View();
            }

            StudentClass sc = new StudentClass
            {
                StudentId = studentId,
                ClassId = classId
            };
            var StudentClasses = _contextDAO.StudentClasses.Include(sc => sc.Student).Include(sc => sc.Class)
                .ToList();
            ViewBag.StudentClasses = StudentClasses;
            _contextDAO.StudentClasses.Add(sc);
            _contextDAO.SaveChanges();

            return RedirectToAction("ManageClass");
        }


        public IActionResult ManageClass()
        {
            var StudentClasses = _contextDAO.StudentClasses.Include(sc => sc.Student).Include(sc => sc.Class)
                .ToList();
            ViewBag.StudentClasses = StudentClasses;
            return View();
        }

        public IActionResult SearchClass(string Search)
        {
            var StudentClasses = _contextDAO.StudentClasses.Include(sc => sc.Student).Include(sc => sc.Class)
                .Where(sc => sc.Class.Code.Contains(Search))
                .ToList();
            ViewBag.StudentClasses = StudentClasses;
            return View("ManageClass");
        }

        public IActionResult ManageSubjects()
        {
            var subjects = _contextDAO.Subjects
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Code,
                    s.Category,
                    CategoryName = s.CategoryNavigation != null ? s.CategoryNavigation.Name : "N/A",
                })
                .ToList();

            ViewBag.Subjects = subjects;
            return View();
        }

        public IActionResult AddSubject() 
        {
            var Categories = _contextDAO.Categories.ToList();
            ViewBag.Categories = Categories;
            return View();
        }

        [HttpPost]
        public IActionResult AddSubject(string Name, string Code, int Category)
        {
            Subject su = new Subject
            {
                Name = Name,
                Code = Code,
                Category = Category
            };
            _contextDAO.Subjects.Add(su);
            _contextDAO.SaveChanges();
            var subjects = _contextDAO.Subjects
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Code,
                    s.Category,
                    CategoryName = s.CategoryNavigation != null ? s.CategoryNavigation.Name : "N/A",
                })
                .ToList();

            ViewBag.Subjects = subjects;
            return View("ManageSubjects");
        }

        public IActionResult SearchSubjects(string Search)
        {
            var subjects = _contextDAO.Subjects.Where(su => su.Name.Contains(Search))
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Code,
                    s.Category,
                    CategoryName = s.CategoryNavigation != null ? s.CategoryNavigation.Name : "N/A",
                })
                .ToList();

            ViewBag.Subjects = subjects;
            return View("ManageSubjects");
        }

        public IActionResult EditSubject(int Id)
        {
            var subject = _contextDAO.Subjects.FirstOrDefault(su => su.Id == Id);
            var Categories = _contextDAO.Categories.ToList();
            ViewBag.Categories = Categories;
            ViewBag.subject = subject;
            return View();
        }

        [HttpPost]
        public IActionResult EditSubject(int Id,string Name, string Code, int Category)
        {
            var sub = _contextDAO.Subjects.FirstOrDefault(su => su.Id == Id);
            sub.Name = Name;
            sub.Code = Code;
            sub.Category = Category;
            _contextDAO.SaveChanges();
            return RedirectToAction("ManageSubjects");
        }

        public IActionResult RemoveSubject(int subId)
        {
            try
            {
                var sub = _contextDAO.Subjects.FirstOrDefault(su => su.Id == subId);
                _contextDAO.Subjects.Remove(sub);
                _contextDAO.SaveChanges();
                TempData["SuccessMessage"] = "Student removed successfully.";
                return RedirectToAction("ManageSubjects");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Delete fail because subjects is exited in a class ";
            }

            return RedirectToAction("ManageSubjects");
        }

    }
}
