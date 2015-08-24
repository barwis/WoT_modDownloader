using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoT_modDownloader
{
    class ComplexResponse
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public double Percent { get; set; }
        public long TotalBytes { get; set; }
        public long CurrentBytes { get; set; }
        public long FileSize { get; set; }
    }
}
