using ConcernsCaseWork.API.UseCases;

namespace ConcernsCaseWork.API.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "ApiKey";
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IUseCase<string, bool> apiKeyValidationService)
        {
	        if (IsApiCall(context))
	        {
		        if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
		        {
			        context.Response.StatusCode = 401;
			        await context.Response.WriteAsync("Api Key was not provided.");
			        return;
		        }

		        var isKeyValid = apiKeyValidationService.Execute(extractedApiKey);

		        if (!isKeyValid)
		        {
			        context.Response.StatusCode = 401;
			        await context.Response.WriteAsync("Unauthorized client.");
			        return;
		        }
	        }

	        await _next(context);
        }

        private bool IsApiCall(HttpContext context) => context.Request.Path.HasValue && context.Request.Path.Value!.StartsWith("/v2");
    }
}