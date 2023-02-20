using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class BookCollectionDbContext : DbContext
    {
        protected readonly IConfiguration configuration;

        public BookCollectionDbContext(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(configuration.GetConnectionString("BookDb"));
        }

        public DbSet<Book> books { get; set; }

    }
}
