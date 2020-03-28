using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Application.Communication;
using Web.API.Resources;
using System;
using System.Text.Json;
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

        private IEnumerable<ProjectResource> GetSorted(IEnumerable<ProjectResource> projects, string orderKey, string order)
        {
            orderKey = String.IsNullOrEmpty(orderKey) ? "utilization" : orderKey.ToLower();
            order = String.IsNullOrEmpty(order) ? "desc" : order.ToLower();
            switch (order)
            {
                case "desc":
                    switch (orderKey)
                    {
                        case "title":
                            return projects.OrderByDescending(project => project.Title);
                        case "province":
                            return projects.OrderByDescending(project => project.Province);
                        case "city":
                            return projects.OrderByDescending(project => project.City);
                        case "enddate":
                            return projects.OrderByDescending(project => project.ProjectEndDate);
                        default:
                            return projects.OrderByDescending(project => project.ProjectStartDate);
                    }
                default:
                    switch (orderKey)
                    {
                        case "title":
                            return projects.OrderBy(project => project.Title);
                        case "province":
                            return projects.OrderBy(project => project.Province);
                        case "city":
                            return projects.OrderBy(project => project.City);
                        case "enddate":
                            return projects.OrderBy(project => project.ProjectEndDate);
                        default:
                            return projects.OrderBy(project => project.ProjectStartDate);
                    }
            }
        }

        public async Task<IEnumerable<ProjectResource>> GetAllProjectResources(string orderKey, string order, int page)
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
                    AND p.ProjectEndDate > @DateTimeSpecific
                ORDER BY
                    p.Id
                    OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                    FETCH NEXT @RowsPerPage ROWS ONLY
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var projects = await connection.QueryAsync<ProjectResource>(sql, new
            {
                DateTimeSpecific = DateTime.Today,
                PageNumber = page,
                RowsPerPage = 50
            });
            return GetSorted(projects, orderKey, order);
        }

        public async Task<IEnumerable<ProjectResource>> GetAllProjectResourcesWithTitle(string searchWord, string orderKey, string order, int page)
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
                    AND LOWER(TRIM(p.Title)) LIKE @SearchWord
                ORDER BY
                    p.Id
                    OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                    FETCH NEXT @RowsPerPage ROWS ONLY
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var projects = await connection.QueryAsync<ProjectResource>(sql, new
            {
                SearchWord = GetFilteredSearchWord(searchWord),
                PageNumber = page,
                RowsPerPage = 50
            });
            return GetSorted(projects, orderKey, order);
        }

        private string GetFilteredSearchWord(string searchWordReq)
        {
            return String.IsNullOrEmpty(searchWordReq) ? "%" : $"%{searchWordReq.ToLower()}%";
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

        public async Task<IEnumerable<ProjectResource>> GetAllProjectResourcesOfUser(string userId)
        {
            var sql = @"
                SELECT
                    p.Id, p.Title, p.ProjectStartDate, p.ProjectEndDate,
                    p.ManagerId, p.LocationId, p.Number,
                    u.FirstName, u.LastName,
                    l.Province, l.City
                FROM
                    Positions as pos, Projects as p, Users u, Locations l
                WHERE
                    pos.ResourceId = u.Id
                    AND pos.ResourceId = @UserId 
                    AND pos.ProjectId = p.Id
                    AND l.Id = p.LocationId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<ProjectResource>(sql, new { UserId = userId });
        }

        public async Task<string> CreateAProject(ProjectProfile projectProfile, int locationId)
        {
            using var connection = new SqlConnection(connectionString);

            var createdProjectId = await this.CreateAProject(connection, locationId, projectProfile.ProjectSummary, projectProfile.ProjectManager);

            if (createdProjectId != 0)
            {
                if (projectProfile.Openings != null)
                {
                    foreach (var opening in projectProfile.Openings)
                    {
                        var newOpeningId = await this.CreateAnOpeningPosition(connection, opening, createdProjectId);

                        if (opening.Skills.Count != 0)
                        {
                            foreach (var skill in opening.Skills)
                            {
                                await this.CreatePositionSkill(connection, newOpeningId, skill);
                            }
                        }
                    }
                }
            }

            return projectProfile.ProjectSummary.ProjectNumber;
        }

        private async Task<int> CreateAProject(
            SqlConnection connection, int locationId,
            ProjectSummary projectSummary, ProjectManager projectManager
        )
        {
            var sql = @"
                INSERT INTO Projects 
                    ([Number], [Title], [LocationId], [ManagerId], [ProjectStartDate], [ProjectEndDate])
                VALUES 
                    (@Number, @Title, @LocationId, @ManagerId, @ProjectStartDate, @ProjectEndDate);
                SELECT CAST(scope_identity() as int);
            ;";

            connection.Open();
            var createdProjectId = await connection.QuerySingleAsync<int>(sql, new
            {
                Number = projectSummary.ProjectNumber,
                Title = projectSummary.Title,
                LocationId = locationId,
                ManagerId = projectManager.UserID,
                ProjectStartDate = projectSummary.ProjectStartDate,
                ProjectEndDate = projectSummary.ProjectEndDate
            });
            connection.Close();

            return createdProjectId;
        }

        private async Task<int> CreateAnOpeningPosition(SqlConnection connection, OpeningPositionsSummary opening, int projectId)
        {
            var sql = @"
                INSERT INTO Positions
                    ([DisciplineId], [ProjectId], [ProjectedMonthlyHours], [ResourceId], [PositionName], [YearsOfExperience], [IsConfirmed])
                VALUES
                    (
                        (SELECT Id FROM Disciplines WHERE Name = @DisciplineName),
                        @ProjectId, @ProjectedMonthlyHours,
                        NULL, NULL, @YearsOfExperience, 0
                    );
                SELECT CAST(scope_identity() as int);
            ;";

            string hours = JsonSerializer.Serialize<JsonElement>(opening.CommitmentMonthlyHours);

            connection.Open();
            var id = await connection.QuerySingleAsync<int>(sql, new
            {
                DisciplineName = opening.Discipline,
                ProjectId = projectId,
                ProjectedMonthlyHours = hours,
                YearsOfExperience = opening.YearsOfExp,
            });
            connection.Close();

            return id;
        }

        private async Task CreatePositionSkill(SqlConnection connection, int positionId, string skill)
        {
            var sql = @"
                INSERT INTO PositionSkills 
                VALUES
                    (
                        @PositionId,
                        (SELECT Id FROM Skills WHERE Name = @SkillName),
                        (SELECT DisciplineId FROM Positions WHERE Id = @PositionId)
                    )
            ;";

            connection.Open();
            await connection.QueryFirstOrDefaultAsync(sql, new
            {
                PositionId = positionId,
                SkillName = skill,
            });
            connection.Close();
        }

        private async Task CreatePositionSkill(SqlConnection connection, int positionId, int skillId)
        {
            var sql = @"
                INSERT INTO PositionSkills 
                VALUES
                    (
                        @PositionId,
                        @SkillId,
                        (SELECT DisciplineId FROM Positions WHERE Id = @PositionId)
                    )
            ;";

            connection.Open();
            await connection.QueryFirstOrDefaultAsync(sql, new
            {
                PositionId = positionId,
                SkillId = skillId,
            });
            connection.Close();
        }

        public async Task<string> UpdateAProject(ProjectProfile projectProfile, int locationId)
        {
            using var connection = new SqlConnection(connectionString);

            var projectSummary = projectProfile.ProjectSummary;
            var projectManager = projectProfile.ProjectManager;
            var success = await this.UpdateAProject(connection, locationId, projectSummary, projectManager);

            if (success == 1)
            {
                if (projectProfile.Openings != null)
                {
                    var projectId = await this.GetProjectId(connection, projectSummary.ProjectNumber);
                    var currentOpeningIds = await this.GetCurrentOpeningIdsForProject(connection, projectId);
                    foreach (var opening in projectProfile.Openings)
                    {
                        if (!currentOpeningIds.Contains(opening.PositionID))
                        {
                            var newOpeningId = await this.CreateAnOpeningPosition(connection, opening, projectId);
                            if (opening.Skills.Count != 0)
                            {
                                foreach (var skill in opening.Skills)
                                {
                                    await this.CreatePositionSkill(connection, newOpeningId, skill);
                                }
                            }
                        }
                        else
                        {
                            var updateOpeningSuccess = await this.UpdateAnOpeningPosition(connection, opening, projectId);
                            if (updateOpeningSuccess == 1)
                            {
                                if (opening.Skills.Count != 0)
                                {
                                    var currentSkillIds = await GetCurrentPositionSkillIds(connection, opening.PositionID);
                                    var openingSkillIds = await GetSkillIds(connection, opening.Skills, opening.Discipline);
                                    foreach (var id in openingSkillIds)
                                    {
                                        if (!currentSkillIds.Contains(id))
                                        {
                                            await this.CreatePositionSkill(connection, opening.PositionID, id);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return projectSummary.ProjectNumber;
            }
            else
            {
                return null;
            }
        }

        private async Task<IEnumerable<int>> GetCurrentPositionSkillIds(SqlConnection connection, int positionId)
        {
            var sql = @"
                SELECT SkillId
                FROM PositionSkills
                WHERE
                    SkillDisciplineId = (SELECT DisciplineId FROM Positions WHERE Id = @PositionId)
                    AND PositionId = @PositionId
            ";

            connection.Open();
            var skillIds = await connection.QueryAsync<int>(sql, new
            {
                PositionId = positionId
            });
            connection.Close();

            return skillIds;
        }

        private async Task<IEnumerable<int>> GetSkillIds(SqlConnection connection, IEnumerable<string> skills, string disciplineName)
        {
            var sql = @"
                SELECT Id
                FROM Skills
                WHERE
                    DisciplineId = (SELECT Id FROM Disciplines WHERE Name = @DisciplineName)
                    AND Name IN @SkillsNames
            ";

            connection.Open();
            var skillIds = await connection.QueryAsync<int>(sql, new
            {
                DisciplineName = disciplineName,
                SkillsNames = skills
            });
            connection.Close();

            return skillIds;
        }

        private async Task<int> UpdateAnOpeningPosition(SqlConnection connection, OpeningPositionsSummary opening, int projectId)
        {
            var sql = @"
                UPDATE Positions 
                SET
                    DisciplineId = (SELECT Id FROM Disciplines WHERE Name = @DisciplineName),
                    ProjectId = @ProjectId,
                    ProjectedMonthlyHours = @ProjectedMonthlyHours,
                    ResourceId = NULL,
                    YearsOfExperience = @YearsOfExperience,
                    IsConfirmed = 0
                WHERE
                    Id = @PositionId
            ;";

            var hours = JsonSerializer.Serialize<JsonElement>(opening.CommitmentMonthlyHours);

            connection.Open();
            int success = await connection.ExecuteAsync(sql, new
            {
                DisciplineName = opening.Discipline,
                ProjectId = projectId,
                ProjectedMonthlyHours = hours,
                YearsOfExperience = opening.YearsOfExp,
                PositionId = opening.PositionID
            });
            connection.Close();

            return success;
        }

        private async Task<IEnumerable<int>> GetCurrentOpeningIdsForProject(SqlConnection connection, int projectId)
        {
            var sql = @"
                SELECT Id
                FROM Positions
                WHERE
                    ProjectId = @ProjectId
                    AND ResourceId IS NULL
            ";

            connection.Open();
            var ids = await connection.QueryAsync<int>(sql, new
            {
                ProjectId = projectId
            });
            connection.Close();

            return ids;
        }

        private async Task<int> UpdateAProject(
            SqlConnection connection, int locationId,
            ProjectSummary projectSummary, ProjectManager projectManager
        )
        {
            var sql = @"
                UPDATE Projects 
                SET 
                    Title = @Title,
                    LocationId = @LocationId,
                    ManagerId = @ManagerId,
                    ProjectStartDate = @ProjectStartDate,
                    ProjectEndDate = @ProjectEndDate
                WHERE
                    Number = @Number
            ;";

            connection.Open();
            int success = await connection.ExecuteAsync(sql, new
            {
                Number = projectSummary.ProjectNumber,
                Title = projectSummary.Title,
                LocationId = locationId,
                ManagerId = projectManager.UserID,
                ProjectStartDate = projectSummary.ProjectStartDate,
                ProjectEndDate = projectSummary.ProjectEndDate
            });
            connection.Close();

            return success;
        }

        private async Task<int> GetProjectId(SqlConnection connection, string projectNumber)
        {
            var sqlGetProjectId = @"
                SELECT Id
                FROM Projects
                WHERE Number = @Number
            ;";

            connection.Open();
            var projectId = await connection.QueryFirstOrDefaultAsync<int>(sqlGetProjectId, new { Number = projectNumber });
            connection.Close();

            return projectId;
        }

        public async Task<Project> DeleteAProject(string number)
        {
            var project = await GetAProject(number);
            var sql = @"
                DELETE
                FROM Projects
                WHERE Number = @Number
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.ExecuteAsync(sql, new { Number = number });
            connection.Close();
            
            return project;
        }
    }
}
