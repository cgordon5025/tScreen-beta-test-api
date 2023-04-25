using Domain.Entities;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Domain.Configurations
{
    public static class ConfigureAdminTablesExtensions
    {
        public static void ConfigureAdminTables(this ModelBuilder builder)
        {
            builder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Type)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.Slug)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.Name)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(FieldDefaults.StandardDescriptionSize);
                
                ConfigureTable.StandardEntityFields(entity);
            });
            
            builder.Entity<Location>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(FieldDefaults.StandardDescriptionSize);
                
                entity.Property(e => e.StreetLineOne)
                    .HasMaxLength(FieldDefaults.StandardStringSize);
                
                entity.Property(e => e.StreetLineTwo)
                    .HasMaxLength(FieldDefaults.StandardStringSize);
                
                entity.Property(e => e.City)
                    .HasMaxLength(FieldDefaults.StandardStringSize);
                
                entity.Property(e => e.Country)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.Company)
                    .WithMany(e => e.Locations)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<CustomField>(entity =>
            {
                entity.HasIndex(e => new { e.LocationId, e.DeletedAt });
                
                entity.Property(e => e.Type)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.Name)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(FieldDefaults.StandardDescriptionSize);
                
                entity.Property(e => e.PlaceHolder)
                    .HasMaxLength(FieldDefaults.StandardStringSize);
                
                entity.Property(e => e.DefaultValue)
                    .HasMaxLength(FieldDefaults.StandardStringSize);
                
                entity.Property(e => e.ValidationRule)
                    .HasMaxLength(3000);

                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.Location)
                    .WithMany(e => e.CustomFields)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<StudentCustomField>(entity =>
            {
                entity.HasKey(e => new { e.CustomFieldId, e.StudentId });
                entity.HasIndex(e => e.CustomFieldId);
                
                entity.Property(e => e.Value)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.CustomField)
                    .WithMany(e => e.StudentCustomFields)
                    .HasForeignKey(e => e.CustomFieldId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.StudentCustomFields)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Person>(entity =>
            {
                entity.HasIndex(e => new { e.Id, e.DeletedAt });
                
                ConfigureTable.StandardDataFieldsWithoutId(entity);
                
                entity.HasKey(e => e.Id);
                
                // Cannot have a person record without a user ID. The user ID is the primary key
                // for the person table, thus having a 1:1 relationship. Remove auto generation
                // to help mitigate inserting just a person record.
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                // entity.HasNoKey();
                
                // entity.Property(e => e.IdentityId)
                //     .HasMaxLength(FieldDefaults.StandardStringSize)
                //     .IsRequired();
                //
                // entity.Property(e => e.IdentityType)
                //     .HasMaxLength(FieldDefaults.StandardStringSize)
                //     .IsRequired();

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(255);
                
                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(255);

                entity.Property(e => e.Dob)
                    .HasColumnType("DateTime2");

                entity.Property(e => e.StartDate)
                    .HasColumnType("DateTime2");
                
                entity.Property(e => e.EndDate)
                    .HasColumnType("DateTime2");

                entity.HasOne(e => e.Company)
                    .WithMany(e => e.Persons)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithOne(e => e.Person)
                    .HasForeignKey<Person>(e => e.Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<File>(entity =>
            {
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

                entity.HasOne(e => e.Location)
                    .WithMany(e => e.Files)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<Student>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(FieldDefaults.StandardStringSize);

                entity.Property(e => e.LastName)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(FieldDefaults.StandardStringSize);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(20);
                
                entity.Property(e => e.Dob)
                    .HasColumnType(FieldDefaults.TypeDateTime2);

                entity.Property(e => e.MinimumProvided)
                    .HasDefaultValue(false)
                    .IsRequired();
                
                ConfigureTable.StandardEntityFields(entity);
            });

            builder.Entity<PersonStudent>(entity =>
            {
                entity.HasIndex(e => new { e.PersonId, e.DeletedAt });
                entity.HasIndex(e => new { e.StudentId, e.DeletedAt });
                entity.HasIndex(e => e.PersonId);
                entity.HasIndex(e => e.StudentId);

                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.Person)
                    .WithMany(e => e.PersonStudents)
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.PersonStudents)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<LocationPerson>(entity =>
            {
                entity.HasIndex(e => new { e.LocationId, e.DeletedAt });
                
                ConfigureTable.StandardEntityFields(entity);

                entity.Property(e => e.Type)
                    .HasMaxLength(255);

                entity.HasOne(e => e.Location)
                    .WithMany(e => e.LocationPersons)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Person)
                    .WithMany(e => e.LocationPersons)
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<WorkList>(entity =>
            {
                entity.HasIndex(e => new { e.SessionId, e.PersonId });
                
                entity.Property(e => e.Type)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.Person)
                    .WithMany(e => e.WorkLists)
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<Note>(entity =>
            {
                entity.Property(e => e.Type)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                entity.Property(e => e.Body)
                    .HasMaxLength(3000)
                    .IsRequired();

                ConfigureTable.StandardEntityFields(entity);
            });

            builder.Entity<WorkListNote>(entity =>
            {
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.WorkList)
                    .WithMany(e => e.WorkListNotes)
                    .HasForeignKey(e => e.WorkListId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Note)
                    .WithMany(e => e.WorkListNotes)
                    .HasForeignKey(e => e.NoteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<ReportVersion>(entity => 
            {
                entity.Property(e => e.ClassName)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.MethodName)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                entity.Property(e => e.Version)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();
                
                ConfigureTable.StandardEntityFields(entity);
            });

            builder.Entity<SessionNote>(entity =>
            {
                entity.HasIndex(e => new { e.SessionId, e.DeletedAt });
                
                ConfigureTable.StandardEntityFields(entity);
                
                entity.HasOne(e => e.Session)
                    .WithMany(e => e.SessionNotes)
                    .HasForeignKey(e => e.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Note)
                    .WithMany(e => e.SessionNotes)
                    .HasForeignKey(e => e.NoteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<History>(entity =>
            {
                entity.Property(e => e.Type)
                    .HasMaxLength(FieldDefaults.StandardStringSize)
                    .IsRequired();

                ConfigureTable.StandardEntityFields(entity);
            });

            builder.Entity<HistoryWorkList>(entity =>
            {
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.History)
                    .WithMany(e => e.HistoryWorkLists)
                    .HasForeignKey(e => e.HistoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.WorkList)
                    .WithMany(e => e.HistoryWorkLists)
                    .HasForeignKey(e => e.WorkListId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<HistoryPerson>(entity =>
            {
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.History)
                    .WithMany(e => e.HistoryPersons)
                    .HasForeignKey(e => e.HistoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Person)
                    .WithMany(e => e.HistoryPersons)
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<HistoryStudent>(entity =>
            {
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.History)
                    .WithMany(e => e.HistoryStudents)
                    .HasForeignKey(e => e.HistoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Student)
                    .WithMany(e => e.HistoryStudents)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<HistorySession>(entity =>
            {
                ConfigureTable.StandardEntityFields(entity);

                entity.HasOne(e => e.History)
                    .WithMany(e => e.HistorySessions)
                    .HasForeignKey(e => e.HistoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Session)
                    .WithMany(e => e.HistorySessions)
                    .HasForeignKey(e => e.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}