CREATE TABLE [dbo].[ResourceDiscipline]
(
	[ResourceId] [int] NOT NULL,
	[DisciplineId] [int] NOT NULL,
	[YearsOfExperience] [int],
	CONSTRAINT [FK_ResourceDiscipline_Disciplines] FOREIGN KEY ([DisciplineId]) REFERENCES [Disciplines]([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_ResourceDiscipline_Users] FOREIGN KEY ([ResourceId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [PK_ResourceDiscipline] PRIMARY KEY ([ResourceId], [DisciplineId])
)
