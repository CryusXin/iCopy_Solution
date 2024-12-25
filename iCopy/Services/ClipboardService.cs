using System;
using System.Windows;
using iCopy.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace iCopy.Services
{
    public class ClipboardService : IDisposable
    {
        private static ClipboardService _instance;
        private readonly DatabaseService _databaseService;
        private ClipboardWatcher _clipboardWatcher;
        private bool _isCtrlCPressed;
        public ObservableCollection<ClipboardItem> ClipboardItems { get; private set; }

        public static ClipboardService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClipboardService();
                }
                return _instance;
            }
        }

        private ClipboardService()
        {
            _databaseService = DatabaseService.Instance;
            ClipboardItems = new ObservableCollection<ClipboardItem>();
            LoadClipboardItems();
            StartClipboardMonitor();
            StartKeyboardMonitor();
        }

        private void StartKeyboardMonitor()
        {
            KeyboardService.Instance.KeyPressed += (key) =>
            {
                if (key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    _isCtrlCPressed = true;
                }
                else
                {
                    _isCtrlCPressed = false;
                }
            };
        }

        private void LoadClipboardItems()
        {
            var items = _databaseService.GetClipboardItems();
            foreach (var item in items.OrderByDescending(x => x.CreateTime))
            {
                ClipboardItems.Add(item);
            }
        }

        private void StartClipboardMonitor()
        {
            _clipboardWatcher = new ClipboardWatcher();
            _clipboardWatcher.ClipboardChanged += ClipboardWatcher_ClipboardChanged;
        }

        private void ClipboardWatcher_ClipboardChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_isCtrlCPressed)
                {
                    return;
                }

                _isCtrlCPressed = false; // 重置标志

                if (System.Windows.Clipboard.ContainsText())
                {
                    string text = System.Windows.Clipboard.GetText();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var clipboardItem = new ClipboardItem
                        {
                            Content = text,
                            CreateTime = DateTime.Now
                        };

                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            _databaseService.AddClipboardItem(clipboardItem);
                            ClipboardItems.Insert(0, clipboardItem);
                            //System.Diagnostics.Debug.WriteLine($"Added clipboard item: {text}");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ClipboardWatcher_ClipboardChanged: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _clipboardWatcher?.Dispose();
        }
    }

    public class ClipboardWatcher : Form
    {
        public event EventHandler ClipboardChanged;
        private IntPtr _clipboardViewerNext;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        public ClipboardWatcher()
        {
            _clipboardViewerNext = SetClipboardViewer(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_DRAWCLIPBOARD = 0x0308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    ClipboardChanged?.Invoke(this, EventArgs.Empty);
                    break;
                case WM_CHANGECBCHAIN:
                    if (m.WParam == _clipboardViewerNext)
                    {
                        _clipboardViewerNext = m.LParam;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ChangeClipboardChain(Handle, _clipboardViewerNext);
            }
            base.Dispose(disposing);
        }
    }
}