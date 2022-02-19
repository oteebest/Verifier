using System.Threading.Tasks;
using Verifier.Shared.Enums;

namespace Verifier.Application.Interfaces.Services
{
    public interface IRedisService
    {
        Task SaveAsync(string key, string data, int durationInMins, RedisCacheOptions? option);
        Task<string> GetAsync(string key);
    }
}
