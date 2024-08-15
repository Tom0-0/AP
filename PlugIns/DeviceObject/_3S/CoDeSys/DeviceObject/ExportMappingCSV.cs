using System;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{E898828E-47A5-4e5c-BC0F-4008FF1999EB}")]
	public class ExportMappingCSV : AbstractImportExportCommand, IStandardCommand2, IStandardCommand, ICommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "device", "exportcsv" };

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
					val.GetArgsForExportCommand(out result);
					return result;
				}
			}
			IMetaObjectStub selectedStub = DeviceCommandHelper.GetSelectedStub();
			if (selectedStub != null)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.DefaultExt = ".csv";
				saveFileDialog.Filter = "csv files (*.csv)|*.csv";
				saveFileDialog.FileName = selectedStub.Name.Replace("/", "_");
				result = ((saveFileDialog.ShowDialog((IWin32Window)APEnvironment.FrameForm) != DialogResult.OK) ? null : new string[3]
				{
					saveFileDialog.FileName,
					selectedStub.ProjectHandle.ToString(),
					selectedStub.ObjectGuid.ToString()
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
			if (arguments.Length != 0)
			{
				ImportExportCSV importExportCSV = new ImportExportCSV();
				int.TryParse(arguments[1], out var result);
				importExportCSV.ExportAsCSV(ObjectGuid: new Guid(arguments[2]), stFileName: arguments[0], nProjectHandle: result);
			}
		}
	}
}
