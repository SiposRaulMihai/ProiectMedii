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

namespace ParcAuto.Web.Pages.Maintenances
{
    public class EditModel : PageModel
    {
        private readonly ParcAuto.Web.Data.ParcAutoContext _context;

        public EditModel(ParcAuto.Web.Data.ParcAutoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Maintenance Maintenance { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenance =  await _context.Maintenances.FirstOrDefaultAsync(m => m.MaintenanceId == id);
            if (maintenance == null)
            {
                return NotFound();
            }
            Maintenance = maintenance;
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

            _context.Attach(Maintenance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceExists(Maintenance.MaintenanceId))
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

        private bool MaintenanceExists(int id)
        {
            return _context.Maintenances.Any(e => e.MaintenanceId == id);
        }
    }
}
