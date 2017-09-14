using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildingAgency.Controllers
{
    [Authorize]
    public class PropertyForRentsController : Controller
    {
        private readonly PropertyForRentsService _service;

        public PropertyForRentsController(PropertyForRentsService service)
        {
            _service = service;
        }

        // GET: PropertyForRents
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            // Сортировка по (default:FullName), PrefType, MaxRent
            var list = await _service.FilteredList();
            
            ViewData["SortParams"] =
                new List<string> { "Property No", "City", "Type", "Rooms", "Rent" };

            ViewData["SearchParams"] =
                new List<string> { "Property No", "City", "Type", "Post Code", "Owner's Passport", "Staff's Passport" };

            return View(list);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ApplyFilters(int sortParam, int searchParam, string searchInput)
        {
            var list = await _service.FilteredList(sortParam, searchParam, searchInput);

            return View("FilteredList", list);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ExtendedSearch(int sortParam, 
            string propertyNo, string city, string type, 
            string postcode, string ownerPassport, string staffPassport)
        {
            var list = await _service.ExtendedSearch(sortParam, propertyNo, city, type, postcode, ownerPassport, staffPassport);

            return View("FilteredList", list);
        }
        
        public async Task<IActionResult> ExtendedSearch2(int sortParam,
            string propertyNo, string city, string type,
            string postcode, string ownerPassport, string staffPassport)
        {
            var list = await _service.ExtendedSearch(sortParam, propertyNo, city, type, postcode, ownerPassport, staffPassport);

            return View("FilteredList2", list);
        }

        // GET: PropertyForRents/ValidPropertiesList
        public async Task<IActionResult> ValidPropertiesList()
        {
            User myUser = await _service.GetUserFromEmail(User.Identity.Name);

            if (myUser == null)
            {
                return NotFound();
            }

            // If client with such passport doesnt exist
            //if (_service.ClientWithPassportExists(myUser.Passport) == false)
            //{
            //    return RedirectToAction("AddClientInfo", "Account",
            //        new { userPassport = myUser.Passport, modelError = "You have to register yourself as a client before accessing contract conclusions!" });
            //}

            ViewData["SortParams"] =
                new List<string> { "Property No", "City", "Type", "Rooms", "Rent" };

            ViewData["SearchParams"] =
                new List<string> { "Property No", "City", "Type", "Post Code", "Owner's Passport", "Staff's Passport" };

            var list = _service.GetRentableProperties();

            //.Where(x => x.PropertyId != x.Contract);//.Include(p => p.OverseesBy).Include(p => p.Owner);

            return View(list);
        }

        public async Task<IActionResult> UserProperties()
        {
            User myUser = await _service.GetUserFromEmail(User.Identity.Name);

            if (myUser == null)
                return NotFound();

            // If client with such passport doesnt exist
            //if (_service.ClientWithPassportExists(myUser.Passport) == false)
            //    return RedirectToAction("AddClientInfo", "Account",
            //        new { userPassport = myUser.Passport, modelError = "You have to register yourself as a client before accessing contracts list!" });

            var list = await _service.UserProperties(myUser.Passport);

            return View(list);
        }

        [Authorize(Roles = "admin")]
        // GET: PropertyForRents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyForRent = await _service.GetPropertyForRent(id);

            if (propertyForRent == null)
            {
                return NotFound();
            }

            return View(propertyForRent);
        }

        // GET: PropertyForRents/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["OverseesById"] = new SelectList(_service.Staffs(), "StaffId", "FullName");
            ViewData["OwnerId"] = new SelectList(_service.PrivateOwner(), "OwnerId", "FullName");
            return View();
        }

        // POST: PropertyForRents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("PropertyId,PropertyNo,City,Street,PostCode,Type,Rooms,Rent,OwnerId,OverseesById")] PropertyForRent propertyForRent)
        {
            if (ModelState.IsValid)
            {
                int result = await _service.Create(propertyForRent);

                if (result == 0)
                {
                    ViewData["BackController"] = "PropertyForRents";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Form data has been lost. Database refused your request. Contact sysadmin.";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            ViewData["OverseesById"] = new SelectList(_service.Staffs(), "StaffId", "FullName", propertyForRent.OverseesById);
            ViewData["OwnerId"] = new SelectList(_service.PrivateOwner(), "OwnerId", "FullName", propertyForRent.OwnerId);
            return View(propertyForRent);
        }

        // GET: PropertyForRents/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyForRent = await _service.GetPropertyForRent(id);

            if (propertyForRent == null)
            {
                return NotFound();
            }
            ViewData["OverseesById"] = new SelectList(_service.Staffs(), "StaffId", "FullName", propertyForRent.OverseesById);
            ViewData["OwnerId"] = new SelectList(_service.PrivateOwner(), "OwnerId", "FullName", propertyForRent.OwnerId);
            return View(propertyForRent);
        }

        // POST: PropertyForRents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyId,PropertyNo,City,Street,PostCode,Type,Rooms,Rent,OwnerId,OverseesById")] PropertyForRent propertyForRent)
        {
            if (id != propertyForRent.PropertyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                int result = await _service.Edit(id, propertyForRent);

                if (result == 0)
                {
                    ViewData["BackController"] = "PropertyForRents";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Entity which you tried to modify doesn't exist anymore";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            ViewData["OverseesById"] = new SelectList(_service.Staffs(), "StaffId", "FullName", propertyForRent.OverseesById);
            ViewData["OwnerId"] = new SelectList(_service.PrivateOwner(), "OwnerId", "FullName", propertyForRent.OwnerId);
            return View(propertyForRent);
        }

        // GET: PropertyForRents/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyForRent = await _service.GetPropertyForRent(id);

            if (propertyForRent == null)
            {
                return NotFound();
            }

            return View(propertyForRent);
        }

        // POST: PropertyForRents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int result = await _service.DeleteConfirmed(id);

            if (result == 0)
            {
                ViewData["BackController"] = "PropertyForRents";
                ViewData["BackAction"] = "Index";
                ViewData["ErrorMessage"] = "Database refused your request. Contact sysadmin.";
                return View("DbUpdateError");
            }

            return RedirectToAction("Index");
        }

        private bool PropertyForRentExists(int id)
        {
            return _service.PropertyForRentExists(id);
        }
    }
}
