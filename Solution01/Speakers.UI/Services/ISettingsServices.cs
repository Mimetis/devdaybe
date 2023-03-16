using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.Services
{
    public interface ISettingServices
    {
        string DataSource { get; }
        string DataSourcePath { get; }
        string DataSourceName { get; }
        string BatchDirectoryPath { get; }
        string BatchDirectoryName { get; }
        string SyncApiUrl { get; }
        int BatchSize { get; }
    }
}
