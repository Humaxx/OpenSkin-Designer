﻿using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

using OpenSkinDesigner.Structures;
using System.IO;

namespace OpenSkinDesigner.Logic
{
    public class cXMLHandler
    {
        public XmlDocument xmlDocument;
        ArrayList ElementList = null;
        int rootNode = 0;
        TreeView xmlTreeView;
        /// <summary>
        /// Initialisiert eine neue Instanz der MultiClipboard Klasse.
        /// </summary>
        public cXMLHandler()
        {
        }

        /// <summary>
        /// Eine vorher Exportierte Xml Datei wieder in ein TreeView importieren
        /// </summary>
        /// <param name="path">Der Quellpfad der Xml Datei</param>
        /// <param name="treeView">Ein TreeView in dem der Inhalt der Xml Datei wieder angezeigt werden soll</param>
        /// <exception cref="FileNotFoundException">gibt an das die Datei nicht gefunden werden konnte</exception>
        public void XmlToTreeView(String path, TreeView treeView)
        {
            xmlDocument = new XmlDocument();
            
            xmlDocument.Load(path);
            treeView.Nodes.Clear();
            ElementList = new ArrayList();

            TreeNode treeNode;
            treeNode = new TreeNode("skin");
            treeView.Nodes.Add(treeNode);

            rootNode = treeNode.GetHashCode();
            sElementList element = new sElementList(rootNode, 0, treeNode, xmlDocument.DocumentElement/*.ParentNode*/);
            ElementList.Add(element);


            XmlRekursivImport(treeNode.Nodes, xmlDocument.DocumentElement.ChildNodes);

            xmlTreeView = treeView;
        }

        public void XmlToFile(String path)
        {
            String xmlPath = cProperties.getProperty("path") + "/" + cProperties.getProperty("path_skin_xml");
            xmlDocument.Save(xmlPath);
            xmlPath = xmlPath.Substring(0, xmlPath.Length - 4) + "_"; // drop ".xml"
            foreach (TreeNode node in xmlTreeView.Nodes[0].Nodes)
            {
                XmlDocument incDoc = node.Tag as XmlDocument;
                if (incDoc != null)
                {
                    String skin = node.Text.Substring(node.Text.LastIndexOf(' ') + 1);
                    incDoc.Save(xmlPath + skin + ".xml");
                }
            }
        }

        private String[] XmlElementStringLookup(String element)
        {
            String stmp = null;
            switch (element)
            {
                case "#comment":
                    stmp = "Comment";
                    break;
                //case "screen":
                //    stmp = "name";
                //    break;
                case "eLabel":
                    stmp = "text";
                    break;
                case "ePixmap":
                    stmp = "pixmap";
                    break;
                case "widget":
                    return new String[2]{"pixmap", "text"};
                default:
                    break;
            }

            return stmp == null ? null : new String[1]{stmp};
        }

        private String getTreeName(XmlNode node)
        {
            String name = node.Name;

            if (node.Attributes != null && node.Attributes["name"] != null)
                name += " - " + node.Attributes["name"].Value;

            String[] exts = XmlElementStringLookup(node.Name);
            if (exts != null)
            {
                bool attr = false;
                foreach (String ext in exts)
                {
                    if (node.Attributes != null && node.Attributes[ext] != null)
                    {
                        name += " : " + node.Attributes[ext].Value;
                        attr = true;
                    }
                }
                if (!attr)
                    name += " " + node.Value;
            }

            return name;
        }

        private void XmlRekursivImport(TreeNodeCollection elem, XmlNodeList xmlNodeList)
        {
            TreeNode treeNode;
            sElementList element;
            foreach (XmlNode myXmlNode in xmlNodeList)
            {
                if (myXmlNode.Name == "output" ||
                    myXmlNode.Name == "windowstyle" ||
                    myXmlNode.Name == "#whitespace")
                    continue;
                //MessageBox.Show(myXmlNode.Name);

                String name = getTreeName(myXmlNode);

                if (myXmlNode.Name == "include")
                {
                    XmlDocument incDocument = new XmlDocument();
                    string fname = myXmlNode.Attributes["filename"].Value;
                    incDocument.Load(cDataBase.getPath(fname));
                    string pname = cProperties.getProperty("path_skin_xml");
                    string sname = pname.Substring(0, pname.Length - 4) + "_"; // "Name-of-skin/skin_"
                    if (!fname.StartsWith(sname))
                        sname = "skin_";
                    string iname = fname.StartsWith(sname) && fname.EndsWith(".xml") ? fname.Substring(sname.Length, fname.Length - 4 - sname.Length) : fname;
                    treeNode = new TreeNode(name + " : " + iname);
                    treeNode.Tag = incDocument;
                    setImageIndex(myXmlNode, treeNode);
                    XmlRekursivImport(treeNode.Nodes, incDocument.DocumentElement.ChildNodes);
                    elem.Add(treeNode);
                    element = new sElementList(treeNode.GetHashCode(), treeNode.Parent.GetHashCode(), treeNode, myXmlNode);
                    ElementList.Add(element);
                    continue;
                }

                treeNode = new TreeNode(name/*Attributes["value"].Value*/);
                setImageIndex(myXmlNode,  treeNode);

                if (myXmlNode.ChildNodes.Count > 0)
                {
                    XmlRekursivImport(treeNode.Nodes, myXmlNode.ChildNodes);
                }
                elem.Add(treeNode);
                element = new sElementList(treeNode.GetHashCode(), treeNode.Parent.GetHashCode(), treeNode, myXmlNode);
                ElementList.Add(element);
            }
        }


