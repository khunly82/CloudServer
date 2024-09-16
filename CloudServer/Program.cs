using CloudServer.Hubs;
using CloudServer.Infrastructure;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// récupération de la config Mqtt
var mqttConfig = builder.Configuration.GetSection("Mqtt").Get<MqttConnection.Configuration>();

builder.Services.AddSingleton(mqttConfig ?? throw new Exception("Mqtt config is missing"));

var connection = new MqttConnection(mqttConfig);

builder.Services.AddSingleton(connection);

builder.Services.AddCors(c => c.AddDefaultPolicy(options =>
{
    options.AllowAnyMethod();
    options.WithOrigins("http://localhost:4200");
    options.AllowCredentials();
    options.AllowAnyHeader();
}));

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapHub<IOTHub>("ws/iot", 
    o => o.Transports = HttpTransportType.WebSockets
);


// S'abonner au broker
// A chaque message sur le topic test du broker 
connection.SubscribeAsync("test", payload =>
{
    // Appeler la methode send hub SignalR
    IHubContext<IOTHub> context = app.Services.CreateScope().ServiceProvider.GetService<IHubContext<IOTHub>>();

    context.Clients.All.SendAsync("test", payload);
});

app.Run();

