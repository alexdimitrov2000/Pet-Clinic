namespace PetClinic.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using DataValidation = System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using PetClinic.Data;
    using PetClinic.Dto;
    using PetClinic.Models;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using System.IO;
    using System.Globalization;

    public class Deserializer
    {
        public const string Failure_Message = "Error: Invalid data.";

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var animalAidsJson = JsonConvert.DeserializeObject<AnimalAidDto[]>(jsonString);

            List<AnimalAid> animalAids = new List<AnimalAid>();

            var sb = new StringBuilder();

            foreach (var aid in animalAidsJson)
            {
                if (!IsValid(aid) || context.AnimalAids.Any(a => a.Name == aid.Name) || animalAids.Any(a => a.Name == aid.Name))
                {
                    sb.AppendLine(Failure_Message);
                    continue;
                }

                animalAids.Add(new AnimalAid
                {
                    Name = aid.Name,
                    Price = aid.Price
                });

                sb.AppendLine($"Record {aid.Name} successfully imported.");
            }

            context.AnimalAids.AddRange(animalAids);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var animalsJson = JsonConvert.DeserializeObject<AnimalDto[]>(jsonString);

            var animals = new List<Animal>();

            var sb = new StringBuilder();

            foreach (var animalDto in animalsJson)
            {
                if (!IsPassportValid(animalDto.Passport) || !IsValid(animalDto))
                {
                    sb.AppendLine(Failure_Message);
                    continue;
                }

                var passport = context.Passports.FirstOrDefault(p => p.SerialNumber == animalDto.Passport.SerialNumber);

                if (passport != null)
                {
                    sb.AppendLine(Failure_Message);
                    continue;
                }

                var passportDto = animalDto.Passport;

                passport = new Passport
                {
                    SerialNumber = passportDto.SerialNumber,
                    OwnerName = passportDto.OwnerName,
                    OwnerPhoneNumber = passportDto.OwnerPhoneNumber,
                    RegistrationDate = DateTime.ParseExact(passportDto.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                };

                context.Passports.Add(passport);
                context.SaveChanges();

                animals.Add(new Animal
                {
                    Name = animalDto.Name,
                    Type = animalDto.Type,
                    Age = animalDto.Age,
                    PassportSerialNumber = passport.SerialNumber,
                    Passport = passport
                });

                sb.AppendLine($"Record {animalDto.Name} Passport №: {passport.SerialNumber} successfully imported.");
            }

            context.Animals.AddRange(animals);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(VetDto[]), new XmlRootAttribute("Vets"));

            var vetsXml = (VetDto[])serializer.Deserialize(new StringReader(xmlString));

            var vets = new List<Vet>();

            var sb = new StringBuilder();

            foreach (var vetDto in vetsXml)
            {
                if (!IsValid(vetDto) ||
                    context.Vets.Any(v => v.PhoneNumber == vetDto.PhoneNumber) ||
                    vets.Any(v => v.PhoneNumber == vetDto.PhoneNumber))
                {
                    sb.AppendLine(Failure_Message);
                    continue;
                }

                //if (vetDto.PhoneNumber != null)
                //{
                //    if (!IsPhoneNumberValid(vetDto.PhoneNumber))
                //    {
                //        sb.AppendLine(Failure_Message);
                //        continue;
                //    }
                //}

                if (vetDto.PhoneNumber == null || !IsPhoneNumberValid(vetDto.PhoneNumber))
                {
                    sb.AppendLine(Failure_Message);
                    continue;
                }

                vets.Add(new Vet
                {
                    Name = vetDto.Name,
                    PhoneNumber = vetDto.PhoneNumber,
                    Age = vetDto.Age,
                    Profession = vetDto.Profession
                });

                sb.AppendLine($"Record {vetDto.Name} successfully imported.");
            }

            context.Vets.AddRange(vets);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));

            var proceduresXml = (ProcedureDto[])serializer.Deserialize(new StringReader(xmlString));

            var procedures = new List<Procedure>();

            var aids = new List<AnimalAid>();

            var procedureAnimalAids = new List<ProcedureAnimalAid>();

            var sb = new StringBuilder();

            foreach (var proc in proceduresXml)
            {
                if (!IsValid(proc))
                {
                    sb.AppendLine(Failure_Message);
                    continue;
                }

                bool repeatedAid = false;
                aids = new List<AnimalAid>();
                procedureAnimalAids = new List<ProcedureAnimalAid>();

                var vet = context.Vets.FirstOrDefault(v => v.Name == proc.VetName);
                var animal = context.Animals.FirstOrDefault(a => a.PassportSerialNumber == proc.AnimalSerialNumber);

                var animalAids = proc.AnimalAids;

                bool allAidsExist = context.AnimalAids.Any(a => animalAids.Any(aa => aa.Name == a.Name));

                if (vet == null || animal == null || !allAidsExist)
                {
                    sb.AppendLine(Failure_Message);
                    continue;
                }

                foreach (var aid in animalAids)
                {
                    var aidToAdd = context.AnimalAids.First(a => a.Name == aid.Name);
                    if (aids.Contains(aidToAdd))
                    {
                        repeatedAid = true;
                        break;
                    }
                    aids.Add(aidToAdd);
                }

                if (repeatedAid)
                {
                    sb.AppendLine(Failure_Message);
                    continue;
                }

                var procedure = new Procedure
                {
                    Animal = animal,
                    Vet = vet,
                    DateTime = DateTime.ParseExact(proc.DateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                };

                foreach (var aid in aids)
                {
                    procedureAnimalAids.Add(new ProcedureAnimalAid
                    {
                        AnimalAid = aid,
                        Procedure = procedure
                    });
                }

                procedure.ProcedureAnimalAids = procedureAnimalAids;

                procedures.Add(procedure);

                sb.AppendLine("Record successfully imported.");
            }

            context.Procedures.AddRange(procedures);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new DataValidation.ValidationContext(obj);
            var validationResults = new List<DataValidation.ValidationResult>();

            return DataValidation.Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }

        private static bool IsPassportValid(PassportDto passport)
        {
            if (!IsValid(passport))
                return false;

            var ownerPhoneNumber = passport.OwnerPhoneNumber;

            return IsPhoneNumberValid(ownerPhoneNumber);
        }

        private static bool IsPhoneNumberValid(string phoneNumber)
        {
            var regexFirstPattern = @"(\+359)[0-9]{9}";
            var regexSecondPattern = @"(0)[0-9]{9}";

            bool firstTypePhoneNumber = Regex.IsMatch(phoneNumber, regexFirstPattern);
            bool secondTypePhoneNumber = Regex.IsMatch(phoneNumber, regexSecondPattern);

            return firstTypePhoneNumber || secondTypePhoneNumber;
        }
    }
}
