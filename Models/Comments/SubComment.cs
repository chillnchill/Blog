using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models.Comments
{
	public class SubComment : Comment
	{
		[ForeignKey(nameof(MainComment))]
        public int MainCommentId { get; set; }
		public virtual MainComment MainComment { get; set; } = null!;
    }
}
