using Blog.Models;
using Blog.Models.Comments;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Blog.Data
{
	public class BlogDbContext : IdentityDbContext
	{
        public BlogDbContext(DbContextOptions<BlogDbContext> options)
            : base(options)
        {         
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<MainComment> MainComments { get; set; }
        public DbSet<SubComment> SubComments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			Assembly configAssembly = Assembly.GetAssembly(typeof(BlogDbContext)) ??
							Assembly.GetExecutingAssembly();
			modelBuilder.ApplyConfigurationsFromAssembly(configAssembly);


			base.OnModelCreating(modelBuilder);
		}
	}
}
