using Web.API.Application.Models;
using Web.API.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.API.Application.Repository
{
    public interface IPositionsRepository
    {
        // GET
        Task<IEnumerable<Position>> GetAllPositions();
        Task<Position> GetAPosition(int positionId);
        Task<OpeningPositionsResource> GetAnOpeningPositionsResource(int positionId);
        Task<IEnumerable<PositionResource>> GetPositionsOfUser(string userId);
        Task<IEnumerable<Position>> GetAllPositionsOfUser(string userId);
        Task<IEnumerable<Position>> GetAllUnassignedPositionsOfProject(Project project);
        Task<IEnumerable<OpeningPositionsResource>> GetAllUnassignedPositionsResourceOfProject(int projectId);

        // POST
        Task<Position> CreateAPosition(Position position);

        // PUT
        Task<Position> UpdateAPosition(Position position);

        // DELETE
        Task<Position> DeleteAPosition(int positionId);
    }
}
