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
    internal class RxStructsGetter : IRxMetaAlgorithm
    {
        private List<RxStructCodeData>? GetItems(PropertyInfo[] properties, object instance)
        {
            var items = new List<RxStructCodeData>();
            foreach (var prop in properties)
            {
                if (!prop.CanWrite && ReflectionHelpers.IsVirtual(prop))
                {
                    bool nullable = false;
                    string? propTypeName = prop.PropertyType.FullName;
                    if (propTypeName == null)
                        propTypeName = prop.Name;
                    Type? propType = ReflectionHelpers.GetNullableType(prop);
                    if (propType != null)
                    {
                        nullable = true;
                        propTypeName = propType.FullName;
                        if (propTypeName == null)
                            propTypeName = prop.Name;
                    }
                    else
                    {
                        propType = prop.PropertyType;
                    }
                    int array = -1;
                    Type? enumType = ReflectionHelpers.GetEnumerableElement(prop.PropertyType);
                    if (enumType != null)
                    {
                        propType = enumType;
                        array = 0;
                    }
                    RxStructCodeData data = new RxStructCodeData()
                    {
                        name = prop.Name,
                        isNullAble = nullable,
                        codeType = propTypeName,
                        itemId = prop.Name,
                        array = array
                    };
                    items.Add(data);
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
                var met = ReflectionHelpers.GetStructPropertyInfos(objType.type);
                var structs = GetItems(met, instance);
                if (structs == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.definedStructs = structs.ToArray();
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