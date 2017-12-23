﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace FLive.Media.Common.Contracts
{
    public interface IMediaService
    {
        CloudMediaContext MediaContext { get; }
        void Reset();
        IMediaProcessor GetMediaProcessorByName(string name);
    }
}
