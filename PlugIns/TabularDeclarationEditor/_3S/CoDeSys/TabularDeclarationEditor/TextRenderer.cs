using System;
using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.PInvoke;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal static class TextRenderer
	{
		internal static int GetWidth(Graphics graphics, IntPtr fontHandle, string stText)
		{
			int[] positions;
			return GetWidth(graphics, fontHandle, stText, out positions);
		}

		internal static int GetWidth(Graphics graphics, IntPtr fontHandle, string stText, out int[] positions)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected I4, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected I4, but got Unknown
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			IntPtr hdc = graphics.GetHdc();
			IntPtr intPtr = Gdi32.SelectObject(hdc, fontHandle);
			positions = new int[stText.Length];
			RECT val = default(RECT);
			DrawTextFlags val2 = (DrawTextFlags)3872;
			for (int i = 0; i < stText.Length; i++)
			{
				User32.DrawText(hdc, stText, i, ref val, (int)val2);
				positions[i] = val.right - val.left;
			}
			User32.DrawText(hdc, stText, -1, ref val, (int)val2);
			int result = val.right - val.left;
			Gdi32.SelectObject(hdc, intPtr);
			graphics.ReleaseHdc(hdc);
			return result;
		}

		internal static void Draw(Graphics graphics, int x, int y, Alignment alignment, IntPtr fontHandle, Color color, string stText)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected I4, but got Unknown
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected I4, but got Unknown
			IntPtr hdc = graphics.GetHdc();
			IntPtr intPtr = Gdi32.SelectObject(hdc, fontHandle);
			int num = Gdi32.SetBkMode(hdc, 1);
			int num2 = color.R | (color.G << 8) | (color.B << 16);
			int num3 = Gdi32.SetTextColor(hdc, num2);
			RECT val = default(RECT);
			val.top = y;
			val.bottom = int.MaxValue;
			DrawTextFlags val2 = (DrawTextFlags)2848;
			switch ((int)alignment)
			{
			case 1:
				val.left = int.MinValue;
				val.right = int.MaxValue;
				val2 = (DrawTextFlags)((int)val2 | 1);
				break;
			case 2:
				val.left = int.MinValue;
				val.right = x;
				val2 = (DrawTextFlags)((int)val2 | 2);
				break;
			default:
				val.left = x;
				val.right = int.MaxValue;
				val2 = (DrawTextFlags)(val2 | 0);
				break;
			}
			User32.DrawText(hdc, stText, -1, ref val, (int)val2);
			Gdi32.SetTextColor(hdc, num3);
			Gdi32.SetBkMode(hdc, num);
			Gdi32.SelectObject(hdc, intPtr);
			graphics.ReleaseHdc(hdc);
		}
	}
}
