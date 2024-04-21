using DotnetJobs.Application.Database;
using DotnetJobs.Application.Helpers;
using DotnetJobs.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetJobs.Application.Services
{
	public class JobService
	{
		private readonly DatabaseContext _db;

		public JobService(DatabaseContext db)
		{
			_db = db;
		}

		public Job? Find(int id)
		{
			return _db.Jobs.Find(id);
		}

		public async Task<List<Job>> GetAll()
		{
			return await _db.Jobs
							.Include(job => job.RemotePolicy)
							.Include(job => job.Company)
							.Include(job => job.WorkType)
							.Include(job => job.ExperienceLevel)
							.OrderByDescending(job => job.CreatedAt)
							.ToListAsync();
		}

		public async Task<List<Job>> GetBoardJobs()
		{
			return await _db.Jobs
				.Include(job => job.RemotePolicy)
				.Include(job => job.Company)
				.Include(job => job.WorkType)
				.Include(job => job.ExperienceLevel)
				.Where(job => job.HasPaid)
				.Where(job => job.CreatedAt > DateTime.UtcNow.AddDays(-60))
				.OrderByDescending(job => job.IsPremium && job.CreatedAt > DateTime.UtcNow.AddDays(-30))
				.ThenByDescending(job => job.CreatedAt)
				.ToListAsync();
		}

		public async Task<int> BoardJobsCount()
        {
			return await _db.Jobs
				.Include(job => job.RemotePolicy)
				.Include(job => job.Company)
				.Include(job => job.WorkType)
				.Include(job => job.ExperienceLevel)
				.Where(job => job.HasPaid)
				.Where(job => job.CreatedAt > DateTime.UtcNow.AddDays(-60))
				.CountAsync();
        }

		public async Task<List<Job>> GetBoardJobs(int page, int take)
		{
			return await _db.Jobs
				.Include(job => job.RemotePolicy)
				.Include(job => job.Company)
				.Include(job => job.WorkType)
				.Include(job => job.ExperienceLevel)
				.Where(job => job.HasPaid)
				.Where(job => job.CreatedAt > DateTime.UtcNow.AddDays(-60))
				.OrderByDescending(job => job.IsPremium && job.CreatedAt > DateTime.UtcNow.AddDays(-30))
				.ThenByDescending(job => job.CreatedAt)
				.ToPagedListAsync(page, take);
		}

		public async Task<List<Job>> GetRemoteJobs(int page, int take)
		{
			return await _db.Jobs
				.Include(job => job.RemotePolicy)
				.Include(job => job.Company)
				.Include(job => job.WorkType)
				.Include(job => job.ExperienceLevel)
				.Where(job => job.RemotePolicyId == 1)
				.Where(job => job.HasPaid)
				.Where(job => job.CreatedAt > DateTime.UtcNow.AddDays(-60))
				.OrderByDescending(job => job.IsPremium && job.CreatedAt > DateTime.UtcNow.AddDays(-30))
				.ThenByDescending(job => job.CreatedAt)
				.ToPagedListAsync(page, take);
		}

		public async Task<List<Job>> GetNewsletterJobs()
        {
            return await _db.Jobs
                .Include(job => job.RemotePolicy)
                .Include(job => job.Company)
                .Include(job => job.WorkType)
                .Include(job => job.ExperienceLevel)
                .Where(job => job.HasPaid)
                .Where(job => job.CreatedAt > DateTime.UtcNow.AddDays(-7))
                .OrderByDescending(job => job.IsPremium && job.CreatedAt > DateTime.UtcNow.AddDays(-7))
                .ThenByDescending(job => job.CreatedAt)
                .ToListAsync();
        }

        public Job Store(Job job)
		{
            _db.Jobs.Add(job);
            _db.SaveChanges();
			return job;
		}

		public async Task<Job?> GetBySlug(string slug)
		{
			return await _db.Jobs
							.Where(d => d.Slug == slug)
							.Include(job => job.RemotePolicy)
							.Include(job => job.Company)
							.Include(job => job.WorkType)
							.Include(job => job.ExperienceLevel)
							.FirstOrDefaultAsync();
		}

		public async Task<List<WorkType>> GetWorkTypes()
		{
			return await _db.WorkTypes.ToListAsync();
		}

		public async Task<List<RemotePolicy>> GetRemotePolicies()
		{
			return await _db.RemotePolicies.ToListAsync();
		}

		public async Task<List<ExperienceLevel>> GetExperienceLevels()
		{
			return await _db.ExperienceLevels.ToListAsync();
		}

		public void UpdateJobViews(Job job)
		{
			var currentJob = _db.Jobs.Where(j => j.Id== job.Id).FirstOrDefault();
			if (currentJob.ViewCount == null)
			{
				currentJob.ViewCount = 1;
			}
			else
			{
				currentJob.ViewCount++;
			}
            _db.SaveChanges();
		}

		public void UpdateApplyCount(Job job)
		{
			var currentJob = _db.Jobs.Where(j => j.Id == job.Id).FirstOrDefault();
			if (job.ApplyCount == null)
			{
				job.ApplyCount = 1;
			}
			else
			{
				job.ApplyCount++;
			}
            _db.SaveChanges();
		}

		public async Task<Job?> GetByPaymentId(string id)
		{
			return await _db.Jobs
                .Include(job => job.RemotePolicy)
                .Include(job => job.Company)
                .Include(job => job.WorkType)
                .Include(job => job.ExperienceLevel)
                .Where(d => d.PaymentCallbackGuid == id)
				.FirstOrDefaultAsync();
		}

		public Job UpdateHasPaid(Job job)
		{
            _db.Attach(job);
			job.HasPaid = true;
            _db.SaveChanges();
			return job;
		}
	}
}
