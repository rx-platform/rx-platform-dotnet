using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using ENSACO.RxPlatform.Json;
using System.Threading.Tasks;
using System.Globalization;

namespace ENSACO.RxPlatform.Model
{
    public enum RxNodeIdType : sbyte
    {
        Numeric = 0,
        String = 1,
        Uuid = 2,
        Bytes = 3
    };

    [Serializable]
    [JsonConverter(typeof(RxNodeIdJsonConverter))]
    public struct RxNodeId
    {
        public ushort Namespace { get; set; }
        public RxNodeIdType NodeType { get; set; }

        internal object? referenceValue;
        private uint numericValue;
        private Guid uuidValue;

        public static RxNodeId NullId = new RxNodeId(0, 0);

        public bool IsNull()
        {
            return NodeType == RxNodeIdType.Numeric && Namespace == 0 && numericValue == 0;
        }

        public RxNodeId(uint intId, ushort namespaceId)
        {
            uuidValue = Guid.Empty;
            numericValue = intId;
            Namespace = namespaceId;
            NodeType = RxNodeIdType.Numeric;
            referenceValue = null;
        }
        public RxNodeId(string stringId, ushort namespaceId)
        {
            uuidValue = Guid.Empty;
            referenceValue = stringId;
            Namespace = namespaceId;
            NodeType = RxNodeIdType.String;
            numericValue = 0;
        }
        public RxNodeId(Guid guidId, ushort namespaceId)
        {
            uuidValue = guidId;
            Namespace = namespaceId;
            NodeType = RxNodeIdType.Uuid;
            referenceValue = null;
            numericValue = 0;
        }
        public RxNodeId(byte[] bytesId, ushort namespaceId)
        {
            uuidValue = Guid.Empty;
            referenceValue = bytesId;
            Namespace = namespaceId;
            NodeType = RxNodeIdType.Bytes;
            numericValue = 0;
        }

        public uint IntValue
        {
            get
            {
                if (NodeType != RxNodeIdType.Numeric)
                    throw new InvalidCastException();
                else
                    return numericValue;
            }
        }
        public string? StringValue
        {
            get
            {
                if (NodeType != RxNodeIdType.String)
                    throw new InvalidCastException();
                else
                    return referenceValue as string;
            }
        }
        public Guid UuidValue
        {
            get
            {
                if (NodeType != RxNodeIdType.Uuid)
                    throw new InvalidCastException();
                else
                    return uuidValue;
            }
        }
        public byte[]? BytesValue
        {
            get
            {
                if (NodeType != RxNodeIdType.Bytes)
                    throw new InvalidCastException();
                else
                    return referenceValue as byte[];
            }
        }
        public override string? ToString()
        {
            if (IsNull())
                return null;

            StringBuilder ret = new StringBuilder();

            ret.Append(Namespace);
            ret.Append(':');

            switch (NodeType)
            {
                case RxNodeIdType.Numeric:
                    ret.Append("i:");
                    break;
                case RxNodeIdType.Bytes:
                    ret.Append("b:");
                    break;
                case RxNodeIdType.Uuid:
                    ret.Append("g:");
                    break;
                case RxNodeIdType.String:
                    ret.Append("s:");
                    break;
            }

            switch (NodeType)
            {
                case RxNodeIdType.Numeric:
                    ret.Append(numericValue);
                    break;
                case RxNodeIdType.Bytes:
                    {
                        var bytes = referenceValue as byte[];
                        if(bytes!=null)
                        {
                            for (int i = 0; i < bytes.Length; i++)
                            {
                                ret.Append(bytes[i].ToString("X2"));
                            }
                        }
                        break;
                    }
                case RxNodeIdType.Uuid:
                    ret.Append(uuidValue);
                    break;
                case RxNodeIdType.String:
                    ret.Append(referenceValue as string);
                    break;
            }
            return ret.ToString();
        }
        public static RxNodeId OpcStandardNode(uint id)
        {
            return new RxNodeId(id, 0);
        }
        public static RxNodeId FromString(string strid)
        {
            if (string.IsNullOrEmpty(strid))
                return new RxNodeId();

            int idx1 = strid.IndexOf(':');
            if (idx1 > 0)
            {// found first separator
                int idx2 = strid.IndexOf(':', idx1 + 1);
                int type = -1;
                ushort namesp = 1;
                if (idx2 > 0)
                {// try to parse namespace
                    if (!ushort.TryParse(strid.Substring(0, idx1), out namesp))
                    {
                        if (type != 1)
                        {// if not string and no namespace return invalid
                            return RxNodeId.NullId;
                        }
                        else
                        {// type is string so : is possible somewhere
                        }
                    }
                    else
                    {// namespace parsed so set idx1
                        idx1 = idx2;
                    }
                }
                switch (strid[idx1 - 1])
                {
                    case 'i':
                        type = 0;
                        break;
                    case 's':
                        type = 1;
                        break;
                    case 'g':
                        type = 2;
                        break;
                    case 'b':
                        type = 3;
                        break;
                }
                if (type >= 0)
                {
                    string value;
                    {// no namespace so zero is the answer
                        value = strid.Substring(idx1 + 1);
                    }
                    switch (type)
                    {
                        case 0:// int value
                            {
                                UInt32 val = 0;
                                if (UInt32.TryParse(value, out val))
                                    return new RxNodeId(val, namesp);
                            }
                            break;
                        case 1:// string value
                            return new RxNodeId(value, namesp);
                        case 2:// uuid value
                            {
                                try
                                {
                                    return new RxNodeId(new Guid(value), namesp);
                                }
                                catch (Exception)
                                {
                                }
                            }
                            break;
                        case 3:// bytes value
                            {
                                List<byte> val = new List<byte>();
                                for (int i = 0; i < value.Length; i += 2)
                                {
                                    string temp = value.Substring(i, 2);
                                    byte bval = 0;
                                    if (!byte.TryParse(temp, NumberStyles.HexNumber, null, out bval))
                                    {
                                        return RxNodeId.NullId;
                                    }
                                    else
                                        val.Add(bval);
                                }
                                return new RxNodeId(val.ToArray(), namesp);
                            }
                    }
                }
            }
            else
            {// parse as guid
                Guid guid;
                if (Guid.TryParse(strid, out guid))
                {
                    return new RxNodeId(guid, 999);
                }
            }
            return RxNodeId.NullId;
        }
    }
}
