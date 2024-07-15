using Blog.Models.Comments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Blog.Data.Configurations
{
	public class SubCommentConfiguration : IEntityTypeConfiguration<SubComment>
	{
		public void Configure(EntityTypeBuilder<SubComment> builder)
		{
			builder
				.HasOne(sc => sc.MainComment)
				.WithMany(mc => mc.SubComments)
				.HasForeignKey(sc => sc.MainCommentId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
