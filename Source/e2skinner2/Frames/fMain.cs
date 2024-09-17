using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;


using OpenSkinDesigner.Logic;
using OpenSkinDesigner.Structures;
using System.Reflection;
using ScintillaNET;
using System.Threading;
using System.Linq;
using System.Configuration;

namespace OpenSkinDesigner.Frames
{
    public partial class fMain : Form
    {
        private cXMLHandler pXmlHandler = null;
        private cDesigner pDesigner = null;
        private cCommandQueue pQueue = null;
        //  private TreeView treeNodeCache = new System.Windows.Forms.TreeView();
        private ImageList treeImageList = new ImageList();
        private TreeView treeViewCache = new TreeView();

        private bool keyCapture = false;

        public fMain()
        {
            InitializeComponent();
            if (Properties.Settings.Default.UpgradeRequired == true)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            lblSkinName.Text = null;
            lblSkinName.Visible = false;
            MiSetFallbackColor.BackColor = Properties.Settings.Default.FallbackColor;
            MiUseFullAttributlist.Checked = Properties.Settings.Default.useFullAttribList;
            MiAddUndefinedColors.Checked = Properties.Settings.Default.addUndefinedColor;
            MiShowAttributList.Checked = Properties.Settings.Default.showAttributList;
            MiShowNotificationUnsafedChanges.Checked = Properties.Settings.Default.ShowChanges;
            MiShowNotificationUnsafedChangesEditor.Checked = Properties.Settings.Default.ShowChangesEditor;
            MiExperimentalDeleteMode.Checked = Properties.Settings.Default.ExperimentalDelete;
            MiLinewrapping.Checked = Properties.Settings.Default.LineWrapping;
            MiDontReplaceColors.Checked = Properties.Settings.Default.DontReplaceColors;
            SetLineWrapping();
            treeView1.CheckBoxes = Properties.Settings.Default.ExperimentalDelete;
            btnSkinned_Alpha.Checked = cProperties.getPropertyBool("enable_alpha");
            
            trackBarZoom.Enabled = false;
            numericUpDownZoom.Enabled = false;
            //jToolStripMenuItem.Visible = false; 
            AddCustomLanguages();
            SelectCustomLanguage();

            if (Platform.sysPlatform != Platform.ePlatform.MONO)
                textBoxEditor2.ConfigurationManager.Language = "xml";

            this.Text = String.Format("{0} v{1}", ((AssemblyProductAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product, Assembly.GetExecutingAssembly().GetName().Version.ToString());

            pDesigner = new cDesigner(pictureBox1.CreateGraphics());

            DirectoryInfo folder = new DirectoryInfo("elements");

            buildAddElementsMenu(folder, MiAddElement);

            pQueue = new cCommandQueue();
            pQueue.UndoPossibleEvent += new cCommandQueue.UndoRedoHandler(eventUndoPossible);
            pQueue.RedoPossibleEvent += new cCommandQueue.UndoRedoHandler(eventRedoPossible);

			 // Logging aktivieren
			Logger.SetLoggingEnabled(true);
			// Logging deaktivieren
			//Logger.SetLoggingEnabled(false);
        }

        private void AddCustomLanguages()
        {
            if (!(Directory.Exists("languages")))
            {
                Properties.Settings.Default.language = null;
                return;
            }
                
            DirectoryInfo di = new DirectoryInfo("languages");
                
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension.ToLower()==".lng")
                {
                    MiLanguage.Visible = true;
                    ToolStripMenuItem a = new ToolStripMenuItem();
                    a.Text = Path.GetFileNameWithoutExtension(fi.Name);
                    // a.CheckOnClick = true;
                    a.Click += new System.EventHandler(this.AddCustomLanguages_Click);
                    MiLanguage.DropDownItems.Add(a);
                }
            }

        }
        private void buildAddElementsMenu(DirectoryInfo folder, ToolStripMenuItem toolStripMenuItem)
        {
            foreach (DirectoryInfo f in folder.GetDirectories())
            {
                ToolStripMenuItem a = new ToolStripMenuItem();
                a.Text = GetTranslation(f.Name);
                a.Tag = f;
                toolStripMenuItem.DropDownItems.Add(a);
                buildAddElementsMenu(f, a);
            }

            foreach (FileInfo f in folder.GetFiles("*.ess"))
            {
                ToolStripMenuItem a = new ToolStripMenuItem();
                a.Text = GetTranslation(f.Name.Remove(f.Name.Length - f.Extension.Length));
                a.Tag = f.Name.Remove(f.Name.Length - f.Extension.Length);
                a.Click += new System.EventHandler(this.addUserElement_Click);
                toolStripMenuItem.DropDownItems.Add(a);
            }
        }
        private void eventUndoPossible(bool sender, EventArgs e)
        {
            MiUndo.Enabled = sender;
            btnUndo.Enabled = sender;
        }
        private void eventRedoPossible(bool sender, EventArgs e)
        {
            MiRedo.Enabled = sender;
            btnRedo.Enabled = sender;
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool result = CheckChangesEditorNew();
            if (result==false)
            {
                return;
            }
            else
            {
                result = CheckChangesNew();
                if (result == false)
                {
                    return;
                }
               
            }
            fOpen ftmp = new fOpen();
            //ftmp.setup(pXmlHandler);
            ftmp.ShowDialog();
            if (ftmp.Status == fOpen.eStatus.OK)
            {
				// Logging-Code aufrufen
				Logger.LogMessage("fMain.cs -- hier wird die skin.xml geladen");
                cProperties.setProperty("path_skin_xml", ftmp.SkinName + "/skin.xml");
                open(ftmp.SkinName);
                TreeNode treeNode = treeView1.Nodes[0];
                treeView1.SelectedNode = treeNode;
            }
            pQueue.clear();
                       
        }
        private bool CheckChangesEditorNew()
        {
            if (MyGlobaleVariables.UnsafedChangesEditor == false || Properties.Settings.Default.ShowChangesEditor == false)
            {
                return true;
            }
            else
            {
                DialogResult dialog = new DialogResult();
                dialog = MessageBox.Show(GetTranslation("You have made changes in the editor. Do you want to continue anyway?"), GetTranslation("Continue"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        private bool CheckChangesNew()
        {
            if (MyGlobaleVariables.UnsafedChanges == false || Properties.Settings.Default.ShowChanges == false)
            {
                return true;
            }
            else
            {
                DialogResult dialog = new DialogResult();
                dialog = MessageBox.Show(GetTranslation("You have unsafed changes. Do you want to continue anyway?"), GetTranslation("Continue"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    return true;
                }
                else
                    return false;
            }

        }
        private void fillImageList()
        {
            treeImageList.Images.Add(Properties.Resources.treeRoot);
            treeImageList.Images.Add(Properties.Resources.treeX);
            treeImageList.Images.Add(Properties.Resources.treeComment);
            treeImageList.Images.Add(Properties.Resources.treePanel);
            treeImageList.Images.Add(Properties.Resources.treeScreen);
            treeImageList.Images.Add(Properties.Resources.treeSelected);
            treeImageList.Images.Add(Properties.Resources.treeElement);
            treeImageList.Images.Add(Properties.Resources.treePixmap);
            treeImageList.Images.Add(Properties.Resources.treeLabel);
            treeImageList.Images.Add(Properties.Resources.treeWidget);
        }
        public bool getVariables()
        {
            TextBox TB = new TextBox();
            String Dateiname = cProperties.getProperty("path") + "/" + cProperties.getProperty("path_skin_xml");
            String strTemp = string.Empty;
            String strVariablenName = string.Empty;
            if (!File.Exists(Dateiname))
                return false;
            try
            {
                TB.Text = File.ReadAllText(Dateiname);
                TB.Text = TB.Text.Substring(TB.Text.IndexOf("<variables>"));
                TB.Text = TB.Text.Substring(0, TB.Text.IndexOf("</variables>") + 12);
                for (int b = 0; b < TB.Lines.Length - 1; b++)
                {
                    if (TB.Lines[b].Contains("<variable ") && TB.Lines[b].Contains("name=") && TB.Lines[b].Contains("value="))
                    {
                        strVariablenName = TB.Lines[b].Substring(TB.Lines[b].IndexOf("name=" + "\"") + 6);
                        MyGlobaleVariables.SkinName.Add(strVariablenName.Substring(0, strVariablenName.IndexOf("\"")));
                        strTemp = TB.Lines[b].Substring(TB.Lines[b].IndexOf("value=" + "\"") + 7);
                        MyGlobaleVariables.SkinValue1.Add(strTemp.Substring(0, strTemp.IndexOf(",")));
                        strTemp = strTemp.Substring(strTemp.IndexOf(",") + 1);
                        MyGlobaleVariables.SkinValue2.Add(strTemp.Substring(0, strTemp.IndexOf("\"")));
                    }
                }
                return true;

            }
            catch
            {
                return false;
            }
        }
        public void open(String path)
        {
            // Close all open
            close();
			// Logging-Code aufrufen
			Logger.LogMessage("fMain.cs -- hier wird die skin.xml eingelesen in fillimagelist");
            trackBarZoom.Enabled = true; //MOD
            numericUpDownZoom.Enabled = true; //MOD
            cProperties.setProperty("path_skin", path);
            cProperties.setProperty("path", "./skins");
            cProperties.setProperty("path_fonts", "./fonts");

            pXmlHandler = new cXMLHandler();
            fillImageList();
            // Evtl. vorhandene Variablen einlesen
            getVariables();
            //treeview TO Xml
            pXmlHandler.XmlPathToTreeView(cProperties.getProperty("path") + "/" + cProperties.getProperty("path_skin_xml"), treeView1);
            cDataBase.init(pXmlHandler);

            pXmlHandler.XmlToTreeView(treeView1); // Update if double defined colors found
           
            foreach (TreeNode node in treeView1.Nodes)
            {
                treeViewCache.Nodes.Add((TreeNode)node.Clone());
            }

            treeView1.ImageList = treeImageList;
            treeView1.ImageIndex = 0; //MOD was set to 1 before changes
            treeView1.SelectedImageIndex = 5;
            treeView1.GetNodeAt(0, 0).Expand();
            float xratio = (float)panelDesignerInner.Width / cDataBase.pResolution.getResolution().Xres;
            float yratio = (float)panelDesignerInner.Height / cDataBase.pResolution.getResolution().Yres;
            float zoom = xratio < yratio ? xratio : yratio;
            trackBarZoom.Value = (int)((zoom - 1.0f) * 100.0f - 0.5f);
            pDesigner.drawFrame();

            MiOpen.Enabled = true;
            MiSave.Enabled = true;
            MiSaveAs.Enabled = true;
            MiClose.Enabled = true;
            MiResolution.Enabled = true;
            MiColors.Enabled = true;
            MiFonts.Enabled = true;
            MiWindowStyles.Enabled = true;
            btnSave.Enabled = true;
            btnSaveEditor.Enabled = true;
            MiAddElement.Enabled = true;
            this.MiAddLabel.Enabled = true;
            this.MiAddPixmap.Enabled = true;
            this.MiAddWidget.Enabled = true;
            this.MiDeleteSelected.Enabled = true;
            this.btnEditRoot.Enabled = true;
            this.tbxTreeFilter.Enabled = true;
            this.btnAddLabel.Enabled = true;
            this.btnAddPixmap.Enabled = true;
            this.btnAddWidget.Enabled = true;
            this.btnDelete.Enabled = true;
            this.btnAddScreen.Enabled = true;
            this.btnAddPanel.Enabled = true;
            this.toolStripEditor.Enabled = true;
            this.textBoxEditor2.Enabled = true;
            lblSkinName.Visible = true;
            lblSkinName.Text = cProperties.getProperty("path_skin");
        }
        public void close()
        {
            pXmlHandler = null;
            pQueue.clear();//MOD
            trackBarZoom.Enabled = false; //MOD
            numericUpDownZoom.Enabled = false; //MOD
            MyGlobaleVariables.SkinName.Clear();
            MyGlobaleVariables.SkinValue1.Clear();
            MyGlobaleVariables.SkinValue2.Clear();
            MyGlobaleVariables.Reload = false;
            pDesigner.clear();
            cDataBase.clear();
            treeView1.Nodes.Clear();
            treeView1.Invalidate();
            trackBarZoom.Enabled = false;//MOD
            numericUpDownZoom.Enabled = false;//MOD

            if (Platform.sysPlatform == Platform.ePlatform.MONO)
                textBoxEditor.Clear();
            else
                textBoxEditor2.Text = "";
            propertyGrid1.SelectedObject = null;

            pictureBox1.Invalidate();

            MiOpen.Enabled = true;
            MiAddElement.Enabled = false;
            MiSave.Enabled = false;
            MiSaveAs.Enabled = false;
            MiClose.Enabled = false;
            MiResolution.Enabled = false;
            MiColors.Enabled = false;
            MiFonts.Enabled = false;
            MiWindowStyles.Enabled = false;
            btnSave.Enabled = false;
            btnSaveEditor.Enabled = false;
            lblSkinName.Text = null;
            lblSkinName.Visible = false;
            TslStatus.Text = GetTranslation("No Errors.");

            this.MiAddLabel.Enabled = false;
            this.MiAddPixmap.Enabled = false;
            this.MiAddWidget.Enabled = false;
            this.MiDeleteSelected.Enabled = false;
            this.btnAddLabel.Enabled = false;
            this.btnAddPixmap.Enabled = false;
            this.btnAddWidget.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnAddScreen.Enabled = false;
            this.btnEditRoot.Enabled = false;
            this.tbxTreeFilter.Enabled = false;
            this.btnAddPanel.Enabled = false;
            this.toolStripEditor.Enabled = false;
            this.textBoxEditor2.Enabled = false;

            MyGlobaleVariables.UnsafedChanges = false;
            MyGlobaleVariables.UnsafedChangesEditor = false;

            tbxSearchCode.Text = GetTranslation("Search...");
            tbxTreeFilter.Text = GetTranslation("Search...");

            pQueue.clear();
        }
        public void reload()
        {
            save();
            close();
            open(cProperties.getProperty("path_skin_xml"));
        }
        public void save()
        {
            pXmlHandler.XmlToFile(cProperties.getProperty("path_skin_xml"));
            MyGlobaleVariables.UnsafedChanges = false;
            MyGlobaleVariables.UnsafedChangesEditor = false;
        }
        public void saveAs(String name)
        {
            pXmlHandler.XmlToFileAs(name);
            MyGlobaleVariables.UnsafedChanges = false;
            MyGlobaleVariables.UnsafedChangesEditor = false;
            //cProperties.setProperty("path_skin_xml", name); //MOD changed because without it, it will crash on save button...
        }
        private void refreshEditor()
        {
            lbxSearchCodeEditor.Visible = false;
            tbxSearchCode.Text = GetTranslation("Search...");
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                int hash = selectedNode.GetHashCode();
                XmlNode node = pXmlHandler.getXmlNode(hash);
                {
                    String text = node.OuterXml;
                    text = FormatXml(node);

                    if (Platform.sysPlatform == Platform.ePlatform.MONO)
                    {
                        textBoxEditor.Clear();
                        textBoxEditor.AppendText(text);

                        textBoxEditor.SelectionStart = 0;
                        textBoxEditor.ScrollToCaret();
                    }
                    else
                        textBoxEditor2.Text = text;
                }
            }
        }
        private XmlNode screenSearch(string screen, TreeNodeCollection tree)
        {
            foreach (TreeNode node in tree)
            {
                XmlNode nodeXml = pXmlHandler.getXmlNode(node.GetHashCode());
                if (nodeXml.Name == "screen" &&
                    nodeXml.Attributes["name"] != null &&
                    nodeXml.Attributes["name"].Value == screen)
                    return nodeXml;
                if (nodeXml.Name == "include")
                {
                    nodeXml = screenSearch(screen, node.Nodes);
                    if (nodeXml != null)
                        return nodeXml;
                }
            }
            return null;
        }
        private void refresh()
        {
            lbxSearch.Visible = false;
            refreshEditor();
            tbxTreeFilter.Text = GetTranslation("Search...");
            propertyGrid1.SelectedObject = null;

            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                int hash = selectedNode.GetHashCode();
                XmlNode node = pXmlHandler.getXmlNode(hash);
                {
                    //Get Screen Node
                    XmlNode screenNode = node;
                    //As we could be selecting a sub Element, walk up
                    while (screenNode != null && screenNode.Name != "screen")
                    {
                        hash = pXmlHandler.XmlGetParentHandle(hash);
                        screenNode = pXmlHandler.getXmlNode(hash);
                    }

                    pDesigner.clear();

                    if (screenNode != null)
                    {
                        //Draw Screen and its Elements

                        pDesigner.drawBackground();

                        sAttribute subattr = null;

                        sAttribute attr = new sAttributeScreen(screenNode);
                        propertyGrid1.SelectedObject = attr;

                        pDesigner.draw(attr);

                        XmlNode[] nodes = pXmlHandler.XmlGetChildNodes(hash);

                        if (nodes.Length > 0 && screenNode != node)
                            propertyGrid1.SelectedObject = null;

                        foreach (XmlNode tmpnode in nodes)
                        {
                            if (tmpnode.Name == "eLabel")
                            {
                                subattr = new sAttributeLabel(attr, tmpnode);
                                pDesigner.draw(subattr);
                            }
                            else if (tmpnode.Name == "ePixmap")
                            {
                                subattr = new sAttributePixmap(attr, tmpnode);
                                pDesigner.draw(subattr);
                            }
                            else if (tmpnode.Name == "widget")
                            {
                                subattr = new sAttributeWidget(attr, tmpnode);
                                pDesigner.draw(subattr);
                            }
                            else if (tmpnode.Name == "panel")
                            {
                                if (cProperties.getPropertyBool("skinned_panels"))
                                {
                                    if (tmpnode.Attributes["name"] != null)
                                    {
                                        XmlNode panelNodeXml = screenSearch(tmpnode.Attributes["name"].Value, treeView1.Nodes[0].Nodes);
                                        if (panelNodeXml != null)
                                            showPanel(panelNodeXml, attr);
                                    }
                                    else
                                    {
                                        subattr = new sAttribute(attr, tmpnode);
                                        showPanel(tmpnode, subattr);
                                    }
                                }
                            }
                            else
                            {
                                subattr = null;
                            }

                            if (tmpnode == node && subattr != null)
                            {
                                propertyGrid1.SelectedObject = subattr;
                                if (cProperties.getPropertyBool("fading"))
                                    try { pDesigner.drawFog((int)subattr.pAbsolutX, (int)subattr.pAbsolutY, (int)subattr.pWidth, (int)subattr.pHeight); }
                                    catch { }
                            }
                        }

                        pDesigner.drawFrame();
                    }

                    pDesigner.sort();
                    pictureBox1.Invalidate();
                }
            }
        }
        private void showPanel(XmlNode node, sAttribute attr)
        {
            //Get Screen Node
            XmlNode screenNode = node;
            if (screenNode != null)
            {
                //Draw Screen and its Elements
                sAttribute subattr = null;

                XmlNodeList nodes = screenNode.ChildNodes;
                Console.WriteLine("building panel");

                foreach (XmlNode tmpnode in nodes)
                {
                    if (tmpnode.Name == "eLabel")
                    {
                        subattr = new sAttributeLabel(attr, tmpnode);
                        pDesigner.draw(subattr);
                    }
                    else if (tmpnode.Name == "ePixmap")
                    {
                        subattr = new sAttributePixmap(attr, tmpnode);
                        pDesigner.draw(subattr);
                    }
                    else if (tmpnode.Name == "widget")
                    {
                        subattr = new sAttributeWidget(attr, tmpnode);
                        pDesigner.draw(subattr);
                    }
                    else if (tmpnode.Name == "panel")
                    {
                        if (tmpnode.Attributes["name"] != null)
                        {
                            XmlNode panelNodeXml = screenSearch(tmpnode.Attributes["name"].Value, treeView1.Nodes[0].Nodes);
                            if (panelNodeXml != null)
                                showPanel(panelNodeXml, attr);
                        }
                        else
                        {
                            subattr = new sAttribute(attr, tmpnode);
                            showPanel(tmpnode, subattr);
                        }
                    }
                }
            }
        }
        private void treeView1_AfterSelect(object sender, EventArgs e)
        {
            MyGlobaleVariables.UnsafedChangesEditor = false;
            refresh();
        }
        private void treeView1_AfterDoubleclick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            refresh();
        }
        private void resolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MessageBox.Show(GetTranslation("This function is not sufficiently tested and seems unstable. It is not recommended to use it!"),
                GetTranslation("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult rst = MessageBox.Show(GetTranslation("Note:") + " " + GetTranslation("The skin will be automatically saved if you want to continue!") + "\n" + GetTranslation("It is recommend to create a backup of your skin first!") + "\n\n" + GetTranslation("Press 'Cancel' to abort."),
               GetTranslation("Warning"), MessageBoxButtons.OKCancel);

            if (rst == DialogResult.OK)
            {
                pQueue.clear();
                reload();
                fResolution ftmp = new fResolution();
                ftmp.setup(pXmlHandler);
                ftmp.ShowDialog();
                reload();
            }
        }
        private string FormatXml(XmlNode sUnformattedXml)
        {
            //will hold formatted xml
            StringBuilder sb = new StringBuilder();

            //pumps the formatted xml into the StringBuilder above
            StringWriter sw = new StringWriter(sb);

            //does the formatting
            XmlTextWriter xtw = null;

            try
            {
                //point the xtw at the StringWriter
                xtw = new XmlTextWriter(sw);

                //we want the output formatted
                xtw.Formatting = Formatting.Indented;

                //get the dom to dump its contents into the xtw 
                //xd.WriteTo(xtw);
                sUnformattedXml.WriteTo(xtw);
            }
            finally
            {
                //clean up even if error
                if (xtw != null)
                    xtw.Close();
            }

            //return the formatted xml
            return sb.ToString();
        }
        private void colorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pQueue.clear(); //Has to be done as colors could have been renamed

            fColors ftmp = new fColors();
            ftmp.setup(pXmlHandler);
            ftmp.ShowDialog();
            pXmlHandler.XmlToTreeView(treeView1);
            treeView1.GetNodeAt(0, 0).Expand();
            refresh();
        }
        private void fontsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pQueue.clear();

            fFonts ftmp = new fFonts();
            ftmp.setup(pXmlHandler);
            ftmp.ShowDialog();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            pDesigner.paint(sender, e);
        }

