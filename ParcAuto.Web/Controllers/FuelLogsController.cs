using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]  // Allow access for mobile app during development
public class FuelLogsController : ControllerBase
{
    private readonly ParcAutoContext _context;

    public FuelLogsController(ParcAutoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FuelLog>>> GetFuelLogs()
    {
        return await _context.FuelLogs
            .Include(f => f.Vehicle)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FuelLog>> GetFuelLog(int id)
    {
        var fuelLog = await _context.FuelLogs
            .Include(f => f.Vehicle)
            .FirstOrDefaultAsync(f => f.FuelLogId == id);

        if (fuelLog == null)
        {
            return NotFound();
        }

        return fuelLog;
    }

    [HttpPost]
    public async Task<ActionResult<FuelLog>> CreateFuelLog(FuelLog fuelLog)
    {
        _context.FuelLogs.Add(fuelLog);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetFuelLog), new { id = fuelLog.FuelLogId }, fuelLog);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFuelLog(int id, FuelLog fuelLog)
    {
        if (id != fuelLog.FuelLogId)
        {
            return BadRequest();
        }

        _context.Entry(fuelLog).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await FuelLogExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer,Identity.Application")]
    public async Task<IActionResult> DeleteFuelLog(int id)
    {
        var fuelLog = await _context.FuelLogs.FindAsync(id);
        if (fuelLog == null)
        {
            return NotFound();
        }

        _context.FuelLogs.Remove(fuelLog);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> FuelLogExists(int id)
    {
        return await _context.FuelLogs.AnyAsync(e => e.FuelLogId == id);
    }
}
