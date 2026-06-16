using P9_Blog_Generator_AI_Backend.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace P9_Blog_Generator_AI_Backend.Services.Implementations;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AIService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    public async Task<(string Title, string Content)>
    GenerateBlogAsync(
    string category,
    string topic,
    string audience,
    string tone,
    int wordCount)
    {
        var prompt = $@"
Write a professional blog.

Category: {category}
Topic: {topic}
Audience: {audience}
Tone: {tone}
Word Count: {wordCount}

Return only valid HTML using:
<h1>
<h2>
<p>
<ul>
<li>

Do not include markdown symbols.
";

        var apiKey = _configuration["Gemini:ApiKey"];

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
            new
            {
                parts = new[]
                {
                    new
                    {
                        text = prompt
                    }
                }
            }
        }
        };

        var json = JsonSerializer.Serialize(requestBody);

        var response = await _httpClient.PostAsync(
            url,
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json"));

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(result);

        var content =
            doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

        return
        (
            $"Blog About {topic}",
            content ?? "No content generated."
        );
    }
}