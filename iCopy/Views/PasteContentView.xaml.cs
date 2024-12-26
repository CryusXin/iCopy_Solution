using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using iCopy.Messages;
using iCopy.Services;
using iCopy.ViewModels;

namespace iCopy.Views
{
    /// <summary>
    /// PasteContent.xaml 的交互逻辑
    /// </summary>
    public partial class PasteContentView : Window
    {
        private readonly PasteContentViewModel _viewModel;
        private readonly MessageService _messageService;
        private bool _isClosing;

        public PasteContentView()
        {
            InitializeComponent();
            _viewModel = new PasteContentViewModel();
            DataContext = _viewModel;

            _messageService = MessageService.Instance;
            _messageService.Register<CloseWindowMessage>(_ => SafeClose());

            // 注册窗口失去焦点事件
            Deactivated += (s, e) =>
            {
                if (!_isClosing)
                {
                    _messageService.Send(new CloseWindowMessage());
                }
            };
        }

        public void ShowCopySuccess()
        {
            if (FindName("CopySuccessTip") is Border copySuccessTip)
            {
                copySuccessTip.Visibility = Visibility.Visible;
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timer.Tick += (s, e) =>
                {
                    copySuccessTip.Visibility = Visibility.Collapsed;
                    timer.Stop();
                };
                timer.Start();
            }
        }

        private void SafeClose()
        {
            if (_isClosing) return;

            _isClosing = true;
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        Close();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error closing window: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SafeClose: {ex.Message}");
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        protected override void OnClosed(EventArgs e)
        {
            _messageService.Unregister<CloseWindowMessage>(_ => SafeClose());
            _viewModel.Cleanup();
            base.OnClosed(e);
        }
    }
}
