using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace OneClickTweak.WindowsHandlers.Registry;

public class Hive : IDisposable
{
    private const int ErrorSuccess = 0;

    [DllImport("advapi32.dll", SetLastError = true)]
    static extern int RegLoadKey(IntPtr hKey, string lpSubKey, string lpFile);

    [DllImport("advapi32.dll", SetLastError = true)]
    static extern int RegSaveKey(IntPtr hKey, string lpFile, uint securityAttrPtr = 0);

    [DllImport("advapi32.dll", SetLastError = true)]
    static extern int RegUnLoadKey(IntPtr hKey, string lpSubKey);

    [DllImport("ntdll.dll", SetLastError = true)]
    static extern IntPtr RtlAdjustPrivilege(int privilege, bool bEnablePrivilege, bool isThreadPrivilege, out bool previousValue);

    [DllImport("advapi32.dll")]
    static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref UInt64 lpLuid);

    [DllImport("advapi32.dll")]
    static extern bool LookupPrivilegeValue(IntPtr lpSystemName, string lpName, ref UInt64 lpLuid);

    private readonly RegistryKey parentKey;
    private bool parentKeyLoaded;
    private readonly string name;

    private Hive(RegistryKey parentKey, string name)
    {
        this.parentKey = parentKey;
        this.name = name;
    }

    public RegistryKey? RootKey { get; private set; }

    public static (Hive? hive, string? error) LoadFromFile(string filePath)
    {
        RegistryKey? parentKey;
        try
        {
            parentKey = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Default);
        }
        catch (Exception e)
        {
            return (null, e.Message);
        }

        var hive = new Hive(parentKey, Guid.NewGuid().ToString());

        var parentHandle = hive.parentKey.Handle.DangerousGetHandle();
        hive.parentKeyLoaded = RegLoadKey(parentHandle, hive.name, filePath) == ErrorSuccess;
        if (!hive.parentKeyLoaded)
        {
            return (null, $"Unable to load the registry hive: {filePath}");
        }

        var lastError = Marshal.GetLastWin32Error();
        if (lastError != ErrorSuccess)
        {
            return (null, $"Error {lastError} loading the registry hive: {filePath}");
        }

        var rootKey = hive.parentKey.OpenSubKey(hive.name, true);
        if (rootKey == null)
        {
            return (null, $"Error opening the registry hive: {filePath}");
        }

        lastError = Marshal.GetLastWin32Error();
        if (lastError != ErrorSuccess)
        {
            return (null, $"Error {lastError} opening the registry hive: {filePath}");
        }

        hive.RootKey = rootKey;
        return (hive,null);
    }

    public static void AcquirePrivileges()
    {
        ulong luid = 0;
        LookupPrivilegeValue(IntPtr.Zero, "SeRestorePrivilege", ref luid);
        RtlAdjustPrivilege((int)luid, true, false, out _);
        LookupPrivilegeValue(IntPtr.Zero, "SeBackupPrivilege", ref luid);
        RtlAdjustPrivilege((int)luid, true, false, out _);
    }

    public static void ReturnPrivileges()
    {
        ulong luid = 0;
        LookupPrivilegeValue(IntPtr.Zero, "SeRestorePrivilege", ref luid);
        RtlAdjustPrivilege((int)luid, false, false, out _);
        LookupPrivilegeValue(IntPtr.Zero, "SeBackupPrivilege", ref luid);
        RtlAdjustPrivilege((int)luid, false, false, out _);
    }

    public void SaveAndUnload()
    {
        RegUnLoadKey(parentKey.Handle.DangerousGetHandle(), name);
        parentKey.Close();
    }

    public void Dispose()
    {
        RootKey?.Close();
        RootKey?.Dispose();

        if (parentKeyLoaded)
        {
            RegUnLoadKey(parentKey.Handle.DangerousGetHandle(), name);
        }

        parentKey.Dispose();
    }
}