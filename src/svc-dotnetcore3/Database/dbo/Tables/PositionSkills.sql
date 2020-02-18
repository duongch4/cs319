CREATE TABLE [dbo].PositionSkills
(
	[PositionId] INT NOT NULL,
	[SkillId] INT NOT NULL,
	[SkillDisciplineId] INT NOT NULL,
	CONSTRAINT [FK_PositionSkills_Skills] FOREIGN KEY (SkillDisciplineId, SkillId) REFERENCES Skills([DisciplineId], [Id]),
	CONSTRAINT [FK_PositionSkills_Positions] FOREIGN KEY (PositionId) REFERENCES Positions([Id]) ON DELETE CASCADE,
	CONSTRAINT [PK_PositionSkills] PRIMARY KEY ([SkillId], [SkillDisciplineId], [PositionId]),
)