using gRPCContract.Utils;
using MongoDB.Driver;

namespace gRPCServer.Services.DB
{
    public class MongoClientFactory
    {
        private readonly MongoClient _client;

        public MongoClient Client => _client;
        public MongoClientFactory()
        {
            var settings = new MongoClientSettings()
            {
                Credential = MongoCredential.CreateCredential("admin", Env.Get("MONGO_USERNAME"), Env.Get("MONGO_PASSWORD")),
                Server = new(Env.Get("MONGO_SERVER"), int.Parse(Env.Get("MONGO_PORT"))),
                Scheme = MongoDB.Driver.Core.Configuration.ConnectionStringScheme.MongoDB,
                ConnectTimeout = TimeSpan.FromSeconds(30)
            };

            _client = new MongoClient(settings);
        }
    }
}
