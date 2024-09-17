using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using OpenSkinDesigner.Logic;
using System.IO;
using System.Diagnostics;


namespace OpenSkinDesigner.Structures
{
    public class sWindowStyle
    {
        public class sBorderSet
        {
            public String pName;

            public enum ePosition
            {
                bpTopLeft, bpTop, bpTopRight,
                bpLeft, bpRight,
                bpBottomLeft, bpBottom, bpBottomRight
            };

            public String pbpTopLeftName;
            public String pbpTopName;
            public String pbpTopRightName;
            public String pbpLeftName;
            public String pbpRightName;
            public String pbpBottomLeftName;
            public String pbpBottomName;
            public String pbpBottomRightName;

            public Size pbpTopLeft;
            public Size pbpTop;
            public Size pbpTopRight;
            public Size pbpLeft;
            public Size pbpRight;
            public Size pbpBottomLeft;
            public Size pbpBottom;
            public Size pbpBottomRight;

            public sBorderSet(String name,
                                String bpTopLeftName,
                                String bpTopName,
                                String bpTopRightName,
                                String bpLeftName,
                                String bpRightName,
                                String bpBottomLeftName,
                                String bpBottomName,
                                String bpBottomRightName)
            {
                pName = name;

                pbpTopLeftName = bpTopLeftName;
                pbpTopName = bpTopName;
                pbpTopRightName = bpTopRightName;
                pbpLeftName = bpLeftName;
                pbpRightName = bpRightName;
                pbpBottomLeftName = bpBottomLeftName;
                pbpBottomName = bpBottomName;
                pbpBottomRightName = bpBottomRightName;

                // Hole den Namen der aufrufenden Methode
                string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
                // Log-Nachricht erstellen
                string logMessage = $"============= sWindowsStyle -  () wurde von {callerName} aufgerufen .";
                // Loggen
                Logger.LogMessage(logMessage);
                // Weiter mit der eigentlichen Funktion


                // MOD: fixed Unhandled Exception when a file(borderset) not found... And display a message
                int anz = 0;
                if (pbpTopLeftName.Length > 0 && !System.IO.File.Exists(cDataBase.getPath(pbpTopLeftName)))
                {
                    anz += 1;
                }
                if (pbpTopName.Length > 0 && !System.IO.File.Exists(cDataBase.getPath(pbpTopName)))
                {
                    anz += 1;
                }
                if (pbpTopRightName.Length > 0 && !System.IO.File.Exists(cDataBase.getPath(pbpTopRightName)))
                {
                    anz += 1;
                }
                if (pbpLeftName.Length > 0 && !System.IO.File.Exists(cDataBase.getPath(pbpLeftName)))
                {
                    anz += 1;
                }
                if (pbpRightName.Length > 0 && !System.IO.File.Exists(cDataBase.getPath(pbpRightName)))
                {
                    anz += 1;
                }
                if (pbpBottomLeftName.Length > 0 && !System.IO.File.Exists(cDataBase.getPath(pbpBottomLeftName)))
                {
                    anz += 1;
                }
                if (pbpBottomName.Length > 0 && !System.IO.File.Exists(cDataBase.getPath(pbpBottomName)))
                {
                    anz += 1;
                }
                if (pbpBottomRightName.Length > 0 && !System.IO.File.Exists(cDataBase.getPath(pbpBottomRightName)))
                {
                    anz += 1;
                }
                if (anz > 0)
                {
                    MessageBox.Show("Some borderset-files from '" + pName + "' are not existing!" + "\n\n",
                    "File(s) not found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
                string PNGPath = "";
                bool shown = false; // Only show ones
                if (pbpTopLeftName.Length > 0 && System.IO.File.Exists(cDataBase.getPath(pbpTopLeftName)))
                {
                    if (cDataBase.getPath(pbpTopLeftName).ToLower().EndsWith(".svg"))
                    {
                        if (shown == false)
                        {
                            shown = true;
                            MessageBox.Show("'.svg'-graphics can't be displayed by OSD, searching for '.png' graphics instead.","svg-graphics",
                            MessageBoxButtons.OK,MessageBoxIcon.Information );
                        }
                        
                        PNGPath = cDataBase.getPNGPath(cDataBase.getPath(pbpTopLeftName));
                        if (File.Exists(PNGPath))
                        {
                            Image pixmap = Image.FromFile(PNGPath);
                            pbpTopLeft = pixmap.Size;
                            pixmap.Dispose();
                        }
                        //Application.Exit();
                    }
                    else
                    {
                        Image pixmap = Image.FromFile(cDataBase.getPath(pbpTopLeftName));
                        pbpTopLeft = pixmap.Size;
                        pixmap.Dispose();
                    }
                        
                }
                if (pbpTopName.Length > 0 && System.IO.File.Exists(cDataBase.getPath(pbpTopName)))
                {
                    if (cDataBase.getPath(pbpTopName).ToLower().EndsWith(".svg"))
                    {
                        if (shown == false)
                        {
                            shown = true;
                            MessageBox.Show("'.svg'-graphics can't be displayed by OSD, searching for '.png' graphics instead.", "svg-graphics",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        PNGPath = cDataBase.getPNGPath(cDataBase.getPath(pbpTopName));
                        if (File.Exists(PNGPath))
                        {
                            Image pixmap = Image.FromFile(PNGPath);
                            pbpTop = pixmap.Size;
                            pixmap.Dispose();
                        }
                        //Application.Exit();
                    }
                    else
                    {
                        Image pixmap = Image.FromFile(cDataBase.getPath(pbpTopName));
                        pbpTop = pixmap.Size;
                        pixmap.Dispose();
                    }
                    
                }
                if (pbpTopRightName.Length > 0 && System.IO.File.Exists(cDataBase.getPath(pbpTopRightName)))
                {
                    if (cDataBase.getPath(pbpTopRightName).ToLower().EndsWith(".svg"))
                    {
                        if (shown == false)
                        {
                            shown = true;
                            MessageBox.Show("'.svg'-graphics can't be displayed by OSD, searching for '.png' graphics instead.", "svg-graphics",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        PNGPath = cDataBase.getPNGPath(cDataBase.getPath(pbpTopRightName));
                        if (File.Exists(PNGPath))
                        {
                            Image pixmap = Image.FromFile(PNGPath);
                            pbpTopRight = pixmap.Size;
                            pixmap.Dispose();
                        }
                        //Application.Exit();
                    }
                    else
                    {
                        Image pixmap = Image.FromFile(cDataBase.getPath(pbpTopRightName));
                        pbpTopRight = pixmap.Size;
                        pixmap.Dispose();
                    }
                    
                }


                if (pbpLeftName.Length > 0 && System.IO.File.Exists(cDataBase.getPath(pbpLeftName)))
                {
                    if (cDataBase.getPath(pbpLeftName).ToLower().EndsWith(".svg"))
                    {
                        if (shown == false)
                        {
                            shown = true;
                            MessageBox.Show("'.svg'-graphics can't be displayed by OSD, searching for '.png' graphics instead.", "svg-graphics",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        PNGPath = cDataBase.getPNGPath(cDataBase.getPath(pbpLeftName));
                        if (File.Exists(PNGPath))
                        {
                            Image pixmap = Image.FromFile(PNGPath);
                            pbpLeft = pixmap.Size;
                            pixmap.Dispose();
                        }
                        //Application.Exit();
                    }
                    else
                    {
                        Image pixmap = Image.FromFile(cDataBase.getPath(pbpLeftName));
                        pbpLeft = pixmap.Size;
                        pixmap.Dispose();
                    }
                    
                }
                if (pbpRightName.Length > 0 && System.IO.File.Exists(cDataBase.getPath(pbpRightName)))
                {
                    if (cDataBase.getPath(pbpRightName).ToLower().EndsWith(".svg"))
                    {
                        if (shown == false)
                        {
                            shown = true;
                            MessageBox.Show("'.svg'-graphics can't be displayed by OSD, searching for '.png' graphics instead.", "svg-graphics",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        PNGPath = cDataBase.getPNGPath(cDataBase.getPath(pbpRightName));
                        if (File.Exists(PNGPath))
                        {
                            Image pixmap = Image.FromFile(PNGPath);
                            pbpRight = pixmap.Size;
                            pixmap.Dispose();
                        }
                        //Application.Exit();
                    }

                    else
                    {
                        Image pixmap = Image.FromFile(cDataBase.getPath(pbpRightName));
                        pbpRight = pixmap.Size;
                        pixmap.Dispose();
                    }
                    
                }


                if (pbpBottomLeftName.Length > 0 && System.IO.File.Exists(cDataBase.getPath(pbpBottomLeftName)))
                {
                    if (cDataBase.getPath(pbpBottomLeftName).ToLower().EndsWith(".svg"))
                    {
                        if (shown == false)
                        {
                            shown = true;
                            MessageBox.Show("'.svg'-graphics can't be displayed by OSD, searching for '.png' graphics instead.", "svg-graphics",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        PNGPath = cDataBase.getPNGPath(cDataBase.getPath(pbpBottomLeftName));
                        if (File.Exists(PNGPath))
                        {
                            Image pixmap = Image.FromFile(PNGPath);
                            pbpBottomLeft = pixmap.Size;
                            pixmap.Dispose();
                        }
                        //Application.Exit();
                    }
                    else
                    {
                        Image pixmap = Image.FromFile(cDataBase.getPath(pbpBottomLeftName));
                        pbpBottomLeft = pixmap.Size;
                        pixmap.Dispose();
                    }
                    
                }
                if (pbpBottomName.Length > 0 && System.IO.File.Exists(cDataBase.getPath(pbpBottomName)))
                {
                    if (cDataBase.getPath(pbpBottomName).ToLower().EndsWith(".svg"))
                        
                    {
                        if (shown == false)
                        {
                            shown = true;
                            MessageBox.Show("'.svg'-graphics can't be displayed by OSD, searching for '.png' graphics instead.", "svg-graphics",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        PNGPath = cDataBase.getPNGPath(cDataBase.getPath(pbpBottomName));
                        if (File.Exists(PNGPath))
                        {
                            Image pixmap = Image.FromFile(PNGPath);
                            pbpBottom = pixmap.Size;
                            pixmap.Dispose();
                        }
                        // Application.Exit();
                    }
                    else
                    {
                        Image pixmap = Image.FromFile(cDataBase.getPath(pbpBottomName));
                        pbpBottom = pixmap.Size;
                        pixmap.Dispose();
                    }
                   
                }
                if (pbpBottomRightName.Length > 0 && System.IO.File.Exists(cDataBase.getPath(pbpBottomRightName)))
                {
                    if (cDataBase.getPath(pbpBottomRightName).ToLower().EndsWith(".svg"))
                    {
                        if (shown == false)
                        {
                            shown = true;
                            MessageBox.Show("'.svg'-graphics can't be displayed by OSD, searching for '.png' graphics instead.", "svg-graphics",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        PNGPath = cDataBase.getPNGPath(cDataBase.getPath(pbpBottomRightName));
                        if (File.Exists(PNGPath))
                        {
                            Image pixmap = Image.FromFile(PNGPath);
                            pbpBottomRight = pixmap.Size;
                            pixmap.Dispose();
                        }
                        //Application.Exit();
                        
                        

                    }
                    else
                    {
                        Image pixmap = Image.FromFile(cDataBase.getPath(pbpBottomRightName));
                        pbpBottomRight = pixmap.Size;
                        pixmap.Dispose();
                    }
                    
                }
            }
        }

        public sFont pFont;
        public float pTitleSize;
        public Int32 pXOff;
        public Int32 pYOff;
        public Hashtable pColors;
        public Hashtable pBorderSets;

        public sWindowStyle(sFont font, float titlesize, Int32 xOff, Int32 yOff, Hashtable colors, sBorderSet[] bordersets)
        {
            pFont = font;
            pTitleSize = titlesize;
            pXOff = xOff;
            pYOff = yOff;

            pColors = colors;/*new Hashtable();
            foreach (sColor color in colors)
                pColors.Add(color.pName, color);*/

            pBorderSets = new Hashtable();
            foreach (sBorderSet borderset in bordersets)
                pBorderSets.Add(borderset.pName, borderset);
        }
    }
}