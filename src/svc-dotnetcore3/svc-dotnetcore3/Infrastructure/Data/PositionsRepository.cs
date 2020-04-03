using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Resources;
using Serilog;

namespace Web.API.Infrastructure.Data
{
    public class PositionsRepository : IPositionsRepository
    {
        private readonly string connectionString = string.Empty;
        // private readonly System.Data.SqlClient.SqlConnection connection;


        public PositionsRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
            // connection = new SqlConnection(connectionString);
            // connection.Open();
        }
        public async Task<IEnumerable<Position>> GetAllPositions()
        {
            var sql = @"
                select *
                from Positions
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Position>(sql);
        }
        public async Task<Position> GetAPosition(int positionId)
        {
            var sql = @"
                select *
                from Positions
                where Id = @PositionId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var sqlRes = await connection.QueryFirstOrDefaultAsync<PositionRaw>(sql, new { PositionId = positionId });
            var monthlyHoursObj = JsonConvert.DeserializeObject(sqlRes.ProjectedMonthlyHours);
            
            Position position = await ConvertToPosition(sqlRes);
            return position;
        }

        public async Task<OpeningPositionsResource> GetAnOpeningPositionsResource(int positionId)
        {
            var sql = @" 
                begin
                if ((select Id 
                        from 
                            positions as pos
                        where exists (select 
                            PositionId 
                        from 
                            positionSkills as  ps2
                        where ps2.PositionId = @PositionId
                              and ps2.PositionId = pos.Id))
                    is null)
                    BEGIN
                            select
                                pos.Id as Id, 
                                pos.ProjectedMonthlyHours as CommitmentMonthlyHours, 
                                d.[Name] as Discipline, 
                                null as Skills,
                                pos.YearsOfExperience as YearsOfExperience
                            from 
                                positions as pos,
                                disciplines d
                            where 
                                pos.Id = @PositionId
                                and pos.DisciplineId = d.Id   ;   
                    END
                else 
                    BEGIN 
                            select
                                pos.Id as Id, 
                                pos.ProjectedMonthlyHours as CommitmentMonthlyHours, 
                                d.[Name] as Discipline, 
                                STRING_AGG (s.[Name], ',' )as Skills, 
                                pos.YearsOfExperience as YearsOfExperience
                            from 
                                positions as pos,
                                PositionSkills as ps,
                                disciplines d,
                                skills s
                            where 
                                pos.Id = 928
                                and pos.Id = ps.PositionId
                                and pos.ResourceId is null
                                and ps.SkillDisciplineId = d.Id
                                and ps.SkillId = s.Id
                            group by
                                pos.Id, 
                                pos.YearsOfExperience,
                                pos.ProjectedMonthlyHours,
                                d.Name;
                    end
                end
                ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QuerySingleOrDefaultAsync<OpeningPositionsResource>(sql, new { PositionId = positionId} );
        }
        public async Task<IEnumerable<PositionResource>> GetPositionsOfUser(string userId)
        {
            var sql = @"
                select
                    d.Name as DisciplineName, pos.ProjectedMonthlyHours,
                    p.Title as ProjectTitle, pos.Id AS PositionID, pos.PositionName
                from Disciplines as d, Positions as pos, Projects as p
                where pos.DisciplineId = d.Id and pos.ProjectId = p.Id and pos.ResourceId = @UserId;
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<PositionResource>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Position>> GetAllPositionsOfUser(string userId)
        {
             var sql = @"
                select 
                    Id, DisciplineId, ProjectId, ProjectedMonthlyHours,
                    ResourceId, PositionName, IsConfirmed 
                from Positions
                where ResourceId = @UserId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var sqlRes = await connection.QueryAsync<PositionRaw>(sql, new { UserId = userId });

            List<Position> positions = new List<Position>{}; 
            foreach (PositionRaw raw in sqlRes) {
                Position position = await ConvertToPosition(raw);
                positions.Add(position);
            }
            return positions;
        }
        public async Task<IEnumerable<Position>> GetAllUnassignedPositionsOfProject(Project project)
        {
            var sql = @"
                select *
                from Positions
                where
                    ProjectId = @ProjectId
                    AND ResourceId IS NOT NULL
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Position>(sql, new { ProjectId = project.Id });
        }

        public async Task<IEnumerable<OpeningPositionsResource>> GetAllUnassignedPositionsResourceOfProject(int projectId)
        {
            var sql = @"
                SELECT
                    p.Id, p.YearsOfExperience,
                    p.ProjectedMonthlyHours AS CommitmentMonthlyHours,
                    d.Name AS Discipline,
                    STRING_AGG (s.Name, ',') as Skills
                FROM
                    Positions p, Disciplines d, Skills s, PositionSkills ps
                WHERE
                    p.ProjectId = @ProjectId
                    AND p.ResourceId IS NULL
                    AND p.DisciplineId = d.Id
                    AND p.Id = ps.PositionId
					AND ps.SkillId = s.Id
					AND ps.SkillDisciplineId = s.DisciplineId
				GROUP BY
                    p.Id, p.YearsOfExperience,
                    p.ProjectedMonthlyHours,
                    d.Name
            ;";
            
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<OpeningPositionsResource>(sql, new { ProjectId = projectId });
        }

        //POST
        public async Task<Position> CreateAPosition(Position position)
        {
            var sql = @"
                insert into Positions
                    (
                        DisciplineId, ProjectId, ProjectedMonthlyHours,
                        ResourceId, PositionName, IsConfirmed
                    )
                values
                    (
                        @DisciplineId, @ProjectId, @ProjectedMonthlyHours,
                        @ResourceId, @PositionName, @IsConfirmed
                    );
                select cast(scope_identity() as int);
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            position.Id = await connection.QuerySingleAsync<int>(sql, new
            {
                DisciplineId = position.DisciplineId,
                ProjectId = position.ProjectId,
                ProjectedMonthlyHours = position.ProjectedMonthlyHours,
                ResourceId = position.ResourceId,
                PositionName = position.PositionName,
                IsConfirmed = position.IsConfirmed
            });

            return position;
        }

        private async Task<PositionRaw> ConvertToRawPosition(Position position) {
            string monthlyHours = JsonConvert.SerializeObject(position.ProjectedMonthlyHours);

            PositionRaw rawPosition = new PositionRaw{
                                                        Id = position.Id,
                                                        DisciplineId = position.DisciplineId,
                                                        ProjectId = position.ProjectId,
                                                        ProjectedMonthlyHours = monthlyHours,
                                                        ResourceId = position.ResourceId,
                                                        PositionName = position.PositionName,
                                                        IsConfirmed = position.IsConfirmed
                                                    };
            return await Task.FromResult(rawPosition);
        }

        private async Task<Position> ConvertToPosition(PositionRaw rawPosition) {
             var monthlyHoursObj = JsonConvert.DeserializeObject<Dictionary<string, int>>(rawPosition.ProjectedMonthlyHours);

            Position position = new Position{Id = rawPosition.Id, 
                                             DisciplineId = rawPosition.DisciplineId, 
                                             ProjectId = rawPosition.ProjectId, 
                                             ProjectedMonthlyHours = monthlyHoursObj,
                                             ResourceId = rawPosition.ResourceId, 
                                             PositionName = rawPosition.PositionName, 
                                             IsConfirmed = rawPosition.IsConfirmed};
            return await Task.FromResult(position);
        }

        //PUT
        public async Task<Position> UpdateAPosition(Position position)
        {
            var rawPosition = await ConvertToRawPosition(position);

            var sql = @"
                update
                    Positions
                set 
                    DisciplineId = @DisciplineId,
                    ProjectId = @ProjectId,
                    ProjectedMonthlyHours = @ProjectedMonthlyHours,
                    ResourceId = @ResourceId,
                    PositionName = @PositionName,
                    IsConfirmed = @IsConfirmed
                where 
                    Id = @Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new
            {
                Id = rawPosition.Id,
                DisciplineId = rawPosition.DisciplineId,
                ProjectId = rawPosition.ProjectId,
                ProjectedMonthlyHours = rawPosition.ProjectedMonthlyHours,
                ResourceId = rawPosition.ResourceId,
                PositionName = rawPosition.PositionName,
                IsConfirmed = rawPosition.IsConfirmed
            });
            return (result == 1) ? position : null;
        }
        //DELETE
        public async Task<Position> DeleteAPosition(int positionId)
        {
            var position = await GetAPosition(positionId);
            var sql = @"
                delete from Positions
                where Id = @PositionId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.ExecuteAsync(sql, new { PositionId = positionId });
            return position;
        }
    }
}