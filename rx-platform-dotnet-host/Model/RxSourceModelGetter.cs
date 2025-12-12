using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using System.Text;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{

    internal class RxSourceModelGetter : IRxMetaAlgorithm
    {
        private List<SourceWriteMethodData>? GetItems(Type type, MethodInfo[] methods)
        {
            StringBuilder connectionsBuilder = new StringBuilder();
            StringBuilder initialDataBuilder = new StringBuilder();
            var items = new List<SourceWriteMethodData>();
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != 2)
                    continue;
                if (!ReflectionHelpers.IsRxPlatformResultDelegate(parameters[1].ParameterType))
                    continue;
                var paramType = parameters[0].ParameterType;
                switch(Type.GetTypeCode(paramType))
                {
                    case TypeCode.Boolean:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.Bool,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.SByte:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.Int8,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.Byte:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.UInt8,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.Int16:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.Int16,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.UInt16:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.UInt16,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.Int32:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.Int32,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.UInt32:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.UInt32,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.Int64:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.Int64,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.UInt64:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.UInt64,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.Single:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.Float,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.Double:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.Double,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.String:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.String,
                            methodInfo = method
                        });
                        break;
                    case TypeCode.DateTime:
                        items.Add(new SourceWriteMethodData
                        {
                            typeCode = (byte)rx_value_t.Time,
                            methodInfo = method
                        });
                        break;
                    default:
                        {
                            if(paramType == typeof(Guid))
                            {
                                items.Add(new SourceWriteMethodData
                                {
                                    typeCode = (byte)rx_value_t.Uuid,
                                    methodInfo = method
                                });
                            }
                            else if(paramType == typeof(byte[]))
                            {
                                items.Add(new SourceWriteMethodData
                                {
                                    typeCode = (byte)rx_value_t.Bytes,
                                    methodInfo = method
                                });
                            }
                            else if (paramType.IsEnum)
                            {
                                items.Add(new SourceWriteMethodData
                                {
                                    typeCode = (byte)rx_value_t.Int8,
                                    methodInfo = method
                                });
                            }
                            else
                            {
                                if(paramType.GetCustomAttribute<RxPlatformDataType>() == null)
                                {
                                    // not a rx data type
                                    continue;
                                }
                                // complex type
                                items.Add(new SourceWriteMethodData
                                {
                                    typeCode = (byte)rx_value_t.Struct,
                                    methodInfo = method
                                });
                            }
                        }
                        break;

                }

            }
            return items;
        }
        private void FillTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformSourceType>> data)
        {
            foreach (var kvp in data)
            {
                if (!kvp.Value.valid)
                    continue;
                if (!kvp.Value.runtimeType)
                    continue;

                var objType = kvp.Value;

                if (objType.type == null)
                {
                    objType.valid = false;
                    continue;
                }
                var methods = ReflectionHelpers.GetSourceWriteMethods(objType.type);
                var items = GetItems(objType.type, methods);
                if (items == null)
                {
                    objType.valid = false;
                    continue;
                }
                
                objType.sourceWriteMethods = items.ToArray();
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {

            FillTypes(data.SourceTypes);

        }
    }
}