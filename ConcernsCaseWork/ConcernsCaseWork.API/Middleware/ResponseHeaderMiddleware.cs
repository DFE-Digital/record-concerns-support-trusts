namespace ConcernsCaseWork.API.Middleware
{
	public class ResponseHeaderMiddleware
	{
		private readonly RequestDelegate _next;
		public ResponseHeaderMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext httpContext, ILogger<ResponseHeaderMiddleware> logger)
		{
			await _next(httpContext);

			// httpContext.Response.Headers["Content-Security-Policy"] = "default-src 'self';";
		}
	}
}
