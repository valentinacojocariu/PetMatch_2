using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMatch.Data;
using PetMatch.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetMatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly PetMatchContext _context;

        public AnimalsController(PetMatchContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAnimals()
        {
            var animals = await _context.Animal
                .Select(a => new
                {
                    ID = a.ID,
                    Name = a.Name,
                    ImageUrl = a.ImageUrl,
                    Breed = a.Breed,
                    Age = a.Age,
                    Description = a.Description,
                    IsAdopted = a.IsAdopted,
                    RescueDate = a.RescueDate,

                    CategoryName = a.Category != null ? a.Category.Name : "Necunoscut",

                    ShelterName = a.Shelter != null ? a.Shelter.Name : "Fără Adăpost",

                    CategoryID = a.CategoryID,
                    ShelterID = a.ShelterID
                })
                .ToListAsync();

            return Ok(animals);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _context.Animal.FindAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            return animal;
        }

        [HttpPost]
        public async Task<ActionResult<Animal>> PostAnimal(Animal animal)
        {
            _context.Animal.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnimal", new { id = animal.ID }, animal);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _context.Animal.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            _context.Animal.Remove(animal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimal(int id, Animal animal)
        {
            if (id != animal.ID)
            {
                return BadRequest();
            }

            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool AnimalExists(int id)
        {
            return _context.Animal.Any(e => e.ID == id);
        }
    }
}