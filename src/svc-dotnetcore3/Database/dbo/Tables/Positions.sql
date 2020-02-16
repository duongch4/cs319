CREATE TABLE [dbo].Positions
(
		[Id] INT NOT NULL PRIMARY KEY,
		[DisciplineId] INT NOT NULL, 
		[ProjectId] INT NOT NULL,
		[ProjectedMonthlyHours] INT NOT NULL,
		[ResourceId] INT NULL,
		[PositionName] nvarchar(100) NOT NULL,
		[IsConfirmed] [bit] NOT NULL,
	CONSTRAINT [FK_Positions_Projects] FOREIGN KEY (ProjectId) REFERENCES Projects([Id]),
	CONSTRAINT [FK_Poisitions_Disciplines] FOREIGN KEY (DisciplinesId) REFERENCES Disciplines([Id]),
	CONSTRAINT [FK_Poisitions_Users] FOREIGN KEY (ResourceId) REFERENCES [Users]([Id])
	)