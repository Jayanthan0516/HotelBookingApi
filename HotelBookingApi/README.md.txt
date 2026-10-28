Hotel Room Booking API

A RESTful API built using C#, ASP.NET Core .NET 8, Entity Framework Core, and SQLite for managing hotel room availability, bookings, and cancellations.

Technologies Used

- C# and ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- xUnit

How to Run the API

Prerequisites: .NET 8 SDK installed.

1. Clone the repository and open the solution directory.
2. Restore the dependencies:

   ```bash
   dotnet restore
   ```

3. Run the API:

   ```bash
   dotnet run --project HotelBookingApi/HotelBookingApi.csproj
   ```

4. Open the Swagger URL displayed in the console to explore and test the API endpoints.

## How to Run the Tests

Run the following command from the repository root:

```bash
dotnet test HotelBookingApi.Tests/HotelBookingApi.Tests.csproj
```

The xUnit test suite currently contains five tests covering booking date validation, overlapping bookings, maximum stay duration, and the 24-hour cancellation rule.

What I Would Improve With More Time

- Add more unit tests for successful bookings, invalid email addresses, inactive rooms, and filtering and pagination.
- Improve concurrency handling to prevent simultaneous conflicting bookings.
- Add structured logging and more detailed Swagger response documentation.
- Improve production configuration and database migration management.

