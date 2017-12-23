﻿using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Linq;
using FLive.Media.Common.Contracts;

namespace FLive.Media.Common.Implementations
{
    public class LocatorService : ILocatorService
    {
        private IMediaService _mediaService;
        public LocatorService(IMediaService mediaService)
        {
            this._mediaService = mediaService;
        }

        public IMediaService MediaService
        {
            get { return this._mediaService; }
        }

        public string GetSmoothStreamingOriginLocator(Models.Asset assetToStream)
        {
            // Get a reference to the manifest file from the collection 
            // of streaming files in the asset. 
            var manifestFile = assetToStream.MediaAsset.AssetFiles.Where(x => x.Name.EndsWith(".ism")).FirstOrDefault();
            // Cast the reference to a true IFileInfo type. 
            if (null == manifestFile)
            {
                return null;
            }

            // Create an 1-day readonly access policy. 
            IAccessPolicy streamingPolicy = this.MediaService.MediaContext.AccessPolicies.Create("Streaming policy",
                TimeSpan.FromDays(1),
                AccessPermissions.Read);

            // Create the origin locator. Set the start time as 5 minutes 
            // before the present so that the locator can be accessed immediately 
            // if there is clock skew between the client and server.
            ILocator originLocator =
                (from l in this.MediaService.MediaContext.Locators
                 where l.AssetId.Equals(assetToStream.MediaAsset.Id)
                      && l.Type == LocatorType.OnDemandOrigin
                 select l).FirstOrDefault();

            if (originLocator != null && originLocator.ExpirationDateTime <= DateTime.UtcNow)
            {
                originLocator.Delete();
                originLocator = null;
            }

            if (originLocator == null)
            {
                originLocator = this.MediaService.MediaContext
                    .Locators.CreateLocator(LocatorType.OnDemandOrigin, assetToStream.MediaAsset,
                 streamingPolicy,
                 DateTime.UtcNow.AddMinutes(-5));
            }
            // Create a full URL to the manifest file. Use this for playback
            // in streaming media clients. 
            string urlForClientStreaming = originLocator.Path + manifestFile.Name + "/manifest";

            // Display the full URL to the streaming manifest file.
            Console.WriteLine("URL to manifest for client streaming: ");
            Console.WriteLine(urlForClientStreaming);

            return urlForClientStreaming;
        }

        public string GetSmoothStreamingAzureCDNLocator(Models.Asset assetToStream)
        {
            // Get a reference to the manifest file from the collection 
            // of streaming files in the asset. 
            var manifestFile = assetToStream.MediaAsset.AssetFiles.Where(x => x.Name.EndsWith(".ism")).FirstOrDefault();
            // Cast the reference to a true IFileInfo type. 
            if (null == manifestFile)
            {
                return null;
            }

            // Create an 1-day readonly access policy. 
            IAccessPolicy streamingPolicy = this.MediaService.MediaContext.AccessPolicies.Create("CDN Streaming policy",
                TimeSpan.FromHours(1),
                AccessPermissions.Read);

            // Create the origin locator. Set the start time as 5 minutes 
            // before the present so that the locator can be accessed immediately 
            // if there is clock skew between the client and server.
            ILocator originLocator =
                (from l in this.MediaService.MediaContext.Locators
                 where l.AssetId.Equals(assetToStream.MediaAsset.Id) && l.Type == LocatorType.None
                 select l).FirstOrDefault();

            if (originLocator != null && originLocator.ExpirationDateTime <= DateTime.UtcNow)
            {
                originLocator.Delete();
                originLocator = null;
            }

            if (originLocator == null)
            {
                originLocator = this.MediaService.MediaContext
                    .Locators.CreateLocator(LocatorType.None,
                        assetToStream.MediaAsset,
                        streamingPolicy,
                 DateTime.UtcNow.AddMinutes(-5));
            }
            // Create a full URL to the manifest file. Use this for playback
            // in streaming media clients. 
            string urlForClientStreaming = originLocator.Path + manifestFile.Name + "/manifest";

            // Display the full URL to the streaming manifest file.
            Console.WriteLine("URL to manifest for client streaming: ");
            Console.WriteLine(urlForClientStreaming);

            return urlForClientStreaming;
        }

