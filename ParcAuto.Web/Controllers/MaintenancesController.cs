using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]  // Allow access for mobile app during development
public class MaintenancesController : ControllerBase
{
    private readonly ParcAutoContext _context;

    public MaintenancesController(ParcAutoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Maintenance>>> GetMaintenances()
    {
        return await _context.Maintenances
            .Include(m => m.Vehicle)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Maintenance>> GetMaintenance(int id)
    {
        var maintenance = await _context.Maintenances
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.MaintenanceId == id);

        if (maintenance == null)
        {
            return NotFound();
        }

        return maintenance;
    }

    [HttpPost]
    public async Task<ActionResult<Maintenance>> CreateMaintenance(Maintenance maintenance)
    {
        _context.Maintenances.Add(maintenance);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMaintenance), new { id = maintenance.MaintenanceId }, maintenance);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMaintenance(int id, Maintenance maintenance)
    {
        if (id != maintenance.MaintenanceId)
        {
            return BadRequest();
        }

        _context.Entry(maintenance).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await MaintenanceExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer,Identity.Application")]
    public async Task<IActionResult> DeleteMaintenance(int id)
    {
        var maintenance = await _context.Maintenances.FindAsync(id);
        if (maintenance == null)
        {
            return NotFound();
        }

        _context.Maintenances.Remove(maintenance);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> MaintenanceExists(int id)
    {
        return await _context.Maintenances.AnyAsync(e => e.MaintenanceId == id);
    }
}
