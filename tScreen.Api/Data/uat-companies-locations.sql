INSERT INTO [dbo].[Company] (Id, Type, Name, Slug, Description, CreatedAt)
VALUES ('0a09902c-97ab-4d8f-8b82-f173eab03528', 'Practice', 'Total Health Academy', 'total-health-academy',
        'FT UTA Test company', '2022-07-20'),
       ('a3ce84f8-60c3-4bd4-8066-6ca67e95c5aa', 'Practice', 'Mentally Healthy Pediatrics ',
        'mentally-healthy-pediatrics ', 'FT UTA Test company', '2022-07-20'),
       ('e2d6b08f-a64f-4187-b55c-eb9fdfba0e58', 'Practice', 'Mental Health Matters Clinic',
        'mental-health-matters-clinic', 'FT UTA Test company', '2022-07-20'),
       ('650298cc-b5fe-4d28-81eb-f8150ecd9bc0', 'Mental heath', 'Oct Test MFt',
        'oct-test-mft', 'Family Therapy (FT UTA Test company)', '2022-10-26'),
        ('3d674b04-936c-4dd5-8662-09a71365e7e6', 'School', 'Big School UAT', 
         'big-school-uat', 'A school representing a large dataset of users/students', '2022-10-28')
GO
INSERT INTO [dbo].[Location] (Id, CompanyId, Type, Name, Description, StreetLineOne, City, State, PostalCode, Country, CreatedAt)
VALUES ('0d72cb39-b185-4e99-8688-cfdac4cffbdc', '0a09902c-97ab-4d8f-8b82-f173eab03528', 'Default', 'Default',
        'FT UAT Default location', '67 Deep', 'Darien', 'CT', '06820', 'USA', '2022-07-20'),
       ('fcbf752d-2c54-4799-b7ac-e871e660181e', 'a3ce84f8-60c3-4bd4-8066-6ca67e95c5aa', 'Default', 'Darien Office',
        'FT UAT Default location', '67 Wood Rd', 'Darien', 'CT', '06820', 'USA', '2022-07-20'),
       ('682cb710-2aff-42a6-b52d-1df282592099', 'e2d6b08f-a64f-4187-b55c-eb9fdfba0e58', 'Default', 'Default',
        'FT UAT Default location', '67 Deepwood Rd', 'Darien', 'CT', '06820', 'USA', '2022-07-20'),
       ('bb12157a-9da0-44bf-be4b-85bc4dc5d746', '650298cc-b5fe-4d28-81eb-f8150ecd9bc0', 'Default', 'Manhattan MFT',
        'Pilot (FT UAT Default location)', '1051 Boston Post Rd', 'Darien', 'CT', '06820', 'USA', '2022-10-26'),
       ('980e7669-c999-44cf-91a0-379b8cdf0a8a', '3d674b04-936c-4dd5-8662-09a71365e7e6', 'Default', 'Main Location',
        'FT UTA Default Location', '5430 Hidalgo St', 'Houston', 'TX', '77056', 'USA', '2022-10-28')
GO
INSERT INTO [dbo].[CustomField] (id, LocationId, Type, Position, Name)
VALUES ('0e232baf-9653-4cb0-b7a3-3875d826d073', '682cb710-2aff-42a6-b52d-1df282592099', 'Text', 0, 'Doctor'),
       ('86f6f03a-192a-47db-99e2-cab47f1f8e1f', '682cb710-2aff-42a6-b52d-1df282592099', 'Text', 1, 'Age'),
       ('2612aa6e-cdec-41c9-91aa-0580a2b7fa22', '0d72cb39-b185-4e99-8688-cfdac4cffbdc', 'Text', 0, 'Grade'),
       ('53640097-c587-414d-9f12-99eb4ad3bc15', '0d72cb39-b185-4e99-8688-cfdac4cffbdc', 'Text', 1, 'Homeroom'),
       ('9ea6ccad-17ae-45c1-8b66-750dec706f5e', 'fcbf752d-2c54-4799-b7ac-e871e660181e', 'Text', 0, 'Doctor'),
       ('c1a1c6a5-0889-4f56-bfb4-bb1cd8576041', '980E7669-C999-44CF-91A0-379B8CDF0A8A', 'Text', 0, 'Color'),
       ('fc395601-b215-4eb9-b668-ac02c9cd05f1', '980E7669-C999-44CF-91A0-379B8CDF0A8A', 'Text', 1, 'Homeroom');