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
        // Task<Location> GetALocationWithCity(string city);
        Task<Location> GetALocation(int locationId);
        Task<Location> GetALocation(string city);
        
        Dictionary<string, IEnumerable<string>> GetStaticLocations();

        Task<Location> GetUserLocation(User user);
        //POST 
        // Task<Location> CreateALocation(Location location);

        // //PUT
        // Task<Location> UpdateALocation(Location location);

        //DELETE
        Task<Location> DeleteALocation(Location locationCode);

        Task<Location> GetLocationIdByCityProvince(LocationResource location);
    }
}
