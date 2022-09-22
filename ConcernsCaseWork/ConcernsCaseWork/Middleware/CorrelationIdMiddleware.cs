using ConcernsCaseWork.Logging;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Middleware
{
	/// <summary>
	/// Middleware that checks incoming requests for a correlation and causation id header. If not found then default values will be created.
	/// Saves these values in the correlationContext instance. Be sure to register correlation context as scoped or the equivalent in you ioc container.
	/// </summary>
	public class CorrelationIdMiddleware
	{
		private const string _correlationIdHeaderKey = "x-correlation-id";
		private const string _causationIdHeaderKey = "x-causation-id";

		private readonly RequestDelegate _next;

		public CorrelationIdMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		// ReSharper disable once UnusedMember.Global
		// Invoked by asp.net
		public Task Invoke(HttpContext httpContext, ICorrelationContext correlationContext)
		{

			string incomingCorrelationId;
			string previousRequestId;

			// correlation id. An ID that spans many requests
			if (httpContext.Request.Headers.ContainsKey(_correlationIdHeaderKey) && !string.IsNullOrWhiteSpace(httpContext.Request.Headers[_correlationIdHeaderKey]))
			{
				incomingCorrelationId = httpContext.Request.Headers[_correlationIdHeaderKey];
			}
			else
			{
				// guid is easily generated in tools like postman. Not sure a .net trace Id is so easy, so use a guid.
				incomingCorrelationId = Guid.NewGuid().ToString();
			}

			// causationId. An id of the request that caused this request.
			if (httpContext.Request.Headers.ContainsKey(_causationIdHeaderKey) && !string.IsNullOrWhiteSpace(httpContext.Request.Headers[_causationIdHeaderKey]))
			{
				previousRequestId = httpContext.Request.Headers[_causationIdHeaderKey];
			}
			else
			{
				// when no causationId because this is the first request of a chain, use the correlationId
				previousRequestId = incomingCorrelationId;
			}

			// And for any n
			httpContext.Request.Headers[_correlationIdHeaderKey] = incomingCorrelationId;
			httpContext.Request.Headers[_causationIdHeaderKey] = httpContext.TraceIdentifier;

			correlationContext.SetContext(incomingCorrelationId, previousRequestId, httpContext.TraceIdentifier);

			using (LogContext.PushProperty("CorrelationId", correlationContext.CorrelationId))
			using (LogContext.PushProperty("ApplicationId", ApplicationContext.ApplicationName))
			{
				return _next(httpContext);
			}
		}
	}
}
