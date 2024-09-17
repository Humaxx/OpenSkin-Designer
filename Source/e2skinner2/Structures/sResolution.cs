using System;
using System.Collections.Generic;
using System.Diagnostics;

//using System.Linq;
using System.Text;

namespace OpenSkinDesigner.Structures
{
    public class sResolution
    {
        public UInt32 Xres = 0;
        public UInt32 Yres = 0;
        public UInt32 Bpp = 0;

        public sResolution(UInt32 xres, UInt32 yres, UInt32 bpp)
        {

            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sResolution - Xres Yres Bpp () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion




            Xres = xres;
            Yres = yres;
            Bpp = bpp;
        }
    }
}
