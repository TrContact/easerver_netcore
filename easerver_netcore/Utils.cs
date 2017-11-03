using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace easerver_netcore {
    public static class Extensions {

        public static string ToHex(this byte[] bytes) {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx) {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }

        public static byte[] HexToBytes(this string str) {
            if (str.Length == 0 || str.Length % 2 != 0)
                return new byte[0];

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx) {
                // Convert first half of byte
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                // Convert second half of byte
                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
            }

            return buffer;
        }

    }
    public class Utils {

        public static class RC4 {
            public static string Encrypt(string key, string data) {
                Encoding unicode = Encoding.Unicode;

                return Convert.ToBase64String(Encrypt(unicode.GetBytes(key), unicode.GetBytes(data)));
            }

            public static string Decrypt(string key, string data) {
                Encoding unicode = Encoding.Unicode;

                return unicode.GetString(Encrypt(unicode.GetBytes(key), Convert.FromBase64String(data)));
            }

            public static byte[] Encrypt(byte[] key, byte[] data) {
                return EncryptOutput(key, data).ToArray();
            }

            public static byte[] Decrypt(byte[] key, byte[] data) {
                return EncryptOutput(key, data).ToArray();
            }

            private static byte[] EncryptInitalize(byte[] key) {
                byte[] s = Enumerable.Range(0, 256)
                  .Select(i => (byte)i)
                  .ToArray();

                for (int i = 0, j = 0; i < 256; i++) {
                    j = (j + key[i % key.Length] + s[i]) & 255;

                    Swap(s, i, j);
                }

                return s;
            }

            private static IEnumerable<byte> EncryptOutput(byte[] key, IEnumerable<byte> data) {
                byte[] s = EncryptInitalize(key);

                int i = 0;
                int j = 0;

                return data.Select((b) =>
                {
                    i = (i + 1) & 255;
                    j = (j + s[i]) & 255;

                    Swap(s, i, j);

                    return (byte)(b ^ s[(s[i] + s[j]) & 255]);
                });
            }

            private static void Swap(byte[] s, int i, int j) {
                byte c = s[i];

                s[i] = s[j];
                s[j] = c;
            }
        }

        public static byte[] Decompress(byte[] data) {
            // init a buffer, size comes from eamuemu
            byte[] res = new byte[0x190000];
            // current data location
            int p = 0;
            // current output location
            int r = 0;
            // traceback location
            int t = 0;
            // bitmask location
            int b = 8; // read next bitmask byte on start
            byte mask = 0;

            while (true) {
                // read bitmask on block end
                if (b == 8) {
                    mask = data[p];
                    p += 1;
                    b = 0;
                }
                // get mask for next byte
                if ((mask & 1) == 1) { // not coded
                    // copy the byte from data to result
                    res[r] = data[p];
                    r += 1;
                    p += 1;
                } else { // coded
                    // read descriptors
                    int distance = data[p];
                    int count = data[p + 1];
                    // EOF mark
                    if (distance == 0 && count == 0) {
                        break;
                    }
                    p += 2;
                    // shift to correct location
                    distance <<= 4;
                    distance |= count >> 4;
                    count = (count & 0x0F) + 3;
                    // copy earlier result bytes to the end
                    t = r - distance; // initialize traceback location
                    for (int i = 0; i < count; i++) {
                        res[r] = t < 0 ? (byte)0x00 : res[t];
                        r += 1;
                        t += 1;
                    }
                }
                // shift mask
                mask >>= 1;
                b += 1;
            }
            // r = result length
            byte[] output = new byte[r];
            Array.Copy(res, output, r);
            return output;
        }

        public static byte[] CompressEmpty(byte[] data) {
            byte[] res = new byte[data.Length + data.Length / 8 + 3];
            int p = 0;
            for (int i = 0; i < data.Length; i++) {
                if (i % 8 == 0) {
                    if (data.Length - i < 8) {
                        res[p] = (byte) (Math.Pow(2, data.Length - i) - 1);
                    } else {
                        res[p] = 255;
                    }
                    p += 1;
                }
                res[p] = data[i];
                p += 1;
            }
            res[p] = 0;
            res[p + 1] = 0;
            return res;
        }

        public static string m_encoding = "SHIFT_JIS";
        #region XMLToBinary
        public static void XMLToBinary(byte[] input, ref byte[] output) {
            byte[] numArray1 = new byte[4];
            byte[] numArray2 = new byte[4];
            byte[] numArray3 = new byte[4096];
            byte[] numArray4 = new byte[4];
            byte[] numArray5 = new byte[4096];
            string[] array1 = new string[13]
            {
      "s8",
      "u8",
      "s16",
      "u16",
      "s32",
      "u32",
      "s64",
      "u64",
      "bin",
      "str",
      "ip4",
      "time",
      "bool"
            };
            string[] array2 = new string[3]
            {
      "__type",
      "__count",
      "__size"
            };
            int[] numArray6 = new int[13]
            {
      2,
      3,
      4,
      5,
      6,
      7,
      8,
      9,
      10,
      11,
      12,
      13,
      52
            };
            int pt = 0;
            int num1 = 0;
            int wordp = 0;
            int bytep = 0;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(Encoding.GetEncoding(m_encoding).GetString(input));
            byte[] bytes1 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(-1606254465));
            int num2 = 0;
            XmlNode node = (XmlNode)xmlDocument.DocumentElement;
            while (num2 == 0) {
                string str1 = CXMLmng.node(node, "@__type").Value;
                string str2 = CXMLmng.node(node, "@__count").Value;
                string str3 = CXMLmng.node(node, "@__size").Value;
                string innerText = node.InnerText;
                int num3;
                byte kind;
                if (node.HasChildNodes && node.FirstChild.NodeType != XmlNodeType.Text) {
                    num3 = 1;
                } else {
                    num3 = 0;
                    if (str1 != null || !(innerText == "")) {
                        int index = Array.IndexOf<string>(array1, str1);
                        if (index < 0) {
                            index = 9;
                            str1 = array1[9];
                        }
                        kind = (byte)numArray6[index];
                        if (((uint)kind < 10U || (int)kind == 52) && str2 != null) {
                            kind |= (byte)64;
                            goto label_10;
                        } else
                            goto label_10;
                    }
                }
                kind = (byte)1;
                label_10:
                XMLToBinaryNode(kind, node.Name, ref numArray3, ref pt);
                if (num3 == 0) {
                    if (str2 != null)
                        Convert.ToInt32(str2);
                    if (str3 != null)
                        Convert.ToInt32(str3);
                    if (str1 != null || innerText != "") {
                        char[] chArray = new char[1] { ' ' };
                        string[] strArray = innerText.Split(chArray);
                        switch (Array.IndexOf<string>(array1, str1)) {
                            case 0:
                                if (str2 == null) {
                                    XMLToBinarySetBytes((long)Convert.ToSByte(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 1);
                                    break;
                                }
                                int length1 = strArray.Length;
                                byte[] data1 = new byte[length1];
                                int index1 = 0;
                                if (0 < length1) {
                                    do {
                                        data1[index1] = (byte)Convert.ToSByte(strArray[index1]);
                                        ++index1;
                                    }
                                    while (index1 < strArray.Length);
                                }
                                XMLToBinarySetData(ref data1, ref numArray5, ref num1);
                                break;
                            case 1:
                                if (str2 == null) {
                                    XMLToBinarySetBytes((long)Convert.ToByte(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 1);
                                    break;
                                }
                                int length2 = strArray.Length;
                                byte[] data2 = new byte[length2];
                                int index2 = 0;
                                if (0 < length2) {
                                    do {
                                        data2[index2] = Convert.ToByte(strArray[index2]);
                                        ++index2;
                                    }
                                    while (index2 < strArray.Length);
                                }
                                XMLToBinarySetData(ref data2, ref numArray5, ref num1);
                                break;
                            case 2:
                                if (str2 == null) {
                                    XMLToBinarySetBytes((long)Convert.ToInt16(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 2);
                                    break;
                                }
                                int length3 = strArray.Length;
                                byte[] data3 = new byte[length3 << 1];
                                int index3 = 0;
                                if (0 < length3) {
                                    do {
                                        Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt16(strArray[index3]))), 0, (Array)data3, index3 << 1, 2);
                                        ++index3;
                                    }
                                    while (index3 < strArray.Length);
                                }
                                XMLToBinarySetData(ref data3, ref numArray5, ref num1);
                                break;
                            case 3:
                                if (str2 == null) {
                                    XMLToBinarySetBytes((long)Convert.ToUInt16(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 2);
                                    break;
                                }
                                int length4 = strArray.Length;
                                byte[] data4 = new byte[length4 << 1];
                                int index4 = 0;
                                if (0 < length4) {
                                    do {
                                        Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)Convert.ToUInt16(strArray[index4]))), 0, (Array)data4, index4 << 1, 2);
                                        ++index4;
                                    }
                                    while (index4 < strArray.Length);
                                }
                                XMLToBinarySetData(ref data4, ref numArray5, ref num1);
                                break;
                            case 4:
                                if (str2 == null) {
                                    XMLToBinarySetBytes((long)Convert.ToInt32(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 4);
                                    break;
                                }
                                int length5 = strArray.Length;
                                byte[] data5 = new byte[length5 << 2];
                                int index5 = 0;
                                if (0 < length5) {
                                    do {
                                        Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt32(strArray[index5]))), 0, (Array)data5, index5 << 2, 4);
                                        ++index5;
                                    }
                                    while (index5 < strArray.Length);
                                }
                                XMLToBinarySetData(ref data5, ref numArray5, ref num1);
                                break;
                            case 5:
                                if (str2 == null) {
                                    XMLToBinarySetBytes((long)Convert.ToUInt32(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 4);
                                    break;
                                }
                                int length6 = strArray.Length;
                                byte[] data6 = new byte[length6 << 2];
                                int index6 = 0;
                                if (0 < length6) {
                                    do {
                                        Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)Convert.ToUInt32(strArray[index6]))), 0, (Array)data6, index6 << 2, 4);
                                        ++index6;
                                    }
                                    while (index6 < strArray.Length);
                                }
                                XMLToBinarySetData(ref data6, ref numArray5, ref num1);
                                break;
                            case 6:
                                if (str2 == null) {
                                    XMLToBinarySetBytes(Convert.ToInt64(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 8);
                                    break;
                                }
                                int length7 = strArray.Length;
                                byte[] data7 = new byte[length7 << 3];
                                int index7 = 0;
                                if (0 < length7) {
                                    do {
                                        Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt64(strArray[index7]))), 0, (Array)data7, index7 << 3, 8);
                                        ++index7;
                                    }
                                    while (index7 < strArray.Length);
                                }
                                XMLToBinarySetData(ref data7, ref numArray5, ref num1);
                                break;
                            case 7:
                                if (str2 == null) {
                                    XMLToBinarySetBytes((long)Convert.ToUInt64(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 8);
                                    break;
                                }
                                int length8 = strArray.Length;
                                byte[] data8 = new byte[length8 << 3];
                                int index8 = 0;
                                if (0 < length8) {
                                    do {
                                        Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)Convert.ToUInt64(strArray[index8]))), 0, (Array)data8, index8 << 3, 8);
                                        ++index8;
                                    }
                                    while (index8 < strArray.Length);
                                }
                                XMLToBinarySetData(ref data8, ref numArray5, ref num1);
                                break;
                            case 8:
                                byte[] data9 = new byte[(innerText.Length + 1) / 2];
                                int index9 = 0;
                                if (0 < data9.Length) {
                                    do {
                                        data9[index9] = Convert.ToByte(innerText.Substring(index9 << 1, 2), 16);
                                        ++index9;
                                    }
                                    while (index9 < data9.Length);
                                }
                                XMLToBinarySetData(ref data9, ref numArray5, ref num1);
                                break;
                            case 9:
                                byte[] bytes2 = Encoding.GetEncoding(m_encoding).GetBytes(innerText + "\0");
                                XMLToBinarySetData(ref bytes2, ref numArray5, ref num1);
                                break;
                            case 10:
                                XMLToBinarySetBytes((long)BitConverter.ToUInt32(IPAddress.Parse(innerText).GetAddressBytes(), 0), ref numArray5, ref num1, ref wordp, ref bytep, 4);
                                break;
                            case 11:
                                XMLToBinarySetBytes((long)Convert.ToUInt32(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 4);
                                break;
                            case 12:
                                if (str2 == null) {
                                    XMLToBinarySetBytes((long)Convert.ToByte(strArray[0]), ref numArray5, ref num1, ref wordp, ref bytep, 1);
                                    break;
                                }
                                int length9 = strArray.Length;
                                byte[] data10 = new byte[length9];
                                int index10 = 0;
                                if (0 < length9) {
                                    do {
                                        data10[index10] = Convert.ToByte(strArray[index10]);
                                        ++index10;
                                    }
                                    while (index10 < strArray.Length);
                                }
                                XMLToBinarySetData(ref data10, ref numArray5, ref num1);
                                break;
                        }
                    }
                }
                string[] array3 = new string[node.Attributes.Count];
                string[] array4 = new string[array3.Length];
                int newSize1 = 0;
                foreach (XmlAttribute attribute in (XmlNamedNodeMap)node.Attributes) {
                    if (Array.IndexOf<string>(array2, attribute.Name) < 0) {
                        array3[newSize1] = attribute.Name;
                        array4[newSize1] = attribute.Value;
                        ++newSize1;
                    }
                }
                if (array3.Length != newSize1) {
                    Array.Resize<string>(ref array3, newSize1);
                    Array.Resize<string>(ref array4, newSize1);
                }
                IComparer comparer = (IComparer)new i_Comparer();
                Array.Sort((Array)array3, (Array)array4, comparer);
                int index11 = 0;
                if (0 < array3.Length) {
                    do {
                        XMLToBinaryNode((byte)46, array3[index11], ref numArray3, ref pt);
                        byte[] bytes2 = Encoding.GetEncoding(m_encoding).GetBytes(array4[index11] + "\0");
                        XMLToBinarySetData(ref bytes2, ref numArray5, ref num1);
                        ++index11;
                    }
                    while (index11 < array3.Length);
                }
                if (num3 != 0)
                    node = node.FirstChild;
                else if (num2 == 0) {
                    int newSize2 = pt + 1;
                    XmlNode nextSibling;
                    while (true) {
                        if (numArray3.Length < newSize2)
                            Array.Resize<byte>(ref numArray3, newSize2);
                        numArray3[pt] = (byte)254;
                        ++pt;
                        ++newSize2;
                        nextSibling = node.NextSibling;
                        if (nextSibling == null) {
                            if (node.ParentNode.GetType() != typeof(XmlDocument))
                                node = node.ParentNode;
                            else
                                break;
                        } else
                            goto label_91;
                    }
                    int newSize3 = pt + 1;
                    if (numArray3.Length < newSize3)
                        Array.Resize<byte>(ref numArray3, newSize3);
                    numArray3[pt] = byte.MaxValue;
                    pt = newSize3;
                    num2 = 1;
                    continue;
                    label_91:
                    node = nextSibling;
                }
            }
            pt = pt + 3 & -4;
            if (numArray3.Length < pt)
                Array.Resize<byte>(ref numArray3, pt);
            int num4 = bytep <= wordp ? wordp : bytep;
            int num5 = (num4 <= num1 ? num1 : num4) + 3 & -4;
            if (numArray5.Length < num5)
                Array.Resize<byte>(ref numArray5, num5);
            Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder(pt)), 0, (Array)numArray2, 0, 4);
            Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder(num5)), 0, (Array)numArray4, 0, 4);
            Array.Resize<byte>(ref output, bytes1.Length + (numArray2.Length + num5 + numArray4.Length) + pt);
            Array.Copy((Array)bytes1, 0, (Array)output, 0, bytes1.Length);
            Array.Copy((Array)numArray2, 0, (Array)output, bytes1.Length, numArray2.Length);
            Array.Copy((Array)numArray3, 0, (Array)output, numArray2.Length + bytes1.Length, pt);
            Array.Copy((Array)numArray4, 0, (Array)output, bytes1.Length + (pt + numArray2.Length), numArray4.Length);
            Array.Copy((Array)numArray5, 0, (Array)output, bytes1.Length + (numArray2.Length + pt + numArray4.Length), num5);
        }
        static void CompressBits(string str, ref byte[] @out, ref int pt, byte bits) {
            int num1 = 0;
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            int num2 = (bytes.Length * (int)bits + 7) / 8 + 1;
            int newSize = num2 + pt;
            if (@out.Length < newSize)
                Array.Resize<byte>(ref @out, newSize);
            @out[pt] = (byte)bytes.Length;
            int index1 = pt + 1;
            Array.Clear((Array)@out, index1, num2 - 1);
            int index2 = 0;
            if (0 < bytes.Length) {
                do {
                    byte num3 = bytes[index2];
                    byte num4 = (uint)(byte)((uint)num3 + 208U) > 10U ? ((uint)(byte)((uint)num3 + 191U) > 25U ? ((int)num3 != 95 ? ((uint)(byte)((uint)num3 + 159U) > 25U ? (byte)0 : (byte)((int)num3 - 59)) : (byte)((int)num3 - 58)) : (byte)((int)num3 - 54)) : (byte)((int)num3 - 48);
                    int num5 = (int)bits;
                    int num6 = num5;
                    if (0 < num6) {
                        int num7 = num5 - 1;
                        uint num8 = (uint)num6;
                        do {
                            int index3 = index1 + num1 / 8;
                            byte[] numArray = @out;
                            numArray[index3] = (byte)((int)numArray[index3] | ((int)((uint)num4 >> (int)(byte)num7) & 1) << 7 - num1 % 8);
                            ++num1;
                            --num7;
                            --num8;
                        }
                        while (num8 > 0U);
                    }
                    ++index2;
                }
                while (index2 < bytes.Length);
            }
            pt += num2;
        }
        static void XMLToBinaryNode(byte kind, string name, ref byte[] @out, ref int pt) {
            int newSize = pt + 1;
            if (@out.Length < newSize)
                Array.Resize<byte>(ref @out, newSize);
            @out[pt] = kind;
            ++pt;
            CompressBits(name, ref @out, ref pt, (byte)6);
        }
        static void XMLToBinarySetData(ref byte[] data, ref byte[] @out, ref int pt) {
            int num = data.Length + 4;
            int newSize = num + pt;
            if (@out.Length < newSize)
                Array.Resize<byte>(ref @out, newSize);
            Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length)), 0, (Array)@out, pt, 4);
            byte[] numArray = data;
            Array.Copy((Array)numArray, 0, (Array)@out, pt + 4, numArray.Length);
            pt = num + pt + 3 & -4;
        }
        static void XMLToBinarySetBytes(long data, ref byte[] @out, ref int datap, ref int wordp, ref int bytep, int size) {
            if (bytep % 4 == 0)
                bytep = datap;
            if (wordp % 4 == 0)
                wordp = datap;
            switch (size) {
                case 1:
                    int newSize1 = size + bytep;
                    if (@out.Length < newSize1)
                        Array.Resize<byte>(ref @out, newSize1);
                    @out[bytep] = (byte)(int)data;
                    bytep += size;
                    break;
                case 2:
                    int newSize2 = size + wordp;
                    if (@out.Length < newSize2)
                        Array.Resize<byte>(ref @out, newSize2);
                    Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(int)data)), 0, (Array)@out, wordp, size);
                    wordp += size;
                    break;
                case 4:
                    int newSize3 = size + datap;
                    if (@out.Length < newSize3)
                        Array.Resize<byte>(ref @out, newSize3);
                    Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)data)), 0, (Array)@out, datap, size);
                    datap += size;
                    break;
                case 8:
                    int newSize4 = size + datap;
                    if (@out.Length < newSize4)
                        Array.Resize<byte>(ref @out, newSize4);
                    Array.Copy((Array)BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data)), 0, (Array)@out, datap, size);
                    datap += size;
                    break;
            }
            int num1 = bytep;
            int num2 = wordp;
            int num3 = num1 <= num2 ? num2 : num1;
            if (datap >= num3)
                return;
            datap = num3 + 3 & -4;
        }
        class CXMLmng : IDisposable {
            public static workNode node(XmlNode node, string xpath) {
                return new workNode(node, xpath);
            }
            public virtual void Dispose() {
                Dispose();
                GC.SuppressFinalize((object)this);
            }
        }
        public class workNode {
            public XmlNode m_node;
            public XmlNode m_nodeorg;
            public string m_xpath;

            public string InnerXml {
                get {
                    XmlNode node = m_node;
                    if (node != null)
                        return node.InnerXml;
                    return (string)null;
                }
                set {
                    if (m_node == null)
                        buildnode();
                    m_node.InnerXml = value;
                }
            }

            public string InnerText {
                get {
                    XmlNode node = m_node;
                    if (node != null)
                        return node.InnerText;
                    return (string)null;
                }
                set {
                    if (m_node == null)
                        buildnode();
                    m_node.InnerText = value;
                }
            }

            public string Value {
                get {
                    XmlNode node = m_node;
                    if (node != null)
                        return node.Value;
                    return (string)null;
                }
                set {
                    if (m_node == null)
                        buildnode();
                    m_node.Value = value;
                }
            }

            public XmlNode Build {
                get {
                    if (m_node == null)
                        buildnode();
                    return m_node ?? (XmlNode)null;
                }
            }

            //public XmlNode this[] {
            //    get {
            //        return m_node ?? (XmlNode)null;
            //    }
            //    set {
            //        if (m_node == null)
            //            buildnode();
            //        m_node = value;
            //    }
            //}

            public XmlNode this[string name] {
                get {
                    XmlNode node = m_node;
                    if (node != null)
                        return (XmlNode)node[name];
                    return (XmlNode)null;
                }
            }

            public workNode(XmlNode node, string xpath) {
                m_nodeorg = node;
                m_node = node.SelectSingleNode(xpath);
                m_xpath = xpath;
            }

            public void buildnode() {
                string[] strArray = m_xpath.Split('/');
                if (strArray[0] == "") {
                    workNode workNode = this;
                    XmlDocument ownerDocument = workNode.m_nodeorg.OwnerDocument;
                    workNode.m_node = (XmlNode)ownerDocument;
                } else {
                    workNode workNode = this;
                    XmlNode nodeorg = workNode.m_nodeorg;
                    workNode.m_node = nodeorg;
                }
                int index = 0;
                if (0 >= strArray.Length)
                    return;
                do {
                    string name1 = strArray[index];
                    if (name1 != "") {
                        if (name1.IndexOf("@") >= 0) {
                            string name2 = name1.Replace("@", "");
                            if (m_node.Attributes[name2] == null)
                                m_node.Attributes.Append(m_node.OwnerDocument.CreateAttribute(name2));
                            workNode workNode = this;
                            XmlAttribute attribute = workNode.m_node.Attributes[name2];
                            workNode.m_node = (XmlNode)attribute;
                        } else {
                            if (m_node[name1] == null)
                                m_node.AppendChild((XmlNode)m_node.OwnerDocument.CreateElement(name1));
                            workNode workNode = this;
                            XmlElement xmlElement = workNode.m_node[name1];
                            workNode.m_node = (XmlNode)xmlElement;
                        }
                    }
                    ++index;
                }
                while (index < strArray.Length);
            }
        }
        class i_Comparer : IComparer {
            public int Compare(object x, object y) {
                byte[] bytes1 = Encoding.ASCII.GetBytes((string)x);
                byte[] bytes2 = Encoding.ASCII.GetBytes((string)y);
                int length1 = bytes2.Length;
                int length2 = bytes1.Length;
                int num1 = length2 >= length1 ? length1 : length2;
                int index = 0;
                int num2;
                if (0 < num1) {
                    do {
                        byte num3 = bytes1[index];
                        byte num4 = bytes2[index];
                        if ((uint)num3 >= (uint)num4) {
                            if ((uint)num3 <= (uint)num4)
                                ++index;
                            else
                                goto label_5;
                        } else
                            goto label_4;
                    }
                    while (index < num1);
                    goto label_7;
                    label_4:
                    num2 = -1;
                    goto label_6;
                    label_5:
                    num2 = 1;
                    label_6:
                    if (index < num1)
                        goto label_8;
                }
                label_7:
                num2 = length2 >= length1 ? (length2 > length1 ? 1 : 0) : -1;
                label_8:
                return num2;
            }
        }
        #endregion

        #region BinaryToXML
        public static void BinaryToXML(byte[] input, ref byte[] output) {
            int num = 0;
            int num3 = 0;
            XmlDocument node = new XmlDocument();
            XmlDeclaration newChild = node.CreateXmlDeclaration("1.0", m_encoding, null);
            node.InsertBefore(newChild, node.DocumentElement);
            int nodep = 8;
            if (4 <= (input.Length - 4)) {
                num = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(input, 4)) + 8;
            }
            int datap = num + 4;
            int wordp = datap;
            int bytep = datap;
            if (num <= (input.Length - 4)) {
                num3 = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(input, num)) + datap;
            }
            BinaryToXMLnode(input, ref nodep, ref datap, ref bytep, ref wordp, num, num3, node);
            output = Encoding.GetEncoding(m_encoding).GetBytes(node.OuterXml);
        }
        static void BinaryToXMLnode(byte[] bin, ref int nodep, ref int datap, ref int bytep, ref int wordp, int nodee, int datae, XmlNode node) {
            string str = null;
            int length = 0;
            string name = null;
            byte[] @out = new byte[0];
            byte[] buffer = new byte[0];
            int num21 = 0;
            if (nodep >= nodee) {
                return;
            }
            Label_0020:
            if (num21 != 0) {
                return;
            }
            if (bin[nodep] == 0) {
                do {
                    int num20 = nodep;
                    if (num20 >= nodee) {
                        break;
                    }
                    nodep = num20 + 1;
                }
                while (bin[nodep] == 0);
            }
            int index = nodep;
            byte num17 = bin[index];
            nodep = index + 1;
            switch (num17) {
                case 0xfe:
                case 0xff:
                    break;

                default: {
                        byte len = bin[index + 1];
                        nodep = (index + 1) + 1;
                        DecompressBits(bin, ref @out, ref nodep, len, 6);
                        int num3 = 0;
                        int num18 = len;
                        if (0 < num18) {
                            do {
                                byte num4 = @out[num3];
                                if (num4 < 11) {
                                    @out[num3] = (byte)(num4 + 0x30);
                                } else if (num4 < 0x25) {
                                    @out[num3] = (byte)(num4 + 0x36);
                                } else {
                                    int num25 = (num4 < 0x26) ? 0x3a : 0x3b;
                                    @out[num3] = (byte)(num4 + num25);
                                }
                                num3++;
                            }
                            while (num3 < num18);
                        }
                        name = Encoding.ASCII.GetString(@out);
                        break;
                    }
            }
            string str3 = "";
            string str2 = str3;
            byte num = (byte)(num17 & 0x40);
            switch ((num17 & 0xbf)) {
                case 1:
                    if (node.GetType() != typeof(XmlDocument)) {
                        node = node.AppendChild(node.OwnerDocument.CreateElement(name));
                    } else {
                        node = node.AppendChild(((XmlDocument)node).CreateElement(name));
                    }
                    goto Label_0B13;

                case 2:
                    str2 = "s8";
                    if (num != 0) {
                        BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                        str = "";
                        byte[] buffer5 = buffer;
                        int num16 = 0;
                        if (0 < buffer.Length) {
                            do {
                                byte num23 = buffer5[num16];
                                str = str + (Convert.ToString((int)((sbyte)num23)) + " ");
                                num16++;
                            }
                            while (num16 < buffer5.Length);
                        }
                        char[] trimChars = new char[] { ' ' };
                        str = str.TrimEnd(trimChars);
                        length = buffer.Length;
                    } else {
                        BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 1, ref buffer);
                        str = Convert.ToString((int)((sbyte)buffer[0]));
                    }
                    goto Label_0B13;

                case 3:
                    str2 = "u8";
                    if (num != 0) {
                        BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                        str = "";
                        byte[] buffer3 = buffer;
                        int num8 = 0;
                        if (0 < buffer.Length) {
                            do {
                                byte num22 = buffer3[num8];
                                str = str + (Convert.ToString(num22) + " ");
                                num8++;
                            }
                            while (num8 < buffer3.Length);
                        }
                        char[] chArray7 = new char[] { ' ' };
                        str = str.TrimEnd(chArray7);
                        length = buffer.Length;
                    } else {
                        BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 1, ref buffer);
                        str = Convert.ToString(buffer[0]);
                    }
                    goto Label_0B13;

                case 4:
                    str2 = "s16";
                    if (num != 0) {
                        BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                        str = "";
                        int startIndex = 0;
                        if (0 < buffer.Length) {
                            do {
                                str = str + (Convert.ToString(IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, startIndex))) + " ");
                                startIndex += 2;
                            }
                            while (startIndex < buffer.Length);
                        }
                        char[] chArray = new char[] { ' ' };
                        str = str.TrimEnd(chArray);
                        length = buffer.Length / 2;
                    } else {
                        BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 2, ref buffer);
                        str = Convert.ToString(BitConverter.ToInt16(buffer, 0));
                    }
                    goto Label_0B13;

                case 5:
                    str2 = "u16";
                    if (num != 0) {
                        BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                        str = "";
                        int num9 = 0;
                        if (0 < buffer.Length) {
                            do {
                                str = str + (Convert.ToString((ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, num9))) + " ");
                                num9 += 2;
                            }
                            while (num9 < buffer.Length);
                        }
                        char[] chArray6 = new char[] { ' ' };
                        str = str.TrimEnd(chArray6);
                        length = buffer.Length / 2;
                    } else {
                        BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 2, ref buffer);
                        str = Convert.ToString(BitConverter.ToUInt16(buffer, 0));
                    }
                    goto Label_0B13;

                case 6:
                    str2 = "s32";
                    if (num != 0) {
                        BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                        str = "";
                        int num14 = 0;
                        if (0 < buffer.Length) {
                            do {
                                str = str + (Convert.ToString(IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, num14))) + " ");
                                num14 += 4;
                            }
                            while (num14 < buffer.Length);
                        }
                        char[] chArray5 = new char[] { ' ' };
                        str = str.TrimEnd(chArray5);
                        length = buffer.Length / 4;
                    } else {
                        BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 4, ref buffer);
                        str = Convert.ToString(BitConverter.ToInt32(buffer, 0));
                    }
                    goto Label_0B13;

                case 7:
                    str2 = "u32";
                    if (num != 0) {
                        BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                        str = "";
                        int num13 = 0;
                        if (0 < buffer.Length) {
                            do {
                                str = str + (Convert.ToString((uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, num13))) + " ");
                                num13 += 4;
                            }
                            while (num13 < buffer.Length);
                        }
                        char[] chArray4 = new char[] { ' ' };
                        str = str.TrimEnd(chArray4);
                        length = buffer.Length / 4;
                    } else {
                        BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 4, ref buffer);
                        str = Convert.ToString(BitConverter.ToUInt32(buffer, 0));
                    }
                    goto Label_0B13;

                case 8:
                    str2 = "s64";
                    if (num != 0) {
                        BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                        str = "";
                        int num12 = 0;
                        if (0 < buffer.Length) {
                            do {
                                str = str + (Convert.ToString(IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buffer, num12))) + " ");
                                num12 += 8;
                            }
                            while (num12 < buffer.Length);
                        }
                        char[] chArray3 = new char[] { ' ' };
                        str = str.TrimEnd(chArray3);
                        length = buffer.Length / 8;
                    } else {
                        BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 8, ref buffer);
                        str = Convert.ToString(BitConverter.ToInt64(buffer, 0));
                    }
                    goto Label_0B13;

                case 9:
                    str2 = "u64";
                    if (num != 0) {
                        BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                        str = "";
                        int num11 = 0;
                        if (0 < buffer.Length) {
                            do {
                                str = str + (Convert.ToString((ulong)IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buffer, num11))) + " ");
                                num11 += 8;
                            }
                            while (num11 < buffer.Length);
                        }
                        char[] chArray2 = new char[] { ' ' };
                        str = str.TrimEnd(chArray2);
                        length = buffer.Length / 8;
                    } else {
                        BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 8, ref buffer);
                        str = Convert.ToString(BitConverter.ToUInt64(buffer, 0));
                    }
                    goto Label_0B13;

                case 10:
                    str2 = "bin";
                    BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                    str3 = Convert.ToString(buffer.Length);
                    str = BitConverter.ToString(buffer).Replace("-", "");
                    goto Label_0B13;

                case 11:
                    str2 = "str";
                    BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                    str3 = Convert.ToString(buffer.Length);
                    for (int i = buffer.Length; i > 0; i = buffer.Length) {
                        if (buffer[i - 1] != 0) {
                            break;
                        }
                        Array.Resize<byte>(ref buffer, i - 1);
                    }
                    break;

                case 12:
                    str2 = "ip4";
                    BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 4, ref buffer);
                    str = new IPAddress(buffer).ToString();
                    goto Label_0B13;

                case 13:
                    str2 = "time";
                    BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 4, ref buffer);
                    str = string.Format("{0}", BitConverter.ToUInt32(buffer, 0));
                    goto Label_0B13;

                case 0x2e:
                    BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                    for (int j = buffer.Length; j > 0; j = buffer.Length) {
                        if (buffer[j - 1] != 0) {
                            break;
                        }
                        Array.Resize<byte>(ref buffer, j - 1);
                    }
                    str = Encoding.GetEncoding(m_encoding).GetString(buffer);
                    node.Attributes.Append(node.OwnerDocument.CreateAttribute(name));
                    node.Attributes[name].Value = str;
                    goto Label_0B13;

                case 0x34:
                    str2 = "bool";
                    if (num != 0) {
                        BinaryToXMLGetData(bin, ref datap, ref bytep, ref wordp, ref buffer);
                        str = "";
                        byte[] buffer4 = buffer;
                        int num15 = 0;
                        if (0 < buffer.Length) {
                            do {
                                byte num24 = buffer4[num15];
                                str = str + (Convert.ToString(num24) + " ");
                                num15++;
                            }
                            while (num15 < buffer4.Length);
                        }
                        char[] chArray8 = new char[] { ' ' };
                        str = str.TrimEnd(chArray8);
                        length = buffer.Length;
                    } else {
                        BinaryToXMLGetBytes(bin, ref datap, ref bytep, ref wordp, 1, ref buffer);
                        str = Convert.ToString(buffer[0]);
                    }
                    goto Label_0B13;

                case 190:
                    if (node.ParentNode != null) {
                        node = node.ParentNode;
                    }
                    goto Label_0B13;

                case 0xbf:
                    num21 = 1;
                    goto Label_0B13;

                default:
                    goto Label_0B13;
            }
            str = Encoding.GetEncoding(m_encoding).GetString(buffer);
            Label_0B13:
            if (str2 != "") {
                node = node.AppendChild(node.OwnerDocument.CreateElement(name));
                node.Attributes.Append(node.OwnerDocument.CreateAttribute("__type"));
                node.Attributes["__type"].Value = str2;
                if (str3 != "") {
                    node.Attributes.Append(node.OwnerDocument.CreateAttribute("__size"));
                    node.Attributes["__size"].Value = str3;
                }
                if (num == 0x40) {
                    node.Attributes.Append(node.OwnerDocument.CreateAttribute("__count"));
                    node.Attributes["__count"].Value = Convert.ToString(length);
                }
                node.InnerText = str;
            }
            if (nodep >= nodee) {
                return;
            }
            goto Label_0020;
        }
        static void BinaryToXMLGetBytes(byte[] bin, ref int datap, ref int bytep, ref int wordp, int size, ref byte[] @out) {
            int num;
            if ((bytep % 4) == 0) {
                bytep = datap;
            }
            if ((wordp % 4) == 0) {
                wordp = datap;
            }
            Array.Resize<byte>(ref @out, size);
            if ((size != 4) && (size != 8)) {
                if (size == 2) {
                    Array.Copy(bin, wordp, @out, 0, 2);
                    Array.Reverse(@out);
                    wordp += 2;
                } else {
                    Array.Copy(bin, bytep, @out, 0, size);
                    Array.Reverse(@out);
                    bytep += size;
                }
            } else {
                Array.Copy(bin, datap, @out, 0, size);
                Array.Reverse(@out);
                datap += size;
            }
            int num3 = bytep;
            int num2 = wordp;
            if (num3 > num2) {
                num = num3;
            } else {
                num = num2;
            }
            if (datap < num) {
                datap = (num + 3) & -4;
            }
        }
        static void BinaryToXMLGetData(byte[] bin, ref int datap, ref int bytep, ref int wordp, ref byte[] @out) {
            int newSize = 0;
            int startIndex = datap;
            if (startIndex <= (bin.Length - 4)) {
                newSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bin, startIndex));
            }
            Array.Resize<byte>(ref @out, newSize);
            Array.Copy(bin, datap + 4, @out, 0, newSize);
            datap = ((newSize + datap) + 7) & -4;
        }
        static void DecompressBits(byte[] bin, ref byte[] @out, ref int pt, int len, byte bits) {
            int index = 0;
            int num = 0;
            Array.Resize<byte>(ref @out, 0);
            if (0 < len) {
                int num5 = bits;
                do {
                    byte num4 = 0;
                    if (0 < num5) {
                        int num6 = pt;
                        uint num3 = (uint)num5;
                        do {
                            num4 = (byte)(((bin[(num / 8) + num6] >> ((byte)(7 - (num % 8)))) & 1) | ((byte)(num4 << 1)));
                            num++;
                            num3--;
                        }
                        while (num3 > 0);
                    }
                    Array.Resize<byte>(ref @out, index + 1);
                    @out[index] = num4;
                    index++;
                }
                while (index < len);
            }
            pt += (num + 7) / 8;
        }
        #endregion
    }
}
