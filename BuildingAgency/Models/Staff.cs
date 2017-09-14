using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingAgency.Models
{
    public partial class Staff
    {
        public Staff()
        {
            PropertyForRent = new HashSet<PropertyForRent>();
        }

        [Required]
        public int StaffId { get; set; }
        [Required]
        [RegularExpression(@"^[A-Z]{2}\d{6}", ErrorMessage = "Формат паспорта АА111222! Серия паспорта - латинскими буквами.")]
        public string StaffPassportNo { get; set; }
        [Required]
        [RegularExpression(@"^[А-Я][а-я]+ [А-Я][а-я]+ [А-Я][а-я]+", ErrorMessage = "Только кирилица! Первые буквы - заглавные. Формат: Иваненко Иван Иванович")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Недопустимая длина ФИО. Не менее 5 и не более 50 символов.")]
        public string FullName { get; set; }
        [Required]
        [RegularExpression(@"^[а-я]+", ErrorMessage = "Недопустимый ввод. Только кириллица, без заглавных букв.")]
        public string Position { get; set; }
        [Required]
        [RegularExpression(@"[мж]", ErrorMessage = "Доступные значения: м/ж")]
        public string Sex { get; set; }
        [Required]
        [PastDate(ErrorMessage = "Дата рождения должна быть не позже сегодняшнего числа.")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]*\.?[0-9]*", ErrorMessage = "Cумма зарплаты должна быть либо целого либо дробного типа.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Недопустимая сумма зарплаты! Не менее 1 доллара.")]
        public double Salary { get; set; }
        public int? SuperviserId { get; set; }

        public User User { get; set; }

        public virtual ICollection<PropertyForRent> PropertyForRent { get; set; }
        public virtual Staff Superviser { get; set; }
        public virtual ICollection<Staff> InverseSuperviser { get; set; }
    }
}
