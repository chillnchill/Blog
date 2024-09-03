using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;

namespace Blog.Data.Services
{
    public interface IRepository
    {
        Task<Post> GetPostAsync(string id);
		Task<IndexViewModel> GetAllPostsForPaginationAsync(int pageNumber, string category);
        Task<IEnumerable<PostViewModel>> GetAllPostsAsync();
		Task AddPostAsync(Post post);
        void UpdatePost(Post post);
		Task RemovePostAsync(string id);
		Task AddSubCommentAsync(SubComment comment);
		Task<IEnumerable<SubComment>> GetSubCommentsByMainCommentIdAsync(int mainCommentId);
		Task<bool> SaveChangesAsync();
        public void DeleteRange(IEnumerable<object> entities);




	}
}
