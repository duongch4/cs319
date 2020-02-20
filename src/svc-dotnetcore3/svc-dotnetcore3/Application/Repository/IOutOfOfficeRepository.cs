using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;

namespace Web.API.Application.Repository
{
    public interface IOutOfOfficeRepository
    {
        //GET
        Task<IEnumerable<OutOfOffice>> GetAllOutOfOfficeForUser(User user);
        Task<OutOfOffice> DeleteOutOfOffice(OutOfOffice avail);
        Task<OutOfOffice> InsertOutOfOffice(OutOfOffice avail);
    }
}
