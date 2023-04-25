SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('dbo.fDurationSessionAll') IS NOT NULL
BEGIN
    DROP FUNCTION dbo.fDurationSessionAll
END
GO

CREATE FUNCTION [dbo].[fDurationSessionAll] (
    @sessionId UNIQUEIDENTIFIER
)
    RETURNS TABLE AS
        RETURN
            (
                SELECT RIGHT('0' + CAST(DurationInSeconds / 3600 AS NVARCHAR(5)), 2) + ':' +
                       RIGHT('0' + CAST((DurationInSeconds % 3600) / 60 AS NVARCHAR(5)), 2) + ':' +
                       RIGHT('0' + CAST(DurationInSeconds % 60 AS NVARCHAR(5)), 2) AS SessionDuration
                FROM (SELECT DATEDIFF(SECOND, MIN(H.CreatedAt), MAX(H.CreatedAt)) AS DurationInSeconds
                      FROM TweenScreenApp.Session S
                               INNER JOIN dbo.HistorySession HS ON S.Id = HS.SessionId
                               INNER JOIN dbo.History H ON HS.HistoryId = H.Id
                      WHERE S.Id = @sessionId) A
            )
        