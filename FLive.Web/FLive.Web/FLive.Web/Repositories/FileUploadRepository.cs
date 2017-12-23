using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FLive.Web.Repositories
{
    public class FileUploadRepository : IFileUploadRepository
    {
        private readonly IConfiguration _configuration;

        public FileUploadRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsBlob(Stream stream, string filename, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("StorageConnectionString"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(containerName);

            var containerExists = await container.ExistsAsync();
            if (!containerExists)
            {
                await
                    container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, new BlobRequestOptions(),
                        new OperationContext());

                if (containerName == "profileimages")
                {
                    var container50x50 = blobClient.GetContainerReference($"profileimages-50x50");
                    await
                        container50x50.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container,
                            new BlobRequestOptions(),
                            new OperationContext());
                    var container40x40 = blobClient.GetContainerReference($"profileimages-40x40");
                    await
                        container40x40.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container,
                            new BlobRequestOptions(),
                            new OperationContext());
                }
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            var extension = Path.GetExtension(filename);
            var uniqueFileName = $"{fileNameWithoutExtension.Replace(" ", "_")}_{Guid.NewGuid()}{extension}";

            var blockBlob = container.GetBlockBlobReference(uniqueFileName);

            await blockBlob.UploadFromStreamAsync(stream);

            stream.Dispose();
            return blockBlob?.Uri.ToString();
        }
    }
}