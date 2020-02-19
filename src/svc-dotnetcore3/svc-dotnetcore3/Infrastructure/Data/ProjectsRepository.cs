using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;

namespace Web.API.Infrastructure.Data
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly string connectionString = string.Empty;

        public ProjectsRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            var sql = @"
                select
                    Id, Number, Title, LocationId, CreatedAt, UpdatedAt
                from
                    Projects
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Project>(sql);
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
                    p.Id, p.Title, p.ProjectStartDate, p.ProjectEndDate, p.LocationId,
                    l.Province, l.City 
                from
                    Projects p, Locations l
                where
                    p.LocationId = l.Id
                    AND p.Number = @Number
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
                where pos.ResourceId = "+ user.Id + "and pos.ProjectId = p.Id;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Project>(sql, new { UserId = user.Id });
        }

        public async Task<Project> CreateAProject(Project project)
        {
            var sql = @"
                insert into Projects 
                    (Number, Title, LocationId)
                values 
                    (@Number, @Title, @LocationId);
                select cast(scope_identity() as int);
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var id = await connection.QuerySingleAsync<int>(sql, new
            {
                project.Number,
                project.Title,
                project.LocationId
            });
            project.Id = id;
            return project;
        }

        public async Task<Project> UpdateAProject(Project project)
        {
            var sql = @"
                update
                    Projects
                set 
                    Number = @Number,
                    Title = @Title,
                    LocationId = @LocationId
                where 
                    Id = @Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new
            {
                project.Id,
                Number = project.Number,
                Title = project.Title,
                LocationId = project.LocationId
            });
            return result == 1 ? project : null;
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
