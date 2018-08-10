using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Models
{
    public class ProcedureAnimalAid
    {
        [Required]
        [ForeignKey(nameof(Procedure))]
        public int ProcedureId { get; set; }
        public Procedure Procedure { get; set; }

        [Required]
        [ForeignKey(nameof(AnimalAid))]
        public int AnimalAidId { get; set; }
        public AnimalAid AnimalAid { get; set; }

    }
}