using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace PetClinic.Dto
{
    [XmlType("AnimalAid")]
    public class AidDto
    {
        [XmlElement("Name")]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
    }
}
