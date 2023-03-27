using gRPCServer.Intefaces.DB;
using gRPCServer.Intefaces.Repository;
using gRPCServer.Services.Utils;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace gRPCServer.Services.Repository
{
    public class FilesRepository : IFilesRepository
    {
        private IGridFSBucket _bucket;
        private readonly IDBContext _context;

        public FilesRepository(IDBContext context)
        {
            _context = context;
        }

        public void SetBucketName(string bucketName)
        {
            var settings = new GridFSBucketOptions()
            {
                DisableMD5 = false,
                ChunkSizeBytes = int.Parse(Env.Get("CHUNK_SIZE")),
                BucketName = bucketName
            };

            _bucket = new GridFSBucket(_context.Database, settings);
        }

        public async Task DeleteByBucket()
        {
            await _bucket.DropAsync();
        }

        public async Task DeleteFile(ObjectId id)
        {
            await _bucket.DeleteAsync(id);
        }

        public async Task<GridFSDownloadStream<ObjectId>> GetAsync(ObjectId id) => await _bucket.OpenDownloadStreamAsync(id);

        public async Task<ObjectId> Upsert(string filename, Stream file)
        {
            file.Seek(0, SeekOrigin.Begin);

            var documentId = await _bucket.UploadFromStreamAsync(filename, file);

            return documentId;
        }
    }
}
