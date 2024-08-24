using FormulaAirline.Models;
using FormulaAirline.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMessageProducer, MessageProducer>();

var app = builder.Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

// needed for Dapr pub/sub routing
app.MapSubscribeHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("api/v1/booking", (Booking newBooking, IMessageProducer producer) =>
{
    if (newBooking == null)
    {
        throw new ArgumentNullException(nameof(newBooking));
    }

    try
    {
        producer.PublishNewMessageAsync(newBooking);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Could not send the message: {ex} ");
    }

    return Results.Created($"api/v1/commands", newBooking);
});


app.Run();
