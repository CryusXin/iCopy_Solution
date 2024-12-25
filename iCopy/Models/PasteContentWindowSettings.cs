using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCopy.Models
{
    public class PasteContentWindowSettings
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public bool TopMost { get; set; }
        public bool ShowInTaskbar { get; set; }
        public double AutoCloseSeconds { get; set; } = 10; // 自动关闭时间

        public PasteContentWindowSettings()
        {
            TopMost = true;
            ShowInTaskbar = false;
        }
    }
}
