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

            var member = await _context.Member.FirstOrDefaultAsync(m => m.Email == request.UserEmail);

            if (member == null)
            {
                member = new Member
                {
                    Email = request.UserEmail,
                    FullName = "Utilizator Mobil"
                };
                _context.Member.Add(member);
                await _context.SaveChangesAsync();
            }

            var animal = await _context.Animal.FirstOrDefaultAsync(a => a.ID == request.AnimalID);
            if (animal == null) return BadRequest("Animalul nu mai este disponibil.");

            var newRequest = new AdoptionRequest
            {
                MemberID = member.ID,
                AnimalID = animal.ID,
                RequestDate = DateTime.Now,
                Status = "Pending",
                Message = "Cerere trimisă din aplicația mobilă"
            };

            _context.AdoptionRequest.Add(newRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Succes!" });
        }
        [HttpGet("check-status")]
        public async Task<IActionResult> CheckStatus(string email)
        {
            var member = await _context.Member.FirstOrDefaultAsync(m => m.Email == email);
            if (member == null) return Ok(new { hasUpdate = false });

            var request = await _context.AdoptionRequest
                .Include(r => r.Animal)
                .Where(r => r.MemberID == member.ID && (r.Status == "Approved" || r.Status == "Acceptat"))
                .OrderByDescending(r => r.RequestDate)
                .FirstOrDefaultAsync();

            if (request != null)
            {
                string animalName = request.Animal != null ? request.Animal.Name : "animalul";

                return Ok(new
                {
                    hasUpdate = true,
                    requestId = request.ID, 
                    message = $"Felicitări! Cererea pentru {animalName} a fost acceptată! Vino la adăpost."
                });
            }

            return Ok(new { hasUpdate = false });
        }
    }
}