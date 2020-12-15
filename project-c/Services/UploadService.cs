using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace project_c.Services
{
    public class UploadService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _blobContainerName;

        public UploadService(string container = "media")
        {
            this._blobContainerName = container;
            this._blobServiceClient = new BlobServiceClient(
                "DefaultEndpointsProtocol=https;AccountName=mediablob12;AccountKey=QeNYAzk+fVmlhpuRBgTCSBn/UWWrWdluox3gFU2LljFHD53qhkz0SoQHMELTlLt6f7iE7oyR9Qr8ld8dgOWypA==;EndpointSuffix=core.windows.net");
        }

        private BlobContainerClient GetBlobContainerClient()
        {
            return this._blobServiceClient.GetBlobContainerClient(this._blobContainerName);
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            BlobClient blobClient = GetBlobContainerClient().GetBlobClient($"{Guid.NewGuid()}.jpg");
            await blobClient.UploadAsync(new MemoryStream(await GetFileBytes(file)));

            BlobHttpHeaders headers = new BlobHttpHeaders()
            {
                ContentType = "image/jpeg"
            };

            await blobClient.SetHttpHeadersAsync(headers);
            

            return blobClient.Uri.ToString();
        }

        private async Task<byte[]> GetFileBytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}