using System.Runtime.CompilerServices;
using gRPCServer.Intefaces.DB;
using gRPCServer.Intefaces.Repository;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace gRPCServer.Services.Repository
{
    public class InfoRepository<TDoc> : IInfoRepository<TDoc> where TDoc : class
    {
        private readonly IDBContext<TDoc> _context;
        private readonly FilterDefinitionBuilder<TDoc> _filter = Builders<TDoc>.Filter;

        public InfoRepository(IDBContext<TDoc> context)
        {
            _context = context;
        }

        public async Task<long> DeleteFile(Expression<Func<TDoc, bool>> expression) => (await _context.Collection.DeleteManyAsync(_filter.Where(expression))).DeletedCount;

        public async Task<IEnumerable<TDoc>> GetAll(Expression<Func<TDoc, bool>> expression) => await (await _context.Collection.FindAsync<TDoc>(_filter.Where(expression))).ToListAsync();

        public async Task Upsert(TDoc file, Expression<Func<TDoc, bool>> expression = null) =>
         await _context.Collection.ReplaceOneAsync<TDoc>(expression,
            file,
            new ReplaceOptions() 
            {
                IsUpsert = true
            });
    }
}
