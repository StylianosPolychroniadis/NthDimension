using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NthStudio.Forms
{
    public partial class PhysicsSettingsForm : Form
    {
        public PhysicsSettingsForm()
        {
            InitializeComponent();

            this.numAllowedPenetration.Value = (decimal)((StudioWindow)StudioWindow.Instance).Scene.PhysicsSettings.AllowedPenetration;
            this.numBiasFactor.Value = (decimal)((StudioWindow)StudioWindow.Instance).Scene.PhysicsSettings.BiasFactor;
            this.numMaxBias.Value = (decimal)((StudioWindow)StudioWindow.Instance).Scene.PhysicsSettings.MaximumBias;
            this.numBreakThreshold.Value = (decimal)((StudioWindow)StudioWindow.Instance).Scene.PhysicsSettings.BreakThreshold;
            this.numMinVelocity.Value = (decimal)((StudioWindow)StudioWindow.Instance).Scene.PhysicsSettings.MinimumVelocity;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            NthStudio.StudioWindow.Instance.Scene.PhysicsSettings.AllowedPenetration = (float)numAllowedPenetration.Value;
            NthStudio.StudioWindow.Instance.Scene.PhysicsSettings.BiasFactor = (float)numBiasFactor.Value;
            NthStudio.StudioWindow.Instance.Scene.PhysicsSettings.MaximumBias = (float)numMaxBias.Value;
            NthStudio.StudioWindow.Instance.Scene.PhysicsSettings.BreakThreshold = (float)numBreakThreshold.Value;
            NthStudio.StudioWindow.Instance.Scene.PhysicsSettings.MinimumVelocity = (float)numMinVelocity.Value;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
