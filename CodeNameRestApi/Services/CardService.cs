using CodeNameRestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordsDatabaseAPI.DatabaseModels;

namespace CodeNameRestApi.Services
{
    public class CardService
    {
        public readonly IDatabaseHandler _mongoHandler;
        public CardService(ICodeNameDatabaseSettings settings)
        {
            DatabaseInfo dbInfo = new DatabaseInfo(settings.Port, settings.ConnectionString,
                                                   settings.DatabaseName, settings.CodeNameCollectionName);
            _mongoHandler = new MongoHandler(dbInfo);
        }
    }
}
