using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCopy.ViewModels
{
    public class GeneralViewModel : ViewModelBase
    {
        private string _computerName;
        private string _description;
        private bool _autoManagePageFile;
        private bool _showAnimations;

        public string ComputerName
        {
            get => _computerName;
            set => SetProperty(ref _computerName, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public bool AutoManagePageFile
        {
            get => _autoManagePageFile;
            set => SetProperty(ref _autoManagePageFile, value);
        }

        public bool ShowAnimations
        {
            get => _showAnimations;
            set => SetProperty(ref _showAnimations, value);
        }
    }
}
