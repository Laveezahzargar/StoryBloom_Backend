using P9_Blog_Generator_AI_Backend.DTOs.Blog;

namespace P9_Blog_Generator_AI_Backend.Services.Interfaces;

public interface IBlogService
{
    Task<BlogResponseDto> GenerateBlogAsync(
        int userId,
        GenerateBlogDto dto);

    Task<List<BlogListDto>> GetMyBlogsAsync(
    int userId);

    Task<BlogResponseDto?> GetBlogByIdAsync(
        int blogId);

    Task<bool> DeleteBlogAsync(
        int blogId);

    Task<BlogResponseDto> RegenerateBlogAsync(
        int blogRequestId);
}