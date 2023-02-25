using Dotmim.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.Services
{
    public interface ISyncServices
    {
        SyncAgent GetSyncAgent();
        HttpClient GetHttpClient();
    }
}
