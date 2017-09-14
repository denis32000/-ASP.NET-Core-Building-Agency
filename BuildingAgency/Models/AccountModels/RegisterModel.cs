using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BuildingAgency.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан Email.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Не указан номер паспорта.")]
        [RegularExpression(@"^[A-Z]{2}\d{6}", ErrorMessage = "Формат паспорта АА111222! Серия паспорта - латинскими буквами.")]
        public string PassportNo { get; set; }
    }
}