        private void setImageIndex(XmlNode myXmlNode, TreeNode treeNode)
        {
            //ICONS FOR TREE VIEW
            if (myXmlNode.Name == "skin")
            {
                treeNode.ImageIndex = 0;
            }
            else if (myXmlNode.Name == "colors" || myXmlNode.Name == "fonts" || myXmlNode.Name == "font" || myXmlNode.Name == "sub" || myXmlNode.Name == "subtitles" || myXmlNode.Name == "color" || myXmlNode.Name == "include")
            {
                treeNode.ImageIndex = 1;
            }
            else if (myXmlNode.Name == "#comment")
            {
                treeNode.ImageIndex = 2;
            }
            else if (myXmlNode.Name == "ePixmap")
            {
                treeNode.ImageIndex = 7;
            }
            else if (myXmlNode.Name == "eLabel")
            {
                treeNode.ImageIndex = 8;
            }
            else if (myXmlNode.Name == "widget")
            {
                treeNode.ImageIndex = 9;
            }
            else if (myXmlNode.Name == "screen" || myXmlNode.Name == "panel")
            {
                if (myXmlNode.Attributes["position"] == null || myXmlNode.Name == "panel")
                {
                    treeNode.ImageIndex = 3;
                }
                else
                {
                    treeNode.ImageIndex = 4;
                }
            }
            else
            {
                treeNode.ImageIndex = 6;
            }
        }

        //syncs the name
        public void XmlSyncTreeChilds(int hash, TreeNode elem)
        {
            foreach (sElementList temp in ElementList)
            {
                if (temp.Handle == hash)
                {
                    String name = getTreeName(temp.Node);
                    temp.TreeNode.Name = name;

                    //if (treenode.GetHashCode() == temp.Handle)
                            elem.Text = name;
                }
            }
        }

        public TreeNode XmlSyncAddTreeChild(int hash, TreeNode elem, XmlNode node)
        {
            TreeNode treeNode = new TreeNode("new"/*Attributes["value"].Value*/);
            elem.Nodes.Add(treeNode);

            sElementList element = new sElementList(treeNode.GetHashCode(), treeNode.Parent.GetHashCode(), treeNode, node);
            ElementList.Add(element);

            setImageIndex(node,treeNode);

            return treeNode;
        }

        public XmlNode XmlAppendNode(XmlNode node, String[] attributes)
        {
            XmlNode xmlNode = null;
            XmlDocument xmlDoc = node.OwnerDocument;
            xmlNode = xmlDoc.CreateElement(attributes[0]);

            for (int i = 1; i < attributes.Length; i += 2)
            {
                xmlNode.Attributes.Append(xmlDoc.CreateAttribute(attributes[i]));
                xmlNode.Attributes[attributes[i]].Value = attributes[i+1];
            }

            if (node != null)
                node.AppendChild(xmlNode);

            return xmlNode;
        }

        public XmlNode XmlAppendNode(XmlNode node, String outerXml)
        {
            XmlNode xmlNode = null;
            XmlTextReader xmlReader = new XmlTextReader(new StringReader(outerXml));
            xmlNode = node.OwnerDocument.ReadNode(xmlReader);

            if(node != null)
                node.AppendChild(xmlNode);

            return xmlNode;
        }

        public XmlNode getXmlNode(TreeNode treeNode)
        {
            return getXmlNode(getHash(treeNode));
        }


        public XmlNode getXmlNode(int hash)
        {
            foreach (sElementList temp in ElementList)
            {
                if (temp.Handle == hash)
                {
                    return temp.Node;
                }
            }
            return null;
        }

        public int getHash(XmlNode node)
        {
            foreach (sElementList temp in ElementList)
            {
                if (temp.Node == node)
                {
                    return temp.Handle;
                }
            }
            return 0;
        }

