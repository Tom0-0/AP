using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.DeviceObject
{
	internal static class ProjectOptionsHelper
	{
		private static IOptionKey CompileOptionKey
		{
			get
			{
				return SystemInstances.OptionStorage.GetRootKey(OptionRoot.Project).CreateSubKey(ProjectOptionsHelper.COMPILE_OPTION_ID);
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000880 RID: 2176 RVA: 0x0003C626 File Offset: 0x0003A826
		public static bool SupportUnicodeIdentifiers
		{
			get
			{
				return ProjectOptionsHelper.CompileOptionKey.HasValue(ProjectOptionsHelper.OP_UNICODE_IDENTIFIERS, typeof(bool)) && (bool)ProjectOptionsHelper.CompileOptionKey[ProjectOptionsHelper.OP_UNICODE_IDENTIFIERS];
			}
		}

		// Token: 0x04000268 RID: 616
		internal static readonly string COMPILE_OPTION_ID = "{E709B08B-B6E4-4966-8EED-D793A13114C6}";

		// Token: 0x04000269 RID: 617
		internal static readonly string OP_UNICODE_IDENTIFIERS = "UnicodeIdentifiers";

		// Token: 0x0400026A RID: 618
		private static readonly string SUB_KEY = "{FE9F63D4-9E34-43ce-9322-D46747A00F33}";
	}
}
