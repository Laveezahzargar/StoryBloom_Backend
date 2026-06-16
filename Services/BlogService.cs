using Microsoft.EntityFrameworkCore;
using P9_Blog_Generator_AI_Backend.Data;
using P9_Blog_Generator_AI_Backend.DTOs.Blog;
using P9_Blog_Generator_AI_Backend.Models;
using P9_Blog_Generator_AI_Backend.Services.Interfaces;

namespace P9_Blog_Generator_AI_Backend.Services.Implementations;

public class BlogService : IBlogService
{
    private readonly ApplicationDbContext _context;
    private readonly IAIService _aiService;
    public BlogService(ApplicationDbContext dbContext, IAIService aiService)
    {
        _context = dbContext;
        _aiService = aiService;
    }
    public async Task<BlogResponseDto> GenerateBlogAsync(
     int userId,
     GenerateBlogDto dto)
    {
        var blogRequest = new BlogRequest
        {
            UserId = userId,
            Category = dto.Category,
            Topic = dto.Topic,
            Audience = dto.Audience,
            Tone = dto.Tone,
            WordCount = dto.WordCount
        };

        _context.BlogRequests.Add(blogRequest);
        await _context.SaveChangesAsync();

        var aiResult = await _aiService.GenerateBlogAsync(
            dto.Category,
            dto.Topic,
            dto.Audience,
            dto.Tone,
            dto.WordCount);

        var generatedBlog = new GeneratedBlog
        {
            BlogRequestId = blogRequest.BlogRequestId,
            Title = aiResult.Title,
            Content = aiResult.Content
        };

        _context.GeneratedBlogs.Add(generatedBlog);
        await _context.SaveChangesAsync();

        return new BlogResponseDto
        {
            GeneratedBlogId = generatedBlog.GeneratedBlogId,
            Title = generatedBlog.Title,
            Content = generatedBlog.Content,
            GeneratedAt = generatedBlog.GeneratedAt
        };
    }
    public async Task<List<BlogListDto>> GetMyBlogsAsync(
    int userId)
    {
        return await _context.GeneratedBlogs
            .Where(g => g.BlogRequest!.UserId == userId)
            .Select(g => new BlogListDto
            {
                GeneratedBlogId = g.GeneratedBlogId,
                Title = g.Title,
                GeneratedAt = g.GeneratedAt
            })
            .ToListAsync();
    }
    public async Task<BlogResponseDto?> GetBlogByIdAsync(
    int blogId)
    {
        return await _context.GeneratedBlogs
            .Where(g => g.GeneratedBlogId == blogId)
            .Select(g => new BlogResponseDto
            {
                GeneratedBlogId = g.GeneratedBlogId,
                Title = g.Title,
                Content = g.Content,
                GeneratedAt = g.GeneratedAt
            })
            .FirstOrDefaultAsync();
    }
    public async Task<bool> DeleteBlogAsync(
    int blogId)
    {
        var blog = await _context.GeneratedBlogs
            .FirstOrDefaultAsync(g =>
                g.GeneratedBlogId == blogId);

        if (blog == null)
            return false;

        _context.GeneratedBlogs.Remove(blog);

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<BlogResponseDto> RegenerateBlogAsync(
    int blogRequestId)
    {
        var request = await _context.BlogRequests
            .FirstOrDefaultAsync(br =>
                br.BlogRequestId == blogRequestId);

        if (request == null)
            throw new Exception("Blog request not found.");

        var aiResult = await _aiService.GenerateBlogAsync(
            request.Category,
            request.Topic,
            request.Audience,
            request.Tone,
            request.WordCount);

        var generatedBlog = new GeneratedBlog
        {
            BlogRequestId = request.BlogRequestId,
            Title = aiResult.Title,
            Content = aiResult.Content
        };

        _context.GeneratedBlogs.Add(generatedBlog);

        await _context.SaveChangesAsync();

        return new BlogResponseDto
        {
            GeneratedBlogId = generatedBlog.GeneratedBlogId,
            Title = generatedBlog.Title,
            Content = generatedBlog.Content,
            GeneratedAt = generatedBlog.GeneratedAt
        };
    }
}