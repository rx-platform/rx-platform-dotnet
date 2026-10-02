using ENSACO.RxPlatform.Attributes;


namespace TestFacility;


[RxPlatformDataType(nodeId: "B1C2D3E4-F5A6-4789-ABCD-0987654321BA", directory: "data")]
[RxPlatformDeclare()]
public struct SensorData
{
    public bool HasHysterezis { get; set; }
    public double Hysteresis { get; set; }
}

[RxPlatformDataType(nodeId: "A1B2C3D4-E5F6-4789-ABCD-1234567890AB", directory: "data")]
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