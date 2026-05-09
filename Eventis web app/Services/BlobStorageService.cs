using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Eventis_web_app.Services;

public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadImageAsync(string containerName, IFormFile file)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var extension = Path.GetExtension(file.FileName);
        var blobName = $"{Guid.NewGuid()}{extension}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var blobHttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType };

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

        return blobClient.Uri.ToString();
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return;

        var uri = new Uri(imageUrl);
        // Path format: /container-name/blob-name
        var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
        if (segments.Length < 2)
            return;

        var containerName = segments[0];
        var blobName = segments[1];

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
}
