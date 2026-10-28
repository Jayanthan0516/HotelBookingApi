
using HotelBookingApi.Entities;

namespace HotelBookingApi.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Rooms.Any())
        {
            return;
        }

        var rooms = new List<Room>
        {
            new() { RoomNumber = "101", Type = RoomType.Single, PricePerNight = 1500m },
            new() { RoomNumber = "102", Type = RoomType.Single, PricePerNight = 1500m },
            new() { RoomNumber = "103", Type = RoomType.Double, PricePerNight = 2500m },
            new() { RoomNumber = "104", Type = RoomType.Double, PricePerNight = 2500m },
            new() { RoomNumber = "105", Type = RoomType.Double, PricePerNight = 2800m },
            new() { RoomNumber = "201", Type = RoomType.Suite, PricePerNight = 5000m },
            new() { RoomNumber = "202", Type = RoomType.Suite, PricePerNight = 5500m },
            new() { RoomNumber = "203", Type = RoomType.Single, PricePerNight = 1600m },
            new() { RoomNumber = "204", Type = RoomType.Double, PricePerNight = 2700m },
            new() { RoomNumber = "205", Type = RoomType.Suite, PricePerNight = 6000m }
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();
    }
}
