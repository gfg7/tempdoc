using gRPCServer.Intefaces.DB;
using gRPCServer.Services.Utils;
using MongoDB.Driver;
using WebContract.Models.Stored;

namespace gRPCServer.Services.DB
{
    public class DBContext : IDBContext, IDBContext<StoredFileInfo>
    {
        public DBContext(IMongoClient mongoClient)
        {
            MongoClient = mongoClient;
            Database = MongoClient.GetDatabase(Env.Get("MONGO_DB"));
            Collection = Database.GetCollection<StoredFileInfo>(Env.Get("INFO_COLLECTION"));

            CreateIndexes();
        }
        public IMongoDatabase Database { get; }
        public IMongoClient MongoClient { get; }
        public IMongoCollection<StoredFileInfo> Collection { get; }

        private void CreateIndexes()
        {
            var indexes = new List<CreateIndexModel<StoredFileInfo>>()
            {
                new CreateIndexModel<StoredFileInfo>(Builders<StoredFileInfo>.IndexKeys.Ascending(x => x.KeepTillDate)),

                new CreateIndexModel<StoredFileInfo>(Builders<StoredFileInfo>.IndexKeys.Ascending(x => x.Id).Ascending(x => x.BucketName))
            };

            Collection.Indexes.CreateMany(indexes);
        }
    }
}
