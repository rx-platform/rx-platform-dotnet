using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;
using System.Text;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{

    internal class RxInitialDataFill : IRxMetaAlgorithm
    {
        
        private void FillTypes(Dictionary<RxNodeId, PlatformDataTypeBuildMeta> data)
        {
            foreach (var kvp in data)
            {
                var objType = kvp.Value;
                if (!objType.valid)
                    continue;
                if (objType.type == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.defaultConstructor = ReflectionHelpers.CreateConstructorFunc(objType.type);

                if (objType.defaultConstructor == null)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxInitialDataFill", 100
                        , $"Class {objType.type.FullName} does not have accessible default constructor function! Ignoring type definition.");
                    objType.valid = false;
                    continue;
                }
                data[kvp.Key] = objType;
            }
        }
        private void FillTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformVariableType>> data)
        {
            foreach (var kvp in data)
            {
                var objType = kvp.Value;
                if (!objType.valid)
                    continue;
                if (objType.type == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.defaultConstructor = ReflectionHelpers.CreateConstructorFunc(objType.type);

                if (objType.defaultConstructor == null)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxInitialDataFill", 100
                        , $"Class {objType.type.FullName} does not have accessible default constructor function! Ignoring type definition.");
                    objType.valid = false;
                    continue;
                }
                objType.mappers = new RxMapperDataItem[0];
                objType.filters = new RxFilterDataItem[0];
                objType.sources = new RxSourceDataItem[0];
                objType.displays = new RxDisplayDataItem[0];
                data[kvp.Key] = objType;
            }
        }
        private void FillTypes<T>(Dictionary<RxNodeId, PlatformTypeBuildMeta<T>> data) where T : RxPlatformTypeAttribute
        {
            foreach (var kvp in data)
            {
                var objType = kvp.Value;
                if (!objType.valid)
                    continue;
                if (objType.type == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.defaultConstructor = ReflectionHelpers.CreateConstructorFunc(objType.type);

                if (objType.defaultConstructor == null)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxInitialDataFill", 100
                        , $"Class {objType.type.FullName} does not have accessible default constructor function! Ignoring type definition.");
                    objType.valid = false;
                    continue;
                }
                RxPlatformRuntimeBase? instance = objType.defaultConstructor.Invoke() as RxPlatformRuntimeBase;
                if (instance != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(objType.type.Assembly.FullName);
                    sb.Append("|");
                    sb.Append(objType.type.FullName);

                    objType.codeInfo = sb.ToString();// instance.__GetCodeInfo();
                }

                objType.methods = new RxMethodDataItem[0];
                objType.mappers = new RxMapperDataItem[0];
                objType.filters = new RxFilterDataItem[0];
                objType.sources = new RxSourceDataItem[0];
                if (objType.runtimeType)
                {
                    objType.codeNamespace = objType.type.Namespace;
                    MethodInfo? startMethod = objType.type.GetMethod("Started");
                    if (startMethod == null
                        || startMethod.ReturnType != typeof(void)
                        || startMethod.GetParameters().Length != 0)
                    {
                        RxPlatformObject.Instance.WriteLogWarning("RxInitialDataFill", 100
                            , $"Started method for runtime type {objType.path}/{objType.name} not found or has invalid return type.");
                        objType.startMethod = null;
                    }
                    else
                    {
                        objType.startMethod = startMethod;
                    }
                    MethodInfo? stopMethod = objType.type.GetMethod("Stopping");
                    if (stopMethod == null
                        || stopMethod.ReturnType != typeof(void)
                        || stopMethod.GetParameters().Length != 0)
                    {
                        RxPlatformObject.Instance.WriteLogWarning("PlatformRuntimeTypes.BuildPlatformTypes", 100
                            , $"Stopping method for runtime type {objType.path}/{objType.name} not found or has invalid return type.");
                        objType.stopMethod = null;
                    }
                    else
                    {
                        objType.stopMethod = stopMethod;
                    }
                }
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {
            FillTypes(data.DataTypes);

            FillTypes(data.EventTypes);
            FillTypes(data.SourceTypes);
            FillTypes(data.MapperTypes);
            FillTypes(data.FilterTypes);
            FillTypes(data.VariableTypes);
            FillTypes(data.StructTypes);
            FillTypes(data.DisplayTypes);

            FillTypes(data.ObjectTypes);
            FillTypes(data.PortTypes);
            FillTypes(data.DomainTypes);
            FillTypes(data.ApplicationTypes);

            //
        }
    }
}