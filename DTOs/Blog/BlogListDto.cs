namespace P9_Blog_Generator_AI_Backend.DTOs.Blog;

public class BlogListDto
{
    public int GeneratedBlogId { get; set; }
    public int BlogRequestId { get; set; }
    public string Title { get; set; }

    public string ContentPreview { get; set; }

    public string Category { get; set; }

    public string Audience { get; set; }

    public string Tone { get; set; }

    public DateTime GeneratedAt { get; set; }
}