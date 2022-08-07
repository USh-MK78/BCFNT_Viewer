using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCFNT_Viewer
{
    [TypeConverter(typeof(CustomSortTypeConverter))]
    public class CFNT_PropertyGridSetting
    {
        public char[] CFNTHeader { get; set; }
        public byte[] BOM { get; set; }
        public EndianConvert.Endian Endian => new EndianConvert(BOM).EndianCheck();
        public short HeaderSize { get; set; }
        public int Version { get; set; }
        public CFNTFormat.FINFVersion FINF_Version => (CFNTFormat.FINFVersion)Version;
        public int FileSize { get; set; }
        public int BlockNumCount { get; set; }

        [Category("FINF_v3")]
        [TypeConverter(typeof(CustomExpandableObjectSortTypeConverter))]
        public Version3_FINF_PGS Version3_FINF { get; set; }
        public class Version3_FINF_PGS
        {
            public char[] FINFHeader { get; set; }
            public int SectionSize { get; set; }
            public byte FontType { get; set; }
            public byte LineFeed { get; set; }
            public short AlterCharIndex { get; set; }

            [Category("FINF_v3_DefaultWidth")]
            [TypeConverter(typeof(CustomExpandableObjectSortTypeConverter))]
            public DefaultWidth Default_Width { get; set; }
            public class DefaultWidth
            {
                public int Left { get; set; }
                public int GlyphWidth { get; set; }
                public int CharWidth { get; set; }

                public DefaultWidth(CFNTFormat.DefaultWidth defaultWidth)
				{
                    Left = defaultWidth.Left;
                    GlyphWidth = defaultWidth.GlyphWidth;
                    CharWidth = defaultWidth.CharWidth;
				}

                public override string ToString()
                {
                    return "DefaultWidth";
                }
            }

            public byte Encoding { get; set; }
            public int TGLP_Offset { get; set; } //r:-8, w:+8

            [Category("TGLP_v3")]
            [TypeConverter(typeof(CustomExpandableObjectSortTypeConverter))]
            public Version3_TGLP TGLP_Ver3 { get; set; }
            public class Version3_TGLP
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
                public Textures.ImageFormat ImageFormat => (Textures.ImageFormat)SheetImgFormat;
                public short NumberOfColumns { get; set; }
                public short NumberOfRows { get; set; }
                public short SheetWidth { get; set; }
                public short SheetHeight { get; set; }
                //public int SheetDataOffset { get; set; }
                public List<Bitmap> TGLPImageList { get; set; }
                public byte[] UnknownArea { get; set; }

                public Version3_TGLP(CFNTFormat.TGLP.Version3 version3)
				{
                    TGLPHeader = version3.TGLPHeader;
                    SectionSize = version3.SectionSize;
                    CellWidth = version3.CellWidth;
                    CellHeight = version3.CellHeight;
                    Baseline_Position = version3.Baseline_Position;
                    MaxCharacterWidth = version3.MaxCharacterWidth;
                    SheetSize = version3.SheetSize;
                    NumberOfSheets = version3.NumberOfSheets;
                    SheetImgFormat = version3.SheetImgFormat;
                    NumberOfColumns = version3.NumberOfColumns;
                    NumberOfRows = version3.NumberOfRows;
                    SheetWidth = version3.SheetWidth;
                    SheetHeight = version3.SheetHeight;

                    TGLPImageList = version3.TGLPImgDataList.Select(x => x.SheetImg).ToList();
                    UnknownArea = version3.UnknownArea;
				}

                public override string ToString()
                {
                    return "TGLP_v3";
                }
            }

            public int CWDH_Offset { get; set; } //r:-8, w:+8

            [Category("CWDH_Data")]
            [TypeConverter(typeof(CustomExpandableObjectSortTypeConverter))]
            public CWDH CWDHSection { get; set; }
            public class CWDH
            {
                public char[] CWDHHeader { get; set; }
                public int SectionSize { get; set; }
                public short StartIndex { get; set; }
                public short EndIndex { get; set; }
                public int NextCWDHOffset { get; set; }

                [Category("N_CWDH_Data")]
                [TypeConverter(typeof(CustomExpandableObjectSortTypeConverter))]
                public CWDH N_CWDH { get; set; }

                public List<CharWidth> CharWidth_List { get; set; }
                public class CharWidth
                {
                    public int GlyphNumber { get; set; }
                    public byte Left { get; set; }
                    public byte Glyph_Width { get; set; }
                    public byte Char_Width { get; set; }

                    public CharWidth(CFNTFormat.CharWidth charWidth)
					{
                        GlyphNumber = charWidth.GlyphNumber;
                        Left = charWidth.Left;
                        Glyph_Width = charWidth.Glyph_Width;
                        Char_Width = charWidth.Char_Width;
					}

                    public override string ToString()
                    {
                        return "CharWidth " + GlyphNumber;
                    }
                }

                public CWDH(CFNTFormat.CWDH CWDHData)
				{
                    CWDHHeader = CWDHData.CWDHHeader;
                    SectionSize = CWDHData.SectionSize;
                    StartIndex = CWDHData.StartIndex;
                    EndIndex = CWDHData.EndIndex;
                    NextCWDHOffset = CWDHData.NextCWDHOffset;
                    if (CWDHData.NextCWDHOffset != 0 && CWDHData.N_CWDH != null)
					{
                        N_CWDH = new CWDH(CWDHData.N_CWDH);
					}
                    //CharWidth_List = CWDHData.CharWidth_List;
                    CharWidth_List = new List<CharWidth>();

                    for (int i = 0; i < CWDHData.CharWidth_List.Count; i++)
					{
                        CharWidth_List.Add(new CharWidth(CWDHData.CharWidth_List[i]));
					}

				}

                public override string ToString()
                {
                    return "CWDH";
                }
            }

            public int CMAP_Offset { get; set; } //r:-8, w:+8

            [Category("CMAP_Data")]
            [TypeConverter(typeof(CustomExpandableObjectSortTypeConverter))]
            public CMAP CMAPSection { get; set; }
            public class CMAP
			{
                public char[] CMAPHeader { get; set; }
                public int SectionSize { get; set; }
                public short CodeBegin { get; set; }
                public short CodeEnd { get; set; }
                public short MappingMethod { get; set; }
                public CFNTFormat.CMAP.MappingType MappingTypes => (CFNTFormat.CMAP.MappingType)MappingMethod;
                public short ReservedByte { get; set; }
                public int NextCMAPOffset { get; set; }

                [Category("N_CMAP_Data")]
                [TypeConverter(typeof(CustomExpandableObjectSortTypeConverter))]
                public CMAP N_CMAP { get; set; }

                public int UnknownData { get; set; } //MappingType : 0 => Enable, MappingType : 1, 2 => Disable
                public List<byte[]> CharList { get; set; }

                public CMAP(CFNTFormat.CMAP CMAPData)
				{
                    CMAPHeader = CMAPData.CMAPHeader;
                    SectionSize = CMAPData.SectionSize;
                    CodeBegin = CMAPData.CodeBegin;
                    CodeEnd = CMAPData.CodeEnd;
                    MappingMethod = CMAPData.MappingMethod;
                    ReservedByte = CMAPData.ReservedByte;
                    NextCMAPOffset = CMAPData.NextCMAPOffset;
                    if (NextCMAPOffset != 0 && CMAPData.N_CMAP != null)
					{
                        N_CMAP = new CMAP(CMAPData.N_CMAP);
					}
                    UnknownData = CMAPData.UnknownData;
                    CharList = CMAPData.CharList;
				}

				public override string ToString()
				{
					return "CMAP";
				}
			}

            public byte Height { get; set; }
            public byte Width { get; set; }
            public byte Ascent { get; set; }
            public byte Reserved { get; set; }

            public Version3_FINF_PGS(CFNTFormat.FINF.Version3 FINFData)
			{
                FINFHeader = FINFData.FINFHeader;
                SectionSize = FINFData.SectionSize;
                FontType = FINFData.FontType;
                LineFeed = FINFData.LineFeed;
                AlterCharIndex = FINFData.AlterCharIndex;
                Default_Width = new DefaultWidth(FINFData.DefaultWidth);
                Encoding = FINFData.Encoding;
                TGLP_Offset = FINFData.TGLP_Offset;
                TGLP_Ver3 = new Version3_TGLP(FINFData.TGLP_Ver3);
                CWDH_Offset = FINFData.CWDH_Offset;
                CWDHSection = new CWDH(FINFData.CWDH);
                CMAP_Offset = FINFData.CMAP_Offset;
                CMAPSection = new CMAP(FINFData.CMAP);
                Height = FINFData.Height;
                Width = FINFData.Width;
                Ascent = FINFData.Ascent;
                Reserved = FINFData.Reserved;
			}

            public override string ToString()
            {
                return "FINF_v3";
            }
        }

		#region Backup
		//[Category("FINF_v4")]
		//[TypeConverter(typeof(ExpandableObjectConverter))]
		//public Version4_FINF_PGS Version4_FINF { get; set; } = new Version4_FINF_PGS();
		//public class Version4_FINF_PGS
		//{
		//    public int FontType { get; set; }
		//    public int Height { get; set; }
		//    public int Width { get; set; }
		//    public byte Ascent { get; set; }
		//    public int LineFeed { get; set; }
		//    public int AlterCharIndex { get; set; }

		//    [Category("FINF_v4_DefaultWidth")]
		//    [TypeConverter(typeof(ExpandableObjectConverter))]
		//    public DefaultWidth Default_Width { get; set; } = new DefaultWidth();
		//    public class DefaultWidth
		//    {
		//        public int Left { get; set; }
		//        public int GlyphWidth { get; set; }
		//        public int CharWidth { get; set; }

		//        public override string ToString()
		//        {
		//            return "DefaultWidth";
		//        }
		//    }

		//    public byte Encoding { get; set; }

		//    [Category("TGLP_v4")]
		//    [TypeConverter(typeof(ExpandableObjectConverter))]
		//    public Version4_TGLP TGLP_Ver4 { get; set; } = new Version4_TGLP();
		//    public class Version4_TGLP
		//    {
		//        public int CellWidth { get; set; }
		//        public int CellHeight { get; set; }
		//        public int NumberOfSheets { get; set; }
		//        public int MaxCharacterWidth { get; set; }
		//        public int Baseline_Position { get; set; }
		//        public int SheetImgFormat { get; set; }
		//        public int NumberOfSheetColumns { get; set; }
		//        public int NumberOfSheetRows { get; set; }
		//        public int SheetWidth { get; set; }
		//        public int SheetHeight { get; set; }

		//        public override string ToString()
		//        {
		//            return "TGLP_v4";
		//        }
		//    }

		//    public override string ToString()
		//    {
		//        return "FINF_v4";
		//    }
		//}
		#endregion

        public CFNT_PropertyGridSetting(CFNTFormat.CFNT CFNTData)
		{
            CFNTHeader = CFNTData.CFNTHeader;
            BOM = CFNTData.BOM;
            HeaderSize = CFNTData.HeaderSize;
            Version = CFNTData.Version;
            FileSize = CFNTData.FileSize;
            BlockNumCount = CFNTData.BlockNumCount;
            Version3_FINF = new Version3_FINF_PGS(CFNTData.FINF_v3);
            //Ver_4
		}

		public override string ToString()
		{
			return "CFNT";
		}
	}

    public class CustomSortTypeConverter : TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection PDC = TypeDescriptor.GetProperties(value, attributes);

            Type type = value.GetType();

            List<string> list = type.GetProperties().Select(x => x.Name).ToList();

            return PDC.Sort(list.ToArray());
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class CustomExpandableObjectSortTypeConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection PDC = TypeDescriptor.GetProperties(value, attributes);

            Type type = value.GetType();

            List<string> list = type.GetProperties().Select(x => x.Name).ToList();

            return PDC.Sort(list.ToArray());
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
