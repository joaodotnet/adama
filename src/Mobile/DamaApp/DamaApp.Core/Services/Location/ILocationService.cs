using System.Threading.Tasks;

namespace DamaApp.Core.Services.Location
{    
    public interface ILocationService
    {
        Task UpdateUserLocation(DamaApp.Core.Models.Location.Location newLocReq, string token);
    }
}