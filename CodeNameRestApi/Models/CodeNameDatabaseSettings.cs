using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeNameRestApi.Models
{
    public class CodeNameDatabaseSettings : ICodeNameDatabaseSettings
    {
        public string CodeNameCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Port { get; set; }

    }

    public interface ICodeNameDatabaseSettings
    {
        string CodeNameCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string Port { get; set; }
    }
}
