using ENSACO.RxPlatform.Model;
using System.Reflection;

namespace ENSACO.RxPlatform.Runtime
{

    public class RxPlatformSourceRuntime : RxPlatformRuntimeBase
    {
        override internal byte RxType { get { return 11;/*rx_source*/ } }


        protected void SourceChanged(bool value)
        {
            if (__runtimeFunctions.SourceChangedBool != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedBool(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(sbyte value)
        {
            if (__runtimeFunctions.SourceChangedInt8 != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedInt8(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(short value)
        {
            if (__runtimeFunctions.SourceChangedInt16 != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedInt16(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(int value)
        {
            if (__runtimeFunctions.SourceChangedInt32 != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedInt32(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(long value)
        {
            if (__runtimeFunctions.SourceChangedInt64 != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedInt64(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(byte value)
        {
            if (__runtimeFunctions.SourceChangedUInt8 != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedUInt8(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(ushort value)
        {
            if (__runtimeFunctions.SourceChangedUInt16 != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedUInt16(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(uint value)
        {
            if (__runtimeFunctions.SourceChangedUInt32 != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedUInt32(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(ulong value)
        {
            if (__runtimeFunctions.SourceChangedUInt64 != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedUInt64(this.__nativeObjectPtr, value);
            }
        }

        protected void SourceChanged(float value)
        {
            if (__runtimeFunctions.SourceChangedFloat != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedFloat(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(double value)
        {
            if (__runtimeFunctions.SourceChangedDouble != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedDouble(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(string value)
        {
            if (__runtimeFunctions.SourceChangedString != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedString(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(Guid value)
        {
            if (__runtimeFunctions.SourceChangedUuid != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedUuid(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(DateTime value)
        {
            if (__runtimeFunctions.SourceChangedDateTime != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedDateTime(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChanged(byte[] value)
        {
            if (__runtimeFunctions.SourceChangedBytes != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedBytes(this.__nativeObjectPtr, value);
            }
        }

        protected void SourceChanged(object value)
        {
            if (__runtimeFunctions.SourceChangedObject != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedObject(this.__nativeObjectPtr, value);
            }
        }
        protected void SourceChangedBad()
        {
            if (__runtimeFunctions.SourceChangedBad != null && this.__nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.SourceChangedBad(this.__nativeObjectPtr);
            }
        }
        public virtual byte __GetWriteConvert(byte input)
        {
            return 0;// rx_type_t.Null;
        }

    }
}