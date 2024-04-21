using DotnetJobs.Application.Database;
using DotnetJobs.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetJobs.Application.Services
{

	public class WorkTypeService
	{
		private readonly DatabaseContext _db;

		public WorkTypeService(DatabaseContext db)
		{
			_db = db;
		}

		public async Task<List<WorkType>> GetAll()
		{
			return await _db.WorkTypes.ToListAsync();
		}

		public async Task<WorkType?> Get(int id)
		{
			return await _db.WorkTypes
							.Where(d => d.Id == id)
							.FirstOrDefaultAsync();
		}
	}
}
