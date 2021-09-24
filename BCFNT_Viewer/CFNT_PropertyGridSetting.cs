using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCFNT_Viewer
{
    public class CFNT_PropertyGridSetting
    {
        public byte[] BOM { get; set; }
        public int Version { get; set; }

        [Category("FINF_v3")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Version3_FINF_PGS Version3_FINF { get; set; } = new Version3_FINF_PGS();
        public class Version3_FINF_PGS
        {
            public int FontType { get; set; }
            public int LineFeed { get; set; }
            public int AlterCharIndex { get; set; }

            [Category("FINF_v3_DefaultWidth")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public DefaultWidth Default_Width { get; set; } = new DefaultWidth();
            public class DefaultWidth
            {
                public int Left { get; set; }
                public int GlyphWidth { get; set; }
                public int CharWidth { get; set; }

                public override string ToString()
                {
                    return "DefaultWidth";
                }
            }

            public byte Encoding { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
            public byte Ascent { get; set; }

            [Category("TGLP_v3")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public Version3_TGLP TGLP_Ver3 { get; set; } = new Version3_TGLP();
            public class Version3_TGLP
            {
                public int CellWidth { get; set; }
                public int CellHeight { get; set; }
                public int Baseline_Position { get; set; }
                public int MaxCharacterWidth { get; set; }
                public int NumberOfSheets { get; set; }
                public int SheetImgFormat { get; set; }
                public int NumberOfColumns { get; set; }
                public int NumberOfRows { get; set; }
                public int SheetWidth { get; set; }
                public int SheetHeight { get; set; }

                public override string ToString()
                {
                    return "TGLP_v3";
                }
            }

            public override string ToString()
            {
                return "FINF_v3";
            }
        }

        [Category("FINF_v4")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Version4_FINF_PGS Version4_FINF { get; set; } = new Version4_FINF_PGS();
        public class Version4_FINF_PGS
        {
            public int FontType { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
            public byte Ascent { get; set; }
            public int LineFeed { get; set; }
            public int AlterCharIndex { get; set; }

            [Category("FINF_v4_DefaultWidth")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public DefaultWidth Default_Width { get; set; } = new DefaultWidth();
            public class DefaultWidth
            {
                public int Left { get; set; }
                public int GlyphWidth { get; set; }
                public int CharWidth { get; set; }

                public override string ToString()
                {
                    return "DefaultWidth";
                }
            }

            public byte Encoding { get; set; }

            [Category("TGLP_v4")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public Version4_TGLP TGLP_Ver4 { get; set; } = new Version4_TGLP();
            public class Version4_TGLP
            {
                public int CellWidth { get; set; }
                public int CellHeight { get; set; }
                public int NumberOfSheets { get; set; }
                public int MaxCharacterWidth { get; set; }
                public int Baseline_Position { get; set; }
                public int SheetImgFormat { get; set; }
                public int NumberOfSheetColumns { get; set; }
                public int NumberOfSheetRows { get; set; }
                public int SheetWidth { get; set; }
                public int SheetHeight { get; set; }

                public override string ToString()
                {
                    return "TGLP_v4";
                }
            }

            public override string ToString()
            {
                return "FINF_v4";
            }
        }


        [Category("CWDH_Data")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CWDH CWDHData { get; set; } = new CWDH();
        public class CWDH
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public List<CharWidth> CharWidth_List { get; set; }
            public class CharWidth
            {
                public int GlyphNumber { get; set; }
                public byte Left { get; set; }
                public byte Glyph_Width { get; set; }
                public byte Char_Width { get; set; }

                public override string ToString()
                {
                    return "CharWidth" + GlyphNumber;
                }
            }

            public override string ToString()
            {
                return "CWDH";
            }
        }

        [Category("CMAPList")]
        public List<CMAP> CMAP_List = new List<CMAP>();
        public List<CMAP> CMAPList { get => CMAP_List; set => CMAP_List = value; }
        public class CMAP
        {
            public byte[] CodeBegin { get; set; }
            public byte[] CodeEnd { get; set; }

            //0 = Direct, 1 = Table, 2 = Scan
            public int MappingMethod { get; set; }
            public List<string> CharList { get; set; }
        }






        //public List<ClipValues> ClipValue_List = new List<ClipValues>();
        //public List<ClipValues> ClipValueList { get => ClipValue_List; set => ClipValue_List = value; }
        //public class ClipValues
        //{
        //    public int ID { get; set; }
        //    public int Value { get; set; }

        //    public override string ToString()
        //    {
        //        return "ClipValue" + ID;
        //    }
        //}

    }
}
