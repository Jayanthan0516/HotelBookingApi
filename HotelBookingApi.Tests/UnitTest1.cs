
using HotelBookingApi.Data;
using HotelBookingApi.DTOs;
using HotelBookingApi.Entities;
using HotelBookingApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApi.Tests;

public class UnitTest1
{
    [Fact]
    public async Task CreateBookingAsync_WhenDatesOverlap_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var room = new Room
        {
            Id = 1,
            RoomNumber = "101",
            Type = RoomType.Single,
            PricePerNight = 1500,
            IsActive = true
        };

        context.Rooms.Add(room);

        context.Bookings.Add(new Booking
        {
            RoomId = 1,
            GuestName = "Existing Guest",
            GuestEmail = "existing@example.com",
            CheckIn = new DateTime(2026, 11, 1),
            CheckOut = new DateTime(2026, 11, 5),
            Status = BookingStatus.Confirmed,
            TotalPrice = 6000,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var request = new CreateBookingRequest
        {
            RoomId = 1,
            GuestName = "New Guest",
            GuestEmail = "new@example.com",
            CheckIn = new DateTime(2026, 11, 3),
            CheckOut = new DateTime(2026, 11, 7)
        };

        // Act and Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateBookingAsync(request));
    }

    [Fact]
    public async Task CancelBookingAsync_WhenCheckInIsWithin24Hours_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var room = new Room
        {
            Id = 1,
            RoomNumber = "101",
            Type = RoomType.Single,
            PricePerNight = 1500,
            IsActive = true
        };

        context.Rooms.Add(room);

        var booking = new Booking
        {
            RoomId = 1,
            GuestName = "Test Guest",
            GuestEmail = "test@example.com",
            CheckIn = DateTime.UtcNow.AddHours(12),
            CheckOut = DateTime.UtcNow.AddDays(2),
            Status = BookingStatus.Confirmed,
            TotalPrice = 3000,
            CreatedAt = DateTime.UtcNow
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CancelBookingAsync(booking.Id));
    }

    [Fact]
    public async Task CreateBookingAsync_WhenCheckOutIsSameAsCheckIn_ThrowsArgumentException()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var service = new BookingService(context);

        var checkIn = DateTime.UtcNow.Date.AddDays(2);

        var request = new CreateBookingRequest
        {
            RoomId = 1,
            GuestName = "Test Guest",
            GuestEmail = "test@example.com",
            CheckIn = checkIn,
            CheckOut = checkIn
        };

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateBookingAsync(request));
    }

    [Fact]
    public async Task CreateBookingAsync_WhenCheckInIsInThePast_ThrowsArgumentException()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var service = new BookingService(context);

        var request = new CreateBookingRequest
        {
            RoomId = 1,
            GuestName = "Test Guest",
            GuestEmail = "test@example.com",
            CheckIn = DateTime.UtcNow.Date.AddDays(-1),
            CheckOut = DateTime.UtcNow.Date.AddDays(1)
        };

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateBookingAsync(request));
    }

    [Fact]
    public async Task CreateBookingAsync_WhenStayExceeds30Nights_ThrowsArgumentException()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var service = new BookingService(context);

        var checkIn = DateTime.UtcNow.Date.AddDays(2);

        var request = new CreateBookingRequest
        {
            RoomId = 1,
            GuestName = "Test Guest",
            GuestEmail = "test@example.com",
            CheckIn = checkIn,
            CheckOut = checkIn.AddDays(31)
        };

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateBookingAsync(request));
    }

}
