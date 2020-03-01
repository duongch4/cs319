using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using Serilog;

namespace Web.API.Infrastructure.Data
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly string connectionString = string.Empty;

        public ProjectsRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<ProjectResource>> GetAllProjects()
        {
            var sql = @"
                SELECT
                    p.Id, p.Title, p.ProjectStartDate, p.ProjectEndDate,
                    p.ManagerId, p.LocationId, p.Number,
                    u.FirstName, u.LastName,
                    l.Province, l.City 
                FROM
                    Projects p, Locations l, Users u
                WHERE
                    p.LocationId = l.Id
                    AND p.ManagerId = u.Id
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<ProjectResource>(sql, new { DateTimeNow = DateTime.Today });
        }

        public async Task<IEnumerable<ProjectResource>> GetProjectsWithEndDateAfterSpecificDate(DateTime dateTime, int page)
        {
            Log.Information("DateTime: {@dt}", dateTime);
            var sql = @"
                SELECT
                    p.Id, p.Title, p.ProjectStartDate, p.ProjectEndDate,
                    p.ManagerId, p.LocationId, p.Number,
                    u.FirstName, u.LastName,
                    l.Province, l.City 
                FROM
                    Projects p, Locations l, Users u
                WHERE
                    p.LocationId = l.Id
                    AND p.ManagerId = u.Id
                    AND p.ProjectEndDate > @DateTimeSpecific
                ORDER BY
                    p.ProjectStartDate
                    OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                    FETCH NEXT @RowsPerPage ROWS ONLY
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<ProjectResource>(sql, new
            {
                DateTimeSpecific = dateTime,
                PageNumber = page,
                RowsPerPage = 50
            });
        }

        public async Task<IEnumerable<Project>> GetMostRecentProjects()
        {
            var sql = @"
                select top(25)
                    Id, Number, Title, LocationId, CreatedAt, UpdatedAt
                from
                    Projects
                order by
                    UpdatedAt desc
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Project>(sql);
        }

        public async Task<Project> GetAProject(string projectNumber)
        {
            var sql = @"
                select *
                from Projects
                where Number = @Number
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Project>(sql, new { Number = projectNumber });
        }

        public async Task<ProjectResource> GetAProjectResource(string projectNumber)
        {
            var sql = @"
                select
                    p.Id, p.Title, p.ProjectStartDate, p.ProjectEndDate,
                    p.ManagerId, p.LocationId, p.Number,
                    u.FirstName, u.LastName,
                    l.Province, l.City 
                from
                    Projects p, Locations l, Users u
                where
                    p.LocationId = l.Id
                    AND p.Number = @Number
                    AND p.ManagerId = u.Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<ProjectResource>(sql, new { Number = projectNumber });
        }

        public async Task<IEnumerable<Project>> GetAllProjectsOfUser(User user)
        {
            var sql = @"
                select 
                    p.Id, p.Number, p.Title, p.LocationId, 
                    p.CreatedAt, p.UpdatedAt, p.ManagerId, 
                    p.ProjectStartDate, p.ProjectEndDate
                from Positions as pos, Projects as p
                where pos.ResourceId = " + user.Id + "and pos.ProjectId = p.Id;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Project>(sql, new { UserId = user.Id });
        }

        public async Task<string> CreateAProject(ProjectProfile projectProfile, int locationId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var projectSummary = projectProfile.ProjectSummary;
            var projectManager = projectProfile.ProjectManager;
            var usersSummary = projectProfile.UsersSummary;
            var openings = projectProfile.Openings;
            var openingsSKills = this.GetOpeningsSkills(openings);

            var createdProjectId = await this.CreateAProject(connection, locationId, projectSummary, projectManager);
            var openingIds = await this.GetOpeningIds(connection, createdProjectId);

            //TODO: For each user in usersSummary: create a position entry
            List<int> createdPositionIds = await this.CreatePositionsForAProject(connection, createdProjectId, openings, openingIds);
            await this.CreatePositionSkillsForPositions(connection, openingsSKills, createdPositionIds);
            return projectSummary.ProjectNumber;
        }

        private List<HashSet<string>> GetOpeningsSkills(IEnumerable<OpeningPositionsSummary> openings)
        {
            List<HashSet<string>> openingsSkills = new List<HashSet<string>>();
            foreach (var opening in openings)
            {
                openingsSkills.Add(opening.Skills);
            }
            return openingsSkills;
        }

        private async Task<List<int>> GetOpeningIds(SqlConnection connection, int projectId)
        {
            var sqlGetOpeningIds = @"
                select Id from Positions
                where ProjectId = @ProjectId AND ResourceId IS NULL
            ;";
            var openingIds = (List<int>)await connection.QueryAsync<int>(sqlGetOpeningIds, new { ProjectId = projectId });
            return openingIds;
        }

        private async Task<int> CreateAProject(
            SqlConnection connection, int locationId,
            ProjectSummary projectSummary, ProjectManager projectManager
        )
        {
            var sql = @"
                insert into Projects 
                    ([Number], [Title], [LocationId], [ManagerId], [ProjectStartDate], [ProjectEndDate])
                values 
                    (@Number, @Title, @LocationId, @ManagerId, @ProjectStartDate, @ProjectEndDate);
                select cast(scope_identity() as int);
            ;";
            var createdProjectId = await connection.QuerySingleAsync<int>(sql, new
            {
                Number = projectSummary.ProjectNumber,
                Title = projectSummary.Title,
                LocationId = locationId,
                ManagerId = projectManager.UserID,
                ProjectStartDate = projectSummary.ProjectStartDate,
                ProjectEndDate = projectSummary.ProjectEndDate
            });
            return createdProjectId;
        }

        private async Task<List<int>> CreatePositionsForAProject(
            SqlConnection connection, int projectId,
            IEnumerable<OpeningPositionsSummary> openings, IEnumerable<int> openingIds
        )
        {
            await this.RemoveAllOpenings(connection, openingIds);

            List<int> createdPositionIds = new List<int>();
            foreach (var opening in openings)
            {
                var id = await this.CreateAPosition(connection, opening, projectId);
                createdPositionIds.Add(id);
            }
            return createdPositionIds;
        }

        private async Task<int> CreateAPosition(SqlConnection connection, OpeningPositionsSummary opening, int projectId)
        {
            var sql = @"
                insert into Positions
                    ([DisciplineId], [ProjectId], [ProjectedMonthlyHours], [ResourceId], [PositionName], [YearsOfExperience], [IsConfirmed])
                values
                    (
                        (select Id from Disciplines where Name = @DisciplineName),
                        @ProjectId, @ProjectedMonthlyHours,
                        NULL, NULL, @YearsOfExperience, 0
                    );
                select cast(scope_identity() as int);
            ;";

            var id = await connection.QuerySingleAsync<int>(sql, new
            {
                DisciplineName = opening.Discipline,
                ProjectId = projectId,
                ProjectedMonthlyHours = opening.CommitmentMonthlyHours,
                YearsOfExperience = opening.YearsOfExp,
            });

            return id;
        }

        private async Task RemoveAllOpenings(
            SqlConnection connection, IEnumerable<int> openingIds
        )
        {
            foreach (var openingId in openingIds)
            {
                var sqlDel = @"
                    delete from PositionSkills where PositionId = @PositionId;
                    delete from Positions where Id = @PositionId;
                ";
                await connection.ExecuteAsync(sqlDel, new { PositionId = openingId });
            }
        }

        private async Task CreatePositionSkillsForPositions(
            SqlConnection connection, List<HashSet<string>> openingsSkills, List<int> positionIds
        )
        {
            for (int k = 0; k < openingsSkills.Count; k++)
            {
                if (openingsSkills[k].Count == 0) continue;

                foreach (var skill in openingsSkills[k])
                {
                    await this.CreatePositionSkill(connection, positionIds[k], skill);
                }
            }
        }

        private async Task CreatePositionSkill(SqlConnection connection, int positionId, string skill)
        {
            var sql = @"
                insert into PositionSkills 
                values
                    (
                        @PositionId,
                        (select Id from Skills where Name = @SkillName),
                        (select DisciplineId from Positions where Id = @PositionId)
                    )
            ;";

            await connection.QueryFirstOrDefaultAsync(sql, new
            {
                PositionId = positionId,
                SkillName = skill,
            });
        }

        public async Task<string> UpdateAProject(ProjectProfile projectProfile, int locationId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var projectSummary = projectProfile.ProjectSummary;
            var projectManager = projectProfile.ProjectManager;
            var usersSummary = projectProfile.UsersSummary;
            var openings = projectProfile.Openings;
            var openingsSKills = this.GetOpeningsSkills(openings);

            var success = await this.UpdateAProject(connection, locationId, projectSummary, projectManager);
            var projectId = await this.GetProjectId(connection, projectSummary.ProjectNumber);
            var openingIds = await this.GetOpeningIds(connection, projectId);

            if (success == 1)
            {
                //TODO: For each user in usersSummary: create a position entry
                List<int> createdPositionIds = await this.CreatePositionsForAProject(connection, projectId, openings, openingIds);
                await this.CreatePositionSkillsForPositions(connection, openingsSKills, createdPositionIds);
                return projectSummary.ProjectNumber;
            }
            else
            {
                return null;
            }
        }

        private async Task<int> UpdateAProject(
            SqlConnection connection, int locationId,
            ProjectSummary projectSummary, ProjectManager projectManager
        )
        {
            var sql = @"
                update Projects 
                set 
                    Title = @Title,
                    LocationId = @LocationId,
                    ManagerId = @ManagerId,
                    ProjectStartDate = @ProjectStartDate,
                    ProjectEndDate = @ProjectEndDate
                where
                    Number = @Number
            ;";
            int success = await connection.ExecuteAsync(sql, new
            {
                Number = projectSummary.ProjectNumber,
                Title = projectSummary.Title,
                LocationId = locationId,
                ManagerId = projectManager.UserID,
                ProjectStartDate = projectSummary.ProjectStartDate,
                ProjectEndDate = projectSummary.ProjectEndDate
            });
            return success;
        }

        private async Task<int> GetProjectId(SqlConnection connection, string projectNumber)
        {
            var sqlGetProjectId = @"
                select Id from Projects where Number = @Number
            ;";
            var projectId = await connection.QueryFirstOrDefaultAsync<int>(sqlGetProjectId, new { Number = projectNumber });
            return projectId;
        }

        public async Task<Project> DeleteAProject(string number)
        {
            var project = await GetAProject(number);
            var sql = @"
                delete from Projects
                where Number = @Number
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.ExecuteAsync(sql, new { Number = number });
            return project;
        }
    }
}
