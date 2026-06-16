namespace P9_Blog_Generator_AI_Backend.Services.Interfaces;

public interface IAIService
{
    Task<(string Title, string Content)> GenerateBlogAsync(
        string category,
        string topic,
        string audience,
        string tone,
        int wordCount);
}