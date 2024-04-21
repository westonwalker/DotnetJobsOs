using DotnetJobs.Application.Database;
using DotnetJobs.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetJobs.Application.Services
{
	public class ExperienceLevelService
	{
		private readonly DatabaseContext _db;

		public ExperienceLevelService(DatabaseContext db)
		{
			_db = db;
		}
		public async Task<List<ExperienceLevel>> GetAll()
		{
			return await _db.ExperienceLevels.ToListAsync();
		}

		public async Task<ExperienceLevel?> Get(int id)
		{
			return await _db.ExperienceLevels
							.Where(d => d.Id == id)
							.FirstOrDefaultAsync();
		}
	}
}
