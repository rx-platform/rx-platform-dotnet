using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{
    internal class RxOwnMethodsGetter : IRxMetaAlgorithm
    {
        private Tuple<List<RxMethodDataItem>, List<RxOwnMethodCodeData>>? GetItems(MethodInfo[] methods, object instance)
        {
            var items = new Tuple<List<RxMethodDataItem>, List<RxOwnMethodCodeData>>(new List<RxMethodDataItem>(), new List<RxOwnMethodCodeData>());
            foreach (var method in methods)
            {
                Type? paramType = null;
                Type? resultType = null;

                bool isAsync = false;
                string? argTypeName = null;
                bool argIsNullable = false;
                bool argIsJson = false;
                string? argDefaultValue = null;

                string? resultTypeName = null;
                bool resultIsNullable = false;
                bool resultIsJson = false;

                var parmsInfo = method.GetParameters();
                if (parmsInfo == null || parmsInfo.Length == 0)
                {
                    paramType = typeof(void);
                    argTypeName = "";// void type
                    argIsNullable = false;
                    argIsJson = false;
                    argDefaultValue = null;
                }
                else if (parmsInfo != null && parmsInfo.Length == 1)
                {
                    paramType = parmsInfo[0].ParameterType;
                    if (paramType != null)
                    {
                        if (parmsInfo[0].HasDefaultValue)
                        {
                            argDefaultValue = RxMemoryCompiler.ValueToSourceCode(parmsInfo[0].DefaultValue, paramType);
                        }
                        argTypeName = paramType.FullName;
                        Type? argType = ReflectionHelpers.GetNullableType(parmsInfo[0]);
                        if (argType != null)
                        {
                            argIsNullable = true;
                            argTypeName = argType.FullName;
                            paramType = argType;

                        }
                        else
                        {
                            argIsNullable = false;
                            paramType = parmsInfo[0].ParameterType;
                            argTypeName = paramType.FullName;

                        }
                        argIsJson = paramType.GetCustomAttribute<RxPlatformDataType>(false) != null;
                    }
                }
                resultType = method.ReturnType;
                if (resultType != null)
                {
                    var type = ReflectionHelpers.GetTaskType(resultType);
                    if(type!= null)
                    {
                        resultType = type;
                        isAsync = true;

                    }
                    if (resultType == typeof(void))
                    {
                        resultTypeName = isAsync ? typeof(Task).FullName : "void";// void type
                        resultIsNullable = false;
                        resultIsJson = false;
                    }
                    else
                    {

                        Type? resType = ReflectionHelpers.GetNullableType(method.ReturnParameter);
                        if (resType != null)
                        {
                            resultIsNullable = true;
                            resultTypeName = resType.FullName;
                            resultType = resType;
                        }
                        else
                        {
                            resultIsNullable = false;
                            resultTypeName = resultType.FullName;
                            resultType = method.ReturnType;
                        }
                        resultIsJson = resultType.GetCustomAttribute<RxPlatformDataType>(false) != null;

                    }
                }
                if (paramType != null && resultType != null)
                {
                    string? nodeIdIn = null;
                    if (paramType == typeof(void))
                    {
                        unsafe
                        {
                            string_value_struct nodeIdStr;
                            rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_CLASS_DATA_BASE_ID);
                            if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                            {
                                nodeIdIn = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                                CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                            }
                            CommonInterface.rx_destory_node_id(&nodeId);
                        }
                    }
                    else
                    {
                        var attrIn = paramType.GetCustomAttribute<RxPlatformDataType>(false);
                        if (attrIn != null)
                        {
                            nodeIdIn = attrIn.NodeId.ToString();
                        }
                    }
                    if (nodeIdIn == null)
                        continue;

                    string? nodeIdOut = null;
                    if (resultType == typeof(void))
                    {
                        unsafe
                        {
                            string_value_struct nodeIdStr;
                            rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_CLASS_DATA_BASE_ID);
                            if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                            {
                                nodeIdOut = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                                CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                            }
                            CommonInterface.rx_destory_node_id(&nodeId);
                        }
                    }
                    else
                    {
                        var attrIn = resultType.GetCustomAttribute<RxPlatformDataType>(false);
                        if (attrIn != null)
                        {
                            nodeIdOut = attrIn.NodeId.ToString();
                        }
                    }
                    if (nodeIdOut == null)
                        continue;

                    string? targetIdStr = null;
                    unsafe
                    {
                        string_value_struct nodeIdStr;
                        rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_DOTNET_METHOD_TYPE_ID);
                        if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                        {
                            targetIdStr = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                            CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                        }
                        CommonInterface.rx_destory_node_id(&nodeId);
                    }
                    if(targetIdStr == null)
                        continue;

                    RxMethodDataItem methodDataItem = new RxMethodDataItem
                    {
                        name = method.Name,
                        inType = new RXHostReferenceId { id = nodeIdIn },
                        outType = new RXHostReferenceId { id = nodeIdOut },
                        description = "",
                        target = new RXHostReferenceId { id = targetIdStr }

                    };
                    RxOwnMethodCodeData methodCodeData = new RxOwnMethodCodeData
                    {
                        name = method.Name,
                        isNullAbleArgument = argIsNullable,
                        argumentType = argTypeName,
                        defaultValue = argDefaultValue,
                        jsonArgument = argIsJson,
                        isNullAbleResult = resultIsNullable,
                        resultType = resultTypeName,
                        isAsync = isAsync


                        //isAsync = ReflectionHelpers.IsAsyncMethod(method),
                    };
                    items.Item1.Add(methodDataItem);
                    items.Item2.Add(methodCodeData);
                }
            }
            return items;
        }
        private void FillTypes<T>(Dictionary<RxNodeId, PlatformTypeBuildMeta<T>> data) where T : RxPlatformTypeAttribute
        {
            foreach (var kvp in data)
            {
                if (!kvp.Value.valid)
                    continue;
                if (!kvp.Value.definedType)
                    continue;
                if (!kvp.Value.runtimeType)
                    continue;

                var objType = kvp.Value;

                if (objType.type == null || objType.defaultConstructor == null)
                {
                    objType.valid = false;
                    continue;
                }
                object? instance = objType.defaultConstructor();
                if (instance == null)
                {
                    objType.valid = false;
                    continue;
                }
                var met = ReflectionHelpers.GetDefinedMethods(objType.type);
                var methods = GetItems(met, instance);
                if (methods == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.methods = methods.Item1.ToArray();
                objType.definedMethods = methods.Item2.ToArray();
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {
            FillTypes(data.ObjectTypes);
            FillTypes(data.PortTypes);
            FillTypes(data.DomainTypes);
            FillTypes(data.ApplicationTypes);

        }
    }
}