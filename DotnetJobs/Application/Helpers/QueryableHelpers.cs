using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace DotnetJobs.Application.Helpers
{
	public static class QueryableHelpers
	{
		public static async Task<List<T>> ToPagedListAsync<T>(
			this IQueryable<T> source,
			int page,
			int pageSize,
			CancellationToken token = default)
		{
			var count = await source.CountAsync(token);
			if (count > 0)
			{
				return await source
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync(token);
			}

			return new List<T>();
		}
	}

	public class PagedList<T> : IReadOnlyList<T>
	{
		private readonly IList<T> subset;
		public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
		{
			PageNumber = pageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			subset = items as IList<T> ?? new List<T>(items);
		}

		public int PageNumber { get; }

		public int TotalPages { get; }

		public bool IsFirstPage => PageNumber == 1;

		public bool IsLastPage => PageNumber == TotalPages;

		public int Count => subset.Count;

		public T this[int index] => subset[index];

		public IEnumerator<T> GetEnumerator() => subset.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => subset.GetEnumerator();
	}
}