        private bool pSemaphorePropertyGrid = false;
        private void eventDoPropertyGrid(cCommandQueue.cCommand sender, EventArgs e)
        {
            pSemaphorePropertyGrid = true;

            Array objFrom = sender.To as Array;

            treeView1.SelectedNode = pXmlHandler.getTreeNode((sender.Helper as sAttribute).myNode);

            propertyGrid1.Refresh();

            PropertyInfo pi = (sender.Helper as sAttribute).GetType().GetProperty(objFrom.GetValue(0).ToString());
            pi.SetValue((sender.Helper as sAttribute), objFrom.GetValue(1), null);

            refreshPropertyGrid();

            pSemaphorePropertyGrid = false;

            // This is a workaround, if a Do was made, an undo is possible, so activate it.
            MiUndo.Enabled = pQueue.isUndoPossible();
            btnUndo.Enabled = pQueue.isUndoPossible();
        }
        private void eventUndoPropertyGrid(cCommandQueue.cCommand sender, EventArgs e)
        {
            pSemaphorePropertyGrid = true;

            Array objFrom = sender.From as Array;

            treeView1.SelectedNode = pXmlHandler.getTreeNode((sender.Helper as sAttribute).myNode);

            propertyGrid1.Refresh();

            PropertyInfo pi = (sender.Helper as sAttribute).GetType().GetProperty(objFrom.GetValue(0).ToString());
            pi.SetValue((sender.Helper as sAttribute), objFrom.GetValue(1), null);

            refreshPropertyGrid();

            pSemaphorePropertyGrid = false;
        }
        private void refreshPropertyGrid()
        {
            //Property changed, so save it to xml, and repaint screen
            //if (treeView1.SelectedNode != null)
            {
                //Actually, this only syncs the name of the element with treeview, no saveing is done here
                if (treeView1.SelectedNode != null)
                    pXmlHandler.XmlSyncTreeChilds(treeView1.SelectedNode.GetHashCode(), treeView1.SelectedNode);
            }

            refresh();
            //pDesigner.sort();
            propertyGrid1.Refresh();
            pictureBox1.Invalidate();
        }
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e == null)
                return;

