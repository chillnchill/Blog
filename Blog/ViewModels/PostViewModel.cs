namespace Blog.ViewModels
{
	public class PostViewModel
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = null!;
		public string Body { get; set; } = null!;
		public string CurrentImage { get; set; } = null!;

		//this format is for media
		public IFormFile? Image { get; set; }
	}
}
