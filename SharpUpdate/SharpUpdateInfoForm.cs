using System;
using System.Windows.Forms;

namespace SharpUpdate
{
    internal partial class SharpUpdateInfoForm : Form
    {
        public SharpUpdateInfoForm(ISharpUpdatable applcationInfo, SharpUpdateXml updateInfo )
        {
            InitializeComponent();

            if (applcationInfo.ApplcationIcon != null)
            {
                this.Icon = applcationInfo.ApplcationIcon;
            }

            this.Text = applcationInfo.ApplicationName + " - Update info";
            this.lblVersions.Text = String.Format("Obecna wersja {0}\nNowa wersja {1}", applcationInfo.ApplcationAssembly.GetName().Version.ToString(), updateInfo.Version.ToString());
            this.txDescription.Text = updateInfo.Description;
        }

        public void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Control && e.KeyCode == Keys.C))
            {
                e.SuppressKeyPress = true;
            }
        }
    }
}
