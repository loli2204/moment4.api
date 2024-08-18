using Microsoft.EntityFrameworkCore;

namespace MinimalSongApi.Models
{
    public class SongContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }

        public SongContext(DbContextOptions<SongContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>().HasData(
                
            );
        }
    }
}
