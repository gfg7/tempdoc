using gRPCContract.Models.Stored;
using gRPCServer.Intefaces.Repository;
using System.Linq.Expressions;
using TempDocTest.Mock.TestData;

namespace TempDocTest.Mock.Repository
{
    internal class InfoRepositoryMock : IInfoRepository<StoredFileInfo>
    {
        private TestFileInfo _context;
        public InfoRepositoryMock(TestFileInfo context)
        {
            _context = context;
        }

        public Task<long> DeleteFile(Expression<Func<StoredFileInfo, bool>> expression)
        {
            var predicate = expression.Compile();

            _context.Data.RemoveAll(x => predicate(x));

            return Task.FromResult((long)1);
        }

        public Task<IEnumerable<StoredFileInfo>> GetAll(Expression<Func<StoredFileInfo, bool>> expression)
        {
            var predicate = expression.Compile();

            return Task.FromResult(_context.Data.Where(x => predicate(x)));
        }

        public Task Upsert(List<StoredFileInfo> files)
        {
            foreach (var item in files)
            {
                Upsert(item);
            }

            return Task.CompletedTask;
        }

        public Task Upsert(StoredFileInfo file, Expression<Func<StoredFileInfo, bool>> expression = null)
        {
             if (_context.Data.Contains(file))
            {
                _context.Data.Remove(file);
            }
            _context.Data.Add(file);

            return Task.CompletedTask;
        }
    }
}
