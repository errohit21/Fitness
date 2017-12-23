using FLive.Web.MediaService.Models;
using FLive.Web.MediaService.Models.flive;
using Hangfire;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;

namespace FLive.Web.MediaService.Helpers
{
    public static class MediaServiceJobHelper
    {
        public static void CreateEncodingJob(int mediaAssetId, string assetId)
        {
            var mediaContext = new CloudMediaContext(ConfigurationManager.AppSettings["MediaServerAccountName"],
                  ConfigurationManager.AppSettings["MediaServerAccountKey"]);
            var context = new ApplicationDbContext();


            var mediaAsset = context.MediaAssets.Single(a => a.Id == mediaAssetId);
            var asset = mediaContext.Assets.Where(a => a.Id == assetId).First();

            var job = mediaContext.Jobs.CreateWithSingleTask(MediaProcessorNames.MediaEncoderStandard,
                MediaEncoderStandardTaskPresetStrings.H264SingleBitrate1080p, asset, string.Format("{0}_encoded", asset.Name), AssetCreationOptions.None);

            Console.WriteLine("Submitting the job to media server");

            #region --- Updating Media Asset ---

            mediaAsset.Mesage = "Submitting Encoding job for asset.";
            mediaAsset.Status = "Scheduling Encoding Job";
            mediaAsset.CurrentProgress = 0;
            mediaAsset.DateEncodingStarted = DateTime.Now;
            context.SaveChanges();

            #endregion

            job.Submit();
            job = job.StartExecutionProgressTask(j =>
            {
                Console.WriteLine("Job state : {0}", j.State);
                Console.WriteLine("Job Progress : {0:0.##}%", j.GetOverallProgress());

                #region --- Updating Media Asset ---

                if (context.Database.Connection.State == System.Data.ConnectionState.Closed)
                {
                    mediaAsset.Mesage = "Encoding media asset, Current progress : " + j.GetOverallProgress();
                    mediaAsset.Status = "Encoding";
                    mediaAsset.CurrentProgress = j.GetOverallProgress();
                    context.SaveChanges();
                }

                #endregion

            }, CancellationToken.None).Result;

            Console.WriteLine("Finished the encoding job");
            var outputAsset = job.OutputMediaAssets[0];

            #region --- Updating Media Asset ---

            mediaAsset.Mesage = "Encoding media asset completed. Starting to publish";
            mediaAsset.Status = "Publishing";
            mediaAsset.CurrentProgress = 0;
            mediaAsset.IsEncodingCompleted = true;
            mediaAsset.DateEncodingCompleted = DateTime.Now;
            mediaAsset.MediaServiceEncodedAssetID = outputAsset.Id;
            context.SaveChanges();

            #endregion


            mediaContext.Locators.Create(LocatorType.OnDemandOrigin, outputAsset, AccessPermissions.Read, TimeSpan.FromDays(365));

            Console.WriteLine("Smooth Streaming URI : {0}", outputAsset.GetSmoothStreamingUri());
            Console.WriteLine("HLS URI : {0}", outputAsset.GetHlsUri());
            Console.WriteLine("Smooth DASH URI : {0}", outputAsset.GetMpegDashUri());

            #region --- Updating Media Asset ---

            mediaAsset.Mesage = "Process completed";
            mediaAsset.Status = "Completed";
            mediaAsset.CurrentProgress = 0;

            if (outputAsset != null && outputAsset.GetSmoothStreamingUri() != null)
            {
                mediaAsset.StreamingUrl = outputAsset.GetSmoothStreamingUri().AbsoluteUri;
                mediaAsset.HSLUrl = outputAsset.GetHlsUri().AbsoluteUri;
                mediaAsset.DashUrl = outputAsset.GetMpegDashUri().AbsoluteUri;
                mediaAsset.DatePublished = DateTime.Now;
                mediaAsset.DateExpire = DateTime.Now.AddDays(365);

                LiveWorkout.UpdateWorkoutStreamingURL(mediaAsset.FliveWorkoutId, mediaAsset.DashUrl);
            }

            context.SaveChanges();

            #endregion

        }

        public static void CreateAsset(int assetId)
        {
            var context = new ApplicationDbContext();
            var mediaContext = new CloudMediaContext(ConfigurationManager.AppSettings["MediaServerAccountName"],
                  ConfigurationManager.AppSettings["MediaServerAccountKey"]);
            var mediaAsset = context.MediaAssets.Single(a => a.Id == assetId);

            var asset = mediaContext.Assets.CreateFromFile(mediaAsset.OriginalFileName, AssetCreationOptions.None,
                     (af, p) =>
                     {
                         if (context.Database.Connection.State == System.Data.ConnectionState.Closed)
                         {
                             mediaAsset.CurrentProgress = p.Progress;
                             mediaAsset.Mesage = string.Format("Uploading to media service, Current progress : {0}", p.Progress);

                             context.SaveChanges();
                         }
                     });

            #region --- Updating Media Asset ---

            mediaAsset.Mesage = "Uploading to media service completed.";
            mediaAsset.Status = "Scheduling Encoding Job";
            mediaAsset.CurrentProgress = 0;
            mediaAsset.IsSyncCompleted = true;
            mediaAsset.DateSyncToServiceCompleted = DateTime.Now;
            mediaAsset.MediaServiceOriginalAssetID = asset.Id;
            context.SaveChanges();

            #endregion

            System.IO.File.Delete(mediaAsset.OriginalFileName);
            BackgroundJob.Enqueue(() => MediaServiceJobHelper.CreateEncodingJob(mediaAsset.Id, asset.Id));
        }
    }
}