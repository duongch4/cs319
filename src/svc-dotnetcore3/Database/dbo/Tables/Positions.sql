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