
using HotelBookingApi.DTOs;
using HotelBookingApi.Entities;

namespace HotelBookingApi.Services.IServices;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(
        CreateBookingRequest request);

    Task<BookingResponse?> GetBookingByIdAsync(int id);
    Task<PagedResponse<BookingResponse>> GetBookingsAsync(BookingStatus? status,string? guestEmail,int page,int pageSize);
    Task<BookingResponse> CancelBookingAsync(int id);
}
