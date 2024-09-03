using Blog.Models.Comments;
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
	using static Blog.Constants.ValidationConstants;
	public class PostViewModel
	{
        public PostViewModel()
        {
            MainComments = new List<MainComment>();
        }		
        public Guid Id { get; set; }

		[StringLength(PostTitleMaxLength, MinimumLength = PostTitleMinLength, ErrorMessage = FieldErrorMessage)]
		public string Title { get; set; } = null!;

		[StringLength(PostBodyMaxLength, MinimumLength = PostBodyMinLength, ErrorMessage = FieldErrorMessage)]
		public string Body { get; set; } = null!;

		[StringLength(PostDescriptionMaxLength, MinimumLength = PostDescriptionMinLength, ErrorMessage = FieldErrorMessage)]
		public string Description { get; set; } = null!;

		[StringLength(TagsMaxLength, MinimumLength = TagsMinLength, ErrorMessage = FieldErrorMessage)]
		public string Tags { get; set; } = null!;		
		public string Category { get; set; } = null!;
		public string? CurrentImage { get; set; }

		
		public IFormFile? Image { get; set; }
		public List<MainComment> MainComments { get; set; }
	}
}
