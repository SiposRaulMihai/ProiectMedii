using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Pages.Maintenances
{
    public class DetailsModel : PageModel
    {
        private readonly ParcAuto.Web.Data.ParcAutoContext _context;

        public DetailsModel(ParcAuto.Web.Data.ParcAutoContext context)
        {
            _context = context;
        }

        public Maintenance Maintenance { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenance = await _context.Maintenances.FirstOrDefaultAsync(m => m.MaintenanceId == id);
            if (maintenance == null)
            {
                return NotFound();
            }
            else
            {
                Maintenance = maintenance;
            }
            return Page();
        }
    }
}
