-- Role data
INSERT INTO [Identity].[Role] ([Id], [Name], [NormalizedName], [ConcurrencyStamp], [CreatedAt])
VALUES ('E91893DB-7346-4083-A5F9-0BC10ED437B7', 'Owner', 'OWNER', '262F7FD3-5D59-4870-B0ED-8598094943BC', '2022-01-15 18:40:16.6233333'),
    ('EA1893DB-7346-4083-A5F9-0BC10ED437B7', 'Admin', 'ADMIN', '02089166-856D-445D-9DCE-1929D365355E', '2022-01-15 18:40:16.6500000'),
    ('7DE5146C-634F-4154-97A6-D237300856C5', 'User', 'USER', '7EE5146C-634F-4154-97A6-D237300856C5', '2022-01-15 18:40:16.6500000'),
    ('80E5146C-634F-4154-97A6-D237300856C5', 'Player', 'PLAYER', '81E5146C-634F-4154-97A6-D237300856C5', '2022-01-15 18:40:16.6500000');

-- Company Data
INSERT INTO [dbo].[Company] ([Id], [Type], [Name], [Slug], [Description])
VALUES ('4BE54D47-6D54-408F-B9CA-CBF727A6C372', 'School', 'Acme School', 'acme-school', 'Test company for the school'),
    ('4CE54D47-6D54-408F-B9CA-CBF727A6C372', 'Practice', 'Acme Practice', 'acme-practice', 'Test company for the practice type'),
    ('4DE54D47-6D54-408F-B9CA-CBF727A6C372', 'Hospital', 'Acme Hospital', 'acme-hospital', 'Test company for the hospital type');

-- Company associated locations
INSERT INTO [dbo].[Location] ([Id], [CompanyId], [Type], [Name], [Description], [Country])
VALUES ('5B82736F-AFDF-4C05-B3DF-13533AF542BA', '4BE54D47-6D54-408F-B9CA-CBF727A6C372', NULL, 'Default', 'Default location', 'USA'),
    ('5C82736F-AFDF-4C05-B3DF-13533AF542BA', '4CE54D47-6D54-408F-B9CA-CBF727A6C372', NULL, 'Default', 'Default location', 'USA'),
    ('5D82736F-AFDF-4C05-B3DF-13533AF542BA', '4DE54D47-6D54-408F-B9CA-CBF727A6C372', NULL, 'Default', 'Default location', 'USA');
-- Custom fields
-- INSERT INTO [dbo].[CustomField] ([Id], [LocationId], [Type], [Position], [Name], [Description], [PlaceHolder])
-- VALUES ('FCAE2E3C-DF54-4B9C-AC5C-2EB9C774B135', '5B82736F-AFDF-4C05-B3DF-13533AF542BA', 'Text', 0, 'Color', 'Place for the Student''s favorite color', 'Your favorite color');

-- User
INSERT [Identity].[User] ([Id],[CompanyId],[LastSignedIn],[Status],[CreatedAt],[UpdatedAt],[ArchivedAt],[DeletedAt],[UserName],[NormalizedUserName],[Email],[NormalizedEmail],[EmailConfirmed],[PasswordHash],[SecurityStamp],[ConcurrencyStamp],[PhoneNumber],[PhoneNumberConfirmed],[TwoFactorEnabled],[LockoutEnd],[LockoutEnabled],[AccessFailedCount])
SELECT '82AC433E-F36B-1410-8A0D-00F2E42120C8','4BE54D47-6D54-408F-B9CA-CBF727A6C372','0001-01-01 00:00:00.0000000',NULL,'2022-01-20 21:18:25.9633333','2022-01-20 21:25:06.7792660',NULL,NULL,N'test@test.com',N'TEST@TEST.COM',N'test@test.com',N'TEST@TEST.COM',1,N'AQAAAAEAACcQAAAAECQ6YTVcC8kKyqLvtHB8gAoAskZGq63eXwuhfHs/aL/PaII/A+46KgzUj/s1qOvebg==',N'GBTXZ4TIMZONKCVMCR6TGAYYCHYJSLH5',N'6ff52ca6-bf8d-49ee-9404-4c49fc09699a',NULL,0,0,NULL,1,0 UNION ALL
SELECT '86AC433E-F36B-1410-8A0D-00F2E42120C8','4BE54D47-6D54-408F-B9CA-CBF727A6C372','0001-01-01 00:00:00.0000000',NULL,'2022-01-20 21:18:26.3400000',NULL,NULL,NULL,N'admin@test.com',N'ADMIN@TEST.COM',N'admin@test.com',N'ADMIN@TEST.COM',1,N'AQAAAAEAACcQAAAAEF5OGCRck9JIyRvUORERRsvPOf6rnZ3ezNNqoFY+O9MGsiPQpwvQFXdzEDGCPB/U2Q==',N'HJLKSEBSPXJHPURSRT76AFHWIFO674BX',N'75c05592-b19b-44ad-aae5-a12056c4fb6c',NULL,0,0,NULL,1,0 UNION ALL
SELECT '8BAC433E-F36B-1410-8A0D-00F2E42120C8','4BE54D47-6D54-408F-B9CA-CBF727A6C372','0001-01-01 00:00:00.0000000',NULL,'2022-01-20 21:18:26.4866667',NULL,NULL,NULL,N'user@test.com',N'USER@TEST.COM',N'user@test.com',N'USER@TEST.COM',1,N'AQAAAAEAACcQAAAAECd2TFh5nEOakaloRC00sAnE38tUfojecBhu6AgQ1IEMgIisXhPFtA6bZ35ktmnn8Q==',N'AUWKQ7LS6NJ5HFISYXMUBZQYN4H2LT44',N'ea71e572-272a-4432-9c1b-d563f60dfa4f',NULL,0,0,NULL,1,0 UNION ALL
SELECT '90AC433E-F36B-1410-8A0D-00F2E42120C8','4BE54D47-6D54-408F-B9CA-CBF727A6C372','0001-01-01 00:00:00.0000000',NULL,'2022-01-20 21:18:26.5666667',NULL,NULL,NULL,N'player@test.com',N'PLAYER@TEST.COM',N'player@test.com',N'PLAYER@TEST.COM',1,N'AQAAAAEAACcQAAAAEAXOGZtjNmRIzzI3TkuX27S6fEkwJwDQ2NbTFBYLlUsCWEZZzBhV+dAERXqQRBw7kw==',N'D4BCM5CLOHJT2GYSCYFHYQPVAWTT4KMR',N'9399bf19-168b-4633-893d-99595465870e',NULL,0,0,NULL,1,0 UNION ALL
SELECT '92AC433E-F36B-1410-8A0D-00F2E42120C8','4BE54D47-6D54-408F-B9CA-CBF727A6C372','0001-01-01 00:00:00.0000000',NULL,'2022-01-20 21:18:26.7600000','2022-01-20 21:27:21.7414820',NULL,NULL,N'all@test.com',N'ALL@TEST.COM',N'all@test.com',N'ALL@TEST.COM',1,N'AQAAAAEAACcQAAAAEHZw3ESvb8iZAa5Z5eDD9jYZil3ISGlV1s4BdgF3OPPPWj7haI07AMcAdOhMuvpDoQ==',N'CKJHEIDAVESUHNUAS4TAZ357BUO24CYO',N'ff789baa-c558-4ad9-9bb2-99a29299d158',NULL,0,0,NULL,1,0;

