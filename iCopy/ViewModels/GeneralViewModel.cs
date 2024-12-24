using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCopy.ViewModels
{
    public class GeneralViewModel : ViewModelBase
    {
        public string DeviceName { get; } = Environment.MachineName;
        public string SystemType { get; } = Environment.Is64BitOperatingSystem ? "64位操作系统" : "32位操作系统";
        public string SystemVersion { get; } = Environment.OSVersion.ToString();
    }
}
