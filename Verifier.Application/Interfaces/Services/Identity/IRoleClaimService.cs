using Verifier.Application.Interfaces.Common;
using Verifier.Shared.Models.Response.Identity;
using Verifier.Shared.WrappersCore.Wrappers;

namespace Application.Interfaces.Services.Identity
{
    public interface IRoleClaimService
    {
        Task<Result<List<RoleClaimInputServiceModel>>> GetAllAsync();
        Task<int> GetCountAsync();
        Task<Result<RoleClaimInputServiceModel>> GetByIdAsync(int id);
        Task<Result<List<RoleClaimInputServiceModel>>> GetAllByRoleIdAsync(string roleId);
        Task<Result<string>> SaveAsync(RoleClaimInputServiceModel roleClaimInput);
        Task<Result<string>> DeleteAsync(int id);
    }
}