using System;
using System.Collections.Generic;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class AppearanceFactoryManager
	{
		private static AppearanceFactoryManager s_instance;

		public static AppearanceFactoryManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new AppearanceFactoryManager();
				}
				return s_instance;
			}
		}

		private AppearanceFactoryManager()
		{
		}

		public bool ShowEditor(int nProjectHandle, Guid objectGuid, IConnector connector, IEditorPage page, out bool bShowEditor)
		{
			bShowEditor = false;
			bool result = false;
			int num = -1;
			bool flag = default(bool);
			foreach (IAppearancePageFactory appearancePageFactory in APEnvironment.AppearancePageFactories)
			{
				int num2 = appearancePageFactory.ShowEditor(nProjectHandle, objectGuid, connector, page, out flag);
				if (num2 > num)
				{
					result = true;
					num = num2;
					bShowEditor = flag;
				}
			}
			return result;
		}

		public bool SortPages(int nProjectHandle, Guid objectGuid, List<IEditorPage> pages, out List<IEditorPage> sortedPages)
		{
			sortedPages = new List<IEditorPage>();
			bool result = false;
			int num = -1;
			List<IEditorPage> list = default(List<IEditorPage>);
			foreach (IAppearancePageFactory appearancePageFactory in APEnvironment.AppearancePageFactories)
			{
				if (appearancePageFactory is IAppearancePageFactory2)
				{
					int num2 = ((IAppearancePageFactory2)((appearancePageFactory is IAppearancePageFactory2) ? appearancePageFactory : null)).SortPages(nProjectHandle, objectGuid, pages, out list);
					if (num2 > num)
					{
						result = true;
						num = num2;
						sortedPages = list;
					}
				}
			}
			return result;
		}
	}
}
