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

        CFNTFormat.CFNT CFNTData;

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

            CFNTData = new CFNTFormat.CFNT(CFNTFormat.FINFVersion.Version3);
            CFNTData.ReadCFNT(br1);

            pictureBox1.BackColor = Color.Black;
            pictureBox1.Image = CFNTData.FINF_v3.TGLP_Ver3.TGLPImgDataList[0].SheetImg;

            propertyGrid1.SelectedObject = new CFNT_PropertyGridSetting(CFNTData);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CFNTData = null;
            pictureBox1.Image = null;
            propertyGrid1.SelectedObject = null;
            pictureBox1.BackColor = Color.White;
        }

        private void saveBCFNTToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
