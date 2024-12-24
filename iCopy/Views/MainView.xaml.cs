using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using iCopy.ViewModels;
using Application = System.Windows.Forms.Application;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;

namespace iCopy.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private NotifyIcon notifyIcon;
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            // 创建托盘图标
            notifyIcon = new NotifyIcon();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string iconPath = System.IO.Path.Combine(Application.StartupPath, "Assets", "image", "logo.ico");
            Icon originalIcon = new Icon(iconPath);
            notifyIcon.Icon = originalIcon;
            notifyIcon.Visible = true;
            notifyIcon.Text = "iCopy";

            // 创建右键菜单
            ContextMenu contextMenu = new ContextMenu();
            MenuItem openMenuItem = new MenuItem("打开面板", Open_Click);
            // MenuItem aboutMenuItem = new MenuItem("关于", About_Click);
            MenuItem exitMenuItem = new MenuItem("退出", Exit_Click);
            contextMenu.MenuItems.Add(openMenuItem);
            contextMenu.MenuItems.Add(exitMenuItem);
            notifyIcon.ContextMenu = contextMenu;

            // 双击图标显示窗口
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide(); // 隐藏窗口
        }

        private void Open_Click(object sender, EventArgs e)
        {
            this.Show(); // 显示窗口
            this.WindowState = WindowState.Normal; // 窗口状态恢复为正常
        }

        // private void About_Click(object sender, EventArgs e)
        // {
        //     AboutWindow aboutWindow = new AboutWindow();
        //     aboutWindow.Show();
        // }

        private void Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false; // 隐藏托盘图标
            notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown(); // 关闭应用程序
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show(); // 显示窗口
            this.WindowState = WindowState.Normal; // 窗口状态恢复为正常
        }

    }
}
