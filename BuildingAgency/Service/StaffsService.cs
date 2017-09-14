using BuildingAgency.DataAccessLevel;
using BuildingAgency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.Service
{
    public enum StaffsSort
    {
        FullName,
        Position,
        DateOfBirth,
        Salary
    }

    public enum StaffsSearch
    {
        FullName,
        Position,
        DateOfBirth,
        SuperviserPassport,
        PassportNo
    }

    public class StaffsService
    {
        private readonly StaffsRepository _repository;

        public StaffsService(StaffsRepository repository)
        {
            _repository = repository;
        }

        // GET: Staffs
        public async Task<List<Staff>> FilteredList(int sortParam = 0, int searchParam = 0, string searchInput = "")
        {
            return await _repository.FilteredList((StaffsSort)sortParam, (StaffsSearch)searchParam, searchInput);
        }
        
        // GET: Staffs/Details/5
        public async Task<Staff> GetStaff(int? id)
        {
            return await _repository.Get(id);
        }

        public async Task<int> Create(Staff staff)
        {
            return await _repository.Create(staff);
        }

        // POST: Staffs/Edit/5
        public async Task<int> Edit(int id, Staff staff)
        {
            return await _repository.Edit(id, staff);
        }

        // POST: Staffs/Delete/5
        public async Task<int> DeleteConfirmed(int id)
        {
            return await _repository.Delete(id);
        }

        public bool StaffExists(int id)
        {
            return _repository.StaffExists(id);
        }

        public List<Staff> Staffs()
        {
            return _repository.Staffs();
        }
    }
}
