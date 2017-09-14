using BuildingAgency.DataAccessLevel;
using BuildingAgency.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildingAgency.Service
{
    public enum PrivateOwnersSort
    {
        FullName,
        City,
        PassportNo
    }

    public enum PrivateOwnersSearch
    {
        FullName,
        PhoneNumber,
        City,
        PassportNo
    }

    public class PrivateOwnersService
    {
        private readonly PrivateOwnersRepository _repository;

        public PrivateOwnersService(PrivateOwnersRepository repository)
        {
            _repository = repository;
        }

        // GET: PrivateOwners
        public async Task<List<PrivateOwner>> FilteredList(int sortParam = 0, int searchParam = 0, string searchInput = "")
        {
            return await _repository.FilteredList((PrivateOwnersSort)sortParam, (PrivateOwnersSearch)searchParam, searchInput);
        }
        
        // GET: PrivateOwners/Details/5
        public async Task<PrivateOwner> GetPrivateOwner(int? id)
        {
            return await _repository.Get(id);
        }

        public async Task<int> Create(PrivateOwner client)
        {
            return await _repository.Create(client);
        }

        // POST: PrivateOwners/Edit/5
        public async Task<int> Edit(int id, PrivateOwner client)
        {
            return await _repository.Edit(id, client);
        }

        // POST: PrivateOwners/Delete/5
        public async Task<int> DeleteConfirmed(int id)
        {
            return await _repository.Delete(id);
        }

        public bool PrivateOwnerExists(int id)
        {
            return _repository.PrivateOwnerExists(id);
        }
    }
}
