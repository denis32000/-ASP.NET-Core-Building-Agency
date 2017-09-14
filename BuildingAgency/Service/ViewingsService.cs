using System;
using BuildingAgency.DataAccessLevel;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingAgency.Models;

namespace BuildingAgency.Service
{
    public enum ViewingsSort
    {
        ViewNo,
        ViewDate
    }

    public enum ViewingsSearch
    {
        ViewNo,
        ClientPassport,
        PropertyNo,
        ViewDate,
        Comment
    }

    public class ViewingsService
    {
        private readonly ViewingsRepository _repository;

        public ViewingsService(ViewingsRepository repository)
        {
            _repository = repository;
        }

        // GET: Viewings
        public async Task<List<Viewing>> FilteredList(int sortParam = 0, int searchParam = 0, string searchInput = "")
        {
            return await _repository.FilteredList((ViewingsSort)sortParam, (ViewingsSearch)searchParam, searchInput);
        }
        
        // GET: Viewings/Details/5
        public async Task<Viewing> GetViewing(int? id)
        {
            return await _repository.Get(id);
        }

        public async Task<int> Create(Viewing viewing)
        {
            return await _repository.Create(viewing);
        }

        // POST: Viewings/Edit/5
        public async Task<int> Edit(int id, Viewing viewing)
        {
            return await _repository.Edit(id, viewing);
        }

        // POST: Viewings/Delete/5
        public async Task<int> DeleteConfirmed(int id)
        {
            return await _repository.Delete(id);
        }

        public bool ViewingExists(int id)
        {
            return _repository.ViewingExists(id);
        }
        
        public List<Client> Clients()
        {
            return _repository.Clients();
        }

        public List<PropertyForRent> Properties()
        {
            return _repository.Properties();
        }
        public Contract GetContractById(int id)
        {
            return _repository.GetContractById(id);
        }
        public List<Viewing> GetPropertyComments(int? id)
        {
            int notNull = (int)id;
            return _repository.GetPropertyComments(notNull);
        }
    }
}
