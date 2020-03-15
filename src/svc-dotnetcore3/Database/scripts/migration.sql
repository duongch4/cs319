DROP TABLE OutOfOffice;
DROP TABLE PositionSkills;
DROP TABLE ResourceSkill;
DROP TABLE Positions;
DROP TABLE ResourceDiscipline;
DROP TABLE Skills;
DROP TABLE Projects;
DROP TABLE Users;
DROP TABLE Locations;
DROP TABLE Disciplines;

CREATE TABLE [dbo].[Disciplines]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL UNIQUE
)
CREATE TABLE [dbo].[Provinces]
(
	[Name] [nvarchar] (100) NOT NULL PRIMARY KEY
)
CREATE TABLE [dbo].[Locations]
(
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Province] [nvarchar](100),
	[City] [nvarchar](100) NOT NULL,
	CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_Locations_Provinces] FOREIGN KEY ([Province]) REFERENCES [Provinces]([Name]) ON DELETE NO ACTION,
	CONSTRAINT [UK_Locations_CityProv] UNIQUE ([City] , [Province])
)
CREATE TABLE [dbo].[Users]
(
	[Id] [int] NOT NULL IDENTITY(1,1),
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[LocationId] [int] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[IsManager] [bit] NOT NULL,
	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [UK_Users_Username] UNIQUE ([Username]),
	CONSTRAINT [FK_Users_Locations] FOREIGN KEY ([LocationId]) REFERENCES [Locations]([Id]) ON DELETE NO ACTION
)
CREATE TABLE [dbo].[Projects]
(
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Number] [nvarchar](50) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[LocationId] [int] NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	[UpdatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	[ManagerId] [int] NOT NULL,
	[ProjectStartDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	[ProjectEndDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_Projects_Locations] FOREIGN KEY ([LocationId]) REFERENCES [Locations]([Id]),
	CONSTRAINT [FK_Projects_User] FOREIGN KEY ([ManagerId]) REFERENCES [Users]([Id]),
	CONSTRAINT [UK_Projects_Number] UNIQUE ([Number])
)
​
CREATE TABLE [dbo].[Skills]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[DisciplineId] INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	CONSTRAINT [FK_Skills_Disciplines] FOREIGN KEY ([DisciplineId]) REFERENCES [Disciplines]([Id]) ON DELETE NO ACTION,
	CONSTRAINT [PK_Skills] PRIMARY KEY ([DisciplineId], [Id]),
	CONSTRAINT [UK_Skills_Disciplines] UNIQUE ([DisciplineId], [Name])
)
​
  CREATE TABLE [dbo].[ResourceDiscipline]
(
	[ResourceId] [int] NOT NULL,
	[DisciplineId] [int] NOT NULL,
	[YearsOfExperience] NVARCHAR(10),
	CONSTRAINT [FK_ResourceDiscipline_Disciplines] FOREIGN KEY ([DisciplineId]) REFERENCES [Disciplines]([Id]) ON DELETE NO ACTION,
	CONSTRAINT [FK_ResourceDiscipline_Users] FOREIGN KEY ([ResourceId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
	CONSTRAINT [PK_ResourceDiscipline] PRIMARY KEY ([ResourceId], [DisciplineId])
)
​
CREATE TABLE [dbo].Positions
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[DisciplineId] INT NOT NULL,
	[ProjectId] INT NOT NULL,
	[ProjectedMonthlyHours] INT NOT NULL,
	[ResourceId] INT NULL,
	[PositionName] nvarchar(100) NULL,
	[YearsOfExperience] nvarchar(10),
	[IsConfirmed] [bit] NOT NULL,
	CONSTRAINT [FK_Positions_Projects] FOREIGN KEY (ProjectId) REFERENCES Projects([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_Positions_Disciplines] FOREIGN KEY (DisciplineId) REFERENCES Disciplines([Id]),
	CONSTRAINT [FK_Positions_Users] FOREIGN KEY (ResourceId) REFERENCES [Users]([Id])
)
​
CREATE TABLE [dbo].[ResourceSkill]
(
	[ResourceId] [int] Not Null,
	[ResourceDisciplineId] [int] Not Null,
	[SkillDisciplineId] [int] Not Null,
	[SkillId] [int] Not Null,
	CONSTRAINT [FK_ResourceSkill_ResourceDiscipline] FOREIGN KEY ([ResourceId], [ResourceDisciplineId]) 
		REFERENCES [ResourceDiscipline] ([ResourceId], [DisciplineId]) ON DELETE NO ACTION,
	CONSTRAINT [FK_ResourceSkill_Skills] FOREIGN KEY ([SkillDisciplineId], [SkillId]) 
		REFERENCES [Skills] ([DisciplineId], [Id]),
	CONSTRAINT [PK_ResourseSkill] PRIMARY KEY ([ResourceId], [ResourceDisciplineId], [SkillDisciplineId], [SkillId]),
	CONSTRAINT [CK_DisciplinesMatch] CHECK ([ResourceDisciplineId] = [SkillDisciplineId])
)
CREATE TABLE [dbo].PositionSkills
(
	[PositionId] INT NOT NULL,
	[SkillId] INT NOT NULL,
	[SkillDisciplineId] INT NOT NULL,
	CONSTRAINT [FK_PositionSkills_Skills] FOREIGN KEY (SkillDisciplineId, SkillId) REFERENCES Skills([DisciplineId], [Id]),
	CONSTRAINT [FK_PositionSkills_Positions] FOREIGN KEY (PositionId) REFERENCES Positions([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT [PK_PositionSkills] PRIMARY KEY ([SkillId], [SkillDisciplineId], [PositionId]),
)
CREATE TABLE [dbo].[OutOfOffice]
(
	[ResourceId][int] NOT NULL,
	[FromDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	[ToDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	[Reason] nvarchar(100)
		CONSTRAINT [FK_OutOfOffice_ResourceId] FOREIGN KEY ([ResourceId]) REFERENCES [Users]([Id])
		CONSTRAINT [PK_OutOfOffice] PRIMARY KEY ([ResourceId], [FromDate], [ToDate])
)



