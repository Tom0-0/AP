#define DEBUG
using System;
using System.Diagnostics;
using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;

namespace _3S.CoDeSys.DeviceEditor
{
	public class FbInstanceTreeTableNode : ITreeTableNode
	{
		private static readonly Image s_image = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.StructuredParameterSmall.ico").Handle);

		private FbInstanceTreeTableModel _model;

		public static string GVL_IOCONFIG_GLOBALS = "IoConfig_Globals";

		private bool _bCreateVariable;

		private string _stType;

		private IFbInstance _fbInstance;

		public bool HasChildren => false;

		public int ChildCount => 0;

		public ITreeTableNode Parent => null;

		internal FbInstanceTreeTableModel Model => _model;

		internal string Variable => _fbInstance.Instance.Variable;

		internal string Type => _stType;

		internal IFbInstance FbInstance => _fbInstance;

		internal FbInstanceTreeTableNode(IFbInstance instance, FbInstanceTreeTableModel model)
		{
			_model = model;
			_fbInstance = instance;
			_bCreateVariable = instance.Instance.CreateVariable;
			_stType = instance.FbName;
			if (_model.InstanceProvider == null)
			{
				return;
			}
			IDeviceObject host = _model.InstanceProvider.Editor.GetHost();
			IDeviceObject2 val = (IDeviceObject2)(object)((host is IDeviceObject2) ? host : null);
			if (val != null && val.DriverInfo is IDriverInfo8 && (val.DriverInfo as IDriverInfo8).EnableDiagnosis)
			{
				string fbNameDiag = ((IFbInstance3)((instance is IFbInstance3) ? instance : null)).FbNameDiag;
				if (!string.IsNullOrEmpty(fbNameDiag))
				{
					_stType = fbNameDiag;
				}
			}
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Expected O, but got Unknown
			switch (nColumnIndex)
			{
			case 1:
				if (_fbInstance.Instance.Variable.Equals(""))
				{
					return MappingTreeTableViewCellValue.None;
				}
				if (_bCreateVariable)
				{
					return MappingTreeTableViewCellValue.CreateVariable;
				}
				return MappingTreeTableViewCellValue.MapToExistingVariable;
			case 2:
				return _stType;
			case 0:
			{
				string text = _fbInstance.Instance.Variable;
				bool flag = false;
				if (_fbInstance is IFbInstance5)
				{
					FbInstanceTreeTableModel model = _model;
					object obj;
					if (model == null)
					{
						obj = null;
					}
					else
					{
						FbInstanceProvider instanceProvider = model.InstanceProvider;
						if (instanceProvider == null)
						{
							obj = null;
						}
						else
						{
							IOMappingEditor editor = instanceProvider.Editor;
							if (editor == null)
							{
								obj = null;
							}
							else
							{
								IDeviceObject deviceObject = editor.GetDeviceObject(bToModify: false);
								obj = ((deviceObject != null) ? ((IObject)deviceObject).MetaObject.Name : null);
							}
						}
					}
					string text2 = (string)obj;
					IFbInstance fbInstance = _fbInstance;
					string baseName = ((IFbInstance5)((fbInstance is IFbInstance5) ? fbInstance : null)).BaseName;
					if (!string.IsNullOrEmpty(text2))
					{
						baseName = baseName.Replace("$(DeviceName)", text2);
						if (baseName == text)
						{
							flag = true;
						}
					}
				}
				if (APEnvironment.LocalizationManagerOrNull != null && _model.LocalizationActive)
				{
					text = APEnvironment.LocalizationManagerOrNull.GetLocalizedExpression(text, (LocalizationContent)2);
				}
				if (flag)
				{
					return new InstanceIconLabelTreeTableViewCellData(s_image, text, _fbInstance);
				}
				return (object)new IconLabelTreeTableViewCellData(s_image, (object)text);
			}
			default:
				Debug.Fail($"Invalid column index {nColumnIndex}");
				throw new ArgumentException("nColumnIndex");
			}
		}

		public void SetValue(int nColumnIndex, object value)
		{
			if (nColumnIndex == 0)
			{
				string text = ((IconLabelTreeTableViewCellData)((value is IconLabelTreeTableViewCellData) ? value : null)).Label.ToString();
				IIoProvider obj = ((Model.InstanceProvider?.Editor)?.GetParameterSetProvider())?.GetIoProvider(bToModify: true);
				object obj2;
				if (obj == null)
				{
					obj2 = null;
				}
				else
				{
					IMetaObject metaObject = obj.GetMetaObject();
					obj2 = ((metaObject != null) ? metaObject.Name : null);
				}
				string text2 = (string)obj2;
				if (!string.IsNullOrEmpty(text2))
				{
					text = text.Replace("$(DeviceName)", text2);
				}
				IToken val = default(IToken);
				if (((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(text, false, false, false, false).Match((TokenType)13, true, out val) != text.Length)
				{
					throw new Exception(Strings.ErrorInvalidIdentifier);
				}
				MaintenanceFBRenameAction maintenanceFBRenameAction = new MaintenanceFBRenameAction(this, ((IconLabelTreeTableViewCellData)((value is IconLabelTreeTableViewCellData) ? value : null)).Label.ToString());
				_model.UndoManager.AddAction((IUndoableAction)(object)maintenanceFBRenameAction);
				maintenanceFBRenameAction.Redo();
				_model.RaiseChanged(this);
			}
		}

		public int GetIndex(ITreeTableNode node)
		{
			return -1;
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			return null;
		}

		public bool IsEditable(int nColumnIndex)
		{
			if (nColumnIndex == 0)
			{
				return true;
			}
			return false;
		}

		internal RefactoringContextType GetRefactoringContext(out Guid objectGuid, out string stName)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Expected O, but got Unknown
			IDeviceObject obj = _model?.InstanceProvider?.Editor?.GetHost();
			IDeviceObject2 val = (IDeviceObject2)(object)((obj is IDeviceObject2) ? obj : null);
			if (val != null)
			{
				IDriverInfo5 val2 = (IDriverInfo5)val.DriverInfo;
				if (val2 != null)
				{
					IPreCompileContext2 val3 = (IPreCompileContext2)((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(((IDriverInfo2)val2).IoApplication);
					if (val3 != null)
					{
						ISignature[] array = ((IPreCompileContext)val3).FindSignature(GVL_IOCONFIG_GLOBALS);
						if (array.Length != 0)
						{
							ISignature[] array2 = array;
							for (int i = 0; i < array2.Length; i++)
							{
								IVariable[] all = array2[i].All;
								for (int j = 0; j < all.Length; j++)
								{
									if (string.Compare(all[j].Name, _fbInstance.Instance.Variable, StringComparison.InvariantCultureIgnoreCase) == 0)
									{
										stName = _fbInstance.Instance.Variable;
										objectGuid = array[0].ObjectGuid;
										return (RefactoringContextType)2;
									}
								}
							}
						}
					}
				}
			}
			stName = string.Empty;
			objectGuid = Guid.Empty;
			return (RefactoringContextType)0;
		}
	}
}
