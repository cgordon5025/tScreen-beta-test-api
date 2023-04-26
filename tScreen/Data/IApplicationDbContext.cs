using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Company { get; set; }
        DbSet<Person> Person { get; set; }
        DbSet<Location> Location { get; set; }
        DbSet<LocationPerson> LocationPerson { get; set; }
        DbSet<PersonStudent> PersonStudent { get; set; }
        DbSet<File> File { get; set; }
        DbSet<WorkList> WorkList { get; set; }
        DbSet<WorkListNote> WorkListNote { get; set; }
        DbSet<Note> Note { get; set; }
        DbSet<SessionNote> SessionNote { get; set; }
        DbSet<History> History { get; set; }
        DbSet<HistoryWorkList> HistoryWorkList { get; set; }
        DbSet<HistoryPerson> HistoryPerson { get; set; }
        DbSet<HistoryStudent> HistoryStudent { get; set; }
        DbSet<CustomField> CustomField { get; set; }
        DbSet<Student> Student { get; set; }
        DbSet<StudentCustomField> StudentCustomField { get; set; }

        // ==================================================================================
        // Core Schema
        // ==================================================================================

        DbSet<Domain.Entities.Core.Adventure> CoreAdventures { get; set; }
        DbSet<Domain.Entities.Core.CoreFile> CoreFiles { get; set; }
        DbSet<Domain.Entities.Core.Scene> CoreScenes { get; set; }
        DbSet<Domain.Entities.Core.SceneQuestion> CoreSceneQuestions { get; set; }
        DbSet<Domain.Entities.Core.Question> CoreQuestions { get; set; }
        DbSet<Domain.Entities.Core.QuestionContingent> CoreQuestionContingents { get; set; }

        // ==================================================================================
        // App Schema
        // ==================================================================================

        DbSet<Domain.Entities.App.Avatar> AppAvatars { get; set; }
        DbSet<Domain.Entities.App.Answer> AppAnswers { get; set; }
        DbSet<Domain.Entities.App.Session> AppSessions { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}