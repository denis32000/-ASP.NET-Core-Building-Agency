using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.DataAccessLevel
{
    public class ContractsRepository
    {
        private readonly BuildingAgencyContext _context;

        public ContractsRepository(BuildingAgencyContext context)
        {
            _context = context;
        }

        public async Task<List<Contract>> FilteredList(ContractsSort sortParam, ContractsSearch searchParam, string searchInput)
        {
            var _contractsContext = _context.Contract
                .Include(c => c.Client)
                .Include(c => c.Property)
                .OrderBy(c => c.ContractId);

            var list = await _contractsContext.ToListAsync();

            if (searchInput != null && searchInput != String.Empty)
            {
                switch (searchParam)
                {
                    case ContractsSearch.ContractId: list = list.FindAll(v => v.ContractId.ToString() == searchInput); break;
                    case ContractsSearch.PaymentMethod: list = list.FindAll(v => v.PaymentMethod.Contains(searchInput)); break;
                    case ContractsSearch.RentStart: list = list.FindAll(v => v.RentStart.ToString().Contains(searchInput)); break;
                    case ContractsSearch.PropertyNo: list = list.FindAll(v => v.Property.PropertyNo.ToString() == searchInput); break;
                    case ContractsSearch.ClientPassport: list = list.FindAll(v => v.Client.ClientPassportNo.Contains(searchInput)); break;
                }
            }

            switch (sortParam)
            {
                case ContractsSort.ContractId: return list.OrderBy(v => v.ContractId).ToList();
                case ContractsSort.Duration: return list.OrderBy(v => v.Duration).ToList();
                case ContractsSort.Rent: return list.OrderBy(v => v.Rent).ToList();
                case ContractsSort.RentStart: return list.OrderBy(v => v.RentStart).ToList();
                case ContractsSort.RentFinish: return list.OrderBy(v => v.RentFinish).ToList();
                default: return list.OrderBy(v => v.ContractId).ToList();
            }
        }

        // GET: Contracts/Details/5
        public async Task<Contract> Get(int? id)
        {
            var contract = await _context.Contract
                .Include(c => c.Client)
                .Include(c => c.Property)
                .SingleOrDefaultAsync(m => m.ContractId == id);

            return contract;
        }

        public async Task<int> Create(Contract contract)
        {
            _context.Add(contract);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> Edit(int id, Contract contract)
        {
            try
            {
                _context.Update(contract);
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(id))
                {
                    return 0;
                }
                else
                {
                    throw;
                }
            }
            //return RedirectToAction("Index");
        }

        public async Task<int> Delete(int id)
        {
            var contract = await _context.Contract.SingleOrDefaultAsync(m => m.ContractId == id);
            _context.Contract.Remove(contract);
            return await _context.SaveChangesAsync();
        }

        public bool ContractExists(int id)
        {
            return _context.Contract.Any(e => e.ContractId == id);
        }

        public async Task<List<Contract>> UserContracts(string passport)
        {
            var buildingAgencyContext = _context.Contract
                .Include(c => c.Property)
                .Include(c => c.Client)
                .Include(c => c.Client.Viewing)
                .Where(c => c.Client.ClientPassportNo == passport);

            var list = await buildingAgencyContext.ToListAsync();

            return list;
        }

        public async Task<User> GetUserFromEmail(string email)
        {
            var usersContext = _context.User.Include(u => u.Client);
            List<User> myList = await usersContext.ToListAsync();
            return myList.Find(x => x.Email == email);
        }

        public bool ClientWithPassportExists(string userPassport)
        {
            return _context.Client.Any(x => x.ClientPassportNo == userPassport);
        }

        public List<Client> Clients()
        {
            return _context.Client.ToList();
        }

        public List<PropertyForRent> Properties()
        {
            return _context.PropertyForRent
                .Include(p => p.Contract)
                .ToList();
        }

        public List<PropertyForRent> GetRentableProperties()
        {
            var list = _context.PropertyForRent
                    .Include(p => p.OverseesBy)
                    .Include(p => p.Owner)
                    .Include(p => p.Contract)
                    .Where(p => p.Contract.All(x => x.RentFinish.CompareTo(DateTime.Now) <= 0))
                    .ToList();

            return list;
        }

        public List<string> GetPaymentMethods()
        {
            return _context.Contract
                .Select(c => c.PaymentMethod)
                .Distinct()
                .ToList();
        }

        //public bool ValidDate(DateTime startDate, DateTime endDate)
        //{
        //    return startDate.CompareTo(DateTime.Now.Date) > 0 && endDate.CompareTo(startDate) > 0;
        //}
    }
}
