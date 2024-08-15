#define DEBUG
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject.LogicalDevice
{
	public class LogicalGVLEmbeddesDiffViewer : UserControl, IEmbeddedDiffViewer2, IEmbeddedDiffViewer
	{
		private IEmbeddedDiffViewer2 _interfaceDiffViewer;

		private string _stLeftLabel;

		private string _stRightLabel;

		private static readonly ITextDocumentProvider TEXT_DOCUMENT_PROVIDER = (ITextDocumentProvider)(object)new LogicalGVLTextDocumentProvider();

		private IContainer components;

		public DiffDisplayMode CurrentDisplayMode => _interfaceDiffViewer.CurrentDisplayMode;

		public DiffDisplayMode SupportedDisplayModes => _interfaceDiffViewer.SupportedDisplayModes;

		public AcceptState? CurrentAcceptState => _interfaceDiffViewer.CurrentAcceptState;

		public DiffState? CurrentDiffState => _interfaceDiffViewer.CurrentDiffState;

		public Control EmbeddedControl => this;

		public Control[] Panes => null;

		public bool CanNextDiff => ((IEmbeddedDiffViewer)_interfaceDiffViewer).CanNextDiff;

		public bool CanPreviousDiff => ((IEmbeddedDiffViewer)_interfaceDiffViewer).CanPreviousDiff;

		public bool CanAcceptBlock => false;

		public bool CanAcceptSingle => false;

		public bool CanAcceptProperties => ((IEmbeddedDiffViewer)_interfaceDiffViewer).CanAcceptProperties;

		public bool CanOpposeChanges => ((IEmbeddedDiffViewer)_interfaceDiffViewer).CanOpposeChanges;

		public string LeftLabel => _stLeftLabel;

		public string RightLabel => _stRightLabel;

		public bool CanHandleIgnoreWhitespace => ((IEmbeddedDiffViewer)_interfaceDiffViewer).CanHandleIgnoreWhitespace;

		public bool CanHandleIgnoreComments => ((IEmbeddedDiffViewer)_interfaceDiffViewer).CanHandleIgnoreComments;

		public bool CanHandleIgnoreProperties => ((IEmbeddedDiffViewer)_interfaceDiffViewer).CanHandleIgnoreProperties;

		public int AdditionsCount => ((IEmbeddedDiffViewer)_interfaceDiffViewer).AdditionsCount;

		public int DeletionsCount => ((IEmbeddedDiffViewer)_interfaceDiffViewer).DeletionsCount;

		public int ChangesCount => ((IEmbeddedDiffViewer)_interfaceDiffViewer).ChangesCount;

		public LogicalGVLEmbeddesDiffViewer()
		{
			InitializeComponent();
		}

		internal void Initialize(int nLeftProjectHandle, Guid leftObjectGuid, int nRightProjectHandle, Guid rightObjectGuid, bool bIgnoreWhitespace, bool bIgnoreComments, bool bIgnoreProperties, string stLeftLabel, string stRightLabel)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			_stLeftLabel = stLeftLabel;
			_stRightLabel = stRightLabel;
			IIECEmbeddedDiffViewer val = APEnvironment.CreateIECEmbeddedDiffViewer();
			val.Initialize(nLeftProjectHandle, leftObjectGuid, nRightProjectHandle, rightObjectGuid, bIgnoreWhitespace, bIgnoreComments, bIgnoreProperties, stLeftLabel, stRightLabel, TEXT_DOCUMENT_PROVIDER);
			_interfaceDiffViewer = (IEmbeddedDiffViewer2)val;
			Debug.Assert(_interfaceDiffViewer != null);
			((IEmbeddedDiffViewer)_interfaceDiffViewer).EmbeddedControl.Dock = DockStyle.Fill;
		}

		public void SetAllAcceptStates(AcceptState state)
		{
		}

		public void SetBlockAcceptState(AcceptState state)
		{
		}

		public void SetSingleAcceptState(AcceptState state)
		{
		}

		public void SetDisplayMode(DiffDisplayMode mode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_interfaceDiffViewer.SetDisplayMode(mode);
		}

		public int? GetAcceptionCount(DiffState state = DiffState.AnyChange)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return _interfaceDiffViewer.GetAcceptionCount(state);
		}

		public bool IsEverythingAccepted()
		{
			return false;
		}

		public void UpdateContents(bool bOpposeChanges)
		{
			((IEmbeddedDiffViewer)_interfaceDiffViewer).UpdateContents(bOpposeChanges);
			base.Controls.Add(((IEmbeddedDiffViewer)_interfaceDiffViewer).EmbeddedControl);
		}

		public void Finish(bool bCommit)
		{
		}

		public void NextDiff()
		{
			((IEmbeddedDiffViewer)_interfaceDiffViewer).NextDiff();
		}

		public void PreviousDiff()
		{
			((IEmbeddedDiffViewer)_interfaceDiffViewer).PreviousDiff();
		}

		public void AcceptBlock()
		{
		}

		public void AcceptSingle()
		{
		}

		public void AcceptProperties()
		{
			((IEmbeddedDiffViewer)_interfaceDiffViewer).AcceptProperties();
		}

		public bool IsDirty()
		{
			return ((IEmbeddedDiffViewer)_interfaceDiffViewer).IsDirty();
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
			components = new System.ComponentModel.Container();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}
	}
}
