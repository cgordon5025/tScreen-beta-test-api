SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('dbo.fDumpSessionLastByStudent') IS NOT NULL
BEGIN
    DROP FUNCTION dbo.fDumpSessionLastByStudent
END
GO

CREATE FUNCTION [dbo].[fDumpSessionLastByStudent] (
    @studentId UNIQUEIDENTIFIER
)
    RETURNS TABLE AS
        RETURN
        WITH SessionDump AS (SELECT S.Id                                             AS [SessionId],
                                    A.QuestionId                                     AS [QuestionId],
                                    A.Id                                             AS [AnswerId],
                                    Q.Category                                       AS [Category],
                                    Q.Title                                          AS [Title],
                                    Q.Type                                           AS [Type],
                                    A.Data                                           AS [Data],
                                    A.CreatedAt                                      AS [CreatedAt],
                                    S.[Checkpoint]                                   AS [Checkpoint],
                                    IIF(QC.ParentId IS NOT NULL, 'Contingent', NULL) AS [HeirarchyType],
                                    Q.Position                                       AS [QuestionPosition]
                             FROM TweenScreenApp.Session S
                                      INNER JOIN TweenScreenApp.Answer A on S.Id = A.SessionId
                                      INNER JOIN TweenScreenCore.Question Q ON A.QuestionId = Q.Id
                                      LEFT JOIN TweenScreenCore.QuestionContingent QC ON Qc.QuestionId = A.QuestionId
                             WHERE S.Id = (SELECT TOP 1 Id FROM TweenScreenApp.Session WHERE StudentId = @studentId 
                                                                                       ORDER BY CreatedAt DESC))
        SELECT SD.SessionId,
               SD.QuestionId,
               SD.AnswerId,
               SD.Category,
               SD.Title,
               SD.Type AS QuestionType,
               SD.Data,
               SD.CreatedAt,
               SD.[Checkpoint],
               SD.HeirarchyType,
               SD.QuestionPosition,
               DD.DuplicatedAnswers
        FROM SessionDump SD
                 INNER JOIN (SELECT QuestionId, COUNT(*) DuplicatedAnswers
                             FROM SessionDump
                             GROUP BY QuestionId) DD ON SD.QuestionId = DD.QuestionId