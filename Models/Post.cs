using Blog.Models.Comments;

namespace Blog.Models
{
	public class Post
	{
		public Post()
		{
			Id = Guid.NewGuid();
			MainComments = new List<MainComment>();
		}

		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Body { get; set; } = string.Empty;
		public string Image { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Tags { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public DateTime CreatedOn { get; set; }
		public List<MainComment> MainComments { get; set; }
	}
}
