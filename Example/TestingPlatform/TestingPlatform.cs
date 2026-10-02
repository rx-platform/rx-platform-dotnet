using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Host;
using ENSACO.RxPlatform.Runtime;

namespace TestingPlatform
{
    [RxPlatformLibrary()]
    public class TestingPlatformMain
    {
        public static PlatformLibraryInfo Initialize(string path)
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