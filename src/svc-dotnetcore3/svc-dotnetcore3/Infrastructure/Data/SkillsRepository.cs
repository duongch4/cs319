using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Serilog;

namespace Web.API.Infrastructure.Data
{
    public class SkillsRepository : ISkillsRepository
    {
        private readonly string connectionString = string.Empty;

        public SkillsRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }
        //GET
        public async Task<IEnumerable<Skill>> GetAllSkills()
        {
            var sql = @"
                select *
                from Skills
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Skill>(sql);
        }
        public async Task<IEnumerable<Skill>> GetSkillsWithDiscipline(string disciplineName)
        {
            var sql = @"
                select
                    s.Id, s.DisciplineId, s.Name
                from Skills s, Disciplines d
                where
                    d.Id = s.DisciplineId
                    AND d.Name = @DisciplineName
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Skill>(sql, new { DisciplineName = disciplineName });
        }
        public async Task<IEnumerable<Skill>> GetSkillsWithName(string skillName)
        {
            var sql = @"
                select *
                from Skills
                where Name = @SkillName
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Skill>(sql, new { SkillName = skillName });
        }
        public async Task<Skill> GetASkill(int skillId)
        {
            var sql = @"
                select *
                from Skills
                where Id = @SkillId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Skill>(sql, new { SkillId = skillId });
        }
        //POST
        public async Task<Skill> CreateASkill(Skill skill)
        {
            var sql = @"
                insert into Skills
                    (DisciplineId, Name)
                values
                    (@DisciplineId, @Name);
                select cast(scope_identity() as int);
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            skill.Id = await connection.QuerySingleAsync<int>(sql, new
            {
                DisciplineId = skill.DisciplineId,
                Name = skill.Name
            });

            return skill;
        }
        //PUT
        public async Task<Skill> UpdateASkill(Skill skill)
        {
            var sql = @"
                update
                    Skills
                set 
                    DisciplineId = @DisciplineId,
                    Name = @Name
                where 
                    Id = @Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new
            {
                Id = skill.Id,
                Number = skill.Name,
                DisciplineId = skill.DisciplineId
            });
            return (result == 1) ? skill : null;
        }
        //DELETE
        public async Task<Skill> DeleteASkill(int skillId)
        {
            var skill = await GetASkill(skillId);
            var sql = @"
                delete from Skills
                where Id = @SkillId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.ExecuteAsync(sql, new { SkillId = skillId });
            return skill;
        }

        public async Task<IEnumerable<ResourceSkill>> GetUserSkills(User user)
        {
            var sql = @"
            select 
                rs.ResourceId, d.Name as ResourceDisciplineName, s.Id, s.Name
            from 
                ResourceSkill as rs, Disciplines as d, Skills as s
            where
                rs.ResourceId = @Id
                and rs.ResourceDisciplineId = d.Id
                and rs.SkillDisciplineId = s.DisciplineId
                and rs.SkillId = s.Id;
                ";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<ResourceSkill>(sql, new
            {
                Id = user.Id
            });
        }

        public async Task<ResourceSkill> DeleteResourceSkill(ResourceSkill skill)
        {
            var sql = @"
                delete from ResourceSkill
                where ResourceId = @ResourceId
                and SkillDisciplineId = (select Id 
                                        from Disciplines 
                                        where Name = @ResourceDisciplineName)
                and ResourceDisciplineID = SkillDisciplineID
                and SkillId = (select Id
                                from Skills
                                where Name = @Name);

            ";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.QueryAsync<ResourceDiscipline>(sql, new
            {
                ResourceId = skill.ResourceId,
                ResourceDisciplineName = skill.ResourceDisciplineName,
                Name = skill.Name
            });
            return skill;
        }
        public async Task<ResourceSkill> InsertResourceSkill(ResourceSkill skill)
        {
            var sql = @"
                insert into ResourceSkill
                values 
                    (@ResourceId, 
                    (select Id from Disciplines where Name = @ResourceDisciplineName), 
                    (select Id from Disciplines where Name = @ResourceDisciplineName), 
                    (select Id from Skills where Name = @Name))
                ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.QueryFirstOrDefaultAsync(sql, new
            {
                ResourceId = skill.ResourceId,
                ResourceDisciplineName = skill.ResourceDisciplineName,
                Name = skill.Name
            });
            return skill;
        }

        public async Task<int> GetSkillByDisciplineAndName(ResourceSkill skill)
        {
            var sql = @"select s.id
                from Disciplines as d, Skills as s
                where d.[Name] = @DisciplineName
	            and d.id = s.DisciplineId
	            and s.[Name] = @SkillName;";
            
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var result = await connection.QuerySingleAsync(sql, new {
                DisciplineName = skill.ResourceDisciplineName,
                SkillName = skill.Name
            });
            Log.Information(result);
            return result;
        }
    }
}