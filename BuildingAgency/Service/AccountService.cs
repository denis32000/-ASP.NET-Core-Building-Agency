using BuildingAgency.DataAccessLevel;
using BuildingAgency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingAgency.Service
{
    public class AccountService
    {
        private readonly AccountRepository _repository;

        public AccountService(AccountRepository repository)
        {
            _repository = repository;
        }
        public async Task<User> FindUserByEmail(string email, string passport = "")
        {
            return await _repository.TryFindUserWithEmailAndPassport(email, passport);
        }
        public async Task<Role> GetRoleIdByName(string name)
        {
            return await _repository.GetRoleIdByName(name);
        }
        public async Task<int> AddUser(User user)
        {
            return await _repository.AddUser(user);
        }

        public async Task<User> GetUser(string email, string password)
        {
            return await _repository.GetUser(email, password);
        }
        public async Task<List<Client>> GetClients()
        {
            return await _repository.Clients();
        }
        public async Task<List<string>> PrefTypes()
        {
            return await _repository.PrefTypes();
        }
    }
}
