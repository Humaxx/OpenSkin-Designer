using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
using OpenSkinDesigner.Logic;
using System.Windows.Forms;
using System.Diagnostics;

namespace OpenSkinDesigner.Structures
{
    class sGraphicListbox : sGraphicElement
    {

        // ############################################
        private int scrollbarAbsolutX;
        private int scrollbarAbsolutY;
        private int scrollbarBreite;
        private int scrollbarHeight;
       
        // ############################################

        //protected sAttributeListbox pAttr;

        public sGraphicListbox(sAttributeListbox attr)
            : base(attr)
        {
            pAttr = attr;
        }

        

        public override void paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicListbox - paint () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion

  
            if (!pAttr.pTransparent)
            {
                //Background
                Int32 tx = (Int32)pAttr.pAbsolutX;
                Int32 ty = (Int32)pAttr.pAbsolutY;
                Int32 tw = (Int32)pAttr.pWidth;
                Int32 th = (Int32)pAttr.pHeight;

                if (((sAttributeListbox)pAttr).pBackgroundPixmap != null)
                {

                    new sGraphicImage(pAttr, ((sAttributeListbox)pAttr).pBackgroundPixmapName).paint(sender, e);
                }
                else
                {
                    Logger.LogMessage("============= sGraphicListbox - ruft cGraphicRectangel.cs auf 53  = 7 ");
                    new sGraphicRectangel((Int32)(tx > 0 ? tx : 0), (Int32)(ty > 0 ? ty : 0), (Int32)(tw > 0 ? tw : 0), (Int32)(th > 0 ? th : 0), true, (float)1.0, ((sAttributeListbox)pAttr).pListboxBackgroundColor)
                        
                        .paint(sender, e);
                }
            }

            //BorderLayout
            Int32 x = pAttr.pAbsolutX, xm = pAttr.pAbsolutX + pAttr.pWidth;

            if (((sAttributeListbox)pAttr).pbpTopLeftName != null)
            {
                new sGraphicImage(pAttr,
                    ((sAttributeListbox)pAttr).pbpTopLeftName,
                    x - (Int32)(((sAttributeListbox)pAttr).pbpLeft != null ? ((sAttributeListbox)pAttr).pbpLeft.Width : 0),
                    pAttr.pAbsolutY - (Int32)((sAttributeListbox)pAttr).pbpTopLeft.Height
                    ).paint(sender, e);
                //painter.blit(tl, ePoint(x, pos.top()));
                //x += (UInt32)pAttr.pbpTopLeft.Width;
            }

            if (((sAttributeListbox)pAttr).pbpTopRightName != null)
            {
                //xm -= (UInt32)pAttr.pbpTopRight.Width;
                new sGraphicImage(pAttr,
                    ((sAttributeListbox)pAttr).pbpTopRightName,
                    xm + (Int32)(((sAttributeListbox)pAttr).pbpRight != null ? ((sAttributeListbox)pAttr).pbpRight.Width : 0) - (Int32)((sAttributeListbox)pAttr).pbpTopRight.Width,
                    pAttr.pAbsolutY - (Int32)((sAttributeListbox)pAttr).pbpTopRight.Height
                    ).paint(sender, e);
                //painter.blit(tr, ePoint(xm, pos.top()), pos);
            }

            if (((sAttributeListbox)pAttr).pbpTopName != null)
            {
                x += (Int32)(((sAttributeListbox)pAttr).pbpTopLeft != null ? ((sAttributeListbox)pAttr).pbpTopLeft.Width : 0) - (Int32)(((sAttributeListbox)pAttr).pbpLeft != null ? ((sAttributeListbox)pAttr).pbpLeft.Width : 0);
                int diff = (((sAttributeListbox)pAttr).pbpRight != null ? ((sAttributeListbox)pAttr).pbpRight.Width : 0) - (((sAttributeListbox)pAttr).pbpTopRight != null ? ((sAttributeListbox)pAttr).pbpTopRight.Width : 0);
                xm -= (Int32)(diff > 0 ? diff : -diff);
                while (x < xm)
                {
                    new sGraphicImage(pAttr,
                        ((sAttributeListbox)pAttr).pbpTopName,
                        x,
                        pAttr.pAbsolutY - (Int32)((sAttributeListbox)pAttr).pbpTop.Height,
                        xm - x,
                        (Int32)((sAttributeListbox)pAttr).pbpTop.Height
                        ).paint(sender, e);
                    //painter.blit(t, ePoint(x, pos.top()), eRect(x, pos.top(), xm - x, pos.height()));
                    x += (Int32)((sAttributeListbox)pAttr).pbpTop.Width;
                }
            }

