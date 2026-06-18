using Microsoft.EntityFrameworkCore;
using P9_Blog_Generator_AI_Backend.Data;
using P9_Blog_Generator_AI_Backend.DTOs.Blog;
using P9_Blog_Generator_AI_Backend.Models;
using P9_Blog_Generator_AI_Backend.Services.Interfaces;
//using Serilog;


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
    //    Log.Information(
    //"Blog generation started. UserId: {UserId}, Topic: {Topic}",
    //userId, dto.Topic);
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

    //    Log.Information(
    //"AI content generated successfully for Topic: {Topic}",
    //dto.Topic);

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

    //    Log.Information(
    //"Blog saved successfully. BlogId: {BlogId}, UserId: {UserId}",
    //generatedBlog.GeneratedBlogId, userId);

        return new BlogResponseDto
        {
            GeneratedBlogId = generatedBlog.GeneratedBlogId,
            BlogRequestId = generatedBlog.BlogRequestId,
            Title = generatedBlog.Title,
            Content = generatedBlog.Content,
            GeneratedAt = generatedBlog.GeneratedAt
        };
    }
    public async Task<List<BlogListDto>> GetMyBlogsAsync(
    int userId)
    {
    //    Log.Information(
    //"Fetching blogs for UserId: {UserId}",
    //userId);

        var blogs= await _context.GeneratedBlogs
            .Where(g => g.BlogRequest!.UserId == userId)
            .Select(g => new BlogListDto
            {
                GeneratedBlogId = g.GeneratedBlogId,

                BlogRequestId = g.BlogRequestId,

                Title = g.Title,

                ContentPreview =
                g.Content.Length > 100
                    ? g.Content.Substring(0, 100) + "..."
                    : g.Content,

                Category = g.BlogRequest.Category,

                Audience = g.BlogRequest.Audience,

                Tone = g.BlogRequest.Tone,

                GeneratedAt = g.GeneratedAt
            })
        .ToListAsync();

    //    Log.Information(
    //"Retrieved {Count} blogs for UserId: {UserId}",
    //blogs.Count, userId);
        return blogs;

    }
    public async Task<BlogResponseDto?> GetBlogByIdAsync(
    int blogId)
    {
    //    Log.Information(
    //"Fetching blog details. BlogId: {BlogId}",
    //blogId);
        return await _context.GeneratedBlogs
            .Where(g => g.GeneratedBlogId == blogId)
            .Select(g => new BlogResponseDto
            {
                GeneratedBlogId = g.GeneratedBlogId,
                BlogRequestId = g.BlogRequestId,
                Title = g.Title,
                Content = g.Content,
                GeneratedAt = g.GeneratedAt
            })
            .FirstOrDefaultAsync();
    }
    public async Task<bool> DeleteBlogAsync(
    int blogId)
    {
    //    Log.Information(
    //"Delete blog request received. BlogId: {BlogId}",
    //blogId);

        var blog = await _context.GeneratedBlogs
            .FirstOrDefaultAsync(g =>
                g.GeneratedBlogId == blogId);

        if (blog == null)
        {
   //         Log.Warning(
   //"Delete failed. Blog not found. BlogId: {BlogId}",
   //blogId);
            return false;
        }

        _context.GeneratedBlogs.Remove(blog);

        await _context.SaveChangesAsync();

    //    Log.Information(
    //"Blog deleted successfully. BlogId: {BlogId}",
    //blogId);

        return true;
    }
    public async Task<BlogResponseDto> RegenerateBlogAsync(
    int blogRequestId)
    {
    //    Log.Information(
    //"Blog regeneration started. BlogRequestId: {BlogRequestId}",
    //blogRequestId);

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
 //       Log.Information(
 //"AI regenerated content successfully. BlogRequestId: {BlogRequestId}",
 //blogRequestId);


        _context.GeneratedBlogs.Add(generatedBlog);

        await _context.SaveChangesAsync();

        //Log.Information(
        //    "Regenerated blog saved. New BlogId: {BlogId}",
        //    generatedBlog.GeneratedBlogId);

        return new BlogResponseDto
        {
            GeneratedBlogId = generatedBlog.GeneratedBlogId,
            BlogRequestId = generatedBlog.BlogRequestId,
            Title = generatedBlog.Title,
            Content = generatedBlog.Content,
            GeneratedAt = generatedBlog.GeneratedAt
        };
    }
}