using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace iCopy.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private string _versionInfo;
        private string _buildDate;
        private string _systemInfo;

        public string VersionInfo
        {
            get => _versionInfo;
            set
            {
                _versionInfo = value;
                OnPropertyChanged();
            }
        }

        public string BuildDate
        {
            get => _buildDate;
            set
            {
                _buildDate = value;
                OnPropertyChanged();
            }
        }

        public string SystemInfo
        {
            get => _systemInfo;
            set
            {
                _systemInfo = value;
                OnPropertyChanged();
            }
        }

        public ICommand VisitWebsiteCommand { get; }
        public ICommand CheckUpdateCommand { get; }
        public ICommand GetSupportCommand { get; }

        public AboutViewModel()
        {
            InitializeData();
            InitializeCommands();
        }

        private void InitializeData()
        {
            // 获取应用程序版本
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            VersionInfo = $"版本 {version.Major}.{version.Minor}.{version.Build}";

            // 设置构建日期
            BuildDate = $"构建日期: 2024-12-24";

            // 获取系统信息
            SystemInfo = GetSystemInfo();
        }

        private void InitializeCommands()
        {
            //VisitWebsiteCommand = new RelayCommand(ExecuteVisitWebsite);
            //CheckUpdateCommand = new RelayCommand(ExecuteCheckUpdate);
            //GetSupportCommand = new RelayCommand(ExecuteGetSupport);
        }

        private string GetSystemInfo()
        {
            return $"操作系统: {Environment.OSVersion}\n" +
                   $"处理器架构: {Environment.ProcessorCount} 核心\n" +
                   $".NET 版本: {Environment.Version}\n" +
                   $"系统内存: {GetTotalMemory()} GB";
        }

        private string GetTotalMemory()
        {
            try
            {
                //return (new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory
                //       / (1024.0 * 1024 * 1024)).ToString("F1");
                return "未知";
            }
            catch
            {
                return "未知";
            }
        }

        private void ExecuteVisitWebsite()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://your-website.com",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开网站: {ex.Message}");
            }
        }

        private void ExecuteCheckUpdate()
        {
            // 实现检查更新逻辑
            MessageBox.Show("正在检查更新...", "更新检查");
        }

        private void ExecuteGetSupport()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "mailto:support@your-company.com",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开邮件客户端: {ex.Message}");
            }
        }
    }
}
