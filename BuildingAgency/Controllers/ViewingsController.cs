using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.Controllers
{
    [Authorize]
    public class ViewingsController : Controller
    {
        private readonly ViewingsService _service;

        public ViewingsController(ViewingsService service)
        {
            _service = service;
        }

        // GET: Viewings
        public async Task<IActionResult> Index()
        {
            // Сортировка по (default:FullName), PrefType, MaxRent
            var list = await _service.FilteredList();
            
            ViewData["SortParams"] =
                new List<string> { "View No", "View Date" };

            ViewData["SearchParams"] =
                new List<string> { "View No", "Client's Passport", "Property No", "View Date", "Comment" };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ApplyFilters(int sortParam, int searchParam, string searchInput)
        {
            var list = await _service.FilteredList(sortParam, searchParam, searchInput);

            return View("FilteredList", list);
        }

        [HttpGet]
        public IActionResult ShowComments(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var list = _service.GetPropertyComments(id);

            //.Where(x => x.PropertyId != x.Contract);//.Include(p => p.OverseesBy).Include(p => p.Owner);

            return View(list);
        }

        // GET: Viewings/LeaveComment/contractId
        [HttpGet]
        public IActionResult LeaveComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            Contract contract = _service.GetContractById((int)id);

            ViewData["ClientId"] = contract.ClientId;
            ViewData["PropertyId"] = contract.PropertyId;
            ViewData["ContractId"] = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([Bind("ViewNo,ClientId,PropertyId,ViewDate,Comment")] Viewing viewing)
        {
            if (ModelState.IsValid)
            {
                int result = await _service.Create(viewing);

                if (result == 0)
                {
                    ViewData["BackController"] = "Home";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Database refused your request.. Please, try later";
                    return View("DbUpdateError");
                }

                return RedirectToAction("UserContracts", "Contracts");
            }
            ViewData["ClientId"] = viewing.ClientId;
            ViewData["PropertyId"] = viewing.PropertyId;
            return View(viewing);
        }

        // GET: Viewings/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewing = await _service.GetViewing(id);

            if (viewing == null)
            {
                return NotFound();
            }

            return View(viewing);
        }

        // GET: Viewings/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo");
            ViewData["PropertyId"] = new SelectList(_service.Properties(), "PropertyId", "PropertyNo");
            return View();
        }

        // POST: Viewings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("ViewNo,ClientId,PropertyId,ViewDate,Comment")] Viewing viewing)
        {
            if (ModelState.IsValid)
            {
                int result = await _service.Create(viewing);

                if (result == 0)
                {
                    ViewData["BackController"] = "Viewings";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Form data has been lost. Database refused your request. Contact sysadmin.";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            ViewData["ClientId"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo", viewing.ClientId);
            ViewData["PropertyId"] = new SelectList(_service.Properties(), "PropertyId", "PropertyNo", viewing.PropertyId);
            return View(viewing);
        }

        // GET: Viewings/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewing = await _service.GetViewing(id);

            if (viewing == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo", viewing.ClientId);
            ViewData["PropertyId"] = new SelectList(_service.Properties(), "PropertyId", "PropertyNo", viewing.PropertyId);
            return View(viewing);
        }

        // POST: Viewings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ViewNo,ClientId,PropertyId,ViewDate,Comment")] Viewing viewing)
        {
            if (id != viewing.ViewNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                int result = await _service.Edit(id, viewing);

                if (result == 0)
                {
                    ViewData["BackController"] = "Viewings";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Entity which you tried to modify doesn't exist anymore";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            ViewData["ClientId"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo", viewing.ClientId);
            ViewData["PropertyId"] = new SelectList(_service.Properties(), "PropertyId", "PropertyNo", viewing.PropertyId);
            return View(viewing);
        }

        // GET: Viewings/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewing = await _service.GetViewing(id);

            if (viewing == null)
            {
                return NotFound();
            }

            return View(viewing);
        }

        // POST: Viewings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int result = await _service.DeleteConfirmed(id);

            if (result == 0)
            {
                ViewData["BackController"] = "Viewings";
                ViewData["BackAction"] = "Index";
                ViewData["ErrorMessage"] = "Database refused your request. Please, try later.";
                return View("DbUpdateError");
            }

            return RedirectToAction("Index");
        }

        private bool ViewingExists(int id)
        {
            return _service.ViewingExists(id);
        }
    }
}
