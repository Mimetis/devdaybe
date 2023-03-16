using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using Dotmim.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.Services
{
    public class SyncServices : ISyncServices
    {

        private readonly SqliteSyncProvider sqliteSyncProvider;
        private readonly SyncAgent syncAgent;
        private readonly WebRemoteOrchestrator webRemoteOrchestrator;
        private readonly HttpClient httpClient;
        private readonly ISettingServices settings;

        public SyncServices(ISettingServices settingServices)
        {
            this.settings = settingServices;
            var syncApiUri = new Uri(this.settings.SyncApiUrl);

            this.httpClient = new HttpClient();

            var handler = new HttpClientHandler();
#if DEBUG
            if (DeviceInfo.Platform == DevicePlatform.Android && syncApiUri.Host == "10.0.2.2")
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (cert.Issuer.Equals("CN=localhost"))
                        return true;
                    return errors == System.Net.Security.SslPolicyErrors.None;
                };
            }
#endif


            handler.AutomaticDecompression = DecompressionMethods.GZip;

            this.httpClient = new HttpClient(handler);

            // Check if we are trying to reach a IIS Express.
            // IIS Express does not allow any request other than localhost
            // So far,hacking the Host-Content header to mimic localhost call
            if (DeviceInfo.Platform == DevicePlatform.Android && syncApiUri.Host == "10.0.2.2")
                this.httpClient.DefaultRequestHeaders.Host = $"localhost:{syncApiUri.Port}";

            this.httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            this.webRemoteOrchestrator = new WebRemoteOrchestrator(this.settings.SyncApiUrl, client: this.httpClient, maxDownladingDegreeOfParallelism: 1);

            this.sqliteSyncProvider = new SqliteSyncProvider(this.settings.DataSource);

            var clientOptions = new SyncOptions { BatchSize = settings.BatchSize, BatchDirectory = settings.BatchDirectoryPath };

            this.syncAgent = new SyncAgent(sqliteSyncProvider, webRemoteOrchestrator, clientOptions);
        }

        public SyncAgent GetSyncAgent() => this.syncAgent;

        public HttpClient GetHttpClient() => this.httpClient;

    }
}
