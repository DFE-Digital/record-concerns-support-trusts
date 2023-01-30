using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.ResponseModels;
using System.Net;
using System.Text;

namespace ConcernsCaseWork.API.Middleware;

public class ExceptionHandlerMiddleware
{
	private readonly RequestDelegate _next;
	public ExceptionHandlerMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext httpContext, ILogger<ExceptionHandlerMiddleware> logger)
	{
		try
		{
			bool captureBadRequestsWithBodyAsWarnings = (logger.IsEnabled(LogLevel.Warning) && IsApiRequest(httpContext.Request.Path));
			if (captureBadRequestsWithBodyAsWarnings)
			{
				httpContext.Request.EnableBuffering();
			}

			await _next(httpContext);

			// if non-success and warnings are enabled, log it.

			if (captureBadRequestsWithBodyAsWarnings && httpContext.Response.StatusCode >= (int)HttpStatusCode.BadRequest)
			{
				var bodyString = await GetRawBodyAsync(httpContext.Request);
				logger.LogWarning($"Returning bad request for path:{httpContext.Request.Path}. Request body:{bodyString}");
			}
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
	}

	private bool IsApiRequest(string path) => path.StartsWith("/v2/");

	private async Task<string> BodyToString(Stream requestBody)
	{
		var sr = new StreamReader(requestBody);
		return await sr.ReadToEndAsync();
	}

	public async Task<string> GetRawBodyAsync(HttpRequest request, Encoding encoding = null)
	{
		if (!request.Body.CanSeek)
		{
			// We only do this if the stream isn't *already* seekable,
			// as EnableBuffering will create a new stream instance
			// each time it's called
			request.EnableBuffering();
		}

		request.Body.Position = 0;

		var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

		var body = await reader.ReadToEndAsync().ConfigureAwait(false);

		request.Body.Position = 0;

		return body;
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