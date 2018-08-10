using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace PetClinic.Dto
{
    [XmlType("Vet")]
    public class VetDto
    {
        [StringLength(40, MinimumLength = 3)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [XmlElement("Profession")]
        public string Profession { get; set; }

        [Range(22, 65)]
        [XmlElement("Age")]
        public int Age { get; set; }

        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