        public string GetMp4StreamingOriginLocator(Models.Asset assetToStream)
        {
            // Get a reference to the manifest file from the collection 
            // of streaming files in the asset. 
            var theManifest =
                                from f in assetToStream.MediaAsset.AssetFiles
                                where f.Name.EndsWith(".mp4")
                                select f;
            // Cast the reference to a true IFileInfo type. 
            IAssetFile manifestFile = theManifest.First();

            // Create an 1-day readonly access policy. 
            IAccessPolicy streamingPolicy = this.MediaService.MediaContext.AccessPolicies.Create("Readonly 1 hour policy",
                TimeSpan.FromHours(1),
                AccessPermissions.Read);

            // Create the origin locator. Set the start time as 5 minutes 
            // before the present so that the locator can be accessed immediately 
            // if there is clock skew between the client and server.
            ILocator originLocator =
                (from l in this.MediaService.MediaContext.Locators
                 where l.AssetId.Equals(assetToStream.MediaAsset.Id)
                 && l.Type == LocatorType.Sas
                 select l).FirstOrDefault();

            if (originLocator != null && originLocator.ExpirationDateTime <= DateTime.UtcNow)
            {
                originLocator.Delete();
                originLocator = null;
            }

            if (originLocator == null)
            {
                originLocator = this.MediaService.MediaContext
                    .Locators.CreateSasLocator(assetToStream.MediaAsset,
                 streamingPolicy,
                 DateTime.UtcNow.AddMinutes(-5));
            }
            // Create a full URL to the manifest file. Use this for playback
            // in streaming media clients. 
            UriBuilder bldr = new UriBuilder(originLocator.Path);
            bldr.Path += "/" + manifestFile.Name;
            string urlForClientStreaming = bldr.ToString();

            // Display the full URL to the streaming manifest file.
            Console.WriteLine("URL to for progressive download: ");
            Console.WriteLine(urlForClientStreaming);

            return urlForClientStreaming;
        }


        public string GetSasLocator(Models.Asset asset)
        {
            // Create an 1-day readonly access policy. 
            IAccessPolicy streamingPolicy = this.MediaService.MediaContext.AccessPolicies.Create("Full Access Policy",
                TimeSpan.FromMinutes(20),
                AccessPermissions.List | AccessPermissions.Read | AccessPermissions.Read);

            // Create the origin locator. Set the start time as 5 minutes 
            // before the present so that the locator can be accessed immediately 
            // if there is clock skew between the client and server.
            ILocator sasLocator =
                (from l in this.MediaService.MediaContext.Locators
                 where l.Type == LocatorType.Sas && l.AssetId.Equals(asset.MediaAsset.Id)
                 select l).FirstOrDefault();

            if (sasLocator != null && sasLocator.ExpirationDateTime < DateTime.UtcNow)
            {
                sasLocator.Delete();
                sasLocator = null;
            }

            if (sasLocator == null)
            {
                sasLocator = this.MediaService.MediaContext
                    .Locators.CreateSasLocator(asset.MediaAsset,
                 streamingPolicy,
                 DateTime.UtcNow.AddMinutes(-5));
            }
            // Create a full URL to the manifest file. Use this for playback
            // in streaming media clients. 
            string sasUrl = sasLocator.Path;

            // Display the full URL to the streaming manifest file.
            Console.WriteLine("URL to for blob upload: ");
            Console.WriteLine(sasUrl);

            return sasUrl;
        }


        public string GetHLSOriginLocator(Models.Asset assetToStream)
        {
            // Get a reference to the manifest file from the collection 
            // of streaming files in the asset. 
            var manifestFile = assetToStream.MediaAsset.AssetFiles.Where(x => x.Name.EndsWith(".ism")).FirstOrDefault();
            // Cast the reference to a true IFileInfo type. 
            if (null == manifestFile)
            {
                return null;
            }

            // Create an 1-day readonly access policy. 
            IAccessPolicy streamingPolicy = this.MediaService.MediaContext.AccessPolicies.Create("Streaming policy",
                TimeSpan.FromDays(1),
                AccessPermissions.Read);

            // Create the origin locator. Set the start time as 5 minutes 
            // before the present so that the locator can be accessed immediately 
            // if there is clock skew between the client and server.
            ILocator originLocator =
                (from l in this.MediaService.MediaContext.Locators
                 where l.AssetId.Equals(assetToStream.MediaAsset.Id)
                 && l.Type == LocatorType.OnDemandOrigin
                 select l).FirstOrDefault();

            if (originLocator != null && originLocator.ExpirationDateTime <= DateTime.UtcNow)
            {
                originLocator.Delete();
                originLocator = null;
            }

            if (originLocator == null)
            {
                originLocator = this.MediaService.MediaContext
                    .Locators.CreateLocator(LocatorType.OnDemandOrigin, assetToStream.MediaAsset,
                 streamingPolicy,
                 DateTime.UtcNow.AddMinutes(-5));
            }
            // Create a full URL to the manifest file. Use this for playback
            // in streaming media clients. 
            string urlForClientStreaming = originLocator.Path + manifestFile.Name + "/manifest(format=m3u8-aapl)";

            // Display the full URL to the streaming manifest file.
            Console.WriteLine("URL to manifest for client streaming: ");
            Console.WriteLine(urlForClientStreaming);

            return urlForClientStreaming;

        }
    }
}
