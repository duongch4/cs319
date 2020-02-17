CREATE TABLE [dbo].PositionSkills
(
    [PositionId] INT NOT NULL,
    [SkillId] INT NOT NULL,
    [SkillDisciplineId] INT NOT NULL,
    CONSTRAINT [FK_PositionSkills_Skills] FOREIGN KEY (SkillId, SkillDisciplineId) REFERENCES Skills([Id], [DisciplineId]),
    CONSTRAINT [FK_PositionSkills_Positions] FOREIGN KEY (PositionId) REFERENCES Positions([Id]) ON UPDATE CASCADE,
    CONSTRAINT [PK_PositionSkills] PRIMARY KEY ([SkillId], [SkillDisciplineId], [PositionId]),
)
