using System.Threading.Tasks;
using Verifier.Application.Interfaces.Common;
using Verifier.Shared.Models.Request.User;
using Verifier.Shared.Models.ServiceModel;
using Verifier.Shared.Models.ServiceModel.Account;
using Verifier.Shared.Models.ServiceModel.Token;
using Verifier.Shared.Wrappers;
using Verifier.Shared.WrappersCore.Wrappers;

namespace Verifier.Application.Interfaces.Services.Identity
{
    public interface IAccountService
    {
        Task<IResult> UpdateProfileAsync(UpdateProfileInputServiceModel updateProfileInput, string userId);
        Task<IResult> ChangePasswordAsync(ChangePasswordInputServiceModel changePasswordInput, string userId);
        Task<IResult<string>> GetProfilePictureAsync(string userId);
        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureInputServiceModel updateProfilePictureInput, string userId);
        Task<IResult<UserProfileOutputServiceModel>> GetMyUserProfileAsync(string userId);
        Task<IResult<bool>> CheckUsernameAvailability(string username);
        Task<IResult<bool>> CheckEmailAvailability(string email);

        Task<Result<TokenOutputServiceModel>> RegisterExternalUserGetTokenAsync(RegisterInputServiceModel registerInput,
            string userSchemeId, string loginScheme);
        Task<IResult> ThirdPartyRegisterAsync(ThirdPartyRegistrationInputServiceModel thirdPartyRegistrationInput);
        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);
        Task<IResult> ForgotPasswordAsync(ForgotPasswordInputServiceModel request, string origin);
        Task<IResult> ResetPasswordAsync(ResetPasswordInputServiceModel request);
        Task<IResult<UserResponse>> GetAsync(string userId);
        Task<IResult<UserRolesResponse>> GetRolesAsync(string userId);
    }
}