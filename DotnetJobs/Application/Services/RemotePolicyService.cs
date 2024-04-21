using DotnetJobs.Application.Database;
using DotnetJobs.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetJobs.Application.Services
{
	public class RemotePolicyService
	{
		private readonly DatabaseContext _db;

		public RemotePolicyService(DatabaseContext db)
		{
			_db = db;
		}

		public async Task<List<RemotePolicy>> GetAll()
		{
			return await _db.RemotePolicies.ToListAsync();
		}

		public async Task<RemotePolicy?> Get(int id)
		{
			return await _db.RemotePolicies
							.Where(d => d.Id == id)
							.FirstOrDefaultAsync();
		}
	}
}
