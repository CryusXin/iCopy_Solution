
using iCopy.Views;

namespace iCopy.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private int _selectedTabIndex;
        private object _currentView;

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

        public MainViewModel()
        {
            // 默认显示General页面
            SelectedTabIndex = 0;
            UpdateCurrentView();
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
