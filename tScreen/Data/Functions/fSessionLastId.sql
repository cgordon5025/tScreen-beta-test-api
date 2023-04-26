SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.fSessionLastId') IS NOT NULL
    BEGIN
        DROP FUNCTION dbo.fSessionLastId
    END
GO

CREATE FUNCTION dbo.fSessionLastId()
    RETURNS UNIQUEIDENTIFIER AS
BEGIN
    DECLARE @Id UNIQUEIDENTIFIER;
    SELECT TOP 1 @id = Id FROM TweenScreenApp.Session ORDER BY CreatedAt DESC;
    return @id;
END;