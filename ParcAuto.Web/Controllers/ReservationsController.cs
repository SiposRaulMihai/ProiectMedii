using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]  // Allow access for mobile app during development
public class ReservationsController : ControllerBase
{
    private readonly ParcAutoContext _context;

    public ReservationsController(ParcAutoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
    {
        return await _context.Reservations
            .Include(r => r.Vehicle)
            .Include(r => r.Driver)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetReservation(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Vehicle)
            .Include(r => r.Driver)
            .FirstOrDefaultAsync(r => r.ReservationId == id);

        if (reservation == null)
        {
            return NotFound();
        }

        return reservation;
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> CreateReservation(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.ReservationId }, reservation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
    {
        if (id != reservation.ReservationId)
        {
            return BadRequest();
        }

        _context.Entry(reservation).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ReservationExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer,Identity.Application")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> ReservationExists(int id)
    {
        return await _context.Reservations.AnyAsync(e => e.ReservationId == id);
    }
}
