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
    public class IndexModel : PageModel
    {
        private readonly ParcAuto.Web.Data.ParcAutoContext _context;

        public IndexModel(ParcAuto.Web.Data.ParcAutoContext context)
        {
            _context = context;
        }

        public IList<FuelLog> FuelLog { get;set; } = default!;

        public async Task OnGetAsync()
        {
            FuelLog = await _context.FuelLogs
                .Include(f => f.Vehicle).ToListAsync();
        }
    }
}
