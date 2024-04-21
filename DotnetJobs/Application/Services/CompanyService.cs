using DotnetJobs.Application.Database;
using DotnetJobs.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetJobs.Application.Services
{
	public class CompanyService
	{
		private readonly DatabaseContext _db;

		public CompanyService(DatabaseContext db)
		{
			_db = db;
		}

		public async Task<Company?> GetByEmail(string email)
		{
			return await _db.Companies
							.Where(d => d.Email == email)
							.FirstOrDefaultAsync();
		}

		public async Task<Company?> GetBySlug(string slug)
		{
			return await _db.Companies
							.Where(d => d.Slug == slug)
							.FirstOrDefaultAsync();
		}

		public async Task<Company> Store(Company company)
		{
			_db.Companies.Add(company);
			_db.SaveChanges();
			return company;
		}
	}
}
