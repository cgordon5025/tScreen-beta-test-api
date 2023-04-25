using Domain.Common;
using Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;

namespace Domain.Configurations
{
    public static class ConfigureCoreTablesExtensions
    {
        public static void ConfigureCoreTables(this ModelBuilder builder)
        {
            builder.Entity<Adventure>(entity =>
            {
                entity.ToTable(nameof(Adventure), Schema.TweenScreenCore);

                entity.Property(e => e.Name)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.Name)
                    .HasMaxLength(FieldDefaults.StandardDescriptionSize);

                entity
                    .Property(e => e.Position)
                    .HasDefaultValue(0);
                
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.CoreFile)
                    .WithMany(e => e.Adventures)
                    .HasForeignKey(e => e.FileId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<Question>(entity =>
            {
                entity.ToTable(nameof(Question), Schema.TweenScreenCore);

                entity.Property(e => e.Category)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.Type)
                    .HasMaxLength(FieldDefaults.StandardStringSize);
                
                entity.Property(e => e.Title)
                    .HasMaxLength(FieldDefaults.StandardStringSize);
                
                entity.Property(e => e.Body)
                    .HasMaxLength(FieldDefaults.StandardDescriptionSize);

                entity
                    .Property(e => e.Position)
                    .HasDefaultValue(0);
                
                ConfigureTable.StandardEntityFields(entity);
            });
            
            builder.Entity<Scene>(entity =>
            {
                entity.ToTable(nameof(Scene), Schema.TweenScreenCore);

                entity.Property(e => e.Name)
                    .HasMaxLength(FieldDefaults.StandardStringSize);
                
                entity.Property(e => e.Name)
                    .HasMaxLength(FieldDefaults.StandardDescriptionSize);

                entity
                    .Property(e => e.Position)
                    .HasDefaultValue(0);
                
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.Adventure)
                    .WithMany(e => e.Scenes)
                    .HasForeignKey(e => e.AdventureId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<SceneQuestion>(entity =>
            {
                ConfigureTable.StandardEntityFields(entity);
                
                entity.HasOne(e => e.Scene)
                    .WithMany(e => e.SceneQuestions)
                    .HasForeignKey(e => e.SceneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Question)
                    .WithMany(e => e.SceneQuestions)
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<QuestionContingent>(entity =>
            {
                entity.ToTable(nameof(QuestionContingent), Schema.TweenScreenCore);

                entity
                    .Property(e => e.Position)
                    .HasDefaultValue(0);
                
                entity.Property(e => e.Rule)
                    .HasMaxLength(FieldDefaults.StandardStringSize);

                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.Question)
                    .WithMany(e => e.QuestionContingents)
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.QuestionParent)
                    .WithMany(e => e.QuestionParentContingents)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<CoreFile>(entity =>
            {
                entity.ToTable(nameof(CoreFile), Schema.TweenScreenCore);

                entity.Property(e => e.Category)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.MimeType)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.BlobName)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.FileName)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.FileSize)
                    .IsRequired();
                
                entity.Property(e => e.FileHash)
                    .HasMaxLength(128)
                    .IsRequired();

                entity.Property(e => e.BlurHash)
                    .HasMaxLength(FieldDefaults.StandardStringSize);

                entity.Property(e => e.Formats)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.DisplayName)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.Description)
                    .HasMaxLength(FieldDefaults.StandardDescriptionSize)
                    .IsRequired();
                
                entity.Property(e => e.StorageAccount)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.StorageContainer)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                ConfigureTable.StandardEntityFields(entity);
            });
        }
    }
}