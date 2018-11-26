using Microsoft.EntityFrameworkCore;

namespace Tamagotchi.Authorization.Models
{
    public partial class UserContext : DbContext
    {
        public UserContext()
        {
        }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ConfirmationCode> ConfirmationCode { get; set; }
        public virtual DbSet<User> TamagotchiUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                //entity.Property(e => e.Login).HasMaxLength(24);
            });
            modelBuilder.Entity<ConfirmationCode>(entity =>
            {
                //entity.Property(e => e.CodeValue).HasMaxLength(24);
                //entity.Property(e => e.Email).HasMaxLength(50);
            });
        }
    }
}
