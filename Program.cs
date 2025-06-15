using EventSourcedBankAccountManagement;
using EventSourcedBankAccountManagement.Infrastructure.Projections;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterEventsSourcingServices();


var app = builder.Build();


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
