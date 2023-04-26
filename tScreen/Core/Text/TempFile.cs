using System.IO;

namespace Core;

public class TempFile
{
    public static string GetFilePath() => Path.GetTempPath();

    public static string GetFullyQualifiedPath(string fileName)
    {
        var path = Path.Combine(GetFilePath(), fileName);
        return path;
    }

    public static string GetFileName() => Path.GetTempFileName();

    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}