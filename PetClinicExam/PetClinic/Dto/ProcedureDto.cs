using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace PetClinic.Dto
{
    [XmlType("Procedure")]
    public class ProcedureDto
    {
        [XmlElement("Vet")]
        [StringLength(40, MinimumLength = 3)]
        public string VetName { get; set; }

        [XmlElement("Animal")]
        [StringLength(20, MinimumLength = 3)]
        public string AnimalSerialNumber { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        [XmlArrayItem("AnimalAid")]
        public AidDto[] AnimalAids { get; set; }
    }
}
