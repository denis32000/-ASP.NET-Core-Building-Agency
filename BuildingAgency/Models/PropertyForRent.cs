using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingAgency.Models
{
    public partial class PropertyForRent
    {
        public PropertyForRent()
        {
            Contract = new HashSet<Contract>();
            Viewing = new HashSet<Viewing>();
        }

        public int PropertyId { get; set; }
        public string PropertyNo { get; set; }
        [Required]
        [RegularExpression(@"^[А-Я][а-я]+", ErrorMessage = "Недопустимый ввод. Первая буква заглавная. Только кириллица. Пример: Одесса")]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [RegularExpression(@"^[0-9]{6}", ErrorMessage = "Только числа! Длина 6 символов.")]
        public string PostCode { get; set; }
        [RegularExpression(@"^[а-я]+", ErrorMessage = "Недопустимый ввод. Только кириллица, без заглавных букв.")]
        public string Type { get; set; }
        [RegularExpression(@"^[0-9]+", ErrorMessage = "Только числа!")]
        [Range(1, 20, ErrorMessage = "Недопустимая цена аренды! От 1 до 20")]
        public int Rooms { get; set; }
        [RegularExpression(@"^[0-9]*\.?[0-9]*", ErrorMessage = "Цена аренды должна быть либо целого либо дробного типа.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Недопустимая цена аренды! Не менее 1 доллара.")]
        public double Rent { get; set; }

        public int? OwnerId { get; set; }
        public int? OverseesById { get; set; }

        public virtual ICollection<Contract> Contract { get; set; }
        public virtual ICollection<Viewing> Viewing { get; set; }
        public virtual Staff OverseesBy { get; set; }
        public virtual PrivateOwner Owner { get; set; }
    }
}
