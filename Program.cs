using EventSourcedBankAccountManagement.Applications.Handlers;
using EventSourcedBankAccountManagement.Infrastructure.EventStore;
using EventSourcedBankAccountManagement.Infrastructure.Projections;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
builder.Services.AddSingleton<AccountBalanceProjection>();
builder.Services.AddSingleton<ProjectionWorker>();
builder.Services.AddTransient<OpenAccountHandler>();
builder.Services.AddTransient<DipositMoneyHandler>();
builder.Services.AddTransient<WithdarwMoneyHandler>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Launch projection worker
var worker = app.Services.GetRequiredService<ProjectionWorker>();
var cts = new CancellationTokenSource();
_ = Task.Run(() => worker.Start(cts.Token));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
