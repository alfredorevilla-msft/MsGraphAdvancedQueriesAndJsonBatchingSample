using System.Collections.Generic;

namespace MsGraphAdvancedQueriesAndJsonBatchingSample.Models
{
    public class ApplicationModel
    {
        public string DisplayName { get; set; }

        public string AppId { get; set; }

        public IEnumerable<string> ServicePrincipalsIds { get; set; }
        public IEnumerable<string> OwnersNames { get; set; }
    }
}