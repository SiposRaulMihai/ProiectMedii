using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]  // Allow access for mobile app during development
public class DriversController : ControllerBase
{
    private readonly ParcAutoContext _context;

    public DriversController(ParcAutoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
    {
        return await _context.Drivers.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Driver>> GetDriver(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null)
        {
            return NotFound();
        }
        return driver;
    }

    [HttpPost]
    public async Task<ActionResult<Driver>> CreateDriver(Driver driver)
    {
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDriver), new { id = driver.DriverId }, driver);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDriver(int id, Driver driver)
    {
        if (id != driver.DriverId)
        {
            return BadRequest();
        }

        _context.Entry(driver).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await DriverExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer,Identity.Application")]
    public async Task<IActionResult> DeleteDriver(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null)
        {
            return NotFound();
        }

        _context.Drivers.Remove(driver);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> DriverExists(int id)
    {
        return await _context.Drivers.AnyAsync(e => e.DriverId == id);
    }
}
