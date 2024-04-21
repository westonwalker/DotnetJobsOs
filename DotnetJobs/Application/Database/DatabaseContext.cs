using DotnetJobs.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DotnetJobs.Application.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    { }

    public virtual DbSet<User> Users { set; get; }
    public virtual DbSet<Role> Roles { set; get; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<Company> Companies { set; get; }
    public virtual DbSet<Job> Jobs { set; get; }
    public virtual DbSet<WorkType> WorkTypes { set; get; }
    public virtual DbSet<RemotePolicy> RemotePolicies { set; get; }
    public virtual DbSet<ExperienceLevel> ExperienceLevels { set; get; }
	public virtual DbSet<JobUpload> JobUploads { set; get; }

	protected override void OnModelCreating(ModelBuilder builder)
    {
        // it should be placed here, otherwise it will rewrite the following settings!
        base.OnModelCreating(builder);

        // Custom application mappings
        builder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(450).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Password).IsRequired();
        });

        builder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        builder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.RoleId);
            entity.Property(e => e.UserId);
            entity.Property(e => e.RoleId);
            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasForeignKey(d => d.RoleId);
            entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasForeignKey(d => d.UserId);
        });

        builder.Entity<Role>().HasData(
            new Role { Id = 1, Name = CustomRoles.User },
            new Role { Id = 2, Name = CustomRoles.Admin }
        );

		builder.Entity<ExperienceLevel>().HasData(
			new ExperienceLevel { Id = 1, Name = "Junior", IsDefault = false },
			new ExperienceLevel { Id = 2, Name = "Mid-level", IsDefault = false },
			new ExperienceLevel { Id = 3, Name = "Senior", IsDefault = false },
			new ExperienceLevel { Id = 4, Name = "Lead", IsDefault = false }
		);

		builder.Entity<RemotePolicy>().HasData(
			new RemotePolicy { Id = 1, Name = "Remote", IsDefault = true },
			new RemotePolicy { Id = 2, Name = "Hybrid" },
			new RemotePolicy { Id = 3, Name = "On-site" }
		);

		builder.Entity<WorkType>().HasData(
			new WorkType { Id = 1, Name = "Full-time", IsDefault = true },
			new WorkType { Id = 2, Name = "Part-time" },
			new WorkType { Id = 3, Name = "Internship" },
			new WorkType { Id = 4, Name = "Freelance" },
			new WorkType { Id = 5, Name = "Contract" }
		);

		builder.Entity<User>().HasData(
			new User { 
                Id = 1, 
                Name = "Wes",
                Email = "walk8919@gmail.com",
                Password = "HErVg3ksn0p1C5U9mtIgoUrFF7xObNrLE5FF58JmzU4="
			}
		);
	}
}
