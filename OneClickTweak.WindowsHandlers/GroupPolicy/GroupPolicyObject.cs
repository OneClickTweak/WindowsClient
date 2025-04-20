using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace OneClickTweak.WindowsHandlers.GroupPolicy;

[ComImport, Guid("EA502722-A23D-11d1-A7D3-0000F87571E3")]
internal class GPClass
{
}

[ComImport, Guid("EA502723-A23D-11d1-A7D3-0000F87571E3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IGroupPolicyObject
{
    uint New([MarshalAs(UnmanagedType.LPWStr)] string domainName, [MarshalAs(UnmanagedType.LPWStr)] string displayName, uint flags);

    uint OpenDSGPO([MarshalAs(UnmanagedType.LPWStr)] string path, uint flags);

    uint OpenLocalMachineGPO(uint flags);

    uint OpenRemoteMachineGPO([MarshalAs(UnmanagedType.LPWStr)] string computerName, uint flags);

    uint Save([MarshalAs(UnmanagedType.Bool)] bool machine, [MarshalAs(UnmanagedType.Bool)] bool add, [MarshalAs(UnmanagedType.LPStruct)] Guid extension, [MarshalAs(UnmanagedType.LPStruct)] Guid app);

    uint Delete();

    uint GetName([MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int maxLength);

    uint GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int maxLength);

    uint SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name);

    uint GetPath([MarshalAs(UnmanagedType.LPWStr)] StringBuilder path, int maxPath);

    uint GetDSPath(uint section, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path, int maxPath);

    uint GetFileSysPath(uint section, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path, int maxPath);

    uint GetRegistryKey(uint section, out IntPtr key);

    uint GetOptions(out uint options);

    uint SetOptions(uint options, uint mask);

    uint GetType(out IntPtr gpoType);

    uint GetMachineName([MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int maxLength);

    uint GetPropertySheetPages(out IntPtr pages);
}


public enum GroupPolicySection
{
    Root = 0,
    User = 1,
    Machine = 2,
}

public abstract class GroupPolicyObject : IDisposable
{
    protected const int MaxLength = 1024;

    /// <summary>
    /// The snap-in that processes .pol files
    /// </summary>
    private static readonly Guid RegistryExtension = new Guid(0x35378EAC, 0x683F, 0x11D2, 0xA8, 0x9A, 0x00, 0xC0, 0x4F, 0xBB, 0xCF, 0xA2);

    /// <summary>
    /// This application
    /// </summary>
    private static readonly Guid LocalGuid = new(GetAssemblyAttribute<GuidAttribute>(Assembly.GetExecutingAssembly())!.Value);

    protected readonly IGroupPolicyObject Instance;

    static T? GetAssemblyAttribute<T>(ICustomAttributeProvider assembly) where T : Attribute
    {
        var attributes = assembly.GetCustomAttributes(typeof(T), true);
        if (attributes.Length == 0)
        {
            return null;
        }

        return (T)attributes[0];
    }

    internal GroupPolicyObject()
    {
        Instance = GetInstance();
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(Instance);
    }

    public void Save()
    {
        var result = Instance.Save(true, true, RegistryExtension, LocalGuid);
        if (result != 0)
        {
            throw new Exception("Error saving machine settings");
        }

        result = Instance.Save(false, true, RegistryExtension, LocalGuid);
        if (result != 0)
        {
            throw new Exception("Error saving user settings");
        }
    }

    public void Delete()
    {
        var result = Instance.Delete();
        if (result != 0)
        {
            throw new Exception("Error deleting the GPO");
        }
    }

    public RegistryKey GetRootRegistryKey(GroupPolicySection section)
    {
        IntPtr key;
        var result = Instance.GetRegistryKey((uint)section, out key);
        if (result != 0)
        {
            throw new Exception($"Unable to get section '{Enum.GetName(typeof(GroupPolicySection), section)}'");
        }

        var handle = new SafeRegistryHandle(key, true);
        return RegistryKey.FromHandle(handle, RegistryView.Default);
    }

    public abstract string GetPathTo(GroupPolicySection section);

    protected static IGroupPolicyObject GetInstance()
    {
        object concrete = new GPClass();
        return (IGroupPolicyObject)concrete;
    }
}

public class GroupPolicyObjectSettings
{
    public readonly bool LoadRegistryInformation;
    public readonly bool Readonly;

    public GroupPolicyObjectSettings(bool loadRegistryInfo = true, bool readOnly = false)
    {
        LoadRegistryInformation = loadRegistryInfo;
        Readonly = readOnly;
    }

    private const uint RegistryFlag = 0x00000001;
    private const uint ReadonlyFlag = 0x00000002;

    internal uint Flag
    {
        get
        {
            uint flag = 0x00000000;
            if (LoadRegistryInformation)
            {
                flag |= RegistryFlag;
            }

            if (Readonly)
            {
                flag |= ReadonlyFlag;
            }

            return flag;
        }
    }
}

public class ComputerGroupPolicyObject : GroupPolicyObject
{
    public readonly bool IsLocal;

    public ComputerGroupPolicyObject(GroupPolicyObjectSettings? options = null)
    {
        options = options ?? new GroupPolicyObjectSettings();
        var result = Instance.OpenLocalMachineGPO(options.Flag);
        if (result != 0)
        {
            throw new Exception("Unable to open local machine GPO");
        }
        IsLocal = true;
    }

    public ComputerGroupPolicyObject(string computerName, GroupPolicyObjectSettings? options = null)
    {
        options ??= new GroupPolicyObjectSettings();
        var result = Instance.OpenRemoteMachineGPO(computerName, options.Flag);
        if (result != 0)
        {
            throw new Exception($"Unable to open GPO on remote machine '{computerName}'");
        }
        IsLocal = false;
    }

    public static Exception? SetPolicySetting(GroupPolicySection section, string keyPath, string key, RegistryValueKind valueKind, object? value)
    {
        // Thread must be STA
        Exception? exception = null;
        var t = new Thread(() =>
        {
            try
            {
                using var gpo = new ComputerGroupPolicyObject();
                using var rootRegistryKey = gpo.GetRootRegistryKey(section);
                // Data can't be null so we can use this value to indicate key must be deleted
                if (value == null)
                {
                    using var subKey = rootRegistryKey.OpenSubKey(keyPath, true);
                    if (subKey != null)
                    {
                        subKey.DeleteValue(key);
                    }
                }
                else
                {
                    using var subKey = rootRegistryKey.CreateSubKey(keyPath);
                    subKey.SetValue(key, value, valueKind);
                }

                gpo.Save();
            }
            catch (Exception? ex)
            {
                exception = ex;
            }
        });

        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join();

        return exception;
    }

    public static (Exception? exception, object? value) GetPolicySetting(GroupPolicySection section, string keyPath, string key)
    {
        // Thread must be STA
        Exception? exception = null;
        object? result = null;
        var t = new Thread(() =>
        {
            try
            {
                using var gpo = new ComputerGroupPolicyObject();
                using var rootRegistryKey = gpo.GetRootRegistryKey(section);
                // Data can't be null so we can use this value to indicate key must be deleted
                using var subKey = rootRegistryKey.OpenSubKey(keyPath, true);
                result = subKey?.GetValue(key);
            }
            catch (Exception? e)
            {
                exception = e;
            }
        });

        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join();

        return (exception, result);
    }

    /// <summary>
    ///     Retrieves the file system path to the root of the specified GPO section.
    ///     The path is in UNC format.
    /// </summary>
    public override string GetPathTo(GroupPolicySection section)
    {
        var sb = new StringBuilder(MaxLength);
        var result = Instance.GetFileSysPath((uint)section, sb, MaxLength);
        if (result != 0)
        {
            throw new Exception($"Unable to retrieve path to section '{Enum.GetName(typeof(GroupPolicySection), section)}'");
        }

        return sb.ToString();
    }
}
