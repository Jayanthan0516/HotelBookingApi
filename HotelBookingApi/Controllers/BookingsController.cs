using HotelBookingApi.Entities;
using HotelBookingApi.DTOs;
using HotelBookingApi.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApi.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost(Name = "AddBooking")]
    [EndpointSummary("Add Booking")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            var booking = await _bookingService.CreateBookingAsync(request);

            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = booking.Id },
                booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpGet("{id:int}",Name = "GetBookingsById")]
    [EndpointSummary("Get Booking by ID")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);

        if (booking == null)
        {
            return NotFound(new { error = "Booking not found." });
        }

        return Ok(booking);
    }

    [HttpGet(Name = "ListBookings")]
    [EndpointSummary("List Bookings")]
    public async Task<IActionResult> GetBookings([FromQuery] BookingStatus? status,[FromQuery] string? guestEmail,[FromQuery] int page = 1,[FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _bookingService.GetBookingsAsync(status, guestEmail, page, pageSize);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:int}/cancel",Name = "CancelBooking")]
    [EndpointSummary("Cancel Booking")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        try
        {
            var booking = await _bookingService.CancelBookingAsync(id);

            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

}
