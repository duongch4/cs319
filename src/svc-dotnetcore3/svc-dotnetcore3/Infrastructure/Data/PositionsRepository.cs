using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Resources;
using Serilog;

namespace Web.API.Infrastructure.Data
{
    public class PositionsRepository : IPositionsRepository
    {
        private readonly string connectionString = string.Empty;

        public PositionsRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
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
            return await connection.QueryFirstOrDefaultAsync<Position>(sql, new { PositionId = positionId });
        }
        public async Task<IEnumerable<PositionResource>> GetPositionsOfUser(User user)
        {
            var sql = @"
                select d.Name as DisciplineName, pos.ProjectedMonthlyHours, p.Title as ProjectTitle
                from Disciplines as d, Positions as pos, Projects as p
                where pos.DisciplineId = d.Id and pos.ProjectId = p.Id and pos.ResourceId = @UserId;
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<PositionResource>(sql, new { UserId = user.Id });
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
                select
                    p.ProjectedMonthlyHours AS CommitmentMonthlyHours, d.Name AS Discipline,
                    rd.YearsOfExperience, STRING_AGG (s.Name, ',') as Skills
                from
                    Positions p, ResourceDiscipline rd, Disciplines d, Skills s
                where
                    p.ProjectId = @ProjectId
                    AND p.ResourceId IS NOT NULL
                    AND p.DisciplineId = d.Id
                    AND p.DisciplineId = s.DisciplineId
                    AND p.DisciplineId = rd.DisciplineId
                group by
                    p.DisciplineId, p.ProjectedMonthlyHours,
                    d.Name, rd.YearsOfExperience
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
        //PUT
        public async Task<Position> UpdateAPosition(Position position)
        {
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
                Id = position.Id,
                DisciplineId = position.DisciplineId,
                ProjectId = position.ProjectId,
                ProjectedMonthlyHours = position.ProjectedMonthlyHours,
                ResourceId = position.ResourceId,
                PositionName = position.PositionName,
                IsConfirmed = position.IsConfirmed
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