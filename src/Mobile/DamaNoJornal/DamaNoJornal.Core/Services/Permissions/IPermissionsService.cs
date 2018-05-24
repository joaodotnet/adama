using System.Collections.Generic;
using System.Threading.Tasks;
using DamaNoJornal.Core.Models.Permissions;

namespace DamaNoJornal.Core.Services.Permissions
{
    public interface IPermissionsService
    {
        Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission);
        Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(params Permission[] permissions);
    }
}
