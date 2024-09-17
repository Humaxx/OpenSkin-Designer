using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OpenSkinDesigner.Frames
{
    public partial class fOpen : Form
    {
        private DirectoryInfo skins = new DirectoryInfo("./skins");

        public enum eStatus { OK, CANCEL };
        private eStatus pStatus = eStatus.CANCEL;
        private String pSkinName = "";

        public eStatus Status
        {
            get { return pStatus; }
        }

        public String SkinName
        {
            get { return pSkinName; }
        }

        public void search(DirectoryInfo dir)
        {
            DirectoryInfo[] dis = dir.GetDirectories();
            if (dis.Length > 0)
            {
                foreach (DirectoryInfo di in dis)
                {
					// Logging-Code aufrufen
					// Logger.LogMessage("fOpen.cs -- hier wird die skin.xml geladen in FileInfo ???");
                    FileInfo[] xmlFiles = di.GetFiles("skin.xml");
                    foreach (FileInfo fi in xmlFiles)
                    {
                        String skinName = fi.DirectoryName.Substring(skins.FullName.Length + 1, fi.DirectoryName.Length - skins.FullName.Length - 1);
                        Console.Write(skinName);

                        System.Windows.Forms.ListViewItem listViewItem = new System.Windows.Forms.ListViewItem(skinName);
                        listView1.Items.Add(listViewItem);
                    }
                    search(di);
                }
            }
        }

        public fOpen()
        {
            InitializeComponent();
            SetLanguage();
            
            search(skins);
            if (listView1.Items.Count > 0)
            {
                listView1.Items[0].Selected = true;
                listView1_SelectedIndexChanged(null, null);
            }
        }

        private void SetLanguage()
        {
            this.Text = fMain.GetTranslation("Open project");
            lblSkinName.Text = fMain.GetTranslation("No skin selected");
            btnCancel.Text = fMain.GetTranslation("Cancel");
            btnOpen.Text = fMain.GetTranslation("Open");
        }

        private void fOpen_Load(object sender, EventArgs e)
        {



        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String skinName = "";

            if (sender == null && e == null)
            {
                skinName = listView1.Items[0].Text;
            }
            else if ((sender as ListView).SelectedItems.Count <= 0)
                return;
            else
            {
                ListViewItem lbi = ((sender as ListView).SelectedItems[0] as ListViewItem);
                skinName = lbi.Text;
            }

            if (new FileInfo("./skins/" + skinName + "/prev.png").Exists)
            {
                pictureBox1.Image = Image.FromFile("./skins/" + skinName + "/prev.png");
                //this.Size = new Size(listView1.Size.Width + 30 + pictureBox1.Image.Size.Width, pictureBox1.Image.Size.Height + 60);
            }
                
            
            else if (new FileInfo("./skins/" + skinName + "/preview.png").Exists)
            {
                pictureBox1.Image = Image.FromFile("./skins/" + skinName + "/preview.png");
               // this.Size = new Size(listView1.Size.Width + 30 + pictureBox1.Image.Size.Width, pictureBox1.Image.Size.Height + 60);
            }
                
            else
                pictureBox1.Image = null;

            lblSkinName.Text = skinName;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                pStatus = eStatus.OK;
                pSkinName = listView1.SelectedItems[0].Text;
                this.Close();
            }
            else
                btnCancel_Click(sender, e);
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            pStatus = eStatus.CANCEL;
            this.Close();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            btnOpen_Click(sender,e);
        }
    }
}
