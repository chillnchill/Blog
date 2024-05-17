using Blog.Models;

namespace Blog.Data.Services
{
    public interface IRepository
    {
        Post GetPost(string id);
        //FrontPostViewModel GetFrontPost(int id);
        List<Post> GetAllPosts();
        //IndexViewModel GetAllPosts(int pageNumber, string category, string search);
        void AddPost(Post post);
        void UpdatePost(Post post);
        void RemovePost(string id);
        //void AddSubComment(SubComment comment);
        Task<bool> SaveChangesAsync();


    }
}
