using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P9_Blog_Generator_AI_Backend.DTOs.Blog;
using P9_Blog_Generator_AI_Backend.Services.Interfaces;

namespace P9_Blog_Generator_AI_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateBlog(
        GenerateBlogDto dto)
    {
        int userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!
                .Value);

        var result = await _blogService.GenerateBlogAsync(
            userId,
            dto);

        return Ok(result);
    }

    [HttpPost("regenerate/{blogRequestId}")]
    public async Task<IActionResult> RegenerateBlog(
        int blogRequestId)
    {
        var result =
            await _blogService.RegenerateBlogAsync(
                blogRequestId);

        return Ok(result);
    }

    [HttpGet("myblogs")]
    public async Task<IActionResult> GetMyBlogs()
    {
        int userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!
                .Value);

        var blogs =
            await _blogService.GetMyBlogsAsync(
                userId);

        return Ok(blogs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBlogById(
        int id)
    {
        var blog =
            await _blogService.GetBlogByIdAsync(id);

        if (blog == null)
        {
            return NotFound();
        }

        return Ok(blog);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(
        int id)
    {
        var deleted =
            await _blogService.DeleteBlogAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok("Blog deleted successfully.");
    }
}