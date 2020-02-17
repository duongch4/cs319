CREATE TABLE [dbo].[ResourceDiscipline]
(
	[ResourceId] [int] NOT NULL,
	[DisciplineName] nvarchar(100) NOT NULL,
	[YearsOfExperience] nvarchar(100),
	CONSTRAINT [FK_ResourceDiscipline_Disciplines] FOREIGN KEY ([DisciplineName]) REFERENCES [Disciplines]([Name]) ON DELETE CASCADE,
	CONSTRAINT [FK_ResourceDiscipline_Users] FOREIGN KEY ([ResourceId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [PK_ResourceDiscipline] PRIMARY KEY ([ResourceId], [DisciplineName])
)