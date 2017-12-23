using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using FLive.Infrastructure.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using Serilog;
using Stripe;
using Microsoft.CSharp;

namespace FLive.Infrastructure
{
    public class DownloadService
    {
        private readonly string _connection;

        public DownloadService(string connection)
        {
            _connection = connection;

            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["Stripe.SecretKey"]);
        }

        public async Task DownloadRecordings()
        {
            using (var connection = new SqlConnection(_connection))
            {
                var items =
                    connection.Query<LiveWorkout>(
                        $"select * from  LiveWorkouts WHERE WorkoutStatus = 4");

                foreach (var item in items)
                {
                    await GetDownloadUrlAndAssignOnLiveWorkout(item.StreamId, item.Id, connection);
                }
            }
        }

        private async Task GetDownloadUrlAndAssignOnLiveWorkout(string streamId, long workoutId,
            SqlConnection connection)
        {
            try
            {
                Console.WriteLine($"Starting to download recording {streamId} for workout{workoutId}");

                var httpClient = GetHttpClient();

                var result = await httpClient.GetAsync($"transcoders/{streamId}/recordings");

                var jsonResult = await result.Content.ReadAsStringAsync();
                dynamic d = JObject.Parse(jsonResult);

                //var fileRepo = new FileUploadRepository();
                //var fileName = fileRepo.UploadFileAsBlob(stream, $"{streamId}.mp4", "recordings");

                var downloadUrl = d.recordings[0].download_url.Value;
               
                if (!string.IsNullOrEmpty(downloadUrl))
                {
                    connection.Execute(
                        $"update LiveWorkouts set WorkoutStatus=5,RecordingUrl='{downloadUrl}'  where Id={workoutId}");

                    Console.WriteLine(
                        $"Completed downloading recording {streamId} for workout{workoutId} and stored as {downloadUrl}");
                }
            }
            catch (Exception exception)
            {
                Log.Logger.Error("Failed to download the URLs for stream : {streamId} , workout :  {workoutId} : exception {exception}", streamId, workoutId, exception);
                Console.WriteLine($"Failed to download recordings for {streamId}. Error {exception.Message} occured");
            }
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("wsc-api-key", ConfigurationManager.AppSettings["Wowza.ApiKey"]);
            httpClient.DefaultRequestHeaders.Add("wsc-access-key", ConfigurationManager.AppSettings["Wowza.AccessKey"]);
            httpClient.BaseAddress = new Uri($"https://{ConfigurationManager.AppSettings["Wowza.Endpoint"]}/api/v1/");
            return httpClient;
        }
    }

    //TODO : this is not being used at the moment. But might want in the future if getting the download url from AWS is not working
    public class FileUploadRepository
    {
        public async Task<string> UploadFileAsBlob(Stream stream, string filename, string containerName)
        {
            var storageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ToString());

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(containerName);

            var containerExists = await container.ExistsAsync();
            if (!containerExists)
            {
                await
                    container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, new BlobRequestOptions(),
                        new OperationContext());
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