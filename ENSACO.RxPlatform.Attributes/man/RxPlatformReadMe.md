<div align="left">
<img src="http://rx-platform.org/images/logo.png" />
</div>
<h1 align="left">
<b>{rx-platform}</b> .NET Core "<i>Read Me</i>" Manual
</h1>

___
<div style:"word-wrap: break-word;">

**```{rx-platform}```** is distributed platform for building applications that exchange Real-Time data. 
It enables seamless integration of various OT (Operational Technology) systems, 
providing a unified framework for
 - data acquisition and control
 - data processing
 - visualization.

`{rx-platform}` `dotnet` hosting package enables building .NET Core libraries **_that are hosted inside the platform process_**.
This and flexibility of the .NET Core enables to run your code on various devices and/or various server platforms.

All this gives your dotnet applications interface with the OT technologies from within
and opens new architectural perspective on the real-time systems development.

- You can download the latest version of platform binaries from [this link](https://ensaco.rs/install/rx-platform-win32-x64.exe).
- Commercial support is available at [ENSACO Solutions doo.](https://ensaco.rs/)



</div>

___

<h2 align="left">
Introduction
</h2>
<div style:"word-wrap: break-word;">

This documentation's intention is to provide guidance on 
how to set up and use the hosting capabilities of **```{rx-platform}```** for .NET Core applications for 
both programmers and AI agents. This might explain why I'm using the MD file format for this documentation.

To start using the platform you first need to install the binaries fro the link above.

After successful installation you can verify it using the ```rx-interactive --version``` command from command line.

This should produce output similar to this one:
```term
Parsing command line...OK

{rx-platform}  Molecule Ver 2.8.1.20251028
common library Ver 3.4.0.20251028

hosts:
   GNU Console Ver 1.1.2.20251028
   Interactive Console Ver 1.0.9.20251028

ABI interface  Ver 1.5.1.20251028

Native .NET hosting  .NET Core Hosting Ver 1.2.8.20251028

Loading static plug-ins...OK

modbus      RX Modbus Protocol Ver 1.13.0.20250921
simulation  LINN Simulation Plugin Ver 1.1.0.20250921
iec61850    IEC-61850 plugin Ver 0.8.2.20250930

```
After this step you are ready to create your first .NET Core application using the platform hosting capabilities.

<h2 align="left">
Quick Start
</h2>


Start by creating a new console application using the following command:
```bash
dotnet new classlib -n TestingPlatform
```
This will create a new folder named `TestingPlatform` with the basic structure of a console application.
Navigate to the newly created folder:
```bash
cd TestingPlatform
```
Next, you need to add a reference to the **```{rx-platform}```** hosting package.
This can be done using the following command:
```bash
dotnet add package ENSACO.RxPlatform
```
After this you can open the Class1.cs file and modify it to use the platform hosting capabilities as shown below:
```csharp
using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Runtime;

namespace TestingPlatform
{
    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "D915E2C1-2EDB-4C80-AF9E-255A0646E7D2")]
    public class Class1 : RxPlatformObjectRuntime
    {
        public virtual string? MyProperty { get; set; } = "initial value";
        public virtual int? Counter { get; set; } = 0;		
        public void MyMethod()
        {
            Counter++;
            MyProperty = $"I've been called {Counter} times";
        }
    }
}
```
The ```RxPlatformRuntime``` attribute marks the class as a platform runtime object. 
This is an object that can be dynamically created by the platform.

The ```RxPlatformObjectType``` attribute specifies the unique node ID for this class
that will be used by the platform to identify it. This attribute also triggers platform to parse the class and to create obect type 

The class is derived from ```RxPlatformObjectRuntime``` base class.
The mechanism for the runtime interaction between the platform and the .NET Core application is based on inheritance.
This breaks possible design patterns so be prepared for it.

When the platform creates an instance of this class, it will use reflection to map existing
Properties and Methods to the platform's internal representation.

Inside a class we have a property ```MyProperty``` and a method ```MyMethod```.
The property is initialized with a default value of "initial value"
, this will be parsed by the platform and the appropriate item inside the it will be created.

The method ```MyMethod``` increments a counter each time it is called and updates the value of ```MyProperty```
with the number of times the method has been called. 
This method is accessible by the platform and can be invoked from within.

After modifying the Class1.cs file, you must define an entry point class for your library.
Add file named `TestingPlatform.cs` to the project with the following content:
```csharp
using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Host;
using ENSACO.RxPlatform.Runtime;

namespace TestingPlatform
{
    [RxPlatformLibrary()]
    public class TestingPlatformMain
    {
        public static PlatformLibraryInfo Initialize()
        {
            return new PlatformLibraryInfo
            {
                Name = "meTesting"
            };
        }
        public static async void Start() 
        {			
            await RxPlatformObjectRuntime.CreateObject(new Class1(), "myObject");
        }
        public static void Deinitialize()
        {
        }
    }
}
```


Now you can build the application using the following command:
```bash
dotnet build
```
With dotnet assembly built now we have to setup ```rx-platform``` to load our library.
First create directory for platform configuration files:
```bash
mkdir rx-config
```
After this, we need to create a configuration file for the platform.
To do this, create a new file named `rx-platform.yml` in the same directory and add the following content:
```yaml

dotnet.version: "latest"
dotnet.app_path: "bin\Debug\net8.0"
storage.user: "rx-config"

#in case you need to change http port
# default is 31421
http.port: 80

```
Now you are ready to run the platform hosting your .NET Core application.
You can do this using the following command:
```bash
rx-interactive
```

This will start the platform and load your .NET Core library.
To verify that your object is loaded correctly,
you can use the platform's interactive console and execute the following command:
```term
/world
host@RX003:>dotnet -l
meTesting       TesingPlatform Ver 1.0.0.0
````
This will list all loaded .NET Core libraries, including your `meTesting` library.


<h2 align="left">
Interacting with .NET Core code
</h2>

In order to interact with your object, you can use the platform's interactive console.
From the interactive console, you can execute the following commands:
```term
/world                                                   
host@RX003:>brw myObject                                 
Name        Value           Type                         
=============================================            
_Object     {...}           struct                       
MyProperty  initial value   value                        
Counter     0               value                        
_Name       myObject        const_value                  
Domain      <=SystemDomain  relation_target              
MyMethod    0               method                       
                                                         
/world                                                   
host@RX003:>exec myObject.MyMethod
Execute prepared.                                        
Start time: 2025-11-21 08:45:27.237                      
Execute MyMethod succeeded.                              
Signal Level: 0                                          
Result:{}                                                
Time elapsed: 9627 us                                    
                                                         
/world                                                   
host@RX003:>brw myObject                                 
Name        Value                     Type               
=======================================================  
_Object     {...}                     struct             
MyProperty  I've been called 0 times  value              
Counter     1                         value              
_Name       myObject                  const_value        
Domain      <=SystemDomain            relation_target    
MyMethod    0                         method             
                                                         
/world                                                   
host@RX003:>                                        
```
You can also access the property value directly:
```term
/world                                                   
host@RX003:>write myObject.Counter 99                    
Write prepared.                                          
Counter <= 99                                            
Start time: 2025-11-21 08:45:53.520                      
Write to Counter succeeded.                              
Signal Level: 0Time elapsed: 89 us                       
                                                         
/world                                                   
host@RX003:>exec myObject.MyMethod                       
Execute prepared.                                        
Start time: 2025-11-21 08:46:12.078                      
Execute MyMethod succeeded.                              
Signal Level: 0                                          
Result:{}                                                
Time elapsed: 2397 us                                    
                                                         
/world                                                   
host@RX003:>brw myObject                                 
Name        Value                      Type              
======================================================== 
_Object     {...}                      struct            
MyProperty  I've been called 99 times  value             
Counter     100                        value             
_Name       myObject                   const_value       
Domain      <=SystemDomain             relation_target   
MyMethod    0                          method            
                                                         
/world                                                   
host@RX003:>  
```
By default in platform all objects are exposed through the HTTP API.
You can access the object using any HTTP client or using curl from the command line:
```bash
curl http://localhost/myObject.rx?pretty
```
This will return a JSON representation of the object.

To write property you use PUT method:
```bash
curl -X PUT http://localhost/myObject.Counter.rx -d "{ ""val"" : 1000}"
```
To execute a method you use POST method:
```bash
curl -X POST http://localhost/myObject.MyMethod.rx?pretty -d"{ }"
```
After your class is inside the platform you can expose it through various industrial protocols supported by the platform
like Modbus, IEC-61850, OPC-UA, MQTT, etc.

Your object can also use all this protocols to directly access your class properties and methods.

You can also use the platform's built-in various simulation and filtering algorithms to process the data.

<h2 align="left">
Debugging .NET Core Applications
</h2>

Debugging .NET Core applications hosted inside platform can be achieved 
using any dotnet debugger.
If you are using Visual Studio or Visual Studio Code.
To debug your application, follow these steps:

1. Open your .NET Core project in Visual Studio or Visual Studio Code.
2. Set breakpoints in your code where you want to pause execution.
3. Start **```{rx-interactive}```** using the following command:
```bash
rx-interactive
```
4. Attach the debugger to the **```{rx-interactive}```** process:
- In Visual Studio, go to `Debug` > `Attach to Process...`, select the **```{rx-interactive}```** process, and click `Attach`.
- In Visual Studio Code, use the `Debug: Attach to Process` command from the Command Palette and select the **```{rx-interactive}```** process.
4. Interact with your .NET Core application through the **```{rx-interactive}```** interactive console or HTTP API to trigger the breakpoints.
5. When execution hits a breakpoints, you can inspect variables, step through code, and evaluate expressions as you would in a standard debugging session.

User loaded assemblies are loaded from memory so you can change the code and recompile without restarting the platform.
In order to apply the changes you need to update the assembly in the platform using the following command from the interactive console:
```term
dotnet -r meTesting
```
Then you can reattach the debugger to the process and debug the new code.

If restarting of a platform is not an issue you can define platform process to start when debug is activated.

In Visual Studio you can set command line arguments in the project properties using the Launch Profiles section.
Create new Executable profile named `rx-platform` with the following executable `rx-interactive.exe`.


In Visual Studio Code you can create launch configuration as shown below:
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "rx-platform",
            "type": "coreclr",
            "request": "launch",
            "program": "rx-interactive.exe",
            "args": [ "" ],
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "internalConsoleOptions": "openOnSessionStart"
        }
    ]
}
```
Place this configuration in the `.vscode/launch.json` file inside your project folder.

You can also start platform from the terminal with additional debugging options enabled. 
To activate the debugger on dotnet code startup use the command switch `dotnet-debug-break`:
```bash
rx-interactive --dotnet-debug-break
```

These steps should enable you to effectively debug your .NET Core applications hosted inside the **```{rx-platform}```** environment.



<h2 align="left">
What's Next?
</h2>

<div style:"word-wrap: break-word;">

After successfully setting up and running your first .NET Core application with **```{rx-platform}```**, you might want to explore more advanced features and capabilities of the platform.
Here are some suggestions on what to explore next:
- **```{rx-platform}```** [_.NET Core Coding Reference_](RxPlatformDotNetCodeManual.md) - 
detailed coding reference for building your abstractions using platform hosting capabilities.
- **```{rx-platform}```** [_.NET Core Configuration Reference_](RxPlatformDotNetConfigManual.md) - 
reference for building configuration types for your abstractions 
with full access to various industrial protocols and data sources.
- **```{rx-platform}```** [_.NET Sources and Mappers Reference_](RxPlatformDotNetSourceMapperManual.md) - 
reference for building source and mapper types that connect platform to various 
data sources and protocols easily using .NET Core language constructs.

</div>

