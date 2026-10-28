
using HotelBookingApi.Data;
using HotelBookingApi.DTOs;
using HotelBookingApi.Entities;
using HotelBookingApi.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApi.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BookingResponse> CreateBookingAsync(
        CreateBookingRequest request)
    {
        // 1. Validate check-in and check-out dates
        if (request.CheckOut.Date <= request.CheckIn.Date)
        {
            throw new ArgumentException(
                "Check-out must be after check-in.");
        }

        // 2. Check-in must not be in the past
        if (request.CheckIn.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException(
                "Check-in cannot be in the past.");
        }

        // 3. Stay must not exceed 30 nights
        var nights = (request.CheckOut.Date - request.CheckIn.Date).Days;

        if (nights > 30)
        {
            throw new ArgumentException(
                "Stay cannot exceed 30 nights.");
        }

        // 4. Validate guest details
        if (string.IsNullOrWhiteSpace(request.GuestName))
        {
            throw new ArgumentException(
                "Guest name is required.");
        }

        if (!IsValidEmail(request.GuestEmail))
        {
            throw new ArgumentException(
                "A valid guest email is required.");
        }

        // 5. Find the room and verify it is active
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == request.RoomId);

        if (room == null || !room.IsActive)
        {
            throw new KeyNotFoundException(
                "Room does not exist or is inactive.");
        }

        // 6. Check for overlapping confirmed bookings
        var hasOverlap = await _context.Bookings.AnyAsync(b =>
            b.RoomId == request.RoomId &&
            b.Status == BookingStatus.Confirmed &&
            b.CheckIn < request.CheckOut &&
            b.CheckOut > request.CheckIn);

        if (hasOverlap)
        {
            throw new InvalidOperationException(
                "Room is already booked for the selected dates.");
        }

        // 7. Create and save the booking
        var booking = new Booking
        {
            RoomId = room.Id,
            GuestName = request.GuestName.Trim(),
            GuestEmail = request.GuestEmail.Trim(),
            CheckIn = request.CheckIn.Date,
            CheckOut = request.CheckOut.Date,
            Status = BookingStatus.Confirmed,
            TotalPrice = nights * room.PricePerNight,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // 8. Return a response DTO
        return new BookingResponse
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            GuestName = booking.GuestName,
            GuestEmail = booking.GuestEmail,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            Status = booking.Status.ToString(),
            TotalPrice = booking.TotalPrice,
            CreatedAt = booking.CreatedAt
        };
    }

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
               new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                   .IsValid(email.Trim());
    }

    public async Task<BookingResponse?> GetBookingByIdAsync(int id)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return null;
        }

        return new BookingResponse
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            GuestName = booking.GuestName,
            GuestEmail = booking.GuestEmail,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            Status = booking.Status.ToString(),
            TotalPrice = booking.TotalPrice,
            CreatedAt = booking.CreatedAt
        };
    }

    public async Task<PagedResponse<BookingResponse>> GetBookingsAsync(BookingStatus? status,string? guestEmail,int page,int pageSize)
    {
        if (page < 1)
        {
            throw new ArgumentException("Page must be at least 1.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentException(
                "Page size must be between 1 and 100.");
        }

        var query = _context.Bookings.AsNoTracking().AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(guestEmail))
        {
            var email = guestEmail.Trim();

            query = query.Where(b => b.GuestEmail == email);
        }

        var totalCount = await query.CountAsync();

        var bookings = await query
            .OrderBy(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookingResponse
            {
                Id = b.Id,
                RoomId = b.RoomId,
                GuestName = b.GuestName,
                GuestEmail = b.GuestEmail,
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut,
                Status = b.Status.ToString(),
                TotalPrice = b.TotalPrice,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return new PagedResponse<BookingResponse>
        {
            Items = bookings,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BookingResponse> CancelBookingAsync(int id)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Booking is already cancelled.");
        }

        var hoursUntilCheckIn = (booking.CheckIn - DateTime.UtcNow).TotalHours;

        if (hoursUntilCheckIn < 24)
        {
            throw new InvalidOperationException(
                "Booking cannot be cancelled less than 24 hours before check-in.");
        }

        booking.Status = BookingStatus.Cancelled;

        await _context.SaveChangesAsync();

        return new BookingResponse
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            GuestName = booking.GuestName,
            GuestEmail = booking.GuestEmail,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            Status = booking.Status.ToString(),
            TotalPrice = booking.TotalPrice,
            CreatedAt = booking.CreatedAt
        };
    }


}
