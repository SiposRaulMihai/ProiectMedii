using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Pages.Reservations
{
    public class EditModel : PageModel
    {
        private readonly ParcAuto.Web.Data.ParcAutoContext _context;

        public EditModel(ParcAuto.Web.Data.ParcAutoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation =  await _context.Reservations.FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }
            Reservation = reservation;
            ViewData["DriverId"] = new SelectList(_context.Drivers, "DriverId", "Email");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "LicensePlate");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns if validation fails
                ViewData["DriverId"] = new SelectList(_context.Drivers, "DriverId", "Email");
                ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "LicensePlate");
                return Page();
            }

            _context.Attach(Reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(Reservation.ReservationId))
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

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
