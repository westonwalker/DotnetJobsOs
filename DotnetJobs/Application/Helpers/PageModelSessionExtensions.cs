using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetJobs.Application.Helpers;

public static class PageModelSessionExtensions
{
	public static void SetPageFlash(this HttpContext HttpContext, string key, string value)
	{
		HttpContext.Session.SetString(key, value);
	}

	public static string? GetPageFlash(this HttpContext HttpContext, string key)
	{
		string? message = HttpContext.Session.GetString(key);
		HttpContext.Session.Remove(key);
		return message;
	}

	public static bool PageHasKey(this HttpContext HttpContext, string key)
	{
		string? message = HttpContext.Session.GetString(key);
		return string.IsNullOrEmpty(message) ? false : true;
	}
}
