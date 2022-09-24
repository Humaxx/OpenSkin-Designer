using OpenSkinDesigner.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
//using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OpenSkinDesigner.Frames
{
    partial class fAbout : Form
    {
        public fAbout()
        {
            InitializeComponent();
            SetLanguage();
            this.Text = String.Format(fMain.GetTranslation("Information about") + " {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = AssemblyDescription + Environment.NewLine +
                Environment.NewLine + "Changelog:" + Environment.NewLine + Environment.NewLine +
                "v3.1.0.0 by Jason Hood" + Environment.NewLine +
                "- Add the GPLv3 license file" + Environment.NewLine +
                "- Replace 'Suchen' with 'Search'" + Environment.NewLine +
                "- Restore the cursor on leaving the preview" + Environment.NewLine +
                "- Tweak tree labels" + Environment.NewLine +
                "- Support for 'alias'" + Environment.NewLine +
                "- Add skin path if file doesn't exist" + Environment.NewLine +
                "- Disable 'Save As'" + Environment.NewLine +
                "- Allow Escape to reset the search" + Environment.NewLine +
                "- Support 'include' and 'panel' layout" + Environment.NewLine +
                "- Set resolution exactly, starting zoomed" + Environment.NewLine +
                "- Recognise undefined background color in properties" + Environment.NewLine +
                "- Fix typo in event duration converter" + Environment.NewLine +
                "- Use bright magenta (#ff00ff) for undefined colors" + Environment.NewLine +
                "- Recognise relative elements" + Environment.NewLine +
                "- Add 'ShortFullDate' converter" + Environment.NewLine +
                "- Fix setting colors" + Environment.NewLine +
                "- Fix conditional labels" + Environment.NewLine + Environment.NewLine +
                "v3.1.0.1 by Humax" + Environment.NewLine +
                "- Swapped tooltips 'Undo' and 'Delete Element' " + Environment.NewLine +
                "- 'Open Project' - dialog adapted" + Environment.NewLine +
                "- Bug fix - Zooming is only possible if a skin was loaded" + Environment.NewLine +
                // "- Bug fix - MinimumValue 'Trackbar' and 'NumericUpDown'" + Environment.NewLine +
                "- 'File' 'Open' - Button stays active even a skin was loaded" + Environment.NewLine +
                "- Tree-Image-Icon 'skin' fixed" + Environment.NewLine +
                "- Support for 'KravenFHDEventTime'" + Environment.NewLine +
                "- Bug fix in Fonts-Dialog (renaming Fonts-Name in ListView is no more possible - as it should be!)" + Environment.NewLine +
                "- Set minimum window-size's and don't allow to minimize / maximize the windows" + Environment.NewLine +
                "- Fixed 'Save as...'-Dialog" + Environment.NewLine +
                "- Support FHD-background-image (idea from digiteng)" + Environment.NewLine +
                "- Using variables for 'size' and 'position'" + Environment.NewLine +
                "- If a color has been defined twice, you can now decide if the color should be removed (recommended)." + Environment.NewLine +
                "  This will save the skin and reload it. Otherwise, the color will be removed if you save it manually," + Environment.NewLine +
                "  but in this case the color will be preserved in the Treeview until you reload!" + Environment.NewLine + Environment.NewLine +
                "v3.1.0.2 by Humax" + Environment.NewLine +
                "- Fixed font preview (a text box cannot display the font correctly, a picturebox can!)" + Environment.NewLine +
                "- Fixed unhandled exception when trying to save in Codeeditor when no line is selected" + Environment.NewLine +
                "- Fixed a bug that prevented searching for the specified fonts in the skin folder." + Environment.NewLine +
                "- Added Sliders in 'Color Settings Dialog' to adjust values for ARGB" + Environment.NewLine +
                "- Fixed some unhandled exceptions / bugs in 'Color Settings Dialog' - when no Color is selected or spaces are used in names" + Environment.NewLine +
                "- Fixed the 'remove'-color button in 'Color Settings Dialog' (previously it had no function)" + Environment.NewLine +
                "- In 'Color Settings Dialog' when choosing a color via Palette, the transparency is set to '0'" + Environment.NewLine +
                "- Fixed a bug that prevented searching for the specified fonts in the skin folder." + Environment.NewLine + Environment.NewLine +
                "v3.1.0.3 by Scrounger" + Environment.NewLine +
                "- Fonts: loading size from xml implemented" + Environment.NewLine +
                "- Loading predefined fonts from <fonts> added" + Environment.NewLine +
                "- Converter bug fixes: name ServiceX added - Exception in VerticalEPGView_FHD" + Environment.NewLine +
                "- Show dummy text for rendered elements added" + Environment.NewLine +
                "- Converter bug fix for AudioZap" + Environment.NewLine +
                "- Show dummy text for rendered elements " + Environment.NewLine +
                "- Show name or source attribute if available" + Environment.NewLine +
                "- Screen has not title attribute, fallback to name attribute" + Environment.NewLine +
                "- Render VRunningText added" + Environment.NewLine +
                "- Render MetrixReloadedScreenNameLabel added" + Environment.NewLine +
                "- Show picon bug fixed" + Environment.NewLine + Environment.NewLine +
                "v3.2.0.0 by Scrounger" + Environment.NewLine +
                "- Converter: support for 'FullDescription' added" + Environment.NewLine +
                "- Resize picon on element size change" + Environment.NewLine +
                "- Use attribute scale for ePixmap & widget which have 'pixmap' attribute." + Environment.NewLine +
                "- Converter MovieInfo added" + Environment.NewLine +
                "- Show images for widgets wiht any render and 'path' attribute" + Environment.NewLine +
                "- Show EventImage if render attribute contains 'eventimage'" + Environment.NewLine +
                "- Show XHDPicon if render attribute contains 'xhdpicon'" + Environment.NewLine +
                "- Show images with 'pixmaps' attribute" + Environment.NewLine + Environment.NewLine +
                "v3.2.2.0 by Scrounger" + Environment.NewLine +
                "- cConverterSimplePresets added" + Environment.NewLine +
                "- Alias font bug fixes->gobal loading / usage added" + Environment.NewLine +
                "- Fonts sorting added" + Environment.NewLine +
                "- Label: font bug fix property grid->change font or fontsize" + Environment.NewLine +
                "- ListBox font added to property grid" + Environment.NewLine +
                "- Show font style and size for listboxes" + Environment.NewLine +
                "- Font bug fix-> catch exception if font is not defined or exist" + Environment.NewLine +
                "- Label metrixreloadedvrunningtext added" + Environment.NewLine +
                "- ListBox: count of entries to show bug fixed" + Environment.NewLine +
                "- sAttributePixmap: element with attribute 'path'->bug fix if skinPath is part of attribute path" + Environment.NewLine +
                "- converterSimple.xml: MetrixReloaded converters added" + Environment.NewLine +
                "- ListBox: Show entries added" + Environment.NewLine + Environment.NewLine +
                "v3.2.3.0 by Humax" + Environment.NewLine +
                "- Undefined colors are added alternatively ('#' is not replaced by 'un')'" + Environment.NewLine +
                "- Added a option how to add undefined colors (with '#' or with 'un')" + Environment.NewLine +
                "- Fixed unhandled exception if a borderset-file isn't existing" + Environment.NewLine +
                "- Bug fix in 'Windowstyle-preview': Now displaying correct borderset and filename" + Environment.NewLine +
                "- Fix unhandled exception in 'Windowstyle-preview' if no borderstyle is declared in skin.xml" + Environment.NewLine +
                "- Text-preview: using lcd.ttf if declared font is not found" + Environment.NewLine +
                "- Fixed a bug that probably exists since 3.1.0.3. Font preview is now again working" + Environment.NewLine +
                "- Editor: now showing up to 99999 line numbers instead of max 999" + Environment.NewLine +
                "- Editor: background color changed for better contrast" + Environment.NewLine +
                "- Added VTi-Fonts" + Environment.NewLine +
                "- Converter bug fixes: 'TimeshiftService' added to prevent a exception in 'Timeshiftstate'" + Environment.NewLine + Environment.NewLine +
                "v3.2.3.1 by Humax" + Environment.NewLine +
                "- More sources rendered as listbox" + Environment.NewLine +
                "- Fix unhandled exception when Source = null" + Environment.NewLine + Environment.NewLine +
                "v3.2.3.2 by Humax" + Environment.NewLine +
                "- Fixed unhandled exception if no Font is declared or only alias - then using 'lcd.ttf'" + Environment.NewLine +
                "- Fixed unhandled exceptions if a color is missing or declared with 'foregroundColors'" + Environment.NewLine +
                "- Ask to show messageboxes again or not" + Environment.NewLine +
                "- Added option to set 'Fallback-Color', which is used for previewing some text" + Environment.NewLine +
                "- Bugfix: show picon also when a path is set" + Environment.NewLine + Environment.NewLine +
                "v3.2.3.3 by Humax" + Environment.NewLine +
                "- Fixed path not found exception" + Environment.NewLine +
                "- Updated converter.xml" + Environment.NewLine +
                "- Added some entries to attribut-list like 'foregroundColors' 'options' 'pixmaps' and more" + Environment.NewLine +
                "- Added a option to enable showing full attribut-list" + Environment.NewLine +
                "- Autocomplete attribut list - max preview set to 15 instead of 5" + Environment.NewLine +
                "- Added render 'speedyAXBlueRunningText'" + Environment.NewLine + Environment.NewLine +
                "v3.2.3.4 by Humax" + Environment.NewLine +
                "- Added render 'ChamaeleonRunningText'" + Environment.NewLine +
                "- Bugfix: pixmap path" + Environment.NewLine +
                "- Added all renders containing 'runningtext'" + Environment.NewLine +
                "- Handling all renders containing 'list' as listbox" + Environment.NewLine +
                "- Notifying about unsafed changes" + Environment.NewLine +
                "- If pixmap have a path without specified filename, take random image" + Environment.NewLine + Environment.NewLine +
                "v3.2.3.5 by Humax" + Environment.NewLine +
                "- Fixed an unhandled exception if image is corrupt" + Environment.NewLine +
                "- Only take 'jpg'; 'jpeg' and 'png' for random image selection" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.0 by Humax" + Environment.NewLine +
                "- Fixed Typos" + Environment.NewLine +
                "- Fixed unhandled exception in 'Color Dialog'" + Environment.NewLine +
                "- Allow only valid characters in Textboxes in 'Color Dialog'" + Environment.NewLine +
                "- Support for language file" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.1 by Humax" + Environment.NewLine +
                "- Add search for searching text in code editor" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.2 by Humax" + Environment.NewLine +
                "- Upgraded search-function" + Environment.NewLine +
                "- Added missing translation" + Environment.NewLine +
                "- Fixed text from 'Open-Button' in 'Open-Dialog'" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.3 by Humax" + Environment.NewLine +
                "- Added missing translations" + Environment.NewLine +
                "- Settings are now saved" + Environment.NewLine +
                "- Multilanguage support" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.4 by Humax" + Environment.NewLine +
                "- Added missing translations" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.5 by Humax" + Environment.NewLine +
                "- Display skin-name" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.6 by Humax" + Environment.NewLine +
                "- Add turkish language (thanks to 'audi06')" + Environment.NewLine +
                "- Bugfix: restoring language only searches for first language file in languages-directory" + Environment.NewLine +
                "- Translate existing element-items after changing language" + Environment.NewLine +
                "- Fixed unhandled exception when using right-click in designer" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.7 by Humax" + Environment.NewLine +
                "- Fixed polish language" + Environment.NewLine +
                "- Added missing translations" + Environment.NewLine +
                "- Bugfix: now displaying an error message if a font is not valid" + Environment.NewLine +
                "- Add albanian language (thanks to 'kqiqi1')" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.8 by Humax" + Environment.NewLine +
                "- Added missing translations" + Environment.NewLine +
                "- Added 'ExtEvent' to converter.xml" + Environment.NewLine +
                "- Bugfix: Now also a notification is shown, if colors are changed" + Environment.NewLine +
                "- Add options to show notifications about unsafed changes" + Environment.NewLine + Environment.NewLine +
                "v3.2.4.9 by Humax" + Environment.NewLine +
                "- Using 'delete'-key to delete selected element" + Environment.NewLine + Environment.NewLine +
                "v3.2.5.0 by Humax" + Environment.NewLine +
                "- Bugfix: Dont delete root-node" + Environment.NewLine +
                "- Bugfix: 'Color-Dialog': changed 'Change' - button to 'Rename' - button" + Environment.NewLine +
                "- Bugfix: 'Color-Dialog': changing a color now triggers unsafed-changes - notification" + Environment.NewLine +
                "- Closing 'Color-Dialog' instead of hiding" + Environment.NewLine +
                "- Nomore saveing and reloading needed if a color is defined two times." + Environment.NewLine +
                "- Added 'experimental delete-mode'" + Environment.NewLine + Environment.NewLine +
                "v3.2.5.1 by Humax" + Environment.NewLine +
                 "- Bugfix: fixed unhandled exception if file (include) was not found" + Environment.NewLine +
                 "- Bugfix: fixed unhandled exception if * is used for integer value" + Environment.NewLine +
                 "- Added an example in converterSimple.xml for converter-preview-text" + Environment.NewLine + Environment.NewLine +
                 "v3.2.5.2 by Humax" + Environment.NewLine +
                 "- Typos" + Environment.NewLine +
                 "- Added missing translations" + Environment.NewLine +
                 "- Added an option for linewrapping in code - editor" + Environment.NewLine + Environment.NewLine +
                 "v3.2.5.3 by Humax" + Environment.NewLine +
                 "- Bugfix: fixed unhandled exception if using delete - key without selected item" + Environment.NewLine +
                 "- Bugfix: using delete-key no longer deletes a selected item in propertygrid" + Environment.NewLine + Environment.NewLine +
                 "v3.2.5.4 by Humax" + Environment.NewLine +
                 "- Bugfix: Selected Treeviewnode was deleted when pressing any key in Designer - Mode" + Environment.NewLine + Environment.NewLine +
                 "v3.2.5.5 by Humax" + Environment.NewLine +
                 "- Add an option to not replace color beginning with '#'" + Environment.NewLine + Environment.NewLine +
                 "v3.2.5.6 by Humax" + Environment.NewLine +
                 "- Support for QHD (WQHD) and 4K UHD (Ultra HD)" + Environment.NewLine + Environment.NewLine +
                 "v3.2.5.7 by Humax" + Environment.NewLine +
                 "- Support for resolution 3200 x 1800" + Environment.NewLine +
                 "- Fixed unhandled Exception when borderset has no filename" + Environment.NewLine + Environment.NewLine +
                 "v3.2.5.8 by Humax" + Environment.NewLine +
                 "- Fixed unhandled Exception when borderset path has not been specified" + Environment.NewLine + Environment.NewLine +
                 "v3.2.5.9 by Humax" + Environment.NewLine +
                 "- Fixed borderset - bug" + Environment.NewLine +
                 "- Application will be terminated if a '.svg' graphic is used in the 'borderset's" + Environment.NewLine + Environment.NewLine +
                  "v3.2.6.0 by Humax" + Environment.NewLine +
                 "- Undo application termination if a '.svg' graphic is used in the 'borderset's" + Environment.NewLine +
                 "- If '.svg' graphic is used, the application searches for a corresponding '.png' graphic" + Environment.NewLine + Environment.NewLine +
                 "v3.2.6.1 by Humax" + Environment.NewLine +
                 "- Added an option to hide attribut-list in code-editor" + Environment.NewLine +
                 "- Updated language-files" + Environment.NewLine +
                 "- After opening the skin, the main node is displayed in the code editor" + Environment.NewLine +
                 "- Bugfix: Notification about unsafed changes, hasn't work in every case" + Environment.NewLine + Environment.NewLine +
                 "v3.2.6.2 by Humax" + Environment.NewLine + 
                 "- Fixed the display of the error display of the code view";
        }

        private void SetLanguage()
        {
            okButton.Text = fMain.GetTranslation("OK");          
        }

        #region Assemblyattributaccessoren

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
