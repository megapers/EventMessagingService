using System.Text.Json;
using Dapr;
using FormulaAirline.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();

// Dapr subscription in [Topic] routes bookings topic to this route
app.MapPost("/bookings", [Topic("bookings_pubsub", "bookings")] (Booking booking) =>
{
    try
    {
        // Log the received booking object details
        Console.WriteLine($"Subscriber received: Id={booking.Id}, PassengerName={booking.PassengerName}, PassportNb={booking.PassportNb}, From={booking.From}, To={booking.To}, Status={booking.Status}");

        return Results.Ok(booking);
    }
    catch (JsonException jsonEx)
    {
        // Handle JSON deserialization errors
        Console.WriteLine($"JSON deserialization error: {jsonEx.Message}");
        return Results.BadRequest("Invalid JSON format.");
    }
    catch (Exception ex)
    {
        // Handle any other errors
        Console.WriteLine($"Unexpected error: {ex.Message}");
        return Results.StatusCode(500);
    }
});

await app.RunAsync();
