﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLive.Media.Common.Models;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace FLive.Media.Common.Contracts
{
    public interface IJobService
    {
        IMediaService MediaService { get; }
        IEnumerable<IJob> Jobs { get; }

        void CreateEncodeToSmoothStreamingJob(Asset asset, bool decrypt = false);
        void CreateNewJob(Asset asset, string mediaEncoder, string taskPreset);
        void DecryptAsset(Asset theAsset);
        void DeleteJob(string jobId);
        void CancelJob(string jobId);
        string GetPlayReadyTask(string contentKey = "", string keyId = "", string keySeed = "", string playReadyServerUrl = "");
        string GetMp4ToSmoothTask();
        string GetSmoothToHlsTask();
        void CreateThumbnails(Models.Asset asset);
    }
}
