


namespace P9_Blog_Generator_AI_Backend.Models;

public class BlogRequest
{
    public int BlogRequestId { get; set; }

    public int UserId { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Topic { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Tone { get; set; } = string.Empty;

    public int WordCount { get; set; }

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }

    public ICollection<GeneratedBlog> GeneratedBlogs { get; set; }
        = new List<GeneratedBlog>();
}