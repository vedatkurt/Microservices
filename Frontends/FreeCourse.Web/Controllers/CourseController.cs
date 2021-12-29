using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public CourseController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService)
        {
            _sharedIdentityService = sharedIdentityService;
            _catalogService = catalogService;
        } 

        public async Task<IActionResult> Index()
        {
            //return View(await _catalogService.GetAllCourseAsync .GetAllCourseByUserIdAsync(_sharedIdentityService.GetUserId));
            return View(await _catalogService.GetAllCourseAsync());
        }

        public async Task<IActionResult> Create() 
        {
            var categories = await _catalogService.GetAllCategoryAsync();

            ViewBag.categoryList = new SelectList(categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateInput courseCreateInput)
        {
            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.categoryList = new SelectList(categories,"Id", "Name");

            if (!ModelState.IsValid)
            {
                return View();
            }

            courseCreateInput.UserId = _sharedIdentityService.GetUserId;

            var result = await _catalogService.CreateCourseAsync(courseCreateInput);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(string Id)
        {
            var course = await _catalogService.GetByCourseId(Id);
            var categories = await _catalogService.GetAllCategoryAsync();

            if (course == null)
            {
                RedirectToAction(nameof(Index));
            }

            ViewBag.categoryList = new SelectList(categories, "Id", "Name");
            CourseUpdateInput courseUpdate = new CourseUpdateInput()
            {
                Id = course.Id,
                Name = course.Name,
                Price = course.Price,
                Description= course.Description,
                Feature = new FeatureViewModel { Duration = course.Feature.Duration },
                CategoryId = course.CategoryId,
                UserId = course.UserId,
                Picture = course.Picture                    
            };

            return View(courseUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CourseUpdateInput courseUpdateInput)
        {
            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.categoryList = new SelectList(categories, "Id", "Name", courseUpdateInput.Id);

            if (!ModelState.IsValid)
            {
                return View();
            };

            await _catalogService.UpdateCourseAsync(courseUpdateInput);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string Id)
        {
            await _catalogService.DeleteCourseAsync(Id);

            return RedirectToAction(nameof(Index));
        }
    }
}
