using Core;

namespace Application.Common;

public class StorageQueues : Enumeration
{
    public const string CreateReport = "create-report";
    public const string CreateWorklist = "create-worklist";
    
    public static readonly StorageQueues CreateReportQueue = new (1, CreateReport);
    public static readonly StorageQueues CreateWorkListQueue = new(2, CreateWorklist);

    private StorageQueues(int id, string name) : base(id, name)
    {
        
    }
}