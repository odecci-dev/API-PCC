using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Manager;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Dynamic.Core;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class PedigreeController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        public PedigreeController(PCC_DEVContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> view(int id)
        {
            var pedigreeTree = createPedigreeTree(id);
            return Ok(pedigreeTree);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> print(int id)
        {
            var animal = await _context.ABuffAnimals.FindAsync(id);

            var pedigreePrintResponse = new AnimalPedigreePrintResponse();

            var animalDetails = new AnimalDetails();
            animalDetails.DateOfRegistration = animal.CreatedDate;
            animalDetails.BreedRegistrationNumber = animal.breedRegistryNumber;
            animalDetails.HerdCode = animal.HerdCode;
            animalDetails.AnimalIdNumber = animal.AnimalIdNumber;
            animalDetails.Name = animal.AnimalName;
            animalDetails.Rfid = animal.RfidNumber;
            animalDetails.Sex = animal.Sex;
            animalDetails.Breed = animal.BreedCode;
            animalDetails.BloodComposition = animal.BloodCode;
            animalDetails.DateOfBirth = animal.DateOfBirth;
            animalDetails.CountryOfBirth = animal.CountryOfBirth;
            animalDetails.BirthType = animal.BirthType;

            var originOfAcquisition = populateOriginOfAcquistionModel(animal);

            if (originOfAcquisition != null)
            {
                animalDetails.OriginOfAcquisition = originOfAcquisition;
            }
 
            animalDetails.DateOfAcquisition = animal.DateOfAcquisition;
            animalDetails.TypeOfOWnership = animal.TypeOfOwnership;

            var herdDetails = new HerdDetails();
            var buffHerd = getHerdRecord(animal.HerdCode);
            if (buffHerd != null)
            {
                herdDetails.DateOfApplication = buffHerd.DateCreated;
                herdDetails.HerdName = buffHerd.HerdName;
                herdDetails.HerdType = buffHerd.HerdClassDesc;
                herdDetails.HerdSize = buffHerd.HerdSize;

                var buffaloTypeList = new List<string>();
                var feedingSystemList = new List<string>();
                if (buffHerd.buffaloType != null)
                {
                    foreach (HBuffaloType buffaloType in buffHerd.buffaloType)
                    {
                        buffaloTypeList.Add(buffaloType.BreedTypeCode);
                    }
                    herdDetails.TypeOfBuffalo = string.Join(",", buffaloTypeList);

                }
                if (buffHerd.feedingSystem != null)
                {
                    foreach (HFeedingSystem feedingSystem in buffHerd.feedingSystem)
                    {
                        feedingSystemList.Add(feedingSystem.FeedingSystemCode);
                    }
                    herdDetails.FeedingSystem = string.Join(",", feedingSystemList);
                }
                herdDetails.FarmManager = buffHerd.FarmManager;
                herdDetails.FarmAddress = buffHerd.FarmAddress;
            }
           

            pedigreePrintResponse.animalDetails = animalDetails;
            pedigreePrintResponse.herdDetails = herdDetails;
            pedigreePrintResponse.animalPedigree = createPedigreeTree(id);

            return Ok(pedigreePrintResponse);
        }


        private HBuffHerd getHerdRecord(string herdCode)
        {
            var buffHerd = _context.HBuffHerds
                                   .Include(herd => herd.buffaloType)
                                   .Include(herd => herd.feedingSystem)
                                   .Where(herd => herd.HerdCode.Equals(herdCode)).FirstOrDefault();
            return buffHerd;
        }

        private OriginOfAcquisitionModel populateOriginOfAcquistionModel(ABuffAnimal buffAnimal)
        {
            var originOfAcquisition = _context.OriginOfAcquisitionModels.Where(originOfAcquistion => originOfAcquistion.Id.Equals(buffAnimal.OriginOfAcquisition)).FirstOrDefault();
            if (originOfAcquisition == null)
            {
                return null;
            }
            var originOfAcquisitionModel = new OriginOfAcquisitionModel()
            {
                City = originOfAcquisition.City,
                Barangay = originOfAcquisition.Barangay,
                Province = originOfAcquisition.Province,
                Region = originOfAcquisition.Region
            };
            return originOfAcquisitionModel;

        }

        private AnimalPedigreeTree<AnimalPedigreeModel> createPedigreeTree(int id)
        {
            var animal = _context.ABuffAnimals.Find(id);
            var root = new AnimalPedigreeTree<AnimalPedigreeModel>();

            if (animal != null)
            {
                Node<AnimalPedigreeModel> rootNode = new Node<AnimalPedigreeModel>(convertToAnimalPedigreeModel(animal));
                rootNode.level = 0;
                root.Add(rootNode);

                generatePedigree(rootNode, animal);
            }
            return root;
        }

        private void generatePedigree(Node<AnimalPedigreeModel> parentNode, ABuffAnimal buffAnimal)
        {
            if (parentNode.level == 3)
            {
                return;
            }

            ABuffAnimal sire = new ABuffAnimal();
            ABuffAnimal dam = new ABuffAnimal();

            var sireToAnimalModel = convertToAnimalPedigreeModel(sire);
            var damToAnimalModel = convertToAnimalPedigreeModel(dam);

            if (sireToAnimalModel != null)
            {
                Node<AnimalPedigreeModel> sireNode = new Node<AnimalPedigreeModel>(convertToAnimalPedigreeModel(sire));
                sireNode.level = parentNode.level + 1;
                parentNode.AddSire(sireNode);
                generatePedigree(sireNode, sire);
            }

            if (damToAnimalModel != null)
            {
                Node<AnimalPedigreeModel> damNode = new Node<AnimalPedigreeModel>(convertToAnimalPedigreeModel(dam));
                damNode.level = parentNode.level + 1;
                parentNode.AddDam(damNode);
                generatePedigree(damNode, dam);
            }
        }
        private AnimalPedigreeModel convertToAnimalPedigreeModel(ABuffAnimal buffAnimal)
        {
            if (buffAnimal != null)
            {
                var animalPedigreeModel = new AnimalPedigreeModel()
                {
                    RegistrationNumber = buffAnimal.breedRegistryNumber,
                    Photo = buffAnimal.Photo,
                    Name = buffAnimal.AnimalName,
                    DateOfBirth = buffAnimal.DateOfBirth,
                    PlaceOfBirth = buffAnimal.CountryOfBirth
                };
                return animalPedigreeModel;
            }

            return null;
        }

    }
}
