using System.Management;

namespace RecallShield.Services;

public class RecallBlockerService
{
    private const string RecallServiceName = "RecallService";
    private bool _isEnabled = true;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                if (value)
                    EnableRecallBlock();
                else
                    DisableRecallBlock();
            }
        }
    }

    public RecallBlockerService()
    {
        EnableRecallBlock();
    }

    private void EnableRecallBlock()
    {
        try
        {
            // Disable Recall service
            using var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_Service WHERE Name = '" + RecallServiceName + "'");
            
            foreach (var service in searcher.Get())
            {
                var method = service.GetMethodParameters("StopService");
                service.InvokeMethod("StopService", method, null);
                
                method = service.GetMethodParameters("ChangeStartMode");
                method["StartMode"] = "Disabled";
                service.InvokeMethod("ChangeStartMode", method, null);
            }

            // Set registry policies to disable Recall
            using var key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(
                @"SOFTWARE\Policies\Microsoft\Windows\Recall");
            key?.SetValue("EnableRecall", 0, Microsoft.Win32.RegistryValueKind.DWord);
        }
        catch (Exception ex)
        {
            // Log error or handle appropriately
            System.Diagnostics.Debug.WriteLine($"Error enabling Recall block: {ex.Message}");
        }
    }

    private void DisableRecallBlock()
    {
        try
        {
            // Re-enable Recall service
            using var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_Service WHERE Name = '" + RecallServiceName + "'");
            
            foreach (var service in searcher.Get())
            {
                var method = service.GetMethodParameters("ChangeStartMode");
                method["StartMode"] = "Automatic";
                service.InvokeMethod("ChangeStartMode", method, null);
                
                method = service.GetMethodParameters("StartService");
                service.InvokeMethod("StartService", method, null);
            }

            // Remove registry policies
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Policies\Microsoft\Windows\Recall", true);
            key?.DeleteValue("EnableRecall", false);
        }
        catch (Exception ex)
        {
            // Log error or handle appropriately
            System.Diagnostics.Debug.WriteLine($"Error disabling Recall block: {ex.Message}");
        }
    }
} 