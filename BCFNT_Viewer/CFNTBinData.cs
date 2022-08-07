using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace BCFNT_Viewer
{
    public class CFNTFormat
    {
        public enum FINFVersion
		{
            Version3 = 3, //BCFNT
            Version4 = 4 //BFFNT
		}

        public class CFNT
        {
            public char[] CFNTHeader { get; set; }
            public byte[] BOM { get; set; }
            public short HeaderSize { get; set; } //0x2
            public int Version { get; set; }
			public FINFVersion FINFVer => (FINFVersion)Version;
			public int FileSize { get; set; }
            public int BlockNumCount { get; set; }
            public FINF.Version3 FINF_v3 { get; set; }
            public FINF.Version4 FINF_v4 { get; set; }

            public void ReadCFNT(BinaryReader br)
			{
                CFNTHeader = br.ReadChars(4);
                if (new string(CFNTHeader) != "CFNT") throw new Exception("不明なフォーマットです");
                BOM = br.ReadBytes(2);

                EndianConvert endianConvert = new EndianConvert(BOM);
                HeaderSize = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                Version = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)).Reverse().ToArray(), 0);
                FileSize = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                BlockNumCount = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                if (FINFVer == FINFVersion.Version3) FINF_v3.ReadFINF_V3(br, BOM);
                if (FINFVer == FINFVersion.Version4) return; //FINF_Ver4
            }

            public void WriteCFNT(BinaryWriter bw)
            {
                bw.Write(CFNTHeader);
                bw.Write(BOM);

                EndianConvert endianConvert = new EndianConvert(BOM);
                bw.Write(endianConvert.Convert(BitConverter.GetBytes(HeaderSize)));
                bw.Write(endianConvert.Convert(BitConverter.GetBytes(Version)));
                bw.Write(endianConvert.Convert(BitConverter.GetBytes(FileSize)));
                bw.Write(endianConvert.Convert(BitConverter.GetBytes(BlockNumCount)));
                if (FINFVer == FINFVersion.Version3) FINF_v3.WriteFINF_V3(bw, BOM);
                if (FINFVer == FINFVersion.Version4) return; //FINF_Ver4
            }

            public CFNT(FINFVersion FINF_Version)
			{
                CFNTHeader = "CFNT".ToArray();
                BOM = new List<byte>().ToArray();
                HeaderSize = 0;
                Version = (int)FINF_Version;
                FileSize = 0;
                BlockNumCount = 0;
                if (FINF_Version == FINFVersion.Version3) FINF_v3 = new FINF.Version3();
                if (FINF_Version == FINFVersion.Version4) FINF_v4 = new FINF.Version4();
			}
        }

        public class FINF
        {
            /// <summary>
            /// BCFNT
            /// </summary>
            public class Version3
            {
                public char[] FINFHeader { get; set; } //0x4
                public int SectionSize { get; set; } //0x4
                public byte FontType { get; set; } //0x1
                public byte LineFeed { get; set; } //0x1
                public short AlterCharIndex { get; set; } //0x2
                public DefaultWidth DefaultWidth { get; set; } 
                public byte Encoding { get; set; }
                public int TGLP_Offset { get; set; } //r:-8, w:+8
                public TGLP.Version3 TGLP_Ver3 { get; set; }
                public int CWDH_Offset { get; set; } //r:-8, w:+8
                public CWDH CWDH { get; set; }
                public int CMAP_Offset { get; set; } //r:-8, w:+8
                public CMAP CMAP { get; set; }
                public byte Height { get; set; }
                public byte Width { get; set; }
                public byte Ascent { get; set; }
                public byte Reserved { get; set; }

                public void ReadFINF_V3(BinaryReader br, byte[] BOM)
				{
                    EndianConvert endianConvert = new EndianConvert(BOM);
                    FINFHeader = br.ReadChars(4);
                    if (new string(FINFHeader) != "FINF") throw new Exception("不明なフォーマットです");

                    SectionSize = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                    FontType = br.ReadByte();
                    LineFeed = br.ReadByte();
                    AlterCharIndex = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                    DefaultWidth.ReadDefWidth(br);
                    Encoding = br.ReadByte();
                    TGLP_Offset = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                    if (TGLP_Offset != 0)
					{
                        var Pos = br.BaseStream.Position;
                        br.BaseStream.Position = 0;
                        br.BaseStream.Seek(TGLP_Offset - 8, SeekOrigin.Current);
                        TGLP_Ver3.ReadTGLP_V3(br, BOM);
                        br.BaseStream.Position = Pos;
                    }
                    CWDH_Offset = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                    if (CWDH_Offset != 0)
					{
                        var Pos = br.BaseStream.Position;
                        br.BaseStream.Position = 0;
                        br.BaseStream.Seek(CWDH_Offset - 8, SeekOrigin.Current);
                        CWDH.ReadCWDH(br, BOM);
                        br.BaseStream.Position = Pos;
                    }
                    CMAP_Offset = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                    if (CMAP_Offset != 0)
                    {
                        var Pos = br.BaseStream.Position;
                        br.BaseStream.Position = 0;
                        br.BaseStream.Seek(CMAP_Offset - 8, SeekOrigin.Current);
                        CMAP.ReadCMAP(br, BOM);
                        br.BaseStream.Position = Pos;
                    }
                    Height = br.ReadByte();
                    Width = br.ReadByte();
                    Ascent = br.ReadByte();
                    Reserved = br.ReadByte();
                }

                public void WriteFINF_V3(BinaryWriter bw, byte[] BOM)
				{
                    EndianConvert endianConvert = new EndianConvert(BOM);
                    bw.Write(FINFHeader);
                    bw.Write(endianConvert.Convert(BitConverter.GetBytes(SectionSize)));
                    bw.Write(FontType);
                    bw.Write(LineFeed);
                    bw.Write(endianConvert.Convert(BitConverter.GetBytes(AlterCharIndex)));
                    DefaultWidth.WriteDefWidth(bw);
                    bw.Write(Encoding);

                    //TGLP, CWDH, CMAP

                    bw.Write(Height);
                    bw.Write(Width);
                    bw.Write(Ascent);
                    bw.Write(Reserved);
                }

                public Version3()
				{
                    FINFHeader = "FINF".ToArray();
                    SectionSize = 0;
                    FontType = 0x00;
                    LineFeed = 0x00;
                    AlterCharIndex = 0;
                    DefaultWidth = new DefaultWidth();
                    Encoding = 0;
                    TGLP_Offset = 0;
                    TGLP_Ver3 = new TGLP.Version3();
                    CWDH_Offset = 0;
                    CWDH = new CWDH();
                    CMAP_Offset = 0;
                    CMAP = new CMAP();
                    Height = 0;
                    Width = 0;
                    Ascent = 0;
                    Reserved = 0;
                }
            }

            /// <summary>
            /// BFFNT
            /// </summary>
            public class Version4
            {
                public char[] FINFHeader { get; set; }
                public byte[] SectionSize { get; set; }
                public byte FontType { get; set; }
                public byte Height { get; set; }
                public byte Width { get; set; }
                public byte Ascent { get; set; }
                public byte LineFeed { get; set; }
                public byte[] AlterCharIndex { get; set; }
                public DefaultWidth DefaultWidth { get; set; }
                public byte Encoding { get; set; }
                public byte[] TGLP_Offset { get; set; }
                public byte[] CMAP_Offset { get; set; }
                public byte[] CWDH_Offset { get; set; }
                public TGLP.Version4 TGLP_Ver4 { get; set; }
                public CWDH CWDH { get; set; }
                public List<CMAP> CMAP { get; set; }

                public Version4()
				{

				}
            }
        }

        public class DefaultWidth
        {
            public byte Left { get; set; }
            public byte GlyphWidth { get; set; }
            public byte CharWidth { get; set; }

            public int GetSize()
			{
                return 3;
			}

            public void ReadDefWidth(BinaryReader br)
			{
                Left = br.ReadByte();
                GlyphWidth = br.ReadByte();
                CharWidth = br.ReadByte();
			}

            public void WriteDefWidth(BinaryWriter bw)
			{
                bw.Write(Left);
                bw.Write(GlyphWidth);
                bw.Write(CharWidth);
			}

            public DefaultWidth()
			{
                Left = 0;
                GlyphWidth = 0;
                CharWidth = 0;
			}
        }

        public class TGLP
        {
            public class Version3
            {
                public char[] TGLPHeader { get; set; }
                public int SectionSize { get; set; }
                public byte CellWidth { get; set; }
                public byte CellHeight { get; set; }
                public byte Baseline_Position { get; set; }
                public byte MaxCharacterWidth { get; set; }
                public int SheetSize { get; set; }
                public short NumberOfSheets { get; set; }
                public short SheetImgFormat { get; set; }
                public short NumberOfColumns { get; set; }
                public short NumberOfRows { get; set; }
                public short SheetWidth { get; set; }
                public short SheetHeight { get; set; }
                public int SheetDataOffset { get; set; }

                //SectionSize - HeaderSize
                public List<TGLPImgData> TGLPImgDataList { get; set; }
                public class TGLPImgData
				{
                    public int Index { get; set; }

                    public short ImageWidth { get; set; }
                    public short ImageHeight { get; set; }
                    public short ImageFormat { get; set; }

                    public byte[] ImgData { get; set; }
                    public Bitmap SheetImg
                    {
                        get => Textures.ToBitmap(ImgData, ImageWidth, ImageHeight, (Textures.ImageFormat)ImageFormat);
                        set => ImgData = Textures.FromBitmap(value, (Textures.ImageFormat)ImageFormat);
                    }

                    //public byte[] ImgData
                    //{
                    //	get => Textures.FromBitmap(SheetImg, (Textures.ImageFormat)SheetImgFormat) ?? null;
                    //	set => SheetImg = Textures.ToBitmap(value, SheetWidth, SheetHeight, (Textures.ImageFormat)SheetImgFormat) ?? null;
                    //}
                    //public Bitmap SheetImg
                    //{
                    //	get => Textures.ToBitmap(ImgData, SheetWidth, SheetHeight, (Textures.ImageFormat)SheetImgFormat) ?? null;
                    //	set => ImgData = Textures.FromBitmap(value, (Textures.ImageFormat)SheetImgFormat) ?? null;
                    //}

                    public TGLPImgData(int Count, byte[] Input, short ImgWidth, short ImgHeight, short ImgFormat)
					{
                        Index = Count;

                        ImageWidth = ImgWidth;
                        ImageHeight = ImgHeight;
                        ImageFormat = ImgFormat;

                        ImgData = Input;
					}
				}

                public byte[] UnknownArea { get; set; }

                public int GetSize()
				{
                    int Sum = 0;
                    Sum += TGLPHeader.Length;
                    Sum += BitConverter.GetBytes(SectionSize).Length;
                    Sum += BitConverter.GetBytes(CellWidth).Length;
                    Sum += BitConverter.GetBytes(CellHeight).Length;
                    Sum += BitConverter.GetBytes(Baseline_Position).Length;
                    Sum += BitConverter.GetBytes(MaxCharacterWidth).Length;
                    Sum += BitConverter.GetBytes(SheetSize).Length;
                    Sum += BitConverter.GetBytes(NumberOfSheets).Length;
                    Sum += BitConverter.GetBytes(SheetImgFormat).Length;
                    Sum += BitConverter.GetBytes(NumberOfColumns).Length;
                    Sum += BitConverter.GetBytes(NumberOfRows).Length;
                    Sum += BitConverter.GetBytes(SheetWidth).Length;
                    Sum += BitConverter.GetBytes(SheetHeight).Length;
                    Sum += BitConverter.GetBytes(SheetDataOffset).Length;

                    //ImgData
                    for (int i = 0; i < NumberOfSheets; i++)
                    {
                        var ty = TGLPImgDataList[i].ImgData;
                        Sum += ty.Length;
                    }

                    Sum += UnknownArea.Length;
                    return Sum;
				}

                public void ReadTGLP_V3(BinaryReader br, byte[] BOM)
				{
                    EndianConvert endianConvert = new EndianConvert(BOM);
                    TGLPHeader = br.ReadChars(4);
                    SectionSize = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                    CellWidth = br.ReadByte();
                    CellHeight = br.ReadByte();
                    Baseline_Position = br.ReadByte();
                    MaxCharacterWidth = br.ReadByte();
                    SheetSize = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                    NumberOfSheets = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                    SheetImgFormat = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                    NumberOfColumns = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                    NumberOfRows = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                    SheetWidth = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                    SheetHeight = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                    SheetDataOffset = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                    if (SheetDataOffset != 0)
					{
                        var Pos = br.BaseStream.Position;
                        br.BaseStream.Position = 0;
                        br.BaseStream.Seek(SheetDataOffset, SeekOrigin.Current);

                        int AllSheetImgSize = SheetSize * NumberOfSheets;

						//List<Bitmap> CFNTFontImgList = new List<Bitmap>();
						//for (int i = 0; i < NumberOfSheets; i++)
						//{
						//    Bitmap bitmaps = Textures.ToBitmap(br.ReadBytes(SheetSize), SheetWidth, SheetHeight, (Textures.ImageFormat)SheetImgFormat);
						//    CFNTFontImgList.Add(bitmaps);
						//}


						for (int i = 0; i < NumberOfSheets; i++)
						{
                            //TGLPImgData tGLPImgData = new TGLPImgData(br.ReadBytes(SheetSize), i);
                            TGLPImgData tGLPImgData = new TGLPImgData(i, br.ReadBytes(SheetSize), SheetWidth, SheetHeight, SheetImgFormat);
                            TGLPImgDataList.Add(tGLPImgData);
						}

						//int s = BitConverter.ToInt16(version3_TGLP.NumberOfColumns, 0);
						//int s2 = BitConverter.ToInt16(version3_TGLP.NumberOfRows, 0);
						//int s3 = BitConverter.ToInt16(version3_TGLP.NumberOfSheets, 0);
						//var sw = (float)version3_TGLP.CellWidth;
						//var sw2 = (float)version3_TGLP.CellHeight;

						br.BaseStream.Position = Pos;
                    }

                    UnknownArea = br.ReadBytes(44);
                }

                public Version3()
				{
                    TGLPHeader = "TGLP".ToArray();
                    SectionSize = 0;
                    CellWidth = 0x00;
                    CellHeight = 0x00;
                    Baseline_Position = 0x00;
                    MaxCharacterWidth = 0x00;
                    SheetSize = 0;
                    NumberOfSheets = 0;
                    SheetImgFormat = 0;
                    NumberOfColumns = 0;
                    NumberOfRows = 0;
                    SheetWidth = 0;
                    SheetHeight = 0;
                    SheetDataOffset = 0;

                    TGLPImgDataList = new List<TGLPImgData>();
                    UnknownArea = new List<byte>().ToArray();
				}
            }

            public class Version4
            {
                public char[] TGLPHeader { get; set; }
                public byte[] SectionSize { get; set; }
                public byte CellWidth { get; set; }
                public byte CellHeight { get; set; }
                public byte[] NumberOfSheets { get; set; }
                public byte MaxCharacterWidth { get; set; }
                public byte[] SheetSize { get; set; }
                public byte Baseline_Position { get; set; }
                public byte[] SheetImgFormat { get; set; }
                public byte[] NumberOfSheetColumns { get; set; }
                public byte[] NumberOfSheetRows { get; set; }
                public byte[] SheetWidth { get; set; }
                public byte[] SheetHeight { get; set; }
                public byte[] SheetDataOffset { get; set; }

                //SectionSize - HeaderSize
                public byte[] ImgData { get; set; }
            }
        }

        public class CMAP
        {
            public enum MappingType
            {
                Direct = 0, //0x18
                Table = 1, //0x14
                Scan = 2 //0x14
            }

            public char[] CMAPHeader { get; set; }
            public int SectionSize { get; set; }
            public short CodeBegin { get; set; }
            public short CodeEnd { get; set; }
            public short MappingMethod { get; set; }
            public MappingType MappingTypes => (MappingType)MappingMethod;
            public short ReservedByte { get; set; }
            public int NextCMAPOffset { get; set; }
            public CMAP N_CMAP { get; set; }
            public int UnknownData { get; set; } //MappingType : 0 => Enable, MappingType : 1, 2 => Disable
            public List<byte[]> CharList { get; set; }

            public int GetSize()
			{
                int Sum = 0;
                Sum += CMAPHeader.Length;
                Sum += BitConverter.GetBytes(SectionSize).Length;
                Sum += BitConverter.GetBytes(CodeBegin).Length;
                Sum += BitConverter.GetBytes(CodeEnd).Length;
                Sum += BitConverter.GetBytes(MappingMethod).Length;
                Sum += BitConverter.GetBytes(ReservedByte).Length;
                Sum += BitConverter.GetBytes(NextCMAPOffset).Length;

                if (MappingTypes == MappingType.Direct)
				{
                    Sum += BitConverter.GetBytes(UnknownData).Length;
                }
                return Sum;
			}

            public void ReadCMAP(BinaryReader br, byte[] BOM)
			{
                EndianConvert endianConvert = new EndianConvert(BOM);
                CMAPHeader = br.ReadChars(4);
                if (new string(CMAPHeader) != "CMAP") throw new Exception("不明なフォーマットです");
                SectionSize = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                CodeBegin = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                CodeEnd = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                MappingMethod = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                ReservedByte = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                NextCMAPOffset = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                if (NextCMAPOffset != 0)
				{
                    var Pos = br.BaseStream.Position;
                    br.BaseStream.Position = 0;
                    br.BaseStream.Seek(NextCMAPOffset - 8, SeekOrigin.Current);
                    N_CMAP = new CMAP();
                    N_CMAP.ReadCMAP(br, BOM);
                    br.BaseStream.Position = Pos;
                }

                if (MappingTypes == MappingType.Direct)
				{
                    UnknownData = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                }

                //Get char item length
                int CharSetAreaLength = SectionSize - GetSize();
                int CharCount = CharSetAreaLength / 2;

                for (int CharsCount = 0; CharsCount < CharCount; CharsCount++)
                {
                    CharList.Add(br.ReadBytes(2));
                }
            }

            public CMAP()
			{
                CMAPHeader = "CMAP".ToArray();
                SectionSize = 0;
                CodeBegin = 0;
                CodeEnd = 0;
                MappingMethod = 0;
                ReservedByte = 0;
                NextCMAPOffset = 0;
                //N_CMAP = new CMAP();
                N_CMAP = null;
                UnknownData = 0;
                CharList = new List<byte[]>();
			}
        }

        public class CWDH
        {
            public char[] CWDHHeader { get; set; }
            public int SectionSize { get; set; }
            public short StartIndex { get; set; }
            public short EndIndex { get; set; }
            public int NextCWDHOffset { get; set; }
            public CWDH N_CWDH { get; set; }
            public List<CharWidth> CharWidth_List { get; set; }

            public void ReadCWDH(BinaryReader br, byte[] BOM)
			{
                EndianConvert endianConvert = new EndianConvert(BOM);
                CWDHHeader = br.ReadChars(4);
                if (new string(CWDHHeader) != "CWDH") throw new Exception("不明なフォーマットです");
                SectionSize = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
                StartIndex = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                EndIndex = BitConverter.ToInt16(endianConvert.Convert(br.ReadBytes(2)), 0);
                NextCWDHOffset = BitConverter.ToInt32(endianConvert.Convert(br.ReadBytes(4)), 0);
				if (NextCWDHOffset != 0)
				{
					var Pos = br.BaseStream.Position;
					br.BaseStream.Position = 0;
					br.BaseStream.Seek(NextCWDHOffset - 8, SeekOrigin.Current);
                    N_CWDH = new CWDH();
					N_CWDH.ReadCWDH(br, BOM);
					br.BaseStream.Position = Pos;
				}

				for (int Count = 0; Count < EndIndex; Count++)
                {
                    CharWidth charWidth = new CharWidth();
                    charWidth.ReadCharWidth(br, Count);
                    CharWidth_List.Add(charWidth);
                }
            }

            public void WriteCWDH(BinaryReader br, byte[] BOM)
			{

			}

            public CWDH()
			{
                CWDHHeader = "CWDH".ToArray();
                SectionSize = 0;
                StartIndex = 0;
                EndIndex = 0;
                NextCWDHOffset = 0;
                //N_CWDH = new CWDH();
                N_CWDH = null;
                CharWidth_List = new List<CharWidth>();
			}
        }

        public class CharWidth
        {
            public int GlyphNumber { get; set; }
            public byte Left { get; set; }
            public byte Glyph_Width { get; set; }
            public byte Char_Width { get; set; }

            public void ReadCharWidth(BinaryReader br, int Count)
			{
                GlyphNumber = Count;
                Left = br.ReadByte();
                Glyph_Width = br.ReadByte();
                Char_Width = br.ReadByte();
			}

            public void WriteCharWidth(BinaryWriter bw)
			{
                bw.Write(Left);
                bw.Write(Glyph_Width);
                bw.Write(Char_Width);
			}

            public CharWidth(int Idx = 0, int left = 0, int GlyphWidth = 0, int CharWidth = 0)
			{
                GlyphNumber = Idx;
                Left = (byte)left;
                Glyph_Width = (byte)GlyphWidth;
                Char_Width = (byte)CharWidth;
			}
        }
    }
}
