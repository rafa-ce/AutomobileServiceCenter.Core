using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace ASC.DataAccess.Interfaces
{
    //The primary face for all storage interactions. This interface will house all the instances of 
    //repositories and handle rollback operations in case of exceptions

    public interface IUnitOfWork : IDisposable
    {
        Queue<Task<Action>> RollbackActions { get; set; }
        string ConnectionString { get; set; }
        IRepository<T> Repository<T>() where T : TableEntity;
        void CommitTransaction();
    }
}