using System.Windows;
using RecallShield.Services;

namespace RecallShield.UI;

public partial class MainWindow : Window
{
    private readonly RecallBlockerService _recallBlocker;
    private readonly ScreenshotBlockerService _screenshotBlocker;

    public MainWindow(
        RecallBlockerService recallBlocker,
        ScreenshotBlockerService screenshotBlocker)
    {
        InitializeComponent();

        _recallBlocker = recallBlocker;
        _screenshotBlocker = screenshotBlocker;

        // Initialize toggle states
        RecallToggle.IsChecked = _recallBlocker.IsEnabled;
        ScreenshotToggle.IsChecked = _screenshotBlocker.IsEnabled;

        // Wire up toggle events
        RecallToggle.Checked += (s, e) => _recallBlocker.IsEnabled = true;
        RecallToggle.Unchecked += (s, e) => _recallBlocker.IsEnabled = false;
        ScreenshotToggle.Checked += (s, e) => _screenshotBlocker.IsEnabled = true;
        ScreenshotToggle.Unchecked += (s, e) => _screenshotBlocker.IsEnabled = false;

        // Update toggle button content based on state
        RecallToggle.Content = _recallBlocker.IsEnabled ? "Disable Recall Protection" : "Enable Recall Protection";
        ScreenshotToggle.Content = _screenshotBlocker.IsEnabled ? "Disable Screenshot Protection" : "Enable Screenshot Protection";

        // Handle window closing
        Closing += MainWindow_Closing;
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // Hide the window instead of closing
        e.Cancel = true;
        Hide();
    }

    private void MinimizeToTray_Click(object sender, RoutedEventArgs e)
    {
        Hide();
    }
} 