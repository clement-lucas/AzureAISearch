using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FujitsuAzureAISearch.Models
{
    public class StorageConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
        public string VirtualDirectory { get; set; } = string.Empty;
    }

    public class AISearchConfiguration
    {
        public string ServiceName { get; set; } = string.Empty;
        public string DataSourceName { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
