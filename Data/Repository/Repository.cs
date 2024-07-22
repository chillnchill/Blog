using Blog.Data;
using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Services
{
	public class Repository : IRepository
	{
		private readonly BlogDbContext context;

		public Repository(BlogDbContext context)
		{
			this.context = context;
		}
		public void AddPost(Post post)
		{
			context.Posts.Add(post);
		}

		public List<Post> GetAllPosts()
		{
			return context.Posts.ToList();
		}


		public List<Post> GetAllPosts(string category)
		{
			return context.Posts
				.Where(post => post.Category.ToLower().Equals(category.ToLower()))
				.ToList();
		}

		public IndexViewModel GetAllPostsForPagination(int pageNumber, string category)
		{
			int postsPerPage = 3;
			int postsToSkip = postsPerPage * (pageNumber - 1);

			IQueryable<Post> query = context.Posts.AsQueryable();

			if (!string.IsNullOrEmpty(category))
			{
				query = query.Where(p => p.Category == category);
			}

			int postsCount = query.Count(); // Count posts after filtering by category
			int pageCount = (int)Math.Ceiling((double)postsCount / postsPerPage); // Calculate total pages

			List<Post> posts = query
				.Skip(postsToSkip)
				.Take(postsPerPage)
				.ToList();

			return new IndexViewModel()
			{
				PageNumber = pageNumber,
				PageCount = pageCount,
				NextPage = pageNumber < pageCount,
				Posts = posts
			};
		}

		public Post GetPost(string id)
		{
			Post? post = context.Posts
				.Include(p => p.MainComments)
				.ThenInclude(p => p.SubComments)
				.FirstOrDefault(p => p.Id.ToString() == id);

			return post;
		}

		public void RemovePost(string id)
		{
			context.Posts.Remove(GetPost(id));
		}

		public void UpdatePost(Post post)
		{
			var existingPost = context.Posts.Local.FirstOrDefault(p => p.Id == post.Id);
			if (existingPost != null)
			{
				context.Entry(existingPost).State = EntityState.Detached;
			}
			context.Posts.Update(post);
		}

		public void AddSubComment(SubComment comment)
		{
			context.Add(comment);
		}
		public async Task<bool> SaveChangesAsync()
		{
			if (await context.SaveChangesAsync() > 0)
			{
				return true;
			}
			return false;
		}

		
	}
}
