CREATE TABLE [dbo].[ResourceSkill]
(
	[ResourceId] [int] Not Null,
	[ResourceDisciplineName] [int] Not Null,
	[SkillDisciplineId] [int] Not Null,
	[SkillId] [int] Not Null,
	CONSTRAINT [FK_ResourceSkill_ResourceDiscipline] FOREIGN KEY ([ResourceId], [ResourceDisciplineName]) 
		REFERENCES [ResourceDiscipline] ([ResourceId], [DisciplineName]) ON DELETE CASCADE,
	CONSTRAINT [FK_ResourceSkill_Skills] FOREIGN KEY ([SkillDisciplineId], [SkillId]) 
		REFERENCES [Skills] ([DisciplineId], [Id]),
	CONSTRAINT [PK_ResourseSkill] PRIMARY KEY ([ResourceId], [ResourceDisciplineName], [SkillDisciplineId], [SkillId]),
)