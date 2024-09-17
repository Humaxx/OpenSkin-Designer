using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using OpenSkinDesigner.Logic;
using System.Drawing;
using System.Diagnostics;

namespace OpenSkinDesigner.Structures
{
    class sGraphicLabel : sGraphicElement
    {
        //protected sAttributeLabel pAttr;

        public sGraphicLabel(sAttributeLabel attr)
            : base(attr)
        {
            pAttr = attr;
        }

        public override void paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            sAttributeLabel pAttrLabel = ((sAttributeLabel)pAttr);

            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicLabel - paint () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion






            if (cProperties.getPropertyBool("skinned_label"))
            {
                Logger.LogMessage("============= sGraphicLabel - default color green - ruft cGraphicRectangel.cs auf 40  = 4 ? ");
                new sGraphicRectangel(pAttr, false, (float)1.0, new sColor(Color.Green))
                    .withCornerRadius(pAttrLabel.pCornerRadius)
                    .paint(sender, e);
            }

            if (((sAttributeLabel)pAttr).pText != null || ((sAttributeLabel)pAttr).pPreviewText != null)
            {
                // wenn text vorhanden hier malen , achtung wenn name="" ist wie Text vorhanden 
                String text = "";
                if (((sAttributeLabel)pAttr).pText != null)
                    text = ((sAttributeLabel)pAttr).pText;
                else
                    text = ((sAttributeLabel)pAttr).pPreviewText;

                //Hack
                if (((sAttributeLabel)pAttr).pPreviewText == "MAGIC#TRUE") { }
                //pLabel.pText = "";
                else if (((sAttributeLabel)pAttr).pPreviewText == "MAGIC#FALSE")
                    text = "";

                if (((sAttributeLabel)pAttr).pFont != null)
                {
                    // wenn Transparent kein Background malen
                    if (!pAttr.pTransparent)
                    {

                        if (pAttrLabel.pBackgroundGradient != null)
                        {
                            Logger.LogMessage("============= sGraphicLabel - Text - Transparent false - ruft cGraphicRectangel.cs auf 69  = 2 mit Radius ");
                            new sGraphicRectangel(pAttr, pAttrLabel.pBackgroundGradient)
                                .withCornerRadius(pAttrLabel.pCornerRadius)
                                .paint(sender, e);
                            Logger.LogMessage("============= cGraphicLabel.cs - Text - Transparent false BackgroundGradient aus dem XML-Attribut ist vorhanden");
                        }

                        else
                        {
                            Logger.LogMessage("============= sGraphicLabel - Text - Transparent false BackgroundColor - ruft cGraphicRectangel.cs auf 76  = 4 ? ");
                            new sGraphicRectangel(pAttr, true, 1.0F, ((sAttributeLabel)pAttr).pBackgroundColor)
                                .withCornerRadius(pAttrLabel.pCornerRadius)
                                .paint(sender, e);
                        }
                    }

                    if (pAttr.pTransparent)
                    {
                        // hier Text Transparent malen  mit Foreground
                        Logger.LogMessage("============= sGraphicLabel - pFont ja - ruft cGraphicRectangel.cs auf 62  = 9 ? ");
                        new sGraphicFont(pAttr, pAttr.pAbsolutX, pAttr.pAbsolutY, text, ((sAttributeLabel)pAttr).pFontSize * (((float)((sAttributeLabel)pAttr).pFont.Scale) / 100.0F), ((sAttributeLabel)pAttr).pFont, ((sAttributeLabel)pAttr).pForegroundColor, ((sAttributeLabel)pAttr).pHalign, ((sAttributeLabel)pAttr).pValign).paint(sender, e);
                    }
                    else
                    {
                        // hier Text auch Transparent mit Foreground 
                        Logger.LogMessage("============= sGraphicLabel - pFont nein ruft cGraphicRectangel.cs auf 67  = 10 ? ");
                        //new sGraphicFont(pAttr, pAttr.pAbsolutX, pAttr.pAbsolutY, text, ((sAttributeLabel)pAttr).pFontSize * (((float)((sAttributeLabel)pAttr).pFont.Scale) / 100.0F), ((sAttributeLabel)pAttr).pFont, ((sAttributeLabel)pAttr).pForegroundColor, ((sAttributeLabel)pAttr).pBackgroundColor == null ? new sColor(Color.Black) : ((sAttributeLabel)pAttr).pBackgroundColor, ((sAttributeLabel)pAttr).pHalign, ((sAttributeLabel)pAttr).pValign).paint(sender, e);
                        new sGraphicFont(pAttr, pAttr.pAbsolutX, pAttr.pAbsolutY, text, ((sAttributeLabel)pAttr).pFontSize * (((float)((sAttributeLabel)pAttr).pFont.Scale) / 100.0F), ((sAttributeLabel)pAttr).pFont, ((sAttributeLabel)pAttr).pForegroundColor, ((sAttributeLabel)pAttr).pHalign, ((sAttributeLabel)pAttr).pValign).paint(sender, e);

                    }

                }

            }
            else
            {
                if (!pAttr.pTransparent)
                {

                    // ----------------------------------------- hier wird gemalt wenn kein Text drin ist Gradient oder Background ---------------------------------------
                    if (pAttrLabel.pBackgroundGradient != null)
                    {
                        Logger.LogMessage("============= sGraphicLabel - ruft cGraphicRectangel.cs auf 48  = 2 mit Radius ");
                        new sGraphicRectangel(pAttr, pAttrLabel.pBackgroundGradient)
                            .withCornerRadius(pAttrLabel.pCornerRadius)
                            .paint(sender, e);
                        Logger.LogMessage("============= cGraphicLabel.cs - BackgroundGradient aus dem XML-Attribut ist vorhanden");
                    }

                    else
                    {
                        Logger.LogMessage("============= sGraphicLabel - Transparent - ruft cGraphicRectangel.cs auf 76  = 4 ? ");
                        new sGraphicRectangel(pAttr, true, 1.0F, ((sAttributeLabel)pAttr).pBackgroundColor)
                            .withCornerRadius(pAttrLabel.pCornerRadius)
                            .paint(sender, e);
                    }
                }
            }
        // -------------------- so wird Boarder dann immer gemacht -------------------------------------------------
        if (pAttr.pBorder)
        {
            Logger.LogMessage("============= sGraphicLabel - Border - ruft cGraphicRectangel.cs auf 83  = 4 ? ");
            new sGraphicRectangel(pAttr, false, (float)pAttr.pBorderWidth, pAttr.pBorderColor)
                .withCornerRadius(pAttrLabel.pCornerRadius)
                .paint(sender, e);
        }

            
        }
    }
}
