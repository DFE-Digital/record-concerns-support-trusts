using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace ConcernsCaseWork.Middleware
{
	/// <summary>
	/// Middleware that checks incoming requests for a correlation and causation id header. If not found then default values will be created.
	/// Saves these values in the correlationContext instance. Be sure to register correlation context as scoped or the equivalent in you ioc container.
	/// </summary>
	public class CorrelationIdMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<CorrelationIdMiddleware> _logger;
		private const string _applicationName = "ConcernsCasework";

		public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
		{
			_next = next;
			_logger = Guard.Against.Null(logger);
		}

		// ReSharper disable once UnusedMember.Global
		// Invoked by asp.net
		public Task Invoke(HttpContext httpContext, ICorrelationContext correlationContext)
		{
			string thisCorrelationId;

			// correlation id. An ID that spans many requests
			if (httpContext.Request.Headers.ContainsKey(correlationContext.HeaderKey) && !string.IsNullOrWhiteSpace(httpContext.Request.Headers[correlationContext.HeaderKey]))
			{
				thisCorrelationId = httpContext.Request.Headers[correlationContext.HeaderKey];
			}
			else
			{
				thisCorrelationId = Guid.NewGuid().ToString();
			}

			httpContext.Request.Headers[correlationContext.HeaderKey] = thisCorrelationId;

			correlationContext.SetContext(thisCorrelationId);

			_logger.LogInformation($"ApplicationId : {_applicationName}");
			_logger.LogInformation($"CorrelationId : {correlationContext.CorrelationId}");
			httpContext.Response.Headers[correlationContext.HeaderKey] = thisCorrelationId;
			return _next(httpContext);
			
		}
	}
}
