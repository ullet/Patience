/*
 *************************************************************************
 * Patience: An extensible graphical card game system.                   *
 * Version 0.1.0 (13 January 2005)                                       *
 * Copyright (C) 2005 Trevor Barnett                                     *
 *                                                                       *
 * This program is free software; you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation; either version 2 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * This program is distributed in the hope that it will be useful,       *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 * GNU General Public License for more details.                          *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program; if not, write to the Free Software           *
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 * USA                                                                   *
 *************************************************************************
 * File:          Patience.cs                                            *
 * Namespace:     SwSt                                                   *
 * Class:         Patience                                               *
 * Last modified: 13 January  2005                                       *
 * Description:   An extensible graphical card game system.              *
 *************************************************************************
 */
        
using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using SwSt;
using SwSt.Cards;
using SwSt.CardGames;
using SwSt.WindowsForms.Dialogs;
using Properties = Patience.Properties;

//-----------------------------------------------------------------//
        
namespace SwSt
{
    // ------------------------------------------------------ //
    
    public class Patience
    {
        private static string m_strPluginCardGameDir = "Games";
        
        public static ArrayList CheckDependencies(Assembly assembly)
        {
            ArrayList a_missingAssemblyNames = new ArrayList();
            
            CheckDependencies(assembly, a_missingAssemblyNames, new ArrayList());
            
            return a_missingAssemblyNames;
        }
        
        public static void CheckDependencies(Assembly assembly,
                                             ArrayList a_missingAssemblyNames,
                                             ArrayList a_checkedAssemblies)
        {
            AssemblyName[] a_assemblyNames = assembly.GetReferencedAssemblies();
            foreach (AssemblyName assemblyName in a_assemblyNames)
            {
                Assembly testAssembly = null;
                
                try
                {
                    if (a_checkedAssemblies.IndexOf(assemblyName.FullName) < 0)
                    {
                        // NB. use FullName as value identical for
                        // two instances of same Assembly, whereas 
                        // obviously two instances will not be equal
                        a_checkedAssemblies.Add(assemblyName.FullName);
                        testAssembly = Assembly.Load(assemblyName);
                        // check dependencies of this assembly
                        CheckDependencies(testAssembly, a_missingAssemblyNames,
                            a_checkedAssemblies);
                    }
                }
                catch
                {
                    // assembly not loaded
                    a_missingAssemblyNames.Add(assemblyName);
                }
                finally
                {
                    testAssembly = null;
                }
            }
        }
        
        //-----------------------------------------------------------------//
        // APPLICATION ENTRY POINT                                         //
        //-----------------------------------------------------------------//
        
        // I read somewhere that the STAThread attribute is needed if using
        // OpenFileDialog.  If not the attribute will be ignored so doesn't hurt.
        
