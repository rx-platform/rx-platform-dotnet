using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Host;
using ENSACO.RxPlatform.Runtime;

namespace TestFacility;

[RxPlatformLibrary()]
public class TestFacility
{
    public static PlatformLibraryInfo Initialize(string path)
    {
        return new PlatformLibraryInfo
        {
            Name = "testFacility"
        };
    }
    public static async void Start() 
    {			
        
    }
    public static void Deinitialize()
    {
    }
}
