using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.DataAccessLevel
{
    public class StaffsRepository
    {
            private readonly BuildingAgencyContext _context;

            public StaffsRepository(BuildingAgencyContext context)
            {
                _context = context;
            }

            public async Task<List<Staff>> FilteredList(StaffsSort sortParam, StaffsSearch searchParam, string searchInput)
            {
                var _staffsContext = _context.Staff.OrderBy(c => c.FullName);
                var list = await _staffsContext.ToListAsync();
            
                if (searchInput != null && searchInput != String.Empty)
                {
                    switch (searchParam)
                    {
                        case StaffsSearch.FullName: list = list.FindAll(v => v.FullName.Contains(searchInput)); break;
                        case StaffsSearch.Position: list = list.FindAll(v => v.Position.Contains(searchInput)); break;
                        case StaffsSearch.DateOfBirth: list = list.FindAll(v => v.DateOfBirth.ToString().Contains(searchInput)); break;
                        case StaffsSearch.SuperviserPassport: list = list.FindAll(v => v.Superviser.StaffPassportNo.Contains(searchInput)); break;
                        case StaffsSearch.PassportNo: list = list.FindAll(v => v.StaffPassportNo.Contains(searchInput)); break;
                    }
                }

                switch (sortParam)
                {
                    case StaffsSort.FullName: return list.OrderBy(v => v.FullName).ToList();
                    case StaffsSort.Position: return list.OrderBy(v => v.Position).ToList();
                    case StaffsSort.DateOfBirth: return list.OrderBy(v => v.DateOfBirth).ToList();
                case StaffsSort.Salary: return list.OrderBy(v => v.Salary).ToList();
                default: return list.OrderBy(v => v.FullName).ToList();
                }
            }

            // GET: Staffs/Details/5
            public async Task<Staff> Get(int? id)
            {
                var staff = await _context.Staff
                    .SingleOrDefaultAsync(m => m.StaffId == id);

                return staff;
            }

            public async Task<int> Create(Staff staff)
            {
                _context.Add(staff);

                return await _context.SaveChangesAsync();
            }

            public async Task<int> Edit(int id, Staff staff)
            {
                try
                {
                    _context.Update(staff);
                    return await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                if (!StaffExists(id))
                {
                    return 0;
                }
                else
                {
                    throw;
                }
            }
            }

            public async Task<int> Delete(int id)
            {
                var staff = await _context.Staff.SingleOrDefaultAsync(m => m.StaffId == id);
                _context.Staff.Remove(staff);
                return await _context.SaveChangesAsync();
            }

            public bool StaffExists(int id)
            {
                return _context.Staff.Any(e => e.StaffId == id);
            }

        public List<Staff> Staffs()
        {
            return _context.Staff.ToList();
        }
    }
}
