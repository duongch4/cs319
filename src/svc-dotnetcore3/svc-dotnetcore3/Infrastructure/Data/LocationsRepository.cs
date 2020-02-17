using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Application.Repository;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Serilog;

namespace Web.API.Infrastructure.Data
{
    public class LocationsRepository : ILocationsRepository
    {
        private readonly string connectionString = string.Empty;
        private readonly Dictionary<string, List<string>> locations;

        public LocationsRepository(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
            this.locations = new Dictionary<string, List<string>>();
            this.SetStaticLocations();
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

        public async Task<Location> GetALocation(int locationId)
        {
            var sql = @"
                select *
                from Locations
                where Id = @Id
            ;";

            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Location>(sql, new { Id = locationId });
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
                // Console.WriteLine(province.Equals("New York"));
                if (country.Equals("Canada") && province != null && !this.locations.ContainsKey(province) && city != null)
                {
                    this.locations.Add(province, new List<string>());
                }
                if (this.locations.ContainsKey(province))
                {
                    this.locations[province].Add(city);
                }
            }
        }

        public Dictionary<string, List<string>> GetStaticLocations()
        {
            return this.locations;
        }
        // //POST 
        // public async Task<Location> CreateALocation(Location location) {
        //     return null;
        // }

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
