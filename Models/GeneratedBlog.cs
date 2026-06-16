namespace P9_Blog_Generator_AI_Backend.Models;


public class GeneratedBlog
{
    public int GeneratedBlogId { get; set; }

    public int BlogRequestId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public BlogRequest? BlogRequest { get; set; }
}