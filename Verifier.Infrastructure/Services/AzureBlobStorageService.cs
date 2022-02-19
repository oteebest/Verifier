using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Verifier.Application.Interfaces.Services;
using Verifier.Shared.Enums;

namespace Verifier.Infrastructure.Services
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly string _connectionString;
        public AzureBlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:StorageConnection"];
        }

        public async Task<bool> DeleteAsync(AzureBlobStorageContainer blobStorageContainer, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            var containerClient = await GetContainerClientAsync(blobStorageContainer);
            var blobClient = containerClient.GetBlobClient(fileName);

            var response = blobClient.DeleteIfExists();

            return response.Value;
        }

        public async Task<string> GetFileUrlAsync(AzureBlobStorageContainer blobStorageContainer, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            var containerClient = await GetContainerClientAsync(blobStorageContainer);
            var blobClient = containerClient.GetBlobClient(fileName);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> StoreAsync(AzureBlobStorageContainer blobStorageContainer, string fileName, Stream content)
        {
            var newFileName = $"{Guid.NewGuid().ToString()}_{fileName}";
            var containerClient = await GetContainerClientAsync(blobStorageContainer);
            var blobClient = containerClient.GetBlobClient(newFileName);

            await blobClient.UploadAsync(content, true);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> StoreAsync(AzureBlobStorageContainer blobStorageContainer, string fileName, byte[] content)
            => await StoreAsync(blobStorageContainer, fileName, new MemoryStream(content));

        private async Task<BlobContainerClient> GetContainerClientAsync(AzureBlobStorageContainer blobStorageContainer)
        {
            if (blobStorageContainer == null)
                throw new ArgumentNullException(nameof(blobStorageContainer));

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(blobStorageContainer.ToString().ToLower());
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            return containerClient;
        }
    }
}
