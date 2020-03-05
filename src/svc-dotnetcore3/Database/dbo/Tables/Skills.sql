CREATE TABLE [dbo].[Skills]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[DisciplineId] INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	CONSTRAINT [FK_Skills_Disciplines] FOREIGN KEY ([DisciplineId]) REFERENCES [Disciplines]([Id]),
	CONSTRAINT [PK_Skills] PRIMARY KEY ([DisciplineId], [Id])
)