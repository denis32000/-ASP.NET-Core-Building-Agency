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
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly ClientsService _service;

        public ClientsController(ClientsService service)
        {
            _service = service;
        }

        // GET: Clients
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            // Сортировка по (default:FullName), PrefType, MaxRent
            var list = await _service.FilteredList();

            ViewData["SortParams"] = 
                new List<string> { "Full Name", "Preferable Type", "Max Rent" };

            ViewData["SearchParams"] =
                new List<string> { "Full Name", "Preferable Type", "Phone Number", "Passport No" };
            
            return View(list);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ApplyFilters(int sortParam, int searchParam, string searchInput)
        {
            var list = await _service.FilteredList(sortParam, searchParam, searchInput);

            return View("FilteredList", list);
        }
        
        // GET: Clients/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _service.GetClient(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["PrefType"] = new SelectList(_service.PropertyTypes());
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("ClientId,ClientPassportNo,FullName,PhoneNumber,PrefType,MaxRent")] Client client)
        {
            if (ModelState.IsValid)
            {
                int result = await _service.Create(client);
                
                if (result == 0)
                {
                    ViewData["BackController"] = "Clients";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Form data has been lost. Database refused your request. Contact sysadmin.";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            ViewData["PrefType"] = new SelectList(_service.PropertyTypes());
            return View(client);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewClient([Bind("ClientId,ClientPassportNo,FullName,PhoneNumber,PrefType,MaxRent")] Client client)
        {
            if (ModelState.IsValid)
            {
                int result = await _service.Create(client);
                
                return RedirectToAction("Index", "Home");
            }
            return NotFound();
        }

        // GET: Clients/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _service.GetClient(id);

            if (client == null)
            {
                return NotFound();
            }
            ViewData["PrefType"] = new SelectList(_service.PropertyTypes());
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,ClientPassportNo,FullName,PhoneNumber,PrefType,MaxRent")] Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                int result = await _service.Edit(id, client);
                
                if (result == 0)
                {
                    ViewData["BackController"] = "Clients";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Entity which you tried to modify doesn't exist anymore";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            ViewData["PrefType"] = new SelectList(_service.PropertyTypes());
            return View(client);
        }

        // GET: Clients/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _service.GetClient(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int result = await _service.DeleteConfirmed(id);

            if (result == 0)
            {
                ViewData["BackController"] = "Clients";
                ViewData["BackAction"] = "Index";
                ViewData["ErrorMessage"] = "Database refused your request. Contact sysadmin.";
                return View("DbUpdateError");
            }

            return RedirectToAction("Index");
        }
        
    }
}
