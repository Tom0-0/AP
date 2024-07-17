#define DEBUG
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Diagnostics;
using System.Text;

namespace _3S.CoDeSys.OnlineCommands
{
    internal static class Common
    {
        private static readonly char[] DOT = new char[1] { '.' };

        internal static void SplitInstancePath(string stInstancePath, out string stDevice, out string stApplication, out Guid applicationGuid)
        {
            Debug.Assert(stInstancePath != null);
            stDevice = null;
            stApplication = null;
            applicationGuid = Guid.Empty;
            string[] array = stInstancePath.Split(DOT, 3);
            if (array == null || array.Length < 3)
            {
                return;
            }
            stDevice = array[0];
            if (ExistsApplication(stDevice, array[1], out applicationGuid))
            {
                stApplication = array[1];
                return;
            }
            IDeviceObject deviceObject = GetDeviceObject(stDevice);
            if (deviceObject != null)
            {
                applicationGuid = ((IObject)deviceObject).MetaObject.ObjectGuid;
            }
        }

        private static bool ExistsApplication(string stDevice, string stApplication, out Guid applicationGuid)
        {
            //IL_0067: Unknown result type (might be due to invalid IL or missing references)
            Debug.Assert(stDevice != null);
            Debug.Assert(stApplication != null);
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle, stApplication);
            foreach (Guid guid in allObjects)
            {
                IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, guid);
                if (objectToRead == null || !(objectToRead.Object is IApplicationObject))
                {
                    continue;
                }
                Guid deviceGuid = ((IOnlineApplicationObject)(IApplicationObject)objectToRead.Object).DeviceGuid;
                if (deviceGuid != Guid.Empty)
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, deviceGuid);
                    if (metaObjectStub != null && metaObjectStub.Name.Equals(stDevice))
                    {
                        applicationGuid = guid;
                        return true;
                    }
                }
            }
            applicationGuid = Guid.Empty;
            return false;
        }

        private static IDeviceObject GetDeviceObject(string stDevice)
        {
            //IL_006d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0073: Expected O, but got Unknown
            Debug.Assert(stDevice != null);
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle, stDevice);
            Debug.Assert(allObjects != null);
            Guid[] array = allObjects;
            foreach (Guid guid in array)
            {
                IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, guid);
                Debug.Assert(objectToRead != null);
                if (objectToRead.Object is IDeviceObject)
                {
                    return (IDeviceObject)objectToRead.Object;
                }
            }
            return null;
        }

        internal static Guid GetApplicationGuid(int projectHandle, Guid objectGuid)
        {
            Guid result = Guid.Empty;
            Guid guid = objectGuid;
            while (guid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(projectHandle, guid))
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, guid);
                Debug.Assert(metaObjectStub != null);
                if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    result = guid;
                    break;
                }
                guid = metaObjectStub.ParentObjectGuid;
            }
            return result;
        }

        private static bool ToString(IExpression exp, StringBuilder strb, IScope scope)
        {
            if (exp is ICompoAccessExpression)
            {
                if (!ToString(((ICompoAccessExpression)((exp is ICompoAccessExpression) ? exp : null)).Left, strb, scope))
                {
                    return false;
                }
                strb.Append(".");
                if (!ToString(((ICompoAccessExpression)((exp is ICompoAccessExpression) ? exp : null)).Right, strb, scope))
                {
                    return false;
                }
            }
            else if (exp is IIndexAccessExpression)
            {
                IIndexAccessExpression val = (IIndexAccessExpression)(object)((exp is IIndexAccessExpression) ? exp : null);
                if (!ToString(val.Var, strb, scope))
                {
                    return false;
                }
                strb.Append("[");
                bool flag = true;
                IExpression[] accesses = val.Accesses;
                int value = default(int);
                foreach (IExpression obj in accesses)
                {
                    if (!flag)
                    {
                        strb.Append(",");
                    }
                    ILiteralValue val2 = obj.Literal(scope);
                    if (val2 != null && val2.GetInt(out value))
                    {
                        strb.Append(value);
                        continue;
                    }
                    return false;
                }
                strb.Append("]");
            }
            else
            {
                if (!(exp is IVariableExpression))
                {
                    return false;
                }
                strb.Append(((IExprement)exp).ToString());
            }
            return true;
        }

        internal static string NormalizeInstancePath(Guid applicationGuid, Guid guidObject, string stInstancePathIn)
        {
            try
            {
                IExpression val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateLanguageModelBuilder().ParseExpression(stInstancePathIn);
                if (val == null)
                {
                    return stInstancePathIn;
                }
                StringBuilder stringBuilder = new StringBuilder();
                ISignature signature = ((ICompileContextCommon)((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(applicationGuid)).GetSignature(guidObject);
                if (signature == null)
                {
                    return stInstancePathIn;
                }
                IScope val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScope(signature, applicationGuid);
                IExpressionTypifier obj = ((ILanguageModelManager28)APEnvironment.LanguageModelMgr).CreateTypifier(applicationGuid, val2, false, false);
                ((IExpressionTypifier2)((obj is IExpressionTypifier4) ? obj : null)).TypifyExpression(val);
                if (ToString(val, stringBuilder, val2))
                {
                    return stringBuilder.ToString();
                }
                return stInstancePathIn;
            }
            catch
            {
                return stInstancePathIn;
            }
        }
    }
}
