using Web.API.Application.Models;
using Web.API.Application.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using Serilog;

namespace Web.API.Infrastructure.Data
{
    public class DisciplinesRespository : IDisciplinesRepository
    {
        private readonly string connectionString = string.Empty;
        public DisciplinesRespository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }
        public async Task<Discipline> GetADiscipline(int disciplineId)
        {
            var sql = @"
                select *
                from Disciplines
                where Id = @DisciplineId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Discipline>(sql, new { DisciplineId = disciplineId });
        }
        public async Task<IEnumerable<Discipline>> GetAllDisciplines()
        {
            var sql = @"
                select *
                from Disciplines
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Discipline>(sql);
        }
        public async Task<IEnumerable<Discipline>> GetDisciplinesByName(string disciplineName)
        {
            var sql = @"
                select *
                from Disciplines
                where Name = @DisciplineName
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Discipline>(sql, new { DisciplineName = disciplineName });
        }
        // POST
        public async Task<Discipline> CreateADiscipline(Discipline discipline)
        {
            var sql = @"
                insert into Disciplines
                    (Name)
                values
                    (@Name);
                select cast(scope_identity() as int);
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            discipline.Id = await connection.QuerySingleAsync<int>(sql, new { Name = discipline.Name });
            return discipline;
        }
        // PUT
        public async Task<Discipline> UpdateADiscipline(Discipline discipline)
        {
            var sql = @"
                update
                    Disciplines
                set 
                    Name = @Name
                where 
                    Id = @Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new
            {
                Id = discipline.Id,
                Name = discipline.Name
            });
            return (result == 1) ? discipline : null;
        }
        // DELETE
        public async Task<Discipline> DeleteADiscipline(int disciplineId)
        {
            var discipline = await GetADiscipline(disciplineId);
            var sql = @"
                delete from Disciplines
                where Id = @DisciplineId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.ExecuteAsync(sql, new { DisciplineId = disciplineId });
            return discipline;
        }

        public async Task<IEnumerable<ResourceDiscipline>> GetUserDisciplines(User user)
        {
            var sql = @"
                select rd.ResourceId, d.Name, rd.YearsOfExperience
                from ResourceDiscipline as rd, Disciplines as d
                where d.Id = rd.DisciplineId and rd.ResourceId = " + user.Id + ";";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<ResourceDiscipline>(sql);
        }

        public async Task<ResourceDiscipline> DeleteResourceDiscipline(ResourceDiscipline discipline)
        {
            var sql = @"
               delete from ResourceDiscipline 
                where ResourceId = @ResourceId
                AND DisciplineId = (select Id	
                                    from Disciplines
                                    where Name = @DisciplineName)
                                    ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.QueryAsync<ResourceDiscipline>(sql, new
            {
                ResourceId = discipline.ResourceId,
                DisciplineName = discipline.Name
            });
            return discipline;
        }

        public async Task<ResourceDiscipline> InsertResourceDiscipline(ResourceDiscipline discipline)
        {
            Log.Information("{@a}", discipline);

            var sql = @"
                insert into ResourceDiscipline 
	                values (@ResourceId, 
                           (select Id from Disciplines where Name = @DisciplineName), 
                           @YearsOfExperience)
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.QueryFirstOrDefaultAsync(sql, new
            {
                ResourceId = discipline.ResourceId,
                DisciplineName = discipline.Name,
                YearsOfExperience = discipline.YearsOfExperience
            });
            return discipline;
        }
    }
}
