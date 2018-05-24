using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.Location
{    
    public interface ILocationService
    {
        Task UpdateUserLocation(DamaNoJornal.Core.Models.Location.Location newLocReq, string token);
    }
}