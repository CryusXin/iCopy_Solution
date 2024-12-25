using System;
using iCopy.Models;
using System.Windows.Threading;
using System.Windows;
using iCopy.Services;
using iCopy.Messages;
using System.Windows.Input;

namespace iCopy.ViewModels
{
    public class PasteContentViewModel : ViewModelBase
    {
        private readonly MessageService _messageService;
        private PasteContentWindowSettings _windowSettings;
        private string _content;
        private readonly DispatcherTimer _autoCloseTimer;
        private double _remainingSeconds;

        private ICommand _closeWindowCommand;
        public ICommand CloseWindowCommand
        {
            get => _closeWindowCommand;
            private set => SetProperty(ref _closeWindowCommand, value);
        }
        public PasteContentWindowSettings WindowSettings
        {
            get => _windowSettings;
            set => SetProperty(ref _windowSettings, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public double RemainingSeconds
        {
            get => _remainingSeconds;
            set => SetProperty(ref _remainingSeconds, value);
        }

        public PasteContentViewModel()
        {
            _messageService = MessageService.Instance;
            InitializeWindowSettings();
            InitializeCommands();

            // 初始化自动关闭计时器
            _autoCloseTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _autoCloseTimer.Tick += AutoCloseTimer_Tick;

            RemainingSeconds = WindowSettings.AutoCloseSeconds;
            _autoCloseTimer.Start();
        }

        private void InitializeWindowSettings()
        {
            try
            {
                // 获取屏幕尺寸
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;

                // 计算窗口尺寸和位置
                double margin = screenHeight * 0.05; // 5% 边距
                double windowHeight = screenHeight - (2 * margin);
                double windowWidth = 100; // 固定宽度

                WindowSettings = new PasteContentWindowSettings
                {
                    Width = windowWidth,
                    Height = windowHeight,
                    Left = screenWidth - windowWidth - 20, // 右侧显示，留20像素边距
                    Top = margin,
                    TopMost = true,
                    ShowInTaskbar = false,
                    AutoCloseSeconds = 10
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InitializeWindowSettings: {ex.Message}");
            }
        }

        private void AutoCloseTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                RemainingSeconds--;
                if (RemainingSeconds <= 0)
                {
                    _autoCloseTimer?.Stop();
                    Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            _messageService?.Send(new CloseWindowMessage());
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error sending close message: {ex.Message}");
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AutoCloseTimer_Tick: {ex.Message}");
            }
        }

        private void InitializeCommands()
        {
            try
            {
                CloseWindowCommand = new RelayCommand(() =>
                {
                    try
                    {
                        _autoCloseTimer?.Stop();
                        _messageService?.Send(new CloseWindowMessage());
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error executing close command: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InitializeCommands: {ex.Message}");
            }
        }

        public override void Cleanup()
        {
            try
            {
                _autoCloseTimer.Stop();
                Content = null;
                WindowSettings = null;
                base.Cleanup();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Cleanup: {ex.Message}");
            }
        }
    }
}
