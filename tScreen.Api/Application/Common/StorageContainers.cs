using Core;

namespace Application.Common;

public class StorageContainers : Enumeration
{
    public const string Private = "private";
    public const string Reports = "reports";

    public static readonly StorageContainers PrivateStorage = new(1, Private);
    public static readonly StorageContainers ReportsStorage = new(1, Reports);
    
    private StorageContainers(int id, string name) : base(id, name)
    {
        
    }
}