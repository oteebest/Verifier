using Core.Models.Response.Identity;
using System.Threading.Tasks;
using Core.Models.Request.User;
using Verifier.Shared.Models.ServiceModel.Token;
using Verifier.Shared.WrappersCore.Wrappers;

namespace Application.Interfaces.Services.Identity
{
    public interface ITokenService 
    {
        Task<Result<TokenOutputServiceModel>> LoginAsync(TokenInputServiceModel tokenInput, bool isThirdParty = false);

        Task<Result<TokenOutputServiceModel>> GetRefreshTokenAsync(RefreshTokenServiceModel refreshTokenInput);
        Task<Result<GetUserClaimsResponse>> GetUserClaimsAsync(string userId);
    }
}
