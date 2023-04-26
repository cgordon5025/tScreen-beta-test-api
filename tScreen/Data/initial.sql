-- Must be run once MSSQL server is stood up, before running migrations.
-- Run the line of code below separately from the rest
CREATE DATABASE TWS;

-- Run all lines of code below under the newly create database TWS
CREATE SCHEMA TweenScreenCore AUTHORIZATION dbo
GO
CREATE SCHEMA TweenScreenApp AUTHORIZATION dbo
GO
ALTER AUTHORIZATION ON DATABASE::TWS TO SA
GO
ALTER AUTHORIZATION ON SCHEMA::TweenScreenCore TO dbo
GO
ALTER AUTHORIZATION ON SCHEMA::TweenScreenApp TO dbo
GO;