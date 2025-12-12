// Auto-generated code for RxPlatform Types

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.Json;



namespace DynamicAssembly__rxImplementation
{

public class DynamicObject : DynamicAssembly.DynamicObject
{
     public override event System.Action<System.String?>? OnObjectProp2Change;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private System.String? ObjectProp2____rxImplementation = "zikica";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object ObjectProp2____rxLock = new object();
    public override System.String? ObjectProp2 
    {
        get 
        {
            lock(ObjectProp2____rxLock)
            {
                return ObjectProp2____rxImplementation;
            }
        }
         set
        {
            if(value != null)
            {
                WriteProperty(0, value);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private System.Boolean? ObjectProp3____rxImplementation = true;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object ObjectProp3____rxLock = new object();
    public override System.Boolean? ObjectProp3 
    {
        get 
        {
            lock(ObjectProp3____rxLock)
            {
                return ObjectProp3____rxImplementation;
            }
        }
        protected set
        {
            if(value != null)
            {
                WriteProperty(1, value);
            }
        }
    }

     public override event DynamicAssembly.ChangedObjProp4? OnObjectProp4Change;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private System.Byte? ObjectProp4____rxImplementation = 55;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object ObjectProp4____rxLock = new object();
    public override System.Byte? ObjectProp4 
    {
        get 
        {
            lock(ObjectProp4____rxLock)
            {
                return ObjectProp4____rxImplementation;
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private DynamicAssembly.DynamicSubDataType? SubData____rxImplementation = new DynamicAssembly.DynamicSubDataType();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object SubData____rxLock = new object();
    public override DynamicAssembly.DynamicSubDataType? SubData 
    {
        get 
        {
            lock(SubData____rxLock)
            {
                return SubData____rxImplementation;
            }
        }
         set
        {
                if (value != null)
                {
                    string strVal = JsonSerializer.Serialize<DynamicAssembly.DynamicSubDataType>(value);
                    WriteProperty(3, strVal);
                }
        }
    }

    public async override Task<bool> WriteSubData(DynamicAssembly.DynamicSubDataType value)
    {
        return await WriteProperty(3, value);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private DynamicAssembly.SubNamespace.SomeOtherDynamicObject? OtherDynamicObj____rxImplementation = null;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object OtherDynamicObj____rxLock = new object();
    public override DynamicAssembly.SubNamespace.SomeOtherDynamicObject? OtherDynamicObj 
    {
        get 
        {
            lock(OtherDynamicObj____rxLock)
            {
                return OtherDynamicObj____rxImplementation;
            }
        }
    }

    protected override void __rxValueCallback(int index, object? value)
    {
        switch(index)
        {
            case 0:
                {
                    System.String? myValue = null;
                    lock(ObjectProp2____rxLock)
                    {
                        try
                        {
                            if(value == null)
                            {
                                ObjectProp2____rxImplementation = null;
                            }
                            else
                            {
                                myValue = (System.String?)value;
                                ObjectProp2____rxImplementation = myValue;
                            }
                        }
                        catch(Exception)
                        {
                            ObjectProp2____rxImplementation = null;
                            myValue = null;
                        }
                     }
                    
                    OnObjectProp2Change?.Invoke(myValue);

                 }
                break;
            case 1:
                {
                    System.Boolean? myValue = null;
                    lock(ObjectProp3____rxLock)
                    {
                        try
                        {
                            if(value == null)
                            {
                                ObjectProp3____rxImplementation = null;
                            }
                            else
                            {
                                myValue = (System.Boolean?)value;
                                ObjectProp3____rxImplementation = myValue;
                            }
                        }
                        catch(Exception)
                        {
                            ObjectProp3____rxImplementation = null;
                            myValue = null;
                        }
                     }
                    
                 }
                break;
            case 2:
                {
                    System.Byte? myValue = null;
                    lock(ObjectProp4____rxLock)
                    {
                        try
                        {
                            if(value == null)
                            {
                                ObjectProp4____rxImplementation = null;
                            }
                            else
                            {
                                myValue = (System.Byte?)value;
                                ObjectProp4____rxImplementation = myValue;
                            }
                        }
                        catch(Exception)
                        {
                            ObjectProp4____rxImplementation = null;
                            myValue = null;
                        }
                     }
                    
                    OnObjectProp4Change?.Invoke(myValue);

                 }
                break;

            case 3:
                {
                    DynamicAssembly.DynamicSubDataType? myValue = null;
                    lock(SubData____rxLock)
                    {
                        try
                        {                        
                            string? strValue = value as string;
                            if(strValue != null)
                            {
                                  myValue = JsonSerializer.Deserialize<DynamicAssembly.DynamicSubDataType>(strValue, __jsonContext);
                            }
                            SubData____rxImplementation = myValue;   
                        }
                        catch(Exception)
                        {
                            SubData____rxImplementation = null;
                            myValue = null;
                        }
                    }
                    
                }
                break;
            case 4:
                {
                    DynamicAssembly.SubNamespace.SomeOtherDynamicObject? myValue = null;
                    lock(OtherDynamicObj____rxLock)
                    {
                        try
                        {
                            if(value == null)
                            {
                                OtherDynamicObj____rxImplementation = null;
                            }
                            else
                            {
                                ulong val = (ulong)value;
                                if(val == 0)
                                {
                                    OtherDynamicObj____rxImplementation = null;
                                }
                                else
                                {
                                    myValue = __GetInstance((nint)val) as DynamicAssembly.SubNamespace.SomeOtherDynamicObject;                            
                                    OtherDynamicObj____rxImplementation = myValue;   
                                }
                            }
                        }
                        catch(Exception)
                        {
                            OtherDynamicObj____rxImplementation = null;
                            myValue = null;
                        }
                     }
                    
                 }
                break;
        }
    }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private DynamicAssembly.DynamicStruct Struktura1____rxImplementation = new DynamicAssembly.DynamicStruct();
    public override DynamicAssembly.DynamicStruct Struktura1 
    {
        get 
        {
            return Struktura1____rxImplementation;
        }
    }

    public  override Task<string> __rxExecuteMethod(string method, string args)
    {
        switch(method)
        {

                case "FunkcijaNeka22":
                    {
                         FunkcijaNeka22();
                        return Task.FromResult("{ }");
                    }
                    break;

        }
        throw new Exception($"Method {method} not found");
    }

}
}

namespace DynamicAssembly.SubNamespace__rxImplementation
{

public class SomeOtherDynamicObject : DynamicAssembly.SubNamespace.SomeOtherDynamicObject
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private System.String? OtherProp1____rxImplementation = "other prop value";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object OtherProp1____rxLock = new object();
    public override System.String? OtherProp1 
    {
        get 
        {
            lock(OtherProp1____rxLock)
            {
                return OtherProp1____rxImplementation;
            }
        }
         set
        {
            if(value != null)
            {
                WriteProperty(0, value);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private System.UInt32? OtherProp2____rxImplementation = 33;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object OtherProp2____rxLock = new object();
    public override System.UInt32? OtherProp2 
    {
        get 
        {
            lock(OtherProp2____rxLock)
            {
                return OtherProp2____rxImplementation;
            }
        }
         set
        {
            if(value != null)
            {
                WriteProperty(1, value);
            }
        }
    }

    protected override void __rxValueCallback(int index, object? value)
    {
        switch(index)
        {
            case 0:
                {
                    System.String? myValue = null;
                    lock(OtherProp1____rxLock)
                    {
                        try
                        {
                            if(value == null)
                            {
                                OtherProp1____rxImplementation = null;
                            }
                            else
                            {
                                myValue = (System.String?)value;
                                OtherProp1____rxImplementation = myValue;
                            }
                        }
                        catch(Exception)
                        {
                            OtherProp1____rxImplementation = null;
                            myValue = null;
                        }
                     }
                    
                 }
                break;
            case 1:
                {
                    System.UInt32? myValue = null;
                    lock(OtherProp2____rxLock)
                    {
                        try
                        {
                            if(value == null)
                            {
                                OtherProp2____rxImplementation = null;
                            }
                            else
                            {
                                myValue = (System.UInt32?)value;
                                OtherProp2____rxImplementation = myValue;
                            }
                        }
                        catch(Exception)
                        {
                            OtherProp2____rxImplementation = null;
                            myValue = null;
                        }
                     }
                    
                 }
                break;
        }
    }
}
}

namespace DynamicAssembly__rxImplementation
{

public class DynamicSubStruct : DynamicAssembly.DynamicSubStruct
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private System.String? SubPeriodString____rxImplementation = "zikica";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object SubPeriodString____rxLock = new object();
    public override System.String? SubPeriodString 
    {
        get 
        {
            lock(SubPeriodString____rxLock)
            {
                return SubPeriodString____rxImplementation;
            }
        }
         set
        {
            if(value != null)
            {
                WriteProperty(0, value);
            }
        }
    }

    protected override void __rxValueCallback(int index, object? value)
    {
        switch(index)
        {
            case 0:
                {
                    System.String? myValue = null;
                    lock(SubPeriodString____rxLock)
                    {
                        try
                        {
                            if(value == null)
                            {
                                SubPeriodString____rxImplementation = null;
                            }
                            else
                            {
                                myValue = (System.String?)value;
                                SubPeriodString____rxImplementation = myValue;
                            }
                        }
                        catch(Exception)
                        {
                            SubPeriodString____rxImplementation = null;
                            myValue = null;
                        }
                     }
                    
                 }
                break;
        }
    }
}
}

namespace DynamicAssembly__rxImplementation
{

public class DynamicStruct : DynamicAssembly.DynamicStruct
{
     public override event System.Action<System.String?>? OnPeriodStringChange;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private System.String? PeriodString____rxImplementation = "zikica";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object PeriodString____rxLock = new object();
    public override System.String? PeriodString 
    {
        get 
        {
            lock(PeriodString____rxLock)
            {
                return PeriodString____rxImplementation;
            }
        }
         set
        {
            if(value != null)
            {
                WriteProperty(0, value);
            }
        }
    }

    protected override void __rxValueCallback(int index, object? value)
    {
        switch(index)
        {
            case 0:
                {
                    System.String? myValue = null;
                    lock(PeriodString____rxLock)
                    {
                        try
                        {
                            if(value == null)
                            {
                                PeriodString____rxImplementation = null;
                            }
                            else
                            {
                                myValue = (System.String?)value;
                                PeriodString____rxImplementation = myValue;
                            }
                        }
                        catch(Exception)
                        {
                            PeriodString____rxImplementation = null;
                            myValue = null;
                        }
                     }
                    
                    OnPeriodStringChange?.Invoke(myValue);

                 }
                break;
        }
    }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private DynamicAssembly.DynamicSubStruct? SubStruct____rxImplementation = new DynamicAssembly.DynamicSubStruct();
    public override DynamicAssembly.DynamicSubStruct? SubStruct 
    {
        get 
        {
            return SubStruct____rxImplementation;
        }
    }

}
}

namespace DynamicAssembly__rxImplementation
{

public class DynamicSource : DynamicAssembly.DynamicSource
{
     public override event System.Action<System.String?>? OnPeriodStringChange;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private System.String? PeriodString____rxImplementation = "zikica";
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object PeriodString____rxLock = new object();
    public override System.String? PeriodString 
    {
        get 
        {
            lock(PeriodString____rxLock)
            {
                return PeriodString____rxImplementation;
            }
        }
         set
        {
            if(value != null)
            {
                WriteProperty(0, value);
            }
        }
    }

    protected override void __rxValueCallback(int index, object? value)
    {
        switch(index)
        {
            case 0:
                {
                    System.String? myValue = null;
                    lock(PeriodString____rxLock)
                    {
                        try
                        {
                            if(value == null)
                            {
                                PeriodString____rxImplementation = null;
                            }
                            else
                            {
                                myValue = (System.String?)value;
                                PeriodString____rxImplementation = myValue;
                            }
                        }
                        catch(Exception)
                        {
                            PeriodString____rxImplementation = null;
                            myValue = null;
                        }
                     }
                    
                    OnPeriodStringChange?.Invoke(myValue);

                 }
                break;
        }
    }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private DynamicAssembly.DynamicSubStruct SubStruct____rxImplementation = new DynamicAssembly.DynamicSubStruct();
    public override DynamicAssembly.DynamicSubStruct SubStruct 
    {
        get 
        {
            return SubStruct____rxImplementation;
        }
    }

    public override byte __GetWriteConvert(byte input)
    {
        byte[] delegates = { 0, 11, 11, 11, 11, 11, 11, 11, 0, 0, 11, 11, 0, 11, 0, 0, 0, 0 };
        if(input < delegates.Length)
        {
            return delegates[input];
        }
        return 0;
    }

}
}
