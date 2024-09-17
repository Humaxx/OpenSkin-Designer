using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace OpenSkinDesigner.Structures
{
    public class sGraphicElement : IComparable
    {
        private Int32 _pX;
        private Int32 _pY;

        [BrowsableAttribute(false)]
        public Int32 pX
        {
            get { return pAttr != null?pAttr.pAbsolutX:_pX; }
            set { _pX = value; }
        }

        [BrowsableAttribute(false)]
        public Int32 pY
        {
            get { return pAttr != null ? pAttr.pAbsolutY : _pY; }
            set { _pY = value; }
        }

        //public UInt32 pX;
        //public UInt32 pY;
        public Int32 pWidth;
        public Int32 pHeight;

        public sAttribute pAttr = null;

        public Int32 pZPosition;

        public sGraphicElement(sAttribute attr)
        {

            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sGraphicElements - pX pY pWidht pHeight pZPosition  () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion

            pAttr = attr;
            if (pAttr != null)
            {
                pX = attr.pAbsolutX;
                pY = attr.pAbsolutY;
                pWidth = attr.pWidth;
                pHeight = attr.pHeight;

                pZPosition = attr.pZPosition;
            } else
                pZPosition = 0;
        }

        public sGraphicElement(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            pX = x;
            pY = y;
            pWidth = width;
            pHeight = height;

            pZPosition = 0;
        }

        public virtual void paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
        }

        public int CompareTo(object obj)
        {
            sGraphicElement val = (sGraphicElement)obj;
            if (val.pZPosition < this.pZPosition)
                return 1;
            else if (val.pZPosition == this.pZPosition)
                return 0;
            else
                return -1;
        }
    }
}
