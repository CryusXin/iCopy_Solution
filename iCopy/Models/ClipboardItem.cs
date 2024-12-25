using System;

namespace iCopy.Models
{
    public class ClipboardItem
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
    }
}