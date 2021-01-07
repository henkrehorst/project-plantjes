using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace project_c.Services
{
    public class UploadService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _blobContainerName;
        private readonly IConfiguration _configuration;

        public UploadService(IConfiguration configuration)
        {
            this._blobContainerName = "media";
            this._configuration = configuration;
            this._blobServiceClient = new BlobServiceClient(this._configuration.GetConnectionString("AzureBlobStorage"));
        }

        private BlobContainerClient GetBlobContainerClient()
        {
            return this._blobServiceClient.GetBlobContainerClient(this._blobContainerName);
        }

        public async Task<string[]> UploadMultipleImages(IFormFile[] files)
        {
            string[] urls = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                urls[i] = await UploadImage(files[i]);
            }

            return urls;
        }
        
        public async Task<string> UploadImage(IFormFile file)
        {
            BlobClient blobClient = GetBlobContainerClient().GetBlobClient($"{Guid.NewGuid()}.jpg");
            await blobClient.UploadAsync(new MemoryStream(await GetFileBytes(file)));

            BlobHttpHeaders headers = new BlobHttpHeaders()
            {
                ContentType = "image/jpeg",
                CacheControl = "max-age=3600"
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