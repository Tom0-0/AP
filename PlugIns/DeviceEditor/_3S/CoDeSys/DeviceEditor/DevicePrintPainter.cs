using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Printing;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class DevicePrintPainter : IPrintPainterEx
	{
		private class PrintPainterInfo
		{
			public object PP;

			public string Caption;

			public bool CaptionPrinted;
		}

		private IDeviceObject _deviceObject;

		private List<PrintPainterInfo> _ppPageList = new List<PrintPainterInfo>();

		private int _nLastPrinted;

		private Font _font;

		internal DevicePrintPainter()
		{
		}

		internal void PrintHiddenGenericEditor(DeviceEditor devEditor, IConnector con, long nPosition, int nLength, HideParameterDelegate filter)
		{
			ConnectorParameterEditor connectorParameterEditor = new ConnectorParameterEditor(filter);
			connectorParameterEditor.ConnectorEditorFrame = (IConnectorEditorFrame)(object)devEditor;
			connectorParameterEditor.ConnectorId = con.ConnectorId;
			connectorParameterEditor.Reload();
			PrintPages(devEditor, connectorParameterEditor.Pages, nPosition, nLength, (IBaseDeviceEditor)(object)connectorParameterEditor);
		}

		internal void PrintPages(DeviceEditor devEditor, IEditorPage[] pages, long nPosition, int nLength, IBaseDeviceEditor editor)
		{
			IMetaObject objectToRead = devEditor.GetObjectToRead();
			foreach (IEditorPage val in pages)
			{
				if ((val is DeviceInformationControl || devEditor.CheckEditorInfoVisibility(val, editor)) && (!(val is DeviceInformationControl) || !(objectToRead.Object is IExplicitConnector)))
				{
					if (val is IPrintableEx)
					{
						PrintPainterInfo printPainterInfo = new PrintPainterInfo();
						printPainterInfo.Caption = val.PageName;
						printPainterInfo.CaptionPrinted = false;
						printPainterInfo.PP = ((IPrintableEx)((val is IPrintableEx) ? val : null)).CreatePrintPainter(nPosition, nLength);
						_ppPageList.Add(printPainterInfo);
					}
					else if (val is IPrintable)
					{
						PrintPainterInfo printPainterInfo2 = new PrintPainterInfo();
						printPainterInfo2.Caption = val.PageName;
						printPainterInfo2.CaptionPrinted = false;
						printPainterInfo2.PP = ((IPrintable)((val is IPrintable) ? val : null)).CreatePrintPainter(nPosition, nLength);
						_ppPageList.Add(printPainterInfo2);
					}
					if (_font == null)
					{
						_font = val.Control.Font;
					}
				}
			}
		}

		internal DevicePrintPainter(DeviceEditor devEditor, IDeviceObject deviceObject, EditorList edlist, long nPosition, int nLength)
		{
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Expected O, but got Unknown
			_deviceObject = deviceObject;
			if (!OptionsHelper.ShowGenericConfiguration)
			{
				bool flag = true;
				bool flag2 = true;
				HideParameterDelegate filter = null;
				foreach (EditorInfo item in edlist)
				{
					if (item.Editor is DeviceParameterEditor && (item.Editor as DeviceParameterEditor).ParameterFilter != null)
					{
						filter = (item.Editor as DeviceParameterEditor).ParameterFilter;
					}
					if (item.Editor is ConnectorParameterEditor && (item.Editor as ConnectorParameterEditor).ParameterFilter != null)
					{
						filter = (item.Editor as ConnectorParameterEditor).ParameterFilter;
					}
					IEditorPage[] pages = item.Pages;
					foreach (IEditorPage obj in pages)
					{
						if (obj is ParameterEditorPage)
						{
							flag = false;
						}
						if (obj is DeviceInformationControl)
						{
							flag2 = false;
						}
					}
				}
				if (flag)
				{
					IMetaObject objectToRead = devEditor.GetObjectToRead();
					if (objectToRead.Object is IDeviceObject)
					{
						IObject @object = objectToRead.Object;
						foreach (IConnector item2 in (IEnumerable)((IDeviceObject)((@object is IDeviceObject) ? @object : null)).Connectors)
						{
							IConnector val = item2;
							if (!val.IsExplicit)
							{
								PrintHiddenGenericEditor(devEditor, val, nPosition, nLength, filter);
							}
						}
					}
					if (objectToRead.Object is IExplicitConnector)
					{
						IObject object2 = objectToRead.Object;
						IExplicitConnector con = (IExplicitConnector)(object)((object2 is IExplicitConnector) ? object2 : null);
						PrintHiddenGenericEditor(devEditor, (IConnector)(object)con, nPosition, nLength, filter);
					}
				}
				if (flag2 && !(devEditor.GetObjectToRead().Object is IExplicitConnector))
				{
					DeviceInformationControl deviceInformationControl = new DeviceInformationControl(null)
					{
						DeviceEditorFrame = (IDeviceEditorFrame)(object)devEditor
					};
					PrintPages(devEditor, (IEditorPage[])(object)new IEditorPage[1] { deviceInformationControl }, nPosition, nLength, (IBaseDeviceEditor)(object)deviceInformationControl);
				}
			}
			foreach (EditorInfo item3 in edlist)
			{
				PrintPages(devEditor, item3.Pages, nPosition, nLength, item3.Editor);
			}
		}

		public bool PrintPage(IPrintContext ctx, out int nConsumedHeight)
		{
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			if (_font == null)
			{
				nConsumedHeight = 0;
				return false;
			}
			int num = ctx.Graphics.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWYXYZabcdefjhijklmnopqrstuvwxyz_0123456789", _font).ToSize()
				.Height + 1;
			nConsumedHeight = 0;
			Rectangle thisPageRect = ctx.ThisPageRect;
			int y = ctx.ThisPageRect.Y;
			int height = ctx.ThisPageRect.Height;
			while (_nLastPrinted < _ppPageList.Count)
			{
				thisPageRect.Y = y + nConsumedHeight;
				thisPageRect.Height = height - nConsumedHeight;
				ctx.ThisPageRect=(thisPageRect);
				if (!_ppPageList[_nLastPrinted].CaptionPrinted)
				{
					if (thisPageRect.Y + 6 * num > ctx.ThisPageRect.Bottom)
					{
						return true;
					}
					int num2 = 0;
					using (Font font = new Font(_font, FontStyle.Bold | FontStyle.Underline))
					{
						if (thisPageRect.Y > y)
						{
							if (_nLastPrinted > 0)
							{
								ctx.Graphics.DrawLine(Pens.Gray, ctx.ThisPageRect.Left, ctx.ThisPageRect.Top, ctx.ThisPageRect.Right, ctx.ThisPageRect.Top);
							}
							ctx.Graphics.DrawString(_ppPageList[_nLastPrinted].Caption, font, Brushes.Black, ctx.ThisPageRect.Left, thisPageRect.Y + 2 * num);
							num2 = 4;
						}
						else
						{
							ctx.Graphics.DrawString(_ppPageList[_nLastPrinted].Caption, font, Brushes.Black, ctx.ThisPageRect.Left, thisPageRect.Y);
							num2 = 2;
						}
					}
					_ppPageList[_nLastPrinted].CaptionPrinted = true;
					thisPageRect = ctx.ThisPageRect;
					thisPageRect.Y += num2 * num;
					thisPageRect.Height -= num2 * num;
					ctx.ThisPageRect=(thisPageRect);
					nConsumedHeight += num2 * num;
				}
				int num3 = 0;
				bool flag = false;
				if (_ppPageList[_nLastPrinted].PP is IPrintPainterEx)
				{
					flag = ((IPrintPainterEx)_ppPageList[_nLastPrinted].PP).PrintPage(ctx, out num3);
				}
				else if (_ppPageList[_nLastPrinted].PP is IPrintPainter)
				{
					flag = ((IPrintPainter)_ppPageList[_nLastPrinted].PP).PrintPage(ctx.Graphics, ctx.ThisPageRect, out num3);
				}
				nConsumedHeight += num3;
				if (flag)
				{
					return true;
				}
				if (_nLastPrinted != _ppPageList.Count - 1 && num3 != 0)
				{
					nConsumedHeight += num;
				}
				_nLastPrinted++;
			}
			if (_nLastPrinted == _ppPageList.Count)
			{
				return false;
			}
			return true;
		}
	}
}
