using SmartId.Authentication.Application.Service;
using SmartId.Authentication.Domain.Settings;
using SmartId.Authentication.Setup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddServices();
builder.Services.Configure<SmartIdSettings>(builder.Configuration.GetSection("SmartIdSettings"));
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapPost("/authentication", async (SmartIdAuthenticationService service, AuthenticationRequest request) =>
{
	var result = await service.RequestAuthentication(request.IdCode, request.CountryCode);
	return Results.Ok(result);
});

app.MapGet("/sessionStatus/{sessionId}", async (SmartIdAuthenticationService service, string sessionId) =>
{
	var result = await service.VerifyAuthentication(sessionId);
	return Results.Ok(result);
});

app.UseHttpsRedirection();
app.UseExceptionHandler("/error");
app.Run();

public class AuthenticationRequest
{
	public required string IdCode { get; set; }
	public required string CountryCode { get; set; }
}


