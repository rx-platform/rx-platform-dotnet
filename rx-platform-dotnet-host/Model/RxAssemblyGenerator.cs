using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Algorithms;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ENSACO.RxPlatform.Hosting.Model
{
    internal static class RxAssemblyGenerator
    {
        private static void ConnectMapperType(string typeName, ref PlatformTypeBuildMeta<RxPlatformMapperType> type, Assembly assembly)
        {
            Type? dynamicType = assembly.GetType(typeName, false);
            if (dynamicType != null)
            {
                type.runtimeConstructor = ReflectionHelpers.CreateConstructorFunc(dynamicType);
                if (type.runtimeConstructor == null)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxAssemblyGenerator.ConnectMapperType", 100
                        , $"Default constructor for runtime type {type.codeNamespace}.{type.name} not found.");
                }
            }
        }
        private static void ConnectEventType(string typeName, ref PlatformTypeBuildMeta<RxPlatformEventType> type, Assembly assembly)
        {
            Type? dynamicType = assembly.GetType(typeName, false);
            if (dynamicType != null)
            {
                type.runtimeConstructor = ReflectionHelpers.CreateConstructorFunc(dynamicType);
                if (type.runtimeConstructor == null)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxAssemblyGenerator.ConnectEventType", 100
                        , $"Default constructor for runtime type {type.codeNamespace}.{type.name} not found.");
                }
            }
        }
        private static void ConnectDisplayType(string typeName, ref PlatformTypeBuildMeta<RxPlatformDisplayType> type, Assembly assembly)
        {
            Type? dynamicType = assembly.GetType(typeName, false);
            if (dynamicType != null)
            {
                type.runtimeConstructor = ReflectionHelpers.CreateConstructorFunc(dynamicType);
                if (type.runtimeConstructor == null)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxAssemblyGenerator.ConnectDisplayType", 100
                        , $"Default constructor for runtime type {type.codeNamespace}.{type.name} not found.");
                }
            }
        }
        private static void ConnectSourceType(string typeName, ref PlatformTypeBuildMeta<RxPlatformSourceType> type, Assembly assembly)
        {
            Type? dynamicType = assembly.GetType(typeName, false);
            if (dynamicType != null)
            {
                type.runtimeConstructor = ReflectionHelpers.CreateConstructorFunc(dynamicType);
                if (type.runtimeConstructor == null)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxAssemblyGenerator.ConnectSourceType", 100
                        , $"Default constructor for runtime type {type.codeNamespace}.{type.name} not found.");
                }
            }
        }

        private static void ConnectStructType(string typeName, ref PlatformTypeBuildMeta<RxPlatformStructType> type, Assembly assembly)
        {
            Type? dynamicType = assembly.GetType(typeName, false);
            if (dynamicType != null)
            {
                type.runtimeConstructor = ReflectionHelpers.CreateConstructorFunc(dynamicType);
                if (type.runtimeConstructor == null)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxAssemblyGenerator.ConnectStructType", 100
                        , $"Default constructor for runtime type {type.codeNamespace}.{type.name} not found.");
                }
            }
        }
        private static void ConnectObjectType(string typeName, ref PlatformTypeBuildMeta<RxPlatformObjectType> type, Assembly assembly)
        {
            Type? dynamicType = assembly.GetType(typeName, false);
            if (dynamicType != null)
            {
                type.runtimeConstructor = ReflectionHelpers.CreateConstructorFunc(dynamicType);
                if(type.runtimeConstructor == null)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxAssemblyGenerator.ConnectObjectType", 100
                        , $"Default constructor for runtime type {type.codeNamespace}.{type.name} not found.");
                }
                type.startMethod = dynamicType.GetMethod("Started");
                if (type.startMethod == null
                    || type.startMethod.ReturnType != typeof(void)
                    || type.startMethod.GetParameters().Length != 0)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxAssemblyGenerator.ConnectObjectType", 100
                        , $"Started method for runtime type {type.codeNamespace}.{type.name} not found or has invalid return type.");
                    type.startMethod = null;
                }
                type.stopMethod = dynamicType.GetMethod("Stopping");
                if (type.stopMethod == null
                    || type.stopMethod.ReturnType != typeof(void)
                    || type.stopMethod.GetParameters().Length != 0)
                {
                    RxPlatformObject.Instance.WriteLogWarning("RxAssemblyGenerator.ConnectObjectType", 100
                        , $"Stopping method for runtime type {type.codeNamespace}.{type.name} not found or has invalid return type.");
                    type.stopMethod = null;
                }
            }
        }
        private static void ConnectTypes(PlatformTypeBuildData tempData, Assembly assembly)
        {
            foreach (var kvp in tempData.ObjectTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType || kvp.Value.codeNamespace == null)
                    continue;
                string typeName = $"{kvp.Value.codeNamespace}{RxMemoryCompiler.overridePostfix}.{kvp.Value.name}";
                var type = kvp.Value;
                ConnectObjectType(typeName, ref type, assembly);
                tempData.ObjectTypes[kvp.Key] = type;
            }
            foreach (var kvp in tempData.StructTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType || kvp.Value.codeNamespace == null)
                    continue;
                string typeName = $"{kvp.Value.codeNamespace}{RxMemoryCompiler.overridePostfix}.{kvp.Value.name}";
                var type = kvp.Value;
                ConnectStructType(typeName, ref type, assembly);
                tempData.StructTypes[kvp.Key] = type;
            }
            foreach (var kvp in tempData.SourceTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType || kvp.Value.codeNamespace == null)
                    continue;
                string typeName = $"{kvp.Value.codeNamespace}{RxMemoryCompiler.overridePostfix}.{kvp.Value.name}";
                var type = kvp.Value;
                ConnectSourceType(typeName, ref type, assembly);
                tempData.SourceTypes[kvp.Key] = type;
            }
            foreach (var kvp in tempData.MapperTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType || kvp.Value.codeNamespace == null)
                    continue;
                string typeName = $"{kvp.Value.codeNamespace}{RxMemoryCompiler.overridePostfix}.{kvp.Value.name}";
                var type = kvp.Value;
                ConnectMapperType(typeName, ref type, assembly);
                tempData.MapperTypes[kvp.Key] = type;
            }
            foreach (var kvp in tempData.EventTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType || kvp.Value.codeNamespace == null)
                    continue;
                string typeName = $"{kvp.Value.codeNamespace}{RxMemoryCompiler.overridePostfix}.{kvp.Value.name}";
                var type = kvp.Value;
                ConnectEventType(typeName, ref type, assembly);
                tempData.EventTypes[kvp.Key] = type;
            }
            foreach (var kvp in tempData.DisplayTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType || kvp.Value.codeNamespace == null)
                    continue;
                string typeName = $"{kvp.Value.codeNamespace}{RxMemoryCompiler.overridePostfix}.{kvp.Value.name}";
                var type = kvp.Value;
                ConnectDisplayType(typeName, ref type, assembly);
                tempData.DisplayTypes[kvp.Key] = type;
            }
        }
        internal static void GenerateAssembly(PlatformTypeBuildData tempData, HostedPlatformLibrary hostLib)
        {
            var parentAssembly = hostLib.GetAssembly();
            if (parentAssembly == null)
                throw new Exception("HostedPlatformLibrary assembly is null in RxAssemblyGenerator.GenerateAssembly.");
            

            var sourceCode = GetTypesSourceCode(tempData, hostLib);

            if(System.Diagnostics.Debugger.IsAttached)
            {
                // For debugging purposes, write the generated code to a file
                string debugPath = @"C:\RX\Native\dotnet\DynamicAssembly\DynamicCode.cs";
                File.WriteAllText(debugPath, sourceCode);
            }

            // Create a syntax tree from the source code
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            // Get references to necessary assemblies
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();


            // Add System.Runtime for basic types
            references.Add(MetadataReference.CreateFromImage(hostLib.GetAssemblyData()));

            string? assemblyName = hostLib.GetAssemblyName();

            // Create a compilation
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                // Emit the compilation into the memory stream
                var ret = compilation.Emit(ms);

                if (!ret.Success)
                {
                    foreach (var diagnostic in ret.Diagnostics)
                    {
                        RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BuildPlatformTypes", 100
                            , $"Error compiling dynamic assembly for assembly {parentAssembly.GetName().Name}: {diagnostic.ToString()}");
                    }
                }
                // Load the assembly from memory
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = hostLib.GetLoadContext()?.LoadFromStream(ms);

                if (assembly == null)
                {
                    throw new Exception("Failed to load assembly from memory.");
                }

                ConnectTypes(tempData, assembly);

            }
            // Further assembly generation logic goes here
        }

        private static string GetTypesSourceCode(PlatformTypeBuildData tempData, HostedPlatformLibrary hostLib)
        {
            StringBuilder codeStream = new StringBuilder();
            codeStream.AppendLine("// Auto-generated code for RxPlatform Types");
            codeStream.AppendLine($"// {DateTime.Now.ToString("yyyy.MM.ddTHH:mm:ss")}");
            codeStream.AppendLine(@"
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.Json;

");
            foreach (var kvp in tempData.ObjectTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType)
                    continue;

                RxMemoryCompiler.GenerateObjectTypeSourceCode(kvp.Value, codeStream);
            }
            foreach (var kvp in tempData.StructTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType)
                    continue;

                RxMemoryCompiler.GenerateStructTypeSourceCode(kvp.Value, codeStream);
            }
            foreach (var kvp in tempData.SourceTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType)
                    continue;

                RxMemoryCompiler.GenerateSourceTypeSourceCode(kvp.Value, codeStream);
            }
            foreach (var kvp in tempData.MapperTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType)
                    continue;

                RxMemoryCompiler.GenerateMapperTypeSourceCode(kvp.Value, codeStream);
            }

            foreach (var kvp in tempData.EventTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType)
                    continue;

                RxMemoryCompiler.GenerateEventTypeSourceCode(kvp.Value, codeStream);
            }
            foreach (var kvp in tempData.DisplayTypes)
            {
                if (!kvp.Value.valid || !kvp.Value.runtimeType)
                    continue;

                RxMemoryCompiler.GenerateDisplayTypeSourceCode(kvp.Value, codeStream);
            }

            return codeStream.ToString();
        }
    }
}