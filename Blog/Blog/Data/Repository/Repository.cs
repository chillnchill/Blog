using Blog.Data;
using Blog.Models;
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

        public Post GetPost(string id)
        {
            return context.Posts.FirstOrDefault(p => p.Id.ToString() == id);
        }

        public void RemovePost(string id)
        {
            context.Posts.Remove(GetPost(id));
        }

		public void UpdatePost(Post post)
		{
			context.Posts.Update(post);
		}


		public async Task<bool> SaveChangesAsync()
        {
            if (await  context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

	
	}
}
