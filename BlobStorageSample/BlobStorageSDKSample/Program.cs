using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        // Set env variable 
        // setx AZURE_STORAGE_CONNECTION_STRING "DefaultEndpointsProtocol=https;AccountName=sampleaccountaz204;AccountKey=rESKM5BX7JoF5X+2LKAHWJ5jH1mhL4ydrBdjWEmBh+9LMt1ynknZlCcK67z5bGImBlKXnHvwYE/++AStLzp4rg==;EndpointSuffix=core.windows.net"

        // Retrieve the connection string for use with the application. 
        string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

        var tableServiceClient = new TableServiceClient(connectionString);
        var blobServiceClient = new BlobServiceClient(connectionString);

        TableClient tableClient = tableServiceClient.GetTableClient("products");
        BlobContainerClient blobContainerClient;

        blobContainerClient = blobServiceClient.GetBlobContainerClient("productimagesfile");
        if (!blobContainerClient.ExistsAsync().GetAwaiter().GetResult())
        {
            blobContainerClient = blobServiceClient.CreateBlobContainerAsync("productimagesfile", publicAccessType: PublicAccessType.Blob).Result;
        }

        var queryResults = tableClient.Query<TableEntity>();

        foreach (TableEntity entity in queryResults)
        {
            var imageURL = entity["Image"].ToString();
            var partitionKey = entity["PartitionKey"].ToString();

            if (string.IsNullOrEmpty(imageURL)) continue;
            if (string.IsNullOrEmpty(partitionKey)) continue;

            var imageExtension = Path.GetExtension(imageURL);
            MemoryStream imageMemoryStream = LoadImage(imageURL).Result;

            if (imageMemoryStream.Length > 0)
            {
                BlobClient blobClient = blobContainerClient.GetBlobClient(partitionKey + imageExtension);

                imageMemoryStream.Position = 0;
                BlobContentInfo blobContentInfo = blobClient.UploadAsync(imageMemoryStream, true).Result;
            }
        }


        foreach (BlobItem blobItem in blobContainerClient.GetBlobs())
        {
            Console.WriteLine("\t" + blobItem.Name);
        }

    }

    public async static Task<MemoryStream> LoadImage(string requestUri)
    {
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            using (HttpClient client = new HttpClient())
            using (var response = await client.GetAsync(requestUri))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                await stream.CopyToAsync(memoryStream);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to load the image: {0}", ex.Message);
        }

        return memoryStream;
    }

}