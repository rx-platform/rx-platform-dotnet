

using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Model;

namespace ENSACO.RxPlatform.Hosting.Construction
{
    class RxRutimeConstructData
    {
        internal IntPtr _nativePtr = IntPtr.Zero;
        internal rx_item_type type = rx_item_type.rx_directory;// invalid type for this purpose

        internal Dictionary<string, RxRutimeConstructData> structs = new Dictionary<string, RxRutimeConstructData>();
    }

}