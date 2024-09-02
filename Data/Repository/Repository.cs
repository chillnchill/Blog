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
		public async Task AddPostAsync(Post post)
		{
			await context.Posts.AddAsync(post);
		}
		public async Task<IEnumerable<Post>> GetAllPostsAsync()
		{
			return await context.Posts.ToListAsync();
		}

		//public async Task<IEnumerable<Post>> GetAllPostsAsync(string category)
		//{
		//	return await context.Posts
		//		.Where(post => post.Category.ToLower().Equals(category.ToLower()))
		//		.ToListAsync();
		//}


		public async Task<IndexViewModel> GetAllPostsForPaginationAsync(int pageNumber, string category)
		{
			int postsPerPage = 3;
			int postsToSkip = postsPerPage * (pageNumber - 1);

			IQueryable<Post> query = context.Posts.AsQueryable();

			if (!string.IsNullOrEmpty(category))
			{
				query = query.Where(p => p.Category == category);
			}

			int postsCount = await query.CountAsync(); 
			int pageCount = (int)Math.Ceiling((double)postsCount / postsPerPage);

			List<Post> posts = await query
				.Skip(postsToSkip)
				.Take(postsPerPage)
				.ToListAsync();

			return new IndexViewModel()
			{
				PageNumber = pageNumber,
				PageCount = pageCount,
				NextPage = pageNumber < pageCount,
				Posts = posts
			};
		}

		public async Task<Post> GetPostAsync(string id)
		{
			Post post = await context.Posts
				.Include(p => p.MainComments)
				.ThenInclude(mc => mc.SubComments)
				.FirstOrDefaultAsync(p => p.Id.ToString() == id);

			return post;
		}

		public async Task RemovePostAsync(string id)
		{
			Post? post = await GetPostAsync(id);

			if (post != null)
			{
				context.Posts.Remove(post);
			}
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

		public async Task AddSubCommentAsync(SubComment comment)
		{
			await context.AddAsync(comment);
		}

		public async Task<IEnumerable<SubComment>> GetSubCommentsByMainCommentIdAsync(int mainCommentId)
		{
			return await context.SubComments.Where(sc => sc.MainCommentId == mainCommentId).ToListAsync();
		}

		public void DeleteRange(IEnumerable<object> entities)
		{
			context.RemoveRange(entities);
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
