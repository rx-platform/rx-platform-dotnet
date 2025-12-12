
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model;
using ENSACO.RxPlatform.Model;

namespace ENSACO.RxPlatform.Hosting.Construction
{
    static class RuntimeConstructAlgorithms
    {
        static void DumpRuntimeRecursive(RxRutimeConstructData data, string path, string tabs)
        {
            Console.WriteLine($"{tabs}Runtime data: Path:{path} Type={data.type}, NativePtr={data._nativePtr}");
            foreach (var kvp in data.structs)
            {
                DumpRuntimeRecursive(kvp.Value, path + "." + kvp.Key, tabs + "\t");
            }
        }
        static void DumpRuntimeConstructionData(string prefix)
        {
            lock (RxMetaData.Instance.TypesLock)
            {
                Console.WriteLine($"Dumping construction data {prefix}\r\n*************************************");
                foreach (var item in RxMetaData.Instance.RuntimeConstruction)
                {
                    Console.WriteLine($"Dumping data for nodeId {item.Key}");
                    DumpRuntimeRecursive(item.Value, "", "\t");
                }
                Console.WriteLine("*************************************");
            }
        }
        internal static void RemoveFromConstructionData(RxNodeId id)
        {
            lock (RxMetaData.Instance.TypesLock)
            {
                //DumpRuntimeConstructionData("Before");
                //Console.WriteLine($"Removing construction data for nodeId {id}");
                var constructionData = RxMetaData.Instance.RuntimeConstruction;
                if (constructionData != null && constructionData.ContainsKey(id))
                {
                    constructionData.Remove(id);
                }
                //DumpRuntimeConstructionData("After");
            }
        }
        internal static bool TryGetConstructionData(RxNodeId id, string path, out RxRutimeConstructData? data)
        {
            var constructionData = RxMetaData.Instance.RuntimeConstruction;
            if (constructionData != null && constructionData.TryGetValue(id, out data))
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var paths = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in paths)
                    {
                        if (data.structs.TryGetValue(p, out var subData) == true)
                        {
                            data = subData;
                        }
                        else
                        {
                            data = default;
                            return false;
                        }
                    }
                }
                return data != null;
            }
            data = default;
            return false;
        }
        internal static void AddToConstructionData(rx_item_type type, RxNodeId id, string path, IntPtr nativePtr, Dictionary<RxNodeId, RxRutimeConstructData> constructionData)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (!constructionData.TryGetValue(id, out var data))
                {
                    data = new RxRutimeConstructData
                    {
                        type = rx_item_type.rx_object,
                        _nativePtr = nativePtr
                    };
                    constructionData[id] = data;
                }
                else
                {
                    data.type = type;
                    data._nativePtr = nativePtr;
                }
            }
            else
            {
                RxRutimeConstructData? data = null;
                var paths = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (!constructionData.TryGetValue(id, out data))
                {
                    data = new RxRutimeConstructData
                    {
                        type = rx_item_type.rx_object
                    };
                    constructionData[id] = data;
                }
                if (data != null)
                {
                    foreach (var p in paths)
                    {
                        if (!data.structs.TryGetValue(p, out var subData))
                        {
                            subData = new RxRutimeConstructData
                            {
                                type = rx_item_type.rx_struct_type
                            };
                            data.structs[p] = subData;
                        }
                        data = subData;
                    }
                    data.type = type;
                    data._nativePtr = nativePtr;
                }
            }
            
        }
    }

}