using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace Blog.ViewModels
{
	public class CommentViewModel
	{		
		[Required]
		public string PostId { get; set; } = null!;

		public int MainCommentId { get; set; }

		[Required]
		[StringLength(15360, MinimumLength = 3 , ErrorMessage = "Comments need to be at least 3 characters long")]
		public string Message { get; set; } = string.Empty;

	}
}
