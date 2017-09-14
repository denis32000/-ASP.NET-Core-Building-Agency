using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.DataAccessLevel
{
    public class AccountRepository
    {
        private readonly BuildingAgencyContext _context;

        public AccountRepository(BuildingAgencyContext context)
        {
            _context = context;
        }

        public async Task<User> TryFindUserWithEmailAndPassport(string email, string passport = "")
        {
            if (passport == String.Empty)
                return await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
            else
                return await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email || u.Passport == passport);
        }

        public async Task<User> GetUser(string email, string password)
        {
            return await _context.User
                       .Include(u => u.Role)
                       .FirstOrDefaultAsync(u => u.Email == email && u.Password == Startup.CalculateMD5Hash(password));
        }

        public async Task<Role> GetRoleIdByName(string name)
        {
            return await _context.Role.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<int> AddUser(User user)
        {
            _context.User.Add(user);
            return await _context.SaveChangesAsync();
        }
        public async Task<List<Client>> Clients()
        {
            return await _context.Client.ToListAsync();
        }
        public async Task<List<string>> PrefTypes()
        {
            return await _context.PropertyForRent.Select(p => p.Type).Distinct().ToListAsync();
        }

    }
}
