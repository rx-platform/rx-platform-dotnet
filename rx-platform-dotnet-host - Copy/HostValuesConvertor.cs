using RxPlatform.Hosting.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ENSACO.RxPlatform.Hosting.Internal
{
    internal static class HostValuesConvertor
    {
        unsafe internal static bool ConvertValueFromRxString(typed_value_type* val, ref string? value)
        {
            string_value_struct str = new string_value_struct();
            if (CommonInterface.rx_get_string_value(ref *val, 0, out str) > 0)
            {
                value = Marshal.PtrToStringAnsi(CommonInterface.rx_c_str(&str));
                CommonInterface.rx_destory_string_value_struct(&str);
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxFloat(typed_value_type* val, ref double value)
        {
            double temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_float_value(ref *val, 0, out temp, out type) > 0)
            {
                value = temp;
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxUint(typed_value_type* val, ref ulong value)
        {
            ulong temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_unassigned_value(ref *val, 0, out temp, out type) > 0)
            {
                value = temp;
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxInt(typed_value_type* val, ref long value)
        {
            long temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_integer_value(ref *val, 0, out temp, out type) > 0)
            {
                value = temp;
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxBool(typed_value_type* val, ref bool value)
        {
            int temp = 0;
            if (CommonInterface.rx_get_bool_value(ref *val, 0, out temp) > 0)
            {
                value = temp != 0;
                return true;
            }
            return false;
        }


        unsafe internal static bool ConvertToRxValue(string value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_string_value(ref val, value, -1) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(double value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_double_value(ref val, value) > 0)
            {
                return true;
            }
            return false;

        }

        unsafe internal static bool ConvertToRxValue(float value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_float_value(ref val, value) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(ulong value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_uint64_value(ref val, value) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(uint value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_uint32_value(ref val, value) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(ushort value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_uint16_value(ref val, value) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(byte value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_uint8_value(ref val, value) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(bool value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_bool_value(ref val, value ? (byte)1 : (byte)0) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(long value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_int64_value(ref val, value) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(int value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_int32_value(ref val, value) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(short value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_int16_value(ref val, value) > 0)
            {
                return true;
            }
            return false;

        }
        unsafe internal static bool ConvertToRxValue(sbyte value, ref typed_value_type val)
        {
            if (CommonInterface.rx_init_int8_value(ref val, value) > 0)
            {
                return true;
            }
            return false;
        }
    }
}