            if (pSemaphorePropertyGrid)
                return;

            //if (treeView1.SelectedNode != null)
            {
                cCommandQueue.cCommand cmd = new cCommandQueue.cCommand("PropertyGridChanged");
                GridItem gI1 = propertyGrid1.SelectedGridItem;
                cmd.Helper = (s as PropertyGrid).SelectedObject;

                String label = e.ChangedItem.Label;
                PropertyInfo pi = ((s as PropertyGrid).SelectedObject as sAttribute).GetType().GetProperty(label);
                Object oldValue = e.OldValue;
                //This is maybe not the correct place for this                
                MyGlobaleVariables.UnsafedChanges = true;
                //
                if (pi == null)
                {
                    //FIXME: This is just a workaround
                    label = e.ChangedItem.Parent.Label;
                    pi = ((s as PropertyGrid).SelectedObject as sAttribute).GetType().GetProperty(label);
                    Object gi = e.ChangedItem.Parent.Value;
                    if (gi != null)
                    {
                        if (gi is OpenSkinDesigner.Structures.sAttribute.Position)
                        {
                            if (e.ChangedItem.Label == "X")
                                (gi as OpenSkinDesigner.Structures.sAttribute.Position).X = (String)e.OldValue;
                            else
                                (gi as OpenSkinDesigner.Structures.sAttribute.Position).Y = (String)e.OldValue;

                            oldValue = (gi as OpenSkinDesigner.Structures.sAttribute.Position);
                        }
                        else if (gi is System.Drawing.Size)
                        {
                            System.Drawing.Size size;

                            size = (System.Drawing.Size)gi;

                            if (e.ChangedItem.Label == "Width")
                                size.Width = Convert.ToInt32(e.OldValue);
                            else
                                size.Height = Convert.ToInt32(e.OldValue);

                            oldValue = size;
                        }
                        else
                        {
                            Console.WriteLine("Error in propertyGrid1_PropertyValueChanged #1");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error in propertyGrid1_PropertyValueChanged #0");
                    }
                }

                if (pi != null)
                {
                    Object newValue = pi.GetValue(((s as PropertyGrid).SelectedObject as sAttribute), null);

                    cmd.From = new Object[] { label, oldValue };
                    cmd.To = new Object[] { label, newValue };

                    cmd.DoEvent += new cCommandQueue.EventHandler(eventDoPropertyGrid);
                    cmd.UndoEvent += new cCommandQueue.EventHandler(eventUndoPropertyGrid);

                    pQueue.addCmd(cmd);
                    propertyGrid1.SelectedGridItem = gI1;
                }
                else
                    Console.WriteLine("Error in propertyGrid1_PropertyValueChanged #2");
            }
        }
        private void btnSkinned_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_screen", btnSkinned.Checked);
            cProperties.setProperty("skinned_label", btnSkinned.Checked);
            cProperties.setProperty("skinned_pixmap", btnSkinned.Checked);
            cProperties.setProperty("skinned_widget", btnSkinned.Checked);




            btnSkinnedScreen.Checked = btnSkinned.Checked;
            btnSkinnedLabel.Checked = btnSkinned.Checked;
            btnSkinnedPixmap.Checked = btnSkinned.Checked;
            btnSkinnedWidget.Checked = btnSkinned.Checked;

