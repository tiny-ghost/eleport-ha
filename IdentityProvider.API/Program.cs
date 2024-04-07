using IdentityProvider.Domain.Settings;
using SmartId.Authentication.Setup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddControllers();
builder.Services.Configure<SmartIdSettings>(builder.Configuration.GetSection("SmartIdSettings"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
