using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Pages.FuelLogs
{
    public class DetailsModel : PageModel
    {
        private readonly ParcAuto.Web.Data.ParcAutoContext _context;

        public DetailsModel(ParcAuto.Web.Data.ParcAutoContext context)
        {
            _context = context;
        }

        public FuelLog FuelLog { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fuellog = await _context.FuelLogs.FirstOrDefaultAsync(m => m.FuelLogId == id);
            if (fuellog == null)
            {
                return NotFound();
            }
            else
            {
                FuelLog = fuellog;
            }
            return Page();
        }
    }
}
