using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Resources;

namespace Web.API.Application.Repository
{
    public interface ILocationsRepository
    {
        //GET
        Task<IEnumerable<Location>> GetAllLocations();
        Task<IEnumerable<MasterLocation>> GetAllLocationsGroupByProvince();
        // Task<Location> GetALocationWithCity(string city);
        Task<Location> GetALocation(int locationId);
        Task<Location> GetALocation(string city);
        
        Dictionary<string, IEnumerable<string>> GetStaticLocations();

        Task<Location> GetUserLocation(User user);
        //POST 
        Task<int> CreateALocation(LocationResource location);

        // //PUT
        // Task<Location> UpdateALocation(Location location);

        //DELETE
        Task<Location> DeleteALocation(int locationId);

        // Task<Location> GetLocationIdByCityProvince(LocationResource location);

        Task<string> GetAProvince(string province);
        Task<string> CreateAProvince(string province);
        Task<string> DeleteAProvince(string province);
    }
}
