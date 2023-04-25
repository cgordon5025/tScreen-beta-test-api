using System.Text.Json;
using Core;
using Domain.Common;
using Domain.Entities.App;
using Microsoft.EntityFrameworkCore;

namespace Domain.Configurations
{
    public static class ConfigureAppTablesExtensions
    {
        public static void ConfigureAppTables(this ModelBuilder builder)
        {
            builder.Entity<Avatar>(entity =>
            {
                entity.ToTable(nameof(Avatar), Schema.TweenScreenApp);
                entity.HasIndex(e => new { e.StudentId, e.DeletedAt });
                
                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasDefaultValue(AvatarTypes.SystemDefined)
                    .IsRequired();

                entity.Property(e => e.BodyColor)
                    .HasMaxLength(10);
                
                entity.Property(e => e.EyeColor)
                    .HasMaxLength(10);
                
                entity.Property(e => e.HairColor)
                    .HasMaxLength(10);
                
                entity.Property(e => e.ShirtColor)
                    .HasMaxLength(10);
                
                entity.Property(e => e.PantsColor)
                    .HasMaxLength(10);
                
                entity.Property(e => e.ShoesColor)
                    .HasMaxLength(10);
                
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.Avatars)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.BodyCoreFile)
                    .WithMany(e => e.AvatarBodies)
                    .HasForeignKey(e => e.BodyId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.EyeCoreFile)
                    .WithMany(e => e.AvatarEyes)
                    .HasForeignKey(e => e.EyeId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.HairCoreFile)
                    .WithMany(e => e.AvatarHair)
                    .HasForeignKey(e => e.HairId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.OutfitCoreFile)
                    .WithMany(e => e.AvatarOutfits)
                    .HasForeignKey(e => e.OutfitId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.HelperCoreFile)
                    .WithMany(e => e.AvatarHelpers)
                    .HasForeignKey(e => e.HelperId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<File>(entity =>
            {
                entity.ToTable("File", Schema.TweenScreenApp);
                
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

                entity.HasOne(e => e.Session)
                    .WithMany(e => e.Files)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<Session>(entity =>
            {
                entity.ToTable(nameof(Session), Schema.TweenScreenApp);

                entity.HasIndex(e => new { e.StudentId, e.Checkpoint, e.DeletedAt });
                
                entity.Property(e => e.FinishedAt)
                    .HasColumnType(FieldDefaults.TypeDateTime2);

                entity.Property(e => e.Type)
                    .HasMaxLength(255);
                
                entity.Property(e => e.Checkpoint)
                    .HasDefaultValue(SessionCheckpoints.Initiated)
                    .HasMaxLength(255)
                    .IsRequired();
                
                entity.Property(e => e.Code)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                entity.Property(e => e.Data)
                    .HasConversion(
                        from => Utility.SerializeObject(from),
                        to => Utility.DeserializeObject<object>(to, null));
                
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.Sessions)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Person)
                    .WithMany(e => e.Sessions)
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Adventure)
                    .WithMany(e => e.Sessions)
                    .HasForeignKey(e => e.AdventureId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Avatar)
                    .WithMany(e => e.Sessions)
                    .HasForeignKey(e => e.AvatarId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<Answer>(entity =>
            {
                entity.ToTable(nameof(Answer), Schema.TweenScreenApp);
                entity.HasIndex(e => new { e.SessionId, e.QuestionId, e.DeletedAt });
                
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.Session)
                    .WithMany(e => e.Answers)
                    .HasForeignKey(e => e.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Question)
                    .WithMany(e => e.Answers)
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}