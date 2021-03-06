﻿using FLive.Media.Common;
using FLive.Web.MediaService.Helpers;
using FLive.Web.MediaService.Models;
using Hangfire;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FLive.Web.MediaService.Controllers
{
    public class UploadController : BaseMediaController
    {
        //
        // GET: /Assets/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateMediaAsset()
        {
            var tmpName = Server.MapPath("~/tmpuploads");
            if (!System.IO.Directory.Exists(tmpName))
            {
                System.IO.Directory.CreateDirectory(tmpName);
            }

            var context = new ApplicationDbContext();
            var mediaAsset = new MediaAsset();
            var dbInProgress = false;
            var workoutId = string.Empty;
            var accessToken = string.Empty;

            
            try
            {
                string pathToTempFile = System.IO.Path.Combine(tmpName, "Unknown");

                if (Request.QueryString["WorkoutId"] == null || Request.QueryString["AccessToken"] == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Unauthorized upload.");
                }

                workoutId = Request.QueryString["WorkoutId"].ToString();
                accessToken = Request.QueryString["AccessToken"].ToString();

                if (string.IsNullOrEmpty(workoutId) || string.IsNullOrEmpty(accessToken))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Unauthorized upload.");
                }

                var accessDt = Helpers.SqlHelper.ExecuteStatement("select [ExternalAccessToken] from [LiveWorkouts] where [Id] = " + workoutId);
                if (accessDt == null || accessDt.Rows.Count == 0 || !accessDt.Rows[0][0].ToString().Equals(accessToken))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Unauthorized upload.");
                }

                if (string.IsNullOrEmpty(workoutId))
                {
                    return Json(new { success = false, message = "Workout Id is required." });
                }

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files.Get(0);
                    var tempfileName = Guid.NewGuid() + file.FileName;
                    pathToTempFile = System.IO.Path.Combine(tmpName, tempfileName);

                    #region --- Creating Media Asset ---

                    mediaAsset = new MediaAsset()
                    {
                        Mesage = "Uploading to web server started.",
                        Status = "Uploading To Web Server",
                        DateUploadStarted = DateTime.Now,
                        OriginalFileName = pathToTempFile,
                        FliveWorkoutId = workoutId
                    };
                    context.MediaAssets.Add(mediaAsset);
                    context.SaveChanges();

                    #endregion

                    file.SaveAs(pathToTempFile);
                }
                else
                {
                    var tempfileName = Guid.NewGuid() + Request.Params["qqfile"];
                    pathToTempFile = System.IO.Path.Combine(tmpName, tempfileName);

                    #region --- Creating Media Asset ---

                    mediaAsset = new MediaAsset()
                    {
                        Mesage = "Uploading to web server started.",
                        Status = "Uploading To Web Server",
                        DateUploadStarted = DateTime.Now,
                        OriginalFileName = pathToTempFile,
                        FliveWorkoutId = workoutId
                    };
                    context.MediaAssets.Add(mediaAsset);
                    context.SaveChanges();

                    #endregion

                    using (var fs = System.IO.File.Create(pathToTempFile))
                    {
                        Request.InputStream.CopyTo(fs);
                        fs.Flush();
                    }
                }

                #region --- Updating Media Asset ---

                mediaAsset.Mesage = "Uploading to web server completed.";
                mediaAsset.Status = "Uploading To Media Service";
                mediaAsset.IsUploadCompleted = true;
                mediaAsset.DateUploadCompleted = DateTime.Now;
                mediaAsset.DateSyncToServiceStarted = DateTime.Now;
                context.SaveChanges();

                #endregion

                BackgroundJob.Enqueue(() => MediaServiceJobHelper.CreateAsset(mediaAsset.Id));
            }
            catch (Exception ex)
            {
                mediaAsset.Status = "Error";
                mediaAsset.Mesage = GetInnerException(ex);
                context.SaveChanges();
            }

            return Json(new { success = true, id = mediaAsset.Id });
        }

        private string GetInnerException(Exception e)
        {
            while (e.InnerException != null) e = e.InnerException;
            return e.Message;
        }

        public ActionResult CreateEmptyAsset(string name)
        {
            this.AssetService.CreateEmptyAsset(name);
            return RedirectToAction("index");
        }

        public ActionResult EncodeAndConvert(string assetId)
        {
            var assetToEncode = this.AssetService.GetAssetById(assetId);
            this.JobService.CreateEncodeToSmoothStreamingJob(assetToEncode);
            return RedirectToAction("Index", "Jobs");
        }

        public ActionResult Details(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        public ActionResult GetStreamingUrl(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            string streamingUrl = this.LocatorService.GetSmoothStreamingOriginLocator(asset);
            if (string.IsNullOrWhiteSpace(streamingUrl))
            {
                return HttpNotFound();
            }
            return View(new StreamingUrlViewModel() { Url = streamingUrl, IsMp4Progressive = false });

        }

        public ActionResult GetHlsStreamingUrl(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            string streamingUrl = this.LocatorService.GetHLSOriginLocator(asset);
            if (string.IsNullOrWhiteSpace(streamingUrl))
            {
                return HttpNotFound();
            }
            return View("GetStreamingUrl", new StreamingUrlViewModel() { Url = streamingUrl, IsMp4Progressive = false });
        }

        public ActionResult GetCDNStreamingUrl(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            string streamingUrl = this.LocatorService.GetSmoothStreamingAzureCDNLocator(asset);
            if (string.IsNullOrWhiteSpace(streamingUrl))
            {
                return HttpNotFound();
            }
            return View("GetStreamingUrl", new StreamingUrlViewModel() { Url = streamingUrl });

        }

        public ActionResult GetMp4StreamingUrl(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            string streamingUrl = this.LocatorService.GetMp4StreamingOriginLocator(asset);
            StreamingUrlViewModel model = new StreamingUrlViewModel();
            model.Url = streamingUrl;
            model.IsMp4Progressive = true;
            return View("GetStreamingUrl", model);
        }


        public ActionResult DecryptAsset(string assetId)
        {
            this.JobService.DecryptAsset(this.AssetService.GetAssetById(assetId));
            return RedirectToAction("Index", "Jobs");
        }

        public ActionResult PublishAsset(string assetId)
        {
            this.AssetService.Publish(assetId);
            return RedirectToAction("Index");
        }

        public ActionResult ConvertToMp4(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            this.JobService.CreateNewJob(asset, MediaEncoders.WINDOWS_AZURE_MEDIA_ENCODER, Tasks.H264_HD_720P_CBR);
            return RedirectToAction("Index", "Jobs");
        }

        public ActionResult CreateThumbnails(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            this.JobService.CreateThumbnails(asset);
            //this.JobService.CreateNewJob(asset, MediaEncoders.WINDOWS_AZURE_MEDIA_ENCODER, Tasks.H264_512k_DSL_CBR);
            return RedirectToAction("Index", "Jobs");
        }

        public ActionResult ConvertToPlayReady(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            this.JobService.CreateNewJob(asset, MediaEncoders.WINDOWS_AZURE_MEDIA_ENCRYPTOR, this.JobService.GetPlayReadyTask(keySeed: PlayReady.DEV_SERVER_KEY_SEED, playReadyServerUrl: PlayReady.DEV_SERVER_LICENSE_URL));
            return RedirectToAction("Index", "Jobs");
        }

        public ActionResult ConvertToHls(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            this.JobService.CreateNewJob(asset, MediaEncoders.WINDOWS_AZURE_MEDIA_PACKAGER, this.JobService.GetSmoothToHlsTask());
            return RedirectToAction("Index", "Jobs");
        }

        public ActionResult ConvertToMultiBitrateMp4(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            this.JobService.CreateNewJob(asset, MediaEncoders.WINDOWS_AZURE_MEDIA_ENCODER, Tasks.H264_ADAPTIVE_BITRATE_SD_16x9);
            return RedirectToAction("Index", "Jobs");
        }

        public ActionResult Mp4ToSmooth(string assetId)
        {
            var asset = this.AssetService.GetAssetById(assetId);
            this.JobService.CreateNewJob(asset, MediaEncoders.WINDOWS_AZURE_MEDIA_ENCODER, Tasks.H264_Smooth_720p_3G_4G);
            //this.JobService.CreateNewJob(asset, MediaEncoders.WINDOWS_AZURE_MEDIA_PACKAGER, this.JobService.GetMp4ToSmoothTask());
            return RedirectToAction("Index", "Jobs");
        }

        public ActionResult DeleteAsset(string assetId)
        {
            this.AssetService.DeleteAsset(assetId);
            return RedirectToAction("Index");
        }

        public ActionResult Rename(string assetId, string newName)
        {
            this.AssetService.Rename(assetId, newName);
            return RedirectToAction("Details", new { assetId = assetId });
        }

        public ActionResult GetSasUrl(string assetId)
        {
            var sasLoc = this.LocatorService.GetSasLocator(this.AssetService.GetAssetById(assetId));
            return View((object)sasLoc);
        }

        public ActionResult CopyFromBlob(string assetId, string srcBlob)
        {
            var src = this.LocatorService.GetSasLocator(this.AssetService.GetAssetById(assetId));
            this.AssetService.CopyFromBlob(src, srcBlob);
            return RedirectToAction("Details", new { assetId = assetId });
        }
    }
}