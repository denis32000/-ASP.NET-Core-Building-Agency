using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.Controllers
{
    [Authorize(Roles = "admin")]
    public class PrivateOwnersController : Controller
    {
        private readonly PrivateOwnersService _service;

        public PrivateOwnersController(PrivateOwnersService service)
        {
            _service = service;
        }

        // GET: PrivateOwners
        public async Task<IActionResult> Index()
        {
            // Сортировка по (default:FullName), PrefType, MaxRent
            var list = await _service.FilteredList();

            ViewData["SortParams"] =
                new List<string> { "Full Name", "City", "Passport No" };

            ViewData["SearchParams"] =
                new List<string> { "Full Name", "Phone Number", "City", "Passport No" };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ApplyFilters(int sortParam, int searchParam, string searchInput)
        {
            var list = await _service.FilteredList(sortParam, searchParam, searchInput);

            return View("FilteredList", list);
        }

        // GET: PrivateOwners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privateOwner = await _service.GetPrivateOwner(id);

            if (privateOwner == null)
            {
                return NotFound();
            }

            return View(privateOwner);
        }

        // GET: PrivateOwners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PrivateOwners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OwnerId,OwnerPassportNo,FullName,PhoneNumber,City,Street")] PrivateOwner privateOwner)
        {
            if (ModelState.IsValid)
            {
                int result = await _service.Create(privateOwner);

                if (result == 0)
                {
                    ViewData["BackController"] = "PrivateOwners";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Form data has been lost. Database refused your request. Contact sysadmin.";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            return View(privateOwner);
        }

        // GET: PrivateOwners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privateOwner = await _service.GetPrivateOwner(id);

            if (privateOwner == null)
            {
                return NotFound();
            }
            return View(privateOwner);
        }

        // POST: PrivateOwners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OwnerId,OwnerPassportNo,FullName,PhoneNumber,City,Street")] PrivateOwner privateOwner)
        {
            if (id != privateOwner.OwnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                int result = await _service.Edit(id, privateOwner);

                if (result == 0)
                {
                    ViewData["BackController"] = "PrivateOwners";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Entity which you tried to modify doesn't exist anymore";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            return View(privateOwner);
        }

        // GET: PrivateOwners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privateOwner = await _service.GetPrivateOwner(id);

            if (privateOwner == null)
            {
                return NotFound();
            }

            return View(privateOwner);
        }

        // POST: PrivateOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int result = await _service.DeleteConfirmed(id);

            if (result == 0)
            {
                ViewData["BackController"] = "PrivateOwners";
                ViewData["BackAction"] = "Index";
                ViewData["ErrorMessage"] = "Database refused your request. Contact sysadmin.";
                return View("DbUpdateError");
            }

            return RedirectToAction("Index");
        }
    }
}
