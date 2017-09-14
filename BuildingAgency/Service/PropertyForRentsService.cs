using System;
using BuildingAgency.DataAccessLevel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using BuildingAgency.Models;

namespace BuildingAgency.Service
{
    public enum PropertyForRentsSort
    {
        PropertyNo,
        City,
        Type,
        Rooms,
        Rent
    }

    public enum PropertyForRentsSearch
    {
        PropertyNo,
        City,
        Type,
        PostCode,
        OverseesPassportNo,
        OwnerPassportNo
    }

    public class PropertyForRentsService
    {
        private readonly PropertyForRentsRepository _repository;

        public PropertyForRentsService(PropertyForRentsRepository repository)
        {
            _repository = repository;
        }

        // GET: PropertyForRents
        public async Task<List<PropertyForRent>> FilteredList(int sortParam = 0, int searchParam = 0, string searchInput = "")
        {
            return await _repository.FilteredList((PropertyForRentsSort)sortParam, (PropertyForRentsSearch)searchParam, searchInput);
        }
        
        // GET: PropertyForRents/Details/5
        public async Task<PropertyForRent> GetPropertyForRent(int? id)
        {
            return await _repository.Get(id);
        }

        public async Task<int> Create(PropertyForRent propertyForRent)
        {
            return await _repository.Create(propertyForRent);
        }

        // POST: PropertyForRents/Edit/5
        public async Task<int> Edit(int id, PropertyForRent propertyForRent)
        {
            return await _repository.Edit(id, propertyForRent);
        }

        // POST: PropertyForRents/Delete/5
        public async Task<int> DeleteConfirmed(int id)
        {
            return await _repository.Delete(id);
        }

        public bool PropertyForRentExists(int id)
        {
            return _repository.PropertyForRentExists(id);
        }

        public List<Staff> Staffs()
        {
            return _repository.Staffs();
        }

        public List<PrivateOwner> PrivateOwner()
        {
            return _repository.PrivateOwner();
        }


        public bool ClientWithPassportExists(string userPassport)
        {
            return _repository.ClientWithPassportExists(userPassport);
        }

        public async Task<User> GetUserFromEmail(string email)
        {
            return await _repository.GetUserFromEmail(email);
        }

        public async Task<List<PropertyForRent>> ExtendedSearch(int sortParam, string param11, string param21, string param31, string param41, string param51, string param61)
        {
            if (String.IsNullOrEmpty(param11)) param11 = String.Empty;
            if (String.IsNullOrEmpty(param21)) param21 = String.Empty;
            if (String.IsNullOrEmpty(param31)) param31 = String.Empty;
            if (String.IsNullOrEmpty(param41)) param41 = String.Empty;
            if (String.IsNullOrEmpty(param51)) param51 = String.Empty;
            if (String.IsNullOrEmpty(param61)) param61 = String.Empty;

            return await _repository.ExtendedSearch((PropertyForRentsSort)sortParam, param11, param21, param31, param41, param51, param61);
        }
        public List<PropertyForRent> GetRentableProperties()
        {
            return _repository.GetRentableProperties();
        }

        public Task<List<PropertyForRent>> UserProperties(string passport)
        {
            return _repository.UserProperties(passport);
        }
    }
}
