namespace Blog.Constants
{
	public class ValidationConstants
	{
		public const int CommentsMaxLength = 15360;
		public const int CommentsMinLength = 3;
		
		public const int PostTitleMaxLength = 255;
		public const int PostTitleMinLength = 3;

		public const int PostBodyMaxLength = 50000;
		public const int PostBodyMinLength = 128;

		public const int PostDescriptionMaxLength = 512;
		public const int PostDescriptionMinLength = 3;

		public const int TagsMaxLength = 128;
		public const int TagsMinLength = 2;

		public const string FieldErrorMessage = "The {0} must be between {2} and {1} characters long.";
	}
}
