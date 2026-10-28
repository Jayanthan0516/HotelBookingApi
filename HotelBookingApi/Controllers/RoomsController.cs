
using HotelBookingApi.Data;
using HotelBookingApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApi.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRooms(
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut,
        [FromQuery] RoomType? type)
    {
        if (checkOut.Date <= checkIn.Date)
        {
            return BadRequest(new
            {
                error = "Check-out must be after check-in."
            });
        }

        var rooms = await _context.Rooms
            .Where(room => room.IsActive)
            .Where(room => type == null || room.Type == type)
            .Where(room => !_context.Bookings.Any(booking =>
                booking.RoomId == room.Id &&
                booking.Status == BookingStatus.Confirmed &&
                booking.CheckIn < checkOut &&
                booking.CheckOut > checkIn))
            .Select(room => new
            {
                room.Id,
                room.RoomNumber,
                Type = room.Type.ToString(),
                room.PricePerNight
            })
            .ToListAsync();

        return Ok(rooms);
    }
}
