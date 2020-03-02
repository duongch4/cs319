Insert into Disciplines (Id, [Name]) values (1, N'Weapons');
Insert into Disciplines (Id, [Name]) values (2, N'Martial Arts');
Insert into Disciplines (Id, [Name]) values (3, N'Language');
Insert into Disciplines (Id, [Name]) values (4, N'Cryptography');
Insert into Disciplines (Id, [Name]) values (5, N'Intel');

Insert into Skills (DisciplineId, Id, [Name]) values (1, 10, N'Glock');
Insert into Skills (DisciplineId, Id, [Name]) values (1, 11, N'Sniper Rifle');
Insert into Skills (DisciplineId, Id, [Name]) values (2, 20, N'Krav Maga');
Insert into Skills (DisciplineId, Id, [Name]) values (2, 21, N'Kali');
Insert into Skills (DisciplineId, Id, [Name]) values (3, 30, N'Russian');
Insert into Skills (DisciplineId, Id, [Name]) values (3, 31, N'Mandarin');
Insert into Skills (DisciplineId, Id, [Name]) values (4, 40, N'Ciphers');
Insert into Skills (DisciplineId, Id, [Name]) values (5, 50, N'False Identity Creation');
Insert into Skills (DisciplineId, Id, [Name]) values (5, 51, N'Deception');

Set IDENTITY_INSERT [dbo].[Locations] ON
INSERT [dbo].[Locations] ([Id], [Province], [City]) VALUES (5, N'Alberta', N'Edmonton')
INSERT [dbo].[Locations] ([Id], [Province], [City]) VALUES (8, N'British Columbia', N'Vancouver')
INSERT [dbo].[Locations] ([Id], [Province], [City]) VALUES (19, N'Saskatchewan', N'Saskatoon')
INSERT [dbo].[Locations] ([Id], [Province], [City]) VALUES (20, N'Alberta', N'Calgary')
INSERT [dbo].[Locations] ([Id], [Province], [City]) VALUES (21, N'British Columbia', N'Kelowna')
Set IDENTITY_INSERT [dbo].[Locations] OFF

Set IDENTITY_INSERT [dbo].[Users] ON
Insert [dbo].[Users] ([Id], [FirstName], [LastName], [UserName], [LocationId], [IsAdmin], [IsManager]) values (1, 'Jason', 'Bourne', 'jsonbourne', 8, 0, 0);
Insert [dbo].[Users] ([Id], [FirstName], [LastName], [UserName], [LocationId], [IsAdmin], [IsManager]) values (2, 'Charles', 'Bartowski', 'chuck', 21, 1, 1);
Insert [dbo].[Users] ([Id], [FirstName], [LastName], [UserName], [LocationId], [IsAdmin], [IsManager]) values (3, 'Natasha', 'Romanov', 'blackwidow', 20, 0, 0);
Insert [dbo].[Users] ([Id], [FirstName], [LastName], [UserName], [LocationId], [IsAdmin], [IsManager]) values (4, 'Clint', 'Barton', 'hawkeye', 19, 0, 1);
Insert [dbo].[Users] ([Id], [FirstName], [LastName], [UserName], [LocationId], [IsAdmin], [IsManager]) values (5, 'Sameen', 'Shaw', 'shaw', 8, 1, 1);
Set IDENTITY_INSERT [dbo].[Users] OFF

Insert into OutOfOffice (ResourceId, FromDate, ToDate, Reason) values (1, '2020-02-29', '2020-03-07', 'Dead Grandma');
Insert into OutOfOffice (ResourceId, FromDate, ToDate, Reason) values (3, '2020-10-31', '2020-11-11', 'Maternal Leave');
Insert into OutOfOffice (ResourceId, FromDate, ToDate, Reason) values (4, '2020-11-01', '2020-11-17', 'Vacation in Mars');
Insert into OutOfOffice (ResourceId, FromDate, ToDate, Reason) values (3, '2020-04-07', '2020-04-19', 'Maternal Leave');
Insert into OutOfOffice (ResourceId, FromDate, ToDate, Reason) values (2, '2020-12-25', '2021-01-07', 'Wedding');

SET IDENTITY_INSERT [dbo].[Projects] ON
INSERT [dbo].[Projects] ([Id], [Number], [Title], [LocationId], [ManagerId], [ProjectStartDate], [ProjectEndDate]) VALUES (1, '2009-VD9D-15', 'Infiltrate Google', 5, 2, '2020-02-27', '2020-04-10');
INSERT [dbo].[Projects] ([Id], [Number], [Title], [LocationId], [ManagerId], [ProjectStartDate], [ProjectEndDate]) VALUES (2, '2005-KJS4-46', 'Budapest', 19, 4, '2020-04-19', '2020-07-01');
INSERT [dbo].[Projects] ([Id], [Number], [Title], [LocationId], [ManagerId], [ProjectStartDate], [ProjectEndDate]) VALUES (3, '2013-P24C-76', 'Jeremy Bearimy', 8, 2, '2020-04-19', '2121-04-19');
INSERT [dbo].[Projects] ([Id], [Number], [Title], [LocationId], [ManagerId], [ProjectStartDate], [ProjectEndDate]) VALUES (4, '2014-HUGC-34', 'Secure Time Loop', 8, 5, '2020-06-13', '2020-11-25');
INSERT [dbo].[Projects] ([Id], [Number], [Title], [LocationId], [ManagerId], [ProjectStartDate], [ProjectEndDate]) VALUES (5, '2011-OYRR-48', 'Acquire Super Soldier Formula', 20, 4, '2020-12-01', '2021-05-05');
INSERT [dbo].[Projects] ([Id], [Number], [Title], [LocationId], [ManagerId], [ProjectStartDate], [ProjectEndDate]) VALUES (6, '2008-2TL3-77', 'Infiltrate and Dismantle Samaritan Operation', 19, 5, '2020-10-31', '2021-02-12');
Set IDENTITY_INSERT [dbo].[Projects] OFF

