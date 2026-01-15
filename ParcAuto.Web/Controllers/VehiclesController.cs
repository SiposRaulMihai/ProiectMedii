using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]  // Allow access for mobile app during development
public class VehiclesController : ControllerBase
{
    private readonly ParcAutoContext _context;

    public VehiclesController(ParcAutoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
    {
        return await _context.Vehicles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetVehicle(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }
        return vehicle;
    }

    [HttpPost]
    public async Task<ActionResult<Vehicle>> CreateVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.VehicleId }, vehicle);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, Vehicle vehicle)
    {
        if (id != vehicle.VehicleId)
        {
            return BadRequest();
        }

        _context.Entry(vehicle).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await VehicleExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer,Identity.Application")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetAvailableVehicles([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = _context.Vehicles.Where(v => v.Status == "Available");

        if (startDate.HasValue && endDate.HasValue)
        {
            var reservedVehicleIds = await _context.Reservations
                .Where(r => r.StartDate < endDate && r.EndDate > startDate)
                .Select(r => r.VehicleId)
                .Distinct()
                .ToListAsync();

            query = query.Where(v => !reservedVehicleIds.Contains(v.VehicleId));
        }

        return await query.ToListAsync();
    }

    private async Task<bool> VehicleExists(int id)
    {
        return await _context.Vehicles.AnyAsync(e => e.VehicleId == id);
    }
}
