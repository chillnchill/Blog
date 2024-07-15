using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
	public class CommentViewModel
	{

		public string PostId { get; set; } = null!;

		public int MainCommentId { get; set; }

		[Required]
		public string Message { get; set; } = string.Empty;
	}
}
