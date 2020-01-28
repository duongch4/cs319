using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;

namespace Web.API.Infrastructure.Data
{
    public class LocationsRepository : ILocationsRepository
    {
        private readonly string connectionString = string.Empty;

        public LocationsRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<Location>> GetAllLocations()
        {
            var sql = @"
                select
                    Id, Code, [Name]
                from
                    Locations
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Location>(sql);
        }

        public async Task<Location> GetALocation(string locationCode)
        {
            var sql = @"
                select
                    Id, Code, [Name]
                from
                    Locations
                where 
                    Code = @Code
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Location>(sql, new { Code = locationCode });
        }
    }
}
