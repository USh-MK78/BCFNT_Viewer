using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCFNT_Viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //エンディアン
        enum Endian
        {
            Little,
            big
        }
        //byte配列の反転
        static byte[] RevEndian(byte[] bytes, Endian endian)
        {
            if (endian == Endian.Little)
            {
                return bytes.Reverse().ToArray();
            }
            else
            {
                return bytes;
            }
        }


        private void OpenBCFNT_TSM_Click(object sender, EventArgs e)
        {
            //ファイルを開く
            OpenFileDialog Open_BCFNT = new OpenFileDialog()
            {
                Title = "Open(BCFNT)",
                InitialDirectory = @"C:\Users\User\Desktop",
                Filter = "bcfnt file|*.bcfnt"
            };

            if (Open_BCFNT.ShowDialog() != DialogResult.OK) return;

            System.IO.FileStream fs1 = new FileStream(Open_BCFNT.FileName, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new BinaryReader(fs1);

            CFNTBinData.CFNT CFNT_Section = new CFNTBinData.CFNT
            {
                CFNTHeader = br1.ReadChars(4),
                BOM = br1.ReadBytes(2),
                HeaderSize = br1.ReadBytes(2),
                Version = br1.ReadBytes(4),
                FileSize = br1.ReadBytes(4),
                BlockNumCount = br1.ReadBytes(4),
                FINF_v3 = new CFNTBinData.FINF.Version3
                {
                    FINFHeader = br1.ReadChars(4),
                    SectionSize = br1.ReadBytes(4),
                    FontType = br1.ReadByte(),
                    LineFeed = br1.ReadByte(),
                    AlterCharIndex = br1.ReadBytes(2),
                    DefaultWidth = new CFNTBinData.DefaultWidth
                    {
                        Left = br1.ReadByte(),
                        GlyphWidth = br1.ReadByte(),
                        CharWidth = br1.ReadByte()
                    },
                    Encoding = br1.ReadByte(),
                    TGLP_Offset = br1.ReadBytes(4),
                    CWDH_Offset = br1.ReadBytes(4),
                    CMAP_Offset = br1.ReadBytes(4),
                    Height = br1.ReadByte(),
                    Width = br1.ReadByte(),
                    Ascent = br1.ReadByte(),
                    Reserved = br1.ReadByte(),
                    TGLP_Ver3 = null,
                    CWDH = null,
                    CMAP = null
                },
                FINF_v4 = new CFNTBinData.FINF.Version4
                {

                }
            };

            if (new string(CFNT_Section.CFNTHeader) != "CFNT") throw new Exception("不明なフォーマットです");
            if (new string(CFNT_Section.FINF_v3.FINFHeader) != "FINF") throw new Exception("不明なフォーマットです(FINF_Version3)");

            int Version = BitConverter.ToInt32(RevEndian(CFNT_Section.Version, Endian.Little), 0);

            if (Version == 3)
            {
                CFNTBinData.TGLP.Version3 version3_TGLP = new CFNTBinData.TGLP.Version3
                {
                    TGLPHeader = br1.ReadChars(4),
                    SectionSize = br1.ReadBytes(4),
                    CellWidth = br1.ReadByte(),
                    CellHeight = br1.ReadByte(),
                    Baseline_Position = br1.ReadByte(),
                    MaxCharacterWidth = br1.ReadByte(),
                    SheetSize = br1.ReadBytes(4),
                    NumberOfSheets = br1.ReadBytes(2),
                    SheetImgFormat = br1.ReadBytes(2),
                    NumberOfColumns = br1.ReadBytes(2),
                    NumberOfRows = br1.ReadBytes(2),
                    SheetWidth = br1.ReadBytes(2),
                    SheetHeight = br1.ReadBytes(2),
                    SheetDataOffset = br1.ReadBytes(4),
                    ImgData = null
                    //ImageData(Hold)
                };

                if (new string(version3_TGLP.TGLPHeader) != "TGLP") throw new Exception("不明なフォーマットです(TGLP_Version3)");
                if (version3_TGLP.SectionSize != null)
                {
                    int SectionSize = BitConverter.ToInt32(RevEndian(version3_TGLP.SectionSize, Endian.big), 0);

                    int Img = SectionSize - 32;

                    version3_TGLP.ImgData = br1.ReadBytes(Img);

                    int W_Size = BitConverter.ToInt16(RevEndian(version3_TGLP.SheetWidth, Endian.big), 0);
                    int H_Size = BitConverter.ToInt16(RevEndian(version3_TGLP.SheetHeight, Endian.big), 0);

                    #region Backup
                    //byte[] vf = CFNT_Section.FINF_v3.TGLP_Ver3.ImgData;
                    //MemoryStream NBit = new MemoryStream(vf, );
                    //Bitmap bmp = new Bitmap(NBit);
                    //bmp = new Bitmap(W_Size, H_Size);
                    //bmp.Save(Open_BCFNT.FileName + "_TestOutput2", System.Drawing.Imaging.ImageFormat.Png);
                    //bmp.Dispose();
                    //NBit.Close();
                    #endregion

                    System.IO.FileStream fs2 = new FileStream(Open_BCFNT.FileName + "_TestOutput", FileMode.Create, FileAccess.Write);
                    BinaryWriter br = new BinaryWriter(fs2);
                    br.Write(version3_TGLP.ImgData);
                    br.Close();
                    fs2.Close();
                }

                CFNT_Section.FINF_v3.TGLP_Ver3 = version3_TGLP;
            }
            if (Version == 4)
            {

            }

            #region CWDH Section
            CFNTBinData.CWDH CWDH = new CFNTBinData.CWDH
            {
                CWDHHeader = br1.ReadChars(4),
                SectionSize = br1.ReadBytes(4),
                StartIndex = br1.ReadBytes(2),
                EndIndex = br1.ReadBytes(2),
                NextCWDHOffset = br1.ReadBytes(4),
                CharWidth_List = null
            };

            if (new string(CWDH.CWDHHeader) != "CWDH") throw new Exception("不明なフォーマットです(CWDH)");
            if (CWDH.EndIndex != null)
            {
                //List<CFNTBinData.CharWidth>を作成
                List<CFNTBinData.CharWidth> DefaultWidthList = new List<CFNTBinData.CharWidth>();

                //EndIndex = Count
                int CharWidthCount = BitConverter.ToInt16(RevEndian(CWDH.EndIndex, Endian.big), 0);
                for (int Count = 0; Count < CharWidthCount; Count++)
                {
                    CFNTBinData.CharWidth DefWidth = new CFNTBinData.CharWidth()
                    {
                        GlyphNumber = Count,
                        Left = br1.ReadByte(),
                        Glyph_Width = br1.ReadByte(),
                        Char_Width = br1.ReadByte()
                    };

                    //dataGridView2.Rows.Add(DefWidth.GlyphNumber, DefWidth.Left, DefWidth.Glyph_Width, DefWidth.Char_Width);

                    //DefWidthをDefaultWidthListに追加
                    DefaultWidthList.Add(DefWidth);
                }

                CWDH.CharWidth_List = DefaultWidthList;

                if (Version == 3)
                {
                    CFNT_Section.FINF_v3.CWDH = CWDH;
                }
                if (Version == 4)
                {
                    CFNT_Section.FINF_v4.CWDH = CWDH;
                }

                //CFNT_Section.FINF_v3.CWDH = CWDH;
            }
            #endregion

            fs1.Seek(4, SeekOrigin.Current);

            #region CMAP
            List<CFNTBinData.CMAP> CMAPList = new List<CFNTBinData.CMAP>();

            while (br1.BaseStream.Position != br1.BaseStream.Length)
            {
                CFNTBinData.CMAP CMAP = new CFNTBinData.CMAP
                {
                    CMAPHeader = br1.ReadChars(4),
                    SectionSize = br1.ReadBytes(4),
                    CodeBegin = br1.ReadBytes(2),
                    CodeEnd = br1.ReadBytes(2),
                    MappingMethod = br1.ReadBytes(2),
                    ReservedByte = br1.ReadBytes(2),
                    NextCMAPOffset = br1.ReadBytes(4),
                    CharList = null
                };

                #region CharSetAreaLength
                int SectionSize = BitConverter.ToInt32(RevEndian(CMAP.SectionSize, Endian.big), 0);
                int CharSetAreaLength = SectionSize - 20;
                int CharCount = CharSetAreaLength / 2;
                #endregion

                int CMAP_MappingMethod = BitConverter.ToInt16(RevEndian(CMAP.MappingMethod, Endian.big), 0);

                //Range of Unicode to be used(?)
                int CMAP_CODEBEGIN = BitConverter.ToInt16(RevEndian(CMAP.CodeBegin, Endian.Little), 0);
                int CMAP_CODEEND = BitConverter.ToInt16(RevEndian(CMAP.CodeEnd, Endian.Little), 0);

                List<byte[]> CharList = new List<byte[]>();

                //0 = Direct, 1 = Table, 2 = Scan
                if (CMAP_MappingMethod == 0)
                {
                    for (int CharsCount = 0; CharsCount < CharCount; CharsCount++)
                    {
                        CharList.Add(br1.ReadBytes(2));
                    }

                    CMAP.CharList = CharList;
                }
                else if (CMAP_MappingMethod == 1)
                {
                    for (int CharsCount = 0; CharsCount < CharCount; CharsCount++)
                    {
                        CharList.Add(br1.ReadBytes(2));
                    }

                    CMAP.CharList = CharList;
                }
                else if (CMAP_MappingMethod == 2)
                {
                    for (int CharsCount = 0; CharsCount < CharCount; CharsCount++)
                    {
                        CharList.Add(br1.ReadBytes(2));
                    }

                    CMAP.CharList = CharList;
                }

                CMAPList.Add(CMAP);

            }

            if (Version == 3)
            {
                CFNT_Section.FINF_v3.CMAP = CMAPList;
            }
            if (Version == 4)
            {
                CFNT_Section.FINF_v4.CMAP = CMAPList;
            }
            #endregion

            fs1.Close();

            #region PropertyGrid
            CFNT_PropertyGridSetting CFNT_PGS = new CFNT_PropertyGridSetting
            {
                BOM = CFNT_Section.BOM,
                Version = BitConverter.ToInt32(RevEndian(CFNT_Section.Version, Endian.big), 0),
                Version3_FINF = null,
                Version4_FINF = null,
                CWDHData = null,
                CMAPList = null
            };

            if(Version == 3)
            {
                #region FINF_PGS
                CFNT_PropertyGridSetting.Version3_FINF_PGS version3_FINF_PGS = new CFNT_PropertyGridSetting.Version3_FINF_PGS
                {
                    FontType = CFNT_Section.FINF_v3.FontType,
                    LineFeed = CFNT_Section.FINF_v3.LineFeed,
                    AlterCharIndex = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.AlterCharIndex, Endian.big), 0),
                    Default_Width = new CFNT_PropertyGridSetting.Version3_FINF_PGS.DefaultWidth
                    {
                        Left = CFNT_Section.FINF_v3.DefaultWidth.Left,
                        GlyphWidth = CFNT_Section.FINF_v3.DefaultWidth.GlyphWidth,
                        CharWidth = CFNT_Section.FINF_v3.DefaultWidth.CharWidth
                    },
                    Ascent = CFNT_Section.FINF_v3.Ascent,
                    Encoding = CFNT_Section.FINF_v3.Encoding,
                    Height = CFNT_Section.FINF_v3.Height,
                    Width = CFNT_Section.FINF_v3.Width,
                    TGLP_Ver3 = new CFNT_PropertyGridSetting.Version3_FINF_PGS.Version3_TGLP
                    {
                        CellWidth = CFNT_Section.FINF_v3.TGLP_Ver3.CellWidth,
                        CellHeight = CFNT_Section.FINF_v3.TGLP_Ver3.CellHeight,
                        Baseline_Position = CFNT_Section.FINF_v3.TGLP_Ver3.Baseline_Position,
                        MaxCharacterWidth = CFNT_Section.FINF_v3.TGLP_Ver3.MaxCharacterWidth,
                        NumberOfSheets = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.TGLP_Ver3.NumberOfSheets, Endian.big), 0),
                        SheetImgFormat = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.TGLP_Ver3.SheetImgFormat, Endian.big), 0),
                        NumberOfColumns = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.TGLP_Ver3.NumberOfColumns, Endian.big), 0),
                        NumberOfRows = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.TGLP_Ver3.NumberOfRows, Endian.big), 0),
                        SheetWidth = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.TGLP_Ver3.SheetWidth, Endian.big), 0),
                        SheetHeight = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.TGLP_Ver3.SheetHeight, Endian.big), 0),
                    }
                };

                CFNT_PGS.Version3_FINF = version3_FINF_PGS;
                #endregion

                #region CWDH_v3
                CFNT_PropertyGridSetting.CWDH CWDH_PGS = new CFNT_PropertyGridSetting.CWDH
                {
                    StartIndex = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.CWDH.StartIndex, Endian.big), 0),
                    EndIndex = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.CWDH.EndIndex, Endian.big), 0),
                    CharWidth_List = null
                };

                List<CFNT_PropertyGridSetting.CWDH.CharWidth> CharWidth_List = new List<CFNT_PropertyGridSetting.CWDH.CharWidth>();

                for (int CharWidthCount = 0; CharWidthCount < CFNT_Section.FINF_v3.CWDH.CharWidth_List.Count; CharWidthCount++)
                {
                    CFNT_PropertyGridSetting.CWDH.CharWidth CharWidth = new CFNT_PropertyGridSetting.CWDH.CharWidth
                    {
                        GlyphNumber = CFNT_Section.FINF_v3.CWDH.CharWidth_List[CharWidthCount].GlyphNumber,
                        Left = CFNT_Section.FINF_v3.CWDH.CharWidth_List[CharWidthCount].Left,
                        Glyph_Width = CFNT_Section.FINF_v3.CWDH.CharWidth_List[CharWidthCount].Glyph_Width,
                        Char_Width = CFNT_Section.FINF_v3.CWDH.CharWidth_List[CharWidthCount].Char_Width
                    };

                    CharWidth_List.Add(CharWidth);
                }

                CWDH_PGS.CharWidth_List = CharWidth_List;

                CFNT_PGS.CWDHData = CWDH_PGS;
                #endregion

                #region CMAP_v3
                List<CFNT_PropertyGridSetting.CMAP> CMAP_PGS_List = new List<CFNT_PropertyGridSetting.CMAP>();

                for(int CMAPCount = 0; CMAPCount < CFNT_Section.FINF_v3.CMAP.Count; CMAPCount++)
                {
                    CFNT_PropertyGridSetting.CMAP CMAP_PGS = new CFNT_PropertyGridSetting.CMAP
                    {
                        CodeBegin = CFNT_Section.FINF_v3.CMAP[CMAPCount].CodeBegin,
                        CodeEnd = CFNT_Section.FINF_v3.CMAP[CMAPCount].CodeEnd,
                        MappingMethod = BitConverter.ToInt16(RevEndian(CFNT_Section.FINF_v3.CMAP[CMAPCount].MappingMethod, Endian.big), 0),
                        CharList = null
                    };

                    List<string> CharList = new List<string>();

                    for(int CMAP_CharListCount = 0; CMAP_CharListCount < CFNT_Section.FINF_v3.CMAP[CMAPCount].CharList.Count; CMAP_CharListCount++)
                    {
                        string t = System.Text.Encoding.Unicode.GetString(RevEndian(CFNT_Section.FINF_v3.CMAP[CMAPCount].CharList[CMAP_CharListCount], Endian.big));
                        CharList.Add(t);
                    }

                    CMAP_PGS.CharList = CharList;

                    CMAP_PGS_List.Add(CMAP_PGS);
                }

                CFNT_PGS.CMAPList = CMAP_PGS_List;
                #endregion
            }

            propertyGrid1.SelectedObject = CFNT_PGS;
            #endregion

        }
    }
}
