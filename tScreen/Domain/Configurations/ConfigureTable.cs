using Domain.Inferfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Principal;

namespace Domain.Configurations
{
    public static class ConfigureTable
    {
        public static void StandardEntityFields<TEntity>(EntityTypeBuilder<TEntity> entity)
            where TEntity : class, IEntity
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.DeletedAt);
            entity.HasIndex(e => new { e.Id, e.DeletedAt });

            entity
                .Property(e => e.Id)
                .HasDefaultValueSql(FieldDefaults.MssqlNewSequentialId)
                .IsRequired();

            entity
                .Property(e => e.Status)
                .HasMaxLength(FieldDefaults.StandardStringSize);

            entity
                .Property(e => e.CreatedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2)
                .HasDefaultValueSql(FieldDefaults.MssqlDefaultUtcDate)
                .IsRequired();

            entity
                .Property(e => e.UpdatedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2);

            entity
                .Property(e => e.ArchivedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2);

            entity
                .Property(e => e.DeletedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2);

            entity.HasQueryFilter(e => e.DeletedAt == null);
        }

        public static void StandardDataFields<TEntity>(EntityTypeBuilder<TEntity> entity)
            where TEntity : class, IEntityData
        {
            entity
                .Property(e => e.Status)
                .HasMaxLength(FieldDefaults.StandardStringSize);

            entity
                .Property(e => e.CreatedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2)
                .HasDefaultValueSql(FieldDefaults.MssqlDefaultUtcDate)
                .IsRequired();

            entity
                .Property(e => e.UpdatedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2);

            entity
                .Property(e => e.ArchivedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2);

            entity
                .Property(e => e.DeletedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2);

            entity.HasQueryFilter(e => e.DeletedAt == null);
        }

        public static void StandardDataFieldsWithoutId<TEntity>(EntityTypeBuilder<TEntity> entity)
            where TEntity : class, IEntity
        {
            entity
                .Property(e => e.Status)
                .HasMaxLength(FieldDefaults.StandardStringSize);

            entity
                .Property(e => e.CreatedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2)
                .HasDefaultValueSql(FieldDefaults.MssqlDefaultUtcDate)
                .IsRequired();

            entity
                .Property(e => e.UpdatedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2);

            entity
                .Property(e => e.ArchivedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2);

            entity
                .Property(e => e.DeletedAt)
                .HasColumnType(FieldDefaults.TypeDateTime2);

            entity.HasQueryFilter(e => e.DeletedAt == null);
        }
    }
}