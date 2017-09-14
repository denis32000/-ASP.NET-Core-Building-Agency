using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingAgency.Models
{
    public partial class User
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^[A-Z]{2}\d{6}", ErrorMessage = "Формат пасспорта АА111222!")]
        public string Passport { get; set; }

        public int? RoleId { get; set; }
        public int? ClientId { get; set; }
        public int? OwnerId { get; set; }
        public int? StaffId { get; set; }

        public Role Role { get; set; }

        public Client Client { get; set; }
        public PrivateOwner PrivateOwner { get; set; }
        public Staff Staff { get; set; }
    }
}
