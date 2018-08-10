using System;
using System.ComponentModel.DataAnnotations;

namespace PetClinic.Dto
{
    public class PassportDto
    {
        [MinLength(10), MaxLength(10)]
        [RegularExpression(@"^[a-zA-Z]{7}[0-9]{3}$")]
        public string SerialNumber { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string OwnerName { get; set; }

        public string OwnerPhoneNumber { get; set; }

        public string RegistrationDate { get; set; }
    }
}
