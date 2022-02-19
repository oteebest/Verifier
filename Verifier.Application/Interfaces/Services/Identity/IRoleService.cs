using Core.Models.Response.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Verifier.Application.Interfaces.Common;
using Verifier.Shared.Models.Response.Identity;
using Verifier.Shared.Models.ServiceModel.Roles;
using Verifier.Shared.WrappersCore.Wrappers;

namespace Application.Interfaces.Services.Identity
{
    public interface IRoleService
    {
        Task<Result<List<RoleOutputServiceModel>>> GetAllAsync();
        Task<int> GetCountAsync();
        Task<Result<RoleOutputServiceModel>> GetByIdAsync(string id);
        Task<Result<string>> SaveAsync(RoleInputServiceModel request);
        Task<Result<string>> DeleteAsync(string id);
        Task<Result<PermissionOutputServiceModel>> GetAllPermissionsAsync(string roleId);
        Task<Result<string>> UpdatePermissionsAsync(PermissionInputServiceModel permissioInput);
    }
}