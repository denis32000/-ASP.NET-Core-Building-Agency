using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BuildingAgency.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан Email.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
