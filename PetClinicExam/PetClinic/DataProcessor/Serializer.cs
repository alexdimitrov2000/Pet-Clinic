namespace PetClinic.DataProcessor
{
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using PetClinic.Data;
    using PetClinic.ExportDto;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animals = context.Animals
                .Include(a => a.Passport)
                .Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                .Select(a => new
                {
                    OwnerName = a.Passport.OwnerName,
                    AnimalName = a.Name,
                    Age = a.Age,
                    SerialNumber = a.Passport.SerialNumber,
                    RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyyy")
                })
                .OrderBy(a => a.Age)
                .ThenBy(a => a.SerialNumber)
                .ToList();

            var serializedAnimals = JsonConvert.SerializeObject(animals, Newtonsoft.Json.Formatting.Indented);

            return serializedAnimals;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var procedures = context.Procedures
                .Include(p => p.ProcedureAnimalAids)
                .ThenInclude(pa => pa.AnimalAid)
                .Include(p => p.Cost)
                .Include(p => p.Animal)
                .ThenInclude(a => a.Passport)
                .OrderBy(p => p.DateTime)
                .Select(p => new ProcedureDto
                {
                    PassportSerialNumber = p.Animal.PassportSerialNumber,
                    OwnerPhoneNumber = p.Animal.Passport.OwnerPhoneNumber,
                    DateTime = p.DateTime.ToString("dd-MM-yyyy"),
                    AnimalAids = p.ProcedureAnimalAids.Select(pa => new AnimalAidDto
                    {
                        Name = pa.AnimalAid.Name,
                        Price = pa.AnimalAid.Price
                    }).ToArray(),
                    TotalPrice = p.ProcedureAnimalAids.Sum(pa => pa.AnimalAid.Price)
                })
                .OrderBy(p => p.PassportSerialNumber)
                .ToArray();

            var serializer = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            var stringWriter = new StringWriter();

            serializer.Serialize(stringWriter, procedures, namespaces);

            return stringWriter.ToString();
        }
    }
}
