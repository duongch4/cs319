  CREATE TABLE [dbo].[ResourceDiscipline]
(
	[ResourceId] [int] NOT NULL,
	[DisciplineId] [int] NOT NULL,
	[YearsOfExperience] NVARCHAR(10),
	CONSTRAINT [FK_ResourceDiscipline_Disciplines] FOREIGN KEY ([DisciplineId]) REFERENCES [Disciplines]([Id]) ON DELETE NO ACTION,
	CONSTRAINT [FK_ResourceDiscipline_Users] FOREIGN KEY ([ResourceId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
	CONSTRAINT [PK_ResourceDiscipline] PRIMARY KEY ([ResourceId], [DisciplineId])
)