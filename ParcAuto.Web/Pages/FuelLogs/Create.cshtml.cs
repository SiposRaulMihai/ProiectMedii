using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Pages.FuelLogs
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
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "LicensePlate");
            return Page();
        }

        [BindProperty]
        public FuelLog FuelLog { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload the vehicle list if validation fails
                ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "LicensePlate");
                return Page();
            }

            _context.FuelLogs.Add(FuelLog);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
