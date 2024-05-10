using PatientAPI_OData.Services.Interfaces;
using PatientAPI_OData.Services;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddControllers().AddOData(options => options//add OData to the services to allow for querying
.Select()//allows to select " https://localhost:7267/api/Patient?$Select=id"
.Filter()//alklows to filter " https://localhost:7267/api/Patient?$filter=Firstname eq 'Johnzzz'"
.OrderBy());//allows to order by " https://localhost:7267/api/Patient?$orderby=Firstname desc"
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
