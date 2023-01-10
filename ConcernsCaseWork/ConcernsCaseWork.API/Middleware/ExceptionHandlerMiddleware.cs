using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.ResponseModels;
using System.Net;

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
			await _next(httpContext);
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