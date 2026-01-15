using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Pages.Reservations
{
    public class IndexModel : PageModel
    {
        private readonly ParcAuto.Web.Data.ParcAutoContext _context;

        public IndexModel(ParcAuto.Web.Data.ParcAutoContext context)
        {
            _context = context;
        }

        public IList<Reservation> Reservation { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Reservation = await _context.Reservations
                .Include(r => r.Driver)
                .Include(r => r.Vehicle).ToListAsync();
        }
    }
}
