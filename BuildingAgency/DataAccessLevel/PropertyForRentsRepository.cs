using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.DataAccessLevel
{
    public class PropertyForRentsRepository
    {
            private readonly BuildingAgencyContext _context;

            public PropertyForRentsRepository(BuildingAgencyContext context)
            {
                _context = context;
            }

            public async Task<List<PropertyForRent>> FilteredList(PropertyForRentsSort sortParam, PropertyForRentsSearch searchParam, string searchInput)
            {
                var _propertysContext = _context.PropertyForRent
                    .Include(p => p.OverseesBy)
                    .Include(p => p.Owner)
                    .OrderBy(c => c.PropertyNo);

                var list = await _propertysContext.ToListAsync();
            
                if (searchInput != null && searchInput != String.Empty)
                {
                    switch (searchParam)
                    {
                        case PropertyForRentsSearch.PropertyNo: list = list.FindAll(v => v.PropertyNo.Contains(searchInput)); break;
                        case PropertyForRentsSearch.City: list = list.FindAll(v => v.City.Contains(searchInput)); break;
                        case PropertyForRentsSearch.Type: list = list.FindAll(v => v.Type.Contains(searchInput)); break;
                        case PropertyForRentsSearch.OverseesPassportNo: list = list.FindAll(v => v.OverseesBy.StaffPassportNo.Contains(searchInput)); break;
                        case PropertyForRentsSearch.OwnerPassportNo: list = list.FindAll(v => v.Owner.OwnerPassportNo.Contains(searchInput)); break;
                    }
                }

                switch (sortParam)
                {
                    case PropertyForRentsSort.PropertyNo: return list.OrderBy(v => v.PropertyNo).ToList();
                    case PropertyForRentsSort.City: return list.OrderBy(v => v.City).ToList();
                    case PropertyForRentsSort.Type: return list.OrderBy(v => v.Type).ToList();
                case PropertyForRentsSort.Rooms: return list.OrderBy(v => v.Rooms).ToList();
                case PropertyForRentsSort.Rent: return list.OrderBy(v => v.Rent).ToList();
                default: return list.OrderBy(v => v.PropertyNo).ToList();
                }
            }

            // GET: PropertyForRents/Details/5
            public async Task<PropertyForRent> Get(int? id)
            {
                var property = await _context.PropertyForRent
                .Include(p => p.OverseesBy)
                .Include(p => p.Owner)
                    .SingleOrDefaultAsync(m => m.PropertyId == id);

                return property;
            }

            public async Task<int> Create(PropertyForRent property)
            {
                _context.Add(property);

                return await _context.SaveChangesAsync();
            }

            public async Task<int> Edit(int id, PropertyForRent property)
            {
                try
                {
                    _context.Update(property);
                    return await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                if (!PropertyForRentExists(id))
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
                var property = await _context.PropertyForRent.SingleOrDefaultAsync(m => m.PropertyId == id);
                _context.PropertyForRent.Remove(property);
                return await _context.SaveChangesAsync();
            }

            public bool PropertyForRentExists(int id)
            {
                return _context.PropertyForRent.Any(e => e.PropertyId == id);
            }

        public bool ClientWithPassportExists(string userPassport)
        {
            return _context.Client.Any(x => x.ClientPassportNo == userPassport);
        }

        public async Task<User> GetUserFromEmail(string email)
        {
            var usersContext = _context.User.Include(u => u.Client);
            List<User> myList = await usersContext.ToListAsync();
            return myList.Find(x => x.Email == email);
        }

        public List<Staff> Staffs()
        {
            return _context.Staff.ToList();
        }

        public List<PrivateOwner> PrivateOwner()
        {
            return _context.PrivateOwner
                .Include(p => p.PropertyForRent)
                .ToList();
        }

        public async Task<List<PropertyForRent>> ExtendedSearch(PropertyForRentsSort sortParam, string propertyNo, string city, 
            string type, string postcode, string ownerpassport, string staffpassport)
        {
            var _propertysContext = _context.PropertyForRent
                    .Include(p => p.OverseesBy)
                    .Include(p => p.Owner)
                    .OrderBy(c => c.PropertyNo);

            var list = await _propertysContext.ToListAsync();


            list = list.FindAll(v => 
                (  (String.IsNullOrEmpty(propertyNo) ? true : v.PropertyNo.Contains(propertyNo)) 
                && (String.IsNullOrEmpty(city) ? true : v.City.Contains(city)) 
                && (String.IsNullOrEmpty(type) ? true : v.Type.Contains(type)) 
                && (String.IsNullOrEmpty(postcode) ? true : v.PostCode.Contains(postcode)) 
                && (String.IsNullOrEmpty(ownerpassport) ? true : v.Owner.OwnerPassportNo.Contains(ownerpassport)) 
                && (String.IsNullOrEmpty(staffpassport) ? true : v.OverseesBy.StaffPassportNo.Contains(staffpassport))
                ) );

            switch (sortParam)
            {
                case PropertyForRentsSort.PropertyNo: return list.OrderBy(v => v.PropertyNo).ToList();
                case PropertyForRentsSort.City: return list.OrderBy(v => v.City).ToList();
                case PropertyForRentsSort.Type: return list.OrderBy(v => v.Type).ToList();
                case PropertyForRentsSort.Rooms: return list.OrderBy(v => v.Rooms).ToList();
                case PropertyForRentsSort.Rent: return list.OrderBy(v => v.Rent).ToList();
                default: return list.OrderBy(v => v.PropertyNo).ToList();
            }
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

        public async Task<List<PropertyForRent>> UserProperties(string passport)
        {
            var _propertysContext = _context.PropertyForRent
                    .Include(p => p.OverseesBy)
                    .Include(p => p.Owner)
                    .Where(p => p.Owner.OwnerPassportNo == passport);

            var list = await _propertysContext.ToListAsync();
            return list;
        }
    }
}
