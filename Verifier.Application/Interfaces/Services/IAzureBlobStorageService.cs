using System.IO;
using System.Threading.Tasks;
using Verifier.Shared.Enums;

namespace Verifier.Application.Interfaces.Services
{
    public interface IAzureBlobStorageService
    {
        Task<string> GetFileUrlAsync(AzureBlobStorageContainer blobStorageContainer, string fileName);
        Task<string> StoreAsync(AzureBlobStorageContainer blobStorageContainer, string fileName, Stream content);
        Task<string> StoreAsync(AzureBlobStorageContainer blobStorageContainer, string fileName, byte[] content);
        Task<bool> DeleteAsync(AzureBlobStorageContainer blobStorageContainer, string fileName);
    }
}
