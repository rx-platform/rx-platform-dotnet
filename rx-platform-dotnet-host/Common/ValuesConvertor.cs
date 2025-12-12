using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace ENSACO.RxPlatform.Hosting.Common
{
    internal static class ValuesConvertor
    {

        unsafe internal static bool ConvertValueFromRxString(ref typed_value_type val, ref string? value)
        {
            string_value_struct str = new string_value_struct();
            if (CommonInterface.rx_get_string_value(ref val, 0, out str) > 0)
            {
                value = Marshal.PtrToStringAnsi(CommonInterface.rx_c_str(&str));
                CommonInterface.rx_destory_string_value_struct(&str);
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxUuid(ref typed_value_type val, ref Guid value)
        {
            return false;
        }
        
        unsafe internal static bool ConvertValueFromRxFloat(ref typed_value_type val, ref double value)
        {
            double temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_float_value(ref val, 0, out temp, out type) > 0)
            {
                value = temp;
                return true;
            }
            return false;
        }

        unsafe internal static bool ConvertValueFromRxFloat(ref typed_value_type val, ref float value)
        {
            double temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_float_value(ref val, 0, out temp, out type) > 0)
            {
                value = (float)temp;
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxUInt(ref typed_value_type val, ref ulong value)
        {
            ulong temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_unassigned_value(ref val, 0, out temp, out type) > 0)
            {
                value = temp;
                return true;
            }
            return false;
        }

        unsafe internal static bool ConvertValueFromRxUInt(ref typed_value_type val, ref byte value)
        {
            ulong temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_unassigned_value(ref val, 0, out temp, out type) > 0)
            {
                value = (byte)temp;
                return true;
            }
            return false;
        }

        unsafe internal static bool ConvertValueFromRxUInt(ref typed_value_type val, ref ushort value)
        {
            ulong temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_unassigned_value(ref val, 0, out temp, out type) > 0)
            {
                value = (ushort)temp;
                return true;
            }
            return false;
        }

        unsafe internal static bool ConvertValueFromRxUInt(ref typed_value_type val, ref uint value)
        {
            ulong temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_unassigned_value(ref val, 0, out temp, out type) > 0)
            {
                value = (uint)temp;
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxInt(ref typed_value_type val, ref long value)
        {
            long temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_integer_value(ref val, 0, out temp, out type) > 0)
            {
                value = temp;
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxInt(ref typed_value_type val, ref sbyte value)
        {
            long temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_integer_value(ref val, 0, out temp, out type) > 0)
            {
                value = (sbyte)temp;
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxInt(ref typed_value_type val, ref short value)
        {
            long temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_integer_value(ref val, 0, out temp, out type) > 0)
            {
                value = (short)temp;
                return true;
            }
            return false;
        }

        unsafe internal static bool ConvertValueFromRxInt(ref typed_value_type val, ref int value)
        {
            long temp = 0;
            byte type = 0;
            if (CommonInterface.rx_get_integer_value(ref val, 0, out temp, out type) > 0)
            {
                value = (int)temp;
                return true;
            }
            return false;
        }
        unsafe internal static bool ConvertValueFromRxBool(ref typed_value_type val, ref bool value)
        {
            int temp = 0;
            if (CommonInterface.rx_get_bool_value(ref val, 0, out temp) > 0)
            {
                value = temp != 0;
                return true;
            }
            return false;
        }

        unsafe internal static bool ConvertValueFromRx(ref typed_value_type val, ref object? value)
        {
            int temp = 0;
            switch(val.value_type)
            {
                case rx_value_t.Null:
                    value = null;
                    return true;
                case rx_value_t.String:
                    {
                        string? str = string.Empty;
                        if (ConvertValueFromRxString(ref val, ref str))
                        {
                            value = str;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.Double:
                    {
                        double d = 0;
                        if (ConvertValueFromRxFloat(ref val, ref d))
                        {
                            value = d;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.Float:
                    {
                        float f = 0;
                        if (ConvertValueFromRxFloat(ref val, ref f))
                        {
                            value = f;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.UInt64:
                    {
                        ulong ul = 0;
                        if (ConvertValueFromRxUInt(ref val, ref ul))
                        {
                            value = ul;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.UInt32:
                    {
                        uint ui = 0;
                        if (ConvertValueFromRxUInt(ref val, ref ui))
                        {
                            value = ui;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.UInt16:
                    {
                        ushort us = 0;
                        if (ConvertValueFromRxUInt(ref val, ref us))
                        {
                            value = us;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.UInt8:
                    {
                        byte b = 0;
                        if (ConvertValueFromRxUInt(ref val, ref b))
                        {
                            value = b;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.Int64:
                    {
                        long l = 0;
                        if (ConvertValueFromRxInt(ref val, ref l))
                        {
                            value = l;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.Int32:
                    {
                        int i = 0;
                        if (ConvertValueFromRxInt(ref val, ref i))
                        {
                            value = i;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.Int16:
                    {
                        short s = 0;
                        if (ConvertValueFromRxInt(ref val, ref s))
                        {
                            value = s;
                            return true;
                        }
                        return false;
                    }
                    case rx_value_t.Int8:
                    {
                        sbyte sb = 0;
                        if (ConvertValueFromRxInt(ref val, ref sb))
                        {
                            value = sb;
                            return true;
                        }
                        return false;
                    }

            }
            if (CommonInterface.rx_get_bool_value(ref val, 0, out temp) > 0)
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

        internal static bool ConvertToRxValue(object? value, out typed_value_type val)
        {
            val = new typed_value_type();
            if (value == null)
            {
                CommonInterface.rx_init_null_value(ref val);
                return true;
            }
            if (value is double d)
            {
                return ConvertToRxValue(d, ref val);
            }
            else if (value is float f)
            {
                return ConvertToRxValue(f, ref val);
            }
            else if (value is ulong ul)
            {
                return ConvertToRxValue(ul, ref val);
            }
            else if (value is uint ui)
            {
                return ConvertToRxValue(ui, ref val);
            }
            else if (value is ushort us)
            {
                return ConvertToRxValue(us, ref val);
            }
            else if (value is byte b)
            {
                return ConvertToRxValue(b, ref val);
            }
            else if (value is long l)
            {
                return ConvertToRxValue(l, ref val);
            }
            else if (value is int i)
            {
                return ConvertToRxValue(i, ref val);
            }
            else if (value is short s)
            {
                return ConvertToRxValue(s, ref val);
            }
            else if (value is sbyte sb)
            {
                return ConvertToRxValue(sb, ref val);
            }
            else if (value is bool bo)
            {
                return ConvertToRxValue(bo, ref val);
            }
            else if (value is string str)
            {
                return ConvertToRxValue(str, ref val);
            }
            else if(value.GetType().GetCustomAttribute<RxPlatformDataType>()!=null)
            {
                string jsonVal = JsonSerializer.Serialize(value, value.GetType(), PlatformHostMain.JsonContext);
                return ConvertToRxValue(jsonVal, ref val);
            }
            return false;
        }
    }
}
