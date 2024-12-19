using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Exceptions;
using System.Net;
using System.Text;

namespace ConcernsCaseWork.API.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext httpContext, ILogger<ExceptionHandlerMiddleware> logger)
	{
		if (!IsApiRequest(httpContext.Request.Path)) 
		{
			await next(httpContext);
			return;
		}

		var originalBodyStream = httpContext.Response.Body;
		using var memoryStream = new MemoryStream();

		try
		{
			httpContext.Request.EnableBuffering();
			httpContext.Response.Body = memoryStream;

			await next(httpContext);

			await LogValidationFailed(httpContext, logger);
		}
		catch (Exception ex)
		{
			var messageWithPrefix = GetStatusCodeWithPrefix(ex);

			var parameters = new HttpExceptionParams()
			{
				Context = httpContext,
				Logger = logger,
				Exception = ex,
				MessagePrefix = messageWithPrefix.MessagePrefix,
				StatusCode = messageWithPrefix.StatusCode
			};

			await HandleHttpException(parameters);
		}
		finally
		{
			// Reset the response stream so it can be used to send the response back to the requester
			memoryStream.Position = 0;
			await memoryStream.CopyToAsync(originalBodyStream);

			httpContext.Response.Body = originalBodyStream;
		}
	}

	private async Task LogValidationFailed(HttpContext httpContext, ILogger<ExceptionHandlerMiddleware> logger)
	{
		bool logRequestResponse = (logger.IsEnabled(LogLevel.Warning) && IsApiRequest(httpContext.Request.Path));

		// if non-success and warnings are enabled, log it.
		if (logRequestResponse && httpContext.Response.StatusCode >= (int)HttpStatusCode.BadRequest)
		{
			var requestBody = await GetRequestBody(httpContext.Request);
			logger.LogWarning($"Validation failed for path:{httpContext.Request.Path}. Request body:{requestBody}");

			var responseBody = await GetResponseBody(httpContext.Response.Body);
			logger.LogWarning($"Validation failed for path:{httpContext.Request.Path}. Response body: {responseBody}");
		}
	}

	private bool IsApiRequest(string path) => path.StartsWith("/v2/");

	private async Task<string> GetRequestBody(HttpRequest request)
	{
		if (!request.Body.CanSeek)
		{
			// We only do this if the stream isn't *already* seekable,
			// as EnableBuffering will create a new stream instance
			// each time it's called
			request.EnableBuffering();
		}

		request.Body.Position = 0;

		var reader = new StreamReader(request.Body, Encoding.UTF8);

		var body = await reader.ReadToEndAsync().ConfigureAwait(false);

		request.Body.Position = 0;

		return body;
	}

	private async Task<string> GetResponseBody(Stream stream)
	{
		stream.Position = 0;
		var reader = new StreamReader(stream);
		var result = await reader.ReadToEndAsync();

		return result;
	}

	private async Task HandleHttpException(HttpExceptionParams parameters)
	{
		parameters.Logger.LogError($"{parameters.MessagePrefix}: {parameters.Exception}");

		parameters.Logger.LogError(parameters.Exception.Message);
		parameters.Logger.LogError(parameters.Exception.StackTrace);
		parameters.Context.Response.ContentType = "application/json";
		parameters.Context.Response.StatusCode = (int)parameters.StatusCode;

		await parameters.Context.Response.WriteAsync(new ErrorResponse()
		{
			StatusCode = parameters.Context.Response.StatusCode,
			Message = $"{parameters.MessagePrefix}: {parameters.Exception.Message}"
		}.ToString());
	}

	private StatusCodeMessagePrefix GetStatusCodeWithPrefix(Exception ex)
	{
		switch (ex)
		{
			case ResourceConflictException:
				return new StatusCodeMessagePrefix() { StatusCode = HttpStatusCode.Conflict, MessagePrefix = "Conflict" };
			case NotFoundException:
				return new StatusCodeMessagePrefix() { StatusCode = HttpStatusCode.NotFound, MessagePrefix = "Not Found" };
			case OperationNotCompletedException:
				return new StatusCodeMessagePrefix() { StatusCode = HttpStatusCode.BadRequest, MessagePrefix = "Operation not completed" };
			default:
				return new StatusCodeMessagePrefix() { StatusCode = HttpStatusCode.InternalServerError, MessagePrefix = "Internal Server Error" };
		}
	}

	private class StatusCodeMessagePrefix
	{
		public HttpStatusCode StatusCode { get; set; }
		public string MessagePrefix { get; set; }
	}

	private class HttpExceptionParams
	{
		public HttpContext Context { get; set; }
		public Exception Exception { get; set; }
		public ILogger<ExceptionHandlerMiddleware> Logger { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public string MessagePrefix { get; set; }
	}
}