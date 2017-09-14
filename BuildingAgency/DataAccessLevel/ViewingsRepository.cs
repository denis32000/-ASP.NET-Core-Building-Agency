using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.DataAccessLevel
{
    public class ViewingsRepository
    {
            private readonly BuildingAgencyContext _context;

            public ViewingsRepository(BuildingAgencyContext context)
            {
                _context = context;
            }

            public async Task<List<Viewing>> FilteredList(ViewingsSort sortParam, ViewingsSearch searchParam, string searchInput)
            {
                var _viewingsContext = _context.Viewing
                    .Include(m => m.Client)
                    .Include(m => m.Property)
                    .OrderBy(c => c.ViewNo);

                var list = await _viewingsContext.ToListAsync();
            
                if (searchInput != null && searchInput != String.Empty)
                {
                    switch (searchParam)
                    {
                        case ViewingsSearch.ViewNo: list = list.FindAll(v => v.ViewNo.ToString().Equals(searchInput)); break;
                        case ViewingsSearch.ClientPassport: list = list.FindAll(v => v.Client.ClientPassportNo.Contains(searchInput)); break;
                        case ViewingsSearch.PropertyNo: list = list.FindAll(v => v.Property.PropertyNo.ToString().Equals(searchInput)); break;
                        case ViewingsSearch.ViewDate: list = list.FindAll(v => v.ViewDate.ToString().Contains(searchInput)); break;
                        case ViewingsSearch.Comment: list = list.FindAll(v => v.Comment.Contains(searchInput)); break;
                    }
                }

                switch (sortParam)
                {
                    case ViewingsSort.ViewNo: return list.OrderBy(v => v.ViewNo).ToList();
                    case ViewingsSort.ViewDate: return list.OrderBy(v => v.ViewDate).ToList();
                    default: return list.OrderBy(v => v.ViewNo).ToList();
                }
            }

            // GET: Viewings/Details/5
            public async Task<Viewing> Get(int? id)
            {
                var viewing = await _context.Viewing
                    .Include(m => m.Client)
                    .Include(m => m.Property)
                    .SingleOrDefaultAsync(m => m.ViewNo == id);

                return viewing;
            }

            public async Task<int> Create(Viewing viewing)
            {
                _context.Add(viewing);

                return await _context.SaveChangesAsync();
            }


        public async Task<int> Edit(int id, Viewing viewing)
            {
                try
                {
                    _context.Update(viewing);
                    return await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
            {
                if (!ViewingExists(id))
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
                var viewing = await _context.Viewing.SingleOrDefaultAsync(m => m.ViewNo == id);
                _context.Viewing.Remove(viewing);
                return await _context.SaveChangesAsync();
            }

            public bool ViewingExists(int id)
            {
                return _context.Viewing.Any(e => e.ViewNo == id);
            }

        public List<Client> Clients()
        {
            return _context.Client
                .Include(c => c.User)
                .Include(c => c.Viewing)
                .Include(c => c.Contract)
                .ToList();
        }

        public List<PropertyForRent> Properties()
        {
            return _context.PropertyForRent
                .Include(p => p.Contract)
                .ToList();
        }
        public Contract GetContractById(int id)
        {
            return _context.Contract.Where(c => c.ContractId == id).First();
        }
        public List<Viewing> GetPropertyComments(int propertyId)
        {
            return _context.Viewing
                .Include(v => v.Property)
                .Where(v => v.PropertyId == propertyId)
                .ToList();
        }
    }
}
