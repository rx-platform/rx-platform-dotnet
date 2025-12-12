using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using RxPlatform.Hosting.Interface;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{
    internal class RxRelationsGetter : IRxMetaAlgorithm
    {
        private Tuple<List<RxRelationDataItem>, List<RxOwnRelationCodeData>>? GetItems(PropertyInfo[] properties, object instance)
        {
            var items = new Tuple<List<RxRelationDataItem>, List<RxOwnRelationCodeData>>(new List<RxRelationDataItem>(), new List<RxOwnRelationCodeData>());
            foreach (var property in properties)
            {
                //Type? paramType = null;
                //Type? resultType = null;

                //bool isAsync = ReflectionHelpers.IsAsyncMethod(method);

                //string? argTypeName = null;
                //bool argIsNullable = false;
                //bool argIsJson = false;
                //string? argDefaultValue = null;

                //string? resultTypeName = null;
                //bool resultIsNullable = false;
                //bool resultIsJson = false;

                //var parmsInfo = method.GetParameters();
                //if (parmsInfo == null || parmsInfo.Length == 0)
                //{
                //    paramType = typeof(void);
                //    argTypeName = "";// void type
                //    argIsNullable = false;
                //    argIsJson = false;
                //    argDefaultValue = null;
                //}
                //else if (parmsInfo != null && parmsInfo.Length == 1)
                //{
                //    paramType = parmsInfo[0].ParameterType;
                //    if (paramType != null)
                //    {
                //        if (parmsInfo[0].HasDefaultValue)
                //        {
                //            argDefaultValue = RxMemoryCompiler.ValueToSourceCode(parmsInfo[0].DefaultValue, paramType);
                //        }
                //        argTypeName = paramType.FullName;
                //        Type? argType = ReflectionHelpers.GetNullableType(parmsInfo[0]);
                //        if (argType != null)
                //        {
                //            argIsNullable = true;
                //            argTypeName = argType.FullName;
                //            paramType = argType;

                //        }
                //        else
                //        {
                //            argIsNullable = false;
                //            paramType = parmsInfo[0].ParameterType;
                //            argTypeName = paramType.FullName;

                //        }
                //        argIsJson = paramType.GetCustomAttribute<RxPlatformDataType>(false) != null;
                //    }
                //}
                //resultType = method.ReturnType;
                //if (resultType != null)
                //{
                //    if (resultType == typeof(void))
                //    {
                //        resultTypeName = "void";// void type
                //        resultIsNullable = false;
                //        resultIsJson = false;
                //    }
                //    else
                //    {
                //        Type? resType = ReflectionHelpers.GetNullableType(method.ReturnParameter);
                //        if (resType != null)
                //        {
                //            resultIsNullable = true;
                //            resultTypeName = resType.FullName;
                //            resultType = resType;
                //        }
                //        else
                //        {
                //            resultIsNullable = false;
                //            resultTypeName = resultType.FullName;
                //            resultType = method.ReturnType;
                //        }
                //        resultIsJson = resultType.GetCustomAttribute<RxPlatformDataType>(false) != null;
                //    }
                //}
                //if (paramType != null && resultType != null)
                //{
                //    string? nodeIdIn = null;
                //    if (paramType == typeof(void))
                //    {
                //        unsafe
                //        {
                //            string_value_struct nodeIdStr;
                //            rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_CLASS_DATA_BASE_ID);
                //            if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                //            {
                //                nodeIdIn = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                //                CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                //            }
                //            CommonInterface.rx_destory_node_id(&nodeId);
                //        }
                //    }
                //    else
                //    {
                //        var attrIn = paramType.GetCustomAttribute<RxPlatformDataType>(false);
                //        if (attrIn != null)
                //        {
                //            nodeIdIn = attrIn.NodeId.ToString();
                //        }
                //    }
                //    if (nodeIdIn == null)
                //        continue;

                //    string? nodeIdOut = null;
                //    if (resultType == typeof(void))
                //    {
                //        unsafe
                //        {
                //            string_value_struct nodeIdStr;
                //            rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_CLASS_DATA_BASE_ID);
                //            if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                //            {
                //                nodeIdOut = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                //                CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                //            }
                //            CommonInterface.rx_destory_node_id(&nodeId);
                //        }
                //    }
                //    else
                //    {
                //        var attrIn = resultType.GetCustomAttribute<RxPlatformDataType>(false);
                //        if (attrIn != null)
                //        {
                //            nodeIdOut = attrIn.NodeId.ToString();
                //        }
                //    }
                //    if (nodeIdOut == null)
                //        continue;

                //    string? targetIdStr = null;
                //    unsafe
                //    {
                //        string_value_struct nodeIdStr;
                //        rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_DOTNET_METHOD_TYPE_ID);
                //        if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                //        {
                //            targetIdStr = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                //            CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                //        }
                //        CommonInterface.rx_destory_node_id(&nodeId);
                //    }
                //    if(targetIdStr == null)
                //        continue;

                //    RxRelationDataItem methodDataItem = new RxRelationDataItem
                //    {
                //        name = method.Name,
                //        inType = new RXHostReferenceId { id = nodeIdIn },
                //        outType = new RXHostReferenceId { id = nodeIdOut },
                //        description = "",
                //        target = new RXHostReferenceId { id = targetIdStr }

                //    };
                //    RxOwnRelationCodeData relCodeData = new RxOwnRelationCodeData
                //    { 
                //        // todo here fill stuff
                //    };
                //    items.Item1.Add(methodDataItem);
                //    items.Item2.Add(relCodeData);
                
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

                var objType = kvp.Value;
                if (!objType.valid)
                    continue;
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
                var props = ReflectionHelpers.GetRelationsPropertyInfos(objType.type);
                var relations = GetItems(props, instance);
                if (relations == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.relations = relations.Item1.ToArray();
                objType.definedRelations = relations.Item2.ToArray();
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {

            FillTypes(data.EventTypes);
            FillTypes(data.SourceTypes);
            FillTypes(data.MapperTypes);
            FillTypes(data.FilterTypes);
            FillTypes(data.VariableTypes);
            FillTypes(data.StructTypes);

            FillTypes(data.ObjectTypes);
            FillTypes(data.PortTypes);
            FillTypes(data.DomainTypes);
            FillTypes(data.ApplicationTypes);

        }
    }
}