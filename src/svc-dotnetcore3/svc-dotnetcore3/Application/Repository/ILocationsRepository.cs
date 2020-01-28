using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;

namespace Web.API.Application.Repository
{
    public interface ILocationsRepository
    {
        Task<IEnumerable<Location>> GetAllLocations();
        Task<Location> GetALocation(string locationCode);
    }
}
