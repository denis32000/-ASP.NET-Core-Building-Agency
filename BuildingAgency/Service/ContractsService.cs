using BuildingAgency.DataAccessLevel;
using BuildingAgency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.Service
{
    public enum ContractsSort
    {
        ContractId,
        RentStart,
        RentFinish,
        Rent,
        Duration
    }
    public enum ContractsSearch
    {
        ContractId,
        PaymentMethod,
        RentStart,
        PropertyNo,
        ClientPassport
    }

    public class ContractsService
    {
        private readonly ContractsRepository _repository;

        public ContractsService(ContractsRepository repository)
        {
            _repository = repository;
        }

        // GET: Contracts
        public async Task<List<Contract>> FilteredList(int sortParam = 0, int searchParam = 0, string searchInput = "")
        {
            return await _repository.FilteredList((ContractsSort)sortParam, (ContractsSearch)searchParam, searchInput);
        }
        
        // GET: Contracts/Details/5
        public async Task<Contract> GetContract(int? id)
        {
            return await _repository.Get(id);
        }

        public async Task<int> Create(Contract client)
        {
            return await _repository.Create(client);
        }

        // POST: Contracts/Edit/5
        public async Task<int> Edit(int id, Contract client)
        {
            return await _repository.Edit(id, client);
        }

        // POST: Contracts/Delete/5
        public async Task<int> DeleteConfirmed(int id)
        {
            return await _repository.Delete(id);
        }

        public bool ContractExists(int id)
        {
            return _repository.ContractExists(id);
        }

        public Task<List<Contract>> UserContracts(string passport)
        {
            return _repository.UserContracts(passport);
        }
        public async Task<User> GetUserFromEmail(string email)
        {
            return await _repository.GetUserFromEmail(email);
        }

        public bool ClientWithPassportExists(string userPassport)
        {
            return _repository.ClientWithPassportExists(userPassport);
        }

        public List<PropertyForRent> GetRentableProperties()
        {
            return _repository.GetRentableProperties();
        }

        public List<Client> Clients()
        {
            return _repository.Clients();
        }

        public List<PropertyForRent> Properties()
        {
            return _repository.Properties();
        }
        public List<string> GetPaymentMethods()
        {
            return _repository.GetPaymentMethods();
        }
    }
}
