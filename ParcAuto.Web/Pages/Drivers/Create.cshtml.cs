using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Pages.Drivers
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
            return Page();
        }

        [BindProperty]
        public Driver Driver { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Drivers.Add(Driver);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