            pictureBox1.Invalidate();
        }
        private void btnSkinnedScreen_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_screen", btnSkinnedScreen.Checked);

            if ((btnSkinnedScreen.Checked && btnSkinnedLabel.Checked && btnSkinnedPixmap.Checked && btnSkinnedWidget.Checked)
                || (!btnSkinnedScreen.Checked && !btnSkinnedLabel.Checked && !btnSkinnedPixmap.Checked && !btnSkinnedWidget.Checked))
                btnSkinned.Checked = btnSkinnedScreen.Checked;

            pictureBox1.Invalidate();
        }
        private void btnSkinned_Conditional_Default_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_conditional_default", true);
            cProperties.setProperty("skinned_conditional_all", false);
            cProperties.setProperty("skinned_conditional_random", false);
            cProperties.setProperty("skinned_conditional_none", false);
            refresh_Conditional_Random();
            this.refresh();
        }
        private void btnSkinned_Conditional_None_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_conditional_default", false);
            cProperties.setProperty("skinned_conditional_all", false);
            cProperties.setProperty("skinned_conditional_random", false);
            cProperties.setProperty("skinned_conditional_none", true);
            refresh_Conditional_Random();
            this.refresh();
        }
        private void btnSkinned_Conditional_All_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_conditional_default", false);
            cProperties.setProperty("skinned_conditional_all", true);
            cProperties.setProperty("skinned_conditional_random", false);
            cProperties.setProperty("skinned_conditional_none", false);
            refresh_Conditional_Random();
            this.refresh();
        }
        private void btnSkinned_Conditional_Random_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_conditional_default", false);
            cProperties.setProperty("skinned_conditional_all", false);
            cProperties.setProperty("skinned_conditional_random", true);
            cProperties.setProperty("skinned_conditional_none", false);
            refresh_Conditional_Random();
            this.refresh();
        }
        private void refresh_Conditional_Random()
        {
            btnSkinned_Conditional_Default.Checked = cProperties.getPropertyBool("skinned_conditional_default");
            btnSkinned_Conditional_All.Checked = cProperties.getPropertyBool("skinned_conditional_all");
            btnSkinned_Conditional_Random.Checked = cProperties.getPropertyBool("skinned_conditional_random");
            btnSkinned_Conditional_None.Checked = cProperties.getPropertyBool("skinned_conditional_none");
        }
        private void btnSkinnedLabel_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_label", btnSkinnedLabel.Checked);

            if ((btnSkinnedScreen.Checked && btnSkinnedLabel.Checked && btnSkinnedPixmap.Checked && btnSkinnedWidget.Checked)
                || (!btnSkinnedScreen.Checked && !btnSkinnedLabel.Checked && !btnSkinnedPixmap.Checked && !btnSkinnedWidget.Checked))
                btnSkinned.Checked = btnSkinnedScreen.Checked;

            pictureBox1.Invalidate();
        }
        private void btnSkinnedPixmap_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_pixmap", btnSkinnedPixmap.Checked);

            if ((btnSkinnedScreen.Checked && btnSkinnedLabel.Checked && btnSkinnedPixmap.Checked && btnSkinnedWidget.Checked)
                || (!btnSkinnedScreen.Checked && !btnSkinnedLabel.Checked && !btnSkinnedPixmap.Checked && !btnSkinnedWidget.Checked))
                btnSkinned.Checked = btnSkinnedScreen.Checked;

            pictureBox1.Invalidate();
        }
        private void btnSkinnedWidget_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_widget", btnSkinnedWidget.Checked);

            if ((btnSkinnedScreen.Checked && btnSkinnedLabel.Checked && btnSkinnedPixmap.Checked && btnSkinnedWidget.Checked)
                || (!btnSkinnedScreen.Checked && !btnSkinnedLabel.Checked && !btnSkinnedPixmap.Checked && !btnSkinnedWidget.Checked))
                btnSkinned.Checked = btnSkinnedScreen.Checked;

            pictureBox1.Invalidate();
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }
        private void windowStylesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fWindowstyle ftmp = new fWindowstyle();
            ftmp.setup(pXmlHandler);
            ftmp.ShowDialog();
        }
        private void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool result = CheckChangesEditorNew();
            if (result == false)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                result = CheckChangesNew();
                if (result == false)
                {
                    e.Cancel = true;
                    return;
                }

            }
            cProperties.saveFile();
              
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "XML - Files (*.xml)|*.xml"; //MOD
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName.ToLower().EndsWith(".xml"))
                {
                    saveAs(saveFileDialog1.FileName);
                }
                else
                {
                    saveAs(saveFileDialog1.FileName + ".xml");
                }
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            save();
        }
        private void MiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void toolStripMenuItemHelp_Click(object sender, EventArgs e)
        {
            new fAbout().ShowDialog();
        }
        private void addLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pQueue.clearUndo(); // We need to clear the UndoList. The problem is that we readded elements an these elements now have different hashes :-(
            _addElement("eLabel", null);
        }
        private void addPixmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pQueue.clearUndo(); // We need to clear the UndoList. The problem is that we readded elements an these elements now have different hashes :-(
            _addElement("ePixmap", null);
        }
        private void widgetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pQueue.clearUndo(); // We need to clear the UndoList. The problem is that we readded elements an these elements now have different hashes :-(
            _addElement("widget", null);
        }
        private void AddCustomLanguages_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in MiLanguage.DropDownItems)
            {
                if (item.Text == (sender as ToolStripDropDownItem).Text)
                {
                    if (item.Checked== true) // Dont use custom language anymore
                    {
                        item.Checked = false;
                        Properties.Settings.Default.language = null;
                    }
                    else
                    {
                        item.Checked = true;
                        Properties.Settings.Default.language = (sender as ToolStripDropDownItem).Text;
                        MyGlobaleVariables.Language = File.ReadAllLines("languages/" + (sender as ToolStripDropDownItem).Text + ".lng");                        
                    }
                    UseCustomLanguage();

                }
                else
                {
                    item.Checked = false;
                }
            }
            Properties.Settings.Default.Save();
        }
        private void SelectCustomLanguage()
        {
            if (Properties.Settings.Default.language == null)
                return;
            foreach (ToolStripMenuItem item in MiLanguage.DropDownItems)
            {
                if (item.Text == Properties.Settings.Default.language)
                {
                    item.Checked = true;
                    MyGlobaleVariables.Language = File.ReadAllLines("languages/" + item.Text + ".lng");
                    UseCustomLanguage();
                    return;
                }
                
            }
            // Selected language not found
            Properties.Settings.Default.language = null;
            Properties.Settings.Default.Save();
        }
        private void addUserElement_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem))
                return;

            TreeNode selectedNode = treeView1.SelectedNode;

            FileInfo f = (sender as ToolStripMenuItem).Tag as FileInfo;

            XmlDocument xDoc = new XmlDocument();

            xDoc.Load(f.FullName);
            if (!xDoc.HasChildNodes)
                return;

            XmlNode rootNode = xDoc.FirstChild;
            if (rootNode.Name.Equals("element"))
            {
                if (rootNode.HasChildNodes)
                {
                    pQueue.clearUndo(); // We need to clear the UndoList. The problem is that we readded elements an these elements now have different hashes :-(

                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        _addElement(null, node.OuterXml);
                    }
                }
            }
        }

        [Obsolete("use _addElement(String type, String outerXml) instead")]
        private void addElement(String type)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                int hash = selectedNode.GetHashCode();
                XmlNode node = pXmlHandler.getXmlNode(hash);
                {
                    //Get Screen Node
                    XmlNode screenNode = node;
                    //As we could be selecting a sub Element, walk up
                    while (screenNode != null && screenNode.Name != "screen")
                    {
                        hash = pXmlHandler.XmlGetParentHandle(hash);
                        screenNode = pXmlHandler.getXmlNode(hash);
                    }

                    String[] attributes = { type,
                                            "name",  "",
                                            "position",  "0, 0",
                                            "size",  "100, 100" };
                    TreeNode treenode = selectedNode;
                    if (!treenode.Text.StartsWith("screen"))
                        treenode = treenode.Parent;

                    treeView1.SelectedNode = pXmlHandler.XmlSyncAddTreeChild(treenode.GetHashCode(), treenode, pXmlHandler.XmlAppendNode(screenNode, attributes));
                    pXmlHandler.XmlSyncTreeChilds(treeView1.SelectedNode.GetHashCode(), treeView1.SelectedNode);
                }
            }
        }
        private void _addElement(String type, String outerXml)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                XmlNode node = pXmlHandler.getXmlNode(selectedNode); // The parent element where we want to add a child. This has to be screen !!

                while (node != null && node.Name != "screen")
                {
                    node = node.ParentNode;
                }

                if (node != null)
                {
                    int nodeHash = pXmlHandler.getHash(node);

                    cCommandQueue.cCommand cmd = new cCommandQueue.cCommand("addElement");
                    cmd.DoEvent += new cCommandQueue.EventHandler(eventDoElement);
                    cmd.UndoEvent += new cCommandQueue.EventHandler(eventUndoElement);

                    cmd.Helper = nodeHash;
                    cmd.From = node.OuterXml; // For from we need the parent outerxml, for to we dont need it

                    //--
                    XmlNode newXmlNode = null;
                    if (outerXml == null)
                    {
                        String[] attributes = { type,
                                                "name",  "",
                                                "position",  "0, 0",
                                                "size",  "100, 100" };

                        newXmlNode = pXmlHandler.XmlAppendNode(node, attributes);
                    }
                    else
                    {
                        newXmlNode = pXmlHandler.XmlAppendNode(node, outerXml);
                    }
                    TreeNode newTreeNode = pXmlHandler.XmlSyncAddTreeChild(nodeHash, pXmlHandler.getTreeNode(nodeHash), newXmlNode);
                    pXmlHandler.XmlSyncTreeChilds(newTreeNode.GetHashCode(), newTreeNode);
                    //--

                    cmd.To = node.OuterXml; // Set the result

                    pQueue.addSilentCmd(cmd);
                    MyGlobaleVariables.UnsafedChanges = true;

                    refresh();
                    refreshPropertyGrid();


                }
            }
        }
        private void _addScreen(object sender, EventArgs e)
        {
            addRootItem("screen");
        }
        private void _addPanel(object sender, EventArgs e)
        {
            addRootItem("panel");
        }
        private void addRootItem(String item)
        {
            if (treeView1.Nodes.Count > 0)
            {
                treeView1.GetNodeAt(0, 0).Expand();
                treeView1.SelectedNode = treeView1.Nodes[0];
            }
            String outerXml = null;
            TreeNode selectedNode = treeView1.SelectedNode;

            if (selectedNode != null)
            {


                while (selectedNode.Parent != null)
                {
                    selectedNode = selectedNode.Parent;
                }
                XmlNode node = pXmlHandler.getXmlNode(selectedNode); // The parent element where we want to add a child. This has to be screen !!

                if (node != null)
                {
                    int nodeHash = pXmlHandler.getHash(node);

                    cCommandQueue.cCommand cmd = new cCommandQueue.cCommand("addElement");
                    cmd.DoEvent += new cCommandQueue.EventHandler(eventDoElement);
                    cmd.UndoEvent += new cCommandQueue.EventHandler(eventUndoElement);

                    cmd.Helper = nodeHash;
                    cmd.From = node.OuterXml; // For from we need the parent outerxml, for to we dont need it

                    //--
                    XmlNode newXmlNode = null;
                    if (outerXml == null)
                    {
                        if (item == "screen")
                        {
                            String[] attributes = { "screen",
                                                "name",  "NewScreen",
                                                "position",  "0, 0",
                                                "flags","wfNoBorders",
                                                "transparent","0",
                                                "backgroundColor","transparent",
                                                "size",  cDataBase.pResolution.getResolution().Xres.ToString()+", "+ cDataBase.pResolution.getResolution().Yres.ToString() };
                            newXmlNode = pXmlHandler.XmlAppendNode(node, attributes);
                        }
                        else
                        {
                            String[] attributes = { "screen",
                                                "name",  "NewScreen" };
                            newXmlNode = pXmlHandler.XmlAppendNode(node, attributes);
                        }


                    }
                    else
                    {
                        newXmlNode = pXmlHandler.XmlAppendNode(node, outerXml);
                    }
                    TreeNode newTreeNode = pXmlHandler.XmlSyncAddTreeChild(nodeHash, pXmlHandler.getTreeNode(nodeHash), newXmlNode);
                    pXmlHandler.XmlSyncTreeChilds(newTreeNode.GetHashCode(), newTreeNode);
                    //--

                    cmd.To = node.OuterXml; // Set the result
                    MyGlobaleVariables.UnsafedChanges = true;
                    pQueue.addSilentCmd(cmd);

                    refresh();
                    //pDesigner.sort();
                    //refreshEditor();
                    propertyGrid1.Refresh();
                    pictureBox1.Invalidate();


                }
            }
            if (treeView1.Nodes.Count > 0)
            {
                tabControl1.SelectedIndex = 1;
                treeView1.SelectedNode = treeView1.Nodes[0].Nodes[treeView1.Nodes[0].Nodes.Count - 1];
                Console.WriteLine(treeView1.SelectedNode.Text);
            }
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) // No Item selected to delete
                return;
            if (tabControl1.SelectedIndex != 1) // // Do this only in Designer-Mode 
            {
                if (MyGlobaleVariables.PropertyGridHasFocus == false) // And do not do this if propertygrid has focus
                {
                    if (MiExperimentalDeleteMode.Checked == false)
                    deleteSelectedElement();
                else
                    SearchCheckedElement(treeView1.Nodes[0]);
                }
                    
            }
        }
        private void deleteSelectedElement()
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (treeView1.SelectedNode == treeView1.Nodes[0])
            {
                MessageBox.Show(GetTranslation("Deleting main node is not allowed!"), GetTranslation("Information"),MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            if (selectedNode != null)
            {
                pQueue.clearUndo(); // We need to clear the UndoList. The problem is that we readded elements an these elements now have different hashes :-(

                XmlNode node = pXmlHandler.getXmlNode(selectedNode); // The element we want to delete
                int nodeHash = pXmlHandler.getHash(node);
                XmlNode parentNode = node.ParentNode;
                int parentNodeHash = pXmlHandler.getHash(parentNode);

                cCommandQueue.cCommand cmd = new cCommandQueue.cCommand("deleteElement");
                cmd.DoEvent += new cCommandQueue.EventHandler(eventDoElement);
                cmd.UndoEvent += new cCommandQueue.EventHandler(eventUndoElement);

                cmd.Helper = parentNodeHash;
                cmd.From = parentNode.OuterXml; // For from we need the parent outerxml, for to we dont need it
                replaceXmlNode(nodeHash, ""); // Delete Element
                cmd.To = parentNode.OuterXml; // Set the result

                MyGlobaleVariables.UnsafedChanges = true;
                pQueue.addSilentCmd(cmd);
            }
        }
        private void SearchCheckedElement(TreeNode node)
        {
            if (node != null)
            {
                foreach (TreeNode TN in node.Nodes)
                {
                    if (TN != null && TN.Checked == true)
                    {
                        deleteCheckedElement(TN);
                        SearchCheckedElement(treeView1.Nodes[0]);
                        return;
                    }
                    else
                    {
                        foreach (TreeNode TN1 in TN.Nodes)
                            if (TN1 != null && TN1.Checked == true)
                            {
                                deleteCheckedElement(TN1);
                                SearchCheckedElement(treeView1.Nodes[0]);
                                return;
                            }
                            else
                            {
                                foreach (TreeNode TN2 in TN1.Nodes)
                                    if (TN2 != null && TN2.Checked == true)
                                    {
                                        deleteCheckedElement(TN2);
                                        SearchCheckedElement(treeView1.Nodes[0]);
                                        return;
                                    }
                            }
                    }

                        
                }
            }
            

        }
        private void deleteCheckedElement(TreeNode node)
        {
            pQueue.clearUndo(); // We need to clear the UndoList. The problem is that we readded elements an these elements now have different hashes :-(
            XmlNode xnode = pXmlHandler.getXmlNode(node); // The element we want to delete
            int nodeHash = pXmlHandler.getHash(xnode);
            XmlNode parentNode = xnode.ParentNode;
            int parentNodeHash = pXmlHandler.getHash(parentNode);
            cCommandQueue.cCommand cmd = new cCommandQueue.cCommand("deleteElement");
            cmd.DoEvent += new cCommandQueue.EventHandler(eventDoElement);
            cmd.UndoEvent += new cCommandQueue.EventHandler(eventUndoElement); 
            
            cmd.Helper = parentNodeHash;
            cmd.From = parentNode.OuterXml; // For from we need the parent outerxml, for to we dont need it
            replaceXmlNode(nodeHash, ""); // Delete Element
            cmd.To = parentNode.OuterXml; // Set the result
            
            MyGlobaleVariables.UnsafedChanges = true;
            pQueue.addSilentCmd(cmd);
        }
        private void eventDoElement(cCommandQueue.cCommand sender, EventArgs e)
        {
            int hash = (int)sender.Helper;
            replaceXmlNode(hash, sender.To as String);

            TreeNode focusNode = pXmlHandler.getTreeNode(hash);
            if (focusNode != null)
                treeView1.SelectedNode = focusNode;

            pQueue.clearRedo(); // We need to clear the RedoList. The problem is that we readded elements an these elements now have different hashes :-(
        }
        private void eventUndoElement(cCommandQueue.cCommand sender, EventArgs e)
        {
            int hash = (int)sender.Helper;
            replaceXmlNode(hash, sender.From as String);

            TreeNode focusNode = pXmlHandler.getTreeNode(hash);
            if (focusNode != null)
                treeView1.SelectedNode = focusNode;
        }
        private void MiPreferences_Click(object sender, EventArgs e)
        {
            new fPreferences().Show();
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {

        }
        private void replaceXmlNode(int hash, String xml)
        {
            pXmlHandler.XmlReplaceNodeAndChilds(hash, xml);
            refresh();
            refreshPropertyGrid();
            //pXmlHandler.XmlSyncTreeChilds(treeView1.SelectedNode.GetHashCode(), treeView1.SelectedNode); //TODO: check
        }
        private void btnSaveEditor_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {

                int hash = treeView1.SelectedNode.GetHashCode();
                try
                {
                    Point p = new Point(0, 0);

                    if (Platform.sysPlatform == Platform.ePlatform.MONO)
                    {
                        p = textBoxEditor.AutoScrollOffset;
                        //pXmlHandler.XmlReplaceNode(hash, textBoxEditor.Text);
                        replaceXmlNode(hash, textBoxEditor.Text);
                    }
                    else
                    {
                        p = textBoxEditor2.AutoScrollOffset;
                        //pXmlHandler.XmlReplaceNode(hash, textBoxEditor2.Text);
                        replaceXmlNode(hash, textBoxEditor2.Text);
                    }

                    TslStatus.Text = GetTranslation("No Errors.");


                    if (Platform.sysPlatform == Platform.ePlatform.MONO)
                    {
                        textBoxEditor.AutoScrollOffset = p;
                    }
                    else
                    {
                        textBoxEditor2.AutoScrollOffset = p;
                    }

                    pXmlHandler.XmlSyncTreeChilds(treeView1.SelectedNode.GetHashCode(), treeView1.SelectedNode);
                    MyGlobaleVariables.UnsafedChangesEditor = false;
                    MyGlobaleVariables.UnsafedChanges = true;
                    pQueue.clear();
                }
                catch (Exception ex)
                {
                    string fixNewLine = ex.Message;
                    if (fixNewLine.Contains("\r"))
                    {
                        fixNewLine = fixNewLine.Replace("\r", "Return");
                    }
                    TslStatus.Text = GetTranslation("Error:") + " " + fixNewLine;
                }
            }
            else
                TslStatus.Text = GetTranslation("Error:") + " " + GetTranslation("No element selected");
        }
        private void btnFading_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("fading", btnFading.Checked);

            pictureBox1.Invalidate();
        }
        private void MiClose_Click(object sender, EventArgs e)
        {
            bool result = CheckChangesEditorNew();
            if (result == false)
            {
                return;
            }
            else
            {
                result = CheckChangesNew();
                if (result == false)
                {
                    return;
                }

            }
            close();
        }

        private Int32 _StartX = 0;
        private Int32 _StartY = 0;
        private Boolean mouseDown = false;
        private Boolean isResize = false;
        private Boolean isResizeW = false;
        private Boolean isResizeE = false;
        private Boolean isResizeN = false;
        private Boolean isResizeS = false;
        //sAttribute _Attr = null;

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private Size remeberAttrSizeForUndo;
        private sAttribute.Position remeberAttrPositionForUndo;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //System.Console.WriteLine("pictureBox1_MouseDown");
            if (e.Button == MouseButtons.Left)
            {
                _StartX = (int)(((MouseEventArgs)e).X / pDesigner.zoomLevel());
                _StartY = (int)(((MouseEventArgs)e).Y / pDesigner.zoomLevel());

                sGraphicElement elem = pDesigner.getElement((uint)_StartX, (uint)_StartY);
                if (elem != null)
                {
                    sAttribute _Attr = elem.pAttr;
                    if (_Attr != null)
                    {
                        /*if (isResize)
                        {
                            mouseDown = true;
                        }
                        else*/
                        if (inBounds(new PointF(_StartX, _StartY), _Attr.Rectangle, -2 / pDesigner.zoomLevel()))
                        {
                            treeView1.SelectedNode = pXmlHandler.getTreeNode(_Attr.myNode);

                            mouseDown = true;
                            this.Cursor = Cursors.SizeAll;
                        }
                        else if (inBounds(new PointF(_StartX, _StartY), _Attr.Rectangle, +2 / pDesigner.zoomLevel()))
                        {
                            treeView1.SelectedNode = pXmlHandler.getTreeNode(_Attr.myNode); // CHECK: Hopefully this fixes the bug in designer were an command ist set without any change.

                            mouseDown = true;
                            //this.Cursor = Cursors.SizeAll;
                        }

                        if (mouseDown)
                        {
                            remeberAttrSizeForUndo = _Attr.Size;
                            remeberAttrPositionForUndo = _Attr.Relativ;
                            Console.Out.WriteLine("Size {0}", remeberAttrSizeForUndo);
                        }
                    }
                }

                tabControl1.Focus();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (propertyGrid1.SelectedObject == null)
                    return;
                if (propertyGrid1.SelectedObject.GetType() != typeof(sAttributeScreen))
                    if ((propertyGrid1.SelectedObject as sAttribute).Parent != null)
                    {
                        //propertyGrid1.SelectedObject = (propertyGrid1.SelectedObject as sAttribute).Parent;
                        treeView1.SelectedNode = pXmlHandler.getTreeNode((propertyGrid1.SelectedObject as sAttribute).Parent.myNode);
                        //refresh();
                        //pDesigner.sort();
                        //refreshEditor();
                        //propertyGrid1.Refresh();
                        //pictureBox1.Invalidate();
                    }
            }
        }
        private bool inRange(float myx, float targetX, float margin)
        {
            float targetXMax = targetX + margin;
            float targetXMin = targetX - margin;
            if (myx > targetXMin && myx < targetXMax)
                return true;
            return false;
        }
        private bool inBounds(PointF myx, RectangleF target, float margin)
        {
            //System.Console.WriteLine("{0} {1} {2}", myx, target, margin);
            RectangleF targetMax = new RectangleF(target.X - margin, target.Y - margin, target.Width + 5 * margin, target.Height + 5 * margin);
            //targetMin = new Rectangle(target.X - margin, target.Y - margin, target.Width - margin, target.Height - margin);

            return targetMax.Contains(myx);
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //System.Console.WriteLine("pictureBox1_MouseMove {0} {1}", ((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            Int32 curX = (int)(((MouseEventArgs)e).X / pDesigner.zoomLevel());
            Int32 curY = (int)(((MouseEventArgs)e).Y / pDesigner.zoomLevel());
            //System.Console.WriteLine("{0} {1}", curX, curY);
            if (mouseDown)
            {
                if (propertyGrid1.SelectedObject != null)
                {
                    //Console.WriteLine(_Attr.pRelativX + "+(" + curX + "-" + _StartX + ")");
                    if (curX != _StartX || curY != _StartY)
                    {
                        if (isResize)
                        {
                            sAttribute _Attr = (sAttribute)propertyGrid1.SelectedObject;
                            Size size = new Size();
                            size.Width = _Attr.pWidth;
                            size.Height = _Attr.pHeight;

                            if (isResizeW || isResizeE)
                            {
                                Int32 sizeX = (Int32)(_Attr.pWidth + (curX - _StartX));
                                if (isResizeE)
                                    size.Width = (Int32)sizeX;
                                else
                                {
                                    Int32 posX = (Int32)(_Attr.pRelativX + (curX - _StartX));
                                    Int32 posY = (Int32)_Attr.pRelativY;
                                    sAttribute.Position pos = new sAttribute.Position();
                                    pos.X = ((Int32)posX).ToString();
                                    pos.Y = ((Int32)posY).ToString();
                                    _Attr.Relativ = pos;

                                    sizeX = (Int32)(_Attr.pWidth + (_StartX - curX));
                                    size.Width = (Int32)sizeX;
                                }
                            }
                            if (isResizeN || isResizeS)
                            {
                                Int32 sizeY = (Int32)(_Attr.pHeight + (curY - _StartY));
                                if (isResizeS)
                                    size.Height = (Int32)sizeY;
                                else
                                {
                                    Int32 posX = (Int32)_Attr.pRelativX;
                                    Int32 posY = (Int32)(_Attr.pRelativY + (curY - _StartY));
                                    sAttribute.Position pos = new sAttribute.Position();
                                    pos.X = ((Int32)posX).ToString();
                                    pos.Y = ((Int32)posY).ToString();
                                    _Attr.Relativ = pos;

                                    sizeY = (Int32)(_Attr.pHeight + (_StartY - curY));
                                    size.Height = (Int32)sizeY;
                                }

                            }


                            /* TODO: If we would not set it here directly to Relative but to X and Y the value would only be
                             * temprarly saved, and so we could set it finaly in mouse up, this has an advantage cause we 
                             * could easier implement a UNDO functionality.
                             */
                            _Attr.Size = size;
                        }
                        else
                        {
                            sAttribute _Attr = (sAttribute)propertyGrid1.SelectedObject;
                            Int32 posX = (Int32)(_Attr.pRelativX + (curX - _StartX));
                            Int32 posY = (Int32)(_Attr.pRelativY + (curY - _StartY));
                            sAttribute.Position pos = new sAttribute.Position();

                            pos.X = ((Int32)posX).ToString();
                            pos.Y = ((Int32)posY).ToString();


                            /* TODO: If we would not set it here directly to Relative but to X and Y the value would only be
                             * temprarly saved, and so we could set it finaly in mouse up, this has an advantage cause we 
                             * could easier implement a UNDO functionality.
                             */
                            _Attr.Relativ = pos;
                        }

                        propertyGrid1.Refresh();
                        sAttribute subattr = (sAttribute)propertyGrid1.SelectedObject;
                        pDesigner.redrawFog((int)subattr.pAbsolutX, (int)subattr.pAbsolutY, (int)subattr.pWidth, (int)subattr.pHeight);
                        MyGlobaleVariables.UnsafedChanges = true;
                        pictureBox1.Invalidate();
                    }
                }
            }
            else
            {
                sAttribute _Attr = (sAttribute)propertyGrid1.SelectedObject;
                if (_Attr != null)
                {
                    if (inBounds(new PointF(curX, curY), _Attr.Rectangle, 2 / pDesigner.zoomLevel()))
                    {
                        isResize = true;
                        if (inRange(curX, _Attr.Absolut.X, 2))
                        {
                            this.Cursor = Cursors.SizeWE;
                            isResizeW = true;
                        }
                        else if (inRange(curX, _Attr.Absolut.X + _Attr.Size.Width, 2 / pDesigner.zoomLevel()))
                        {
                            this.Cursor = Cursors.SizeWE;
                            isResizeE = true;
                        }
                        else if (inRange(curY, _Attr.Absolut.Y, 2 / pDesigner.zoomLevel()))
                        {
                            this.Cursor = Cursors.SizeNS;
                            isResizeN = true;
                        }
                        else if (inRange(curY, _Attr.Absolut.Y + _Attr.Size.Height, 2 / pDesigner.zoomLevel()))
                        {
                            this.Cursor = Cursors.SizeNS;
                            isResizeS = true;
                        }
                        else
                        {
                            this.Cursor = Cursors.Help;
                            isResize = false;
                            isResizeW = false;
                            isResizeE = false;
                            isResizeN = false;
                            isResizeS = false;
                        }
                    }
                    else
                    {
                        this.Cursor = Cursors.Default;
                        isResize = false;
                    }
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    isResize = false;
                }

            }
            _StartX = curX;
            _StartY = curY;
        }
        private void pictureBox1_MouseUp(object s, MouseEventArgs e)
        {
            //System.Console.WriteLine("pictureBox1_MouseUp");
            if (mouseDown)
            {
                cCommandQueue.cCommand cmd = new cCommandQueue.cCommand("MouseChange");

                cmd.Helper = propertyGrid1.SelectedObject;

                cmd.DoEvent += new cCommandQueue.EventHandler(eventDoPropertyGrid);
                cmd.UndoEvent += new cCommandQueue.EventHandler(eventUndoPropertyGrid);

                if ((propertyGrid1.SelectedObject as sAttribute).Size != remeberAttrSizeForUndo)
                {
                    cmd.From = new Object[] { "Size", remeberAttrSizeForUndo };
                    cmd.To = new Object[] { "Size", (propertyGrid1.SelectedObject as sAttribute).Size };
                    pQueue.addCmd(cmd);
                }
                else if ((propertyGrid1.SelectedObject as sAttribute).Relativ.X != remeberAttrPositionForUndo.X ||
                    (propertyGrid1.SelectedObject as sAttribute).Relativ.Y != remeberAttrPositionForUndo.Y)
                {
                    cmd.From = new Object[] { "Relativ", remeberAttrPositionForUndo };
                    cmd.To = new Object[] { "Relativ", (propertyGrid1.SelectedObject as sAttribute).Relativ };
                    pQueue.addCmd(cmd);
                }
            }
            mouseDown = false;
            isResize = false;
            this.Cursor = Cursors.Default;
        }
        private void pictureBox1_MouseLeave(object s, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
        private bool isCTRL(KeyEventArgs e)
        {
            return e.Control;
        }
        private bool isUP(KeyEventArgs e)
        {
            return e.KeyCode == Keys.Up;
        }
        private bool isDOWN(KeyEventArgs e)
        {
            return e.KeyCode == Keys.Down;
        }
        private bool isLEFT(KeyEventArgs e)
        {
            return e.KeyCode == Keys.Left;
        }
        private bool isRIGHT(KeyEventArgs e)
        {
            return e.KeyCode == Keys.Right;
        }
        private bool isCURSOR(KeyEventArgs e)
        {
            return (isUP(e) || isDOWN(e) || isLEFT(e) || isRIGHT(e));
        }
        private bool isPLUS(KeyEventArgs e)
        {
            return ((int)e.KeyCode) == 107;
        }
        private bool isMINUS(KeyEventArgs e)
        {
            return ((int)e.KeyCode) == 109;
        }
        private bool isF11(KeyEventArgs e)
        {
            return e.KeyCode == Keys.F11;
        }
        private bool isF10(KeyEventArgs e)
        {
            return e.KeyCode == Keys.F10;
        }
        private bool isEntf(KeyEventArgs e)
        {
            return e.KeyCode == Keys.Delete;
        }
        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!this.keyCapture)
                return;

            // If CTRL pressed, use margin 1, else margin 5
            //Console.WriteLine(e.Control.ToString());
            if (isCURSOR(e))
            {
                int marging = 5;

                if (isCTRL(e))
                    marging = 1;

                if (propertyGrid1.SelectedObject != null)
                {
                    int x = isLEFT(e) ? -marging : (isRIGHT(e) ? +marging : 0);
                    int y = isUP(e) ? -marging : (isDOWN(e) ? +marging : 0);

                    sAttribute _Attr = (sAttribute)propertyGrid1.SelectedObject;
                    Int32 posX = (Int32)(_Attr.pRelativX + x);
                    Int32 posY = (Int32)(_Attr.pRelativY + y);
                    if (posX < 0) posX = 0;
                    if (posY < 0) posY = 0;

                    sAttribute.Position pos = new sAttribute.Position();

                    pos.X = ((UInt32)posX).ToString();
                    pos.Y = ((UInt32)posY).ToString();

                    cCommandQueue.cCommand cmd = new cCommandQueue.cCommand("KeyboardChange");

                    cmd.Helper = propertyGrid1.SelectedObject;

                    cmd.DoEvent += new cCommandQueue.EventHandler(eventDoPropertyGrid);
                    cmd.UndoEvent += new cCommandQueue.EventHandler(eventUndoPropertyGrid);

                    cmd.From = new Object[] { "Relativ", _Attr.Relativ };
                    cmd.To = new Object[] { "Relativ", pos };
                    pQueue.addCmd(cmd);

                    _Attr.Relativ = pos;

                    propertyGrid1.Refresh();
                    sAttribute subattr = (sAttribute)propertyGrid1.SelectedObject;
                    pDesigner.redrawFog((int)subattr.pAbsolutX, (int)subattr.pAbsolutY, (int)subattr.pWidth, (int)subattr.pHeight);
                    pictureBox1.Invalidate();
                    MyGlobaleVariables.UnsafedChanges = true;
                }


                e.Handled = true;
            }
            else if (isPLUS(e) || isMINUS(e))
            {
                if (isPLUS(e))
                    pDesigner.zoomIn();
                else
                    pDesigner.zoomOut();
                //pictureBox1.Scale(new SizeF((float)0.5, (float)0.5));
                //
                //pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                panelDesignerInner.AutoScrollMinSize = new Size((int)(cDataBase.pResolution.getResolution().Xres * pDesigner.zoomLevel()), (int)(cDataBase.pResolution.getResolution().Yres * pDesigner.zoomLevel()));
                trackBarZoom.Value = (int)((pDesigner.zoomLevel() - 1.0f) * 100.0f);
                pictureBox1.Invalidate();
            }
            /*else if (e.KeyCode == Keys.Z) //UNDO
            {
                pQueue.undoCmd();
            }
            else if (e.KeyCode == Keys.Y) //REDO
            {
                pQueue.redoCmd();
            }*/
        }
        private void tabControl1_Enter(object sender, EventArgs e)
        {
            lbxSearch.Visible = false;
            tbxTreeFilter.Text = GetTranslation("Search...");
            if (sender is TabControl)
            {
                if (((TabControl)sender).SelectedIndex == 0)
                {
                    this.keyCaptureNotifyButton.Image = global::OpenSkinDesigner.Properties.Resources.Lock_icon;
                    this.keyCapture = true;
                }
            }
            else if (sender is TabPage)
            {
                if (((TabPage)sender).Name.Equals("tabPage1"))
                {
                    this.keyCaptureNotifyButton.Image = global::OpenSkinDesigner.Properties.Resources.Lock_icon;
                    this.keyCapture = true;
                }
            }
        }
        private void tabControl1_Leave(object sender, EventArgs e)
        {
            lbxSearchCodeEditor.Visible = false;
            tbxSearchCode.Text = GetTranslation("Search...");
            //if (((TabPage)sender).Name.Equals("tabPage1"))
            {
                this.keyCaptureNotifyButton.Image = global::OpenSkinDesigner.Properties.Resources.UnLock_icon;
                this.keyCapture = false;
                this.Cursor = Cursors.Default;
            }
        }
        
        private bool _bFullScreenMode = false;
        private Form previewForm = null;
        private int x, y, w, h;

        private void toggleFullscreen()
        {
            if (_bFullScreenMode == false)
            {
                x = this.Left;
                y = this.Top;
                w = this.Width;
                h = this.Height;
                this.FormBorderStyle = FormBorderStyle.None;
                menuStrip1.Visible = false;
                toolStripMain.Visible = false;
                this.Left = 0;
                this.Top = 0;
                this.Width = Screen.PrimaryScreen.Bounds.Width;
                this.Height = Screen.PrimaryScreen.Bounds.Height;
                _bFullScreenMode = true;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                toolStripMain.Visible = true;
                menuStrip1.Visible = true;
                this.Left = x;
                this.Top = y;
                this.Width = w;
                this.Height = h;
                _bFullScreenMode = false;
            }
        }
        private void togglePreviewFullscreen()
        {
            if (previewForm == null)
            {
                previewForm = new Form();
                previewForm.FormBorderStyle = FormBorderStyle.None;
                previewForm.TopMost = true;
                previewForm.Left = 0;
                previewForm.Top = 0;
                previewForm.Width = Screen.PrimaryScreen.Bounds.Width;
                previewForm.Height = Screen.PrimaryScreen.Bounds.Height;
                previewForm.BackColor = Color.Black;
                previewForm.Controls.Add(pictureBox1);
                pictureBox1.Size = new Size((int)cDataBase.pResolution.getResolution().Xres, (int)cDataBase.pResolution.getResolution().Yres);// new Size(1280, 720);
                previewForm.KeyUp += fMain_KeyUp;
                previewForm.Visible = true;
            }
            else
            {
                panelDesignerInner.Controls.Add(pictureBox1);
                previewForm.Visible = false;
                previewForm = null;
            }
        }
        private void fMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (isF11(e))
                toggleFullscreen();
            else if (isF10(e))
                togglePreviewFullscreen();
            else if (isEntf(e))
            {
                if (treeView1.SelectedNode == null) // Nothing selected to delete
                    return;
                if (tabControl1.SelectedIndex != 1) // Do this only in Designer-Mode 
                {
                    if (MyGlobaleVariables.PropertyGridHasFocus == false) // And do not do this if propertygrid has focus
                    {
                        if (MiExperimentalDeleteMode.Checked == false)
                            deleteSelectedElement();
                        else
                            SearchCheckedElement(treeView1.Nodes[0]);
                    }


                }
            }
                
                

                    
        }
        private void btnSkinned_Alpha_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("enable_alpha", btnSkinned_Alpha.Checked);

            pictureBox1.Invalidate();
        }
        private void setZoom(float zoom)
        {
            pDesigner.setZoomLevel(zoom);
            pictureBox1.Invalidate();

            panelDesignerInner.AutoScrollMinSize = new Size((int)(cDataBase.pResolution.getResolution().Xres * pDesigner.zoomLevel()), (int)(cDataBase.pResolution.getResolution().Yres * pDesigner.zoomLevel()));
        }
        private void trackBarZoom_ValueChanged(object sender, EventArgs e)
        {
            setZoom(((System.Windows.Forms.TrackBar)sender).Value / 100.0f + 1.0f);
            numericUpDownZoom.Value = ((System.Windows.Forms.TrackBar)sender).Value;
        }
        private void numericUpDownZoom_ValueChanged(object sender, EventArgs e)
        {
            trackBarZoom.Value = (int)((System.Windows.Forms.NumericUpDown)sender).Value;
        }
        private void reloadConverterxmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Structures.cConverter.init();
            propertyGrid1.Refresh();
            pictureBox1.Invalidate();
        }
        private void reloadPreviewTextxmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Structures.cPreviewText.init();
            propertyGrid1.Refresh();
            pictureBox1.Invalidate();
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pQueue.undoCmd();
            MyGlobaleVariables.UnsafedChanges = true;
        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pQueue.redoCmd();
            MyGlobaleVariables.UnsafedChanges = true;
        }
        private void tbxTreeFilter_Enter(object sender, EventArgs e)
        {
            if (tbxTreeFilter.Text == GetTranslation("Search..."))
            {
                tbxTreeFilter.Text = "";
            }
        }
        private void FoldAll(object sender, EventArgs e)
        {
            foreach (ScintillaNET.Line linear in this.textBoxEditor2.Lines)
            {
                if (linear.IsFoldPoint && linear.FoldExpanded && (linear.Text.Contains("<screen") || linear.Text.Contains("<output") || linear.Text.Contains("<colors") || linear.Text.Contains("<fonts") || linear.Text.Contains("<subtitles") || linear.Text.Contains("<windowstyle")))
                {
                    linear.ToggleFoldExpanded();
                }
            }
        }
        private void UnfoldAll(object sender, EventArgs e)
        {
            foreach (ScintillaNET.Line linear in this.textBoxEditor2.Lines)
            {
                if (linear.IsFoldPoint && !linear.FoldExpanded && (linear.Text.Contains("<screen") || linear.Text.Contains("<output") || linear.Text.Contains("<colors") || linear.Text.Contains("<fonts") || linear.Text.Contains("<subtitles") || linear.Text.Contains("<windowstyle")))
                {
                    linear.ToggleFoldExpanded();
                }
            }
        }
        private void FoldAllWidgets(object sender, EventArgs e)
        {
            foreach (ScintillaNET.Line linear in this.textBoxEditor2.Lines)
            {
                if (linear.IsFoldPoint && linear.FoldExpanded && (linear.Text.Contains("<widget") || linear.Text.Contains("<convert")))
                {
                    linear.ToggleFoldExpanded();
                }
            }
        }
        private void UnfoldAllWidgets(object sender, EventArgs e)
        {
            foreach (ScintillaNET.Line linear in this.textBoxEditor2.Lines)
            {
                if (linear.IsFoldPoint && !linear.FoldExpanded && (linear.Text.Contains("<widget") || linear.Text.Contains("<convert")))
                {
                    linear.ToggleFoldExpanded();
                }
            }
        }
        private void doSearch(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.ToLower().Contains(tbxTreeFilter.Text.ToLower()))
                {
                    lbxSearch.Items.Add(node.Text);
                    lbxSearch.Height += lbxSearch.ItemHeight;
                }
                else
                {
                    //XmlNode xnode = pXmlHandler.getXmlNode(node);
                    //if (xnode != null && xnode.Name == "include")
                    if (node.Tag != null)
                        doSearch(node.Nodes);
                }
                if (lbxSearch.Height + lbxSearch.ItemHeight > treeView1.Height)
                    break;
            }
        }
        private void doSearchCode()
        {
            int count = 0;
            string linecount = string.Empty;
            if (!textBoxEditor2.Text.Contains(""))
            {

                lbxSearchCodeEditor.Visible = false;
                return;
            }

            foreach (ScintillaNET.Line linear in this.textBoxEditor2.Lines)
            {
                count += 1;
                if (linear.Text.Contains(tbxSearchCode.Text))
                {
                    linecount = count.ToString();
                    while (linecount.ToString().Length < 7)
                    {
                        linecount = "0" + linecount;
                    }       
                   
                    lbxSearchCodeEditor.Items.Add(linecount + " " + linear.Text);
                    lbxSearchCodeEditor.Height += lbxSearchCodeEditor.ItemHeight;
                }
                if (lbxSearchCodeEditor.Height + lbxSearchCodeEditor.ItemHeight > textBoxEditor2.Height)
                    break;               
                   
            }
            if (lbxSearchCodeEditor.Items.Count == 0)
                lbxSearchCodeEditor.Visible = false;
            else
                lbxSearchCodeEditor.Visible = true;
        }
        private void tbxTreeFilter_TextChanged(object sender, EventArgs e)
        {
            if (tbxTreeFilter.Text != "" && tbxTreeFilter.Text != GetTranslation("Search...") && tbxTreeFilter.Text.Length >= 3)
            {
                lbxSearch.Width = treeView1.Width - 4;
                lbxSearch.Height = 4;
                lbxSearch.Items.Clear();
                lbxSearch.Visible = true;
                doSearch(treeView1.Nodes[0].Nodes);
            }
            else
            {
                lbxSearch.Visible = false;
            }
        }
        private void tbxTreeFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                lbxSearch.Visible = false;
                tbxTreeFilter.Text = "";
            }
        }
        private void btnEditRoot_Click(object sender, EventArgs e)
        {

            if (treeView1.Nodes.Count > 0)
            {
                treeView1.GetNodeAt(0, 0).Expand();
                treeView1.SelectedNode = treeView1.Nodes[0];
            }

            foreach (ScintillaNET.Line linear in this.textBoxEditor2.Lines)
            {
                if (linear.IsFoldPoint && linear.FoldExpanded && (linear.Text.Contains("<screen") || linear.Text.Contains("<output") || linear.Text.Contains("<colors") || linear.Text.Contains("<fonts") || linear.Text.Contains("<subtitles") || linear.Text.Contains("<windowstyle")))
                {
                    linear.ToggleFoldExpanded();
                }
            }
            tabControl1.SelectedIndex = 1;
        }
        private TreeNode findItem(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text == lbxSearch.SelectedItem.ToString())
                {
                    return node;
                }
                else
                {
                    //XmlNode xnode = pXmlHandler.getXmlNode(node);
                    //if (xnode != null && xnode.Name == "include")
                    if (node.Tag != null)
                    {
                        TreeNode inode = findItem(node.Nodes);
                        if (inode != null)
                            return inode;
                    }
                }
            }
            return null;
        }
        private void lbxSearch_Click(object sender, EventArgs e)
        {
            if (tbxTreeFilter.Text != "")
            {
                TreeNode node = findItem(treeView1.Nodes[0].Nodes);
                if (node != null)
                    treeView1.SelectedNode = node;
            }
            lbxSearch.Height = 4;
            lbxSearch.Items.Clear();
            lbxSearch.Visible = false;
            refresh();
        }
        private void autoComplete(object sender, ScintillaNET.CharAddedEventArgs e)
        {
            ScintillaNET.Scintilla txt = (ScintillaNET.Scintilla)sender;
            if (e.Ch == '<')
            {
                if (Properties.Settings.Default.showAttributList == true)
                    showElementsList(txt);
            } 
            else if (e.Ch == ' ')
            {
                if (Properties.Settings.Default.showAttributList == true)
                        showAttrList(txt);
            }
                    
            MyGlobaleVariables.UnsafedChangesEditor = true;
        }
        private void showAttrList(ScintillaNET.Scintilla txt)
        {
            List<string> words = new List<string>();
            words.Add("position");
            words.Add("size");
            words.Add("title");
            words.Add("flags");
            words.Add("backgroundColor");
            words.Add("name");

            words.Add("alphatest");
            words.Add("font");
            words.Add("text");
            words.Add("zPosition");
            words.Add("borderWidth");
            words.Add("borderColor");

            words.Add("foregroundColor");
            words.Add("pixmap");
            words.Add("halign");
            words.Add("valign");
            words.Add("noWrap");

            words.Add("transparent");
            words.Add("source");
            words.Add("render");
            words.Add("scrollbarMode");
            words.Add("enableWrapAround");
            words.Add("orientation");

            words.Add("textOffset");
            words.Add("backgroundColorSelected");
            words.Add("foregroundColorSelected");
            words.Add("shadowColor");
            words.Add("selectionDisabled");
            words.Add("itemHeight");

            words.Add("pointer");
            words.Add("seek_pointer");
            words.Add("shadowOffset");
            words.Add("scale");
            words.Add("scrollbarbackgroundPixmap");
            words.Add("sliderPixmap");

            words.Add("selectionPixmap");
            words.Add("backgroundPixmap");
            words.Add("value");
            words.Add("xres");
            words.Add("yres");
            words.Add("bpp");

            words.Add("filename");
            words.Add("pos");
            words.Add("foregroundColors");
            words.Add("options");
            words.Add("pixmaps");

            if (Properties.Settings.Default.useFullAttribList == true)
            {
                words.Add("type");
                words.Add("backgroundColorRows");
                words.Add("descriptionColor");
                words.Add("secondfont");
                words.Add("scrollbarBackgroundPicture");
                words.Add("scrollbarWidth");
                words.Add("scrollbarSliderBorderWidth");
                words.Add("scrollbarKeepGapColor");
                words.Add("scrollbarGap");
                words.Add("padding");
                words.Add("spacing");
                words.Add("Animation");
                words.Add("NoAnimationAfter");
                words.Add("serviceItemHeight");
                words.Add("serviceNumberFont");
                words.Add("serviceNameFont");
                words.Add("serviceInfoFont");
                words.Add("itemWidth");
                words.Add("colorServiceDescriptionSelected");
                words.Add("serviceNumberAlign");
                words.Add("serviceNameAlign");
                words.Add("serviceTimeAlign");
                words.Add("serviceInfoAlign");
                words.Add("onlyFullListEntries");
                words.Add("boxOrientation");
                words.Add("scrollbarOrientation");
                words.Add("selectionOffset");
                words.Add("colorServiceDescription");
                words.Add("foregroundColorServiceNotAvail");
                words.Add("colorEventProgressbarBorder");
                words.Add("colorEventProgressbarBorderSelected");
                words.Add("progressbarHeight");
                words.Add("progressbarBorderWidth");
                words.Add("lines");
                words.Add("TimeStringColor");
                words.Add("TimeColor");
                words.Add("FontSize1");
                words.Add("FontSize2");
                words.Add("EntryBorderColor");
                words.Add("EntryBackgroundColor");
                words.Add("EntryBackgroundColorSelected");
                words.Add("descriptionfont");
                words.Add("animationMode");
                words.Add("scrollbarSliderForegroundColor");
                words.Add("scrollbarSliderBorderColor");
                words.Add("imageType");
                words.Add("path");
                words.Add("viewMode");
                words.Add("overlayImage");
                words.Add("itemScale");
                words.Add("itemSpace");
                words.Add("aspectRatio");
                words.Add("textColor");
                words.Add("panelColor");
                words.Add("dimensions");
                words.Add("useShadow");
                words.Add("useOverlay");
                words.Add("backgroundColorGlobal");
                words.Add("ServiceNameForegroundColor");
                words.Add("ServiceNameBackgroundColor");
                words.Add("EntryForegroundColor");
                words.Add("colorServiceWithAdvertisment");
                words.Add("EntryNowForegroundColor");
                words.Add("EntryNowBackgroundColor");
                words.Add("EntryForegroundColorSelected");
                words.Add("colorEventProgressbar");
                words.Add("EntryNowForegroundColorSelected");
                words.Add("EntryNowBackgroundColorSelected");
                words.Add("EntryRecColor");
                words.Add("EntryPreColor");
                words.Add("EntryRecIncompleteColor");
                words.Add("EntryRecOffColor");
                words.Add("piconWidth");
                words.Add("foregroundColorMarked");
                words.Add("foregroundColorMarkedSelected");
                words.Add("backgroundColorMarked");
                words.Add("backgroundColorMarkedSelected");
                words.Add("delay");
                words.Add("speed");
                words.Add("animated");
                words.Add("pixtype");
                words.Add("plugin_pixmaps");
                words.Add("colorEventProgressbarSelected");
				words.Add("backgroundGradient");
				words.Add("itemGradientSelected");
				words.Add("scrollbarForegroundGradient");
				words.Add("cornerRadius");
				words.Add("scrollbarSliderBackgroundColor");
				words.Add("scrollbarOffset");
				words.Add("scrollbarMode");
				
				
            }
				//Kitte888
            words.Sort();
            txt.AutoComplete.List = words;
            txt.AutoComplete.IsCaseSensitive = false;
            txt.AutoComplete.DropRestOfWord = false;
            txt.AutoComplete.Show();
        }
        private void setFallbackColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            colorDialog1.Color = Properties.Settings.Default.FallbackColor;
            dr = colorDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Properties.Settings.Default.FallbackColor = colorDialog1.Color;
                Properties.Settings.Default.Save();
                MiSetFallbackColor.BackColor = Properties.Settings.Default.FallbackColor;
            }
                
        }
        private void useFullAttributlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.useFullAttribList = MiUseFullAttributlist.Checked;
            Properties.Settings.Default.Save();
        }
        private void textBoxEditor2_TextChanged(object sender, EventArgs e)
        {
            MyGlobaleVariables.UnsafedChangesEditor = true;
        }
        private void MiNew_Click(object sender, EventArgs e)
        {

        }
        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (MyGlobaleVariables.UnsafedChangesEditor == true)
            {
                bool result = CheckChangesEditorNew();
                if (result == false)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    result = CheckChangesNew();
                    if (result == false)
                    {
                        e.Cancel = true;
                        return;
                    }

                }
                MyGlobaleVariables.UnsafedChangesEditor = false;
            }
        }
        private void UseCustomLanguage()
        {
            MiAddElement.Text = GetTranslation("Add Element");
            MiExperimentalDeleteMode.Text = GetTranslation("Experimental delete-mode");
            MiLinewrapping.Text = GetTranslation("Linewrapping");
            MiFile.Text = GetTranslation("File");
            MiNew.Text = GetTranslation("New");
            MiOpen.Text = GetTranslation("Open");
            MiSave.Text = GetTranslation("Save");
            MiSaveAs.Text = GetTranslation("Save as");
            MiClose.Text = GetTranslation("Close");
            MiExit.Text = GetTranslation("Exit");
            MiEdit.Text = GetTranslation("Edit");
            MiRedo.Text = GetTranslation("Redo");
            MiUndo.Text = GetTranslation("Undo");
            MiElement.Text = GetTranslation("Element");
            MiAddLabel.Text = GetTranslation("Add Label");
            MiAddPixmap.Text = GetTranslation("Add Pixmap");
            MiAddWidget.Text = GetTranslation("Add Widget");
            MiDontReplaceColors.Text = GetTranslation("Dont replace colors beginning with '#'");
            MiDeleteSelected.Text = GetTranslation("Delete selected");
            MiSettings.Text = GetTranslation("Settings");
            MiResolution.Text = GetTranslation("Resolution");
            MiColors.Text = GetTranslation("Colors");
            MiFonts.Text = GetTranslation("Fonts");
            MiWindowStyles.Text = GetTranslation("WindowStyles");
            MiPreferences.Text = GetTranslation("Preferences");
            MiReloadConverterXml.Text = GetTranslation("Reload converter.xml");
            MiReloadPreviewTextXml.Text = GetTranslation("Reload previewText.xml");
            MiAddUndefinedColors.Text = GetTranslation("Add undefined color with '#' instead of 'un'");
            MiSetFallbackColor.Text = GetTranslation("Set 'Fallback'-Color");
            MiUseFullAttributlist.Text = GetTranslation("Use full attribut-list");
            MiShowAttributList.Text = GetTranslation("Show attribut-list");
            MiLanguage.Text = GetTranslation("Language");
            MiShowNotificationUnsafedChanges.Text = GetTranslation("Notification of unsaved changes");
            MiShowNotificationUnsafedChangesEditor.Text = GetTranslation("Notification of unsaved changes in editor");
            btnOpen.Text = GetTranslation("Open");
            btnSave.Text = GetTranslation("Save");
            btnAddLabel.Text = GetTranslation("Add Label");
            btnAddPanel.Text = GetTranslation("Add Panel");
            btnAddPixmap.Text = GetTranslation("Add Pixmap");
            btnAddScreen.Text = GetTranslation("Add Screen");
            btnAddWidget.Text = GetTranslation("Add Widget");
            btnDelete.Text = GetTranslation("Delete Element");
            btnEditRoot.Text = GetTranslation("Edit Root");
            btnUndo.Text = GetTranslation("Undo");
            btnRedo.Text = GetTranslation("Redo");
            keyCaptureNotifyButton.Text = GetTranslation("Capturing Keyboard Events");
            tbxTreeFilter.Text = GetTranslation("Search...");
            tbxSearchCode.Text = GetTranslation("Search...");
            tabControl1.TabPages[0].Text = GetTranslation("Designer");
            tabControl1.TabPages[1].Text = GetTranslation("Code");
            btnSkinned.Text = GetTranslation("Show all");
            TslMarker.Text = GetTranslation("Marker");
            btnSkinned_Alpha.Text = GetTranslation("Alpha");
            btnFading.Text = GetTranslation("Fading");
            btnSkinnedShowPanels.Text = GetTranslation("Show panels");
            btnSkinnedLabel.Text = GetTranslation("Show labels");
            btnSkinnedPixmap.Text = GetTranslation("Show pixmaps");
            btnSkinnedScreen.Text = GetTranslation("Show screens");
            btnSkinnedWidget.Text = GetTranslation("Show widgets");
            TslConditionalWidgets.Text = GetTranslation("Conditional widgets");
            btnSkinned_Conditional_All.Text = GetTranslation("All");
            btnSkinned_Conditional_Default.Text = GetTranslation("Default");
            btnSkinned_Conditional_None.Text = GetTranslation("None");
            btnSkinned_Conditional_Random.Text = GetTranslation("Random");
            TslStatus.Text = GetTranslation("No Errors.");
            btnSaveEditor.Text = GetTranslation("Sync");
            btnFoldOff.Text = GetTranslation("Sync");
            btnFoldOn.Text = GetTranslation("Sync");
            btnFoldElementsOff.Text = GetTranslation("Sync");
            btnFoldElementsOn.Text = GetTranslation("Sync");
            foreach (ToolStripMenuItem t in MiAddElement.DropDownItems)
            {
                t.Text = GetTranslation(t.Tag.ToString());
                foreach (ToolStripMenuItem ts in t.DropDownItems)
                {
                    ts.Text = GetTranslation(ts.Tag.ToString());
                }
            }
            //trackBarZoom.Left = btnSkinned_Conditional_None.Left + btnSkinned_Conditional_None.Width + 5;
            //numericUpDownZoom.Left = trackBarZoom.Right + 5;
        }
        public static string GetTranslation(string Word)
        {
            if (Properties.Settings.Default.language== null)
                return Word;
            string tmp = Array.Find(MyGlobaleVariables.Language, element => element.StartsWith(Word + ": "));
            if (tmp == null) // Maybe adding a notification if a translation is not existing
                return Word;
            string translation = tmp.Substring(tmp.IndexOf(Word + ": ") + Word.Length + 2);
            return translation;
        }
        private void TSAddUndefinedColors_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.addUndefinedColor = MiAddUndefinedColors.Checked;
            Properties.Settings.Default.Save();
        }
        private void tbxSearchCode_Enter(object sender, EventArgs e)
        {
            if (tbxSearchCode.Text == GetTranslation("Search..."))
            {
                tbxSearchCode.Text = "";
            }
        }
        private void tbxSearchCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                lbxSearchCodeEditor.Visible = false;
                tbxSearchCode.Text = "";
            }


            if (e.KeyCode == Keys.Return)
            {
                lbxSearchCodeEditor.Visible = true;
                if (textBoxEditor2.Text != null && textBoxEditor2.Text != "")
                {
                    lbxSearchCodeEditor.Height = 18;
                    lbxSearchCodeEditor.Items.Clear();
                    doSearchCode();
                }
                else
                {
                    lbxSearchCodeEditor.Visible = false;
                }
            }

        }
        private void tbxSearchCode_TextChanged(object sender, EventArgs e)
        {
            lbxSearchCodeEditor.Visible = false;
        }
        private void lbxSearchCodeEditor_DoubleClick(object sender, EventArgs e)
        {
            if (tbxSearchCode.Text != "")
            {
                string linecount = lbxSearchCodeEditor.SelectedItem.ToString().Substring(0,7);
                while (linecount.Substring(0,1) == "0")
                {
                    linecount = linecount.Substring(1);
                }
                textBoxEditor2.Lines[Int32.Parse(linecount) - 1].Select();
                
            }
            lbxSearchCodeEditor.Height = 18;
            lbxSearchCodeEditor.Items.Clear();
            lbxSearchCodeEditor.Visible = false;
        }
        private void textBoxEditor2_Click(object sender, EventArgs e)
        {
            lbxSearch.Visible = false;
            tbxTreeFilter.Text = GetTranslation("Search...");
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbxSearch.Visible = false;
            tbxTreeFilter.Text = GetTranslation("Search...");
        }
        private void fMain_SizeChanged(object sender, EventArgs e)
        {
            CenterLabel();
        }
        private void fMain_ResizeEnd(object sender, EventArgs e)
        {
            CenterLabel();
        }
        private void CenterLabel()
        {
            // This.Width 1190 = lblSkinNAme.Width 740

            lblSkinName.Width = this.Width - 450;
            lblSkinName.Text = null;
            lblSkinName.Text = cProperties.getProperty("path_skin");
        }
        private void MiShowNotificationUnsafedChanges_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowChanges = MiShowNotificationUnsafedChanges.Checked;
            Properties.Settings.Default.Save();
        }
        private void MiShowNotificationUnsafedChangesEditor_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowChangesEditor = MiShowNotificationUnsafedChangesEditor.Checked;
            Properties.Settings.Default.Save();
        }
        private void MiExperimentalDeleteMode_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ExperimentalDelete = MiExperimentalDeleteMode.Checked;
            Properties.Settings.Default.Save();
            treeView1.CheckBoxes = Properties.Settings.Default.ExperimentalDelete;
        }
        private void MiLinewrapping_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LineWrapping = MiLinewrapping.Checked;
            Properties.Settings.Default.Save();
            SetLineWrapping();            
        }
        private void SetLineWrapping()
        {
            switch (Properties.Settings.Default.LineWrapping)
            {
                case true:
                    textBoxEditor2.LineWrapping.Mode = LineWrappingMode.Word;
                    break;
                case false:
                    textBoxEditor2.LineWrapping.Mode = LineWrappingMode.None;
                    break;
            }
        }
        private void propertyGrid1_Enter(object sender, EventArgs e)
        {
            MyGlobaleVariables.PropertyGridHasFocus = true;
        }
        private void propertyGrid1_Leave(object sender, EventArgs e)
        {
            MyGlobaleVariables.PropertyGridHasFocus = false;
        }

        private void MiDontReplaceColors_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DontReplaceColors = MiDontReplaceColors.Checked;
            Properties.Settings.Default.Save();
        }

        private void MiShowAttributList_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.showAttributList = MiShowAttributList.Checked;
            Properties.Settings.Default.Save();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            textBoxEditor2.Styles.LineNumber.BackColor = Color.LightGray;
        }
        private void showElementsList(ScintillaNET.Scintilla txt)
        {
            List<string> words = new List<string>();
            words.Add("widget");
            words.Add("ePixmap");
            words.Add("eLabel");
            words.Add("applet");
            words.Add("panel");
            words.Add("convert");

            words.Add("color");
            words.Add("font");
            words.Add("sub");
            words.Sort();
            txt.AutoComplete.List = words;
            txt.AutoComplete.IsCaseSensitive = false;
            txt.AutoComplete.DropRestOfWord = false;
            txt.AutoComplete.Show();
        }
        private void btnSkinnedShowPanels_Click(object sender, EventArgs e)
        {
            cProperties.setProperty("skinned_panels", btnSkinnedShowPanels.Checked);
            refresh();
        }

    }
}
