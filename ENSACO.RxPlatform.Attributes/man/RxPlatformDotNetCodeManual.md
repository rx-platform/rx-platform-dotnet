<div align="left">
<img src="http://rx-platform.org/images/logo.png" />
</div>
<h1 align="left">
<b>{rx-platform}</b> .NET Core Coding Reference
</h1>

___

.NET Core hosting for **```{rx-platform}```** enables development of platform applications using .NET Core runtime.
All the examples in this manual are given in C# language although other .NET Core languages can be used as well.

This manual describes the mapping between **```{rx-platform}```** configuration types and .NET Core language constructs.
It also describes the rules for defining platform runtime objects using C# class definitions.

<h2 align="left">
{rx-platform} Data Types
</h2>
<div style:"word-wrap: break-word;">

This part of the manual describes the mapping between **```{rx-platform}```** data types and .NET Core types.
It's intent is to provide understanding of platform types inside .NET Core applications.

Inside the platform all _Data Types_ are treated as a "_single write_" data.
This means that when a value is written or read the whole value is transferred at once.
This disables partial updates of complex data types to and from the platform.

Structure inside the platform is provided by different kind of types inside platform,
the _struct types_ and _object types_, which will be explained later.

</div>


<h3 align="left">
Plain Data Types
</h3>
<div style:"word-wrap: break-word;">

Platform supports following **plain data types**:
- ```bool``` - bit data type, maps to C# ```bool```
- ```int8``` - signed 8 bits integer, maps to C# ```sbyte```
- ```uint8``` - unsigned 8 bits integer, maps to C# ```byte```
- ```int16``` - signed 16 bits integer, maps to C# ```short```
- ```uint16``` - unsigned 16 bits integer, maps to C# ```ushort```
- ```int32``` - signed 32 bits integer, maps to C# ```int```
- ```uint32``` - unsigned 32 bits integer, maps to C# ```uint```
- ```int64``` - signed 64 bits integer, maps to C# ```long```
- ```uint64``` - unsigned 64 bits integer, maps to C# ```ulong```
- ```float32``` - 32 bits floating point, maps to C# ```float```
- ```float64``` - 64 bits floating point, maps to C# ```double```
- ```string``` - utf8 string, maps to C# ```string```
- ```time``` - date and time type, maps to C# ```DateTime```
- ```uuid``` - universally unique identifier, maps to C# ```Guid```
- ```byte string``` - byte array, maps to C# ```byte[]```

All these types can be used either as a scalar or arrays, 
so the valid type in C# is ```DateTime[]``` for example.
These types will be refereed later on as **plain data types**.


</div>


<h3 align="left">
Complex Data Types
</h3>
<div style:"word-wrap: break-word;">

Platform supports creation of **complex data types**.
Complex data types are recursive structures that can contain **plain data types** or other **complex data types**.
These types can be created in C# using classes decorated with appropriate attributes.
For example, the following C# class defines a struct with various data types:
```csharp
using ENSACO.RxPlatform.Attributes;
namespace TestingPlatform
{
    [RxPlatformDataType(nodeId: "B1C2D3E4-F5A6-4789-ABCD-0987654321BA")]
    [RxPlatformDeclare()]
    public struct SensorData
    {
        public bool HasHysterezis { get; set; }
        public double Hysteresis { get; set; }
    }

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

When you request creation of the object type from code you actually get derived type
created by the platform runtime:

```csharp
Heater? heater = await RxPlatformObjectRuntime.CreateInstance<TestingPlatform.Heater>();
// heater object type is TestingPlatform__rxImplementation.Heater
Console.WriteLine(heater.GetType().FullName);
```
So basically your class definition is derived from platform runtime class which provides infrastructure
and dynamic type that provides specific functionality you requested in class definition.

This enables best runtime performance and full integration with platform runtime.
This part of a manual covers two types of runtime entities inside platform _Object Type_ and _Struct Type_.

Understanding of these two types is most important when you model your application abstractions.

Other runtime types inside platform (_Source Type_, _Mapper Type_, _Event Type_) are covered in separate manual.


The rest of this chapter is divided in two sections:
- Common Definitions and Struct Types
- Object Types

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
They can contain all the previously defined items including other Struct Types,
thus defining the recursive structure for building abstractions.

```csharp
using ENSACO.RxPlatform.Attributes;
namespace TestingPlatform
{
    [RxPlatformStructType(nodeId: "ABEADB7D-5AC6-44C4-BC45-53FC25EEF945")]
    [RxPlatformRuntime()]
    public class Sensors : RxPlatformStructRuntime
    {
        public virtual double? Temperature { get; }
        public virtual double? Humidity { get; }

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
    - ```void``` return type
    - return type ```Task<T>``` where ```T``` is type decorated with ```RxPlatformDataType``` attribute
  
