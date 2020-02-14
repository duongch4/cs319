CREATE TABLE [dbo].Positions
(
	[PositionId] INT NOT NULL PRIMARY KEY,
	[DisciplinesId] INT NOT NULL, 
	[ProjectId] INT NOT NULL,
	[ProjectedHours] INT NOT NULL,
	[ResourceId] INT NULL,
	[PositionName] nvarchar NOT NULL,
	CONSTRAINT [FK_Positions_Projects] FOREIGN KEY (ProjectId) REFERENCES Projects([Id]),
	CONSTRAINT [FK_Poisitions_Disciplines] FOREIGN KEY (DisciplinesId) REFERENCES Disciplines([Id]),
	CONSTRAINT [FK_Poisitions_Users] FOREIGN KEY (ResourceId) REFERENCES [Users]([Id])
	)