using Microsoft.EntityFrameworkCore;
using TTS.Models;

namespace TTS.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SpeechLog> SpeechLogs { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
