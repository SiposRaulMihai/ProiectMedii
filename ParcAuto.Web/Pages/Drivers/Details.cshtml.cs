using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Pages.Drivers
{
    public class DetailsModel : PageModel
    {
        private readonly ParcAuto.Web.Data.ParcAutoContext _context;

        public DetailsModel(ParcAuto.Web.Data.ParcAutoContext context)
        {
            _context = context;
        }

        public Driver Driver { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers.FirstOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }
            else
            {
                Driver = driver;
            }
            return Page();
        }
    }
}
