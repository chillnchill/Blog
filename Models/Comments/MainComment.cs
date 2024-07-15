namespace Blog.Models.Comments
{
	public class MainComment : Comment
	{
        public MainComment()
        {
            SubComments = new List<SubComment>();
        }
        public List<SubComment> SubComments { get; set; }
    }
}
