using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Entities
{
    public class MessagesDbContext : DbContext
    {
        private const string ConnectionString = "Server={localDataBase}\\SQLEXPRESS;Database=MessagesDb;Trusted_Connection=True;";

        public DbSet<EncryptedMessage> EncryptedMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EncryptedMessage>()
                .Property(r => r.Id)
                .IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}