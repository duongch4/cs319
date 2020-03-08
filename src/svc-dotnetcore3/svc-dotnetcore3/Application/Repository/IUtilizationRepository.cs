using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;

namespace Web.API.Application.Repository
{
    //TODO
    public interface IUtilizationRepository
    {
        //GET
        Task<IEnumerable<rawUtilization>> GetUtilizationOfUser(int userId);
    }
}
