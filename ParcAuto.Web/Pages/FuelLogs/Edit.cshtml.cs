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

namespace ParcAuto.Web.Pages.FuelLogs
{
    public class EditModel : PageModel
    {
        private readonly ParcAuto.Web.Data.ParcAutoContext _context;

        public EditModel(ParcAuto.Web.Data.ParcAutoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FuelLog FuelLog { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fuellog =  await _context.FuelLogs.FirstOrDefaultAsync(m => m.FuelLogId == id);
            if (fuellog == null)
            {
                return NotFound();
            }
            FuelLog = fuellog;
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "LicensePlate");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload the vehicle list if validation fails
                ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "LicensePlate");
                return Page();
            }

            _context.Attach(FuelLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FuelLogExists(FuelLog.FuelLogId))
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

        private bool FuelLogExists(int id)
        {
            return _context.FuelLogs.Any(e => e.FuelLogId == id);
        }
    }
}
