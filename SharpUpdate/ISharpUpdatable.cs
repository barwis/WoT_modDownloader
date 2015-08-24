using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace SharpUpdate
{
    public interface ISharpUpdatable
    {
        string ApplicationName { get; }
        string ApplicationID { get; }
        Assembly ApplcationAssembly { get; }
        Icon ApplcationIcon { get; }
        Uri UpdateXmlLocation { get; }
        Form Context { get; }
    }
}
