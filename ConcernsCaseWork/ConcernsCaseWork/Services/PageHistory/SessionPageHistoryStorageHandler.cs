using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Services.PageHistory;

public class SessionPageHistoryStorageHandler : IPageHistoryStorageHandler
{
	private const string _sessionKeyName = "PageHistory";
	private const string _separator = "||";
	private const int _maxHistoryLength = 50;
	
	public void SetPageHistory(HttpContext httpContext, IEnumerable<string> pageHistory)
	{
		if (httpContext.Session.IsAvailable)
		{
			httpContext.Session.SetString(_sessionKeyName, SerialisePageHistory(pageHistory.TakeLast(_maxHistoryLength)));
		}
	}

	public IEnumerable<string> GetPageHistory(HttpContext httpContext)
	{
		return DeserialisePageHistory(httpContext.Session.GetString(_sessionKeyName));
	}

	private static IEnumerable<string> DeserialisePageHistory(string value) => value?.Split(_separator) ?? Array.Empty<string>();
	private static string SerialisePageHistory(IEnumerable<string> value) => string.Join(_separator, value);
}