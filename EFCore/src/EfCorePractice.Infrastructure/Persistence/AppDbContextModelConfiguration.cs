using System.Text.Json;
using EfCorePractice.Application.Abstractions;
using EfCorePractice.Domain.Entities;
using EfCorePractice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfCorePractice.Infrastructure.Persistence;

internal static class AppDbContextModelConfiguration
{
    public static void Configure(ModelBuilder modelBuilder, ITenantContext tenantContext)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Version).HasColumnName("version").IsConcurrencyToken();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            entity.Property(e => e.Contact)
                .HasColumnName("contact")
                .HasColumnType("longtext")
                .HasConversion(
                    profile => JsonSerializer.Serialize(profile, jsonOptions),
                    json => JsonSerializer.Deserialize<UserContactProfile>(json, jsonOptions) ?? new UserContactProfile(),
                    new ValueComparer<UserContactProfile>(
                        (left, right) => JsonSerializer.Serialize(left, jsonOptions) == JsonSerializer.Serialize(right, jsonOptions),
                        profile => JsonSerializer.Serialize(profile, jsonOptions).GetHashCode(),
                        profile => JsonSerializer.Deserialize<UserContactProfile>(
                            JsonSerializer.Serialize(profile, jsonOptions), jsonOptions)!));

            entity.HasMany(e => e.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(u =>
                !u.IsDeleted &&
                (!tenantContext.HasTenant || u.TenantId == tenantContext.TenantId));
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.OrderNo).HasColumnName("order_no").HasMaxLength(50).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.HasIndex(e => e.UserId);
        });
    }
}
