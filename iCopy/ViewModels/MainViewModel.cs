using System.Windows.Input;
using iCopy.Services;
using iCopy.Views;
using System.Windows;
using System;

namespace iCopy.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private int _selectedTabIndex;
        private object _currentView;
        private bool _isExiting;
        private PasteContentView _pasteWindow;
        private readonly KeyboardService _keyboardService;
        private readonly TrayIconService _trayIconService;

        public bool IsExiting
        {
            get => _isExiting;
            set => SetProperty(ref _isExiting, value);

        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                _selectedTabIndex = value;
                UpdateCurrentView();
                OnPropertyChanged();
            }
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        // 添加命令
        public ICommand ExitApplicationCommand { get; }
        public ICommand ShowWindowCommand { get; }
        public MainViewModel()
        {
            // 默认显示General页面
            _keyboardService = KeyboardService.Instance;
            _trayIconService = TrayIconService.Instance;

            // 初始化命令
            ExitApplicationCommand = new RelayCommand(ExecuteExit);
            ShowWindowCommand = new RelayCommand(ExecuteShow);
            SelectedTabIndex = 0;
            UpdateCurrentView();
            InitializeKeyboardService();
        }

        private void ExecuteExit()
        {
            IsExiting = true;
            System.Windows.Application.Current.Shutdown();
        }

        private void ExecuteShow()
        {
            _trayIconService.ShowMainWindow();
        }



        private void InitializeKeyboardService()
        {
            _keyboardService.KeyPressed += KeyboardService_KeyPressed;
        }

        private void KeyboardService_KeyPressed(Key key)
        {
            try
            {
                // 例如：按下 F2 键时显示窗口
                if (key == Key.F2)
                {
                    System.Diagnostics.Debug.WriteLine("F2 key pressed - Opening paste window");
                    var app = Application.Current;
                    if (app != null && app.Dispatcher != null)
                    {
                        app.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                ShowPasteWindow();
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error showing paste window: {ex.Message}");
                            }
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in KeyboardService_KeyPressed: {ex.Message}");
            }
        }

        private void ShowPasteWindow()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ShowPasteWindow called");
                if (_pasteWindow == null)
                {
                    // 先隐藏主窗口
                    _trayIconService.HideMainWindow();

                    _pasteWindow = new PasteContentView();
                    _pasteWindow.Closed += (s, e) =>
                    {
                        System.Diagnostics.Debug.WriteLine("PasteWindow closed");
                        _pasteWindow = null;
                    };
                    _pasteWindow.Show();
                    _pasteWindow.Activate();
                    System.Diagnostics.Debug.WriteLine("PasteWindow shown and activated");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("PasteWindow already exists");
                    _pasteWindow.Activate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ShowPasteWindow: {ex.Message}");
            }
        }

        public override void Cleanup()
        {
            _keyboardService.KeyPressed -= KeyboardService_KeyPressed;
            _keyboardService.Cleanup();
            _pasteWindow?.Close();
            base.Cleanup();
        }

        private void UpdateCurrentView()
        {
            //CurrentView = SelectedTabIndex switch
            //{
            //    0 => new GeneralView(),
            //    1 => new ServerView(),
            //    2 => new AboutView(),
            //    _ => CurrentView
            //};
            switch (SelectedTabIndex)
            {
                case 0:
                    CurrentView = new GeneralView();
                    break;
                case 1:
                    CurrentView = new ControlView();
                    break;
                case 2:
                    CurrentView = new ServerView();
                    break;
                case 3:
                    CurrentView = new AboutView();
                    break;
                default:
                    break;
            }
        }
    }
}
