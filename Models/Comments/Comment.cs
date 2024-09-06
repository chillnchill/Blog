using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Comments
{
	public class Comment
	{
		[Key]
        public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = null!;

		[Required]
		public string Message { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }
    }
}
