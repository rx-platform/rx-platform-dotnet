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

    internal class RxDisplayModelGetter : IRxMetaAlgorithm
    {
        private List<MethodInfo>? GetItems(Type type, MethodInfo[] methods)
        {
            StringBuilder connectionsBuilder = new StringBuilder();
            StringBuilder initialDataBuilder = new StringBuilder();
            var items = new List<MethodInfo>();
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    continue;
                if (parameters[0].ParameterType != typeof(HttpRequestMessage))
                    continue;
                var paramType = typeof(HttpRequestMessage);

                if(method.ReturnType != typeof(Task<HttpResponseMessage>))
                    continue;
                
                items.Add(method);

            }
            return items;
        }
        private void FillTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformDisplayType>> data)
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
                var methods = ReflectionHelpers.GetRequestHandlers(objType.type);
                var items = GetItems(objType.type, methods);
                if (items == null)
                {
                    objType.valid = false;
                    continue;
                }
                
                objType.requestHandlingMethods = items.ToArray();
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {

            FillTypes(data.DisplayTypes);

        }
    }
}