        [STAThread]
        public static void Main(string[] args)
        {            
            // check have all required 'components'
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            
            ArrayList a_missingAssemblyNames = CheckDependencies(thisAssembly);
            
            // Display message and exit if something is missing
            if (a_missingAssemblyNames.Count > 0)
            {
                string strMissingAssemblies = "";
                foreach(AssemblyName assemblyName in a_missingAssemblyNames)
                {
                    if (strMissingAssemblies != "")
                    {
                        strMissingAssemblies += "\n";
                    }
                    strMissingAssemblies += "  -  " + assemblyName.Name + 
                        ".dll (version " + assemblyName.Version + ")";
                }
                MessageBox.Show("Unable to start application as the following "+
                                "required assemblies were not found:\n\n" + 
                                strMissingAssemblies, "Application Start Error");
                return;
            }
            
            string strCardSetPath;
            string strCardSetDirectoryPath;
            string strCardSetFileName;
            
            if (args.Length < 1)
            {
                strCardSetDirectoryPath = "";
                strCardSetFileName = "";
            }
            else
            {
                strCardSetPath = args[0];
                if (strCardSetPath == "")
                {
                    strCardSetDirectoryPath = "";
                    strCardSetFileName = "";
                }
                else
                {
                    // replace any / with \
                    strCardSetPath.Replace("/", "\\");
                    try
                    {
                        strCardSetPath = Path.GetFullPath(strCardSetPath);
                        strCardSetDirectoryPath = 
                            Path.GetDirectoryName(strCardSetPath);
                        strCardSetFileName = 
                            Path.GetFileName(strCardSetPath);
                    }
                    catch
                    {
                        strCardSetDirectoryPath = "";
                        strCardSetFileName = "";
                    }
                }
            }
            
            // Get plugins
            Type typICGC = typeof(SwSt.CardGames.ICardGameControl);
            Type typCtrl = typeof(Control);
            
            PluginList plugins = PluginManager.GetPlugins(
                        m_strPluginCardGameDir,
                        new Type[]{
                            typeof(SwSt.CardGames.ICardGameControl)},
                        new Type[]{
                            typeof(Control)});
            
            ArrayList a_CardGameControls = new ArrayList();
            foreach (Plugin plugin in plugins)
            {
                try
                {
                    ICardGameControl cgc = 
                        (ICardGameControl)plugin.Assembly.CreateInstance
                            (plugin.Type.FullName);
                    a_CardGameControls.Add(cgc);
                }
                catch
                {
                }
            }
            a_CardGameControls.Sort();
            
            if (a_CardGameControls == null || a_CardGameControls.Count < 1)
            {
                // no plugins available
                MessageBox.Show(
                    "No game modules found.\n" +
                    "Please try uninstalling then \n"+
                    "reinstalling the application.",
                    "Application Start Error");
                return;
            }
            
            // 'Dynamically' create instance of main window
            // so we can check all required DLLs are present
            // If use new or cast as type PatienceWindow and DLL containing
            // class CardGames.GameWindow is not present an exception
            // will be thrown before the above check can be mae
            Object window;
            try
            {
                window = thisAssembly.CreateInstance
                        ("SwSt.PatienceWindow", false, 
                         BindingFlags.CreateInstance, null,
                         new object[]
                         {strCardSetDirectoryPath, 
                          strCardSetFileName, "",
                          a_CardGameControls},
                         null, null);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Unable to open window\n\n"+
                    ex.GetType().ToString() + ": " + ex.Message,
                    "Application Start Error");

                return;
            }
            
