using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.WatchList
{
	public class MonitoringRangeForm : Form
	{
		private bool _bReloading = true;

		private int _nMin;

		private int _nMax;

		private int _nStart;

		private int _nEnd;

		private MonitoringRangeContext _context;

		private ICompileContext20 _comcon;

		private IList<string> _allSubelements;

		private IContainer components;

		private Button _okButton;

		private Button _cancelButton;

		private Label label1;

		private Label label2;

		private Label _lbInfo;

		private ErrorProvider _error;

		private TextBox _tbEnd;

		private TextBox _tbStart;

		private Label _lbLimit;

		private Label _lbRange;

		private Label label4;

		private Label label3;

		private TrackBar _trackBar;

		private Label _lbScrollRight;

		private Label _lbScrollLeft;

		private Label label5;

		internal int NewStart => _nStart;

		internal int NewEnd => _nEnd;

		private ICompileContext20 CompileContext => _comcon ?? (_comcon = CreateCompileContext());

		public MonitoringRangeForm(MonitoringRangeContext context)
		{
			InitializeComponent();
			Cursor.Current = Cursors.WaitCursor;
			_context = context;
			_nMin = (_context.SingleDimension ? context.LowerBound : 0);
			_nMax = (_context.SingleDimension ? context.UpperBound : (_context.TotalRange - 1));
			_nStart = context.Start;
			_nEnd = context.End;
			if (_nMin < _nMax - WatchListModel.DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY)
			{
				_trackBar.Minimum = _nMin;
				_trackBar.Maximum = _nMax - WatchListModel.DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY;
				if (_nStart < _nMin || _trackBar.Maximum < _nStart)
				{
					_nStart = _nMin;
				}
				if (_context.SingleDimension)
				{
					_trackBar.Value = _nStart;
				}
				else
				{
					_trackBar.Value = ((_nStart >= 0) ? _nStart : 0);
				}
				_lbScrollLeft.Text = _trackBar.Minimum.ToString();
				_lbScrollRight.Text = _trackBar.Maximum.ToString();
			}
			else
			{
				_trackBar.Enabled = false;
				_lbScrollLeft.Text = string.Empty;
				_lbScrollRight.Text = string.Empty;
			}
			if (_context.SingleDimension)
			{
				_tbStart.Text = _nStart.ToString();
				_tbEnd.Text = _nEnd.ToString();
			}
			else
			{
				bool flag = _nStart < 0 || _nEnd < 0;
				bool flag2 = default(bool);
				string[] source = CompileContext.SubElementsWithRange((IType)(object)_context.Type, _context.Expression, (!flag) ? _nStart : 0, flag ? 999 : _nEnd, out flag2);
				if (flag2)
				{
					_tbStart.Text = GetIndexStringFromElement(source.First());
					_tbEnd.Text = GetIndexStringFromElement(source.Last());
				}
				if (_allSubelements == null)
				{
					_allSubelements = (from p in CompileContext.SubElementsWithRange((IType)(object)_context.Type, _context.Expression, -1, -1, out flag2)
						select GetIndexStringFromElement(p).Replace(" ", "")).ToList();
				}
			}
			string text = string.Empty;
			for (int i = 0; i < context.LowerBounds.Count; i++)
			{
				text = string.Concat(text, "[" + context.LowerBounds[i] + ".." + context.UpperBounds[i] + "]");
				if (i < context.LowerBounds.Count - 1)
				{
					text += " ";
				}
			}
			_lbRange.Text = text;
			_lbLimit.Text = WatchListModel.MAX_MONITORING_ELEMENTS_PER_ARRAY.ToString();
			_error.SetIconAlignment(_tbStart, ErrorIconAlignment.MiddleLeft);
			_error.SetIconAlignment(_tbEnd, ErrorIconAlignment.MiddleLeft);
			Cursor.Current = Cursors.Default;
			_bReloading = false;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			_tbStart.Select();
			_tbStart.SelectAll();
		}

		private IList<int> GetArrayIndicesFromString(string stArrayIndices)
		{
			Match match = new Regex("\\[.*\\]").Match(stArrayIndices);
			_ = string.Empty;
			List<int> list = null;
			if (match.Success)
			{
				string[] array = match.Value.Substring(1, match.Value.Length - 2).Split(',');
				if (array.Any())
				{
					list = new List<int>();
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (int.TryParse(array2[i].TrimStart(), out var result))
						{
							list.Add(result);
							continue;
						}
						list = null;
						break;
					}
				}
			}
			return list;
		}

		private void DoSanityCheck()
		{
			int result = 0;
			int result2 = 0;
			if (_context.SingleDimension)
			{
				if (int.TryParse(_tbStart.Text, out result) && result >= _nMin)
				{
					_nStart = result;
					_error.SetError(_tbStart, string.Empty);
				}
				else
				{
					_error.SetError(_tbStart, Strings.Error_InvalidValue);
				}
				if (int.TryParse(_tbEnd.Text, out result2) && result2 <= _nMax)
				{
					_error.SetError(_tbEnd, string.Empty);
					_nEnd = result2;
				}
				else
				{
					_error.SetError(_tbEnd, Strings.Error_InvalidValue);
				}
				if (_nStart > _nEnd)
				{
					_error.SetError(_tbStart, Strings.Error_InvalidValue);
					_error.SetError(_tbEnd, Strings.Error_InvalidValue);
				}
				else if (_nEnd - _nStart >= WatchListModel.MAX_MONITORING_ELEMENTS_PER_ARRAY)
				{
					string value = string.Format(Strings.Error_MaximumRangeExceeded, WatchListModel.MAX_MONITORING_ELEMENTS_PER_ARRAY.ToString());
					_error.SetError(_tbStart, value);
					_error.SetError(_tbEnd, value);
				}
			}
			else
			{
				IList<int> arrayIndicesFromString = GetArrayIndicesFromString(_tbStart.Text);
				IList<int> arrayIndicesFromString2 = GetArrayIndicesFromString(_tbEnd.Text);
				if (arrayIndicesFromString != null && arrayIndicesFromString2 != null && arrayIndicesFromString.Count == arrayIndicesFromString2.Count)
				{
					bool flag = false;
					for (int i = 0; i < arrayIndicesFromString.Count; i++)
					{
						try
						{
							int num = arrayIndicesFromString[i];
							int num2 = arrayIndicesFromString2[i];
							if (num >= _context.LowerBounds[i] && num <= _context.UpperBounds[i] && num2 >= _context.LowerBounds[i] && num2 <= _context.UpperBounds[i])
							{
								_error.SetError(_tbStart, string.Empty);
								_error.SetError(_tbEnd, string.Empty);
							}
							else
							{
								_error.SetError(_tbStart, Strings.Error_InvalidValue);
								_error.SetError(_tbEnd, Strings.Error_InvalidValue);
								flag = true;
							}
						}
						catch
						{
							_error.SetError(_tbStart, Strings.Error_InvalidValue);
							_error.SetError(_tbEnd, Strings.Error_InvalidValue);
							flag = true;
						}
					}
					if (!flag)
					{
						string item = "[" + string.Join(",", arrayIndicesFromString.Select((int p) => p.ToString())) + "]";
						string item2 = "[" + string.Join(",", arrayIndicesFromString2.Select((int p) => p.ToString())) + "]";
						_nStart = _allSubelements.IndexOf(item);
						_nEnd = _allSubelements.IndexOf(item2);
					}
					if (_nStart > _nEnd)
					{
						_error.SetError(_tbStart, Strings.Error_InvalidValue);
						_error.SetError(_tbEnd, Strings.Error_InvalidValue);
					}
					else if (_nEnd - _nStart >= WatchListModel.MAX_MONITORING_ELEMENTS_PER_ARRAY)
					{
						string value2 = string.Format(Strings.Error_MaximumRangeExceeded, WatchListModel.MAX_MONITORING_ELEMENTS_PER_ARRAY.ToString());
						_error.SetError(_tbStart, value2);
						_error.SetError(_tbEnd, value2);
					}
				}
				else if (arrayIndicesFromString == null)
				{
					_error.SetError(_tbStart, Strings.Error_InvalidValue);
				}
				else if (arrayIndicesFromString2 == null)
				{
					_error.SetError(_tbEnd, Strings.Error_InvalidValue);
				}
				else
				{
					_error.SetError(_tbStart, Strings.Error_InvalidValue);
					_error.SetError(_tbEnd, Strings.Error_InvalidValue);
				}
			}
			_okButton.Enabled = _error.GetError(_tbStart) == string.Empty && _error.GetError(_tbEnd) == string.Empty;
		}

		private void _tbStart_TextChanged(object sender, EventArgs e)
		{
			if (!_bReloading)
			{
				DoSanityCheck();
			}
		}

		private void _tbEnd_TextChanged(object sender, EventArgs e)
		{
			if (!_bReloading)
			{
				DoSanityCheck();
			}
		}

		private void _trackBar_Scroll(object sender, EventArgs e)
		{
			bool flag = default(bool);
			string[] array = CompileContext.SubElementsWithRange((IType)(object)_context.Type, _context.Expression, _trackBar.Value, _trackBar.Value + WatchListModel.DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY, out flag);
			if (flag && array != null)
			{
				if (_context.SingleDimension)
				{
					_tbStart.Text = _trackBar.Value.ToString();
					_tbEnd.Text = (_trackBar.Value + WatchListModel.DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY).ToString();
				}
				else
				{
					_tbStart.Text = GetIndexStringFromElement(array.First());
					_tbEnd.Text = GetIndexStringFromElement(array.Last());
				}
				DoSanityCheck();
			}
		}

		private string GetIndexStringFromElement(string stElement)
		{
			int startIndex = stElement.IndexOf("[");
			return stElement.Substring(startIndex);
		}

		private ICompileContext20 CreateCompileContext()
		{
			Common.SplitExpression(_context.Expression, out var stResource, out var stApplication, out var _);
			if (_context.ApplicationGuid != Guid.Empty)
			{
				ICompileContext referenceContextIfAvailable = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(_context.ApplicationGuid);
				return (ICompileContext20)(object)((referenceContextIfAvailable is ICompileContext20) ? referenceContextIfAvailable : null);
			}
			ICompileContext referenceContextIfAvailable2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContextGuidByName(stResource, stApplication));
			return (ICompileContext20)(object)((referenceContextIfAvailable2 is ICompileContext20) ? referenceContextIfAvailable2 : null);
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.WatchList.MonitoringRangeForm));
			_okButton = new System.Windows.Forms.Button();
			_cancelButton = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			_lbInfo = new System.Windows.Forms.Label();
			_error = new System.Windows.Forms.ErrorProvider(components);
			_tbStart = new System.Windows.Forms.TextBox();
			_tbEnd = new System.Windows.Forms.TextBox();
			_lbLimit = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			_lbRange = new System.Windows.Forms.Label();
			_trackBar = new System.Windows.Forms.TrackBar();
			_lbScrollLeft = new System.Windows.Forms.Label();
			_lbScrollRight = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)_error).BeginInit();
			((System.ComponentModel.ISupportInitialize)_trackBar).BeginInit();
			SuspendLayout();
			componentResourceManager.ApplyResources(_okButton, "_okButton");
			_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			_okButton.Name = "_okButton";
			_okButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(_cancelButton, "_cancelButton");
			_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_cancelButton.Name = "_cancelButton";
			_cancelButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(label1, "label1");
			label1.Name = "label1";
			componentResourceManager.ApplyResources(label2, "label2");
			label2.Name = "label2";
			componentResourceManager.ApplyResources(_lbInfo, "_lbInfo");
			_lbInfo.Name = "_lbInfo";
			_error.ContainerControl = this;
			componentResourceManager.ApplyResources(_tbStart, "_tbStart");
			_tbStart.Name = "_tbStart";
			_tbStart.TextChanged += new System.EventHandler(_tbStart_TextChanged);
			componentResourceManager.ApplyResources(_tbEnd, "_tbEnd");
			_tbEnd.Name = "_tbEnd";
			_tbEnd.TextChanged += new System.EventHandler(_tbEnd_TextChanged);
			componentResourceManager.ApplyResources(_lbLimit, "_lbLimit");
			_lbLimit.Name = "_lbLimit";
			componentResourceManager.ApplyResources(label3, "label3");
			label3.Name = "label3";
			componentResourceManager.ApplyResources(label4, "label4");
			label4.Name = "label4";
			componentResourceManager.ApplyResources(_lbRange, "_lbRange");
			_lbRange.Name = "_lbRange";
			componentResourceManager.ApplyResources(_trackBar, "_trackBar");
			_trackBar.LargeChange = 1000;
			_trackBar.Name = "_trackBar";
			_trackBar.SmallChange = 100;
			_trackBar.TickFrequency = 100;
			_trackBar.Scroll += new System.EventHandler(_trackBar_Scroll);
			componentResourceManager.ApplyResources(_lbScrollLeft, "_lbScrollLeft");
			_lbScrollLeft.Name = "_lbScrollLeft";
			componentResourceManager.ApplyResources(_lbScrollRight, "_lbScrollRight");
			_lbScrollRight.Name = "_lbScrollRight";
			componentResourceManager.ApplyResources(label5, "label5");
			label5.Name = "label5";
			base.AcceptButton = _okButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = _cancelButton;
			base.Controls.Add(label5);
			base.Controls.Add(_lbScrollRight);
			base.Controls.Add(_lbScrollLeft);
			base.Controls.Add(_trackBar);
			base.Controls.Add(_lbRange);
			base.Controls.Add(label4);
			base.Controls.Add(label3);
			base.Controls.Add(_lbLimit);
			base.Controls.Add(_tbEnd);
			base.Controls.Add(_tbStart);
			base.Controls.Add(_lbInfo);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(_okButton);
			base.Controls.Add(_cancelButton);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MonitoringRangeForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)_error).EndInit();
			((System.ComponentModel.ISupportInitialize)_trackBar).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
