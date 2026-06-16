namespace P9_Blog_Generator_AI_Backend.DTOs.Blog;

public class BlogListDto
{
    public int GeneratedBlogId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; }
}