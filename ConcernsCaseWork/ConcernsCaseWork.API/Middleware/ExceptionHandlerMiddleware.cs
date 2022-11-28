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
		catch (NotFoundException ex)
		{
			logger.LogError($"Not Found: {ex}");
			await HandleNotFoundExceptionAsync(httpContext, ex, logger);
		}
		catch (Exception ex)
		{
			logger.LogError($"Something went wrong: {ex}");
			await HandleExceptionAsync(httpContext, ex, logger);
		}
	}

	private async Task HandleNotFoundExceptionAsync(HttpContext context, Exception exception, ILogger<ExceptionHandlerMiddleware> logger)
	{
		logger.LogError(exception.Message);
		logger.LogError(exception.StackTrace);
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)HttpStatusCode.NotFound;
		await context.Response.WriteAsync(new ErrorResponse()
		{
			StatusCode = context.Response.StatusCode,
			Message = $"Not Found: {exception.Message}"
		}.ToString());
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ExceptionHandlerMiddleware> logger)
	{
		logger.LogError(exception.Message);
		logger.LogError(exception.StackTrace);
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
		await context.Response.WriteAsync(new ErrorResponse()
		{
			StatusCode = context.Response.StatusCode,
			Message = "Internal Server Error: " + exception.Message
		}.ToString());
	}
}