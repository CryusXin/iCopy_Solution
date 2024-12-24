
using System.ComponentModel;
using System.Windows;


namespace iCopy.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : Window, INotifyPropertyChanged
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _UserNmae;

        public string UserName
        {
            get { return _UserNmae; }
            set
            {
                _UserNmae = value;
                RaisePropertyChanged("UserName");
            }
        }

        private string _PassWord;

        public string PassWord
        {
            get { return _PassWord; }
            set
            {
                _PassWord = value;
                RaisePropertyChanged("PassWord");
            }
        }

        private string _Server;

        public string Server
        {
            get { return _Server; }
            set
            {
                _Server = value;
                RaisePropertyChanged("Server");
            }
        }

        private void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("server:" + server + " username:" + username + " password:" + password);
            //判断server是否为空
            if (string.IsNullOrEmpty(Server))
            {
                MessageBox.Show("服务器地址不能为空");
                Clean_Text_Box();
                return;
            }
            //判断用户名或密码是否为空
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(PassWord))
            {
                MessageBox.Show("用户名或密码不能为空");
                Clean_Text_Box();
                return;
            }
            else
            {
                //登录成功
                MainView mainView = new MainView();
                mainView.Show();
                this.Close();
            }
        }

        //清空输入框
        private void Clean_Text_Box()
        {
            UserName = "";
            PassWord = "";
            Server = "";

        }

        private void Button_Click_Local(object sender, RoutedEventArgs e)
        {
            MainView mainView = new MainView();
            mainView.Show();
            this.Close();
        }
    }
}
