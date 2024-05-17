namespace Blog.Models
{
    public class Post
    {
        public Post()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;

        public DateTime CreatedOn { get; set; } 
    }
}
