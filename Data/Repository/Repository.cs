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

		public List<Post> GetAllPostsForPagination(int pageNumber)
		{
			int postsPerPage = 3;

			List<Post> posts = context
				.Posts
				.Skip(postsPerPage * (pageNumber - 1))
				.Take(postsPerPage)
				.ToList();

			return posts;
		}

		public Post GetPost(string id)
		{
			Post post = context.Posts
				.Include(p => p.MainComments)
				.ThenInclude(p => p.SubComments)
				.First(p => p.Id.ToString() == id);

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
