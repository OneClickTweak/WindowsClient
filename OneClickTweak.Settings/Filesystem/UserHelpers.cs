namespace OneClickTweak.Settings.Filesystem;

public static class FilesystemHelpers
{
    public static bool IsDirectoryWritable(string dirPath)
    {
        try
        {
            var tempFileName = Path.Combine(dirPath, Path.GetRandomFileName());
            using var _ = File.Create(tempFileName, 1, FileOptions.DeleteOnClose);
            return true;
        }
        catch
        {
            return false;
        }
    }
}