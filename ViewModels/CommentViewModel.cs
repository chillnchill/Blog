using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace Blog.ViewModels
{
	using static Blog.Constants.ValidationConstants;
	public class CommentViewModel
	{		
		[Required]
		public string PostId { get; set; } = null!;

		public int MainCommentId { get; set; }

		[Required]
		[StringLength(CommentsMaxLength, MinimumLength = CommentsMinLength , ErrorMessage = FieldErrorMessage)]
		public string Message { get; set; } = string.Empty;

	}
}
