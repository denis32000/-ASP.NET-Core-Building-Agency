using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingAgency.Models
{
    public partial class Viewing
    {
        public int ViewNo { get; set; }
        public int ClientId { get; set; }
        public int PropertyId { get; set; }
        public DateTime ViewDate { get; set; }
        [Required]
        //[StringLength(100, MinimumLength = 2, ErrorMessage = "Отзыв не должен быть короче 2 символов.")]
        public string Comment { get; set; }

        public virtual Client Client { get; set; }
        public virtual PropertyForRent Property { get; set; }
    }
}
