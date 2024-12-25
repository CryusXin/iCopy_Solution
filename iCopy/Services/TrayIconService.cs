using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows;
using System.IO;

namespace iCopy.Services
{
    public class TrayIconService : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private static TrayIconService _instance;
        private static readonly object _lock = new object();
        private Window _mainView;
        private bool _isDisposed;

        public static TrayIconService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TrayIconService();
                        }
                    }
                }
                return _instance;
            }
        }

        public void SetMainView(Window mainView)
        {
            _mainView = mainView;
        }

        private TrayIconService()
        {
            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Text = "iCopy"
            };
            InitializeNotifyIcon();
        }

        private void InitializeNotifyIcon()
        {
            try
            {
                if (_isDisposed || _notifyIcon == null) return;

                // 设置图标
                string iconPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
                    "image",
                    "logotp.ico");

                if (File.Exists(iconPath))
                {
                    using (Icon originalIcon = new Icon(iconPath))
                    {
                        _notifyIcon.Icon = (Icon)originalIcon.Clone();
                    }
                }

                // 创建右键菜单
                var contextMenu = new ContextMenu();
                contextMenu.MenuItems.Add(new MenuItem("打开面板", (s, e) => ShowMainWindow()));
                contextMenu.MenuItems.Add(new MenuItem("退出", (s, e) => ExitApplication()));
                _notifyIcon.ContextMenu = contextMenu;

                // 双击事件
                _notifyIcon.DoubleClick += (s, e) => ShowMainWindow();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeNotifyIcon error: {ex.Message}");
            }
        }

        public void ShowMainWindow()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_mainView != null)
                    {
                        // 先恢复任务栏显示
                        _mainView.ShowInTaskbar = true;
                        // 显示窗口
                        _mainView.Show();
                        // 恢复窗口状态
                        _mainView.WindowState = WindowState.Normal;
                        _mainView.Activate();
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowMainWindow error: {ex.Message}");
            }
        }

        public void HideMainWindow()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_mainView != null && _mainView.Visibility == Visibility.Visible)
                    {
                        // 直接隐藏窗口和任务栏图标
                        _mainView.ShowInTaskbar = false;
                        _mainView.Hide();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Main window is null or already hidden");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HideMainWindow error: {ex.Message}");
            }
        }

        private void ExitApplication()
        {
            try
            {
                if (_isDisposed) return;

                // 先将图标设为不可见
                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = false;
                }

                // 清理资源
                Dispose();

                // 关闭应用程序
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.Application.Current.Shutdown();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ExitApplication error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            try
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = false;
                    if (_notifyIcon.Icon != null)
                    {
                        _notifyIcon.Icon.Dispose();
                    }
                    _notifyIcon.Dispose();
                    _notifyIcon = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dispose error: {ex.Message}");
            }
            finally
            {
                _isDisposed = true;
                _instance = null;
            }
        }
    }
}
