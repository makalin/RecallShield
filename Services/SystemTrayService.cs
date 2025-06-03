using System.Windows.Forms;

namespace RecallShield.Services;

public class SystemTrayService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly RecallBlockerService _recallBlocker;
    private readonly ScreenshotBlockerService _screenshotBlocker;
    private bool _disposed;

    public SystemTrayService(
        RecallBlockerService recallBlocker,
        ScreenshotBlockerService screenshotBlocker)
    {
        _recallBlocker = recallBlocker;
        _screenshotBlocker = screenshotBlocker;

        _notifyIcon = new NotifyIcon
        {
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath),
            Text = "RecallShield",
            Visible = true
        };

        CreateContextMenu();
    }

    private void CreateContextMenu()
    {
        var menu = new ContextMenuStrip();

        var recallMenuItem = new ToolStripMenuItem("Block Recall")
        {
            Checked = _recallBlocker.IsEnabled
        };
        recallMenuItem.Click += (s, e) =>
        {
            _recallBlocker.IsEnabled = !_recallBlocker.IsEnabled;
            recallMenuItem.Checked = _recallBlocker.IsEnabled;
        };

        var screenshotMenuItem = new ToolStripMenuItem("Block Screenshots")
        {
            Checked = _screenshotBlocker.IsEnabled
        };
        screenshotMenuItem.Click += (s, e) =>
        {
            _screenshotBlocker.IsEnabled = !_screenshotBlocker.IsEnabled;
            screenshotMenuItem.Checked = _screenshotBlocker.IsEnabled;
        };

        var exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += (s, e) => Application.Exit();

        menu.Items.AddRange(new ToolStripItem[]
        {
            recallMenuItem,
            screenshotMenuItem,
            new ToolStripSeparator(),
            exitMenuItem
        });

        _notifyIcon.ContextMenuStrip = menu;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _notifyIcon.Dispose();
            }
            _disposed = true;
        }
    }
} 