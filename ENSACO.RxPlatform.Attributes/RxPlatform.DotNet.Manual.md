<div align="left">
<img src="http://rx-platform.org/images/logo.png" />
</div>
<h1 align="left">
<b>{rx-platform}</b> .NET Core Hosting
</h1>

___
<div style:"word-wrap: break-word;">

**```{rx-platform}```** is distributed platform for building applications that exchange Real-Time data. 
It enables seamless integration of various OT (Operational Technology) systems, providing a unified framework for data acquisition, processing, and visualization.

Hosting package enables building .NET Core applications for
**```{rx-platform}```** and to interface the OT technologies from within. 

- You can download the latest version of platform binaries from [this link](http://rx-platform.org).
- Commercial support is available at [ENSACO Solutions doo.](https://ensaco.rs/)



</div>

___

<h2 align="left">
Introduction and Quick Start
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

<h3 align="left">
Quick Start
</h3>


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


<h3 align="left">
Interacting with .NET Core code
</h3>

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

<h3 align="left">
Debugging .NET Core Applications
</h3>

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

These steps should enable you to effectively debug your .NET Core applications hosted inside the **```{rx-platform}```** environment.



</div>

___

<h2 align="left">
{rx-platform} Data Types
</h2>
<div style:"word-wrap: break-word;">

This part of the manual describes the mapping between **```{rx-platform}```** data types and .NET Core types.
It's intent is to provide basic support for platform types inside .NET Core applications.

</div>


<h3 align="left">
Plain Data Types
</h3>
<div style:"word-wrap: break-word;">

Platform supports following **plain data types**:
- ```bool``` - maps to C# ```bool```
- ```int8``` - maps to C# ```sbyte```
- ```uint8``` - maps to C# ```byte```
- ```int16``` - maps to C# ```short```
- ```uint16``` - maps to C# ```ushort```
- ```int32``` - maps to C# ```int```
- ```uint32``` - maps to C# ```uint```
- ```int64``` - maps to C# ```long```
- ```uint64``` - maps to C# ```ulong```
- ```float32``` - maps to C# ```float```
- ```float64``` - maps to C# ```double```
- ```string``` - maps to C# ```string```
- ```time``` - maps to C# ```DateTime```
- ```uuid``` - maps to C# ```Guid```
- ```byte string``` - maps to C# ```byte[]```

All these types can be used either as a scalar or arrays, 
so the valid type in C# is ```DateTime[]``` for example.
These types will be refereed later on as **plain data types**.


</div>


<h3 align="left">
Complex Data Types
</h3>
<div style:"word-wrap: break-word;">

Platform supports creation of **complex data types** like Structs.
Complex data types are recursive structure that can contain **plain data types** or other **complex data types**.
These types can be created in C# using classes decorated with appropriate attributes.
For example, the following C# class defines a struct with various data types:
```csharp
using ENSACO.RxPlatform.Attributes;
namespace TestingPlatform
{
    [RxPlatformDataType(nodeId: "A1B2C3D4-E5F6-4789-ABCD-1234567890AB")]
    [RxPlatformDeclare()]
    public class HeaterOptions
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public SensorData TemperatureOptions { get; set; } = new SensorData
        {
            HasHysterezis = true,
            Hysteresis = 1.7
        };
        public SensorData HumidityOptions { get; set; } = new SensorData
        {
            HasHysterezis = false
        };
    }
    [RxPlatformDataType(nodeId: "B1C2D3E4-F5A6-4789-ABCD-0987654321BA")]
    [RxPlatformDeclare()]
    public struct SensorData
    {
        public bool HasHysterezis { get; set; }
        public double Hysteresis { get; set; }
    }
}
```

In this example, the `HeaterOptions` class defines a struct some properties and a nested structs `SensorData` of type `SensorData`.
The `SensorData` struct defines a struct contained as `TemperatureOptions` and `HumidityOptions`,
both class and struct types are supported for **complex data types**.
Notice the properties that are initialized with a default value,
this is transferred to the platform as the default value for this property.

The `RxPlatformDataType` attribute declares these data types inside platform namespace with unique identifiers.
Properties that are mapped to the platform have the following characteristics:
  - non-virtual
  - non-static 
  - property type is plain type
  - property type is complex data type
  - property has both get and set accessor public

Besides for building user types inside the platform 
it is used for building platform configuration from code which will be explained later on.

The `RxPlatformDeclare` attribute indicates that the struct or class is a platform data type declaration 
so this data type will be declared inside platform explicitly from code.

After defining these classes and structs, 
you can use them in your platform runtime objects as properties or event and method parameters.
These types will be refereed later on as **complex data types**.

</div>


___

<h2 align="left">
{rx-platform} Runtime Types
</h2>
<div style:"word-wrap: break-word;">

This part of the manual describes the mapping between **```{rx-platform}```** data types and .NET Core types.

Every runtime object inside platform is created by the platform callback itself during runtime execution.

The reflection based analysis of the object class actually creates dynamic type derived from your class definition. 
This enables platform to override properties and methods as needed based on the class definition.

When you request creation of the object type from code you actually get derived type created by the platform runtime:

```csharp
Heater? heater = await RxPlatformObjectRuntime.CreateInstance<TestingPlatform.Heater>();
// heater object type is TestingPlatform__rxImplementation.Heater
Console.WriteLine(heater.GetType().FullName);
```
So basically your class definition is derived from platform runtime class which provides infrastructure
and dynamic type that provides specific functionality you requested in class definition.

This enables best runtime performance and full integration with platform runtime.

The rest of this chapter is divided in five sections:
- Common Definitions and Struct Types
- Object Types
- Source Types
- Mapper Types
- Event Types

Every chapter describes the rules for reflection based mapping of C# class definitions.

</div>


<h3 align="left">
Common Definitions and Struct Types
</h3>
<div style:"word-wrap: break-word;">


All runtime objects inside platform have some common definitions that are used across all runtime types.
These objects inside platform are containers that can contain these items:
- ```constant``` - this is value that cannot be changed during runtime execution of the object.
- ```value``` - this is value that can be changed during runtime execution of the object.
- ```struct``` - this is recursive structure that can contain constant, value or other struct items.

*```constant``` and ```value``` items can contain **plain data types** or **complex data types**.

Using reflection platform maps C# class definitions to runtime object items.


<h4 align="left">
Initialization
</h4>


In order to initialize the runtime you can provide _Started_ and/or _Stopping_ methods inside your class.
These methods have the following characteristics:
  - virtual
  - non-static
  - ```void``` return type
  - zero arguments

After creation of the runtime object platform will call the _Started_ method after all properties are initialized.
These methods can be used to perform any additional initialization and/or de initialization needed for the object.
Before object destruction platform will call the _Stopping_ method.

_Started_ and _Stopping_ methods are optional and can be async, but
it is important to notice that after _Stopping_ method is started the platform will destroy runtime immediately.

This is shown on the example bellow
```csharp
using ENSACO.RxPlatform.Attributes;
namespace TestingPlatform
{
    [RxPlatformObjectType(nodeId: "801A1925-092E-4053-BB69-A1B8C9838C41")]
    [RxPlatformRuntime()]
    class Heater : RxPlatformObjectRuntime
    {
        public void Started()
        {
            // perform initialization
        }
        public void Stopping()
        {
            // perform de-initialization
        }
    }
}
```

<h4 align="left">
Constant Items
</h4>

```constant```
items are used to create interface for platform to see those values and to export them as needed.
These are mapped from properties that have the following characteristics:
  - non-virtual
  - non-static 
  - property type is plain type
  - property type is complex data type
  - property has only get accessor
  - property has both get and init accessor

These properties are read-only and constant during runtime execution of the object and can be initialized only during object creation.

<h4 align="left">
Value Items
</h4>

```value```
items are mapped directly from class properties but also provide several ways to monitor and/or write their values.

It is important to notice that when properties are overridden by runtime the state of the property is inside the platform,
so reading the property directly from C# code will not provide the correct value.
Also reading and writing properties operation will be thread safe.

  - **C# properties** 
      that are mapped are the one having the following characteristics:
    - virtual
    - non-static
    - property type is null-able
    - property type is plain type
    - property type is complex data type
    - property has only get accessor
    - property has both get and set/init accessor

 Besides direct mapping of properties platform also provides two additional 
 ways to monitor and write previously defined property values:

  - **C# events** named `On`_PropertyName_`Change` where _PropertyName_ is the name of the property already mapped.
    These events will be overridden by the platform runtime and can be used to notify your code about property value changes.
    Events that are mapped have the following characteristics:
    - virtual
    - non-static
    - event type is ```Action<T>``` or any delegate type that takes single argument of type ```T```
    - ```T``` is the same as the property type


  - **C# methods** named `Write`_PropertyName_ where _PropertyName_ is the name of the property already mapped.
    These methods will be overridden by the platform runtime and can be used to
    catch the result of the write operation to the property.
    Methods that are mapped have the following characteristics:
    - virtual
    - non-static
    - ```Task<bool>``` return type
    - exactly one argument of type ```T``` where ```T``` is the same as the property type but not null-able

_OnChange_ events enables to implement algorithms based on the value change. 
This event will be fired by the platform after every property change.
When this event if fired the property value is already updated.

The _Write_ function enables to implement algorithms based on the value being written or not.
The value of the property might be in some remote device so in order 
to enable sequential writes one must ```await``` every write.

This is shown on the example bellow:
```csharp
[RxPlatformObjectType(nodeId: "801A1925-092E-4053-BB69-A1B8C9838C41")]
[RxPlatformRuntime()]
class Heater : RxPlatformObjectRuntime
{

    public virtual double? SetPoint { get; set; } = 20.0;
    public virtual bool? Start { get; set; } = false;

    public virtual Task<bool> WriteSetPoint(double newValue)
    {
        return Task.FromResult(false);
    }
    public virtual Task<bool> WriteStart(bool newValue)
    {
        return Task.FromResult(false);
    }

    public virtual event Action<double>? OnSetPointChange;
}

// usage of the WriteSetPoint and WriteStart methods
class HeaterStarter
{
    public override async Task<bool> StartHeating(Heater obj, double temp)
    {
        obj.OnSetPointChange += (newValue) =>
        {
            Console.WriteLine($"Heater: OnSetPointChange event fired. New Value: {newValue}");
        };
        if(obj.SetPoint!=temp)
        {
            // perform write operation for setting the set-point
            if(!await obj.WriteSetPoint(temp))
            {
                // was unable to set endpoint
                return false;
            }
        }
        // after success write start to turn on the heating
        return await obj.WriteStart(true);
    }
}
```


<h4 align="left">
Struct Items
</h4>

```struct```
items are recursive structure that can contain _Constant Items_, _Value Items_ or other _Struct Items_.
They are mapped from properties that have the following characteristics:
  - non-virtual
  - non-static
  - property type is of class decorated with
    ```RxPlatformStructType``` and ```RxPlatformDeclare``` attributes
  - property has only get accessor
  - property has both get and init accessor



<h4 align="left">
Struct Types
</h4>

Runtime Struct Types are defined using C# classes decorated with appropriate attributes.
They can contain all the previously defined items.

```csharp
using ENSACO.RxPlatform.Attributes;
namespace TestingPlatform
{
    [RxPlatformStructType(nodeId: "ABEADB7D-5AC6-44C4-BC45-53FC25EEF945")]
    [RxPlatformRuntime()]
    public class Sensors
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
```

</div>


<h3 align="left">
Object Types
</h3>
<div style:"word-wrap: break-word;">

Runtime Objects inside a platform are entities that hold runtime properties and methods and can be created on three ways:
  - from code during runtime execution
  - from platform using user configuration
  
Besides common definitions for a runtime Objects can also contain these items:
- ```method``` - this is method that can be executed during runtime execution of the object.
- ```relation``` - this is relation to another object inside platform.

Using reflection platform maps C# class definitions to runtime object items.

  
<h4 align="left">
Method Items
</h4>

```method```
items are mapped from methods inside a class. 
These methods can provide two ways of communication between platform and your C# code.
They can be used to provide interface for platform to execute code inside your C# application 
or to define methods that can be overridden by platform runtime to provide access to platform defined methods.

  - **implemented methods** - these methods are mapped from methods that have the following characteristics:
    - non-virtual
    - non-static
    - zero arguments
    - one argument of type decorated with ```RxPlatformDataType``` attribute
    - ```Task``` return type
    - return type ```Task<T>``` where ```T``` is type decorated with ```RxPlatformDataType``` attribute
  
  - **callable methods** - these methods are mapped from methods that have the following characteristics:
    - virtual
    - non-static
    - method has zero or exactly one argument
    - return type ```Task<T>``` where ```T``` is type decorated with ```RxPlatformDataType``` attribute

Example of implemented and callable methods is shown bellow:
```csharp
[RxPlatformObjectType(nodeId: "801A1925-092E-4053-BB69-A1B8C9838C41")]
[RxPlatformRuntime()]
class Heater : RxPlatformObjectRuntime
{

    public double DefaultSetPoint { get; init; } = 21.0;

    // implemented method
    // called by the platform from sensors data inside your code
    // you have to provide implementation for this method
    public virtual async Task PersonEntered()
    {
        // implementation
        HeaterStarter starter = new HeaterStarter();
        await starter.StartHeating(this, DefaultSetPoint);
    }
    // callable method
    // overridden by platform runtime to provide access to platform defined methods 
    // so you can call it from code to access functionality defined by field devices
    public virtual async Task PerformDiagnostics()
    {
        return Task.CompletedTask;
    }
}
```

<h4 align="left">
Relation Items
</h4>

```relation```
items are mapped from properties inside a class and is not implemented yet!!!!


  
<h4 align="left">
Creating Runtime Objects
</h4>

Bellow is the example of defined object using the rules described before:

```csharp
using ENSACO.RxPlatform.Attributes;
namespace TestingPlatform
{
    [RxPlatformObjectType(nodeId: "801A1925-092E-4053-BB69-A1B8C9838C41")]
    [RxPlatformRuntime()]
    class Heater : RxPlatformObjectRuntime
    {
        // automation data
        public double DefaultSetPoint { get; init; } = 21.0;
        public virtual double? SetPoint { get; set; } = 20.0;
        public virtual bool? Start { get; set; } = false;

        // sensor runtime data
        public virtual Sensors? Sensors { get; } = new Sensors();

        // automation options
        public virtual HeaterOptions? Options { get; set; } = new HeaterOptions();

        public virtual async Task PersonEntered()
        {
            // implementation
            HeaterStarter starter = new HeaterStarter();
            await starter.StartHeating(this, DefaultSetPoint);
        }
        public virtual async Task PerformDiagnostics()
        {
            return Task.CompletedTask;
        }
        // the pump currently related to this heater
        public virtual Pump? WaterPump { get; set; }
    }
}
```
Object types are the only runtime components that can be create explicitly from the code.
You can provide template for initial properties, name and optionally path inside platform namespace
and/or id for the object.

C# Objects created this way can be used to directly interact with platform runtime.

This is shown in the example bellow:
```csharp
// create the object runtime
Heater? heater = await RxPlatformObjectRuntime.CreateInstance<Heater>(
    new DynamicObject
    {
        DefaultSetPoint = 19,
        Options = new HeaterOptions 
        {
            Id = 10001,
            Name = "HaterXZ01"
        }
    }
    , "Heater001");
// assign a pump previously created
heater.WaterPump = myPumps["PumpA1"];
// do diagnostics
await heater.PerformDiagnostics();
// register it inside our collection
myHeaters.Add(heater.Options.Name, heater);
```
It is important to mention once more that object returned by the `CreateInstance` method 
is actually of dynamic type created by the platform runtime and derived from your type.



</div>

___

<h2 align="left">
{rx-platform} Configuration and Variable Types
</h2>
<div style:"word-wrap: break-word;">

Using the platform attributes,
you can define configuration and variable types for your platform objects.

This allows you to change the properties visibility and behavior based on the platform configuration.
Every `value` or `constant` item can be overridden
to adapt for different protocols and to provide various filtering and simulation behaviors.

Variable types are entities that provide additional mapping for properties.
They allow you to define how the properties of your objects are represented and accessed at runtime.
This can be achieved by declaring

</div>