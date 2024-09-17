using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
using OpenSkinDesigner.Logic;
using System.Security.Cryptography;

namespace OpenSkinDesigner.Structures
{
    public class sGradient
    {
        
        public String pValue;
        protected sColor pColorStart;
        protected sColor pColorMid;
        protected sColor pColorEnd;
        protected eGradientDirection pDirection;
        public sColor ColorStart
        {
            get
            {
                Logger.LogMessage("+++++++++++ sGradient.cs - color start ");
                return pColorStart;
            }
        }
        public sColor ColorMid
        {
            get
            {
                Logger.LogMessage("+++++++++++ sGradient.cs - color mid ");
                return pColorMid;
            }
        }
        public sColor ColorEnd
        {
            get
            {
                Logger.LogMessage("+++++++++++ sGradient.cs - color end ");
                return pColorEnd;
            }
        }

        public eGradientDirection Direction
        {
            get
            {
                Logger.LogMessage("+++++++++++ sGradient.cs - direction ");
                return pDirection;
            }
        }

        protected sGradient(String value)
        {
            pValue = value;
            Logger.LogMessage("+++++++++++ sGradient.cs - string value ");
            // Aufteilen des Strings in ein Array
            string[] gradientParts = value.Split(',');

            // Zuweisung der Werte an die entsprechenden Variablen
            pColorStart = (sColor)cDataBase.pColors.get(gradientParts[0]);
            pColorMid = (sColor)cDataBase.pColors.get(gradientParts[1]);
            pColorEnd = (sColor)cDataBase.pColors.get(gradientParts[2]);
            switch (gradientParts[3])
            {
                case "horizontal": pDirection = eGradientDirection.Horizontal; break;
                case "vertical": pDirection = eGradientDirection.Vertical; break;
                default: pDirection = eGradientDirection.Horizontal; break;
            }
        }

        public static sGradient parse(String value)
        {
            Logger.LogMessage("+++++++++++ sGradient.cs - parse ");
            if (value == null)
            {
                return null;
            }
            string[] gradientParts = value.Split(',');
            if (gradientParts.Length != 4)
            {
                return null;
            }
            return new sGradient(value);
        }
    }
}
