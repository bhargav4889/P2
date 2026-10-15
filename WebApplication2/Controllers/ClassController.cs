using Microsoft.AspNetCore.Mvc;
using WebApplication2.Filters;
using WebApplication2.Interfaces;
using WebApplication2.Models;
using WebApplication2.ViewModels.Class;

namespace WebApplication2.Controllers
{
    [SessionAuthorize]
    public class ClassController : Controller
    {
        private readonly IClassService _classMasterService;

        public ClassController(IClassService classMasterService)
        {
            _classMasterService = classMasterService;
        }

        // Display class list
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _classMasterService.GetClasses();

            var model = new ClassPageViewModel
            {
                Class = new TblClass(),
                Classes = response.Data as List<TblClass>
                              ?? new List<TblClass>()
            };

            return View(model);
        }

        // Insert or update class
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ClassPageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var response = await _classMasterService.GetClasses();

                model.Classes = response.Data as List<TblClass>
                                     ?? new List<TblClass>();

                return View(model);
            }

            var result = await _classMasterService
                .InsertOrUpdateClass(model.Class);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message ?? "Unable to save class.");

                var response = await _classMasterService.GetClasses();

                model.Classes = response.Data as List<TblClass>
                                     ?? new List<TblClass>();

                return View(model);
            }

            TempData["Message"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        // Load class for editing
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _classMasterService.GetClassById(id);

            if (!response.Success || response.Data is not TblClass classMaster)
            {
                TempData["Message"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            var listResponse = await _classMasterService.GetClasses();

            var model = new ClassPageViewModel
            {
                Class = classMaster,
                Classes = listResponse.Data as List<TblClass>
                              ?? new List<TblClass>()
            };

            return View("Index", model);
        }

        // Delete class
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _classMasterService.DeleteClass(id);

            TempData["Message"] = response.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
