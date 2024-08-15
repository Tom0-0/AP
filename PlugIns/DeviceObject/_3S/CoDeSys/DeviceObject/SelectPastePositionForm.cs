using System.ComponentModel;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	public class SelectPastePositionForm : Form
	{
		private RadioButton _rbInsert;

		private Container components;

		private RadioButton _rbAppend;

		private Button _btnOK;

		private Button _btnCancel;

		private string _stTarget = "<please assign>";

		private Label label2;

		private Label label3;

		private ListBox _listBox;

		private LList<IPastedObject> _alPasteObjects;

		public string TargetElement
		{
			get
			{
				return _stTarget;
			}
			set
			{
				_stTarget = value;
				_rbInsert.Text = string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SelectPastePositionInsert"), value);
				_rbAppend.Text = string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SelectPastePositionAppend"), value);
			}
		}

		public PastePosition PastePosition
		{
			get
			{
				if (_rbInsert.Checked)
				{
					return PastePosition.InsertBefore;
				}
				return PastePosition.AppendBelow;
			}
		}

		public LList<IPastedObject> PasteObjects
		{
			get
			{
				return _alPasteObjects;
			}
			set
			{
				_alPasteObjects = value;
				_listBox.Items.Clear();
				foreach (IPastedObject item in value)
				{
					_listBox.Items.Add(item.Name);
				}
			}
		}

		public SelectPastePositionForm()
		{
			InitializeComponent();
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceObject.SelectPastePositionForm));
			_rbInsert = new System.Windows.Forms.RadioButton();
			_rbAppend = new System.Windows.Forms.RadioButton();
			_btnOK = new System.Windows.Forms.Button();
			_btnCancel = new System.Windows.Forms.Button();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			_listBox = new System.Windows.Forms.ListBox();
			SuspendLayout();
			_rbInsert.AccessibleDescription = null;
			_rbInsert.AccessibleName = null;
			componentResourceManager.ApplyResources(_rbInsert, "_rbInsert");
			_rbInsert.BackgroundImage = null;
			_rbInsert.Checked = true;
			_rbInsert.Font = null;
			_rbInsert.Name = "_rbInsert";
			_rbInsert.TabStop = true;
			_rbAppend.AccessibleDescription = null;
			_rbAppend.AccessibleName = null;
			componentResourceManager.ApplyResources(_rbAppend, "_rbAppend");
			_rbAppend.BackgroundImage = null;
			_rbAppend.Font = null;
			_rbAppend.Name = "_rbAppend";
			_btnOK.AccessibleDescription = null;
			_btnOK.AccessibleName = null;
			componentResourceManager.ApplyResources(_btnOK, "_btnOK");
			_btnOK.BackgroundImage = null;
			_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			_btnOK.Font = null;
			_btnOK.Name = "_btnOK";
			_btnCancel.AccessibleDescription = null;
			_btnCancel.AccessibleName = null;
			componentResourceManager.ApplyResources(_btnCancel, "_btnCancel");
			_btnCancel.BackgroundImage = null;
			_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_btnCancel.Font = null;
			_btnCancel.Name = "_btnCancel";
			label2.AccessibleDescription = null;
			label2.AccessibleName = null;
			componentResourceManager.ApplyResources(label2, "label2");
			label2.Name = "label2";
			label3.AccessibleDescription = null;
			label3.AccessibleName = null;
			componentResourceManager.ApplyResources(label3, "label3");
			label3.Name = "label3";
			_listBox.AccessibleDescription = null;
			_listBox.AccessibleName = null;
			componentResourceManager.ApplyResources(_listBox, "_listBox");
			_listBox.BackgroundImage = null;
			_listBox.Font = null;
			_listBox.Name = "_listBox";
			_listBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
			base.AcceptButton = _btnOK;
			base.AccessibleDescription = null;
			base.AccessibleName = null;
			componentResourceManager.ApplyResources(this, "$this");
			BackgroundImage = null;
			base.CancelButton = _btnCancel;
			base.Controls.Add(_listBox);
			base.Controls.Add(label3);
			base.Controls.Add(label2);
			base.Controls.Add(_btnCancel);
			base.Controls.Add(_btnOK);
			base.Controls.Add(_rbInsert);
			base.Controls.Add(_rbAppend);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = null;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SelectPastePositionForm";
			base.ShowInTaskbar = false;
			ResumeLayout(false);
		}
	}
}
