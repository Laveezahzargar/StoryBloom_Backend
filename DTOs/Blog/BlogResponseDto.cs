namespace P9_Blog_Generator_AI_Backend.DTOs.Blog;

public class BlogResponseDto
{
    public int GeneratedBlogId { get; set; }
    public int BlogRequestId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; }
}