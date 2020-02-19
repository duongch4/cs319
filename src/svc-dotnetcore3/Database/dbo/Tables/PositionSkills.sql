CREATE TABLE [dbo].PositionSkills
(
		[PositionId] INT NOT NULL,
		[SkillId] INT NOT NULL,
		[SkillDisciplineId] INT NOT NULL
	CONSTRAINT [FK_PositionSkills_Skills] FOREIGN KEY (SkillId, SkillDisciplineId) REFERENCES Skills([DisciplineId], [Id]),
	CONSTRAINT [FK_PositionSkills_Positions] FOREIGN KEY (PositionId) REFERENCES Positions([PositionId]) ON UPDATE CASCADE,
	CONSTRAINT [PK_PositionSkills] PRIMARY KEY ([SkillId], [SkillDisciplineId], [PositionId]),
)