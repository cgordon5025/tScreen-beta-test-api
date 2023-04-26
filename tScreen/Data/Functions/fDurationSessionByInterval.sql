SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('dbo.fDurationSessionByInterval') IS NOT NULL
BEGIN
    DROP FUNCTION dbo.fDurationSessionByInterval
END
GO

CREATE FUNCTION dbo.fDurationSessionByInterval(
    @sessionId UNIQUEIDENTIFIER
)
    RETURNS TABLE AS
        RETURN
            (
                SELECT A.*,
                       RIGHT('0' + CAST(IntervalDurationInSeconds / 3600 AS NVARCHAR(5)), 2) + ':' +
                       RIGHT('0' + CAST((IntervalDurationInSeconds % 3600) / 60 AS NVARCHAR(5)), 2) + ':' +
                       RIGHT('0' + CAST(IntervalDurationInSeconds % 60 AS NVARCHAR(5)), 2) AS [IntervalDuration],

                       RIGHT('0' + CAST(SessionDurationInSeconds / 3600 AS NVARCHAR(5)), 2) + ':' +
                       RIGHT('0' + CAST((SessionDurationInSeconds % 3600) / 60 AS NVARCHAR(5)), 2) + ':' +
                       RIGHT('0' + CAST(SessionDurationInSeconds % 60 AS NVARCHAR(5)), 2)  AS [SessionDuration]
                FROM (SELECT S.[Id]                                                                            AS [SessionId],
                             H.[Data]                                                                          AS [Data],
                             S.[CreatedAt]                                                                     AS [SessionCreatedAt],
                             H.[CreatedAt]                                                                     AS [HistoryCreatedAt],
                             DATEDIFF(SECOND, LAG(H.[CreatedAt]) OVER (ORDER BY H.[CreatedAt]),
                                      H.[CreatedAt])                                                           AS [IntervalDurationInSeconds],
                             DATEDIFF(SECOND, S.[CreatedAt], H.[CreatedAt])                                    AS [SessionDurationInSeconds]
                      FROM [TweenScreenApp].[Session] S
                               INNER JOIN [dbo].[HistorySession] HS ON S.[Id] = HS.[SessionId]
                               INNER JOIN [dbo].[History] H ON HS.[HistoryId] = H.[Id]
                      WHERE S.Id = @sessionId) A
            )