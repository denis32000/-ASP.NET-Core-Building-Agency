using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingAgency.Models
{
    public partial class Client
    {
        public Client()
        {
            Contract = new HashSet<Contract>();
            Viewing = new HashSet<Viewing>();
        }
        [Required]
        public int ClientId { get; set; }
        [Required]
        [RegularExpression(@"^[A-Z]{2}\d{6}", ErrorMessage = "Формат пасспорта АА111222!")]
        public string ClientPassportNo { get; set; }
        [Required]
        [RegularExpression(@"^[А-Я][а-я]+ [А-Я][а-я]+ [А-Я][а-я]+", ErrorMessage = "Только кирилица! Первые буквы - заглавные. Формат: Иваненко Иван Иванович")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Недопустимая длина ФИО. Не менее 5 и не более 50 символов.")]
        public string FullName { get; set; }
        [Required]
        [RegularExpression(@"^\+\d{12}$", ErrorMessage = "Номер телефона должен иметь формат +AAcccFFFxxxx")]
        public string PhoneNumber { get; set; }
        [Required]
        [RegularExpression(@"^[а-я]+", ErrorMessage = "Недопустимый ввод. Только кириллица, без заглавных букв.")]
        public string PrefType { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]*\.?[0-9]*", ErrorMessage = "Сумма аренды должна быть либо целого либо дробного типа.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Недопустимая сумма аренды! Не менее 1 доллара.")]
        public double MaxRent { get; set; }

        public User User { get; set; }

        public virtual ICollection<Contract> Contract { get; set; }
        public virtual ICollection<Viewing> Viewing { get; set; }
    }
}
