using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Identity;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;

// https://stackoverflow.com/questions/63913934/azure-function-file-upload-and-read-content

namespace Company.Function
{
    public static class HttpTrigger
    {
        [FunctionName("HttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            string a = req.ContentType;
            string content = null;
            log.LogInformation(a);
            if (req.ContentType == "text/plain")
            {
                log.LogInformation("inside");
                content = await new StreamReader(req.Body).ReadToEndAsync();
            }
            await OperateBlobAsync(log,content);

            return new OkObjectResult(content);

        }


        private static async Task OperateBlobAsync(ILogger log,string content)
        {
            //string tempDirectory = null;
            //string destinationPath = null;
            //string sourcePath = null;
            BlobContainerClient blobContainerClient = null;

            // Retrieve the connection string for use with the application. The storage connection string is stored
            // in an environment variable on the machine running the application called AZURE_STORAGE_CONNECTIONSTRING.
            // If the environment variable is created after the application is launched in a console or with Visual
            // Studio, the shell needs to be closed and reloaded to take the environment variable into account.
            string storageConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTIONSTRING");


            if (storageConnectionString == null)
            {
                log.LogWarning("A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'AZURE_STORAGE_CONNECTIONSTRING' with your storage " +
                    "connection string as a value.");

                return;
            }
            try
            {
                // Create a container called 'quickstartblob' and append a GUID value to it to make the name unique. 
                string containerName = "quickstartblob" + Guid.NewGuid().ToString();
                blobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
                await blobContainerClient.CreateAsync();
                log.LogInformation($"Created container '{blobContainerClient.Uri}'");

                // Set the permissions so the blobs are public. 
                //await blobContainerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
                //Console.WriteLine("Setting the Blob access policy to public.");
                //Console.WriteLine();

                // Create a file in a temp directory folder to upload to a blob.
                //tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                //Directory.CreateDirectory(tempDirectory);
                //string blobFileName = $"QuickStart_{Path.GetRandomFileName()}.txt";
                //sourcePath = Path.Combine(tempDirectory, blobFileName);

                // Write text to this file.
                //File.WriteAllText(sourcePath, "Storage Blob Quickstart.");
                //Console.WriteLine($"Created Temp file = {sourcePath}");
                //Console.WriteLine();

                // Get a reference to the blob named "sample-blob", then upload the file to the blob.
                //Console.WriteLine($"Uploading file to Blob storage as blob '{blobFileName}'");
                string blobName = "sample-blob";
                BlobClient blob = blobContainerClient.GetBlobClient(blobName);

                // Open this file and upload it to blob
                //using (FileStream fileStream = File.OpenRead(sourcePath))
                //{
                //await blob.UploadAsync(content);
                //}
                var c = Encoding.UTF8.GetBytes(content);
                using(var ms = new MemoryStream(c))
                {
                    await blob.UploadAsync(ms);
                }
                
                log.LogInformation($"Uploaded successfully.'{blobName}'");

                // List the blobs in the container.
                log.LogInformation("Listing blobs in container.");
                await foreach (BlobItem item in blobContainerClient.GetBlobsAsync())
                {
                    log.LogInformation($"The blob name is '{item.Name}'");
                }

                log.LogInformation("Listed successfully.");

                // Append the string "_DOWNLOADED" before the .txt extension so that you can see both files in the temp directory.
                //destinationPath = sourcePath.Replace(".txt", "_DOWNLOADED.txt");

                // Download the blob to a file in same directory, using the reference created earlier. 
                //Console.WriteLine($"Downloading blob to file in the temp directory {destinationPath}");
                //BlobDownloadInfo blobDownload = await blob.DownloadAsync();

                //using (FileStream fileStream = File.OpenWrite(destinationPath))
                //{
                //    await blobDownload.Content.CopyToAsync(fileStream);
                //}

                //Console.WriteLine("Downloaded successfully.");
                //Console.WriteLine();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error returned from the service: {ex.Message}");
            }

        }  

    }
}