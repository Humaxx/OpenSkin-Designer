using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
using OpenSkinDesigner.Logic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace OpenSkinDesigner.Structures
{
    class sGraphicImage : sGraphicElement
    {
        protected Image pImage;

        //protected sAttribute pAttr;

        public sGraphicImage(sAttribute attr, String image, Int32 x, Int32 y, Int32 w, Int32 h)
            : base(attr)
        {
            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicImage - String image - oben x y w h () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion


            //Console.WriteLine("sGraphicImage: " + x + ":" + y + " " + w + "x" + h);
            if (cDataBase.getPath(image).ToLower().EndsWith(".svg"))
                image = cDataBase.getPNGPath(image);
            try
            {
                pImage = Image.FromFile(cDataBase.getPath(image));
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("File not found! (" + cDataBase.getPath(image) + ")\n" + ex);
                return;
            }
            pX = x;
            pY = y;
            
            pWidth = w < (Int32)pImage.Width ? w : (Int32)pImage.Width;
            pHeight = h < (Int32)pImage.Height ? h : (Int32)pImage.Height;
        }
            
        

        public sGraphicImage(sAttribute attr, String image, Int32 x, Int32 y)
            : base(attr)
        {
            //Console.WriteLine("sGraphicImage: " + x + ":" + y);

            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicImage - String image - mitte x y w h () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion


            //pAttr = attr;
            if (image == null || image.Length == 0)
                return;
            if (image.ToUpper().EndsWith(".SVG"))
            {
                image = image.ToUpper().Replace(".SVG", ".PNG");
            }
            try
            {
                pImage = Image.FromFile(cDataBase.getPath(image));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found! (" + cDataBase.getPath(image) + ")");
                return;
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Unsupported (" + cDataBase.getPath(image) + ")");
                return;
            }

            pX = x;
            pY = y;

            pWidth = (Int32)pImage.Width;
            pHeight = (Int32)pImage.Height;
        }

        public sGraphicImage(sAttribute attr, String image)
            : base(attr)
        {
            //Console.WriteLine("sGraphicImage: ");

            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicImage - String image - unten x y w h () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion



            //pAttr = attr;
            if (image == null || image.Length == 0)
                return;
            try
            {
                
                pImage = Image.FromFile(cDataBase.getPath(image));
                Logger.LogMessage("============= sGraphicImage - path unten     : " + pImage);

                // Check correct type
                if (attr.GetType() == typeof(sAttributePixmap))
                {
                    //get size of root element (= size of a widget)
                    sAttributePixmap element = (sAttributePixmap)attr;
                    Size elementSize = element.pPixmap;

                    if (elementSize != null)
                    {
                        //Resize image, if imageSize and element size is diffrent
                        if (elementSize.Width != pImage.Size.Width || elementSize.Height != pImage.Height)
                        {
                            if (element.myNode.Attributes["pixmap"] != null || element.myNode.Attributes["pixmaps"] != null)
                            {
                                // ePixmap or widget element with attribute 'pixmap' (= path to image)
                                pImage = ResizeImage(pImage, elementSize.Width, elementSize.Height);
                            }
                            else if (element.myNode.Attributes["render"] != null && element.myNode.Attributes["render"].Value.ToLower().Contains("picon"))
                            {
                                // is picon
                                pImage = ResizeImageKeepAspectRatio(pImage, elementSize.Width, elementSize.Height);
                            }
                            else if (element.myNode.Attributes["render"] != null && element.myNode.Attributes["render"].Value.ToLower().Contains("eventimage"))
                            {
                                // is EventImage
                                pImage = ResizeImageKeepAspectRatio(pImage, elementSize.Width, elementSize.Height);
                            }
                            else if (element.myNode.Attributes["path"] != null)
                            {
                                //widget element with attribute 'path' (= path to image)
                                pImage = ResizeImage(pImage, elementSize.Width, elementSize.Height);
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found! (" + cDataBase.getPath(image) + ")");
                return;
            }
        }


        public override void paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //if (!cProperties.getPropertyBool("skinned"))
            //{
            //    new sGraphicRectangel(pAttr, false, (float)1.0, Color.Red).paint(sender, e);
            //}
            //else

            if (pImage != null)
            {
                Graphics g = e.Graphics;
                g.DrawImageUnscaledAndClipped(pImage, new Rectangle((int)pX, (int)pY, pWidth < pImage.Width ? (int)pWidth : pImage.Width, pHeight < pImage.Height ? (int)pHeight : pImage.Height));
            }
        }

        private Image ResizeImage(Image imgToResize, int Width, int Height)
        {
            return (Image)(new Bitmap(imgToResize, new Size(Width, Height)));
        }

        private Image ResizeImageKeepAspectRatio(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //grPhoto.Clear(Color.Red);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}
