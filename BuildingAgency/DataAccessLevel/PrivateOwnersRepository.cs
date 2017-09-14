using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.DataAccessLevel
{
    public class PrivateOwnersRepository
    {
        private readonly BuildingAgencyContext _context;

        public PrivateOwnersRepository(BuildingAgencyContext context)
        {
            _context = context;
        }

        public async Task<List<PrivateOwner>> FilteredList(PrivateOwnersSort sortParam, PrivateOwnersSearch searchParam, string searchInput)
        {
            var _ownersContext = _context.PrivateOwner
                .Include(p => p.PropertyForRent)
                .OrderBy(c => c.FullName);

            var list = await _ownersContext.ToListAsync();

            if (searchInput != null && searchInput != String.Empty)
                switch (searchParam)
                {
                    case PrivateOwnersSearch.FullName: list = list.FindAll(v => v.FullName.Contains(searchInput)); break;
                    case PrivateOwnersSearch.City: list = list.FindAll(v => v.City.Contains(searchInput)); break;
                    case PrivateOwnersSearch.PhoneNumber: list = list.FindAll(v => v.PhoneNumber.Contains(searchInput)); break;
                    case PrivateOwnersSearch.PassportNo: list = list.FindAll(v => v.OwnerPassportNo.Contains(searchInput)); break;
                }

            switch (sortParam)
            {
                case PrivateOwnersSort.FullName: return list.OrderBy(v => v.FullName).ToList();
                case PrivateOwnersSort.City: return list.OrderBy(v => v.City).ToList();
                case PrivateOwnersSort.PassportNo: return list.OrderBy(v => v.OwnerPassportNo).ToList();
                default: return list.OrderBy(v => v.FullName).ToList();
            }
        }

        // GET: PrivateOwners/Details/5
        public async Task<PrivateOwner> Get(int? id)
        {
            var owner = await _context.PrivateOwner
                .Include(p => p.PropertyForRent)
                .SingleOrDefaultAsync(m => m.OwnerId == id);

            return owner;
        }

        public async Task<int> Create(PrivateOwner owner)
        {
            _context.Add(owner);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> Edit(int id, PrivateOwner owner)
        {
            try
            {
                _context.Update(owner);
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrivateOwnerExists(id))
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
            var owner = await _context.PrivateOwner.SingleOrDefaultAsync(m => m.OwnerId == id);
            _context.PrivateOwner.Remove(owner);
            return await _context.SaveChangesAsync();
        }

        public bool PrivateOwnerExists(int id)
        {
            return _context.PrivateOwner
                .Any(e => e.OwnerId == id);
        }
    }
}