-- User associated roles
INSERT [Identity].[UserRole] ([UserId],[RoleId],[Status],[CreatedAt],[UpdatedAt],[ArchivedAt],[DeletedAt])
SELECT '82AC433E-F36B-1410-8A0D-00F2E42120C8','E91893DB-7346-4083-A5F9-0BC10ED437B7',NULL,'2022-01-20 21:18:26.1700000',NULL,NULL,NULL UNION ALL
SELECT '86AC433E-F36B-1410-8A0D-00F2E42120C8','EA1893DB-7346-4083-A5F9-0BC10ED437B7',NULL,'2022-01-20 21:18:26.4566667',NULL,NULL,NULL UNION ALL
SELECT '8BAC433E-F36B-1410-8A0D-00F2E42120C8','7DE5146C-634F-4154-97A6-D237300856C5',NULL,'2022-01-20 21:18:26.5333333',NULL,NULL,NULL UNION ALL
SELECT '90AC433E-F36B-1410-8A0D-00F2E42120C8','80E5146C-634F-4154-97A6-D237300856C5',NULL,'2022-01-20 21:18:26.6633333',NULL,NULL,NULL UNION ALL
SELECT '92AC433E-F36B-1410-8A0D-00F2E42120C8','EA1893DB-7346-4083-A5F9-0BC10ED437B7',NULL,'2022-01-20 21:18:26.8800000',NULL,NULL,NULL UNION ALL
SELECT '92AC433E-F36B-1410-8A0D-00F2E42120C8','7DE5146C-634F-4154-97A6-D237300856C5',NULL,'2022-01-20 21:18:26.8800000',NULL,NULL,NULL UNION ALL
SELECT '92AC433E-F36B-1410-8A0D-00F2E42120C8','80E5146C-634F-4154-97A6-D237300856C5',NULL,'2022-01-20 21:18:26.8800000',NULL,NULL,NULL;

-- User associated claims
SET IDENTITY_INSERT [Identity].[UserClaim] ON
INSERT [Identity].[UserClaim] ([Id],[Description],[Status],[CreatedAt],[UpdatedAt],[ArchivedAt],[DeletedAt],[UserId],[ClaimType],[ClaimValue])
SELECT 9, NULL,NULL,'2022-01-20 21:18:26.2733333',NULL,NULL,NULL,'82AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.read' UNION ALL
SELECT 10,NULL,NULL,'2022-01-20 21:18:26.2733333',NULL,NULL,NULL,'82AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.update' UNION ALL
SELECT 11,NULL,NULL,'2022-01-20 21:18:26.2733333',NULL,NULL,NULL,'82AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.delete' UNION ALL
SELECT 12,NULL,NULL,'2022-01-20 21:18:26.2733333',NULL,NULL,NULL,'82AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.archive' UNION ALL
SELECT 13,NULL,NULL,'2022-01-20 21:18:26.4700000',NULL,NULL,NULL,'86AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.read' UNION ALL
SELECT 14,NULL,NULL,'2022-01-20 21:18:26.4700000',NULL,NULL,NULL,'86AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.update' UNION ALL
SELECT 15,NULL,NULL,'2022-01-20 21:18:26.4700000',NULL,NULL,NULL,'86AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.delete' UNION ALL
SELECT 16,NULL,NULL,'2022-01-20 21:18:26.4700000',NULL,NULL,NULL,'86AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.archive' UNION ALL
SELECT 17,NULL,NULL,'2022-01-20 21:18:26.5466667',NULL,NULL,NULL,'8BAC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.read' UNION ALL
SELECT 18,NULL,NULL,'2022-01-20 21:18:26.5466667',NULL,NULL,NULL,'8BAC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.update' UNION ALL
SELECT 19,NULL,NULL,'2022-01-20 21:18:26.5466667',NULL,NULL,NULL,'8BAC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.delete' UNION ALL
SELECT 20,NULL,NULL,'2022-01-20 21:18:26.5466667',NULL,NULL,NULL,'8BAC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.archive' UNION ALL
SELECT 21,NULL,NULL,'2022-01-20 21:18:26.7033333',NULL,NULL,NULL,'90AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.read' UNION ALL
SELECT 22,NULL,NULL,'2022-01-20 21:18:26.7033333',NULL,NULL,NULL,'90AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.update' UNION ALL
SELECT 23,NULL,NULL,'2022-01-20 21:18:26.7033333',NULL,NULL,NULL,'90AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.delete' UNION ALL
SELECT 24,NULL,NULL,'2022-01-20 21:18:26.7033333',NULL,NULL,NULL,'90AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.archive' UNION ALL
SELECT 25,NULL,NULL,'2022-01-20 21:18:26.9200000',NULL,NULL,NULL,'92AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.read' UNION ALL
SELECT 26,NULL,NULL,'2022-01-20 21:18:26.9200000',NULL,NULL,NULL,'92AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.update' UNION ALL
SELECT 27,NULL,NULL,'2022-01-20 21:18:26.9200000',NULL,NULL,NULL,'92AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.delete' UNION ALL
SELECT 28,NULL,NULL,'2022-01-20 21:18:26.9200000',NULL,NULL,NULL,'92AC433E-F36B-1410-8A0D-00F2E42120C8',N'Scope',N'user.archive';
SET IDENTITY_INSERT [Identity].[UserClaim] OFF;

