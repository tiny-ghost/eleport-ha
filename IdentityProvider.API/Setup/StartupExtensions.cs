using IdentityProvider.Application.Service;
using IdentityProvider.Domain.Settings;

namespace SmartId.Authentication.Setup;

public static class StartupExtensions
{
	public static void AddHttpClients(this IServiceCollection services, IConfiguration config)
	{
		var smartIdSettings = config.GetSection("SmartIdSettings").Get<SmartIdSettings>();
		services.AddHttpClient("SmartIdV1", client =>
		{
			client.BaseAddress = new Uri(smartIdSettings!.BaseUrlV1 );
		});
		
		services.AddHttpClient("SmartIdV2", client =>
		{
			client.BaseAddress = new Uri(smartIdSettings!.BaseUrlV2 );
		});
	}
	
	public static void AddServices(this IServiceCollection services)
	{
		services.AddTransient<ISmartIdAuthenticationService, SmartIdAuthenticationService>();
	}
}