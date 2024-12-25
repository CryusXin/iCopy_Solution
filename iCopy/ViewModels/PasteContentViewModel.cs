using System;
using iCopy.Models;
using System.Windows.Threading;
using System.Windows;
using iCopy.Services;
using iCopy.Messages;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace iCopy.ViewModels
{
    public class PasteContentViewModel : ViewModelBase
    {
        private readonly MessageService _messageService;
        private readonly ClipboardService _clipboardService;
        private readonly DatabaseService _databaseService;
        private PasteContentWindowSettings _windowSettings;
        private string _content;
        private readonly DispatcherTimer _autoCloseTimer;
        private double _remainingSeconds;
        private ClipboardItem _selectedItem;

        private ICommand _closeWindowCommand;
        private ICommand _deleteItemCommand;

        public ICommand CloseWindowCommand
        {
            get => _closeWindowCommand;
            private set => SetProperty(ref _closeWindowCommand, value);
        }

        public ICommand DeleteItemCommand
        {
            get => _deleteItemCommand;
            private set => SetProperty(ref _deleteItemCommand, value);
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

        public ObservableCollection<ClipboardItem> ClipboardItems => _clipboardService.ClipboardItems;

        public ClipboardItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value) && value != null)
                {
                    Content = value.Content;
                    System.Windows.Clipboard.SetText(value.Content);
                }
            }
        }

        public PasteContentViewModel()
        {
            _messageService = MessageService.Instance;
            _clipboardService = ClipboardService.Instance;
            _databaseService = DatabaseService.Instance;
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

            // 如果有剪贴板历史，选择最新的一项
            if (ClipboardItems.Any())
            {
                SelectedItem = ClipboardItems.First();
            }
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
                double windowHeight = screenHeight - (3 * margin);
                double windowWidth = 300; // 固定宽度为300像素

                //System.Diagnostics.Debug.WriteLine($"Screen size: {screenWidth}x{screenHeight}");
                //System.Diagnostics.Debug.WriteLine($"Window size: {windowWidth}x{windowHeight}");

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

                //System.Diagnostics.Debug.WriteLine($"Window position: Left={WindowSettings.Left}, Top={WindowSettings.Top}");
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

        private void DeleteItem(ClipboardItem item)
        {
            if (item != null)
            {
                try
                {
                    // 从数据库删除
                    _databaseService.DeleteClipboardItem(item.Id);
                    // 从列表中删除
                    ClipboardItems.Remove(item);
                    //System.Diagnostics.Debug.WriteLine($"Deleted clipboard item: {item.Content}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deleting clipboard item: {ex.Message}");
                }
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

                DeleteItemCommand = new RelayCommand<ClipboardItem>(DeleteItem);
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
