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
    public class ContractsController : Controller
    {
        private readonly ContractsService _service;

        public ContractsController(ContractsService service)
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
                new List<string> { "Contract No", "Rent Start", "Rent Finish", "Rent Cost", "Duration" };

            ViewData["SearchParams"] =
                new List<string> { "Contract No", "Payment Method", "Rent Start", "Property No", "Client Passport" };

            return View(list);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ApplyFilters(int sortParam, int searchParam, string searchInput)
        {
            var list = await _service.FilteredList(sortParam, searchParam, searchInput);

            return View("FilteredList", list);
        }

        public async Task<IActionResult> UserContracts()
        {
            User myUser = await _service.GetUserFromEmail(User.Identity.Name);

            if (myUser == null)
                return NotFound();

            // If client with such passport doesnt exist
            if (_service.ClientWithPassportExists(myUser.Passport) == false)
                return RedirectToAction("AddClientInfo", "Account",
                    new { userPassport = myUser.Passport, modelError = "You have to register yourself as a client before accessing contracts list!" });

            var list = await _service.UserContracts(myUser.Passport);

            List<int> commentedContractsList = new List<int>();
            List<int> contracts = list.FindAll(c => c.Client.Viewing.Any(v => v.PropertyId == c.PropertyId)).Select(s => s.ContractId).ToList();

            if (contracts.Count > 0) commentedContractsList = contracts;

            ViewData["CommentedContractsList"] = commentedContractsList;
            
            return View(list);
        }

        // GET: ConcludeContract/id
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ConcludeContract(int? id)
        {
            if (id == null)
                return NotFound();

            // Если эта проперти уже не доступна
            if (_service.GetRentableProperties().FirstOrDefault(p => p.PropertyId == id) == null)
            {
                ViewData["BackController"] = "Contracts";
                ViewData["BackAction"] = "Index";
                ViewData["ErrorMessage"] = "This property already has a contract.";
                return View("DbUpdateError");
            }

            /* IMPORTANT! */
            //var buildingAgencyContext = _context.PropertyForRent
            //    .Include(c => c.Contract)
            //    .SingleOrDefaultAsync(m => m.PropertyId == id);

            PropertyForRent property = _service.Properties().FirstOrDefault(pfr => pfr.PropertyId == id);

            if (property == null)
                return NotFound();

            User myUser = await _service.GetUserFromEmail(User.Identity.Name);

            if (myUser == null)
                return NotFound();

            Client myClient = _service.Clients().FirstOrDefault(x => x.ClientPassportNo == myUser.Passport);

            if (myClient == null)
                return NotFound();

            //ViewData["clientPassport"] = myUser.Passport;
            ViewData["AllClients"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo");
            ViewData["propertyNo"] = property.PropertyNo;

            ViewData["PaymentMeth"] = new SelectList(_service.GetPaymentMethods());

            var contract = new Contract
            {
                PropertyId = property.PropertyId,
                ClientId = myClient.ClientId,
                Rent = property.Rent,
                Paid = false
            };

            return View(contract);
        }
        
        // POST: ConcludeContract/id
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ConcludeContract(int id, [Bind("ContractId,PropertyId,PaymentMethod,Paid,RentStart,RentFinish,Rent,Deposit,ClientId,Duration")] Contract contract)
        {
            if (ModelState.IsValid
                && contract.RentStart.CompareTo(DateTime.Now.Date) >= 0
                && contract.RentFinish.CompareTo(DateTime.Now.Date) > 0)
            {
                // Если эта проперти уже не доступна
                if (_service.GetRentableProperties().FirstOrDefault(p => p.PropertyId == contract.PropertyId) == null)
                {
                    ViewData["BackController"] = "Contracts";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "This property already has a contract.";
                    return View("DbUpdateError");
                }

                int result = await _service.Create(contract);

                if (result == 0)
                {
                    ViewData["BackController"] = "Contracts";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Form data has been lost. Database refused your request. Contact sysadmin.";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }

            if (!(contract.RentStart.CompareTo(DateTime.Now.Date) >= 0))
            {
                ModelState.AddModelError("RentStart", "Дата начала аренды не должна быть раньше сегодняшнего числа.");
            }
            if (!(contract.RentFinish.CompareTo(DateTime.Now.Date) > 0))
            {
                ModelState.AddModelError("RentFinish", "Дата окончания аренды должна быть позже сегодняшнего числа.");
            }

            PropertyForRent property = _service.Properties().FirstOrDefault(pfr => pfr.PropertyId == id);

            if (property == null)
                return NotFound();

            User myUser = await _service.GetUserFromEmail(User.Identity.Name);

            if (myUser == null)
                return NotFound();

            Client myClient = _service.Clients().FirstOrDefault(x => x.ClientPassportNo == myUser.Passport);

            if (myClient == null)
                return NotFound();

            ViewData["AllClients"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo");
            ViewData["propertyNo"] = property.PropertyNo;
            ViewData["PaymentMeth"] = new SelectList(_service.GetPaymentMethods());

            return View(contract);
        }

        // GET: Contracts/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _service.GetContract(id);

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        // GET: Contracts/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo");
            ViewData["PropertyId"] = new SelectList(_service.GetRentableProperties(), "PropertyId", "PropertyNo");
            ViewData["PaymentMeth"] = new SelectList(_service.GetPaymentMethods());
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,PropertyId,PaymentMethod,Paid,RentStart,RentFinish,Rent,Deposit,ClientId,Duration")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                // Если эта проперти уже не доступна
                if (_service.GetRentableProperties().FirstOrDefault(p => p.PropertyId == contract.PropertyId) == null)
                {
                    ViewData["BackController"] = "Contracts";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "This property already has a contract.";
                    return View("DbUpdateError");
                }

                int result = await _service.Create(contract);

                if (result == 0)
                {
                    ViewData["BackController"] = "Contracts";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Form data has been lost. Database refused your request. Contact sysadmin.";
                    return View("DbUpdateError");
                }

                return RedirectToAction("Index");
            }
            ViewData["ClientId"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo", contract.ClientId);
            ViewData["PropertyId"] = new SelectList(_service.Properties(), "PropertyId", "PropertyNo", contract.PropertyId);
            ViewData["PaymentMeth"] = new SelectList(_service.GetPaymentMethods());
            //ViewData["PropertyId"] = new SelectList(_context.PropertyForRent, "PropertyId", "City", contract.PropertyId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _service.GetContract(id);
            if (contract == null)
                return NotFound();
            ViewData["ClientId"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo", contract.ClientId);
            ViewData["PropertyId"] = new SelectList(_service.Properties(), "PropertyId", "PropertyNo", contract.PropertyId);
            //ViewData["PaymentMeth"] = new SelectList(_service.GetPaymentMethods());
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,PropertyId,PaymentMethod,Paid,RentStart,RentFinish,Rent,Deposit,ClientId,Duration")] Contract contract)
        {
            if (id != contract.ContractId)
                return NotFound();

            if (ModelState.IsValid)
            {
                int result = await _service.Edit(id, contract);

                if (result == 0)
                {
                    ViewData["BackController"] = "Contracts";
                    ViewData["BackAction"] = "Index";
                    ViewData["ErrorMessage"] = "Entity which you tried to modify doesn't exist anymore";
                    return View("DbUpdateError");
                }
                return RedirectToAction("Index");
            }
            ViewData["ClientId"] = new SelectList(_service.Clients(), "ClientId", "ClientPassportNo", contract.ClientId);
            ViewData["PropertyId"] = new SelectList(_service.Properties(), "PropertyId", "PropertyNo", contract.PropertyId);
            //ViewData["PaymentMeth"] = new SelectList(_service.GetPaymentMethods());
            return View(contract);
        }

        // GET: Contracts/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _service.GetContract(id);

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int result = await _service.DeleteConfirmed(id);

            if (result == 0)
            {
                ViewData["BackController"] = "Contracts";
                ViewData["BackAction"] = "Index";
                ViewData["ErrorMessage"] = "Database refused your request. Contact sysadmin.";
                return View("DbUpdateError");
            }

            return RedirectToAction("Index");
        }

        private bool ContractExists(int id)
        {
            return _service.ContractExists(id);
        }
    }
}
