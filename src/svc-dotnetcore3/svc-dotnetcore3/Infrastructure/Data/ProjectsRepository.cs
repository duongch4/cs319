using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
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
                select
                    p.Id, p.Title, p.ProjectStartDate, p.ProjectEndDate,
                    p.ManagerId, p.LocationId, p.Number,
                    u.FirstName, u.LastName,
                    l.Province, l.City 
                from
                    Projects p, Locations l, Users u
                where
                    p.LocationId = l.Id
                    AND p.ManagerId = u.Id
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<ProjectResource>(sql);
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
                    p.CreatedAt, p.UpdateAt, p.ManagerId, 
                    p.ProjectStartDate, p.ProjectEndDate
                from Positions as pos, Projects as p
                where pos.ResourceId = @UserId
                    and pos.ProjectId = p.projectId
                ;";

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
            List<HashSet<string>> openingsSkills = new List<HashSet<string>>();

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

            //TODO: For each user in usersSummary: create a position entry

            List<int> createdPositionIds = await this.CreatePositionsForAProject(connection, createdProjectId, openings, openingsSkills);
            await this.CreatePositionSkillsForPositions(connection, openingsSkills, createdPositionIds);
            return projectSummary.ProjectNumber;
        }

        private async Task<List<int>> CreatePositionsForAProject(
            SqlConnection connection, int projectId,
            IEnumerable<OpeningPositionsSummary> openings, List<HashSet<string>> openingsSkills
        )
        {
            List<int> createdPositionIds = new List<int>();
            foreach (var opening in openings)
            {
                openingsSkills.Add(opening.Skills);

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

                createdPositionIds.Add(id);
            }
            return createdPositionIds;
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
                    var sql = @"
                        insert into PositionSkills 
                        values
                            (
                                @PositionId,
                                (select Id from Skills where Name = @SkillName),
                                (select DisciplineId from Skills where Name = @SkillName)
                            )
                    ;";

                    await connection.QueryFirstOrDefaultAsync(sql, new
                    {
                        PositionId = positionIds[k],
                        SkillName = skill,
                    });
                }
            }
        }

        public async Task<string> UpdateAProject(ProjectProfile projectProfile, int locationId)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var projectSummary = projectProfile.ProjectSummary;
            var projectManager = projectProfile.ProjectManager;
            var usersSummary = projectProfile.UsersSummary;
            var openings = projectProfile.Openings;
            List<HashSet<string>> openingsSkills = new List<HashSet<string>>();

            var sql1 = @"
                select *
                from Projects
                where Number = @Number
            ;";
            var project = await connection.QueryFirstOrDefaultAsync<Project>(sql1, new { Number = projectSummary.ProjectNumber });
            Log.Information("{@old}", project);

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

            var sql2 = @"
                select *
                from Projects
                where Number = @Number
            ;";
            var projectnew = await connection.QueryFirstOrDefaultAsync<Project>(sql2, new { Number = projectSummary.ProjectNumber });
            Log.Information("{@new}", projectnew);

            var sqlGetProjectId = @"
                select Id from Projects where Number = @Number
            ";
            var projectId = await connection.QueryFirstOrDefaultAsync<int>(sqlGetProjectId, new { Number = projectSummary.ProjectNumber });

            if (success == 1)
            {
                List<int> createdPositionIds = await this.CreatePositionsForAProject(connection, projectId, openings, openingsSkills);
                await this.CreatePositionSkillsForPositions(connection, openingsSkills, createdPositionIds);
                return projectSummary.ProjectNumber;
            }
            else
            {
                return null;
            }

            // return projectSummary.ProjectNumber;

            // var sql = @"
            //     update
            //         Projects
            //     set 
            //         Number = @Number,
            //         Title = @Title,
            //         LocationId = @LocationId
            //     where 
            //         Id = @Id
            // ;";

            // using var connection = new SqlConnection(connectionString);
            // connection.Open();
            // int result = await connection.ExecuteAsync(sql, new
            // {
            //     project.Id,
            //     Number = project.Number,
            //     Title = project.Title,
            //     LocationId = project.LocationId
            // });
            // return result == 1 ? project : null;
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
