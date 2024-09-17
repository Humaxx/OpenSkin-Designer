using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using OpenSkinDesigner.Logic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace OpenSkinDesigner.Structures
{
    class sGraphicPixmap : sGraphicElement
    {
        //protected sAttributePixmap pAttr;
        
        public sGraphicPixmap(sAttributePixmap attr)
            : base(attr)
        {
            pAttr = attr;
        }

        public override void paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicPixmap - paint () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion




            if (cProperties.getPropertyBool("skinned_pixmap"))
            {
                if (!((sAttributePixmap)pAttr).pHide)
                    Logger.LogMessage("============= sGraphicPixmap - Hide - ruft cGraphicRectangel.cs auf 39  = 4 ");
                    new sGraphicRectangel(pAttr, false, (float)1.0, new sColor(Color.Blue))
                    .withCornerRadius(pAttr.pCornerRadius)
                    .paint(sender, e);
            }

            if (((sAttributePixmap)pAttr).pPixmapName != null)
            {
                if (!((sAttributePixmap)pAttr).pHide)
                    Logger.LogMessage("============= sGraphicPixmap - PixmapName - ruft cGraphicImage.cs auf 46  =  ");
                    new sGraphicImage(pAttr, ((sAttributePixmap)pAttr).pPixmapName)
                    
                    .paint(sender, e);
            }

            if (pAttr.pBorder)
            {
                Logger.LogMessage("============= sGraphicPixmap - Boarder - ruft cGraphicRectangel.cs auf 51  = 4 ");
                new sGraphicRectangel(pAttr, false, (float)pAttr.pBorderWidth, pAttr.pBorderColor)
                    .withCornerRadius(pAttr.pCornerRadius)
                    .paint(sender, e);
            }
                /*else
                 * 
                 * Show missing icon ?
                 */
            
        }
    }
}
