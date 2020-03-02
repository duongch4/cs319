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

        public async Task<User> GetAUser(int userId)
        {
            var sql = @"
                select
                    Id, FirstName, LastName, Username, LocationId, IsAdmin, IsManager
                from
                    Users
                where 
                    Id = @Id";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = userId });
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
                    p.IsConfirmed, d.Name AS DisciplineName, rd.YearsOfExperience,
                    l.Province, l.City
                from
                    Users u, Positions p, Locations l, ResourceDiscipline rd, Disciplines d
                where
                    p.ProjectId = @ProjectId
                    AND p.ResourceId = u.Id
                    AND u.Id != @ProjectManagerId
                    AND u.LocationId = l.Id
                    AND rd.ResourceId = u.Id
                    AND rd.DisciplineId = d.Id
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

        public async Task<int> UpdateAUser(UserSummary user, Location location) 
        {
             var sql = @"
                update
                    Users
                set 
                    FirstName = @FirstName,
                    LastName = @LastName,
                    LocationId = @LocationId
                where 
                    Id = @Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new
            {
                Id = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LocationId = location.Id
            });
            return result == 1 ? user.UserID : -1;
        }

        public async Task<IEnumerable<UserResource>> GetAllUsersGeneral()
        {
            var sql = @"
                select
                    u.Id, u.FirstName, u.LastName, u.Username, u.LocationId, l.City, l.Province
                from
                    Users as u, Locations as l
                where
                    u.LocationId = l.Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<UserResource>(sql);
        }
    }
}
