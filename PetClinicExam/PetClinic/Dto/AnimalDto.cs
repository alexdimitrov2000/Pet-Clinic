using System.ComponentModel.DataAnnotations;

namespace PetClinic.Dto
{
    public class AnimalDto
    {
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string Type { get; set; }

        [Range(1, int.MaxValue)]
        public int Age { get; set; }

        public PassportDto Passport { get; set; }
    }
}
