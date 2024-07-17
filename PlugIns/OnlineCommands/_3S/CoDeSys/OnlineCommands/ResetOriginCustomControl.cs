using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Online;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    public class ResetOriginCustomControl : UserControl, ICustomControlProvider
    {
        private IList<string> _subControlIds = new List<string>();

        private TreeTableView _tree = new TreeTableView();

        private ResetOriginModel _model;

        private IContainer components;

        private Panel _panel;

        public Control ControlPanel => this;

        public Control ControlToSelect => ((Control)(object)_tree).Controls[0];

        public IList<string> SubControlIDs => _subControlIds;

        public ResetOriginCustomControl(IList<IResetOriginConfigurationItem> items)
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0016: Expected O, but got Unknown
            InitializeComponent();
            foreach (IResetOriginConfigurationItem item in items)
            {
                _subControlIds.Add(item.Description);
            }
            _model = new ResetOriginModel(items);
            _tree.KeepColumnWidthsAdjusted = (true);
            _tree.MultiSelect = (false);
            _tree.ShowLines = (false);
            _tree.GridLines = (true);
            _tree.ShowRootLines = (false);
            _tree.OpenEditOnDblClk = (false);
            _tree.DistributeColumnWidths(new int[2] { 20, 80 });
            _tree.Model = ((ITreeTableModel)(object)_model);
            ((Control)(object)_tree).Dock = DockStyle.Fill;
            _panel.Controls.Add((Control)(object)_tree);
            APEnvironment.DpiAdapter.AdaptControl((Control)(object)_tree);
        }

        public string GetValue(string stSubControlID, out IList<string> AllowedValues)
        {
            AllowedValues = new string[2]
            {
                ResetOriginDeviceCommand.KEEP_ITEM,
                ResetOriginDeviceCommand.DELETE_ITEM
            };
            int num = _subControlIds.IndexOf(stSubControlID);
            if ((bool)((AbstractTreeTableModel)_model).Sentinel.GetChild(num).GetValue(0))
            {
                return ResetOriginDeviceCommand.DELETE_ITEM;
            }
            return ResetOriginDeviceCommand.KEEP_ITEM;
        }

        public void SetValue(string stSubControlID, string stValue)
        {
            int num = _subControlIds.IndexOf(stSubControlID);
            if (stValue == ResetOriginDeviceCommand.DELETE_ITEM)
            {
                ((AbstractTreeTableModel)_model).Sentinel.GetChild(num).SetValue(0, (object)false);
                return;
            }
            if (stValue == ResetOriginDeviceCommand.KEEP_ITEM)
            {
                ((AbstractTreeTableModel)_model).Sentinel.GetChild(num).SetValue(0, (object)true);
                return;
            }
            throw new ArgumentException("Wrong value, only 'KEEP_ITEM' or 'DELETE_ITEM' are allowed.", "stValue");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.ResetOriginCustomControl));
            _panel = new System.Windows.Forms.Panel();
            SuspendLayout();
            resources.ApplyResources(_panel, "_panel");
            _panel.Name = "_panel";
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(_panel);
            base.Name = "ResetOriginCustomControl";
            ResumeLayout(false);
        }
    }
}
