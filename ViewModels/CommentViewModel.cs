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
		public string Message { get; set; } = string.Empty;


	}
}
