using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;
using OpenSkinDesigner.Logic;
using System.Diagnostics;

namespace OpenSkinDesigner.Structures
{
    class sAttributeProgress : sAttribute
    {
        private const String entryName = "Progress";

        public sColor pBackgroundColor;

        // #####################################
        public sGradient pForegroundGradient;
        //public float pCornerRadius;
        public string ppixmap;
        // #####################################

        [Editor(typeof(OpenSkinDesigner.Structures.cProperty.GradeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(OpenSkinDesigner.Structures.cProperty.sColorConverter)),
        CategoryAttribute(entryName)]
        public String BackgroundColor
        {
            get { return pBackgroundColor.pName; }
            set
            {

                // Hole den Namen der aufrufenden Methode
                string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
                // Log-Nachricht erstellen
                string logMessage = $"%%%%%%%%%%%%%%% sAttributeProgress - BackgroundColor () wurde von {callerName} aufgerufen .";
                // Loggen
                Logger.LogMessage(logMessage);
                // Weiter mit der eigentlichen Funktion

                if (value != null)
                    pBackgroundColor = (sColor)cDataBase.pColors.get(value);
                else
                    pBackgroundColor = null;

                if (pBackgroundColor != null && pBackgroundColor != (sColor)((sWindowStyle)cDataBase.pWindowstyles.get()).pColors["Background"])
                {
                    if (myNode.Attributes["backgroundColor"] != null)
                        myNode.Attributes["backgroundColor"].Value = pBackgroundColor.pName;
                    else
                    {
                        myNode.Attributes.Append(myNode.OwnerDocument.CreateAttribute("backgroundColor"));
                        myNode.Attributes["backgroundColor"].Value = pBackgroundColor.pName;
                    }
                }
                else
                    if (myNode.Attributes["backgroundColor"] != null)
                    myNode.Attributes.RemoveNamedItem("backgroundColor");
            }
        }

        public sAttributeProgress(sAttribute parent, XmlNode node)
            : base(parent, node)
        {
            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"%%%%%%%%%%%%%%% sAttributeProgress - backgrounColor 2 () wurde von {callerName} aufgerufen .";
            // Loggen
            Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion

            if (node.Attributes["pixmap"] != null)
            {
                string value = myNode.Attributes["pixmap"].Value;

                Logger.LogMessage("%%%%%%%%%%%%%%% cAttributeProgress.cs - pixmap value : " + value);

                // auch in die variable schreiben 

                ppixmap = value;
            }

            // ________________________________________________________________________________________________________________


            if (myNode.Attributes["cornerRadius"] != null)
            {
                string value = myNode.Attributes["cornerRadius"].Value;

                Logger.LogMessage("%%%%%%%%%%%%%%% cAttributeProgress.cs - cornerRadius ist: " + value);

                float.TryParse(value, out pCornerRadius);
            }



            if (node.Attributes["backgroundColor"] != null)
            {
                pBackgroundColor = (sColor)cDataBase.pColors.get(node.Attributes["backgroundColor"].Value);
            }
            else
            {
                pBackgroundColor = (sColor)((sWindowStyle)cDataBase.pWindowstyles.get()).pColors["Background"];
            }
            // --------------------

            if (node.Attributes["foregroundGradient"] != null)
            {
                string value = myNode.Attributes["foregroundGradient"].Value;

                Logger.LogMessage("%%%%%%%%%%%%%%% cAttributeProgress.cs - Die farbe foregroundGradient  ist: " + value);
                pForegroundGradient = sGradient.parse(value);
            }
            else
            {
                if (node.Attributes["foregroundColor"] != null)
                {
                    string value = myNode.Attributes["foregroundColor"].Value;

                    Logger.LogMessage("%%%%%%%%%%%%%%% cAttributeProgress.cs - Die Farbe foregroundColor: " + value);

                    // Erstelle den Gradient-String basierend auf der foregroundColor
                    string generatedGradientString = $"{value},{value},{value},horizontal";

                    // Parse den generierten String in ein sGradient-Objekt
                    pForegroundGradient = sGradient.parse(generatedGradientString);

                    // Optional: Loggen, dass der Gradient generiert wurde
                    Logger.LogMessage("%%%%%%%%%%%%%%% cAttributeProgress.cs - Generierter foregroundGradient aus foregroundColor: " + generatedGradientString);

                }


            }
        }
    }
}
