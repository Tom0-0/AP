using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{ac0cf67b-9fcf-4cf4-94fd-96aee12373a3}")]
	public class ConnectorObjectFactory : IObjectFactory
	{
		public Guid Namespace => Guid.Empty;

		public string Name => "Connector";

		public string Description => "Connector";

		public Icon SmallIcon
		{
			get
			{
				Icon icon = Icon;
				if (icon != null)
				{
					return new Icon(icon, new Size(16, 16));
				}
				return null;
			}
		}

		public Icon LargeIcon
		{
			get
			{
				Icon icon = Icon;
				if (icon != null)
				{
					return new Icon(icon, new Size(32, 32));
				}
				return null;
			}
		}

		public Control WizardPage => new Control();

		public Control ObjectNameControl => new Control();

		public string ObjectBaseName => "Connector";

		public Type ObjectType => typeof(ExplicitConnector);

		public Type[] EmbeddedObjectTypes => null;

		private Icon Icon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceObject.Resources.MasterConnectorIconSmall.ico");

		public IObject Create()
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public void ObjectCreated(int nProjectHandle, Guid guidObject)
		{
		}

		public IObject Create(string[] stBatchArguments)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public bool AcceptsParentObject(IObject parentObject)
		{
			return false;
		}
	}
}
