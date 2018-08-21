using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using <%=assemblyName%>.Common;
using <%=assemblyName%>.Data.Model;

namespace <%=assemblyName%>.Data
{
    public sealed class NpgSqlContext : DbContextBase, IDesignTimeDbContextFactory<NpgSqlContext>
    {
        public NpgSqlContext() : base()
        {
        }

        public NpgSqlContext(DbContextOptions<NpgSqlContext> options) : base(options)
        {
        }

        public NpgSqlContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NpgSqlContext>();

            optionsBuilder.UseNpgsql(this.DesignTimeConfig?.ConnectionString, o =>
            {
                string assemblyName = typeof(NpgSqlContext).GetAssemblyName();
                o.MigrationsAssembly(assemblyName);
            });

            return new NpgSqlContext(optionsBuilder.Options);
        }

        protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.BeforeModelCreated(modelBuilder);
            CreateModel(modelBuilder, this.DesignTimeConfig?.SchemaName);
            base.AfterModelCreated(modelBuilder);
        }

        private static void CreateModel(ModelBuilder modelBuilder, string schemaName)
        {
            modelBuilder.Entity<Provider>(entity =>
            {
                entity.Property(e => e.ProviderId).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.Enabled);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK_RoleId");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.ParentRoleId)
                    .HasMaxLength(32);

                entity.HasOne(e => e.Parent)
                    .WithMany()
                    .HasForeignKey(o => o.ParentRoleId)
                    .IsRequired(false);

                entity.Property(e => e.Enabled);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.CreatedOn)
                    .IsRequired()
                    .HasColumnType("timestamp WITH TIME ZONE")
                    .HasDefaultValueSql("current_timestamp AT TIME ZONE 'UTC'");

                entity.Property(e => e.LastUpdatedOn)
                    .HasColumnType("timestamp WITH TIME ZONE");

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(o => o.CreatedBy)
                    .IsRequired();

                entity.HasOne(e => e.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(o => o.LastUpdatedBy)
                    .IsRequired(false);
            });

            modelBuilder.Entity<SecurityClaim>(entity =>
            {
                entity.Property(e => e.SecurityClaimId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.CreatedOn)
                    .IsRequired()
                    .HasColumnType("timestamp WITH TIME ZONE")
                    .HasDefaultValueSql("current_timestamp AT TIME ZONE 'UTC'");

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(o => o.CreatedBy)
                    .IsRequired();

                entity.HasOne(e => e.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(o => o.LastUpdatedBy)
                    .IsRequired(false);
            });

            modelBuilder.Entity<RoleSecurityClaim>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.SecurityClaimId })
                    .HasName("PK_RoleSecurityClaim");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.HasOne(e => e.Role)
                    .WithMany(p => p.SecurityClaims)
                    .HasForeignKey(o => o.RoleId);

                entity.Property(e => e.SecurityClaimId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.HasOne(e => e.SecurityClaim)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(o => o.SecurityClaimId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                string sql = modelBuilder.CreateNpgSequence("user_seq", schemaName);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasDefaultValueSql(sql);

                entity.Property(e => e.CreatedOn)
                    .IsRequired()
                    .HasColumnType("timestamp WITH TIME ZONE")
                    .HasDefaultValueSql("current_timestamp AT TIME ZONE 'UTC'");

                entity.Property(e => e.CultureName)
                    .IsRequired();

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Enabled);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.TimeZoneId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(o => o.CreatedBy)
                    .IsRequired(false);

                entity.HasOne(e => e.LastUpdatedByUser)
                    .WithMany()
                    .HasForeignKey(o => o.LastUpdatedBy)
                    .IsRequired(false);
            });

            modelBuilder.Entity<UserProvider>(entity =>
            {
                entity.HasKey(e => new { e.ProviderId, e.UserId })
                    .HasName("PK_UserProvider");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserProvider_UserId");

                entity.Property(e => e.ProviderId)
                    .HasMaxLength(64);

                entity.Property(e => e.CreatedOn)
                    .IsRequired()
                    .HasColumnType("timestamp WITH TIME ZONE")
                    .HasDefaultValueSql("current_timestamp AT TIME ZONE 'UTC'");

                entity.Property(e => e.ExternalId)
                    .HasMaxLength(64);

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ProviderId)
                    .HasConstraintName("FK_UserProvider_Provider");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Providers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserProvider_User");

                entity.HasDiscriminator<string>("UserProviderType")
                   .HasValue<UserProvider>("External")
                   .HasValue<UserProviderLocal>("Local");
            });

            modelBuilder.Entity<Verification>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_Verification");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_Verification_UserId");

                entity.Property(e => e.Code)
                    .IsRequired(true)
                    .HasMaxLength(64);

                entity.Property(e => e.Fingerprint)
                    .IsRequired(true)
                    .HasMaxLength(256);

                entity.Property(e => e.ProviderKey)
                    .IsRequired(true)
                    .HasMaxLength(64);

                entity.Property(e => e.IssuedAt)
                    .IsRequired()
                    .HasColumnType("timestamp WITH TIME ZONE")
                    .HasDefaultValueSql("current_timestamp AT TIME ZONE 'UTC'");

                entity.Property(e => e.RedeemedAt)
                    .IsRequired(false)
                    .HasColumnType("timestamp WITH TIME ZONE");

                entity.HasOne(d => d.User)
                    .WithMany(d => d.Verifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Verification_User");
            });

            modelBuilder.Entity<UserProviderLocal>(entity =>
            {
                entity.Property(e => e.PasswordSalt)
                    .HasMaxLength(128);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(256);

            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.UserId })
                    .HasName("PK_UserRole");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserRole_UserId");

                entity.Property(e => e.RoleId).HasMaxLength(32);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId) 
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserRole_User");
            });
        }
    }
}