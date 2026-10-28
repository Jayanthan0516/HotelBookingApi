
namespace HotelBookingApi.Entities;

public class Room
{
    public int Id { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public RoomType Type { get; set; }

    public decimal PricePerNight { get; set; }

    public bool IsActive { get; set; } = true;
}
