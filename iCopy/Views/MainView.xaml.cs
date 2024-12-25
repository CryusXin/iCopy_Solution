using System;
using System.Windows;
using iCopy.Services;
using iCopy.ViewModels;


namespace iCopy.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private readonly TrayIconService _trayIconService;
        private readonly MainViewModel _viewModel;
        private static MainView _instance;
        private bool _isProcessingClose = false;

        public static MainView Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainView();
                }
                return _instance;
            }
        }

        private MainView()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            // 初始化托盘服务
            _trayIconService = TrayIconService.Instance;
            _trayIconService.SetMainView(this);  // 设置MainView引用

            // 注册窗口事件
            Closing += Window_Closing;
        }

        public static void ShowMainView()
        {
            Instance.Show();
            Instance.Activate();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isProcessingClose)
            {
                e.Cancel = true;
                return;
            }

            _isProcessingClose = true;
            try
            {
                // 如果是用户点击关闭按钮，则只是隐藏窗口
                if (!_viewModel.IsExiting)
                {
                    e.Cancel = true;
                    _trayIconService.HideMainWindow();
                }
                else
                {
                    _instance = null; // 清除实例引用
                }
            }
            finally
            {
                _isProcessingClose = false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _trayIconService.Dispose();
            _instance = null; // 清除实例引用
            base.OnClosed(e);
        }
    }
}
