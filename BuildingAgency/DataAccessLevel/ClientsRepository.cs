using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.DataAccessLevel
{
        public class ClientsRepository
        {
            private readonly BuildingAgencyContext _context;

            public ClientsRepository(BuildingAgencyContext context)
            {
                _context = context;
            }

            public async Task<List<Client>> FilteredList(ClientsSort sortParam, ClientsSearch searchParam, string searchInput)
            {
                var _clientsContext = _context.Client.OrderBy(c => c.FullName);
                var list = await _clientsContext.ToListAsync();

                if (searchInput != null && searchInput != String.Empty)
                {
                    switch (searchParam)
                    {
                        case ClientsSearch.FullName: list = list.FindAll(v => v.FullName.Contains(searchInput)); break;
                        case ClientsSearch.PrefType: list = list.FindAll(v => v.PrefType.Contains(searchInput)); break;
                        case ClientsSearch.PhoneNumber: list = list.FindAll(v => v.PhoneNumber.Contains(searchInput)); break;
                        case ClientsSearch.PassportNo: list = list.FindAll(v => v.ClientPassportNo.Contains(searchInput)); break;
                    }
                }

                switch (sortParam)
                {
                    case ClientsSort.FullName: return list.OrderBy(v => v.FullName).ToList();
                    case ClientsSort.PrefType: return list.OrderBy(v => v.PrefType).ToList();
                    case ClientsSort.MaxRent: return list.OrderBy(v => v.MaxRent).ToList();
                    default: return list.OrderBy(v => v.FullName).ToList();
                }
            }

            // GET: Clients/Details/5
            public async Task<Client> Get(int? id)
            {
                var client = await _context.Client
                    .SingleOrDefaultAsync(m => m.ClientId == id);

                return client;
            }

            public async Task<int> Create(Client client)
            {
                _context.Add(client);
            int result = await _context.SaveChangesAsync();

            Client newClient = _context.Client.FirstOrDefault(c => c.ClientPassportNo == client.ClientPassportNo);

            if (newClient != null)
            {
                User thisUser = _context.User.FirstOrDefault(u => u.Passport == newClient.ClientPassportNo);

                if (thisUser != null)
                {
                    thisUser.ClientId = newClient.ClientId;

                    _context.Update(thisUser);
                    await _context.SaveChangesAsync();
                }
            }

                return result;
            }

            public async Task<int> Edit(int id, Client client)
            {
                try
                {
                    _context.Update(client);
                    return await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(id))
                    {
                        return 0;
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction("Index");
            }

            public async Task<int> Delete(int id)
            {
                var client = await _context.Client.SingleOrDefaultAsync(m => m.ClientId == id);
                _context.Client.Remove(client);
                return await _context.SaveChangesAsync();
            }

            public bool ClientExists(int id)
            {
                return _context.Client.Any(e => e.ClientId == id);
            }

        public List<string> PropertyTypes()
        {
            return _context.PropertyForRent
                .Select(p => p.Type)
                .Distinct()
                .ToList();
        }
    }
}
