using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;

namespace Web.API.Application.Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetAUser(string username);
    }
}
