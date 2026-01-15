using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;
using Microsoft.EntityFrameworkCore;


namespace ParcAuto.Web.Pages.Reservations
{
    public class CreateModel : PageModel
    {
        private readonly ParcAuto.Web.Data.ParcAutoContext _context;

        public CreateModel(ParcAuto.Web.Data.ParcAutoContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["DriverId"] = new SelectList(_context.Drivers, "DriverId", "Email");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "LicensePlate");
            return Page();
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (Reservation.EndDate <= Reservation.StartDate)
            {
                ModelState.AddModelError(string.Empty, "End Date must be after Start Date.");
            }
            bool overlaps = await _context.Reservations.AnyAsync(r =>
                r.VehicleId == Reservation.VehicleId &&
                r.StartDate < Reservation.EndDate &&
                Reservation.StartDate < r.EndDate);

            if (overlaps)
            {
                ModelState.AddModelError(string.Empty, "This vehicle is already reserved in the selected period.");
            }

            if (!ModelState.IsValid)
            {
                // Reload dropdowns if validation fails
                ViewData["DriverId"] = new SelectList(_context.Drivers, "DriverId", "Email");
                ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "LicensePlate");
                return Page();
            }

            _context.Reservations.Add(Reservation);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
