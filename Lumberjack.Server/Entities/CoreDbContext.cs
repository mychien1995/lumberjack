using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Lumberjack.Server.Entities;

#nullable disable

namespace Lumberjack.Server
{
    public partial class CoreDbContext : DbContext
    {

        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiKey> ApiKeys { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<ApplicationInstance> ApplicationInstances { get; set; }
        public virtual DbSet<LogData> LogDatas { get; set; }
        public virtual DbSet<Shard> Shards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApiKeys)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("FK_ApiKey_Application");
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasIndex(e => e.ApplicationCode, "UQ__Applicat__1185325AAD48511B")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ApplicationCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ApplicationName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ApplicationInstance>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.InstanceName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationInstances)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("FK_Instance_Application");
            });

            modelBuilder.Entity<LogData>(entity =>
            {
                entity.HasIndex(e => e.ApplicationId, "IX_ApplicationId");

                entity.HasIndex(e => new { e.ApplicationId, e.LogLevel }, "IX_ApplicationId_LogLevel");

                entity.HasIndex(e => new { e.ApplicationId, e.Timestamp }, "IX_ApplicationId_Timestamp");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Instance)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Namespace)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Request)
                    .HasMaxLength(2000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Shard>(entity =>
            {
                entity.Property(e => e.ShardId).ValueGeneratedNever();

                entity.Property(e => e.TableName)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
