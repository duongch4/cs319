CREATE TABLE [dbo].[ResourceSkill]
(
	[ResourceId] [int] Not Null,
	[ResourceDisciplineId] [int] Not Null,
	[SkillDisciplineId] [int] Not Null,
	[SkillId] [int] Not Null,
	CONSTRAINT [FK_ResourceSkill_ResourceDiscipline] FOREIGN KEY ([ResourceId], [DisciplineId]) 
		REFERENCES [ResourceDiscipline] ([ResourceId], [ResourceDisciplineId]) ON DELETE CASCADE,
	CONSTRAINT [FK_ResourceSkill_Skills] FOREIGN KEY ([SkillDisciplineId], [SkillId]) 
		REFERENCES [Skills] ([DisciplineId], [Id]),
	CONSTRAINT [PK_ResourseSkill] PRIMARY KEY ([ResourceId], [ResourceDisciplineId], [SkillDisciplineId], [SkillId])
)