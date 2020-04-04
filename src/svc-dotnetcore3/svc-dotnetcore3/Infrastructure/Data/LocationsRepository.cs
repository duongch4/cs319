using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;
using Web.API.Resources;

using System.Linq;
using Newtonsoft.Json;
using Serilog;

namespace Web.API.Infrastructure.Data
{
    public class LocationsRepository : ILocationsRepository
    {
        private readonly string connectionString = string.Empty;
        // private readonly System.Data.SqlClient.SqlConnection connection;
        private readonly Dictionary<string, IEnumerable<string>> locations;

        public LocationsRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
            // connection = new SqlConnection(connectionString);
            // connection.Open();
            this.locations = new Dictionary<string, IEnumerable<string>>();
        }

        public async Task<IEnumerable<Location>> GetAllLocations()
        {
            var sql = @"
                SELECT
                    Id, Province, City
                FROM
                    Locations
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<Location>(sql);
        }

        public async Task<IEnumerable<MasterLocation>> GetAllLocationsGroupByProvince()
        {
            var sql = @"
                SELECT
                    p.Name AS Province,
                    STRING_AGG(CONVERT(nvarchar(max),CONCAT(l.City, '#', l.Id)), ',') as CitiesIds
                FROM Provinces p
                LEFT JOIN Locations l
                    ON l.Province = p.Name
                GROUP BY
                    p.Name
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryAsync<MasterLocation>(sql);
        }

        public async Task<Location> GetALocation(int locationId) {

             var sql = @"
                select Id, Province, City
                from Locations
                where Id = @Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Location>(sql, new { Id = locationId });
        }

        public async Task<Location> GetUserLocation(User user) {
            var sql = @"
                select
                    Id, Province, City
                from
                    Locations
                where 
                    Id = @Id";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Location>(sql, new { Id = user.LocationId });
        }

        public async Task<Location> GetALocation(string city)
        {
            var sql = @"
                select *
                from Locations
                where City = @City
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Location>(sql, new { City = city });
        }

        // //POST 
        public async Task<int> CreateALocation(LocationResource location) {
            var sql = @"
                insert into Locations
                    (City, Province)
                values
                    (@City, @Province);
                select cast(scope_identity() as int);
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            location.LocationID = await connection.QuerySingleAsync<int>(sql, new { City = location.City, Province = location.Province });
            return location.LocationID;
        }

        //DELETE
        public async Task<Location> DeleteALocation(int locationId) {
            var location = await GetALocation(locationId);
            var sql = @"
                delete from Locations
                where Id = @LocationId
            ";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.ExecuteAsync(sql, new { LocationId= locationId });
            return location;
        }

        public async Task<Location> GetLocationIdByCityProvince(LocationResource location) {
            var sql = @"
                Select * from Locations
                where City = @City and Province = @Province;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Location>(sql, new { City = location.City, Province = location.Province });
        }

        public async Task<string> GetAProvince(string province) {
            var sql = @"
                select *
                from Provinces
                where Name = @Province
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<string>(sql, new { Province = province });
        }

        public async Task<string> CreateAProvince(string province) {
            var sql = @"
                Insert into Provinces values (@Province);
            ";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.QueryFirstOrDefaultAsync<string>(sql, new { Province = province });
            return province;
        }

        public async Task<string> DeleteAProvince(string province) {
            var provinceName = await GetAProvince(province);
            var sql = @"
                delete from Provinces
                where Name = @Province
            ";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await connection.ExecuteAsync(sql, new { Province = province });
            return provinceName;
        }
    }
}