-- User/Person association
INSERT INTO [dbo].[Person] ([Id], [CompanyId], [FirstName], [LastName])
VALUES ('82AC433E-F36B-1410-8A0D-00F2E42120C8', '4BE54D47-6D54-408F-B9CA-CBF727A6C372', 'Nikola', 'Tesla'),
    ('86AC433E-F36B-1410-8A0D-00F2E42120C8', '4BE54D47-6D54-408F-B9CA-CBF727A6C372', 'Albert', 'Einstein'),
    ('8BAC433E-F36B-1410-8A0D-00F2E42120C8', '4BE54D47-6D54-408F-B9CA-CBF727A6C372', 'Ada', 'Lovelace'),
    ('90AC433E-F36B-1410-8A0D-00F2E42120C8', '4BE54D47-6D54-408F-B9CA-CBF727A6C372', 'Thomas', 'Edison'),
    ('92AC433E-F36B-1410-8A0D-00F2E42120C8', '4BE54D47-6D54-408F-B9CA-CBF727A6C372', 'John', 'Doe');

-- Associate users with location
INSERT INTO [dbo].[LocationPerson] ([Id], [LocationId], [Type], [PersonId], [CreatedAt])
VALUES ('03570C3F-4BEA-434F-86B7-81F6D2688510', '5B82736F-AFDF-4C05-B3DF-13533AF542BA', 'Default', '82AC433E-F36B-1410-8A0D-00F2E42120C8', '2021-11-03 18:08:25.6900000'),
    ('04570C3F-4BEA-434F-86B7-81F6D2688510', '5B82736F-AFDF-4C05-B3DF-13533AF542BA', 'Default', '86AC433E-F36B-1410-8A0D-00F2E42120C8', '2021-11-03 18:08:25.6900000'),
    ('05570C3F-4BEA-434F-86B7-81F6D2688510', '5B82736F-AFDF-4C05-B3DF-13533AF542BA', 'Default', '8BAC433E-F36B-1410-8A0D-00F2E42120C8', '2021-11-03 18:08:25.6900000'),
    ('06570C3F-4BEA-434F-86B7-81F6D2688510', '5B82736F-AFDF-4C05-B3DF-13533AF542BA', 'Default', '90AC433E-F36B-1410-8A0D-00F2E42120C8', '2021-11-03 18:08:25.6900000'),
    ('07570C3F-4BEA-434F-86B7-81F6D2688510', '5B82736F-AFDF-4C05-B3DF-13533AF542BA', 'Default', '92AC433E-F36B-1410-8A0D-00F2E42120C8', '2021-11-03 18:08:25.6900000');

-- Students
INSERT INTO [dbo].[Student] ([Id], [LocationId], [FirstName], [LastName], [Email], [Dob], [GradeLevel])
VALUES ('774B87F5-E34F-463A-8332-6F29B6462644', '5B82736F-AFDF-4C05-B3DF-13533AF542BA', 'Andrea', 'Ria', 'andrea.ria@acme.school.com', '2009-01-20', 5),
    ('E4A7049B-5D60-458D-873B-3C8FF7A3A7BF', '5B82736F-AFDF-4C05-B3DF-13533AF542BA', 'Lindsey', 'Maximino', 'andrea.ria@acme.school.com', '2008-01-20', 5),
    ('E5A7049B-5D60-458D-873B-3C8FF7A3A7BF', '5B82736F-AFDF-4C05-B3DF-13533AF542BA', 'Nathan', 'Howard', 'andrea.ria@acme.school.com', '2009-01-20', 5);

-- Associate students with user@test.com
INSERT INTO [dbo].[PersonStudent] ([Id], [PersonId], [StudentId])
VALUES ('E055AB8A-7322-4B9F-8E46-4D5E80ABE227', '8BAC433E-F36B-1410-8A0D-00F2E42120C8', '774B87F5-E34F-463A-8332-6F29B6462644'),
    ('E155AB8A-7322-4B9F-8E46-4D5E80ABE227', '8BAC433E-F36B-1410-8A0D-00F2E42120C8', 'E4A7049B-5D60-458D-873B-3C8FF7A3A7BF'),
    ('E255AB8A-7322-4B9F-8E46-4D5E80ABE227', '8BAC433E-F36B-1410-8A0D-00F2E42120C8', 'E5A7049B-5D60-458D-873B-3C8FF7A3A7BF');

