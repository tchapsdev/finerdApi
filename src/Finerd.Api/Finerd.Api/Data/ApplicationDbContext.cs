using Finerd.Api.Model;
using Finerd.Api.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Finerd.Api.Data
{
   
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TransactionType> TransactionTypes { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(e => e.ExpiryDate).HasColumnType("smalldatetime");
                entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.TokenSalt).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Ts).HasColumnType("smalldatetime").HasColumnName("TS");
                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RefreshToken_User");
                entity.ToTable("RefreshToken");
            });
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Description).IsRequired().HasMaxLength(100);
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_User");
                entity.ToTable("Transaction");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordSalt).IsRequired().HasMaxLength(255);
                entity.ToTable("User");

            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(75);
                entity.ToTable("Category");
            });
            modelBuilder.Entity<TransactionType>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(75);
                entity.ToTable("TransactionType");
            });
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(75);
                entity.ToTable("PaymentMethod");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