            Application.Run((Form)window); 
        }
    }
    
    public class PatienceWindow : CardGameWindow
    {
        private ArrayList ma_CardGameControls = null;
        private Hashtable m_CardGameControlsHashtable = null;
        private string m_strSelectedGame = "";
        private MenuItem m_AboutGameMenuItem = null;
        private bool m_blnInitialised = false;
        
        public PatienceWindow(
            string strCardSetDirectoryPath,
            string strCardSetFileName,
            string strSelectedGame,
            ArrayList a_CardGameControls) : base()
        {
            this.SuspendLayout();
            
            this.StartPosition = FormStartPosition.CenterScreen;
            
            ma_CardGameControls = a_CardGameControls;
            m_CardGameControlsHashtable = new Hashtable();
            foreach (object obj in a_CardGameControls)
            {
                ICardGameControl cgc = 
                        (ICardGameControl)obj;
                if (cgc != null)
                {
                    if (!m_CardGameControlsHashtable.ContainsKey(cgc.GameTitle))
                    {
                        m_CardGameControlsHashtable.Add(cgc.GameTitle, cgc);
                    }
                }
            }
            
            m_strSelectedGame = strSelectedGame;
            
            Initialise();
            
            // disable plugin cardset menu items if no plugin available
            bool blnVisible = true;
            try
            {
                if (!SwSt.CardGames.CardSet.PluginCardSetAvailable)
                {
                    blnVisible = false;
                }
                
            }
            catch
            {
            }
            this.OpenCardSetMenuItem.Visible = blnVisible;
            this.UseDefaultCardSetMenuItem.Visible = blnVisible;
            
            // hide or disable unused menu items
            this.SaveSettingsOnExitMenuItem.Enabled = false;
            this.SaveSettingsMenuItem.Enabled = false;
            this.HelpContentsMenuItem.Enabled = false;
            
            this.LockCurrentSizeMenuItem.Visible = false;
            int intMenuItemIndex = 
                LockCurrentSizeMenuItem.Parent.MenuItems.IndexOf(
                    LockCurrentSizeMenuItem);
            if (intMenuItemIndex > 0)
            {
                if (LockCurrentSizeMenuItem.Parent.
                        MenuItems[intMenuItemIndex-1].Text == "-")
                {
                     LockCurrentSizeMenuItem.Parent.
                         MenuItems[intMenuItemIndex-1].Visible = false;
                }
            }
            
            this.ResumeLayout();
        }
        
        protected override void Initialise()
        {
            ICardGameControl cgc = 
                (ICardGameControl)m_CardGameControlsHashtable[m_strSelectedGame];
            if (cgc == null)
            {
                cgc = (ICardGameControl)ma_CardGameControls[0];
                m_strSelectedGame = cgc.GameTitle;
            }
            
            if (cgc == null)
            {
                MessageBox.Show(
                    "No game modules found.\n" +
                    "Please try uninstalling then \n"+
                    "reinstalling the application.",
                    "Application Start Error");
                this.Close();
                return;
            }
            
            this.CardGameControl = cgc;
            
            this.Text = "Patience [" + this.CardGameControl.GameTitle + "]" ;
            
            // get window icon from embeded resource
            Icon = GetWindowIcon();
            
            base.Initialise();
            
            // add names of all games to Game menu
            this.GameMenuItem.MenuItems.Add("-");
            foreach (object obj in ma_CardGameControls)
            {
                MenuItem mi = new MenuItem();
                mi.Text = ((ICardGameControl)obj).GameTitle;
                mi.RadioCheck = true;
                mi.Click += new EventHandler(SelectGame_Click);
                if (mi.Text == m_strSelectedGame)
                {
                    mi.Checked = true;
                }
                this.GameMenuItem.MenuItems.Add(mi);
            }
            
            // set default background style
            this.CardGameControl.Background.Style = BackgroundStyle.Gradient;
            this.CardGameControl.Background.GradientDirection = 
                GradientDirection.TopToBottom;
            this.CardGameControl.Background.Colours[0] = Color.Black;
            this.CardGameControl.Background.Colours[1] = Color.White;
            
            Size PreferedClientSize = this.CardGameControl.ClientSizeForCardSet;
            
            this.ClientSize = new Size(300,200);
                
            Size NonClientSize = new Size(
                this.Size.Width - this.ClientSize.Width,
                this.Size.Height - this.ClientSize.Height); 
            
            this.ClientSize = new Size(
                Math.Min(
                    PreferedClientSize.Width, 
                    SystemInformation.WorkingArea.Width -
                    NonClientSize.Width),
                Math.Min(
                    PreferedClientSize.Height, 
                    SystemInformation.WorkingArea.Height -
                    NonClientSize.Height));
            
            SetMinimumSize();
            
            SetControlCoords();
            
            this.Controls.Add((Control)this.CardGameControl);
            
            m_blnInitialised = true;
        }
        
        private void ReInitialise()
        {
            Background currentBackground = this.CardGameControl.Background;
            
            this.Controls.Remove((Control)this.CardGameControl);
            
            this.CardGameControl = 
                (ICardGameControl)m_CardGameControlsHashtable[m_strSelectedGame];
            
            this.Controls.Add((Control)this.CardGameControl);
            
            this.Text = "Patience [" + m_strSelectedGame + "]";
            
            this.AboutGameMenuItem.Text = "About " + m_strSelectedGame + "...";
            
            // check selected game menu item
            foreach (MenuItem mi in this.GameMenuItem.MenuItems)
            {
                if (m_CardGameControlsHashtable[mi.Text] != null)
                {
                    if (mi.Text == m_strSelectedGame)
                    {
                        mi.Checked = true;
                    }
                    else
                    {
                        mi.Checked = false;
                    }
                }
            }
            
            this.CardGameControl.Host = this;
                
            if (this.UseDefaultCardSet)
            {
                this.CardGameControl.LoadCardSet("");
            }
            else
            {
                this.CardGameControl.LoadCardSet(this.CardSetPath);
            }
            
            bool blnEnableValue = true;
            if (!this.CardGameControl.AutoFinishAvailable)
            {
                blnEnableValue = false;
            }
            AutoFinishMenuItem.Visible = blnEnableValue;
            int intMenuItemIndex = 
                AutoFinishMenuItem.Parent.MenuItems.IndexOf(
                    AutoFinishMenuItem);
            if (intMenuItemIndex > 0)
            {
                if (AutoFinishMenuItem.Parent.
                        MenuItems[intMenuItemIndex-1].Text == "-")
                {
                     AutoFinishMenuItem.Parent.
                         MenuItems[intMenuItemIndex-1].Visible = blnEnableValue;
                }
            }
            
            // set background style
            this.CardGameControl.Background.Style = currentBackground.Style;
            this.CardGameControl.Background.GradientDirection = 
                currentBackground.GradientDirection;
            this.CardGameControl.Background.Colours[0] = 
                currentBackground.Colours[0];
            this.CardGameControl.Background.Colours[1] = 
                currentBackground.Colours[1];
            this.CardGameControl.Background.Colour = 
                currentBackground.Colour;
            this.CardGameControl.Background.Bitmap = 
                currentBackground.Bitmap;
            this.CardGameControl.Background.Tile = 
                currentBackground.Tile;
            this.CardGameControl.Background.Stretch = 
                currentBackground.Stretch;
            this.CardGameControl.Background.Centre = 
                currentBackground.Centre;
            this.CardGameControl.Background.KeepAspectRatio = 
                currentBackground.KeepAspectRatio;
            this.CardGameControl.Background.FillWithGradient = 
                currentBackground.FillWithGradient;
                
            SetControlCoords();
            
            SetMinimumSize();
            
            //NB. slight bug here or in the control when 
            //switching games; layout very slightly different
            //second time around.
            //Cards set to default size not resized
            //Very obvious at 640x480
                
            this.CardGameControl.Restart();
        }
        
        private void SelectGame_Click(Object sender, EventArgs mev)
        {
            MenuItem mi = (MenuItem)sender;
            
            if (mi.Text == m_strSelectedGame)
            {
                // do nothing
                return;
            }
            
            // Game changed
            m_strSelectedGame = mi.Text;
            ReInitialise();
        }
        
        protected override void OnResize(EventArgs ev)
        {
            if (m_blnInitialised)
            {
                base.OnResize(ev);
            }
        }
        
        protected override void SetControlCoords()
        {
            ((Control)this.CardGameControl).Size = ClientSize;
        }
        
        protected static Icon GetWindowIcon()
        {
            return Properties.Resources.cardgames_small;
        }
        
        protected override void AboutMenuItem_Click(
                    object sender, System.EventArgs e)
        {
            AboutPatience();
        }
        
        private void AboutGameMenuItem_Click(
                    object sender, System.EventArgs e)
        {
            AboutGame();
        }
        
        private void AboutPatience()
        {
            AboutDialog dlgAbout = new AboutDialog();
            
            dlgAbout.MediumFont = 
                new Font(dlgAbout.SmallFont, 
                         dlgAbout.SmallFont.Style | FontStyle.Bold);
            
            dlgAbout.WindowTitle = "About Patience";
            
            dlgAbout.AddLine(
                "Patience", 
                "", -1, -1, 3, true);
            dlgAbout.AddLine();
            dlgAbout.AddLine(
                "Version " + 
                Assembly.GetExecutingAssembly().GetName().Version.ToString(), 
                "", -1, -1, 1, true); 
            dlgAbout.AddLine();
            dlgAbout.AddLine("Copyright (C) 2005 Trevor Barnett", true);
            dlgAbout.AddLine();
            dlgAbout.AddLine(
                "email: swst@e381.net", 
                "mailto:swst@e381.net",
                7, 13, 1, true);
            dlgAbout.AddLine(
                "web: www.e381.net", 
                "http://www.e381.net",
                5, 12, 1, true);
                
            // Add lines about cardset formats
            PluginCardSetList a_CardSetFormats = new PluginCardSetList();
            if (this.CardGameControl.CardSet.ModuleName != "" &&
                    this.CardGameControl.CardSet.Version != "")
            {
                a_CardSetFormats.Add(this.CardGameControl.CardSet);
            }
            if (this.CardGameControl.CardSet.ChildCardSets != null)
            {
                // "wrapper" cardset format
                // assume non of child cardset formats are
                // themselves "wrappers"
                foreach (ICardSet cardset in 
                    this.CardGameControl.CardSet.ChildCardSets)
                {
                    if (cardset.ModuleName != "" &&
                        cardset.Version != "")
                    {
                        a_CardSetFormats.Add(cardset);
                    }
                }
            }
            
            if (a_CardSetFormats.Count > 0)
            {
                // have at least one cardset to show details of
                dlgAbout.AddLine();
                dlgAbout.AddLine("-");
                dlgAbout.AddLine();
                dlgAbout.AddLine("Supported Cardset Formats:", "", -1, -1, 1, false);
                dlgAbout.AddLine();
                
                int intCounter = 0;
                foreach (ICardSet cardset in a_CardSetFormats)
                {
                    dlgAbout.AddLine(cardset.ModuleName, "", -1, -1, 2, true);
                    dlgAbout.AddLine();
                    dlgAbout.AddLine(cardset.Version, true);
                    
                    if (this.CardGameControl.Copyright != "")
                    {
                        dlgAbout.AddLine();
                        dlgAbout.AddLine(
                            cardset.Copyright, true);
                    }
                    
                    if (cardset.ContactEmail != "" ||
                        cardset.Website != "")
                    {
                        dlgAbout.AddLine();
                    
                        string strEmailDisplay = cardset.ContactEmail;
                        if (strEmailDisplay != "")
                        {
                            string strEmailLink = strEmailDisplay;
                            if (strEmailDisplay.Substring(0,7).ToLower() != "mailto:")
                            {
                                strEmailLink = "mailto:"+strEmailDisplay;
                            }
                            
                            dlgAbout.AddLine(
                                "email: "+strEmailDisplay,
                                strEmailLink,
                                7, strEmailDisplay.Length, 1, true);
                        }
                        
                        string strWebDisplay = cardset.Website;
                        if (strWebDisplay != "")
                        {
                            string strWebLink = strWebDisplay;
                            if (strWebDisplay.Substring(0,7).ToLower() != "http://")
                            {
                                strWebLink = "http://"+strWebDisplay;
                            }
                                
                            dlgAbout.AddLine(
                                "web: " + strWebDisplay, 
                                strWebLink,
                                5, strWebDisplay.Length, 1, true);
                        }
                    }
                    
                    if (cardset.FormatCopyright != "")
                    {
                        dlgAbout.AddLine();
                        dlgAbout.AddLine(
                            cardset.FormatCopyright, true);
                    }
                    
                    if (cardset.ContactEmail != "" ||
                        cardset.Website != "")
                    {
                        dlgAbout.AddLine();
                    
                        string strFEmailDisplay = cardset.FormatContactEmail;
                        if (strFEmailDisplay != "")
                        {
                            string strFEmailLink = strFEmailDisplay;
                            if (strFEmailDisplay.Substring(0,7).ToLower() != "mailto:")
                            {
                                strFEmailLink = "mailto:"+strFEmailDisplay;
                            }
                            
                            dlgAbout.AddLine(
                                "email: "+strFEmailDisplay,
                                strFEmailLink,
                                7, strFEmailDisplay.Length, 1, true);
                        }
                        
                        string strFWebDisplay = cardset.FormatWebsite;
                        if (strFWebDisplay != "")
                        {
                            string strFWebLink = strFWebDisplay;
                            if (strFWebDisplay.Substring(0,7).ToLower() != "http://")
                            {
                                strFWebLink = "http://"+strFWebDisplay;
                            }
                                
                            dlgAbout.AddLine(
                                "web: " + strFWebDisplay, 
                                strFWebLink,
                                5, strFWebDisplay.Length, 1, true);
                        }
                    }
                    dlgAbout.AddLine();
                    
                    if (intCounter != 
                        a_CardSetFormats.Count-1)
                    {
                        // not last, so draw divider line
                        dlgAbout.AddLine("[line 50]");
                        dlgAbout.AddLine();
                    }
                    
                    intCounter++;
                    
                } // next cardset format
            }
                
            dlgAbout.ShowAbout();
            
            dlgAbout.Dispose();
        }
        
        public override void AboutGame()
        {
            if (this.CardGameControl.GameTitle == "" ||
                this.CardGameControl.Version == "")
            {
                // not enough info
                return;
            }   
            
            AboutDialog dlgAbout = new AboutDialog();
            dlgAbout.AddLine(
                this.CardGameControl.GameTitle, 
                "", -1, -1, 3, true);
            dlgAbout.AddLine();
            dlgAbout.AddLine(
                this.CardGameControl.Version, 
                "", -1, -1, 1, true);
            
            if (this.CardGameControl.Copyright != "")
            {
                dlgAbout.AddLine();
                dlgAbout.AddLine(
                    this.CardGameControl.Copyright, true);
            }
            
            if (this.CardGameControl.ContactEmail != "" ||
                this.CardGameControl.Website != "")
            {
                dlgAbout.AddLine();
            
                string strEmailDisplay = this.CardGameControl.ContactEmail;
                if (strEmailDisplay != "")
                {
                    string strEmailLink = strEmailDisplay;
                    if (strEmailDisplay.Substring(0,7).ToLower() != "mailto:")
                    {
                        strEmailLink = "mailto:"+strEmailDisplay;
                    }
                    
                    dlgAbout.AddLine(
                        "email: "+strEmailDisplay,
                        strEmailLink,
                        7, strEmailDisplay.Length, 1, true);
                }
                
                string strWebDisplay = this.CardGameControl.Website;
                if (strWebDisplay != "")
                {
                    string strWebLink = strWebDisplay;
                    if (strWebDisplay.Substring(0,7).ToLower() != "http://")
                    {
                        strWebLink = "http://"+strWebDisplay;
                    }
                        
                    dlgAbout.AddLine(
                        "web: " + strWebDisplay, 
                        strWebLink,
                        5, strWebDisplay.Length, 1, true);
                }
            }
                
            string strAdditionalInfo = this.CardGameControl.AdditionalInfo;
            
            if (strAdditionalInfo != "")
            {
                string[] a_strLines = strAdditionalInfo.Split('\n');
                dlgAbout.AddLine();
                foreach (string strLine in a_strLines)
                {
                    string strLineLessFormatting = strLine;
                    bool blnCentre = false;
                    int intFontSize = 1;
                    
                    if (strLineLessFormatting.Length >= 8 &&
                        strLineLessFormatting.Substring(0,8).ToLower() == "[centre]")
                    {
                        blnCentre = true;
                        strLineLessFormatting =
                            strLineLessFormatting.Substring(8);
                    }
                    if (strLineLessFormatting.Length >= 5 &&
                        strLineLessFormatting.Substring(0,5).ToLower() == "[big]")
                    {
                        intFontSize = 3;
                        strLineLessFormatting =
                            strLineLessFormatting.Substring(5);
                    }
                    if (strLineLessFormatting.Length >= 8 &&
                        strLineLessFormatting.Substring(0,8).ToLower() == "[medium]")
                    {
                        intFontSize = 2;
                        strLineLessFormatting =
                            strLineLessFormatting.Substring(8);
                    }
                    if (strLineLessFormatting.Length >= 7 &&
                        strLineLessFormatting.Substring(0,7).ToLower() == "[small]")
                    {
                        intFontSize = 1;
                        strLineLessFormatting =
                            strLineLessFormatting.Substring(7);
                    }
                    dlgAbout.AddLine(
                        strLineLessFormatting, 
                        "",
                        -1,
                        -1,
                        intFontSize,
                        blnCentre);
                }
            }
                
            dlgAbout.WindowTitle = "About " + this.CardGameControl.GameTitle;
            
            dlgAbout.ShowAbout();
            
            dlgAbout.Dispose();
        }
        
        protected override void SetMenus()
        {
            base.SetMenus();
            this.AboutMenuItem.Text = "About Patience...";
            
            if (this.CardGameControl.GameTitle != "" &&
                this.CardGameControl.Version != "")
            {
                // insert "About Game" menu item after "About Patience" menu item
                Menu mnuParent = this.AboutMenuItem.Parent;
                int intAboutIndex = mnuParent.MenuItems.IndexOf(this.AboutMenuItem);
                
                MenuItem miAboutGame = new MenuItem();
                miAboutGame.Text = "About " + this.CardGameControl.GameTitle + "...";
                miAboutGame.Click += new EventHandler(AboutGameMenuItem_Click);
                this.AboutGameMenuItem = miAboutGame;
                
                mnuParent.MenuItems.Add(intAboutIndex+1, miAboutGame);
            }
        }
        
        protected MenuItem AboutGameMenuItem
        {
            get
            {
                return m_AboutGameMenuItem;
            }
            set
            {
                m_AboutGameMenuItem = value;
            }
        }
    }
}