-- Test File (Not associated with any blob storage file)
INSERT INTO [TweenScreenCore].[CoreFile] ([Id], [Category], [MimeType], [BlobName], [FileName], [FileSize], [FileHash], [DisplayName], [Description], [StorageAccount], [StorageContainer])
VALUES ('dced423e-f36b-1410-8a08-00f2e42120c8', 'Default', 'text/none', 'none', 'none', 0, 'none', 'none', 'none', 'none', 'none')

INSERT INTO [TweenScreenCore].[CoreFile] ([Id], [Category], [MimeType], [BlobName], [FileName], [FileSize], [FileHash], [DisplayName], [Description], [StorageAccount], [StorageContainer])
VALUES ('4B5A8DB8-EF10-44A7-86EE-CFC34AB06425', 'Avatar', 'text/none', 'none', 'none', 0, 'none', 'Boy', 'Represents the male/boy avatar', 'none', 'none')
INSERT INTO [TweenScreenCore].[CoreFile] ([Id], [Category], [MimeType], [BlobName], [FileName], [FileSize], [FileHash], [DisplayName], [Description], [StorageAccount], [StorageContainer])
VALUES ('4C5A8DB8-EF10-44A7-86EE-CFC34AB06425', 'Avatar', 'text/none', 'none', 'none', 0, 'none', 'Girl', 'Represents the female/girl avatar', 'none', 'none')
INSERT INTO [TweenScreenCore].[CoreFile] ([Id], [Category], [MimeType], [BlobName], [FileName], [FileSize], [FileHash], [DisplayName], [Description], [StorageAccount], [StorageContainer])
VALUES ('4D5A8DB8-EF10-44A7-86EE-CFC34AB06425', 'Avatar', 'text/none', 'none', 'none', 0, 'none', 'Body part', 'Represents generic body part which is used to compose the avatar e.g., hair, eyes', 'none', 'none')

INSERT INTO [TweenScreenCore].[CoreFile] ([Id], [Category], [MimeType], [BlobName], [FileName], [FileSize], [FileHash], [DisplayName], [Description], [StorageAccount], [StorageContainer])
VALUES ('E7F2E454-C835-4A60-9BF3-6A1FE9C1CC71', 'Helper', 'text/none', 'none', 'none', 0, 'none', 'Crab', 'Represents the Crab adventure helper', 'none', 'none');
INSERT INTO [TweenScreenCore].[CoreFile] ([Id], [Category], [MimeType], [BlobName], [FileName], [FileSize], [FileHash], [DisplayName], [Description], [StorageAccount], [StorageContainer])
VALUES ('E8F2E454-C835-4A60-9BF3-6A1FE9C1CC71', 'Helper', 'text/none', 'none', 'none', 0, 'none', 'Dog', 'Represents the Dog adventure helper', 'none', 'none');
INSERT INTO [TweenScreenCore].[CoreFile] ([Id], [Category], [MimeType], [BlobName], [FileName], [FileSize], [FileHash], [DisplayName], [Description], [StorageAccount], [StorageContainer])
VALUES ('E9F2E454-C835-4A60-9BF3-6A1FE9C1CC71', 'Helper', 'text/none', 'none', 'none', 0, 'none', 'Fox', 'Represents the Fox adventure helper', 'none', 'none');
INSERT INTO [TweenScreenCore].[CoreFile] ([Id], [Category], [MimeType], [BlobName], [FileName], [FileSize], [FileHash], [DisplayName], [Description], [StorageAccount], [StorageContainer])
VALUES ('EAF2E454-C835-4A60-9BF3-6A1FE9C1CC71', 'Helper', 'text/none', 'none', 'none', 0, 'none', 'Owl', 'Represents the Owl adventure helper', 'none', 'none');

-- Adventures
INSERT INTO [TweenScreenCore].[Adventure] ([Id], [FileId], [Name], [Position]) VALUES ('73C15EDF-B24F-4551-93A1-8AD473DA1F35', 'dced423e-f36b-1410-8a08-00f2e42120c8', 'Playground', 0);
INSERT INTO [TweenScreenCore].[Adventure] ([Id], [FileId], [Name], [Position]) VALUES ('74C15EDF-B24F-4551-93A1-8AD473DA1F35', 'dced423e-f36b-1410-8a08-00f2e42120c8', 'Post Apocalyptic ', 1);
INSERT INTO [TweenScreenCore].[Adventure] ([Id], [FileId], [Name], [Position]) VALUES ('75C15EDF-B24F-4551-93A1-8AD473DA1F35', 'dced423e-f36b-1410-8a08-00f2e42120c8', 'Classroom', 2);
INSERT INTO [TweenScreenCore].[Adventure] ([Id], [FileId], [Name], [Position]) VALUES ('76C15EDF-B24F-4551-93A1-8AD473DA1F35', 'dced423e-f36b-1410-8a08-00f2e42120c8', 'Garden', 3);

