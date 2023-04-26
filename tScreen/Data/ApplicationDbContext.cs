using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Domain.Configurations;
using Domain.Entities;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext<
        User,
        Role,
        Guid,
        UserClaim,
        UserRole,
        UserLogin,
        RoleClaim,
        UserToken
    >
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        // ==================================================================================
        // Admin Schema
        // ==================================================================================

        public DbSet<Company> Company { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<LocationPerson> LocationPerson { get; set; }
        public DbSet<PersonStudent> PersonStudent { get; set; }
        public DbSet<File> File { get; set; }
        public DbSet<WorkList> WorkList { get; set; }
        public DbSet<WorkListNote> WorkListNote { get; set; }
        public DbSet<Note> Note { get; set; }
        public DbSet<SessionNote> SessionNote { get; set; }
        public DbSet<History> History { get; set; }
        public DbSet<HistoryWorkList> HistoryWorkList { get; set; }
        public DbSet<HistoryPerson> HistoryPerson { get; set; }
        public DbSet<HistoryStudent> HistoryStudent { get; set; }
        public DbSet<CustomField> CustomField { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<StudentCustomField> StudentCustomField { get; set; }

        // ==================================================================================
        // Core Schema
        // ==================================================================================

        public DbSet<Domain.Entities.Core.Adventure> CoreAdventures { get; set; }
        public DbSet<Domain.Entities.Core.CoreFile> CoreFiles { get; set; }
        public DbSet<Domain.Entities.Core.Scene> CoreScenes { get; set; }
        public DbSet<Domain.Entities.Core.SceneQuestion> CoreSceneQuestions { get; set; }
        public DbSet<Domain.Entities.Core.Question> CoreQuestions { get; set; }
        public DbSet<Domain.Entities.Core.QuestionContingent> CoreQuestionContingents { get; set; }

        // ==================================================================================
        // App Schema
        // ==================================================================================

        public DbSet<Domain.Entities.App.Avatar> AppAvatars { get; set; }
        public DbSet<Domain.Entities.App.Answer> AppAnswers { get; set; }
        public DbSet<Domain.Entities.App.Session> AppSessions { get; set; }
        public DbSet<Domain.Entities.App.File> AppSessionFile { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        // ==================================================================================
        // Protected methods
        // ==================================================================================

        // Enforce common conventions for string and datetime types. Enable at a future date.
        // protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        // {
        //     configurationBuilder
        //         .Properties<string>()
        //         .AreUnicode(true)
        //         .HaveMaxLength(255);
        //
        //     configurationBuilder
        //         .Properties<DateTime>()
        //         .HaveColumnType("DateTime2");
        // }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("dbo");

            builder.ConfigureIdentityTables();
            builder.ConfigureAdminTables();
            builder.ConfigureCoreTables();
            builder.ConfigureAppTables();
        }
    }
}