using System;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{6AA1FB55-612D-4295-AF34-77FFD904FA44}")]
	public class ImportMappingCSV : AbstractImportExportCommand, IStandardCommand2, IStandardCommand, ICommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "device", "importcsv" };

		public override string[] BatchCommand => BATCH_COMMAND;

		public override string[] CreateBatchArguments(bool bInvokedByContextMenu)
		{
			string[] result = null;
			if (((IEngine)APEnvironment.Engine).Frame != null && ((IEngine)APEnvironment.Engine).Frame.ActiveView != null && ((IEngine)APEnvironment.Engine).Frame.ActiveView.Control
				.ContainsFocus)
			{
				IView activeView = ((IEngine)APEnvironment.Engine).Frame.ActiveView;
				IExportImportCSVCommands val = (IExportImportCSVCommands)(object)((activeView is IExportImportCSVCommands) ? activeView : null);
				if (val != null)
				{
					val.GetArgsForImportCommand(out result);
					return result;
				}
			}
			IMetaObjectStub selectedStub = DeviceCommandHelper.GetSelectedStub();
			IMetaObjectStub hostFromSelectedStub = DeviceCommandHelper.GetHostFromSelectedStub();
			if (hostFromSelectedStub != null && selectedStub != null)
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.DefaultExt = ".csv";
				openFileDialog.Filter = "csv files (*.csv)|*.csv";
				openFileDialog.FileName = selectedStub.Name.Replace("/", "_");
				result = ((openFileDialog.ShowDialog((IWin32Window)APEnvironment.FrameForm) != DialogResult.OK) ? null : new string[3]
				{
					openFileDialog.FileName,
					hostFromSelectedStub.ProjectHandle.ToString(),
					hostFromSelectedStub.ObjectGuid.ToString()
				});
			}
			return result;
		}

		public void ExecuteBatch(string[] arguments)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (arguments.Length > 3)
			{
				throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 3);
			}
			if (arguments.Length < 3)
			{
				throw new BatchTooFewArgumentsException(BatchCommand, arguments.Length, 3);
			}
			if (arguments.Length == 0)
			{
				return;
			}
			ImportExportCSV importExportCSV = new ImportExportCSV();
			int.TryParse(arguments[1], out var result);
			Guid hostDeviceGuid = new Guid(arguments[2]);
			IUndoManager val = null;
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(result);
				if (val != null)
				{
					val.BeginCompoundAction("Import CSV Mappings");
				}
				importExportCSV.ImportFromCSV(arguments[0], result, hostDeviceGuid);
			}
			finally
			{
				if (val != null)
				{
					val.EndCompoundAction();
				}
			}
		}
	}
}
