using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;

namespace OpenSkinDesigner.Structures
{
    class sElementList
    {
        public int Handle = 0;
        public int ParentHandle = 0;
        public TreeNode TreeNode = null;
        public XmlNode Node = null;

        public sElementList(int handle, int parentHandle, TreeNode treenode, XmlNode node)
        {
            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sElementList - () wurde von {callerName} aufgerufen .";
            // Loggen
            // Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion


            Handle = handle;
            ParentHandle = parentHandle;
            TreeNode = treenode;
            Node = node;
        }
    }
}