  - **callable methods** - these methods are mapped from methods that have the following characteristics:
    - virtual
    - non-static
    - zero arguments
    - one argument of type decorated with ```RxPlatformDataType``` attribute
    - ```Task``` return type
    - ```void``` return type
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
items represent relations to other hosted objects inside platform.
Relations are connections between two objects that can be created and removed during runtime execution of the platform.
They can be read-only or dynamic. 

Keep in mind that read-only relations can also be null
in case remote object is not available 
or object is just created and had no time to resolve target reference.
These relations are mapped from properties that have the following characteristics:
  - virtual
  - non-static
  - property type is of class decorated with
    ```RxPlatformObjectType``` and ```RxPlatformRuntime``` attributes
  - property has only get accessor
  - property has both get and set/init accessor

 Relation items provide visibility of .NET Core objects relations to the platform runtime.
 Besides direct mapping of relations platform also provides additional
 way to monitor and write previously defined relation properties.

  - **C# events** named `On`_RelationName_`Connected` and `On`_RelationName_`Disconnected` where _RelationName_ is the name of the property already mapped.
    These events will be overridden by the platform runtime and can be used to notify your code about relation status changes.
    Events that are mapped have the following characteristics:
    - virtual
    - non-static
    - event type is ```Action<T>``` or any delegate type that takes single argument of type ```T```
    - ```T``` is the same as the property type but not null-able



_OnChange_ events enables to implement algorithms based on relation status change.

When _Connect_ event is fired the relation property is already assigned to the related object.
On the other side when _Disconnect_ is fired the relation property is already cleared
and the only way to access disconnected object is to use event argument.

Also keep in mind that relation can be disconnected by platform at any time 
so when using related object you have to extract object form relation property and then check it for null value.

This is shown on the example bellow:
```csharp
[RxPlatformObjectType(nodeId: "801A1925-092E-4053-BB69-A1B8C9838C41")]
[RxPlatformRuntime()]
class Heater : RxPlatformObjectRuntime
{    
      // the pump currently related to this heater
     public virtual Pump? WaterPump { get; set; }
        
    public virtual event Action<Pump>? OnWaterPumpConnected;
    public virtual event Action<Pump>? OnWaterPumpDisconnected;

    public void WritePumpName()
    {
        // wrong usage
        // relation can become null in betwen if and write line calls
        //if(WaterPump!=null)
        //{
        //    Console.WriteLine($"Heater: WaterPump relation is connected. Pump Name: {WaterPump.Options.Name}");
        //}


        // correct usage
        // first extract the related object
        // then check it for null value
        var temp = WaterPump;
        if(temp!=null)
        {
            // use the related object
            Console.WriteLine($"Heater: Using WaterPump relation. Pump Name: {temp.Options.Name}");
        }
        else 
        {
            Console.WriteLine($"Heater: Unable to use WaterPump relation. It is not connected.");
        }
    }
    

    public void Started()
    {
        OnWaterPumpConnected += (pump) =>
        {
            Console.WriteLine($"Heater: OnWaterPumpConnected event fired. Pump Name: {pump.Options.Name}");
        };
        OnWaterPumpDisconnected += (pump) =>
        {
            Console.WriteLine($"Heater: OnWaterPumpDisconnected event fired. Pump Name: {pump.Options.Name}");
        };
    }
}

```

  
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
            Name = "HeaterXZ01"
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

This means that you can add, remove or change virtual properties and methods of your class definition
, and the platform will take care of the underlying implementation details.

</div>

___