-- Questions
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('D63A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Environment', 'Multiselect', 1, 'Select all of the family members in your life.');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('D73A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Environment', 'Multiselect', 2, 'Who do you live with?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('D83A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Environment', 'Composite', 3, 'What about your siblings?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('D93A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Environment', 'Multiselect', 4, 'When you think of your parent or guardian, how would you describe their mood in general?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('DA3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Environment', 'Composite', 5, 'Have you experienced a death in your family or of someone close to you?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('DB3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Environment', 'Multiselect', 6, 'Who in your family do you feel you can count on when you need help?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('DC3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Environment', 'Composite', 7, 'Are there people outside your family you count on when you need help?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('DD3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Environment', 'Composite', 8, 'Would you share what part of your life they are in?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('466BB39E-87B7-4CCE-A3DE-4DB60A7C014E', 'Personal', 'Composite', 9, 'Have you experienced a death in your family or of someone close to you?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('476BB39E-87B7-4CCE-A3DE-4DB60A7C014E', 'Personal', 'Composite', 10, 'Does anyone living with you have a serious mental or physical illness?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('486BB39E-87B7-4CCE-A3DE-4DB60A7C014E', 'Personal', 'Scale', 11, 'On a scale of 1-5 (with 5 being the most worried), do you worry about or fear you are treated differently because of your race, skin color, religion, or ethnicity?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('496BB39E-87B7-4CCE-A3DE-4DB60A7C014E', 'Personal', 'Boolean', 12, 'Are you currently seeing a professional for your mental health?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('DE3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Scale', 13, 'On a scale of 1-5 with 5 being the happiest, how happy are you overall?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('DF3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 14, 'What makes you happy?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E03A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 15, 'If there is one thing in your life that you could change to make you happier, what would it be?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E13A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Scale', 16, 'On a scale of 1-5, with 5 being the most tired, how tired do you get during your school day?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E23A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 17, 'What makes you tired during the school day?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E33A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 18, 'If there were one thing in your life that you could change to make you less tired during the school day what would it be?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E43A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Scale', 19, 'On a scale of 1-5, with 5 being the best, how are you doing in school academically?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E53A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 20, 'If there was one thing that you could do to make your school life better what would it be?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E63A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Scale', 21, 'On a scale of 1-5, with 5 being the best, how are you doing in school socially?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E73A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 22, 'If there was one thing that you could do to make your social life better what would it be?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E83A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Boolean', 23, 'Have any of your peers ever said or done anything that hurt you or make you feel bad?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('E93A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 24, 'What happened? What did you do?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('EA3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Scale', 25, 'On a scale of 1-5, with 5 being the most tired, how tired do you get on the weekend?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('07600418-5210-4186-898A-A4C3FE107FE4', 'Survey', 'Freeform', 26, 'What makes you tired on the weekend?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('EB3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 27, 'If there were one thing in your life that you could change to make you less tired on the weekend what would it be?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('EC3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Scale', 28, 'On a scale of 1-5 with 5 being the most sad, how sad are you overall?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('ED3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 29, 'What makes you sad?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('EE3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Boolean', 30, 'Do you have hobbies or special interests that you like?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('EF3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 31, 'What are they?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F03A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 32, 'Is there anything that stops you from doing them?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F13A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Scale', 33, 'On a scale of 1-5 with 5 being the most often, how often do you feel a pit in your stomach or have a stomach ache?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F23A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Scale', 34, 'On a scale of 1-5 with 5 being the most often, how often do you have headaches?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F33A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Multiselect', 35, 'Check all that apply');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F43A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Boolean', 36, 'Do you have thoughts that distract you or make it hard for you to concentrate?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F53A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 37, 'Describe them');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F63A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Scale', 38, 'On a scale of 1-5 with 5 being the most nervous, how nervous are you overall?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F73A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 39, 'What makes you feel nervous?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F83A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 40, 'If there was one thing in your life that you could change to make you feel less nervous what would it be?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('F93A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 41, 'What is your greatest worry or insecurity?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('FA3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Boolean', 42, 'Has this ever stopped you from going to school or out with friends?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('FB3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Boolean', 43, 'Have you ever felt so hopeless that you wanted to die? Have you ever had thoughts or plans of taking your own life?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('FC3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 44, 'Would you like to share what happened?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('FD3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 45, 'What gives you energy?');
INSERT INTO TweenScreenCore.Question ([Id], [Category], [Type], [Position], [Title]) VALUES ('FE3A79ED-B03A-4BE9-A14F-424761EC9BC6', 'Survey', 'Freeform', 46, 'What are your strengths, gifts, and talents?');

-- Associated adventure scenes
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('768B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 1);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('778B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 2);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('788B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 3);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('798B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 4);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('7A8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 5);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('7B8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 6);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('7C8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 7);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('7D8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 8);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('7E8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 9);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('7F8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 10);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('808B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 11);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('818B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 12);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('828B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 13);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('838B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 14);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('848B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 15);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('858B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 16);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('868B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 17);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('878B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 18);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('888B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 19);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('898B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 20);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('8A8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 21);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('8B8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 22);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('8C8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 23);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('8D8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 24);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('8E8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 25);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('8F8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 26);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('908B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 27);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('918B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 28);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('928B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 29);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('938B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 30);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('948B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 31);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('958B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 32);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('968B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 33);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('978B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 34);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('988B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 35);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('998B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 36);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9A8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 37);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9B8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 38);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9C8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 39);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9D8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 40);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9E8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 41);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9F8B4E09-ED1F-403B-A417-907EAFFD4446', '73C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 42);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('320EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 1);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('330EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 2);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('340EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 3);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('350EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 4);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('360EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 5);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('370EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 6);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('380EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 7);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('390EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 8);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('3A0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 9);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('3B0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 10);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('3C0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 11);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('3D0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 12);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('3E0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 13);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('3F0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 14);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('400EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 15);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('410EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 16);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('420EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 17);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('430EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 18);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('440EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 19);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('450EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 20);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('460EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 21);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('470EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 22);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('480EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 23);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('490EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 24);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('4A0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 25);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('4B0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 26);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('4C0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 27);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('4D0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 28);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('4E0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 29);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('4F0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 30);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('500EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 31);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('510EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 32);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('520EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 33);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('530EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 34);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('540EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 35);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('550EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 36);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('560EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 37);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('570EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 38);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('580EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 39);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('590EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 40);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('5A0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 41);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('5B0EF7FB-F2E2-4081-A18F-3F647528FE14', '74C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 42);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('C3D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 1);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('C4D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 2);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('C5D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 3);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('C6D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 4);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('C7D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 5);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('C8D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 6);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('C9D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 7);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('CAD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 8);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('CBD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 9);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('CCD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 10);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('CDD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 11);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('CED199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 12);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('CFD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 13);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D0D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 14);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D1D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 15);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D2D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 16);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D3D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 17);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D4D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 18);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D5D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 19);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D6D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 20);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D7D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 21);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D8D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 22);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('D9D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 23);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('DAD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 24);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('DBD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 25);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('DCD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 26);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('DDD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 27);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('DED199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 28);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('DFD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 29);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E0D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 30);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E1D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 31);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E2D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 32);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E3D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 33);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E4D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 34);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E5D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 35);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E6D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 36);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E7D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 37);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E8D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 38);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('E9D199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 39);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('EAD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 40);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('EBD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 41);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('ECD199C1-C83A-4054-8281-83314DDCDD6D', '75C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 42);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('96D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 1);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('97D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 2);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('98D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 3);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('99D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 4);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9AD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 5);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9BD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 6);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9CD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 7);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9DD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 8);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9ED7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 9);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('9FD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 10);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A0D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 11);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A1D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 12);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A2D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 13);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A3D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 14);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A4D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 15);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A5D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 16);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A6D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 17);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A7D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 18);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A8D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 19);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('A9D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 20);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('AAD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 21);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('ABD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 22);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('ACD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 23);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('ADD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 24);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('AED7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 25);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('AFD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 26);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B0D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 27);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B1D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 28);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B2D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 29);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B3D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 30);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B4D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 31);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B5D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 32);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B6D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 33);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B7D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 34);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B8D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 35);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('B9D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 36);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('BAD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 37);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('BBD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 38);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('BCD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 39);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('BDD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 40);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('BED7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 41);
INSERT INTO TweenScreenCore.Scene ([Id], [AdventureId], [Name], [Position]) VALUES ('BFD7BD49-3C1B-442B-ACEA-F97B4964E2DE', '76C15EDF-B24F-4551-93A1-8AD473DA1F35', '', 42);

