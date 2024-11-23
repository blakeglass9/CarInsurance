using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarInsurance.Data;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsureeController(ApplicationDbContext context)
        {
            _context = context;
        }
  
        // GET: Insuree
        public async Task<IActionResult> Index()
        {
            return View(await _context.Insurees.ToListAsync());
        }

        // GET: Insuree/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // Calculate the quote
                insuree.Quote = CalculateQuote(insuree);

                // Save the data to the database
                _context.Add(insuree);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(insuree);
        }

        // GET: Insuree/Admin
        public async Task<IActionResult> Admin()
        {
            var insurees = await _context.Insurees.ToListAsync();
            return View(insurees);
        }

        // Private method to calculate the insurance quote
        private decimal CalculateQuote(Insuree insuree)
        {
            decimal quote = 50m; // Base price

            // Calculate age
            int age = DateTime.Now.Year - insuree.DateOfBirth.Year;
            if (insuree.DateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;

            // Age-based adjustments
            if (age <= 18)
            {
                quote += 100m;
            }
            else if (age >= 19 && age <= 25)
            {
                quote += 50m;
            }
            else
            {
                quote += 25m;
            }

            // Car year adjustments
            if (insuree.CarYear < 2000)
            {
                quote += 25m;
            }
            if (insuree.CarYear > 2015)
            {
                quote += 25m;
            }

            // Car make and model adjustments
            if (insuree.CarMake.ToLower() == "porsche")
            {
                quote += 25m;
                if (insuree.CarModel.ToLower() == "911 carrera")
                {
                    quote += 25m;
                }
            }

            // Speeding tickets adjustment
            quote += insuree.SpeedingTickets * 10m;

            // DUI adjustment
            if (insuree.DUI)
            {
                quote *= 1.25m; // Add 25%
            }

            // Coverage type adjustment
            if (insuree.CoverageType)
            {
                quote *= 1.50m; // Add 50%
            }

            return quote;
        }
    }
}
