using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLive.Media.Common.Contracts
{
    public interface IFileService
    {
        IMediaService MediaService { get; }
    }
}
