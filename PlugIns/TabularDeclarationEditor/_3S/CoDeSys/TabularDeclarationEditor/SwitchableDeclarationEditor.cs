#define DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.AutoDeclare;
using _3S.CoDeSys.Bookmarks;
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Printing;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{24CB8130-E4D8-411d-82DB-ACBCDD8CDA8C}")]
	[SuppressMessage("AP.Patterns", "Pat002", Justification = "No F1 help required")]
	public class SwitchableDeclarationEditor : UserControl, ISwitchableDeclarationEditor3, ISwitchableDeclarationEditor2, ISwitchableDeclarationEditor, IEditorView, IView, IEditor, IHasOnlineMode, IHasBookmarks, IPrintable, IEditorBasedFindReplace, INotifyOnVisibilityChanged, IRefactoringCommandContext, ISupportsMultiSelection, ILocalizableEditor, ISupportOnlineInstancePreselection, IAutoDeclarePostProcessor
	{
		private IIECTextEditor _text;

		private TabularDeclarationEditor _table;

		private SwitchableDeclarationEditorMode _mode;

		private bool _objectSet;

		private bool _bThemesEnabled;

		private IContainer components;

		private Panel _panel;

		private ToolStrip _toolBar;

		private ToolStripButton _textualButtonItem;

		private ToolStripButton _tabularButtonItem;

		public SwitchableDeclarationEditorMode Mode
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _mode;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Invalid comparison between Unknown and I4
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				_mode = value;
				_panel.Controls.Clear();
				if (_objectSet)
				{
					Save(bCommit: true);
				}
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						_panel.Controls.Add(_table.Control);
						_textualButtonItem.Checked = false;
						_tabularButtonItem.Checked = true;
						_table.Control.Focus();
					}
					else
					{
						Debug.Fail("Unknown mode.");
					}
				}
				else
				{
					_panel.Controls.Add(((IView)_text).Control);
					_textualButtonItem.Checked = true;
					_tabularButtonItem.Checked = false;
					((IView)_text).Control.Focus();
				}
				if (_objectSet)
				{
					Reload();
				}
				OptionHelper.SetMode(((IEditorView)_text).Editor.ObjectGuid, _mode);
			}
		}

		public IEditor Editor
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.Editor;
					}
					Debug.Fail("Unknown mode.");
					return null;
				}
				return ((IEditorView)_text).Editor;
			}
		}

		public Control Control => this;

		public Control[] Panes
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return new Control[1] { _table.Control };
					}
					Debug.Fail("Unknown mode.");
					return null;
				}
				return new Control[1] { ((IView)_text).Control };
			}
		}

		public string Caption
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.Caption;
					}
					Debug.Fail("Unknown mode.");
					return string.Empty;
				}
				return ((IView)_text).Caption;
			}
		}

		public string Description
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.Description;
					}
					Debug.Fail("Unknown mode.");
					return string.Empty;
				}
				return ((IView)_text).Description;
			}
		}

		public Icon SmallIcon
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.SmallIcon;
					}
					Debug.Fail("Unknown mode.");
					return null;
				}
				return ((IView)_text).SmallIcon;
			}
		}

		public Icon LargeIcon
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.LargeIcon;
					}
					Debug.Fail("Unknown mode.");
					return null;
				}
				return ((IView)_text).LargeIcon;
			}
		}

		public DockingPosition DefaultDockingPosition
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.DefaultDockingPosition;
					}
					Debug.Fail("Unknown mode.");
					return (DockingPosition)4;
				}
				return ((IView)_text).DefaultDockingPosition;
			}
		}

		public DockingPosition PossibleDockingPositions
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.PossibleDockingPositions;
					}
					Debug.Fail("Unknown mode.");
					return (DockingPosition)63;
				}
				return ((IView)_text).PossibleDockingPositions;
			}
		}

		public int ProjectHandle
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.ProjectHandle;
					}
					Debug.Fail("Unknown mode.");
					return -1;
				}
				return ((IEditor)_text).ProjectHandle;
			}
		}

		public Guid ObjectGuid
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.ObjectGuid;
					}
					Debug.Fail("Unknown mode.");
					return Guid.Empty;
				}
				return ((IEditor)_text).ObjectGuid;
			}
		}

		public OnlineState OnlineState
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				SwitchableDeclarationEditorMode mode = _mode;
				if ((int)mode != 0)
				{
					if ((int)mode == 1)
					{
						return _table.OnlineState;
					}
					Debug.Fail("Unknown mode.");
					return default(OnlineState);
				}
				if (_text is IHasOnlineMode)
				{
					return ((IHasOnlineMode)_text).OnlineState;
				}
				return default(OnlineState);
			}
			set
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				if (_text is IHasOnlineMode)
				{
					((IHasOnlineMode)_text).OnlineState=(value);
				}
				_table.OnlineState = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				if (!(_text is IIECTextEditor2) || !((IIECTextEditor2)_text).ReadOnly)
				{
					if (_table != null)
					{
						return ((ITabularDeclarationEditor2)_table).ReadOnly;
					}
					return false;
				}
				return true;
			}
			set
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				if (_text is IIECTextEditor2)
				{
					((IIECTextEditor2)_text).ReadOnly=(value);
				}
				if (_table != null)
				{
					((ITabularDeclarationEditor2)_table).ReadOnly=(value);
				}
			}
		}

		public bool EnableThemes
		{
			get
			{
				return _bThemesEnabled;
			}
			set
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				_bThemesEnabled = value;
				if (_text is IIECTextEditor8)
				{
					((IIECTextEditor8)_text).EnableThemes=(_bThemesEnabled);
				}
			}
		}

		public SwitchableDeclarationEditor()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			InitializeComponent();
			_text = APEnvironment.CreateIECTextEditor();
			((IView)_text).Control.Dock = DockStyle.Fill;
			_table = new TabularDeclarationEditor();
			_table.Control.Dock = DockStyle.Fill;
			_toolBar.Renderer = (ToolStripRenderer)new ProfessionalToolStripRenderer();
		}

		public void Initialize(IEditor editor, Icon smallIcon, Icon largeIcon, ITextDocumentProvider textDocumentProvider, UnresolvedDeclarationContext context)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			_text.Initialize(editor, smallIcon, largeIcon, textDocumentProvider, (MonitoringMode)2, false);
			_table.Initialize(editor, smallIcon, largeIcon, textDocumentProvider, context);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
				if (_text is IDisposable)
				{
					((IDisposable)_text).Dispose();
				}
				if (_table != null)
				{
					((IDisposable)_table).Dispose();
				}
			}
			base.Dispose(disposing);
		}

		public void Mark(long nPosition, int nLength, object tag)
		{
			((IEditorView)_text).Mark(nPosition, nLength, tag);
			_table.Mark(nPosition, nLength, tag);
		}

		public void UnmarkAll(object tag)
		{
			((IEditorView)_text).UnmarkAll(tag);
			_table.UnmarkAll(tag);
		}

		public void Select(long nPosition, int nLength)
		{
			((IEditorView)_text).Select(nPosition, nLength);
			_table.Select(nPosition, nLength);
		}

		public void GetSelection(out long nPosition, out int nLength)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					_table.GetSelection(out nPosition, out nLength);
					return;
				}
				Debug.Fail("Unknown mode.");
				nPosition = -1L;
				nLength = 0;
			}
			else
			{
				((IEditorView)_text).GetSelection(out nPosition, out nLength);
			}
		}

		public int ComparePositions(long nPosition1, long nPosition2)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					return _table.ComparePositions(nPosition1, nPosition2);
				}
				Debug.Fail("Unknown mode.");
				return 0;
			}
			return ((IEditorView)_text).ComparePositions(nPosition1, nPosition2);
		}

		public bool CanExecuteStandardCommand(Guid commandGuid)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					return _table.CanExecuteStandardCommand(commandGuid);
				}
				Debug.Fail("Unknown mode.");
				return false;
			}
			return ((IView)_text).CanExecuteStandardCommand(commandGuid);
		}

		public void ExecuteStandardCommand(Guid commandGuid)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					_table.ExecuteStandardCommand(commandGuid);
				}
				else
				{
					Debug.Fail("Unknown mode.");
				}
			}
			else
			{
				((IView)_text).ExecuteStandardCommand(commandGuid);
			}
		}

		public void SetObject(int nProjectHandle, Guid objectGuid)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			((IEditor)_text).SetObject(nProjectHandle, objectGuid);
			_table.SetObject(nProjectHandle, objectGuid);
			Mode = OptionHelper.GetMode(objectGuid);
			_objectSet = true;
		}

		public void Reload()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Invalid comparison between Unknown and I4
			if ((int)Mode == 0)
			{
				bool readOnly = false;
				if (_text is IIECTextEditor2)
				{
					readOnly = ((IIECTextEditor2)_text).ReadOnly;
				}
				((IEditor)_text).Reload();
				if (_text is IIECTextEditor2)
				{
					((IIECTextEditor2)_text).ReadOnly=(readOnly);
				}
			}
			if ((int)Mode == 1)
			{
				_table.Reload();
			}
		}

		public void Save(bool bCommit)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					_table.Save(bCommit);
				}
				else
				{
					Debug.Fail("Unknown mode.");
				}
			}
			else
			{
				((IEditor)_text).Save(bCommit);
			}
		}

		public IMetaObject GetObjectToRead()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					return _table.GetObjectToRead();
				}
				Debug.Fail("Unknown mode.");
				return null;
			}
			return ((IEditor)_text).GetObjectToRead();
		}

		public IMetaObject GetObjectToModify()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					return _table.GetObjectToModify();
				}
				Debug.Fail("Unknown mode.");
				return null;
			}
			return ((IEditor)_text).GetObjectToModify();
		}

		public bool SelectOnlineState(IOnlineUIServices uiServices, out OnlineState onlineState)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					return _table.SelectOnlineState(uiServices, out onlineState);
				}
				Debug.Fail("Unknown mode.");
				onlineState = default(OnlineState);
				return false;
			}
			if (_text is IHasOnlineMode)
			{
				return ((IHasOnlineMode)_text).SelectOnlineState(uiServices, out onlineState);
			}
			onlineState = default(OnlineState);
			return false;
		}

		public long ModifyPositionForBookmark(long nPosition)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					return _table.ModifyPositionForBookmark(nPosition);
				}
				Debug.Fail("Unknown mode.");
				return nPosition;
			}
			if (_text is IHasBookmarks)
			{
				return ((IHasBookmarks)_text).ModifyPositionForBookmark(nPosition);
			}
			return nPosition;
		}

		public IPrintPainter CreatePrintPainter(long nPosition, int nLength)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					if (_text is IPrintable)
					{
						return ((IPrintable)_text).CreatePrintPainter(nPosition, nLength);
					}
					return null;
				}
				Debug.Fail("Unknown mode.");
				return null;
			}
			if (_text is IPrintable)
			{
				return ((IPrintable)_text).CreatePrintPainter(nPosition, nLength);
			}
			return null;
		}

		public bool UndoableReplace(long nPosition, int nLength, string stReplacement)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					return _table.UndoableReplace(nPosition, nLength, stReplacement);
				}
				Debug.Fail("Unknown mode.");
				return false;
			}
			if (_text is IEditorBasedFindReplace)
			{
				return ((IEditorBasedFindReplace)_text).UndoableReplace(nPosition, nLength, stReplacement);
			}
			return false;
		}

		public void OnVisibilityChanged(bool bVisible)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_text is INotifyOnVisibilityChanged)
			{
				((INotifyOnVisibilityChanged)_text).OnVisibilityChanged(bVisible);
			}
			_table.OnVisibilityChanged(bVisible);
		}

		public void EnableVisibilityChangeNotification()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_text is INotifyOnVisibilityChanged)
			{
				((INotifyOnVisibilityChanged)_text).EnableVisibilityChangeNotification();
			}
			_table.EnableVisibilityChangeNotification();
		}

		private void _textualButtonItem_Activate(object sender, EventArgs e)
		{
			Mode = (SwitchableDeclarationEditorMode)0;
		}

		private void _tabularButtonItem_Activate(object sender, EventArgs e)
		{
			Mode = (SwitchableDeclarationEditorMode)1;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_toolBar.Visible = OptionHelper.IsSwitchable();
		}

		public static IVariable GetSignatureReturnVariable(ISignature sign)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			if ((int)sign.POUType == 87 || ((int)sign.POUType == 118 && !sign.GetFlag((SignatureFlag)128)))
			{
				IVariable[] allOutputs = sign.AllOutputs;
				foreach (IVariable val in allOutputs)
				{
					if (val.GetFlag((VarFlag)4) && val.Name == sign.Name)
					{
						return val;
					}
				}
			}
			return null;
		}

		public RefactoringContextType GetRefactoringContext(out Guid objectGuid, out string stDescription)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Invalid comparison between Unknown and I4
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Invalid comparison between Unknown and I4
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			objectGuid = Guid.Empty;
			stDescription = null;
			GetSelection(out var nPosition, out var nLength);
			IMetaObject objectToRead = GetObjectToRead();
			if (objectToRead != null)
			{
				objectGuid = ObjectGuid;
				if ((int)Mode == 1 && _table.GetSelectedNodeCount() > 1)
				{
					return (RefactoringContextType)16;
				}
				string contentString = objectToRead.Object.GetContentString(ref nPosition, ref nLength, true);
				if (!string.IsNullOrEmpty(contentString))
				{
					IPreCompileContext val = default(IPreCompileContext);
					ISignature val2 = APEnvironment.LanguageModelMgr.FindSignature(ObjectGuid, out val);
					LList<IVariable> obj = new LList<IVariable>((IEnumerable<IVariable>)val2.All);
					long num = default(long);
					short num2 = default(short);
					PositionHelper.SplitPosition(nPosition, out num, out num2);
					IVariable signatureReturnVariable = GetSignatureReturnVariable(val2);
					foreach (IVariable item in obj)
					{
						if (item != signatureReturnVariable && string.Equals(contentString, item.OrgName, StringComparison.OrdinalIgnoreCase) && ((int)Mode == 1 || num == item.SourcePosition.Position))
						{
							stDescription = item.OrgName;
							return (RefactoringContextType)18;
						}
					}
					if (string.Equals(contentString, val2.OrgName, StringComparison.OrdinalIgnoreCase))
					{
						stDescription = val2.OrgName;
						return (RefactoringContextType)17;
					}
					return (RefactoringContextType)16;
				}
				return (RefactoringContextType)16;
			}
			return (RefactoringContextType)0;
		}

		public IEnumerable<SourceRange> GetSelection()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					return _table.GetSelection();
				}
				Debug.Fail("Unknown mode.");
				return (IEnumerable<SourceRange>)(object)new SourceRange[0];
			}
			long num = default(long);
			int num2 = default(int);
			((IEditorView)_text).GetSelection(out num, out num2);
			if (num != -1 || num2 != 0)
			{
				return (IEnumerable<SourceRange>)(object)new SourceRange[1]
				{
					new SourceRange(num, num2)
				};
			}
			return (IEnumerable<SourceRange>)(object)new SourceRange[0];
		}

		public void Select(IEnumerable<SourceRange> selection)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			SwitchableDeclarationEditorMode mode = _mode;
			if ((int)mode != 0)
			{
				if ((int)mode == 1)
				{
					_table.Select(selection);
				}
				else
				{
					Debug.Fail("Unknown mode.");
				}
				return;
			}
			using IEnumerator<SourceRange> enumerator = selection.GetEnumerator();
			if (enumerator.MoveNext())
			{
				SourceRange current = enumerator.Current;
				((IEditorView)_text).Select(current.Position, current.Length);
			}
		}

		public void SetLocalizedObject(IMetaObject obj, bool bLocalizationActive)
		{
			IIECTextEditor obj2 = _text;
			ILocalizableEditor val = (ILocalizableEditor)(object)((obj2 is ILocalizableEditor) ? obj2 : null);
			ILocalizableEditor table = (ILocalizableEditor)(object)_table;
			if (val != null)
			{
				val.SetLocalizedObject(obj, bLocalizationActive);
			}
			if (table != null)
			{
				table.SetLocalizedObject(obj, bLocalizationActive);
			}
		}

		public bool IsComment(long nPositionCombined, string stText)
		{
			IIECTextEditor obj = _text;
			ILocalizableEditor val = (ILocalizableEditor)(object)((obj is ILocalizableEditor) ? obj : null);
			TabularDeclarationEditor table = _table;
			bool flag = false;
			if (val != null)
			{
				flag |= val.IsComment(nPositionCombined, stText);
			}
			if (table != null)
			{
				flag |= val.IsComment(nPositionCombined, stText);
			}
			return flag;
		}

		public string GetPreselectedType(long nPosition)
		{
			IIECTextEditor obj = _text;
			ISupportOnlineInstancePreselection val = (ISupportOnlineInstancePreselection)(object)((obj is ISupportOnlineInstancePreselection) ? obj : null);
			ISupportOnlineInstancePreselection table = (ISupportOnlineInstancePreselection)(object)_table;
			string text = string.Empty;
			if (val != null)
			{
				text = val.GetPreselectedType(nPosition);
			}
			if (string.IsNullOrEmpty(text) && table != null)
			{
				text = table.GetPreselectedType(nPosition);
			}
			return text;
		}

		public string GetInstancePath(long nPosition)
		{
			IIECTextEditor obj = _text;
			ISupportOnlineInstancePreselection val = (ISupportOnlineInstancePreselection)(object)((obj is ISupportOnlineInstancePreselection) ? obj : null);
			ISupportOnlineInstancePreselection table = (ISupportOnlineInstancePreselection)(object)_table;
			string text = string.Empty;
			if (val != null)
			{
				text = val.GetInstancePath(nPosition);
			}
			if (string.IsNullOrEmpty(text) && table != null)
			{
				text = table.GetInstancePath(nPosition);
			}
			return text;
		}

		public void PostProcess(IAutoDeclare[] adDecls, long lSelectionPosition)
		{
			IIECTextEditor obj = _text;
			IIECTextEditor obj2 = ((obj is IAutoDeclarePostProcessor) ? obj : null);
			if (obj2 != null)
			{
				((IAutoDeclarePostProcessor)obj2).PostProcess(adDecls, lSelectionPosition);
			}
			_table?.Reload();
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.TabularDeclarationEditor.SwitchableDeclarationEditor));
			_panel = new System.Windows.Forms.Panel();
			_toolBar = new System.Windows.Forms.ToolStrip();
			_textualButtonItem = new System.Windows.Forms.ToolStripButton();
			_tabularButtonItem = new System.Windows.Forms.ToolStripButton();
			_toolBar.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_panel, "_panel");
			_panel.Name = "_panel";
			resources.ApplyResources(_toolBar, "_toolBar");
			_toolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			_toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { _textualButtonItem, _tabularButtonItem });
			_toolBar.Name = "_toolBar";
			_textualButtonItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(_textualButtonItem, "_textualButtonItem");
			_textualButtonItem.Name = "_textualButtonItem";
			_textualButtonItem.Click += new System.EventHandler(_textualButtonItem_Activate);
			_tabularButtonItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(_tabularButtonItem, "_tabularButtonItem");
			_tabularButtonItem.Name = "_tabularButtonItem";
			_tabularButtonItem.Click += new System.EventHandler(_tabularButtonItem_Activate);
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.Controls.Add(_panel);
			base.Controls.Add(_toolBar);
			base.Name = "SwitchableDeclarationEditor";
			_toolBar.ResumeLayout(false);
			_toolBar.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
