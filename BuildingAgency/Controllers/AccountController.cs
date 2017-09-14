using BuildingAgency.Models;
using BuildingAgency.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildingAgency.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _service;

        public AccountController(AccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ControllPanel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _service.FindUserByEmail(model.Email, model.PassportNo);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    user = new User { Email = model.Email, Password = model.Password, Passport = model.PassportNo };
                    Role userRole = await _service.GetRoleIdByName("user");
                    if (userRole != null)
                        user.Role = userRole;

                    int result = await _service.AddUser(user);
                    //TODO: check result with error message

                    await Authenticate(user); // аутентификация

                    if (await ClientExists(user.Passport))
                        return RedirectToAction("Index", "Home");
                    else
                        return await AddClientInfo(user.Passport);
                }
                else
                    ModelState.AddModelError(string.Empty, "Некорректные логин и(или) номер паcпорта.");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _service.GetUser(model.Email, model.Password);

                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    if (await ClientExists(user.Passport))
                        return RedirectToAction("Index", "Home");
                    else
                        return await AddClientInfo(user.Passport);
                }
                ModelState.AddModelError(string.Empty, "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        public async Task<bool> ClientExists(string userPassport)
        {
            List<Client> clients = await _service.GetClients();

            return clients.FirstOrDefault(x => x.ClientPassportNo == userPassport) != null;
        }

        public async Task<IActionResult> AddClientInfo(string userPassport, string modelError = "")
        {
            if (modelError != String.Empty)
                ModelState.AddModelError(string.Empty, modelError);

            Client newClient = new Client
            {
                ClientPassportNo = userPassport,
                FullName = "Иванов Иван Иванович",
                MaxRent = 500,
                PhoneNumber = "+38095123123",
                PrefType = "дом"
            };
            ViewData["PrefType"] = new SelectList(await _service.PrefTypes());

            return View("RegisterClient", newClient);
        }

        [Authorize]
        public async Task<IActionResult> UserInfo()
        {
            User meUser = await _service.FindUserByEmail(User.Identity.Name);

            if (meUser == null)
                return NotFound();
            
            List<Client> listOfClients = await _service.GetClients();
            Client meClient = listOfClients.FirstOrDefault(x => x.ClientPassportNo == meUser.Passport);

            if (meClient == null)
                return await AddClientInfo(meUser.Passport, "Client with such passport doesn't exist. Please register yourself as a client.");

            ViewData["clientEmail"] = meUser.Email;
            ViewData["clientRole"] = meUser.Role.Name;
            return View(meClient);
        }

        private async Task Authenticate(User user)
        {
            var role = user.Role?.Name;
            int index = role.IndexOf(' ');
            if (index > 0) role = role.Remove(index);

            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }
    }
}
