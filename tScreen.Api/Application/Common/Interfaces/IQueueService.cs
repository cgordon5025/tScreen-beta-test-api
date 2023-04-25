using System.Threading.Tasks;
using Core;

namespace Application.Common.Interfaces;

public interface IQueueService
{
    Task SendMessageAsync(string message, StorageQueues queue);
    
    Task<TEntity?> GetNextMessageAsync<TEntity>(StorageQueues queue) where TEntity : class, new();
}