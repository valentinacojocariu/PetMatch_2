using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMatch.Data;
using PetMatch.Models;

namespace PetMatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdoptionApiController : ControllerBase
    {
        private readonly PetMatchContext _context;

        public AdoptionApiController(PetMatchContext context)
        {
            _context = context;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SubmitRequest([FromBody] AdoptionRequestDto request)
        {
            if (request == null) return BadRequest();

            // 1. Căutăm Membrul după email (În tabelul Member, nu Users)
            // Sper că ai coloana 'Email' în clasa Member!
            var member = await _context.Member.FirstOrDefaultAsync(m => m.Email == request.UserEmail);

            if (member == null)
            {
                return BadRequest("Nu am găsit un membru cu acest email. Asigură-te că ai datele completate în profil.");
            }

            // 2. Căutăm Animalul folosind ID (mare)
            var animal = await _context.Animal.FirstOrDefaultAsync(a => a.ID == request.AnimalID);

            if (animal == null) return BadRequest("Animalul nu există.");

            // 3. Creăm cererea folosind numele tale (AnimalID, MemberID)
            var newRequest = new AdoptionRequest
            {
                MemberID = member.ID,      // <--- Aici era problema (MemberID vs UserId)
                AnimalID = animal.ID,      // <--- Aici era problema (AnimalID vs AnimalId)
                RequestDate = DateTime.Now,
                Status = "Pending",
                Message = "Cerere din aplicatia mobila"
            };

            _context.AdoptionRequest.Add(newRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Succes!" });
        }
    }
}