using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace RxPlatform.Hosting.Interface
{
    // --- Types from rx_common.h and rx_abi.h for interop ---

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct string_value_struct
    {
        public ulong size;
        public nint value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bytes_value_struct
    {
        public ulong size;
        public nint value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct array_value_struct
    {
        public ulong size;
        public nint values;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct struct_value_type
    {
        public ulong size;
        public nint values;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct complex_value_struct
    {
        public double real;
        public double imag;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_time_struct
    {
        public ulong t_value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_full_time
    {
        public uint year;
        public uint month;
        public uint day;
        public uint w_day;
        public uint hour;
        public uint minute;
        public uint second;
        public uint milliseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_uuid_t
    {
        public fixed byte bytes[16];
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct rx_node_id_union
    {
        [FieldOffset(0)]
        public uint int_value;
        [FieldOffset(0)]
        public string_value_struct string_value;
        [FieldOffset(0)]
        public bytes_value_struct bstring_value;
        [FieldOffset(0)]
        public rx_uuid_t uuid_value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_node_id_struct
    {
        public ushort namespace_index;
        public uint node_type;
        public rx_node_id_union value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct lock_reference_struct
    {
        public nint target;
        public int ref_count;
        public nint def;
    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct values_array_struct
    {
        public ulong size;
        public nint values;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_reference_data
    {
        public string_value_struct path;
        public rx_node_id_struct id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_reference_struct
    {
        public int is_path;
        public rx_reference_data data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_result_data
    {
        public uint code;
        public string_value_struct text;
    }

    [StructLayout(LayoutKind.Sequential)]
    [System.Runtime.CompilerServices.InlineArray(4)]
    public unsafe struct rx_result_data_array
    {
        public rx_result_data data;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct rx_result_union
    {
        [FieldOffset(0)]
        public nint ptr_data;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public rx_result_data_array static_data;
    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_result_struct
    {
        public ulong count;
        public rx_result_data_array data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_platform_init_data
    {
        public ulong version;
        public int rx_hd_timer;
        public int is_debug;
        public ulong rx_initial_heap_size;
        public ulong rx_alloc_heap_size;
        public ulong rx_heap_alloc_trigger;
        public ulong rx_bucket_capacity;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_meta_data_struct
    {
        public rx_node_id_struct id;
        public string_value_struct name;
        public string_value_struct path;
        public rx_reference_struct parent;
        public rx_time_struct created_time;
        public rx_time_struct modified_time;
        public uint version;
        public uint attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct rx_value_union
    {
        public rx_node_id_union maxVal;
    }

    public enum rx_value_t : byte
    {
        Null = 0,
        Bool = 1,
        Int8 = 2,
        UInt8 = 3,
        Int16 = 4,
        UInt16 = 5,
        Int32 = 6,
        UInt32 = 7,
        Int64 = 8,
        UInt64 = 9,
        Float = 10,
        Double = 11,
        Complex = 12,
        String = 13,
        Time = 14,
        Uuid = 15,
        Bytes = 16,
        Struct = 17,
        Type = 18,
        NodeId = 19
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct typed_value_type
    {
        public rx_value_t value_type;
        public rx_value_union value;
    }


    // lock_reference_struct2
    [StructLayout(LayoutKind.Sequential)]
    public struct lock_reference_struct2
    {
        public nint target;
        public int ref_count;
        public nint def; // lock_reference_def_struct2*
    }



    // lock_reference_def_struct
    [StructLayout(LayoutKind.Sequential)]
    public struct lock_reference_def_struct
    {
        public nint destroy_reference; // delegate pointer
    }

    // lock_reference_def_struct2
    [StructLayout(LayoutKind.Sequential)]
    public struct lock_reference_def_struct2
    {
        public nint destroy_reference; // delegate pointer
        public int version;
    }
}