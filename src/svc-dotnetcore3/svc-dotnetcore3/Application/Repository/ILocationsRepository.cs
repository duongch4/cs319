using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;

namespace Web.API.Application.Repository
{
    public interface ILocationsRepository
    {
        //GET
        Task<IEnumerable<Location>> GetAllLocations();
        Task<Location> GetALocation(string locationCode);

        //POST 
        Task<Location> CreateALocation(Location location);

        //PUT
        Task<Location> UpdateALocation(Location location);

        //DELETE
        Task<Location> DeleteALocation(Location locationCode);
    }
}
