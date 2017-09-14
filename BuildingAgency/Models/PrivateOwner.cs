using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingAgency.Models
{
    public partial class PrivateOwner
    {
        public PrivateOwner()
        {
            PropertyForRent = new HashSet<PropertyForRent>();
        }

        [Required]
        [Key]
        public int OwnerId { get; set; }
        [Required]
        [RegularExpression(@"^[A-Z]{2}\d{6}", ErrorMessage = "Формат пасспорта АА111222!")]
        public string OwnerPassportNo { get; set; }
        [Required]
        [RegularExpression(@"^[А-Я][а-я]+ [А-Я][а-я]+ [А-Я][а-я]+", ErrorMessage = "Только кирилица! Первые буквы - заглавные. Формат: Иваненко Иван Иванович")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Недопустимая длина ФИО. Не менее 5 и не более 50 символов.")]
        public string FullName { get; set; }
        [Required]
        [RegularExpression(@"^\+\d{12}$", ErrorMessage = "Номер телефона должен иметь формат +AAcccFFFxxxx")]
        public string PhoneNumber { get; set; }
        [RegularExpression(@"^[А-Я][а-я]+", ErrorMessage = "Недопустимый ввод. Первая буква заглавная. Только кириллица. Пример: Одесса")]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }

        public User User { get; set; }

        public virtual ICollection<PropertyForRent> PropertyForRent { get; set; }
    }
}
