using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using tp24TT.Data;
using tp24TT.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Tp24ttContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("tp24TTConnection")));


builder.Services.AddScoped<IReceivablesService, ReceivablesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
