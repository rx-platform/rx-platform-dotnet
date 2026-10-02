using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Runtime;


namespace TestFacility;

[RxPlatformObjectType(nodeId: "801A1925-092E-4053-BB69-A1B8C9838C41")]
[RxPlatformRuntime()]
public class Heater : RxPlatformObjectRuntime
{
    public void Started()
    {
        // perform initialization
    }
    public void Stopping()
    {
        // perform de-initialization
    }

    
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

}