Insert ResourceDiscipline ([ResourceId], [DisciplineId], [YearsOfExperience]) values (1, 2, '10+');
Insert ResourceDiscipline ([ResourceId], [DisciplineId], [YearsOfExperience]) values (5, 1, '1-3');
Insert ResourceDiscipline ([ResourceId], [DisciplineId], [YearsOfExperience]) values (2, 3, '3-5');
Insert ResourceDiscipline ([ResourceId], [DisciplineId], [YearsOfExperience]) values (3, 3, '10+');
Insert ResourceDiscipline ([ResourceId], [DisciplineId], [YearsOfExperience]) values (4, 1, '5-10');
Insert ResourceDiscipline ([ResourceId], [DisciplineId], [YearsOfExperience]) values (5, 5, '5-10');
Insert ResourceDiscipline ([ResourceId], [DisciplineId], [YearsOfExperience]) values (1, 3, '1-3');
Insert ResourceDiscipline ([ResourceId], [DisciplineId], [YearsOfExperience]) values (2, 2, '1-3');

Insert ResourceSkill ([ResourceId], [ResourceDisciplineId], [SkillDisciplineId], [SkillId]) values (1, 2, 2, 21);
Insert ResourceSkill ([ResourceId], [ResourceDisciplineId], [SkillDisciplineId], [SkillId]) values (5, 1, 1, 10);
Insert ResourceSkill ([ResourceId], [ResourceDisciplineId], [SkillDisciplineId], [SkillId]) values (2, 3, 3, 31);
Insert ResourceSkill ([ResourceId], [ResourceDisciplineId], [SkillDisciplineId], [SkillId]) values (3, 3, 3, 30);
Insert ResourceSkill ([ResourceId], [ResourceDisciplineId], [SkillDisciplineId], [SkillId]) values (5, 5, 5, 50);
Insert ResourceSkill ([ResourceId], [ResourceDisciplineId], [SkillDisciplineId], [SkillId]) values (1, 3, 3, 30);
Insert ResourceSkill ([ResourceId], [ResourceDisciplineId], [SkillDisciplineId], [SkillId]) values (2, 2, 2, 20);

Set IDENTITY_INSERT [dbo].[Positions] ON
Insert [dbo].[Positions] ([Id], [DisciplineId], [ProjectId], [ProjectedMonthlyHours], [ResourceId], [PositionName], [YearsOfExperience], [IsConfirmed]) values (1, 4, 1, 90, null, 'Binger', '10+', 0 );
Insert [dbo].[Positions] ([Id], [DisciplineId], [ProjectId], [ProjectedMonthlyHours], [ResourceId], [PositionName], [YearsOfExperience], [IsConfirmed]) values (2, 1, 2, 170, 4, 'Mission Control', '3-5', 1);
Insert [dbo].[Positions] ([Id], [DisciplineId], [ProjectId], [ProjectedMonthlyHours], [ResourceId], [PositionName], [YearsOfExperience], [IsConfirmed]) values (3, 5, 2, 170, 3, 'Intel', '1-2', 1);
Insert [dbo].[Positions] ([Id], [DisciplineId], [ProjectId], [ProjectedMonthlyHours], [ResourceId], [PositionName], [YearsOfExperience], [IsConfirmed]) values (4, 2, 5, 150, 1, 'Preventing evil Cap','3-5', 1);
Insert [dbo].[Positions] ([Id], [DisciplineId], [ProjectId], [ProjectedMonthlyHours], [ResourceId], [PositionName], [YearsOfExperience], [IsConfirmed]) values (5, 1, 4, 120, 1, 'Time Cop', '10+', 1);
Insert [dbo].[Positions] ([Id], [DisciplineId], [ProjectId], [ProjectedMonthlyHours], [ResourceId], [PositionName], [YearsOfExperience], [IsConfirmed]) values (6, 3, 3, 165, 1, 'In the dot', '10+', 0);
Insert [dbo].[Positions] ([Id], [DisciplineId], [ProjectId], [ProjectedMonthlyHours], [ResourceId], [PositionName], [YearsOfExperience], [IsConfirmed]) values (7, 1, 6, 120, 5, 'Protect the Machine', '3-5', 0);
Set IDENTITY_INSERT [dbo].[Positions] OFF

Insert [dbo].[PositionSkills] ([PositionId], [SkillId], [SkillDisciplineId]) values (1, 40, 4);
Insert [dbo].[PositionSkills] ([PositionId], [SkillId], [SkillDisciplineId]) values (2, 10, 1);
Insert [dbo].[PositionSkills] ([PositionId], [SkillId], [SkillDisciplineId]) values (2, 51, 5);
Insert [dbo].[PositionSkills] ([PositionId], [SkillId], [SkillDisciplineId]) values (4, 20, 2);
Insert [dbo].[PositionSkills] ([PositionId], [SkillId], [SkillDisciplineId]) values (4, 21, 2);
Insert [dbo].[PositionSkills] ([PositionId], [SkillId], [SkillDisciplineId]) values (5, 11, 1);
Insert [dbo].[PositionSkills] ([PositionId], [SkillId], [SkillDisciplineId]) values (6, 31, 3);
Insert [dbo].[PositionSkills] ([PositionId], [SkillId], [SkillDisciplineId]) values (7, 10, 1);