using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMatch.Data;
using PetMatch.Models;

namespace PetMatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdoptionController : ControllerBase
    {
        private readonly PetMatchContext _context;

        public AdoptionController(PetMatchContext context)
        {
            _context = context;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestAdoption([FromBody] AdoptionRequestDto request)
        {

            return Ok(new { message = "Cererea a fost trimisă către admin!" });
        }

        [HttpGet("myrequests")]
        public async Task<IActionResult> GetMyRequests(string email)
        {
            var mockRequests = new List<object>
            {
                new { AnimalName = "Ricky", Status = "Aprobat", Message = "Vino să îl iei mâine!" }
            };
            return Ok(mockRequests);
        }
    }

}