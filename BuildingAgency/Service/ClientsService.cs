using BuildingAgency.DataAccessLevel;
using BuildingAgency.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.Service
{
    public enum ClientsSort
    {
        FullName,
        PrefType,
        MaxRent
    }
    public enum ClientsSearch
    {
        FullName,
        PrefType,
        PhoneNumber,
        PassportNo
    }

    public class ClientsService
    {
        private readonly ClientsRepository _repository;

        public ClientsService(ClientsRepository repository)
        {
            _repository = repository;
        }

        // GET: Clients
        public async Task<List<Client>> FilteredList(int sortParam = 0, int searchParam = 0, string searchInput = "")
        {
            return await _repository.FilteredList((ClientsSort)sortParam, (ClientsSearch)searchParam, searchInput);
        }
        
        // GET: Clients/Details/5
        public async Task<Client> GetClient(int? id)
        {
            return await _repository.Get(id);
        }

        public async Task<int> Create(Client client)
        {
            return await _repository.Create(client);
        }

        // POST: Clients/Edit/5
        public async Task<int> Edit(int id, Client client)
        {
            return await _repository.Edit(id, client);
        }

        // POST: Clients/Delete/5
        public async Task<int> DeleteConfirmed(int id)
        {
            return await _repository.Delete(id);
        }

        public bool ClientExists(int id)
        {
            return _repository.ClientExists(id);
        }
        public List<string> PropertyTypes()
        {
            return _repository.PropertyTypes();
        }
    }
}
