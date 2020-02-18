using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Resources;

namespace Web.API.Infrastructure.Data
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string connectionString = string.Empty;

        public UsersRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var sql = @"
                select
                    Id, FirstName, LastName, Username, LocationId, IsAdmin, IsManager
                from
                    Users
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql);
        }

        public async Task<User> GetAUser(string username)
        {
            var sql = @"
                select
                    Id, FirstName, LastName, Username, LocationId, IsAdmin, IsManager
                from
                    Users
                where 
                    Username = @Username
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<IEnumerable<User>> GetAllUsersAtLocation(Location location) 
        {
            var sql = @"
                select 
                    Id, FirstName, LastName, Username, LocationId, IsAdmin, IsManager
                from Users
                where LocationId = @LocationId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql, new { LocationId = location.Id});
        }

        public async Task<IEnumerable<User>> GetAllUsersWithDiscipline(Discipline discipline) 
        {
            var sql = @"
                select 
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.IsAdmin, u.IsManager
                from Users as u, ResourceDiscipline rd
                where rd.DisciplineId = @DisciplineId
                    and rd.ResourceId = u.Id
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql, new { DisciplineId = discipline.Id});
        }

        public async Task<IEnumerable<User>> GetAllUsersWithSkill(Skill skill)
        {
            var sql = @"
                select 
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.IsAdmin, u.IsManager
                from Users as u, ResourceSkill as rs
                where rs.SkillId = @SkillId
                    and rs.ResourceId = u.Id
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql, new { SkillId = skill.Id});
        }

        // //TODO: @Chi plz
        // public async Task<IEnumerable<User>> GetAllUsersWithAvailability(Availability requestedAvailability)
        // {
        //     return null;
        // }

        // public async Task<IEnumerable<User>> GetAllUsersOverNUtilization(int nUtil) 
        // {
        //     return null;
        // }

        public async Task<IEnumerable<User>> GetAllUsersOnProject(Project project) 
        {
            var sql = @"
                select 
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, u.IsAdmin, u.IsManager
                from 
                    Users as u, Positions pos
                where pos.ProjectId = @ProjectId
                    and pos.ResourceId = u.Id
            ;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<User>(sql, new { ProjectId = project.Id});
        }

        public async Task<IEnumerable<UserResource>> GetAllUsersResourceOnProject(int projectId, int projectManagerId)
        {
            var sql = @"
                select
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId,
                    p.IsConfirmed,
                    l.Province, l.City,
                    rd.DisciplineName, rd.YearsOfExperience
                from
                    Users u, Positions p, Locations l, ResourceDiscipline rd
                where
                    p.ProjectId = @ProjectId
                    AND p.ResourceId = u.Id
                    AND u.Id != @ProjectManagerId
                    AND u.LocationId = l.Id
                    AND rd.ResourceId = u.Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<UserResource>(sql, new { ProjectId = projectId, ProjectManagerId = projectManagerId });
        }

        // public async Task<IEnumerable<User>> GetAllUsersWithYearsOfExp(Discipline discipline, int yrsOfExp)
        // {
        //     return null;
        // }

        public async Task<User> GetPMOfProject(Project project)
        {
            var sql = @"
                select *
                from 
                    User
                where Id = @ProjectManagerId
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QuerySingleAsync<User>(sql, new { ProjectManagerId = project.ManagerId});
        }

        public async Task<User> UpdateAUser(User user) 
        {
             var sql = @"
                update
                    Users
                set 
                    Id = @Id,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Username = @Username,
                    LocationId = @LocationId,
                    IsAdmin = @IsAdmin,
                    IsManager = @IsManager
                where 
                    Id = @Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.LocationId,
                user.IsAdmin,
                user.IsManager
            });
            return result == 1 ? user : null;
        }
    }
}
