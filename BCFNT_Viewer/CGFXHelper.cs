using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCFNT_Viewer
{
    public class EndianConvert
    {
        public enum Endian
        {
            BigEndian = 65534,
            LittleEndian = 65279
        }

        public byte[] BOM { get; set; }
        public Endian Endians => EndianCheck();

        public EndianConvert(byte[] InputBOM)
        {
            BOM = InputBOM;
        }

        public Endian EndianCheck()
        {
            bool LE = BOM.SequenceEqual(new byte[] { 0xFF, 0xFE });
            bool BE = BOM.SequenceEqual(new byte[] { 0xFE, 0xFF });

            Endian BOMSetting = Endian.BigEndian;

            if ((LE || BE) == true)
            {
                if (LE == true) BOMSetting = Endian.LittleEndian;
                if (BE == true) BOMSetting = Endian.BigEndian;
            }

            return BOMSetting;
        }

        public byte[] Convert(byte[] Input)
        {
            if (Endians == Endian.BigEndian)
            {
                return Input.Reverse().ToArray();
            }
            if (Endians == Endian.LittleEndian)
            {
                return Input;
            }

            return Input;
        }
    }

    public class ReadByteLine
    {
        public List<byte> charByteList { get; set; }

        public ReadByteLine(List<byte> Input)
        {
            charByteList = Input;
        }

        public ReadByteLine(List<char> Input)
        {
            charByteList = Input.Select(x => (byte)x).ToArray().ToList();
        }

        public void ReadByte(BinaryReader br, byte Split = 0x00)
        {
            //var br = br.BaseStream;
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                byte PickStr = br.ReadByte();
                charByteList.Add(PickStr);
                if (PickStr == Split)
                {
                    break;
                }
            }
        }

        public void ReadMultiByte(BinaryReader br, byte Split = 0x00)
        {
            //var br = br.BaseStream;
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                byte[] PickStr = br.ReadBytes(2);
                charByteList.Add(PickStr[0]);
                charByteList.Add(PickStr[1]);
                if (PickStr[0] == Split)
                {
                    break;
                }
            }
        }

        public void ReadByte(BinaryReader br, char Split = '\0')
        {
            //var br = br.BaseStream;
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                byte PickStr = br.ReadByte();
                charByteList.Add(PickStr);
                if (PickStr == Split)
                {
                    break;
                }
            }
        }

        public void WriteByte(BinaryWriter bw)
        {
            bw.Write(ConvertToCharArray());
        }

        public char[] ConvertToCharArray()
        {
            return charByteList.Select(x => (char)x).ToArray();
        }

        public int GetLength()
        {
            return charByteList.ToArray().Length;
        }
    }

    public class TaskHelper
    {
        public static Task<T> RunTask<T>(object obj)
        {
            Task<T> r = Task.Run(() => { return (T)obj; });
            return r;
        }

        public static Task<T> RunTask<T>(BinaryReader br, int Offset, object obj)
        {
            Task<T> r = Task.Run(() =>
            {
                long Pos = br.BaseStream.Position;

                br.BaseStream.Seek(-4, SeekOrigin.Current);

                //Move DataOffset
                br.BaseStream.Seek(Offset, SeekOrigin.Current);

                var output = (T)obj;

                br.BaseStream.Position = Pos;


                return output;
            });

            return r;
        }
    }

    public class Converter
    {
        public static bool ByteToBoolean(byte Input)
        {
            bool b = new bool();
            if (Input == 0) b = false;
            if (Input == 1) b = true;
            return b;
        }

        public static byte BooleanToByte(bool Input)
        {
            return Convert.ToByte(Input);
        }

        public enum ConvertType
        {
            Boolean,
            Char,
            Double,
            Int16,
            Int32,
            Int64,
            Single,
            UInt16,
            UInt32,
            UInt64
        }

        public static T CustomBitConverter<T>(byte[] byteAry, int startIndex, ConvertType convertType)
        {
            object obj = new object();
            if (convertType == ConvertType.Boolean) obj = BitConverter.ToBoolean(byteAry, startIndex);
            if (convertType == ConvertType.Char) obj = BitConverter.ToChar(byteAry, startIndex);
            if (convertType == ConvertType.Double) obj = BitConverter.ToDouble(byteAry, startIndex);
            if (convertType == ConvertType.Int16) obj = BitConverter.ToInt16(byteAry, startIndex);
            if (convertType == ConvertType.Int32) obj = BitConverter.ToInt32(byteAry, startIndex);
            if (convertType == ConvertType.Int64) obj = BitConverter.ToInt64(byteAry, startIndex);
            if (convertType == ConvertType.Single) obj = BitConverter.ToSingle(byteAry, startIndex);
            if (convertType == ConvertType.UInt16) obj = BitConverter.ToUInt16(byteAry, startIndex);
            if (convertType == ConvertType.UInt32) obj = BitConverter.ToUInt32(byteAry, startIndex);
            if (convertType == ConvertType.UInt64) obj = BitConverter.ToUInt64(byteAry, startIndex);
            return (T)obj;
        }

        public static T CustomBitConverter<T>(byte[] byteAry, int startIndex)
        {
            object obj = new object();
            if (typeof(T) == typeof(bool)) obj = BitConverter.ToBoolean(byteAry, startIndex);
            if (typeof(T) == typeof(char)) obj = BitConverter.ToChar(byteAry, startIndex);
            if (typeof(T) == typeof(double)) obj = BitConverter.ToDouble(byteAry, startIndex);
            if (typeof(T) == typeof(short)) obj = BitConverter.ToInt16(byteAry, startIndex);
            if (typeof(T) == typeof(int)) obj = BitConverter.ToInt32(byteAry, startIndex);
            if (typeof(T) == typeof(long)) obj = BitConverter.ToInt64(byteAry, startIndex);
            if (typeof(T) == typeof(float)) obj = BitConverter.ToSingle(byteAry, startIndex);
            if (typeof(T) == typeof(ushort)) obj = BitConverter.ToUInt16(byteAry, startIndex);
            if (typeof(T) == typeof(uint)) obj = BitConverter.ToUInt32(byteAry, startIndex);
            if (typeof(T) == typeof(ulong)) obj = BitConverter.ToUInt64(byteAry, startIndex);
            return (T)obj;
        }
    }

    public class BinaryReadHelper
    {
        public byte[] BOMs;
        public BinaryReader BR { get; set; }
        public T[] ReadArray<T>(int Count, int ByteLength)
        {
            T[] Ary = new T[Count];
            for (int i = 0; i < Count; i++) Ary[i] = Converter.CustomBitConverter<T>(BR.ReadBytes(ByteLength), 0);
            return Ary;
        }

        public byte[] ReadArray(int Count)
        {
            byte[] Ary = new byte[Count];
            for (int i = 0; i < Count; i++) Ary[i] = BR.ReadByte();
            return Ary;
        }

        public BinaryReadHelper(BinaryReader br, byte[] BOM)
        {
            BOMs = BOM;
            BR = br;
        }
    }
}
