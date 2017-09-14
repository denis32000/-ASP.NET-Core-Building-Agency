using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.Controllers
{
    [Authorize(Roles = "admin")]
    public class StaffsController : Controller
    {
        private readonly StaffsService _service;

        public StaffsController(StaffsService service)
        {
            _service = service;
        }

        // GET: Staffs
        public async Task<IActionResult> Index()
        {
            // Сортировка по (default:FullName), PrefType, MaxRent
            var list = await _service.FilteredList();

            ViewData["SortParams"] =
                new List<string> { "Full Name", "Position", "Date Of Birth", "Salary" };

            ViewData["SearchParams"] =
                new List<string> { "Full Name", "Position", "Date Of Birth", "Superviser Passport", "Passport No" };
            
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ApplyFilters(int sortParam, int searchParam, string searchInput)
        {
            var list = await _service.FilteredList(sortParam, searchParam, searchInput);

            return View("FilteredList", list);
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _service.GetStaff(id);

            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            ViewData["SuperviserId"] = new SelectList(_service.Staffs(), "StaffId", "FullName");
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffId,StaffPassportNo,FullName,Position,Sex,DateOfBirth,Salary,SuperviserId")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                int result = await _service.Create(staff);

                if (result == 0)
                {
                    ViewData["BackController"] = "Staffs";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Form data has been lost. Database refused your request. Contact sysadmin.";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            ViewData["SuperviserId"] = new SelectList(_service.Staffs(), "StaffId", "FullName", staff.SuperviserId);
            return View(staff);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _service.GetStaff(id);

            if (staff == null)
            {
                return NotFound();
            }
            ViewData["SuperviserId"] = new SelectList(_service.Staffs(), "StaffId", "FullName", staff.SuperviserId);
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StaffId,StaffPassportNo,FullName,Position,Sex,DateOfBirth,Salary,SuperviserId")] Staff staff)
        {
            if (id != staff.StaffId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                int result = await _service.Edit(id, staff);

                if (result == 0)
                {
                    ViewData["BackController"] = "Staffs";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Entity which you tried to modify doesn't exist anymore";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            ViewData["SuperviserId"] = new SelectList(_service.Staffs(), "StaffId", "FullName", staff.SuperviserId);
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _service.GetStaff(id);

            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int result = await _service.DeleteConfirmed(id);

            if (result == 0)
            {
                ViewData["BackController"] = "Staffs";
                ViewData["BackAction"] = "Index";
                ViewData["ErrorMessage"] = "Database refused your request. Try one more time or contact sysadmin.";
                return View("DbUpdateError");
            }

            return RedirectToAction("Index");
        }

        private bool StaffExists(int id)
        {
            return _service.StaffExists(id);
        }
    }
}
