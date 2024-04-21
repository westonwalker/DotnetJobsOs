using Markdig;

namespace DotnetJobs.Application.Helpers
{
	public static class MarkdownHelper
	{
		public static string Parse(string markdown)
		{
			var pipeline = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.Build();
			return Markdown.ToHtml(markdown, pipeline);
		}
	}
}
