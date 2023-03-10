using MongoDB.Driver;
using System.Linq.Expressions;

namespace gRPCServer.Intefaces.Repository
{
    public interface IInfoRepository<TDoc> where TDoc :  class
    {
        Task<long> DeleteFile(Expression<Func<TDoc, bool>> expression);
        Task<IEnumerable<TDoc>> GetAll(Expression<Func<TDoc, bool>> expression);
        Task Upsert(TDoc file, Expression<Func<TDoc, bool>> expression);
        Task Insert(TDoc file);
    }
}