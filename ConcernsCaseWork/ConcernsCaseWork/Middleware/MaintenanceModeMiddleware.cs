using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Middleware;

public class MaintenanceModeMiddleware
{
	private readonly RequestDelegate _next;

	public MaintenanceModeMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	// Conditionally registered if maintenace mode is enabled
	public async Task Invoke(HttpContext context)
	{
		if (!context.Request.Path.StartsWithSegments("/Maintenance"))
		{
			context.Response.Redirect("/Maintenance");
		}

		await _next(context);
	}
}