-- Scene/Question link
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('778B4E09-ED1F-403B-A417-907EAFFD4446', 'DE3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('778B4E09-ED1F-403B-A417-907EAFFD4446', 'DF3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('778B4E09-ED1F-403B-A417-907EAFFD4446', 'E03A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('7A8B4E09-ED1F-403B-A417-907EAFFD4446', 'E13A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('7A8B4E09-ED1F-403B-A417-907EAFFD4446', 'E23A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('7A8B4E09-ED1F-403B-A417-907EAFFD4446', 'E33A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('7D8B4E09-ED1F-403B-A417-907EAFFD4446', 'E43A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('7D8B4E09-ED1F-403B-A417-907EAFFD4446', 'E53A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('7F8B4E09-ED1F-403B-A417-907EAFFD4446', 'E63A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('7F8B4E09-ED1F-403B-A417-907EAFFD4446', 'E73A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('818B4E09-ED1F-403B-A417-907EAFFD4446', 'E83A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('818B4E09-ED1F-403B-A417-907EAFFD4446', 'E93A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('838B4E09-ED1F-403B-A417-907EAFFD4446', 'EA3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('838B4E09-ED1F-403B-A417-907EAFFD4446', '07600418-5210-4186-898A-A4C3FE107FE4');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('838B4E09-ED1F-403B-A417-907EAFFD4446', 'EB3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('868B4E09-ED1F-403B-A417-907EAFFD4446', 'EC3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('868B4E09-ED1F-403B-A417-907EAFFD4446', 'ED3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('888B4E09-ED1F-403B-A417-907EAFFD4446', 'EE3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('888B4E09-ED1F-403B-A417-907EAFFD4446', 'EF3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('888B4E09-ED1F-403B-A417-907EAFFD4446', 'F03A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('8B8B4E09-ED1F-403B-A417-907EAFFD4446', 'F13A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('8C8B4E09-ED1F-403B-A417-907EAFFD4446', 'F23A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('8D8B4E09-ED1F-403B-A417-907EAFFD4446', 'F33A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('8E8B4E09-ED1F-403B-A417-907EAFFD4446', 'F43A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('8E8B4E09-ED1F-403B-A417-907EAFFD4446', 'F53A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('908B4E09-ED1F-403B-A417-907EAFFD4446', 'F63A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('908B4E09-ED1F-403B-A417-907EAFFD4446', 'F73A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('908B4E09-ED1F-403B-A417-907EAFFD4446', 'F83A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('938B4E09-ED1F-403B-A417-907EAFFD4446', 'F93A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('948B4E09-ED1F-403B-A417-907EAFFD4446', 'FA3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('958B4E09-ED1F-403B-A417-907EAFFD4446', 'FB3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('958B4E09-ED1F-403B-A417-907EAFFD4446', 'FC3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('978B4E09-ED1F-403B-A417-907EAFFD4446', 'FD3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('988B4E09-ED1F-403B-A417-907EAFFD4446', 'FE3A79ED-B03A-4BE9-A14F-424761EC9BC6');

INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('330EF7FB-F2E2-4081-A18F-3F647528FE14', 'DE3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('330EF7FB-F2E2-4081-A18F-3F647528FE14', 'DF3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('330EF7FB-F2E2-4081-A18F-3F647528FE14', 'E03A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('360EF7FB-F2E2-4081-A18F-3F647528FE14', 'E13A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('360EF7FB-F2E2-4081-A18F-3F647528FE14', 'E23A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('360EF7FB-F2E2-4081-A18F-3F647528FE14', 'E33A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('390EF7FB-F2E2-4081-A18F-3F647528FE14', 'E43A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('390EF7FB-F2E2-4081-A18F-3F647528FE14', 'E53A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('3B0EF7FB-F2E2-4081-A18F-3F647528FE14', 'E63A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('3B0EF7FB-F2E2-4081-A18F-3F647528FE14', 'E73A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('3D0EF7FB-F2E2-4081-A18F-3F647528FE14', 'E83A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('3D0EF7FB-F2E2-4081-A18F-3F647528FE14', 'E93A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('3F0EF7FB-F2E2-4081-A18F-3F647528FE14', 'EA3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('3F0EF7FB-F2E2-4081-A18F-3F647528FE14', '07600418-5210-4186-898A-A4C3FE107FE4');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('3F0EF7FB-F2E2-4081-A18F-3F647528FE14', 'EB3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('420EF7FB-F2E2-4081-A18F-3F647528FE14', 'EC3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('420EF7FB-F2E2-4081-A18F-3F647528FE14', 'ED3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('440EF7FB-F2E2-4081-A18F-3F647528FE14', 'EE3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('440EF7FB-F2E2-4081-A18F-3F647528FE14', 'EF3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('440EF7FB-F2E2-4081-A18F-3F647528FE14', 'F03A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('470EF7FB-F2E2-4081-A18F-3F647528FE14', 'F13A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('480EF7FB-F2E2-4081-A18F-3F647528FE14', 'F23A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('490EF7FB-F2E2-4081-A18F-3F647528FE14', 'F33A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('4A0EF7FB-F2E2-4081-A18F-3F647528FE14', 'F43A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('4A0EF7FB-F2E2-4081-A18F-3F647528FE14', 'F53A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('4C0EF7FB-F2E2-4081-A18F-3F647528FE14', 'F63A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('4C0EF7FB-F2E2-4081-A18F-3F647528FE14', 'F73A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('4C0EF7FB-F2E2-4081-A18F-3F647528FE14', 'F83A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('4F0EF7FB-F2E2-4081-A18F-3F647528FE14', 'F93A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('500EF7FB-F2E2-4081-A18F-3F647528FE14', 'FA3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('510EF7FB-F2E2-4081-A18F-3F647528FE14', 'FB3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('510EF7FB-F2E2-4081-A18F-3F647528FE14', 'FC3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('530EF7FB-F2E2-4081-A18F-3F647528FE14', 'FD3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('540EF7FB-F2E2-4081-A18F-3F647528FE14', 'FE3A79ED-B03A-4BE9-A14F-424761EC9BC6');

INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('C4D199C1-C83A-4054-8281-83314DDCDD6D', 'DE3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('C4D199C1-C83A-4054-8281-83314DDCDD6D', 'DF3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('C4D199C1-C83A-4054-8281-83314DDCDD6D', 'E03A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('C7D199C1-C83A-4054-8281-83314DDCDD6D', 'E13A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('C7D199C1-C83A-4054-8281-83314DDCDD6D', 'E23A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('C7D199C1-C83A-4054-8281-83314DDCDD6D', 'E33A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('CAD199C1-C83A-4054-8281-83314DDCDD6D', 'E43A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('CAD199C1-C83A-4054-8281-83314DDCDD6D', 'E53A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('CCD199C1-C83A-4054-8281-83314DDCDD6D', 'E63A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('CCD199C1-C83A-4054-8281-83314DDCDD6D', 'E73A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('CED199C1-C83A-4054-8281-83314DDCDD6D', 'E83A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('CED199C1-C83A-4054-8281-83314DDCDD6D', 'E93A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D0D199C1-C83A-4054-8281-83314DDCDD6D', 'EA3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D0D199C1-C83A-4054-8281-83314DDCDD6D', '07600418-5210-4186-898A-A4C3FE107FE4');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D0D199C1-C83A-4054-8281-83314DDCDD6D', 'EB3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D3D199C1-C83A-4054-8281-83314DDCDD6D', 'EC3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D3D199C1-C83A-4054-8281-83314DDCDD6D', 'ED3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D5D199C1-C83A-4054-8281-83314DDCDD6D', 'EE3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D5D199C1-C83A-4054-8281-83314DDCDD6D', 'EF3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D5D199C1-C83A-4054-8281-83314DDCDD6D', 'F03A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D8D199C1-C83A-4054-8281-83314DDCDD6D', 'F13A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('D9D199C1-C83A-4054-8281-83314DDCDD6D', 'F23A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('DAD199C1-C83A-4054-8281-83314DDCDD6D', 'F33A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('DBD199C1-C83A-4054-8281-83314DDCDD6D', 'F43A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('DBD199C1-C83A-4054-8281-83314DDCDD6D', 'F53A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('DDD199C1-C83A-4054-8281-83314DDCDD6D', 'F63A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('DDD199C1-C83A-4054-8281-83314DDCDD6D', 'F73A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('DDD199C1-C83A-4054-8281-83314DDCDD6D', 'F83A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('E0D199C1-C83A-4054-8281-83314DDCDD6D', 'F93A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('E1D199C1-C83A-4054-8281-83314DDCDD6D', 'FA3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('E2D199C1-C83A-4054-8281-83314DDCDD6D', 'FB3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('E2D199C1-C83A-4054-8281-83314DDCDD6D', 'FC3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('E4D199C1-C83A-4054-8281-83314DDCDD6D', 'FD3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('E5D199C1-C83A-4054-8281-83314DDCDD6D', 'FE3A79ED-B03A-4BE9-A14F-424761EC9BC6');

INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('97D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'DE3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('97D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'DF3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('97D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E03A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('9AD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E13A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('9AD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E23A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('9AD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E33A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('9DD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E43A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('9DD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E53A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('9FD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E63A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('9FD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E73A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A1D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E83A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A1D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'E93A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A3D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'EA3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A3D7BD49-3C1B-442B-ACEA-F97B4964E2DE', '07600418-5210-4186-898A-A4C3FE107FE4');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A3D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'EB3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A6D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'EC3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A6D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'ED3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A8D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'EE3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A8D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'EF3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('A8D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F03A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('ABD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F13A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('ACD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F23A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('ADD7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F33A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('AED7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F43A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('AED7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F53A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('B0D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F63A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('B0D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F73A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('B0D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F83A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('B3D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'F93A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('B4D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'FA3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('B5D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'FB3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('B5D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'FC3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('B7D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'FD3A79ED-B03A-4BE9-A14F-424761EC9BC6');
INSERT INTO TweenScreenCore.SceneQuestion ([SceneId], [QuestionId]) VALUES ('B8D7BD49-3C1B-442B-ACEA-F97B4964E2DE', 'FE3A79ED-B03A-4BE9-A14F-424761EC9BC6');

-- Question contingents
INSERT INTO TweenScreenCore.QuestionContingent ([Id], [ParentId], [QuestionId], [Rule], [Status], [CreatedAt], [UpdatedAt], [ArchivedAt], [DeletedAt], [Position])
VALUES  (N'CA9017A7-88D1-4348-87B3-4CAFE99B2060', N'DE3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'DF3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'4, 5', null, N'2022-09-30 12:27:35.7800000', N'2022-11-02 22:48:22.2533333', null, null, 1),
        (N'CC9017A7-88D1-4348-87B3-4CAFE99B2060', N'E13A79ED-B03A-4BE9-A14F-424761EC9BC6', N'E23A79ED-B03A-4BE9-A14F-424761EC9BC6', N'3, 4, 5', null, N'2022-09-30 12:27:35.8000000', N'2022-10-19 14:29:15.7533333', null, null, 1),
        (N'CE9017A7-88D1-4348-87B3-4CAFE99B2060', N'E43A79ED-B03A-4BE9-A14F-424761EC9BC6', N'E53A79ED-B03A-4BE9-A14F-424761EC9BC6', N'1, 2', null, N'2022-09-30 12:27:35.8033333', null, null, null, 1),
        (N'CF9017A7-88D1-4348-87B3-4CAFE99B2060', N'E63A79ED-B03A-4BE9-A14F-424761EC9BC6', N'E73A79ED-B03A-4BE9-A14F-424761EC9BC6', N'1, 2', null, N'2022-09-30 12:27:35.8100000', null, null, null, 1),
        (N'D09017A7-88D1-4348-87B3-4CAFE99B2060', N'E83A79ED-B03A-4BE9-A14F-424761EC9BC6', N'E93A79ED-B03A-4BE9-A14F-424761EC9BC6', N'TRUE', null, N'2022-09-30 12:27:35.8133333', null, null, null, 1),
        (N'D19017A7-88D1-4348-87B3-4CAFE99B2060', N'EA3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'07600418-5210-4186-898A-A4C3FE107FE4', N'3, 4, 5', null, N'2022-09-30 12:27:35.8166667', N'2022-10-19 14:29:15.7533333', null, null, 1),
        (N'D39017A7-88D1-4348-87B3-4CAFE99B2060', N'EC3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'ED3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'4, 5', null, N'2022-09-30 12:27:35.8300000', null, null, null, 1),
        (N'D49017A7-88D1-4348-87B3-4CAFE99B2060', N'EE3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'EF3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'TRUE', null, N'2022-09-30 12:27:35.8300000', null, null, null, 1),
        (N'D69017A7-88D1-4348-87B3-4CAFE99B2060', N'F43A79ED-B03A-4BE9-A14F-424761EC9BC6', N'F53A79ED-B03A-4BE9-A14F-424761EC9BC6', N'TRUE', null, N'2022-09-30 12:27:35.8366667', null, null, null, 1),
        (N'D79017A7-88D1-4348-87B3-4CAFE99B2060', N'F63A79ED-B03A-4BE9-A14F-424761EC9BC6', N'F73A79ED-B03A-4BE9-A14F-424761EC9BC6', N'3, 4, 5', null, N'2022-09-30 12:27:35.8400000', N'2022-10-19 14:29:15.7533333', null, null, 1),
        (N'D99017A7-88D1-4348-87B3-4CAFE99B2060', N'FB3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'FC3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'TRUE', null, N'2022-09-30 12:27:35.8466667', null, null, null, 1),
        (N'D89017A7-88D1-4348-87B3-4CAFE99B2060', N'F63A79ED-B03A-4BE9-A14F-424761EC9BC6', N'F83A79ED-B03A-4BE9-A14F-424761EC9BC6', N'4, 5', null, N'2022-09-30 12:27:35.8400000', null, null, null, 2),
        (N'D59017A7-88D1-4348-87B3-4CAFE99B2060', N'EE3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'F03A79ED-B03A-4BE9-A14F-424761EC9BC6', N'TRUE', null, N'2022-09-30 12:27:35.8333333', null, null, null, 2),
        (N'D29017A7-88D1-4348-87B3-4CAFE99B2060', N'EA3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'EB3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'4, 5', null, N'2022-09-30 12:27:35.8233333', null, null, null, 2),
        (N'CD9017A7-88D1-4348-87B3-4CAFE99B2060', N'E13A79ED-B03A-4BE9-A14F-424761EC9BC6', N'E33A79ED-B03A-4BE9-A14F-424761EC9BC6', N'4, 5', null, N'2022-09-30 12:27:35.8033333', null, null, null, 2),
        (N'CB9017A7-88D1-4348-87B3-4CAFE99B2060', N'DE3A79ED-B03A-4BE9-A14F-424761EC9BC6', N'E03A79ED-B03A-4BE9-A14F-424761EC9BC6', N'1, 2, 3', null, N'2022-09-30 12:27:35.7966667', N'2022-11-02 22:48:22.2533333', null, null, 2);
