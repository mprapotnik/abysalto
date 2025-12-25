using AbySalto.CartApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<CartDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Db")));


var app = builder.Build();

app.MapControllers();

app.Run();
