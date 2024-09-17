using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Xml;
using System.Drawing.Drawing2D;

namespace OpenSkinDesigner.Structures
{
    class sGraphicRectangel : sGraphicElement
    {
        protected bool pFilled;
        protected float pLineWidth;
        protected sColor pColor;
        protected sGradient pGradient;
        private float pCornerRadius;

        public sGraphicRectangel(sAttribute attr, bool filled, float linewidth, sColor color)
            : base(attr)
        {
            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicRectangels - erster sollten 4 stueck sein -   () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion

            pFilled = filled;
            pLineWidth = linewidth;
            pColor = color;
        }

        public sGraphicRectangel(Int32 x, Int32 y, Int32 width, Int32 height, bool filled, float linewidth, sColor color)
            : base(x, y, width, height)
        {
            //Console.WriteLine("sGraphicRectangel: " + x + ":" + y + " " + width + "x" + height);
            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicRectangels  -zweiter sollten 7 stueck sein -  () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion


            pFilled = filled;
            pLineWidth = linewidth;
            pColor = color;

            pZPosition = 1000;
        }

        public sGraphicRectangel(sAttribute attr, sGradient gradient)
            : base(attr)
        {
            pGradient = gradient;
        }

        public sGraphicRectangel(Int32 x, Int32 y, Int32 width, Int32 height, sGradient gradient)
            : base(x, y, width, height)
        {
            //Console.WriteLine("sGraphicRectangel: " + x + ":" + y + " " + width + "x" + height);
            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicRectangels  -dritte sollten 5 stueck sein (Gradient) () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion
            
            //this.cornerRadius = cornerRadius;
            pGradient = gradient;

            pZPosition = 1000;
        }
        // ##################################
        public sGraphicRectangel withCornerRadius(float cornerRadius)
        {
            
            pCornerRadius = cornerRadius * 2;
            Logger.LogMessage("============= sGraphicRectangels - cornerRadius uebergeben ist doppelt so groß: () " + cornerRadius);
            return this;
        }

        public override void paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicRectangels - malen () wurde von {callerName} aufgerufen .";
            Logger.LogMessage(logMessage);


            Logger.LogMessage("============= cGraphicRectangel.cs - ist was in Gradient drin = " + pGradient);
            Logger.LogMessage("============= cGraphicRectangel.cs - cornerRadius =  " + pCornerRadius);

            if (pGradient != null)  // ------------------------ backgroundGradient malen 
            {
                Logger.LogMessage("============= cGraphicRectangel.cs - BackgroundGradient malen ");

                // Rechteck-Koordinaten
                float x = pX;
                float y = pY;
                float width = pWidth;
                float height = pHeight;

                // Gradient-Richtung festlegen (horizontal oder vertikal)
                bool gradient_direction = pGradient.Direction == eGradientDirection.Horizontal;

                // Linearen Farbverlauf erstellen
                LinearGradientBrush brush = new LinearGradientBrush(
                    gradient_direction ? new PointF(x, y) : new PointF(x, y),
                    gradient_direction ? new PointF(x + width, y) : new PointF(x, y + height),
                    pGradient.ColorStart.Color,
                    pGradient.ColorEnd.Color);

                // Farbverlauf definieren
                brush.InterpolationColors = new ColorBlend
                {
                    Positions = new float[] { 0f, 0.5f, 1f },
                    Colors = new Color[] { pGradient.ColorStart.Color, pGradient.ColorMid.Color, pGradient.ColorEnd.Color }
                };
                
                // GraphicsPath für das Rechteck mit abgerundeten Ecken erstellen
                GraphicsPath path = new GraphicsPath();
                if (pCornerRadius > 0)
                {
                    Logger.LogMessage("============= cGraphicRectangel.cs - BackgroundGradient malen mit Radius ");
                    path.AddArc(x, y, pCornerRadius, pCornerRadius, 180, 90);
                    path.AddArc(x + width - pCornerRadius, y, pCornerRadius, pCornerRadius, 270, 90);
                    path.AddArc(x + width - pCornerRadius, y + height - pCornerRadius, pCornerRadius, pCornerRadius, 0, 90);
                    path.AddArc(x, y + height - pCornerRadius, pCornerRadius, pCornerRadius, 90, 90);
                    path.CloseFigure(); // Pfad schließen
                }
                else
                {
                    path.AddRectangle(new Rectangle(pX, pY, pWidth, pHeight));
                }
                // Pfad mit dem Farbverlauf füllen
                g.FillPath(brush, path);

            }
            // #########################################################################################################
            else
            {
                // ------------------------------------  nur backgroundColor malen
                Logger.LogMessage("============= cGraphicRectangel.cs - BackgroundColor malen ");
                Logger.LogMessage("============= cGraphicRectangel.cs - cornerRadius =  " + pCornerRadius);



                Color penColor = pColor.Color;
                if (Logic.cProperties.getPropertyBool("enable_alpha"))
                    penColor = pColor.ColorAlpha;

                Logger.LogMessage("penColor: " + penColor);
                Logger.LogMessage("pX      : " + pX);
                Logger.LogMessage("pY      : " + pY);
                Logger.LogMessage("pWidght : " + pWidth);
                Logger.LogMessage("pHeight : " + pHeight);
    

                GraphicsPath path = new GraphicsPath();

                // #############################################
                using (Pen pen = new Pen(penColor, pLineWidth))
               


                // ##############################################
                // Pfad für ein Rechteck mit abgerundeten Ecken erstellen
                if (pCornerRadius > 0)
                {
                    path.AddArc(pX, pY, pCornerRadius, pCornerRadius, 180, 90);
                    path.AddArc(pX + pWidth - pCornerRadius, pY, pCornerRadius, pCornerRadius, 270, 90);
                    path.AddArc(pX + pWidth - pCornerRadius, pY + pHeight - pCornerRadius, pCornerRadius, pCornerRadius, 0, 90);
                    path.AddArc(pX, pY + pHeight - pCornerRadius, pCornerRadius, pCornerRadius, 90, 90);
                    path.CloseFigure(); // Pfad schließen
                }
                else
                {
                    path.AddRectangle(new Rectangle(pX, pY, pWidth, pHeight));
                }

                if (pFilled)
                {
                    Logger.LogMessage("============= cGraphicRectangel.cs - BackgroundColor malen Filled ");
                    g.FillPath(new SolidBrush(penColor), path);
                }
                else
                {
                    Logger.LogMessage("============= cGraphicRectangel.cs - BackgroundColor malen DrawPath");
                    // g.DrawPath(new Pen(penColor, pLineWidth), path);
                    
                   

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Linien zwischen den Bögen zeichnen



                    // Pen erstellen und LineJoin auf Round setzen
                    Pen pen = new Pen(penColor, pLineWidth);
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

                    // Pfad zeichnen
                    g.DrawPath(pen, path);


                }
            }
        }
    }
}
