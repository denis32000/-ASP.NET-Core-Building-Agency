using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BuildingAgency.Models
{
    public sealed class PastDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return DateTime.Parse(value.ToString()).CompareTo(DateTime.Now.Date) < 0;
        }
    }

    //public sealed class StartRentDateAttribute : ValidationAttribute
    //{
    //    public override bool IsValid(object value)
    //    {
    //        return DateTime.Parse(value.ToString()).CompareTo(DateTime.Now.Date) >= 0;
    //    }
    //}
    //public sealed class FinishRentDateAttribute : ValidationAttribute
    //{
    //    public override bool IsValid(object value)
    //    {
    //        return DateTime.Parse(value.ToString()).CompareTo(DateTime.Now.Date) > 0;
    //    }
    //}

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class StartAndFinishDatesValidation : ValidationAttribute
    {
        string _prop;

        public StartAndFinishDatesValidation(string prop)
        {
            this._prop = prop;
        }

        // value = банковский перевод - это значение введенное
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Object obj = validationContext.ObjectInstance;
            var propertyInfo = obj.GetType().GetProperty(this._prop);

            var startDateValue = propertyInfo.GetValue(obj, null);

            if (startDateValue == null)
                return new ValidationResult("Вы должны сразу выбрать дату начала контракта!");

            DateTime startDate = (DateTime)startDateValue;
            DateTime finishDate = DateTime.Parse(value.ToString());

            if (finishDate.CompareTo(startDate) > 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(this.ErrorMessage);
        }
    }


    public partial class Contract
    {
        public int ContractId { get; set; }
        public int PropertyId { get; set; }
        [Required]
        //[RegularExpression(@"^[а-я]+", ErrorMessage = "Недопустимый ввод.")]
        public string PaymentMethod { get; set; }
        [Required]
        public bool Paid { get; set; }

        [Required]
        //[StartRentDate(ErrorMessage = "Дата начала аренды не должна быть раньше сегодняшнего числа.")]
        public DateTime RentStart { get; set; }
        [Required]
        //[FinishRentDate(ErrorMessage = "Дата окончания аренды должна быть позже сегодняшнего числа.")]
        [StartAndFinishDatesValidation("RentStart", ErrorMessage = "Дата окончания аренды должна быть позже даты начала аренды.")]
        public DateTime RentFinish { get; set; }
        
        [Required]
        [RegularExpression(@"^[0-9]*\.?[0-9]*", ErrorMessage = "Цена аренды должна быть либо целого либо дробного типа.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Недопустимая цена аренды! Не менее 1 доллара.")]
        //[DataType(DataType.Currency)]
        //[DoubleModelBinder]
        //[ModelBinder(BinderType = typeof(AuthorEntityBinder))]
        public double Rent { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]*\.?[0-9]*", ErrorMessage = "Сумма депозита должна быть либо целого либо дробного типа.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Недопустимая сумма депозита! Не менее 1 доллара.")]
        public double Deposit { get; set; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Неверно введённое кол-во дней продолжительности контракта! Не менее 1 дня!")]
        public int Duration { get; set; }
        public int? ClientId { get; set; }

        public virtual Client Client { get; set; }
        public virtual PropertyForRent Property { get; set; }
    }
}
