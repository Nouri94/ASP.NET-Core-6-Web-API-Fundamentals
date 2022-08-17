using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;
using Microsoft.EntityFrameworkCore;
//Create serilog logger (Downloaded NuGet)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog(); // Use the new Logger installed
builder.Services.AddControllers(options => //configure API headers
{
    options.ReturnHttpNotAcceptable = true;
})  .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters(); // added support from content-type = application-xml
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>(); // Allow all file extensions such as .pdf. .txt etc

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>(); // On debug mode 
#else
builder.Services.AddTransient<IMailService,CloudMailservice>(); // on Release mode
#endif

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<CitiesDataStore>();
builder.Services.AddDbContext<CityInfoContext>(dbContextOptions =>
dbContextOptions.UseSqlite(builder.Configuration["ConnectionString:CityInfoDBConnectionString"]));

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();   

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); }); 
app.Run();
