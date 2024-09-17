using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
using OpenSkinDesigner.Logic;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace OpenSkinDesigner.Structures
{
    class sAttributePixmap : sAttribute
    {
        private const String entryName = "Pixmap";

        public String pPixmapName;
        public Size pPixmap;
        public bool pHide = false;
        //public Image pPixmap = null;

        // #####################################
        //public float pCornerRadius;
        // #####################################

        public cProperty.eAlphatest pAlphatest = cProperty.eAlphatest.off;

        [CategoryAttribute(entryName)]
        public String Path
        {
            get { return pPixmapName; }
            set
            {
                pPixmapName = value;
                if (pPixmapName != null && pPixmapName.Length > 0)
                {
                    if (myNode.Attributes["pixmap"] != null)
                        myNode.Attributes["pixmap"].Value = pPixmapName;
                    else
                    {
                        myNode.Attributes.Append(myNode.OwnerDocument.CreateAttribute("pixmap"));
                        myNode.Attributes["pixmap"].Value = pPixmapName;
                    }
                }
                else
                    if (myNode.Attributes["pixmap"] != null)
                    myNode.Attributes.RemoveNamedItem("pixmap");
            }
        }

        [CategoryAttribute(entryName),
        ReadOnlyAttribute(true)]
        public Size Resolution
        {
            get { return new Size(pPixmap.Width, pPixmap.Height); }
        }

        [TypeConverter(typeof(cProperty.AlphatestConverter)),
        CategoryAttribute(entryName)]
        public String Alphatest
        {
            get { return pAlphatest.ToString(); }
            set
            {
                if (value != null && value == cProperty.eAlphatest.on.ToString()) pAlphatest = cProperty.eAlphatest.on;
                else if (value != null && value == cProperty.eAlphatest.blend.ToString()) pAlphatest = cProperty.eAlphatest.blend;
                else pAlphatest = cProperty.eAlphatest.off;

                if (myNode.Attributes["alphatest"] != null)
                    myNode.Attributes["alphatest"].Value = "off";
                else
                {
                    myNode.Attributes.Append(myNode.OwnerDocument.CreateAttribute("alphatest"));
                    myNode.Attributes["alphatest"].Value = "off";
                }

                if (pAlphatest == cProperty.eAlphatest.on) myNode.Attributes["alphatest"].Value = "on";
                else if (pAlphatest == cProperty.eAlphatest.blend) myNode.Attributes["alphatest"].Value = "blend";
                else myNode.Attributes["alphatest"].Value = "off";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="node"></param>

        ~sAttributePixmap()
        {
            //if (pPixmap != null) pPixmap.Dispose();
        }

        public sAttributePixmap(sAttribute parent, XmlNode node)
            : base(parent, node)
        {

            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"§§§§§§§§§§§§§§ sAttributePixmap - BackgroundColor () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion


            // ________________________________________________________________________________________________________________


            if (myNode.Attributes["cornerRadius"] != null)
            {
                string value = myNode.Attributes["cornerRadius"].Value;

                Logger.LogMessage("§§§§§§§§§§§§§§ cAttributePixmap.cs - cornerRadius ist: " + value);

                float.TryParse(value, out pCornerRadius);
            }




            // ePixmap or widget element with attribute 'pixmap' (= path to image)
            if (node.Attributes["pixmap"] != null)
            {
                pPixmapName = node.Attributes["pixmap"].Value;
                if (pPixmapName.ToLower().EndsWith(".svg"))
                    pPixmapName = pPixmapName.ToLower().Replace(".svg", ".png");
                try
                {
                    //PVMC Workaround
                    if (pPixmapName.Contains("ProjectValerie"))
                    {
                        pPixmapName = pPixmapName.Substring(pPixmapName.IndexOf("ProjectValerie/skins/") + "ProjectValerie/skins/".Length);
                    }
                    Image pixmap = Image.FromFile(cDataBase.getPath(pPixmapName));

                    // Element has scale attribute -> take size attribute
                    if (node.Attributes["scale"] != null)
                    {
                        pPixmap = new Size(this.Size.Width, this.Size.Height);
                    }
                    else
                    {
                        pPixmap = pixmap.Size;
                    }
                    pixmap.Dispose();
                }
                catch (FileNotFoundException)
                {
                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + "broken.png", true);
                    pPixmap = pixmap.Size;
                    pixmap.Dispose();
                    pPixmapName = "@broken.png";
                }
                catch (OutOfMemoryException) // If Pixmap isn't a image or broken
                {
                    MessageBox.Show("File: '" + pPixmapName + "' seems to be corrupt!","File corrupt?",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + "broken.png", true);
                    pPixmap = pixmap.Size;
                    pixmap.Dispose();
                    pPixmapName = "@broken.png";
                }
            }
            else if (node.Attributes["pixmaps"] != null)
            {
                pPixmapName = node.Attributes["pixmaps"].Value.Split(',')[0];
                try
                {
                    if (pPixmapName.ToLower().EndsWith(".svg"))
                        pPixmapName = pPixmapName.ToLower().Replace(".svg", ".png");
                    Image pixmap = Image.FromFile(cDataBase.getPath(pPixmapName));

                    // Element has scale attribute -> take size attribute
                    if (node.Attributes["scale"] != null)
                    {
                        pPixmap = new Size(this.Size.Width, this.Size.Height);
                    }
                    else
                    {
                        pPixmap = pixmap.Size;
                    }
                    pixmap.Dispose();
                }
                catch (FileNotFoundException)
                {
                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + "broken.png", true);
                    pPixmap = pixmap.Size;
                    pixmap.Dispose();
                    pPixmapName = "@broken.png";
                }
                catch (OutOfMemoryException) // If Pixmap isn't a image or broken
                {
                    MessageBox.Show("File: '" + pPixmapName + "' seems to be corrupt!", "File corrupt?", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + "broken.png", true);
                    pPixmap = pixmap.Size;
                    pixmap.Dispose();
                    pPixmapName = "@broken.png";
                }
            }
            else if (node.Attributes["path"] != null)
            {
                //Any Render that use path attribute
                String path = node.Attributes["path"].Value;
                try
                {
                    // take random picture in path
                    string[] filePaths = null;
                    try
                    {
                        filePaths = Directory.GetFiles(@cDataBase.getPath(path));
                    }
                    catch
                    {
                        // Bug Fix: if path contains skin-path 
                        try
                        {
                            path = path.Replace(cProperties.getProperty("path_skin"), "");
                            filePaths = Directory.GetFiles(@cDataBase.getPath(path));
                        }
                        catch
                        {

                        }
                    }

                    if (filePaths != null && filePaths.Length > 0) // MOD
                    {
                        Random rand = new Random();
                        int rnd = 0;
                        string ext = string.Empty;
                        int count = 0;
                        while (ext != ".jpg" && ext!=".png" && ext != ".jpeg" && count !=50) // Only take images, try for 50 times to find a image
                        {
                            rnd = rand.Next(0, filePaths.Length);
                            ext = System.IO.Path.GetExtension(filePaths[rnd]).ToLower();
                            count++;
                        }
                        
                        pPixmapName = cProperties.getProperty("path_skin").Replace("./", "\\").Replace("/", "\\") + "/" + path + "/" + System.IO.Path.GetFileName(filePaths[rnd]);
                        Image pixmap = Image.FromFile(cDataBase.getPath(pPixmapName));
                        // Element has scale attribute -> take size attribute
                        if (node.Attributes["scale"] != null)
                        {
                            pPixmap = new Size(this.Size.Width, this.Size.Height);
                        }
                        else
                        {
                            pPixmap = pixmap.Size;
                        }
                        pixmap.Dispose();
                    }
                    else
                    {
                        Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + "broken.png", true);
                        pPixmap = pixmap.Size;
                        pixmap.Dispose();
                        pPixmapName = "@broken.png";
                    }

                }
                catch (FileNotFoundException)
                {
                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + "broken.png", true);
                    pPixmap = pixmap.Size;
                    pixmap.Dispose();
                    pPixmapName = "@broken.png";
                }
                catch (OutOfMemoryException) // If Pixmap isn't a image or broken
                {
                    MessageBox.Show("File: '" + pPixmapName + "' seems to be corrupt!", "File corrupt?", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + "broken.png", true);
                    pPixmap = pixmap.Size;
                    pixmap.Dispose();
                    pPixmapName = "@broken.png";
                }
            }
            else if (node.Attributes["render"] != null && node.Attributes["render"].Value.ToLower().Contains("picon"))
            {
                try
                {
                    String piconFileName = "picon.png";

                    if (node.Attributes["render"].Value.ToLower().Contains("xpicon"))
                    {
                        piconFileName = "xpicon.png";
                    }
                    else if (node.Attributes["render"].Value.ToLower().Contains("xhdpicon"))
                    {
                        piconFileName = "xhdpicon.png";
                    }

                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + piconFileName, true);

                    // size of root element (= size of a widget)
                    pPixmap = new Size(this.Size.Width, this.Size.Height);

                    pixmap.Dispose();
                    pPixmapName = "@" + piconFileName;
                }
                catch (FileNotFoundException)
                {
                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + "broken.png", true);
                    pPixmap = pixmap.Size;
                    pixmap.Dispose();
                    pPixmapName = "@broken.png";
                }
               
            }
           
            else if (node.Attributes["render"] != null && node.Attributes["render"].Value.ToLower().Contains("eventimage"))
            {
                try
                {
                    String eventImageFileName = "eventimage.jpg";

                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + eventImageFileName, true);

                    // size of root element (= size of a widget)
                    pPixmap = new Size(this.Size.Width, this.Size.Height);

                    pixmap.Dispose();
                    pPixmapName = "@" + eventImageFileName;
                }
                catch (FileNotFoundException)
                {
                    Image pixmap = Image.FromFile(Application.StartupPath + cProperties.getProperty("path_skins").Replace("./", "\\").Replace("/", "\\") + "broken.png", true);
                    pPixmap = pixmap.Size;
                    pixmap.Dispose();
                    pPixmapName = "@broken.png";
                }
                
            }


                if (node.Attributes["alphatest"] != null)
                pAlphatest = node.Attributes["alphatest"].Value.ToLower() == "on" ? cProperty.eAlphatest.on :
                    node.Attributes["alphatest"].Value.ToLower() == "blend" ? cProperty.eAlphatest.blend :
                    cProperty.eAlphatest.off;
        }
    }
}
