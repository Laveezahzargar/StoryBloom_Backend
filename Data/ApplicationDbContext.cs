

using Microsoft.EntityFrameworkCore;
using P9_Blog_Generator_AI_Backend.Models;

namespace P9_Blog_Generator_AI_Backend.Data;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<BlogRequest> BlogRequests { get; set; }
    public DbSet<GeneratedBlog> GeneratedBlogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.BlogRequests)
            .WithOne(br => br.User)
            .HasForeignKey(br => br.UserId);

        modelBuilder.Entity<BlogRequest>()
            .HasMany(br => br.GeneratedBlogs)
            .WithOne(gb => gb.BlogRequest)
            .HasForeignKey(gb => gb.BlogRequestId);
    }
}


