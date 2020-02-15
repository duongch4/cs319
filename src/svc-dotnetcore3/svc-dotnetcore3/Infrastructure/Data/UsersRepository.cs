using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;

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
                    Id, FirstName, LastName, Username, LocationId
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
                    Id, FirstName, LastName, Username, LocationId
                from
                    Users
                where 
                    Username = @Username
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }
        public async Task<IEnumerable<User>> GetAllUsersAtLocation(Location location) {
            return null;
        }
        public async Task<IEnumerable<User>> GetAllUsersWithDiscipline(Discipline discipline) {
            return null;
        }
        public async Task<IEnumerable<User>> GetAllUsersWithSkill(Skill skill) {
            return null;
        }
        public async Task<IEnumerable<User>> GetAllUsersWithAvailability(Availability requestedAvailability) {
            return null;
        }
        public async Task<IEnumerable<User>> GetAllUsersOverNUtilization(int nUtil) {
            return null;
        }
        public async Task<IEnumerable<User>> GetAllUsersOnProject(Project project) {
            return null;
        }
        public async Task<IEnumerable<User>> GetAllUsersWithYearsOfExp(Discipline discipline) {
            return null;
        }
        
        //PUT
        public async Task<User> UpdateAUser(User user) {
            return null;
        }

    }
}
