
namespace HotelBookingApi.DTOs;

public class BookingResponse
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public string GuestName { get; set; } = string.Empty;

    public string GuestEmail { get; set; } = string.Empty;

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }
}
