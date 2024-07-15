namespace Blog.ViewModels
{
	public class PostViewModel
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = null!;
		public string Body { get; set; } = null!;
		public string Description { get; set; } = null!;
		public string Tags { get; set; } = null!;
		public string Category { get; set; } = null!;
		public string? CurrentImage { get; set; }

		//this format is for media
		public IFormFile? Image { get; set; }

    }
}
