using System.Drawing;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.WatchList
{
	public sealed class ExpressionData
	{
		private string _stFullExpression;

		private string _stDisplayExpression;

		private Image _image;

		private bool _bIsOutdated;

		public string FullExpression => _stFullExpression;

		internal string DisplayExpression => _stDisplayExpression;

		internal Image Image => _image;

		internal bool IsOutdated
		{
			get
			{
				return _bIsOutdated;
			}
			set
			{
				_bIsOutdated = value;
			}
		}

		internal ExpressionData(string stFullExpression, string stDisplayExpression, Image image)
			: this(stFullExpression, stDisplayExpression, image, bTryToConvertToCastExpression: false)
		{
		}

		public ExpressionData(string stFullExpression, string stDisplayExpression, Image image, bool bTryToConvertToCastExpression)
		{
			_stFullExpression = TryToConvertToCastExpression(stFullExpression);
			_stDisplayExpression = TryToConvertToCastExpression(stDisplayExpression);
			_image = image;
		}

		private string TryToConvertToCastExpression(string stExpression)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Invalid comparison between I4 and Unknown
			if (string.IsNullOrEmpty(stExpression))
			{
				return stExpression;
			}
			string[] array = stExpression.Split(':');
			if (1 == array.Length)
			{
				return stExpression;
			}
			if (2 < array.Length)
			{
				throw new InvalidExpressionSyntaxException(string.Format(Strings.InvalidCastSyntax, stExpression));
			}
			string arg = array[0];
			string text = array[1];
			ICompiledType val = APEnvironment.LMServiceProvider.CreatorService.CreateParser(text).ParseTypeDeclaration();
			if (val != null && 22 == (int)((IType)val).Class)
			{
				string text2 = $"__CAST({arg},{text})";
				IParser obj = APEnvironment.LMServiceProvider.CreatorService.CreateParser(text2);
				IExpression obj2 = ((IParser2)((obj is IParser2) ? obj : null)).ParseExpression();
				ICastExpression val2 = (ICastExpression)(object)((obj2 is ICastExpression) ? obj2 : null);
				if (val2.Base is IErrorExpression || val2.ExplicitelySpecifiedType == null)
				{
					throw new InvalidExpressionSyntaxException(string.Format(Strings.SyntaxErrorCast, stExpression));
				}
				return text2;
			}
			throw new InvalidExpressionSyntaxException(string.Format(Strings.InvalidTypeSpecification, text));
		}
	}
}
