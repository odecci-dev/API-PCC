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
            var animal = await _context.ABuffAnimals.FindAsync(id);
            var root = new AnimalPedigreeTree<AnimalPedigreeModel>();

            if (animal != null )
            {
                Node<AnimalPedigreeModel> rootNode = new Node<AnimalPedigreeModel>(convertToAnimalPedigreeModel(animal));
                rootNode.level = 0;
                root.Add(rootNode);

                generatePedigree(rootNode, animal);
            }
            return Ok(root);
        }

        private AnimalPedigreeModel convertToAnimalPedigreeModel(ABuffAnimal buffAnimal)
        {
            if (buffAnimal != null) {
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

        [HttpGet("{id}")]
        public async Task<ActionResult> print(int id)
        {
            return Ok();
        }

        private void generatePedigree(Node<AnimalPedigreeModel> parentNode, ABuffAnimal buffAnimal)
        {
            if (parentNode.level == 3)
            {
                return;
            }
            var sire = _context.ABuffAnimals.Find(buffAnimal.SireId);
            var dam =  _context.ABuffAnimals.Find(buffAnimal.DamId);

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

    }
}
