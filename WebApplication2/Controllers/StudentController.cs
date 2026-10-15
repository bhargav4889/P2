using Microsoft.AspNetCore.Mvc;
using WebApplication2.Filters;
using WebApplication2.Interfaces;
using WebApplication2.Models;
using WebApplication2.Services;
using WebApplication2.ViewModels.Student;

namespace WebApplication2.Controllers
{
    [SessionAuthorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IClassService _classService;

        public StudentController(IStudentService studentService, IClassService classService)
        {
            _studentService = studentService;
            _classService = classService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var studentsResponse = await _studentService.GetStudents();

            var classesResponse = await _classService.GetClasses();

            var model = new StudentPageViewModel
            {
                Student = new Students(),
                Students = studentsResponse.Data as List<Students> ?? new List<Students>(),
                Classes = classesResponse.Data as List<TblClass> ?? new List<TblClass>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(StudentPageViewModel model)
        {
            bool isInsert = model.Student.Id == 0;

            var result = await _studentService
                .InsertOrUpdateStudent(model.Student);

            if (result.Success)
            {
                if (result.Data is Students student)
                {
                    if (isInsert)
                    {
                        // Insert: Add new student to list
                        model.Students.Add(student);
                    }
                    else
                    {
                        // Update: Replace existing student in list
                        var index = model.Students.FindIndex(
                            x => x.Id == student.Id);

                        if (index >= 0)
                        {
                            model.Students[index] = student;
                        }
                    }
                }

                TempData["Message"] = result.Message;

                ModelState.Clear();
                model.Student = new Students();
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = result.Message;
            // Ensure a value is always returned. Optionally add an error/message here
            // e.g. ModelState.AddModelError("", result.Message);
            return View(model);

        }

        // Load selected student into the same Index page
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _studentService.GetStudentById(id);

            if (!response.Success || response.Data is not Students student)
            {
                TempData["Message"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var studentsResponse = await _studentService.GetStudents();

            var classesResponse = await _classService.GetClasses();

            var model = new StudentPageViewModel
            {
                Student = student,
                Students = studentsResponse.Data as List<Students>
                           ?? new List<Students>(),
                Classes = classesResponse.Data as List<TblClass> ?? new List<TblClass>()
            };

            return View("Index", model);
        }

        // Delete selected student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _studentService.DeleteStudent(id);

            TempData["Message"] = response.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
