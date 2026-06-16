namespace P9_Blog_Generator_AI_Backend.DTOs.Blog;

public class GenerateBlogDto
{
    public string Category { get; set; } = string.Empty;

    public string Topic { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Tone { get; set; } = string.Empty;

    public int WordCount { get; set; }
}