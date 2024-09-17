using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing.Text;
using System.IO;
using OpenSkinDesigner.Logic;
using System.Windows.Forms;
using OpenSkinDesigner.Frames;
using System.Diagnostics;

namespace OpenSkinDesigner.Structures
{
    public class sFont
    {
        public const int DEFAULT_SCALE = 100;
        public const bool DEFAULT_REPLACEMENT = false;

        public String Name = "";
        public String Filename = "";
        public String Path = "";
        public String FontName = "";
        public bool Found = true; //MOD if font is existing in a folder for preview
        public System.Drawing.FontFamily FontFamily;
        public System.Drawing.FontStyle FontStyle;
        public int Size = 0;
        public bool isAlias = false;

        //if this isnt defined as global public, mono will lose the reference and crash!!!
        public PrivateFontCollection pfc;

        public System.Drawing.Font Font;
        public int Scale = 0;
        public bool Replacement = false;

        public sFont(String name, String path, int scale, int size, String fontName, bool replacement, bool isAlias = false)
        {
            // Hole den Namen der aufrufenden Methode
            string callerName = new StackTrace().GetFrame(1).GetMethod().Name;
            // Log-Nachricht erstellen
            string logMessage = $"============= sFont - 1 () wurde von {callerName} aufgerufen .";
            // Loggen
            // Logger.LogMessage(logMessage);
            // Weiter mit der eigentlichen Funktion


            String fontPath = cProperties.getProperty("path_fonts");
            String skinPath = cProperties.getProperty("path_skin");
            String skinsPath = cProperties.getProperty("path");
          
            pfc = new PrivateFontCollection();

            this.Name = name;
            this.Path = path;

            this.Scale = scale;
            this.Replacement = replacement;
            this.Size = size;
            this.FontName = fontName;
            this.isAlias = isAlias;

            //This way we have only the file name, but what happens if the fonts are in the skin directory ?
            //Lets check all posibilities
            Filename = Path.Substring(Path.LastIndexOf('/')>0?Path.LastIndexOf('/')+1:0);
            String AbsolutPathFont = fontPath + "/" + Filename;
            String AbsolutPathSkinPathFont = skinsPath + "/" + skinPath + "/" + Filename;
            String RelativPathFont = Path;
            // RelativPathSkinPathFont was the same path as AbsolutPathSkinPathFont
            String RelativPathSkinPathFont = skinsPath + "/" + skinPath + "/" + Path;
            // so changed it to look up in skin-path / fonts
            RelativPathSkinPathFont = skinsPath + "/" + skinPath + "/" + "fonts" + "/" + Path;

            RelativPathFont = Path;
            RelativPathFont = RelativPathFont.Replace("enigma2", "");
            RelativPathFont = RelativPathFont.Replace("usr", "");
            RelativPathFont = RelativPathFont.Replace("local", "");
            RelativPathFont = RelativPathFont.Replace("share", "");
            RelativPathFont = RelativPathFont.Replace("var", "");
            RelativPathFont = skinsPath + "/" + RelativPathFont;

            AbsolutPathFont = AbsolutPathFont.Replace("\\", "/");
            AbsolutPathSkinPathFont = AbsolutPathSkinPathFont.Replace("\\", "/");
            RelativPathFont = RelativPathFont.Replace("\\", "/");
            RelativPathSkinPathFont = RelativPathSkinPathFont.Replace("\\", "/");


            //RelativPathFont = fontPath.Replace("fonts", "") + RelativPathFont;

            String lookupPath = "";
            if (File.Exists(AbsolutPathFont))
                lookupPath = new FileInfo(AbsolutPathFont).FullName;
            else if (File.Exists(AbsolutPathSkinPathFont))
                lookupPath = new FileInfo(AbsolutPathSkinPathFont).FullName;
            else if (File.Exists(RelativPathFont))
                lookupPath = new FileInfo(RelativPathFont).FullName;
            else if (File.Exists(RelativPathSkinPathFont))
                lookupPath = new FileInfo(RelativPathSkinPathFont).FullName;
            else
            {
                Found = false;
                String errorMessage = "";
                errorMessage += fMain.GetTranslation("OpenSkinDesigner has searched in several places for the font") + ": '" + Filename + "'.\"\n\n";
                errorMessage += fMain.GetTranslation("Unfortunatly the search was not successful!") + "\n";
                errorMessage += "\n";
                errorMessage += fMain.GetTranslation("Search locations") + ":\n\n";
                errorMessage += new FileInfo(AbsolutPathFont).FullName + "\n\n";
                errorMessage += new FileInfo(AbsolutPathSkinPathFont).FullName + "\n\n";
                errorMessage += new FileInfo(RelativPathFont).FullName + "\n\n";
                errorMessage += new FileInfo(RelativPathSkinPathFont).FullName + "\n\n";

                // Openskindesigner contains lcd.ttf by default and should be existing...
                AbsolutPathFont = fontPath + "/" + "lcd.ttf";
                if (File.Exists(AbsolutPathFont))
                    errorMessage += "\n" + fMain.GetTranslation("It uses 'lcd.tff' instead.") + "\n";

                errorMessage += Environment.NewLine + Environment.NewLine + fMain.GetTranslation("Show this message again?");

                DialogResult dr = new DialogResult();
                if (MyGlobaleVariables.ShowMsgFontNotFound == true)
                dr = MessageBox.Show(errorMessage,
                    fMain.GetTranslation("Error while loading fonts"),
                    MessageBoxButtons.YesNo ,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.No)
                    MyGlobaleVariables.ShowMsgFontNotFound = false;


                if (File.Exists(AbsolutPathFont))
                    lookupPath = new FileInfo(AbsolutPathFont).FullName;
                else
                    return;              
            }

            try
            {
                pfc.AddFontFile(lookupPath);
            }
            catch (Exception error)
            {
                String errorMessage = "";
                errorMessage += fMain.GetTranslation("OpenSkinDesigner has tried to open the font") + " '" + Filename + "'.\n";
                errorMessage += fMain.GetTranslation("Unfortunatly this was not successful!") + "\n";
                errorMessage += fMain.GetTranslation("Either the font type is not supported by OpenSkinDesigner,") +" \n";
                errorMessage += fMain.GetTranslation("or it's not a valid font.") + "\n";
                errorMessage += "\n";
                errorMessage += fMain.GetTranslation("Location") + ":\n";
                errorMessage += "\t" + new FileInfo(lookupPath).FullName + "\n" + error + "\n";

                MessageBox.Show(errorMessage,
                    fMain.GetTranslation("Error while loading fonts"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);

                return;
            }

            FontFamily = pfc.Families[0];
            String name2 = FontFamily.GetName(0);
            FontStyle = System.Drawing.FontStyle.Regular;
            if (FontFamily.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                FontStyle = System.Drawing.FontStyle.Regular;
            else
                FontStyle = System.Drawing.FontStyle.Bold;

            int t1 = FontFamily.GetCellAscent(FontStyle);
            int t2 = FontFamily.GetCellDescent(FontStyle);
            int t3 = FontFamily.GetEmHeight(FontStyle);
            int t4 = FontFamily.GetLineSpacing(FontStyle);
        }
    }
}
