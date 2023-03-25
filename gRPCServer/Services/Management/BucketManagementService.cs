using gRPCContract.Interfaces;
using gRPCContract.Models.Request;
using gRPCContract.Models.Stored;
using gRPCServer.Intefaces.Repository;
using gRPCServer.Intefaces.Services;
using gRPCServer.Mappers;
using MongoDB.Bson;
using MongoDB.Driver;
using gRPCServer.Models.CustomException;

namespace gRPCServer.Services.Management
{
    public class BucketManagementService : IClientBucketManagement, IBucketRemoval
    {
        readonly IFilesRepository _filesRepository;
        readonly IInfoRepository<StoredFileInfo> _infoRepository;
        public BucketManagementService(IFilesRepository filesRepository, IInfoRepository<StoredFileInfo> infoRepository)
        {
            _filesRepository = filesRepository;
            _infoRepository = infoRepository;
        }

        private ObjectId CodeValidation(string code) => ObjectId.Parse(code);

        public async Task<(string, MemoryStream)> GetFile(string bucket, string code)
        {
            var id = CodeValidation(code);

            _filesRepository.SetBucketName(bucket);

            var stream = await _filesRepository.GetAsync(id);
            MemoryStream file = new();
            await stream.CopyToAsync(file);

            var fileInfo = (await GetBucket(bucket)).First(x => x.Id == code);

            return (fileInfo.Filename, file);
        }

        public async Task<List<StoredFileInfo>> GetBucket(string bucket)
        {
            var result = await _infoRepository.GetAll(x => x.BucketName == bucket);

            if (!result.Any())
            {
                throw new BucketNotFoundException();
            }

            return result.ToList();
        }

        public async Task DropBucket(string bucket)
        {
            _filesRepository.SetBucketName(bucket);

            await GetBucket(bucket);

            await _filesRepository.DeleteByBucket();
            await _infoRepository.DeleteFile(x => x.BucketName == bucket);
        }

        public async Task DeleteFile(string bucket, string code)
        {
            var id = CodeValidation(code);

            _filesRepository.SetBucketName(bucket);

            await _filesRepository.DeleteFile(id);
            await _infoRepository.DeleteFile(x => x.Id == code);
        }

        public async Task<List<StoredFileInfo>> UploadFiles(string bucket, IFormFileCollection files)
        {
            _filesRepository.SetBucketName(bucket);

            foreach (var file in files)
            {
                using var stream = new MemoryStream();

                await file.CopyToAsync(stream);

                var id = await _filesRepository.Upsert(file.FileName, stream);

                var info = new StoredFileInfo().MapInfoFromFile(bucket, file, id.ToString());

                await _infoRepository.Insert(info);
            }

            return await GetBucket(bucket);
        }

        public async Task<StoredFileInfo> SetExtraSettings(string code, FileDtoRequest extra)
        {
            CodeValidation(code);

            var fileInfo = (await _infoRepository.GetAll(x => x.Id == code)).FirstOrDefault();

            if (fileInfo is null)
            {
                throw new FileInfoNotFoundException();
            }

            fileInfo.Description = extra.Description;
            fileInfo.KeepFor = extra.KeepFor;
            fileInfo.KeepTillDate = fileInfo.UploadDate.AddMinutes(extra.KeepFor);

            await _infoRepository.Upsert(fileInfo, x=> x.Id == code);

            var result =  await _infoRepository.GetAll(x=> x.Id == code);

            return result.First();
        }

        public async Task FlushExpired()
        {
            var expiredFiles = await _infoRepository.GetAll(x => x.KeepTillDate <= DateTime.UtcNow);

            foreach (var item in expiredFiles)
            {
                await DeleteFile(item.BucketName, item.Id);
            }
        }
    }
}
