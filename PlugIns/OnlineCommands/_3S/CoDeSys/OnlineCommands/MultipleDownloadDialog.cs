#define DEBUG
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.OnlineUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_multiple_download.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Multiple_Download.htm")]
    public class MultipleDownloadDialog : Form
    {
        private List<IMultipleDownloadExtension> _allExtensions = new List<IMultipleDownloadExtension>();

        private IMultipleDownloadReorganizer _reorganizer = APEnvironment.TryCreateFirstMultipleDownloadReorganizer();

        private List<Tuple<Guid, Guid>> _dependencyList = new List<Tuple<Guid, Guid>>();

        private IMultipleDownloadItem[] _selectedItems;

        private OnlineChangeOption _onlineChangeOption;

        private bool _bInitPersistentVars;

        private bool _bDeleteOldApplications;

        private bool _bStartAllApplications;

        private bool _bDoNotReleaseForcedVariables;

        private IContainer components;

        private Label label1;

        private RadioButton _tryOnlineChangeRadioButton;

        private RadioButton _forceOnlineChangeRadioButton;

        private RadioButton _alwaysDownloadRadioButton;

        private CheckBox _startApplicationsCheckBox;

        private CheckBox _deleteApplicationsCheckBox;

        private Button _cancelButton;

        private Button _okButton;

        private Panel _navigatorPanel;

        private CheckedListBox _checkedListBox;

        private ToolStrip _toolStrip;

        private ToolStripButton _btnMoveUp;

        private ToolStripButton _btnMoveDown;

        private CheckBox _doNotReleaseForcedVariablesCheckBox;

        public IMultipleDownloadItem[] SelectedItems => _selectedItems;

        public OnlineChangeOption OnlineChangeOption => _onlineChangeOption;

        public bool InitPersistentVars => _bInitPersistentVars;

        public bool DeleteOldApplications => _bDeleteOldApplications;

        public bool StartAllApplications => _bStartAllApplications;

        public bool DoNotReleaseForcedVariables => _bDoNotReleaseForcedVariables;

        public MultipleDownloadDialog()
        {
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_003d: Expected O, but got Unknown
            InitializeComponent();
            _toolStrip.Renderer = (ToolStripRenderer)new ProfessionalToolStripRenderer();
        }

        private static bool IsMultiDownloadSuppressed(IMetaObjectStub mosApp)
        {
            if (mosApp != null && typeof(ISuppressMultipleDownload).IsAssignableFrom(mosApp.ObjectType))
            {
                return true;
            }
            return false;
        }

        private static bool IsHiddenApplication(IMetaObjectStub mosApp)
        {
            try
            {
                Guid guid = mosApp.ObjectGuid;
                do
                {
                    IMetaObjectStub mosCurrent = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(mosApp.ProjectHandle, guid);
                    if (typeof(IHiddenObject).IsAssignableFrom(mosCurrent.ObjectType))
                    {
                        return true;
                    }
                    if (APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mosCurrent)))
                    {
                        return true;
                    }
                    guid = mosCurrent.ParentObjectGuid;
                }
                while (guid != Guid.Empty);
            }
            catch
            {
            }
            return false;
        }

        private Guid FindParentApplication(IMetaObjectStub stubChild)
        {
            if (stubChild != null)
            {
                try
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(stubChild.ProjectHandle, stubChild.ParentObjectGuid);
                    while (metaObjectStub.ParentObjectGuid != Guid.Empty)
                    {
                        if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                        {
                            if (IsHiddenApplication(metaObjectStub))
                            {
                                return Guid.Empty;
                            }
                            return metaObjectStub.ObjectGuid;
                        }
                        metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(stubChild.ProjectHandle, metaObjectStub.ParentObjectGuid);
                    }
                }
                catch
                {
                }
            }
            return Guid.Empty;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                Cursor = Cursors.WaitCursor;
                _checkedListBox.BeginUpdate();
                int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                List<Guid> list = new List<Guid>();
                Dictionary<Guid, Guid> dictionary = new Dictionary<Guid, Guid>();
                Guid[] multipleDownloadApplications = ProjectOptionsHelper.MultipleDownloadApplications;
                Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle);
                foreach (Guid guid in allObjects)
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid);
                    Debug.Assert(metaObjectStub != null);
                    if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType) && !IsHiddenApplication(metaObjectStub))
                    {
                        dictionary[metaObjectStub.ObjectGuid] = FindParentApplication(metaObjectStub);
                    }
                }
                List<Guid> multipleDownloadSortedApplications = ProjectOptionsHelper.MultipleDownloadSortedApplications;
                List<string> multipleDownloadSortedExtensions = ProjectOptionsHelper.MultipleDownloadSortedExtensions;
                if (multipleDownloadSortedApplications != null && multipleDownloadSortedApplications.Count > 0)
                {
                    for (int num = multipleDownloadSortedApplications.Count - 1; num >= 0; num--)
                    {
                        if (!dictionary.ContainsKey(multipleDownloadSortedApplications[num]))
                        {
                            if (multipleDownloadSortedExtensions != null && multipleDownloadSortedExtensions.Contains(multipleDownloadSortedApplications[num].ToString()))
                            {
                                multipleDownloadSortedExtensions.Remove(multipleDownloadSortedApplications[num].ToString());
                            }
                            multipleDownloadSortedApplications.Remove(multipleDownloadSortedApplications[num]);
                        }
                    }
                    foreach (Guid item4 in multipleDownloadSortedApplications)
                    {
                        list.Add(item4);
                        if (dictionary.ContainsKey(item4))
                        {
                            dictionary.Remove(item4);
                        }
                    }
                }
                List<Guid> list2 = new List<Guid>();
                foreach (KeyValuePair<Guid, Guid> item5 in dictionary)
                {
                    if (item5.Value == Guid.Empty)
                    {
                        list.Add(item5.Key);
                        list2.Add(item5.Key);
                    }
                }
                for (int num2 = list2.Count - 1; num2 >= 0; num2--)
                {
                    dictionary.Remove(list2[num2]);
                }
                int num3 = 0;
                int num4 = 1 + dictionary.Count * (dictionary.Count - 1);
                while (dictionary.Count > 0 && num3 < num4)
                {
                    int num5 = -1;
                    Guid guid2 = Guid.Empty;
                    foreach (KeyValuePair<Guid, Guid> item6 in dictionary)
                    {
                        if (list.Contains(item6.Value))
                        {
                            num5 = list.IndexOf(item6.Value);
                            guid2 = item6.Key;
                            break;
                        }
                    }
                    if (num5 > -1)
                    {
                        if (num5 == list.Count - 1)
                        {
                            list.Add(guid2);
                        }
                        else
                        {
                            list.Insert(num5 + 1, guid2);
                        }
                        dictionary.Remove(guid2);
                    }
                    num3++;
                }
                List<IMultipleDownloadExtension> list3 = new List<IMultipleDownloadExtension>();
                foreach (IMultipleDownloadExtensionProvider multipleDownloadExtensionProvider in APEnvironment.MultipleDownloadExtensionProviders)
                {
                    IMultipleDownloadExtension[] extensions = multipleDownloadExtensionProvider.GetExtensions();
                    if (extensions != null)
                    {
                        list3.AddRange(extensions);
                    }
                }
                _allExtensions = new List<IMultipleDownloadExtension>(list3);
                if (list3.Count > 0 && multipleDownloadSortedExtensions != null)
                {
                    for (int num6 = multipleDownloadSortedExtensions.Count - 1; num6 >= 0; num6--)
                    {
                        if (!(multipleDownloadSortedExtensions[num6] == MultipleDownloadCommand.DUMMY) && multipleDownloadSortedExtensions[num6].Contains("@"))
                        {
                            string text = multipleDownloadSortedExtensions[num6].Substring(multipleDownloadSortedExtensions[num6].IndexOf("@") + 1).ToLowerInvariant();
                            bool flag = false;
                            for (int num7 = list3.Count - 1; num7 >= 0; num7--)
                            {
                                if (text == list3[num7].Name.ToLowerInvariant())
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                multipleDownloadSortedExtensions.RemoveAt(num6);
                            }
                        }
                    }
                }
                _dependencyList.Clear();
                foreach (Guid item7 in list)
                {
                    IList<IOnlineApplicationObject> childApplicationObjects = OnlineCommandHelper.GetChildApplicationObjects(item7);
                    if (childApplicationObjects == null || childApplicationObjects.Count <= 0)
                    {
                        continue;
                    }
                    foreach (IOnlineApplicationObject item8 in childApplicationObjects)
                    {
                        foreach (Guid item9 in list)
                        {
                            if (item8.ApplicationGuid == item9)
                            {
                                _dependencyList.Add(Tuple.Create(item7, item9));
                            }
                        }
                    }
                }
                foreach (Tuple<Guid, Guid> dependency in _dependencyList)
                {
                    int num8 = list.IndexOf(dependency.Item1);
                    int num9 = list.IndexOf(dependency.Item2);
                    if (num8 > -1 && num9 > -1 && num8 < num9)
                    {
                        list.Remove(dependency.Item1);
                        int index = 1 + list.IndexOf(dependency.Item2);
                        list.Insert(index, dependency.Item1);
                    }
                }
                foreach (Guid item10 in list)
                {
                    IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, item10);
                    if (!IsHiddenApplication(metaObjectStub2) && !IsMultiDownloadSuppressed(metaObjectStub2))
                    {
                        ApplicationItem item = new ApplicationItem(metaObjectStub2);
                        int index2 = _checkedListBox.Items.Add(item);
                        if (multipleDownloadApplications == null || Array.IndexOf(multipleDownloadApplications, metaObjectStub2.ObjectGuid) >= 0)
                        {
                            _checkedListBox.SetItemChecked(index2, value: true);
                        }
                    }
                }
                if (list3.Count > 0)
                {
                    if (multipleDownloadSortedExtensions != null && multipleDownloadSortedExtensions.Count > 0)
                    {
                        for (int j = 0; j < multipleDownloadSortedExtensions.Count; j++)
                        {
                            if (multipleDownloadSortedExtensions[j] == MultipleDownloadCommand.DUMMY || !multipleDownloadSortedExtensions[j].Contains("@"))
                            {
                                continue;
                            }
                            string[] array = multipleDownloadSortedExtensions[j].Split('@');
                            if (array.Length != 2)
                            {
                                continue;
                            }
                            bool value = array[0] == MultipleDownloadCommand.SELECTED;
                            string text2 = array[1].ToLowerInvariant();
                            for (int num10 = list3.Count - 1; num10 >= 0; num10--)
                            {
                                if (text2 == list3[num10].Name.ToLowerInvariant())
                                {
                                    ExtensionItem item2 = new ExtensionItem(list3[num10]);
                                    int index3 = j;
                                    if (_checkedListBox.Items.Count > j)
                                    {
                                        _checkedListBox.Items.Insert(j, item2);
                                    }
                                    else
                                    {
                                        index3 = _checkedListBox.Items.Add(item2);
                                    }
                                    _checkedListBox.SetItemChecked(index3, value);
                                    list3.RemoveAt(num10);
                                    break;
                                }
                            }
                        }
                    }
                    foreach (IMultipleDownloadExtension item11 in list3)
                    {
                        ExtensionItem item3 = new ExtensionItem(item11);
                        int index4 = _checkedListBox.Items.Add(item3);
                        _checkedListBox.SetItemChecked(index4, item11.Selected);
                    }
                }
                _tryOnlineChangeRadioButton.Checked = ProjectOptionsHelper.MultipleDownloadOnlineChangeOption == 0;
                _forceOnlineChangeRadioButton.Checked = ProjectOptionsHelper.MultipleDownloadOnlineChangeOption == 1;
                _alwaysDownloadRadioButton.Checked = ProjectOptionsHelper.MultipleDownloadOnlineChangeOption == 2;
                _deleteApplicationsCheckBox.Checked = ProjectOptionsHelper.MultipleDownloadDeleteOldApps;
                _startApplicationsCheckBox.Checked = ProjectOptionsHelper.MultipleDownloadStartAllApps;
                EvaluateKeepForcesCheckboxState();
                if (GetSelectedItems(null).Count == 0)
                {
                    for (int k = 0; k < _checkedListBox.Items.Count; k++)
                    {
                        _checkedListBox.SetItemChecked(k, value: true);
                    }
                }
                CheckOrder(bIsDuringLoad: true);
            }
            finally
            {
                _checkedListBox.EndUpdate();
                _btnMoveUp.Enabled = false;
                _btnMoveDown.Enabled = false;
                Cursor = Cursors.Default;
                UpdateControlStates();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _alwaysDownloadRadioButton.CheckedChanged += OnAlwaysDownloadRadioButtonCheckedChanged;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            _alwaysDownloadRadioButton.CheckedChanged -= OnAlwaysDownloadRadioButtonCheckedChanged;
        }

        private void OnAlwaysDownloadRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            EvaluateKeepForcesCheckboxState();
        }

        private void EvaluateKeepForcesCheckboxState()
        {
            if (_alwaysDownloadRadioButton.Checked)
            {
                _doNotReleaseForcedVariablesCheckBox.Checked = false;
                _doNotReleaseForcedVariablesCheckBox.Enabled = false;
            }
            else
            {
                _doNotReleaseForcedVariablesCheckBox.Enabled = true;
                _doNotReleaseForcedVariablesCheckBox.Checked = ProjectOptionsHelper.MultipleDownloadDoNotReleaseForcedVariables;
            }
        }

        private List<IMultipleDownloadItem> GetSelectedItems(ItemCheckEventArgs e)
        {
            List<IMultipleDownloadItem> list = new List<IMultipleDownloadItem>();
            for (int i = 0; i < _checkedListBox.Items.Count; i++)
            {
                if (e != null && e.Index == i)
                {
                    if (e.NewValue == CheckState.Checked)
                    {
                        if (_checkedListBox.Items[i] is ApplicationItem)
                        {
                            ApplicationMultipleDownloadItem applicationMultipleDownloadItem = new ApplicationMultipleDownloadItem();
                            applicationMultipleDownloadItem.ApplicationGuid = ((ApplicationItem)_checkedListBox.Items[i]).ObjectGuid;
                            list.Add(applicationMultipleDownloadItem);
                        }
                        else if (_checkedListBox.Items[i] is ExtensionItem)
                        {
                            ExtensionMultipleDownloadItem extensionMultipleDownloadItem = new ExtensionMultipleDownloadItem();
                            extensionMultipleDownloadItem.Extension = ((ExtensionItem)_checkedListBox.Items[i]).Extension;
                            list.Add(extensionMultipleDownloadItem);
                        }
                    }
                }
                else if (_checkedListBox.GetItemChecked(i))
                {
                    if (_checkedListBox.Items[i] is ApplicationItem)
                    {
                        ApplicationMultipleDownloadItem applicationMultipleDownloadItem2 = new ApplicationMultipleDownloadItem();
                        applicationMultipleDownloadItem2.ApplicationGuid = ((ApplicationItem)_checkedListBox.Items[i]).ObjectGuid;
                        list.Add(applicationMultipleDownloadItem2);
                    }
                    else if (_checkedListBox.Items[i] is ExtensionItem)
                    {
                        ExtensionMultipleDownloadItem extensionMultipleDownloadItem2 = new ExtensionMultipleDownloadItem();
                        extensionMultipleDownloadItem2.Extension = ((ExtensionItem)_checkedListBox.Items[i]).Extension;
                        list.Add(extensionMultipleDownloadItem2);
                    }
                }
            }
            return list;
        }

        private void UpdateControlStates()
        {
            _okButton.Enabled = GetSelectedItems(null).Count > 0;
        }

        private void _checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _okButton.Enabled = GetSelectedItems(e).Count > 0;
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            //IL_005b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0071: Unknown result type (might be due to invalid IL or missing references)
            //IL_0087: Unknown result type (might be due to invalid IL or missing references)
            //IL_0328: Unknown result type (might be due to invalid IL or missing references)
            //IL_032d: Unknown result type (might be due to invalid IL or missing references)
            //IL_032f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0342: Expected I4, but got Unknown
            //IL_038e: Unknown result type (might be due to invalid IL or missing references)
            _selectedItems = GetSelectedItems(null).ToArray();
            _bDeleteOldApplications = _deleteApplicationsCheckBox.Checked;
            _bInitPersistentVars = true;
            _bStartAllApplications = _startApplicationsCheckBox.Checked;
            _bDoNotReleaseForcedVariables = _doNotReleaseForcedVariablesCheckBox.Checked;
            if (_tryOnlineChangeRadioButton.Checked)
            {
                _onlineChangeOption = (OnlineChangeOption)1;
            }
            else if (_forceOnlineChangeRadioButton.Checked)
            {
                _onlineChangeOption = (OnlineChangeOption)2;
            }
            else if (_alwaysDownloadRadioButton.Checked)
            {
                _onlineChangeOption = (OnlineChangeOption)0;
            }
            List<Guid> list = new List<Guid>();
            List<string> list2 = new List<string>();
            foreach (object item in _checkedListBox.Items)
            {
                if (item is ApplicationItem)
                {
                    list.Add(((ApplicationItem)item).ObjectGuid);
                    list2.Add(((ApplicationItem)item).ObjectGuid.ToString());
                }
                else if (item is ExtensionItem)
                {
                    list2.Add(MultipleDownloadCommand.UNSELECTED + "@" + ((ExtensionItem)item).Extension.Name);
                }
            }
            List<Guid> list3 = new List<Guid>();
            List<IMultipleDownloadExtension> list4 = new List<IMultipleDownloadExtension>();
            for (int i = 0; i < _selectedItems.Length; i++)
            {
                if (_selectedItems[i] is ApplicationMultipleDownloadItem)
                {
                    list3.Add(((ApplicationMultipleDownloadItem)_selectedItems[i]).ApplicationGuid);
                }
                else if (_selectedItems[i] is ExtensionMultipleDownloadItem)
                {
                    list4.Add(((ExtensionMultipleDownloadItem)_selectedItems[i]).Extension);
                    if (list2.Contains(MultipleDownloadCommand.UNSELECTED + "@" + ((ExtensionMultipleDownloadItem)_selectedItems[i]).Extension.Name))
                    {
                        int index = list2.IndexOf(MultipleDownloadCommand.UNSELECTED + "@" + ((ExtensionMultipleDownloadItem)_selectedItems[i]).Extension.Name);
                        list2[index] = MultipleDownloadCommand.SELECTED + "@" + ((ExtensionMultipleDownloadItem)_selectedItems[i]).Extension.Name;
                    }
                }
            }
            bool flag = false;
            List<Guid> multipleDownloadSortedApplications = ProjectOptionsHelper.MultipleDownloadSortedApplications;
            List<string> multipleDownloadSortedExtensions = ProjectOptionsHelper.MultipleDownloadSortedExtensions;
            Guid[] multipleDownloadApplications = ProjectOptionsHelper.MultipleDownloadApplications;
            int multipleDownloadOnlineChangeOption = ProjectOptionsHelper.MultipleDownloadOnlineChangeOption;
            if (multipleDownloadSortedApplications == null || !multipleDownloadSortedApplications.SequenceEqual(list))
            {
                ProjectOptionsHelper.MultipleDownloadSortedApplications = list;
                flag = true;
            }
            if (multipleDownloadSortedExtensions == null || !multipleDownloadSortedExtensions.SequenceEqual(list2))
            {
                ProjectOptionsHelper.MultipleDownloadSortedExtensions = list2;
                flag = true;
            }
            if (multipleDownloadApplications == null || !multipleDownloadApplications.SequenceEqual(list3))
            {
                ProjectOptionsHelper.MultipleDownloadApplications = list3.ToArray();
                flag = true;
            }
            if (ProjectOptionsHelper.MultipleDownloadDeleteOldApps != _bDeleteOldApplications)
            {
                ProjectOptionsHelper.MultipleDownloadDeleteOldApps = _bDeleteOldApplications;
                flag = true;
            }
            if (ProjectOptionsHelper.MultipleDownloadInitPersistentVars != _bInitPersistentVars)
            {
                ProjectOptionsHelper.MultipleDownloadInitPersistentVars = _bInitPersistentVars;
                flag = true;
            }
            if (ProjectOptionsHelper.MultipleDownloadStartAllApps != _bStartAllApplications)
            {
                ProjectOptionsHelper.MultipleDownloadStartAllApps = _bStartAllApplications;
                flag = true;
            }
            if (ProjectOptionsHelper.MultipleDownloadDoNotReleaseForcedVariables != _bDoNotReleaseForcedVariables)
            {
                ProjectOptionsHelper.MultipleDownloadDoNotReleaseForcedVariables = _bDoNotReleaseForcedVariables;
                flag = true;
            }
            OnlineChangeOption onlineChangeOption = _onlineChangeOption;
            switch ((int)onlineChangeOption)
            {
                case 1:
                    if (multipleDownloadOnlineChangeOption != 0)
                    {
                        ProjectOptionsHelper.MultipleDownloadOnlineChangeOption = 0;
                        flag = true;
                    }
                    break;
                case 2:
                    if (multipleDownloadOnlineChangeOption != 1)
                    {
                        ProjectOptionsHelper.MultipleDownloadOnlineChangeOption = 1;
                        flag = true;
                    }
                    break;
                case 0:
                    if (multipleDownloadOnlineChangeOption != 2)
                    {
                        ProjectOptionsHelper.MultipleDownloadOnlineChangeOption = 2;
                        flag = true;
                    }
                    break;
            }
            MultipleDownloadContext multipleDownloadContext = new MultipleDownloadContext(list3, list4, _bInitPersistentVars, _bDeleteOldApplications, _bStartAllApplications, _bDoNotReleaseForcedVariables, bThrowIfAborted: false, bOmitResultDialog: false, _onlineChangeOption);
            foreach (IMultipleDownloadExtension allExtension in _allExtensions)
            {
                try
                {
                    allExtension.Check((IMultipleDownloadContext)(object)multipleDownloadContext);
                }
                catch (Exception ex)
                {
                    APEnvironment.MessageService.Error(ex.Message, "ErrorCheckingMultiDownloadExtensions", Array.Empty<object>());
                    base.DialogResult = DialogResult.None;
                    return;
                }
            }
            if (flag)
            {
                ProjectOptionsHelper.SaveOptions();
            }
        }

        private void _btnMoveUp_Click(object sender, EventArgs e)
        {
            if (_checkedListBox.Items == null || _checkedListBox.SelectedIndex <= 0)
            {
                return;
            }
            object selectedItem = _checkedListBox.SelectedItem;
            int num = _checkedListBox.SelectedIndex - 1;
            object obj = _checkedListBox.Items[num];
            if (IsMovingItemAllowed((IMultipleDownloadCheckBoxItem)((selectedItem is IMultipleDownloadCheckBoxItem) ? selectedItem : null), (IMultipleDownloadCheckBoxItem)((obj is IMultipleDownloadCheckBoxItem) ? obj : null)))
            {
                _checkedListBox.BeginUpdate();
                bool num2 = _checkedListBox.GetItemCheckState(_checkedListBox.SelectedIndex) == CheckState.Checked;
                _checkedListBox.Items.Remove(selectedItem);
                _checkedListBox.Items.Insert(num, selectedItem);
                _checkedListBox.SelectedIndex = num;
                if (num2)
                {
                    _checkedListBox.SetItemCheckState(num, CheckState.Checked);
                }
                CheckOrder(bIsDuringLoad: false);
                _checkedListBox.EndUpdate();
            }
        }

        private void _btnMoveDown_Click(object sender, EventArgs e)
        {
            if (_checkedListBox.Items == null || _checkedListBox.SelectedIndex >= _checkedListBox.Items.Count)
            {
                return;
            }
            int num = _checkedListBox.SelectedIndex + 1;
            object selectedItem = _checkedListBox.SelectedItem;
            object obj = _checkedListBox.Items[num];
            if (IsMovingItemAllowed((IMultipleDownloadCheckBoxItem)((selectedItem is IMultipleDownloadCheckBoxItem) ? selectedItem : null), (IMultipleDownloadCheckBoxItem)((obj is IMultipleDownloadCheckBoxItem) ? obj : null)))
            {
                _checkedListBox.BeginUpdate();
                bool num2 = _checkedListBox.GetItemCheckState(_checkedListBox.SelectedIndex) == CheckState.Checked;
                _checkedListBox.Items.Remove(selectedItem);
                _checkedListBox.Items.Insert(num, selectedItem);
                _checkedListBox.SelectedIndex = num;
                if (num2)
                {
                    _checkedListBox.SetItemCheckState(num, CheckState.Checked);
                }
                CheckOrder(bIsDuringLoad: false);
                _checkedListBox.EndUpdate();
            }
        }

        private void _checkedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_checkedListBox.Items != null && _checkedListBox.Items.Count > 1)
            {
                _btnMoveUp.Enabled = true;
                _btnMoveDown.Enabled = true;
                int selectedIndex = _checkedListBox.SelectedIndex;
                if (selectedIndex == 0)
                {
                    _btnMoveUp.Enabled = false;
                }
                else if (selectedIndex == _checkedListBox.Items.Count - 1)
                {
                    _btnMoveDown.Enabled = false;
                }
                object selectedItem = _checkedListBox.SelectedItem;
                int num = selectedIndex - 1;
                int num2 = selectedIndex + 1;
                if (num >= 0)
                {
                    object obj = _checkedListBox.Items[num];
                    if (!IsMovingItemAllowed((IMultipleDownloadCheckBoxItem)((selectedItem is IMultipleDownloadCheckBoxItem) ? selectedItem : null), (IMultipleDownloadCheckBoxItem)((obj is IMultipleDownloadCheckBoxItem) ? obj : null)))
                    {
                        _btnMoveUp.Enabled = false;
                    }
                }
                if (num2 < _checkedListBox.Items.Count)
                {
                    object obj2 = _checkedListBox.Items[num2];
                    if (!IsMovingItemAllowed((IMultipleDownloadCheckBoxItem)((selectedItem is IMultipleDownloadCheckBoxItem) ? selectedItem : null), (IMultipleDownloadCheckBoxItem)((obj2 is IMultipleDownloadCheckBoxItem) ? obj2 : null)))
                    {
                        _btnMoveDown.Enabled = false;
                    }
                }
            }
            else
            {
                _btnMoveUp.Enabled = false;
                _btnMoveDown.Enabled = false;
            }
        }

        private bool IsMovingItemAllowed(IMultipleDownloadCheckBoxItem checkitemCurrent, IMultipleDownloadCheckBoxItem checkitemNew)
        {
            if (checkitemCurrent != null && checkitemNew != null)
            {
                foreach (Tuple<Guid, Guid> dependency in _dependencyList)
                {
                    if (checkitemCurrent.ApplicationGuid == dependency.Item1 && checkitemNew.ApplicationGuid == dependency.Item2)
                    {
                        return false;
                    }
                    if (checkitemCurrent.ApplicationGuid == dependency.Item2 && checkitemNew.ApplicationGuid == dependency.Item1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void CheckOrder(bool bIsDuringLoad)
        {
            //IL_0047: Unknown result type (might be due to invalid IL or missing references)
            if (_reorganizer == null || _checkedListBox.Items.Count <= 1)
            {
                return;
            }
            int num = -1;
            try
            {
                List<IMultipleDownloadCheckBoxItem> list = new List<IMultipleDownloadCheckBoxItem>();
                string text = string.Empty;
                if (_checkedListBox.SelectedItem != null)
                {
                    text = ((IMultipleDownloadCheckBoxItem)_checkedListBox.SelectedItem).VisibleName;
                }
                for (int i = 0; i < _checkedListBox.Items.Count; i++)
                {
                    object obj = _checkedListBox.Items[i];
                    list.Add((IMultipleDownloadCheckBoxItem)((obj is IMultipleDownloadCheckBoxItem) ? obj : null));
                    if (_checkedListBox.CheckedItems.Contains(_checkedListBox.Items[i]))
                    {
                        list[i].IsChecked = (true);
                    }
                    else
                    {
                        list[i].IsChecked = (false);
                    }
                }
                if (_reorganizer.CheckOrder((IList<IMultipleDownloadCheckBoxItem>)list, bIsDuringLoad) && list.Count == _checkedListBox.Items.Count)
                {
                    _checkedListBox.Items.Clear();
                    for (int j = 0; j < list.Count; j++)
                    {
                        _checkedListBox.Items.Add(list[j]);
                        if (list[j].IsChecked)
                        {
                            _checkedListBox.SetItemCheckState(j, CheckState.Checked);
                        }
                        else
                        {
                            _checkedListBox.SetItemCheckState(j, CheckState.Unchecked);
                        }
                        if (list[j].VisibleName == text)
                        {
                            num = j;
                        }
                    }
                }
                if (num > -1)
                {
                    _checkedListBox.SelectedItem = list[num];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MultiDownloadReordering failed: " + ex.Message);
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.MultipleDownloadDialog));
            _alwaysDownloadRadioButton = new System.Windows.Forms.RadioButton();
            _forceOnlineChangeRadioButton = new System.Windows.Forms.RadioButton();
            _tryOnlineChangeRadioButton = new System.Windows.Forms.RadioButton();
            _doNotReleaseForcedVariablesCheckBox = new System.Windows.Forms.CheckBox();
            _startApplicationsCheckBox = new System.Windows.Forms.CheckBox();
            _deleteApplicationsCheckBox = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            _cancelButton = new System.Windows.Forms.Button();
            _okButton = new System.Windows.Forms.Button();
            _navigatorPanel = new System.Windows.Forms.Panel();
            _toolStrip = new System.Windows.Forms.ToolStrip();
            _btnMoveUp = new System.Windows.Forms.ToolStripButton();
            _btnMoveDown = new System.Windows.Forms.ToolStripButton();
            _checkedListBox = new System.Windows.Forms.CheckedListBox();
            System.Windows.Forms.GroupBox groupBox = new System.Windows.Forms.GroupBox();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
            System.Windows.Forms.GroupBox groupBox2 = new System.Windows.Forms.GroupBox();
            groupBox.SuspendLayout();
            groupBox2.SuspendLayout();
            _navigatorPanel.SuspendLayout();
            _toolStrip.SuspendLayout();
            SuspendLayout();
            groupBox.Controls.Add(label);
            groupBox.Controls.Add(_alwaysDownloadRadioButton);
            groupBox.Controls.Add(_forceOnlineChangeRadioButton);
            groupBox.Controls.Add(label2);
            groupBox.Controls.Add(_tryOnlineChangeRadioButton);
            resources.ApplyResources(groupBox, "groupBox1");
            groupBox.Name = "groupBox1";
            groupBox.TabStop = false;
            resources.ApplyResources(label, "label3");
            label.Name = "label3";
            resources.ApplyResources(_alwaysDownloadRadioButton, "_alwaysDownloadRadioButton");
            _alwaysDownloadRadioButton.Name = "_alwaysDownloadRadioButton";
            _alwaysDownloadRadioButton.TabStop = true;
            _alwaysDownloadRadioButton.UseVisualStyleBackColor = true;
            resources.ApplyResources(_forceOnlineChangeRadioButton, "_forceOnlineChangeRadioButton");
            _forceOnlineChangeRadioButton.Name = "_forceOnlineChangeRadioButton";
            _forceOnlineChangeRadioButton.TabStop = true;
            _forceOnlineChangeRadioButton.UseVisualStyleBackColor = true;
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            resources.ApplyResources(_tryOnlineChangeRadioButton, "_tryOnlineChangeRadioButton");
            _tryOnlineChangeRadioButton.Name = "_tryOnlineChangeRadioButton";
            _tryOnlineChangeRadioButton.TabStop = true;
            _tryOnlineChangeRadioButton.UseVisualStyleBackColor = true;
            groupBox2.Controls.Add(_doNotReleaseForcedVariablesCheckBox);
            groupBox2.Controls.Add(_startApplicationsCheckBox);
            groupBox2.Controls.Add(_deleteApplicationsCheckBox);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            resources.ApplyResources(_doNotReleaseForcedVariablesCheckBox, "_doNotReleaseForcedVariablesCheckBox");
            _doNotReleaseForcedVariablesCheckBox.Name = "_doNotReleaseForcedVariablesCheckBox";
            _doNotReleaseForcedVariablesCheckBox.UseVisualStyleBackColor = true;
            resources.ApplyResources(_startApplicationsCheckBox, "_startApplicationsCheckBox");
            _startApplicationsCheckBox.Name = "_startApplicationsCheckBox";
            _startApplicationsCheckBox.UseVisualStyleBackColor = true;
            resources.ApplyResources(_deleteApplicationsCheckBox, "_deleteApplicationsCheckBox");
            _deleteApplicationsCheckBox.Name = "_deleteApplicationsCheckBox";
            _deleteApplicationsCheckBox.UseVisualStyleBackColor = true;
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            _cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(_cancelButton, "_cancelButton");
            _cancelButton.Name = "_cancelButton";
            _cancelButton.UseVisualStyleBackColor = true;
            _okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(_okButton, "_okButton");
            _okButton.Name = "_okButton";
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += new System.EventHandler(_okButton_Click);
            _navigatorPanel.Controls.Add(_toolStrip);
            _navigatorPanel.Controls.Add(_checkedListBox);
            resources.ApplyResources(_navigatorPanel, "_navigatorPanel");
            _navigatorPanel.Name = "_navigatorPanel";
            _toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            _toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            _toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { _btnMoveUp, _btnMoveDown });
            _toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(_toolStrip, "_toolStrip");
            _toolStrip.Name = "_toolStrip";
            resources.ApplyResources(_btnMoveUp, "_btnMoveUp");
            _btnMoveUp.Name = "_btnMoveUp";
            _btnMoveUp.Click += new System.EventHandler(_btnMoveUp_Click);
            resources.ApplyResources(_btnMoveDown, "_btnMoveDown");
            _btnMoveDown.Name = "_btnMoveDown";
            _btnMoveDown.Click += new System.EventHandler(_btnMoveDown_Click);
            resources.ApplyResources(_checkedListBox, "_checkedListBox");
            _checkedListBox.FormattingEnabled = true;
            _checkedListBox.Name = "_checkedListBox";
            _checkedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(_checkedListBox_ItemCheck);
            _checkedListBox.SelectedIndexChanged += new System.EventHandler(_checkedListBox_SelectedIndexChanged);
            base.AcceptButton = _okButton;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = _cancelButton;
            base.Controls.Add(_navigatorPanel);
            base.Controls.Add(_okButton);
            base.Controls.Add(_cancelButton);
            base.Controls.Add(groupBox2);
            base.Controls.Add(groupBox);
            base.Controls.Add(label1);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "MultipleDownloadDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            groupBox.ResumeLayout(false);
            groupBox.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            _navigatorPanel.ResumeLayout(false);
            _navigatorPanel.PerformLayout();
            _toolStrip.ResumeLayout(false);
            _toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
