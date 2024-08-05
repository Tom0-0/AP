#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.LibManObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal static class LibH
	{
		public static IEnumerable<IManagedLibrary> GetLibraries(int nProj, Guid gdApp)
		{
			foreach (ILibManItem toplevelLib in GetToplevelLibs(nProj, gdApp))
			{
				IManagedLibrary managedLibFromLibManItem = GetManagedLibFromLibManItem(toplevelLib);
				if (managedLibFromLibManItem != null)
				{
					yield return managedLibFromLibManItem;
				}
			}
		}

		public static IEnumerable<IProject> GetVisibleLibraries(int nProj, Guid gdApp)
		{
			foreach (ILibManItem toplevelLib in GetToplevelLibs(nProj, gdApp))
			{
				IProject projectFromLibManItem = GetProjectFromLibManItem(toplevelLib);
				if (projectFromLibManItem != null)
				{
					yield return projectFromLibManItem;
				}
			}
		}

		public static IProject GetProjectFromHandle(int nProj)
		{
			return Fun.Find<IProject>((Fun1<IProject, bool>)((IProject prj) => prj.Handle == nProj), (IEnumerable<IProject>)((IEngine)APEnvironment.Engine).Projects.Projects);
		}

		public static IList<ILibManItem> GetToplevelLibs(int nProj, Guid gdApp)
		{
			ILibManObject libMan = GetLibMan(nProj, gdApp);
			if (libMan == null)
			{
				return Fun.Tuple<ILibManItem>(Array.Empty<ILibManItem>());
			}
			return libMan.GetAllLibraries(false);
		}

		public static IProject GetProjectFromLibManItem(ILibManItem lmi)
		{
			Debug.Assert(lmi != null);
			IProject projectById = GetProjectById(GetProjectIdFromLibManItem(lmi));
			if (projectById == null || !projectById.Library)
			{
				return null;
			}
			return projectById;
		}

		public static IProject GetProjectById(string stId)
		{
			IProjects projects = ((IEngine)APEnvironment.Engine).Projects;
			if (string.IsNullOrEmpty(stId))
			{
				return projects.PrimaryProject;
			}
			return Fun.Find<IProject>((Fun1<IProject, bool>)((IProject prj) => StrEqCI(stId, prj.Id)), (IEnumerable<IProject>)projects.Projects);
		}

		private static string GetProjectIdFromLibManItem(ILibManItem lmi)
		{
			Debug.Assert(lmi != null);
			IManagedLibrary managedLibFromLibManItem = GetManagedLibFromLibManItem(lmi);
			if (managedLibFromLibManItem != null)
			{
				return managedLibFromLibManItem.DisplayName;
			}
			return lmi.Name;
		}

		public static IManagedLibrary GetManagedLibFromLibManItem(ILibManItem lmi)
		{
			Debug.Assert(lmi != null);
			try
			{
				IPlaceholderLibManItem val = (IPlaceholderLibManItem)(object)((lmi is IPlaceholderLibManItem) ? lmi : null);
				IManagedLibManItem val2 = (IManagedLibManItem)(object)((lmi is IManagedLibManItem) ? lmi : null);
				if (val != null && val.EffectiveResolution != null)
				{
					return val.EffectiveResolution;
				}
				if (val2 != null && val2.ManagedLibrary != null)
				{
					return val2.ManagedLibrary;
				}
			}
			catch
			{
			}
			return null;
		}

		public static bool StrEqCI(string st1, string st2)
		{
			return StrCmpCI(st1, st2) == 0;
		}

		public static int StrCmpCI(string st1, string st2)
		{
			return string.Compare(st1, st2, StringComparison.InvariantCultureIgnoreCase);
		}

		public static ILibManObject GetLibMan(int nProj, Guid gdApp)
		{
			IList<IMetaObjectStub> list = Fun.ToList<IMetaObjectStub>(OH.EnumChildrenTS<ILibManObject>(nProj, gdApp));
			if (list.Count != 0)
			{
				return OH.Resolve<ILibManObject>(list[0]);
			}
			return null;
		}
	}
}
