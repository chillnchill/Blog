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
        public string Image { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Tags { get; set; } = null!;
        public string Category { get; set; } = null!;
        public DateTime CreatedOn { get; set; } 
    }
}
