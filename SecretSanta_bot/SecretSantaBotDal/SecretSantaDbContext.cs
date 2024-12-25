using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SecretSatnaBotDal;

public partial class SecretSantaDbContext : DbContext
{
    public SecretSantaDbContext()
    {
    }

    public SecretSantaDbContext(DbContextOptions<SecretSantaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<Gamer> Gamers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=E:\\1111\\c#-projects\\Rider projects\\SecretSantaBot\\SecretSantaBotDb\\SecretSantaDb.sqlite");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasIndex(e => e.AdminId, "IX_Admins_adminId").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AdminId).HasColumnName("adminId");
            entity.Property(e => e.ChatId).HasColumnName("chatId");
            entity.Property(e => e.Gamestatus).HasColumnName("gamestatus");
            entity.Property(e => e.Maxprice).HasColumnName("maxprice");
        });

        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Gamer>(entity =>
        {
            entity.ToTable("gamers");

            entity.HasIndex(e => e.GiftTo, "IX_gamers_giftTo").IsUnique();

            entity.HasIndex(e => e.UserId, "IX_gamers_userId").IsUnique();

            entity.HasIndex(e => e.Username, "IX_gamers_username").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChatId).HasColumnName("chatId");
            entity.Property(e => e.Gift).HasColumnName("gift");
            entity.Property(e => e.GiftTo).HasColumnName("giftTo");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
