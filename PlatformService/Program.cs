using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPlatformRepo,PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient,HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient,MessageBusClient>();
if(builder.Environment.IsProduction())
{
    Console.WriteLine("Using SQL Server");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opts =>
    {
        opts.UseInMemoryDatabase("InMem");
    });
}
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app,builder.Environment.IsProduction());

app.Run();
