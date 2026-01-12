using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetMatch.Data;
using PetMatch.Models;
using Microsoft.AspNetCore.Hosting; 

namespace PetMatch.Pages.Animals
{
    public class EditModel : PageModel
    {
        private readonly PetMatchContext _context;
        private readonly IWebHostEnvironment _environment; 

        public EditModel(PetMatchContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;

        [BindProperty]
        public IFormFile? AnimalImage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Animal == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal.FirstOrDefaultAsync(m => m.ID == id);
            if (animal == null)
            {
                return NotFound();
            }
            Animal = animal;

            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "CategoryName");
            ViewData["ShelterID"] = new SelectList(_context.Shelter, "ID", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (AnimalImage != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "animals");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = AnimalImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await AnimalImage.CopyToAsync(fileStream);
                }

                Animal.ImageUrl = uniqueFileName;
            }
            else
            {
                var existingAnimal = await _context.Animal.AsNoTracking().FirstOrDefaultAsync(a => a.ID == Animal.ID);
                if (existingAnimal != null && string.IsNullOrEmpty(Animal.ImageUrl))
                {
                    Animal.ImageUrl = existingAnimal.ImageUrl;
                }
            }

            _context.Attach(Animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(Animal.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AnimalExists(int id)
        {
            return (_context.Animal?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}