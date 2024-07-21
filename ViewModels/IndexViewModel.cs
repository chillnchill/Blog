using Blog.Models;

namespace Blog.ViewModels
{
	public class IndexViewModel
	{
        public IndexViewModel()
        {
            Posts = new List<Post>();
        }

        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public bool NextPage { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
