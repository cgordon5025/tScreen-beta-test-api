IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    IF SCHEMA_ID(N'TweenScreenCore') IS NULL EXEC(N'CREATE SCHEMA [TweenScreenCore];');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    IF SCHEMA_ID(N'TweenScreenApp') IS NULL EXEC(N'CREATE SCHEMA [TweenScreenApp];');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[Company] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [Type] nvarchar(255) NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Slug] nvarchar(255) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Company] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [TweenScreenCore].[CoreFile] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [LocationId] uniqueidentifier NOT NULL,
        [Category] nvarchar(255) NOT NULL,
        [MimeType] nvarchar(255) NOT NULL,
        [BlobName] nvarchar(255) NOT NULL,
        [FileName] nvarchar(255) NOT NULL,
        [FileSize] int NOT NULL,
        [FileHash] nvarchar(128) NOT NULL,
        [BlurHash] nvarchar(255) NULL,
        [Formats] nvarchar(1000) NULL,
        [DisplayName] nvarchar(255) NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [StorageAccount] nvarchar(255) NOT NULL,
        [StorageContainer] nvarchar(255) NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_CoreFile] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[History] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [LocationId] uniqueidentifier NOT NULL,
        [PersonId] uniqueidentifier NOT NULL,
        [Type] nvarchar(255) NOT NULL,
        [Data] nvarchar(max) NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_History] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[Note] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [WorkListId] uniqueidentifier NOT NULL,
        [Type] nvarchar(255) NOT NULL,
        [Body] nvarchar(3000) NOT NULL,
        [Data] nvarchar(max) NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Note] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [TweenScreenCore].[Question] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [Category] nvarchar(255) NOT NULL,
        [Type] nvarchar(255) NULL,
        [Title] nvarchar(255) NULL,
        [Body] nvarchar(2000) NULL,
        [Data] nvarchar(max) NULL,
        [Position] int NOT NULL DEFAULT 0,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Question] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[Student] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [LocationId] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(255) NOT NULL,
        [MiddleName] nvarchar(255) NULL,
        [LastName] nvarchar(255) NOT NULL,
        [Email] nvarchar(255) NULL,
        [Dob] DateTime2 NOT NULL,
        [GradeLevel] int NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Student] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[WorkList] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [LocationId] uniqueidentifier NOT NULL,
        [PersonId] uniqueidentifier NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [Type] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_WorkList] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[Person] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [CompanyId] uniqueidentifier NOT NULL,
        [ExternalId] nvarchar(255) NOT NULL,
        [ExternalType] nvarchar(255) NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Person] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Person_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [TweenScreenCore].[Adventure] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [FileId] uniqueidentifier NOT NULL,
        [Name] nvarchar(2000) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Position] int NOT NULL DEFAULT 0,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Adventure] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Adventure_CoreFile_FileId] FOREIGN KEY ([FileId]) REFERENCES [TweenScreenCore].[CoreFile] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [TweenScreenCore].[QuestionContingent] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [ParentId] uniqueidentifier NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [Type] nvarchar(255) NOT NULL,
        [Rule] nvarchar(255) NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_QuestionContingent] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionContingent_Question_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [TweenScreenCore].[Question] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionContingent_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [TweenScreenCore].[Question] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [TweenScreenApp].[Avatar] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [StudentId] uniqueidentifier NOT NULL,
        [BodyId] uniqueidentifier NOT NULL,
        [BodyColor] nvarchar(max) NULL,
        [EyeId] uniqueidentifier NOT NULL,
        [EyeColor] nvarchar(max) NULL,
        [HairId] uniqueidentifier NOT NULL,
        [HairColor] nvarchar(max) NULL,
        [OutfitId] uniqueidentifier NOT NULL,
        [OutfitColor] nvarchar(max) NULL,
        [HelperId] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Avatar] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Avatar_CoreFile_BodyId] FOREIGN KEY ([BodyId]) REFERENCES [TweenScreenCore].[CoreFile] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Avatar_CoreFile_EyeId] FOREIGN KEY ([EyeId]) REFERENCES [TweenScreenCore].[CoreFile] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Avatar_CoreFile_HairId] FOREIGN KEY ([HairId]) REFERENCES [TweenScreenCore].[CoreFile] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Avatar_CoreFile_HelperId] FOREIGN KEY ([HelperId]) REFERENCES [TweenScreenCore].[CoreFile] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Avatar_CoreFile_OutfitId] FOREIGN KEY ([OutfitId]) REFERENCES [TweenScreenCore].[CoreFile] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Avatar_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Student] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[HistoryStudent] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [HistoryId] uniqueidentifier NOT NULL,
        [StudentId] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_HistoryStudent] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HistoryStudent_History_HistoryId] FOREIGN KEY ([HistoryId]) REFERENCES [dbo].[History] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HistoryStudent_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Student] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[Location] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [CompanyId] uniqueidentifier NOT NULL,
        [Type] nvarchar(max) NULL,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [StreetLineOne] nvarchar(255) NULL,
        [StreetLineTwo] nvarchar(255) NULL,
        [City] nvarchar(255) NULL,
        [State] nvarchar(max) NULL,
        [PostalCode] nvarchar(max) NULL,
        [Country] nvarchar(255) NOT NULL,
        [StudentId] uniqueidentifier NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Location] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Location_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Location_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Student] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[HistoryWorkList] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [HistoryId] uniqueidentifier NOT NULL,
        [WorkListId] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_HistoryWorkList] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HistoryWorkList_History_HistoryId] FOREIGN KEY ([HistoryId]) REFERENCES [dbo].[History] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HistoryWorkList_WorkList_WorkListId] FOREIGN KEY ([WorkListId]) REFERENCES [dbo].[WorkList] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[WorkListNote] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [WorkListId] uniqueidentifier NOT NULL,
        [NoteId] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_WorkListNote] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WorkListNote_Note_NoteId] FOREIGN KEY ([NoteId]) REFERENCES [dbo].[Note] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_WorkListNote_WorkList_WorkListId] FOREIGN KEY ([WorkListId]) REFERENCES [dbo].[WorkList] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[HistoryPerson] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [HistoryId] uniqueidentifier NOT NULL,
        [PersonId] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_HistoryPerson] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HistoryPerson_History_HistoryId] FOREIGN KEY ([HistoryId]) REFERENCES [dbo].[History] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HistoryPerson_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[PersonStudent] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [PersonId] uniqueidentifier NOT NULL,
        [StudentId] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_PersonStudent] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PersonStudent_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PersonStudent_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Student] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [TweenScreenCore].[Scene] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [AdventureId] uniqueidentifier NOT NULL,
        [Name] nvarchar(2000) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Position] int NOT NULL DEFAULT 0,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Scene] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Scene_Adventure_AdventureId] FOREIGN KEY ([AdventureId]) REFERENCES [TweenScreenCore].[Adventure] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [TweenScreenApp].[Session] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [StudentId] uniqueidentifier NOT NULL,
        [PersonId] uniqueidentifier NOT NULL,
        [AdventureId] uniqueidentifier NOT NULL,
        [AvatarId] uniqueidentifier NOT NULL,
        [FinishedAt] DateTime2 NOT NULL,
        [RiskRating] int NOT NULL,
        [Code] nvarchar(255) NOT NULL,
        [Data] nvarchar(max) NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Session] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Session_Adventure_AdventureId] FOREIGN KEY ([AdventureId]) REFERENCES [TweenScreenCore].[Adventure] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Session_Avatar_AvatarId] FOREIGN KEY ([AvatarId]) REFERENCES [TweenScreenApp].[Avatar] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Session_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Session_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Student] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[CustomField] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [LocationId] uniqueidentifier NOT NULL,
        [Type] nvarchar(255) NOT NULL,
        [Position] int NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [PlaceHolder] nvarchar(255) NULL,
        [DefaultValue] nvarchar(255) NULL,
        [ValidationRule] nvarchar(3000) NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_CustomField] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CustomField_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Location] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[File] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [LocationId] uniqueidentifier NOT NULL,
        [Category] nvarchar(255) NOT NULL,
        [MimeType] nvarchar(255) NOT NULL,
        [BlobName] nvarchar(255) NOT NULL,
        [FileName] nvarchar(255) NOT NULL,
        [FileSize] int NOT NULL,
        [FileHash] nvarchar(128) NOT NULL,
        [BlurHash] nvarchar(255) NULL,
        [Formats] nvarchar(1000) NULL,
        [DisplayName] nvarchar(255) NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [StorageAccount] nvarchar(255) NOT NULL,
        [StorageContainer] nvarchar(255) NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_File] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_File_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Location] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[LocationPerson] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [LocationId] uniqueidentifier NOT NULL,
        [PersonId] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_LocationPerson] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LocationPerson_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Location] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LocationPerson_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [TweenScreenCore].[SceneQuestion] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [SceneId] uniqueidentifier NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_SceneQuestion] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SceneQuestion_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [TweenScreenCore].[Question] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SceneQuestion_Scene_SceneId] FOREIGN KEY ([SceneId]) REFERENCES [TweenScreenCore].[Scene] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [TweenScreenApp].[Answer] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [SessionId] uniqueidentifier NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [Data] nvarchar(max) NULL,
        [SentimentAnalysisData] nvarchar(max) NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_Answer] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Answer_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [TweenScreenCore].[Question] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Answer_Session_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [TweenScreenApp].[Session] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[SessionNote] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [NoteId] uniqueidentifier NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_SessionNote] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SessionNote_Note_NoteId] FOREIGN KEY ([NoteId]) REFERENCES [dbo].[Note] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SessionNote_Session_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [TweenScreenApp].[Session] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE TABLE [dbo].[StudentCustomField] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [StudentId] uniqueidentifier NOT NULL,
        [CustomFieldId] uniqueidentifier NOT NULL,
        [Value] nvarchar(255) NOT NULL,
        [Status] nvarchar(255) NULL,
        [CreatedAt] DateTime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DateTime2 NULL,
        [ArchivedAt] DateTime2 NULL,
        [DeletedAt] DateTime2 NULL,
        CONSTRAINT [PK_StudentCustomField] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentCustomField_CustomField_CustomFieldId] FOREIGN KEY ([CustomFieldId]) REFERENCES [dbo].[CustomField] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentCustomField_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Student] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Adventure_FileId] ON [TweenScreenCore].[Adventure] ([FileId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Answer_QuestionId] ON [TweenScreenApp].[Answer] ([QuestionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Answer_SessionId] ON [TweenScreenApp].[Answer] ([SessionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Avatar_BodyId] ON [TweenScreenApp].[Avatar] ([BodyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Avatar_EyeId] ON [TweenScreenApp].[Avatar] ([EyeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Avatar_HairId] ON [TweenScreenApp].[Avatar] ([HairId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Avatar_HelperId] ON [TweenScreenApp].[Avatar] ([HelperId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Avatar_OutfitId] ON [TweenScreenApp].[Avatar] ([OutfitId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Avatar_StudentId] ON [TweenScreenApp].[Avatar] ([StudentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_CustomField_LocationId] ON [dbo].[CustomField] ([LocationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_File_LocationId] ON [dbo].[File] ([LocationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_HistoryPerson_HistoryId] ON [dbo].[HistoryPerson] ([HistoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_HistoryPerson_PersonId] ON [dbo].[HistoryPerson] ([PersonId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_HistoryStudent_HistoryId] ON [dbo].[HistoryStudent] ([HistoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_HistoryStudent_StudentId] ON [dbo].[HistoryStudent] ([StudentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_HistoryWorkList_HistoryId] ON [dbo].[HistoryWorkList] ([HistoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_HistoryWorkList_WorkListId] ON [dbo].[HistoryWorkList] ([WorkListId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Location_CompanyId] ON [dbo].[Location] ([CompanyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Location_StudentId] ON [dbo].[Location] ([StudentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_LocationPerson_LocationId] ON [dbo].[LocationPerson] ([LocationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_LocationPerson_PersonId] ON [dbo].[LocationPerson] ([PersonId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Person_CompanyId] ON [dbo].[Person] ([CompanyId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_PersonStudent_PersonId] ON [dbo].[PersonStudent] ([PersonId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_PersonStudent_StudentId] ON [dbo].[PersonStudent] ([StudentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_QuestionContingent_ParentId] ON [TweenScreenCore].[QuestionContingent] ([ParentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_QuestionContingent_QuestionId] ON [TweenScreenCore].[QuestionContingent] ([QuestionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Scene_AdventureId] ON [TweenScreenCore].[Scene] ([AdventureId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_SceneQuestion_QuestionId] ON [TweenScreenCore].[SceneQuestion] ([QuestionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_SceneQuestion_SceneId] ON [TweenScreenCore].[SceneQuestion] ([SceneId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Session_AdventureId] ON [TweenScreenApp].[Session] ([AdventureId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Session_AvatarId] ON [TweenScreenApp].[Session] ([AvatarId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Session_PersonId] ON [TweenScreenApp].[Session] ([PersonId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_Session_StudentId] ON [TweenScreenApp].[Session] ([StudentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_SessionNote_NoteId] ON [dbo].[SessionNote] ([NoteId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_SessionNote_SessionId] ON [dbo].[SessionNote] ([SessionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_StudentCustomField_CustomFieldId] ON [dbo].[StudentCustomField] ([CustomFieldId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_StudentCustomField_StudentId] ON [dbo].[StudentCustomField] ([StudentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_WorkListNote_NoteId] ON [dbo].[WorkListNote] ([NoteId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    CREATE INDEX [IX_WorkListNote_WorkListId] ON [dbo].[WorkListNote] ([WorkListId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102134214_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211102134214_Initial', N'5.0.11');
END;
GO

COMMIT;
GO

