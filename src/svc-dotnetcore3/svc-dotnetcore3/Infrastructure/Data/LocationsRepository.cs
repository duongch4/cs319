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
        private readonly Dictionary<string, IEnumerable<string>> locations;

        public LocationsRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
            this.locations = new Dictionary<string, IEnumerable<string>>();
            this.SetStaticLocations();
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
                    Province, STRING_AGG(CONVERT(nvarchar(max),CONCAT(City, '-', Id)), ',') as CitiesIds
                FROM
                    Locations
                GROUP BY
                    Province
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
        
        private void SetStaticLocations()
        {
            var jsonStr = System.IO.File.ReadAllText(@"Infrastructure/Data/Locations.json");
            dynamic jsonArr = JsonConvert.DeserializeObject(jsonStr);
            foreach (var json in jsonArr)
            {
                string province = (string)json["admin_name"];
                string country = (string)json["country"];
                string city = (string)json["city"];
                if (country.Equals("Canada") && province != null && !this.locations.ContainsKey(province) && city != null)
                {
                    this.locations.Add(province, new List<string>());
                }
                if (this.locations.ContainsKey(province))
                {
                    this.locations[province] = this.locations[province].Append(city);
                }
            }
        }

        public Dictionary<string, IEnumerable<string>> GetStaticLocations()
        {
            return this.locations;
        }
        // //POST 
        // public async Task<Location> CreateALocation(Location location) {
        //     return null;
        // }

        //DELETE
        // public async Task<Location> DeleteALocation(Location locationCode) {
        //     return null;
        // }

        public async Task<Location> GetLocationIdByCityProvince(LocationResource location) {
            var sql = @"
                Select * from Locations
                where City = @City and Province = @Province;";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Location>(sql, new { City = location.City, Province = location.Province });
        }
        // //PUT
        // public async Task<Location> UpdateALocation(Location location) {
        //     return null;
        // }

        // //DELETE
        // public async Task<Location> DeleteALocation(Location locationCode) {
        //     return null;
        // }
    }
}
