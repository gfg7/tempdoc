using MongoDB.Driver;

namespace gRPCServer.Intefaces.DB
{
    public interface IDBContext<TDoc> : IDBContext where TDoc : notnull
    {
        public IMongoCollection<TDoc> Collection { get; }
    }

    public interface IDBContext
    {
        public IMongoDatabase Database { get; }
        public IMongoClient MongoClient { get; }
    }
}
