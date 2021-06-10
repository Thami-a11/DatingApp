using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IVisitsRepository
    {
        Task<UserVisits> GetUserVisit(int sourceUserId, int visitsUserId);

         Task<AppUser> GetUserWithVisits(int userId);

         Task<PagedList<VisitsDto>> GetUserVisits(VisitsParams visitsParams);
    }
}