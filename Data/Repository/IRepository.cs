using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;

namespace Blog.Data.Services
{
    public interface IRepository
    {
        Post GetPost(string id);
        //FrontPostViewModel GetFrontPost(int id);
        List<Post> GetAllPosts();
        IndexViewModel GetAllPostsForPagination(int pageNumber, string category);
		List<Post> GetAllPosts(string category);
        //IndexViewModel GetAllPosts(int pageNumber, string category, string search);
        void AddPost(Post post);
        void UpdatePost(Post post);
        void RemovePost(string id);
		//void AddSubComment(SubComment comment);
		void AddSubComment(SubComment comment);
		Task<bool> SaveChangesAsync();
        Task<IEnumerable<SubComment>> GetSubCommentsByMainCommentIdAsync(int mainCommentId);
        public void DeleteRange(IEnumerable<object> entities);




	}
}
