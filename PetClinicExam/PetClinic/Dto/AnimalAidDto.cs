using System.ComponentModel.DataAnnotations;

namespace PetClinic.Dto
{
    public class AnimalAidDto
    {
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
        
        [Range(0.01, (double)decimal.MaxValue)]
        public decimal Price { get; set; }
    }
}
