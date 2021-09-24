using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCFNT_Viewer
{
    class CFNTBinData
    {
        [Serializable()]
        public class CFNT
        {
            public char[] CFNTHeader { get; set; }
            public byte[] BOM { get; set; }
            public byte[] HeaderSize { get; set; }
            public byte[] Version { get; set; }
            public byte[] FileSize { get; set; }
            public byte[] BlockNumCount { get; set; }
            public FINF.Version3 FINF_v3 { get; set; }
            public FINF.Version4 FINF_v4 { get; set; }
        }

        [Serializable()]
        public class FINF
        {
            /// <summary>
            /// BCFNT
            /// </summary>
            [Serializable()]
            public class Version3
            {
                public char[] FINFHeader { get; set; }
                public byte[] SectionSize { get; set; }
                public byte FontType { get; set; }
                public byte LineFeed { get; set; }
                public byte[] AlterCharIndex { get; set; }
                public DefaultWidth DefaultWidth { get; set; }
                public byte Encoding { get; set; }
                public byte[] TGLP_Offset { get; set; }
                public byte[] CMAP_Offset { get; set; }
                public byte[] CWDH_Offset { get; set; }
                public byte Height { get; set; }
                public byte Width { get; set; }
                public byte Ascent { get; set; }
                public byte Reserved { get; set; }
                public TGLP.Version3 TGLP_Ver3 { get; set; }
                public CWDH CWDH { get; set; }
                public List<CMAP> CMAP { get; set; }
            }

            /// <summary>
            /// BFFNT
            /// </summary>
            [Serializable()]
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
            }
        }

        [Serializable()]
        public class DefaultWidth
        {
            public byte Left { get; set; }
            public byte GlyphWidth { get; set; }
            public byte CharWidth { get; set; }
        }

        public class TGLP
        {
            public class Version3
            {
                public char[] TGLPHeader { get; set; }
                public byte[] SectionSize { get; set; }
                public byte CellWidth { get; set; }
                public byte CellHeight { get; set; }
                public byte Baseline_Position { get; set; }
                public byte MaxCharacterWidth { get; set; }
                public byte[] SheetSize { get; set; }
                public byte[] NumberOfSheets { get; set; }
                public byte[] SheetImgFormat { get; set; }
                public byte[] NumberOfColumns { get; set; }
                public byte[] NumberOfRows { get; set; }
                public byte[] SheetWidth { get; set; }
                public byte[] SheetHeight { get; set; }
                public byte[] SheetDataOffset { get; set; }

                //SectionSize - HeaderSize
                public byte[] ImgData { get; set; }
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
            public char[] CMAPHeader { get; set; }
            public byte[] SectionSize { get; set; }
            public byte[] CodeBegin { get; set; }
            public byte[] CodeEnd { get; set; }

            //0 = Direct, 1 = Table, 2 = Scan
            public byte[] MappingMethod { get; set; }
            public byte[] ReservedByte { get; set; }
            public byte[] NextCMAPOffset { get; set; }
            public List<byte[]> CharList { get; set; }
        }

        public class CWDH
        {
            public char[] CWDHHeader { get; set; }
            public byte[] SectionSize { get; set; }
            public byte[] StartIndex { get; set; }
            public byte[] EndIndex { get; set; }
            public byte[] NextCWDHOffset { get; set; }
            public List<CharWidth> CharWidth_List { get; set; }
        }

        public class CharWidth
        {
            public int GlyphNumber { get; set; }
            public byte Left { get; set; }
            public byte Glyph_Width { get; set; }
            public byte Char_Width { get; set; }
        }
    }
}
