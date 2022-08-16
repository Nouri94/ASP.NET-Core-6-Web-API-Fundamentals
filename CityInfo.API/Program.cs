using Microsoft.AspNetCore.StaticFiles;
using Serilog;

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