            x = pAttr.pAbsolutX;
            xm = pAttr.pAbsolutX + pAttr.pWidth;

            if (((sAttributeListbox)pAttr).pbpBottomLeftName != null)
            {
                new sGraphicImage(pAttr,
                    ((sAttributeListbox)pAttr).pbpBottomLeftName,
                    x - (Int32)(((sAttributeListbox)pAttr).pbpLeft != null ? ((sAttributeListbox)pAttr).pbpLeft.Width : 0),
                    pAttr.pAbsolutY + pAttr.pHeight
                    ).paint(sender, e);
                //painter.blit(bl, ePoint(pos.left(), pos.bottom()-bl->size().height()));
                //x += (UInt32)pAttr.pbpBottomLeft.Width;
            }

            if (((sAttributeListbox)pAttr).pbpBottomRightName != null)
            {
                //xm -= (UInt32)pAttr.pbpBottomRight.Width;
                new sGraphicImage(pAttr,
                    ((sAttributeListbox)pAttr).pbpBottomRightName,
                    xm + (Int32)(((sAttributeListbox)pAttr).pbpRight != null ? ((sAttributeListbox)pAttr).pbpRight.Width : 0) - (Int32)((sAttributeListbox)pAttr).pbpBottomRight.Width,
                    pAttr.pAbsolutY + pAttr.pHeight
                    ).paint(sender, e);
                //painter.blit(br, ePoint(xm, pos.bottom()-br->size().height()), eRect(x, pos.bottom()-br->size().height(), pos.width() - x, bl->size().height()));
            }

            if (((sAttributeListbox)pAttr).pbpBottomName != null)
            {
                x += (Int32)(((sAttributeListbox)pAttr).pbpBottomLeft != null ? ((sAttributeListbox)pAttr).pbpBottomLeft.Width : 0) - (Int32)(((sAttributeListbox)pAttr).pbpLeft != null ? ((sAttributeListbox)pAttr).pbpLeft.Width : 0);
                int diff = (((sAttributeListbox)pAttr).pbpRight != null ? ((sAttributeListbox)pAttr).pbpRight.Width : 0) - (((sAttributeListbox)pAttr).pbpBottomRight != null ? ((sAttributeListbox)pAttr).pbpBottomRight.Width : 0);
                xm -= (Int32)(diff > 0 ? diff : -diff);
                while (x < xm)
                {
                    new sGraphicImage(pAttr,
                        ((sAttributeListbox)pAttr).pbpBottomName,
                        x,
                        pAttr.pAbsolutY + pAttr.pHeight,
                        xm - x,
                        (Int32)((sAttributeListbox)pAttr).pbpBottom.Height
                        ).paint(sender, e);
                    //painter.blit(b, ePoint(x, pos.bottom()-b->size().height()), eRect(x, pos.bottom()-b->size().height(), xm - x, pos.height()));
                    x += (Int32)((sAttributeListbox)pAttr).pbpBottom.Width;
                }
            }

            Int32 y = 0;
            //if (pAttr.pbpTopLeft != null)
            //    y = (UInt32)pAttr.pbpTopLeft.Height;

            y += pAttr.pAbsolutY;

            Int32 ym = pAttr.pAbsolutY + pAttr.pHeight;
            //if (pAttr.pbpBottomLeft != null)
            //    ym -= (UInt32)pAttr.pbpBottomLeft.Height;

            if (((sAttributeListbox)pAttr).pbpLeftName != null)
            {
                while (y < ym)
                {
                    new sGraphicImage(pAttr,
                        ((sAttributeListbox)pAttr).pbpLeftName,
                        pAttr.pAbsolutX - (Int32)((sAttributeListbox)pAttr).pbpLeft.Width,
                        y,
                        (Int32)((sAttributeListbox)pAttr).pbpLeft.Width,
                        ym - y
                        ).paint(sender, e);
                    //painter.blit(l, ePoint(pos.left(), y), eRect(pos.left(), y, pos.width(), ym - y));
                    y += (Int32)((sAttributeListbox)pAttr).pbpLeft.Height;
                }
            }

            y = 0;

            //if (pAttr.pbpTopRight != null)
            //    y = (UInt32)pAttr.pbpTopRight.Height;

            y += pAttr.pAbsolutY;

