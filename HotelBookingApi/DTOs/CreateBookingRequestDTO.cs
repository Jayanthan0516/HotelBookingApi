
using System.ComponentModel.DataAnnotations;

namespace HotelBookingApi.DTOs;

public class CreateBookingRequest
{
    public int RoomId { get; set; }

    [Required]
    [StringLength(100)]
    public string GuestName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string GuestEmail { get; set; } = string.Empty;

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }
}