        public int getHash(TreeNode treeNode)
        {
            return treeNode.GetHashCode();
            /*foreach (sElementList temp in ElementList)
            {
                if (temp.TreeNode == treeNode)
                {
                    return temp.Handle;
                }
            }
            return 0;*/
        }

        public TreeNode getTreeNode(int hash)
        {
            foreach (sElementList temp in ElementList)
            {
                if (temp.Handle == hash)
                {
                    return temp.TreeNode;
                }
            }
            return null;
        }

        public TreeNode getTreeNode(XmlNode node)
        {
            foreach (sElementList temp in ElementList)
            {
                if (temp.Node == node)
                {
                    return temp.TreeNode;
                }
            }
            return null;
        }

        public void XmlReplaceNodeAndChilds(int hash, String node)
        {
            // 3. Remove all Child Nodes
            // 1. Find the node to replace
            // 2. Replace it
            
            // 4. Import the Child Nodes
            // 5. Find old Parent and give him new Element

            XmlTextReader xmlReader = new XmlTextReader(new StringReader(node));

            for (int i = 0; i < ElementList.Count; )
            {
                sElementList tempChild = (sElementList)ElementList[i];
                if (tempChild.ParentHandle == hash)
                {
                    tempChild.TreeNode.Remove();
                    ElementList.Remove(tempChild);
                }
                else
                    i++;
            }

            foreach (sElementList temp in ElementList)
            {
                if (temp.Handle == hash)
                {
                    if (temp.ParentHandle == 0)
                    {
                        //We are the root element

                        temp.Node.OwnerDocument.ReplaceChild(temp.Node.OwnerDocument.ReadNode(xmlReader), temp.Node);
                        temp.Node = temp.Node.OwnerDocument.DocumentElement/*.ParentNode*/;
                        XmlRekursivImport(temp.TreeNode.Nodes, temp.Node.ChildNodes);
                    }
                    else
                    {
                        // Find parent
                        foreach (sElementList tempParent in ElementList)
                        {
                            if (tempParent.Handle == temp.ParentHandle)
                            {
                                //Find with the old node, the childnode and replace it
                                for (int i = 0; i < tempParent.Node.ChildNodes.Count; i++)
                                    if (tempParent.Node.ChildNodes[i] == temp.Node)
                                    {
                                        if (node.Length == 0) // Delete node
                                        {
                                            tempParent.Node.RemoveChild(tempParent.Node.ChildNodes[i]);
                                            temp.TreeNode.Remove();
                                            ElementList.Remove(temp);
                                        }
                                        else // Replace node
                                        {
                                            temp.Node = temp.Node.OwnerDocument.ReadNode(xmlReader);
                                            tempParent.Node.ReplaceChild(temp.Node, tempParent.Node.ChildNodes[i]);
                                            XmlRekursivImport(temp.TreeNode.Nodes, temp.Node.ChildNodes);
                                        }
                                        break;
                                    }
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        public void XmlReplaceNode(int hash, String node)
        {
            XmlTextReader xmlReader = new XmlTextReader(new StringReader(node));
            foreach (sElementList temp in ElementList)
            {
                if (temp.Handle == hash)
                {
                    temp.Node = temp.Node.OwnerDocument.ReadNode(xmlReader);
                    break;
                }
            }
        }

        public XmlNode[] XmlGetChildNodes(int hash)
        {
            ArrayList list = new ArrayList();
            foreach (sElementList temp in ElementList)
            {
                if (temp.ParentHandle == hash)
                {
                    list.Add(temp.Node);
                }
            }
            return (XmlNode[])list.ToArray(typeof(XmlNode));
        }

        public int XmlGetParentHandle(int hash)
        {
            foreach (sElementList temp in ElementList)
            {
                if (temp.Handle == hash)
                {
                    return temp.ParentHandle;
                }
            }
            return 0;
        }

        public XmlNode XmlGetRootNodeElement(String[] name)
        {
            foreach (sElementList temp in ElementList)
            {
                if (temp.Handle == rootNode)
                {
                    if (temp.Node.ChildNodes.Count > 0)
                    {
                        if (name != null && name.Length > 0)
                            return XmlGetSearchChilds(temp.Node.ChildNodes, name);
                        else
                            return temp.Node;
                    }
                }
            }
            return null;
        }

        private XmlNode XmlGetSearchChilds(XmlNodeList node, String[] name)
        {
            foreach (XmlNode myXmlNode in node)
            {
                if (myXmlNode.Name == name[0])
                {
                    if (name.Length > 1)
                    {
                        String[] path = new String[name.Length - 1];
                        for (int i = 0; i < name.Length - 1; i++)
                            path[i] = name[i + 1];

                        return XmlGetSearchChilds(myXmlNode.ChildNodes, path);
                    }
                    else
                    {
                        return myXmlNode;
                    }
                }
            }
            return null;
        }
    }
}
