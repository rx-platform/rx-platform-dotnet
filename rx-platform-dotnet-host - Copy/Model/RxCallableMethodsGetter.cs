using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using RxPlatform.Hosting.Interface;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml.Linq;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{

    internal class RxCallableMethodsGetter : IRxMetaAlgorithm
    {
        public void FillTypes(PlatformTypeBuildData data)
        {
        }
    }
}