            ym = pAttr.pAbsolutY + pAttr.pHeight;
            //if (pAttr.pbpBottomRight != null)
            //    ym -= (UInt32)pAttr.pbpBottomRight.Height;

            if (((sAttributeListbox)pAttr).pbpRightName != null)
            {
                while (y < ym)
                {
                    new sGraphicImage(pAttr,
                        ((sAttributeListbox)pAttr).pbpRightName,
                        pAttr.pAbsolutX + pAttr.pWidth,
                        y,
                        (Int32)((sAttributeListbox)pAttr).pbpRight.Width,
                        ym - y
                        ).paint(sender, e);
                    //painter.blit(r, ePoint(pos.right() - r->size().width(), y), eRect(pos.right()-r->size().width(), y, r->size().width(), ym - y));
                    y += (Int32)((sAttributeListbox)pAttr).pbpRight.Height;
                }
            }

            // scrollbar
            //painter.clip(eRect(m_scrollbar->position() - ePoint(5, 0), eSize(5, m_scrollbar->size().height())));

            // entries
            if (((sAttributeListbox)pAttr).pPreviewEntries != null)
            {
                if (((sAttributeListbox)pAttr).pPreviewEntries.Count >= 1)
                {
                    int itemHeight = ((sAttributeListbox)pAttr).pItemHeight;

                    sFont font = null;
                    float fontSize = 0;
                    if (((sAttributeListbox)pAttr).pFont != null)
                    {
                        font = ((sAttributeListbox)pAttr).pFont;
                        fontSize = font.Size;
                    }
                    else
                    {
                        font = cDataBase.getFont("Regular");
                        fontSize = 20;
                    }


                    cProperty.eHAlign halign = cProperty.eHAlign.Left;
                    cProperty.eVAlign valign = cProperty.eVAlign.Top;
                    sColor foreground = ((sAttributeListbox)pAttr).pListboxSelectedForegroundColor;
                    sColor background = ((sAttributeListbox)pAttr).pListboxSelectedBackgroundColor;
                    String entry = ((sAttributeListbox)pAttr).pPreviewEntries[0];

                    // ----------------------------------- Item Selection malen Foreground -----------------------------------------------------

                    // Selection Pixmap
                    if (((sAttributeListbox)pAttr).pSelectionPixmapName != null)
                    {

                        Logger.LogMessage("============= sGraphicListbox - PixmapName - ruft cGraphicImage.cs auf 238  = 7 ");
                        new sGraphicImage(null, ((sAttributeListbox)pAttr).pSelectionPixmapName, pAttr.pAbsolutX, pAttr.pAbsolutY, pAttr.pWidth, ((sAttributeListbox)pAttr).pItemHeight).paint(sender, e);
                    }

                    else
                    {
                        // ---------------------- wichtig wenn Pixmap gemalt hier kein background oder gradient mal , wird dann pixmap uebermalt ------------------------------------
                        int meineBreite = pWidth - ((sAttributeListbox)pAttr).pscrollbarOffset - ((sAttributeListbox)pAttr).pscrollbarWidth;


                        if (((sAttributeListbox)pAttr).pItemGradientSelected != null)
                        {


                            // hier noch die scrollbar abziehen , widht und offset plus 10 min 
                            Logger.LogMessage("============= sGraphicListbox.cs - ItemGradientSelected ist vorhanden ");
                            Logger.LogMessage("============= sGraphicListbox - Selection Gradient - ruft cGraphicRectangel.cs auf 251  = 5 mit Gradient ");
                            new sGraphicRectangel(pAttr.pAbsolutX, pAttr.pAbsolutY, meineBreite, ((sAttributeListbox)pAttr).pItemHeight, ((sAttributeListbox)pAttr).pItemGradientSelected)
                                .withCornerRadius(pAttr.pCornerRadius)
                                .paint(sender, e);
                        }
                        else
                        {
                            Logger.LogMessage("============= sGraphicListbox - Selection Background - ruft cGraphicRectangel.cs auf 258  = 7 ");
                            new sGraphicRectangel(pAttr.pAbsolutX, pAttr.pAbsolutY, meineBreite, ((sAttributeListbox)pAttr).pItemHeight, true, 1.0F, ((sAttributeListbox)pAttr).pListboxSelectedBackgroundColor)
                                .withCornerRadius(pAttr.pCornerRadius)
                                .paint(sender, e);
                        }
                    }
                    // ---------------------------------------- Scrollbar -----------------------------------------------------------------------------
                    if (((sAttributeListbox)pAttr).pScrollbarMode != cProperty.eScrollbarMode.showNever)
                    {
                        if (((sAttributeListbox)pAttr).pscrollbarSliderBackgroundColor != null)
                        {
                            // farbe vorhanden
                        }
                        else
                        {
                            ((sAttributeListbox)pAttr).pscrollbarSliderBackgroundColor = ((sAttributeListbox)pAttr).pListboxForegroundColor;

                        }

                        if (((sAttributeListbox)pAttr).pscrollbarWidth > 0)
                        {
                            // wert vorhanden
                        }
                        else
                        {
                            ((sAttributeListbox)pAttr).pscrollbarWidth = 10;

                        }

                        if (((sAttributeListbox)pAttr).pscrollbarOffset > 0)
                        {
                            // wert vorhanden
                        }
                        else
                        {
                            ((sAttributeListbox)pAttr).pscrollbarOffset = 10;

                        }

                        scrollbarAbsolutX = pAttr.pAbsolutX + pAttr.pWidth - ((sAttributeListbox)pAttr).pscrollbarWidth;
                        scrollbarAbsolutY = pAttr.pAbsolutY + 10;
                        scrollbarBreite = ((sAttributeListbox)pAttr).pscrollbarWidth;
                        scrollbarHeight = pAttr.pHeight - 20;

                        // ------------------------------- erst malen wir den Background --------------------------------
                        if (pAttr.pTransparent)
                        {
                            Logger.LogMessage("============= sGraphicProgress - Transparent true - es wird nichts gemalt ");
                            //new sGraphicRectangel().paint(sender, e);
                        }
                        else
                        {
                            Logger.LogMessage("============= sGraphicListbox - Srollbar - ruft cGraphicRectangel.cs auf 314  = 7 ");
                            new sGraphicRectangel(scrollbarAbsolutX, scrollbarAbsolutY, scrollbarBreite, scrollbarHeight, true, 1.0F, ((sAttributeListbox)pAttr).pscrollbarSliderBackgroundColor)
                            .withCornerRadius(pAttr.pCornerRadius)
                            .paint(sender, e);
                        }
                        if (((sAttributeListbox)pAttr).pScrollbarBackgroundPicture != null)

                        {
                            // hier image aufrufen
                            Logger.LogMessage("============= sGraphicListbox - PixmapName - ruft cGraphicImage.cs auf 323  = 7 ");
                            new sGraphicImage(null, ((sAttributeListbox)pAttr).pScrollbarBackgroundPictureName, scrollbarAbsolutX, scrollbarAbsolutY, scrollbarBreite, scrollbarHeight).paint(sender, e);

                        }
                        else
                        {
                            scrollbarHeight = scrollbarHeight / 4 * 3;

                            if (((sAttributeListbox)pAttr).pScrollbarForegroundGradient != null)
                            {
                               
                                // hier nur 3/4 die scrollbar malen , scrollbarHeight
                                Logger.LogMessage("============= sGraphicListbox.cs - Gradient ist vorhanden ");
                                Logger.LogMessage("============= sGraphicListbox - Gradient - ruft cGraphicRectangel.cs auf 336  = 5 mit Gradient ");
                                new sGraphicRectangel(scrollbarAbsolutX, scrollbarAbsolutY, scrollbarBreite, scrollbarHeight, ((sAttributeListbox)pAttr).pScrollbarForegroundGradient)
                                    .withCornerRadius(pAttr.pCornerRadius)
                                    .paint(sender, e);

                            }
                            else
                            {
                                if (((sAttributeListbox)pAttr).pscrollbarSliderForegroundColor != null)
                                {
                                    Logger.LogMessage("============= sGraphicListbox - Foreground - ruft cGraphicRectangel.cs auf 346  = 7 ");
                                    new sGraphicRectangel(scrollbarAbsolutX, scrollbarAbsolutY, scrollbarBreite, scrollbarHeight, true, 1.0F, ((sAttributeListbox)pAttr).pscrollbarSliderForegroundColor)
                                        .withCornerRadius(pAttr.pCornerRadius)
                                        .paint(sender, e);
                                }
                            }

                        }

                        if (((sAttributeListbox)pAttr).pscrollbarSliderBorderColor != null)
                        {
                            Logger.LogMessage("============= sGraphicListbox - Border - ruft cGraphicRectangel.cs auf 357  = 4 ");
                            new sGraphicRectangel(pAttr, false, (float)pAttr.pBorderWidth, ((sAttributeListbox)pAttr).pscrollbarSliderBorderColor)
                                .withCornerRadius(pAttr.pCornerRadius)
                                .paint(sender, e);
                        }

                    }
                        // -------------------------------------------------Scrollbar Ende -------------------------------------------------------------------------------


                    if (pAttr.pTransparent)
                    {
                        Logger.LogMessage("============= sGraphicListbox - Transparent true - ruft cGraphicFont.cs auf 367  = 7 ");
                        new sGraphicFont(null, pAttr.pAbsolutX, pAttr.pAbsolutY, entry, fontSize, font, foreground, halign, valign)
                            .paint(sender, e);
                    }
                    else
                    {

                        //float Radius = pAttr.pCornerRadius;
                        Logger.LogMessage("============= sGraphicListbox - Transparent false - ruft cGraphicFont.cs auf 372  = 7 ");
                        new sGraphicFont(null, pAttr.pAbsolutX, pAttr.pAbsolutY, entry, fontSize, font, foreground, background == null ? new sColor(Color.Black) : background, halign, valign)
                            .paint(sender, e);
                    }

                    if (((sAttributeListbox)pAttr).pPreviewEntries.Count > 1)
                    {
                        foreground = ((sAttributeListbox)pAttr).pListboxForegroundColor;
                        background = ((sAttributeListbox)pAttr).pListboxBackgroundColor;

                        for (int i = 1; i < ((sAttributeListbox)pAttr).pPreviewEntries.Count; i++)
                        {
                            //Listen Einträge
                            if (i * itemHeight >= pAttr.pHeight)
                                // Abbrechen wenn Höhe der Listen Einträge größer als Höhe der Liste
                                break;

                            entry = ((sAttributeListbox)pAttr).pPreviewEntries[i];

                            // NonSelection Pixmap
                            if (((sAttributeListbox)pAttr).pBackgroundPixmapName != null)
                            {
                                Logger.LogMessage("============= sGraphicListbox - BackgroundPixmapName ja -  ruft cGraphicImage.cs auf 393  = 7 ");
                                new sGraphicImage(null, ((sAttributeListbox)pAttr).pBackgroundPixmapName, pAttr.pAbsolutX, pAttr.pAbsolutY + i * itemHeight, pAttr.pWidth, ((sAttributeListbox)pAttr).pItemHeight).paint(sender, e);
                            }

                            else
                            {
                                Logger.LogMessage("============= sGraphicListbox - BackgroundPixmapName nein -  ruft cGraphicRectangel.cs auf 399  = 7 ");
                                new sGraphicRectangel(pAttr.pAbsolutX, pAttr.pAbsolutY + i * itemHeight, pAttr.pWidth, ((sAttributeListbox)pAttr).pItemHeight, true, 1.0F, ((sAttributeListbox)pAttr).pListboxBackgroundColor).paint(sender, e);
                            }

                            if (pAttr.pTransparent)
                            {
                                Logger.LogMessage("============= sGraphicListbox - Transparent true - ruft cGraphicFont.cs auf 405  = 7 ");
                                new sGraphicFont(null, pAttr.pAbsolutX, pAttr.pAbsolutY + i * itemHeight, entry, fontSize, font, foreground, halign, valign).paint(sender, e);
                            }
                            else
                            {
                                Logger.LogMessage("============= sGraphicListbox - Transparent false - ruft cGraphicFont.cs auf 410  = 7 ");
                                new sGraphicFont(null, pAttr.pAbsolutX, pAttr.pAbsolutY + i * itemHeight, entry, fontSize, font, foreground, background == null ? new sColor(Color.Black) : background, halign, valign).paint(sender, e);
                            }

                        }
                    }
                }
                else
                {
                    // Selection Pixmap
                    if (((sAttributeListbox)pAttr).pSelectionPixmapName != null)
                    {
                        Logger.LogMessage("============= sGraphicListbox - Selection PixmapName ja - ruft cGraphicImage.cs auf 422  = 4 ");
                        new sGraphicImage(pAttr, ((sAttributeListbox)pAttr).pSelectionPixmapName).paint(sender, e);
                    }

                    else
                    {
                        Logger.LogMessage("============= sGraphicListbox - SelectionPixmapName nein -  ruft cGraphicRectangel.cs auf 428  = 7 ");
                        new sGraphicRectangel(pAttr.pAbsolutX, pAttr.pAbsolutY, pAttr.pWidth, ((sAttributeListbox)pAttr).pItemHeight, true, 1.0F, ((sAttributeListbox)pAttr).pListboxSelectedBackgroundColor).paint(sender, e);
                    }

                }
            }
        }
    }
}
