using Web.API.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Resources;

namespace Web.API.Application.Repository
{
    public interface IPositionsRepository
    {
        // GET
        Task<IEnumerable<Position>> GetAllPositions();
        Task<Position> GetAPosition(int positionId);
        Task<IEnumerable<PositionResource>> GetPositionsOfUser(User user);
        Task<IEnumerable<Position>> GetAllUnassignedPositionOfProject(Project project);

        // POST
        Task<Position> CreateAPosition(Position position);

        // PUT
        Task<Position> UpdateAPosition(Position position);

        // DELETE
        Task<Position> DeleteAPosition(int positionId);
    }
}
