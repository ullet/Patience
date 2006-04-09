/*
 *************************************************************************
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
 * File:          CardGameWindow.cs                                      *
 * Namespace:     SwSt.CardGames                                         *
 * Last modified: 13 January 2005                                        *
 * Class:         CardGameWindow                                         *
 * Description:   Abstract class derived from System.Windows.Forms.Form, *
 *                provides generic functionality for forms used to       *
 *                display a graphical card game.                         *
 *************************************************************************
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.Resources;
using System.Reflection;
using SwSt;
using SwSt.Cards;
using SwSt.CardGames;

//-----------------------------------------------------------------//       
        
namespace SwSt
{       
    namespace CardGames
    {
        
        //-----------------------------------------------------------------//
        
        public interface ICardGameHost
        {
            void GameWon();
            void AboutGame();
        }
        
        //-----------------------------------------------------------------//
        
        public abstract class CardGameWindow : 
            System.Windows.Forms.Form, ICardGameHost
        {
            private ICardGameControl m_GameControl = null;
            private string m_strBackgroundBitmapPath = "";
            private int[]  ma_intCustomColours = new int[16];
            private string m_strCardSetPath = "";
            private string m_strCardSetDirectoryPath = "";
            private string m_strCardSetFileName = "";
            
            private string m_strDefaultCardSetPath = "";
            
            private int m_intFilterIndex = 0;
        
            // menus
            private MainMenu m_mainMenu                         = new MainMenu();
            private MenuItem m_menuItemFile                     = new MenuItem();
            private MenuItem m_menuItemOpen                     = new MenuItem();
            private MenuItem m_menuItemSave                     = new MenuItem();
            private MenuItem m_menuItemExit                     = new MenuItem();
            private MenuItem m_menuItemGame                     = new MenuItem();
            private MenuItem m_menuItemRestart                  = new MenuItem();
            private MenuItem m_menuItemAutoFinish               = new MenuItem();
            private MenuItem m_menuItemOptions                  = new MenuItem();
            private MenuItem m_menuItemBackground               = new MenuItem();
            private MenuItem m_menuItemUseDefaultCardSet        = new MenuItem();
            private MenuItem m_menuItemSaveSettingsOnExit       = new MenuItem();
            private MenuItem m_menuItemLockCurrentSize          = new MenuItem();
            private MenuItem m_menuItemResize                   = new MenuItem();
            private MenuItem m_menuItemStandard                 = new MenuItem();
            private MenuItem m_menuItemSmall                    = new MenuItem();
            private MenuItem m_menuItemMinimum                  = new MenuItem();
            private MenuItem m_menuItemMaximum                  = new MenuItem();
            private MenuItem m_menuItemHelp                     = new MenuItem();
            private MenuItem m_menuItemHelpContents             = new MenuItem();
            private MenuItem m_menuItemAbout                    = new MenuItem();
                    
            public CardGameWindow()
            {
            }
            
            protected virtual void SetMinimumSize()
            {
                // set minimum window size to minimum allowed by control
                int intWidthDiff = this.Size.Width - this.ClientSize.Width;
                int intHeightDiff = this.Size.Height - this.ClientSize.Height;
                this.MinimumSize = new Size(
                    this.CardGameControl.MinimumClientSizeForCardSet.Width +
                    intWidthDiff,
                    this.CardGameControl.MinimumClientSizeForCardSet.Height +
                    intHeightDiff);
            }
            
            protected virtual void Initialise()
            {
                SetMenus();
             
                this.CardGameControl.Host = this;
                
                this.CardGameControl.CardSet.Load(m_strDefaultCardSetPath);
                                
                if (!this.CardGameControl.AutoFinishAvailable)
                {
                    m_menuItemAutoFinish.Visible = false;
                    int intMenuItemIndex = 
                        m_menuItemAutoFinish.Parent.MenuItems.IndexOf(
                            m_menuItemAutoFinish);
                    if (intMenuItemIndex > 0)
                    {
                        if (m_menuItemAutoFinish.Parent.
                                MenuItems[intMenuItemIndex-1].Text == "-")
                        {
                            m_menuItemAutoFinish.Parent.
                                MenuItems[intMenuItemIndex-1].Visible = false;
                        }
                    }
                }
                
                // initialise custom colours to white
                // NB. for custom colours used by ColorDialog, 
                // byte order is B G R; no alpha value is used
                int intWhite = 
                    (Color.White.B<<16)+(Color.White.G<<8)+Color.White.R;
                for (int intIndex=0; intIndex<16; intIndex++)
                {
                    ma_intCustomColours[intIndex] = intWhite;
                }
            }
            
            protected virtual ICardGameControl CardGameControl
            {
                get
                {
                    return m_GameControl;
                }
                set
                {
                    m_GameControl = value;
                }
            }
            
            protected abstract void SetControlCoords();
            
            protected virtual void SetMenus()
            {
                // Add menus and menu items in order
                m_mainMenu.MenuItems.Add(m_menuItemFile);
                  m_menuItemFile.MenuItems.Add(m_menuItemOpen);
                  m_menuItemFile.MenuItems.Add(m_menuItemSave);
                  m_menuItemFile.MenuItems.Add("-");
                  m_menuItemFile.MenuItems.Add(m_menuItemExit);
                m_mainMenu.MenuItems.Add(m_menuItemGame);
                  m_menuItemGame.MenuItems.Add(m_menuItemRestart);
                  m_menuItemGame.MenuItems.Add("-");
                  m_menuItemGame.MenuItems.Add(m_menuItemAutoFinish);
                m_mainMenu.MenuItems.Add(m_menuItemOptions);
                  m_menuItemOptions.MenuItems.Add(m_menuItemBackground);
                  m_menuItemOptions.MenuItems.Add(m_menuItemUseDefaultCardSet);
                  m_menuItemOptions.MenuItems.Add(m_menuItemSaveSettingsOnExit);
                  m_menuItemOptions.MenuItems.Add("-");
                  m_menuItemOptions.MenuItems.Add(m_menuItemLockCurrentSize);
                m_mainMenu.MenuItems.Add(m_menuItemHelp);
                  m_menuItemHelp.MenuItems.Add(m_menuItemHelpContents);
                  m_menuItemHelp.MenuItems.Add("-");
                  m_menuItemHelp.MenuItems.Add(m_menuItemAbout);          
                    
                // Set the caption for the top-level menu items.
                m_menuItemFile.Text    = "File";
                m_menuItemGame.Text    = "Game";
                m_menuItemOptions.Text = "Options";
                m_menuItemHelp.Text    = "Help";
                
                // Set the File sub menu items.
                m_menuItemOpen.Text = "Open Cardset";
                m_menuItemOpen.Shortcut = Shortcut.CtrlO;
                m_menuItemOpen.Click += new System.EventHandler(this.OpenCardsetMenuItem_Click);
                
                m_menuItemSave.Text = "Save Settings";
                m_menuItemSave.Shortcut = Shortcut.CtrlS;
                m_menuItemSave.Click += new System.EventHandler(this.SaveSettingsMenuItem_Click);
                
                m_menuItemExit.Text = "Exit";
                m_menuItemExit.Shortcut = Shortcut.CtrlX;
                m_menuItemExit.Click += new System.EventHandler(this.ExitMenuItem_Click);
                            
                // Set the Game sub menu items
                m_menuItemRestart.Text = "Restart";
                m_menuItemRestart.Click += new System.EventHandler(
                    this.RestartMenuItem_Click);
                
                m_menuItemAutoFinish.Text = "Auto Finish";
                m_menuItemAutoFinish.Click += new System.EventHandler(
                    this.AutoFinishMenuItem_Click);
                
                // Set the Options sub menu items
                m_menuItemBackground.Text = "Background...";
                m_menuItemBackground.Click += new System.EventHandler(
                    this.BackgroundMenuItem_Click);
                
                m_menuItemUseDefaultCardSet.Text = "Use Default Cardset";
                m_menuItemUseDefaultCardSet.Checked = true;
                m_menuItemUseDefaultCardSet.Click += new System.EventHandler(
                    this.UseDefaultCardSetMenuItem_Click);
                
                m_menuItemSaveSettingsOnExit.Text = "Save Settings On Exit";
                m_menuItemSaveSettingsOnExit.Checked = false;
                m_menuItemSaveSettingsOnExit.Click += new System.EventHandler(
                    this.SaveSettingsOnExitMenuItem_Click);
                
                m_menuItemLockCurrentSize.Text = "Lock Current Size";
                m_menuItemLockCurrentSize.Checked = false;
                m_menuItemLockCurrentSize.Click += new System.EventHandler(
                    this.LockCurrentSizeMenuItem_Click);
                
                m_menuItemResize.Text = "Resize";
                
                // Set the Options->resize sub menu items
                m_menuItemStandard.Text = "Standard";
                m_menuItemStandard.Click += new System.EventHandler(
                    this.ResizeStandardMenuItem_Click);
                
                m_menuItemSmall.Text = "Small";
                m_menuItemSmall.Click += new System.EventHandler(
                    this.ResizeSmallMenuItem_Click);
                
                m_menuItemMinimum.Text = "Minimum";
                m_menuItemMinimum.Click += new System.EventHandler(
                    this.ResizeMinimumMenuItem_Click);
                
                m_menuItemMaximum.Text = "Maximum";
                m_menuItemMaximum.Click += new System.EventHandler(
                    this.ResizeMaximumMenuItem_Click);
                
                // Set the Help sub menu items
                
                m_menuItemHelpContents.Text = "Contents";
                m_menuItemHelpContents.Click += new System.EventHandler(
                    this.HelpContentsMenuItem_Click);
                
                m_menuItemAbout.Text = "About...";
                m_menuItemAbout.Click += new System.EventHandler(
                    this.AboutMenuItem_Click);
               
                this.Menu = m_mainMenu;
            }
            
            public int[] CustomColors
            {
                get
                {
                    return ma_intCustomColours;
                }
                set
                {
                    for (int intIndex = 0; 
                         intIndex < Math.Min(ma_intCustomColours.Length,
                                              value.Length); intIndex++)
                    {
                        ma_intCustomColours[intIndex] = value[intIndex];
                    }
                }
            }
            
             //-----------------------------------------------------------------//
            
            protected string CardSetFilter
            {
                get
                {
                    return String.Join("|", this.CardGameControl.CardSet.FileFilter);
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected string CardSetDirectoryPath
            {
                get
                {
                    return m_strCardSetDirectoryPath;
                }
                set
                {
                    m_strCardSetDirectoryPath = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected string CardSetPath
            {
                get
                {
                    return m_strCardSetPath;
                }
                set
                {
                    m_strCardSetPath = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected string CardSetFileName
            {
                get
                {
                    return m_strCardSetFileName;
                }
                set
                {
                    m_strCardSetFileName = value;
                }
            }
            
            protected virtual void OpenCardsetMenuItem_Click(
                object sender, System.EventArgs e)
            {
                string strNewCardSetPath          = m_strCardSetPath;  
                string strNewCardSetDirectoryPath = m_strCardSetDirectoryPath;
                string strNewCardSetFileName      = m_strCardSetFileName;          
                
                // open file requestor
                OpenFileDialog dlgOpenCardSet = new OpenFileDialog();
                dlgOpenCardSet.Title = "Open Card Set";
                dlgOpenCardSet.InitialDirectory = m_strCardSetDirectoryPath;
                dlgOpenCardSet.Filter = CardSetFilter;
                dlgOpenCardSet.FilterIndex = m_intFilterIndex;
                
                DialogResult dr = dlgOpenCardSet.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string strFileName = dlgOpenCardSet.FileName;
                    if (strFileName != "")
                    {
                        // find last \
                        int intPos = strFileName.LastIndexOf("\\");
                        if (intPos < 0)
                        {
                            strNewCardSetDirectoryPath = "";
                            strNewCardSetFileName = strFileName;
                        }
                        else
                        {
                            strNewCardSetDirectoryPath = strFileName.Substring(0, intPos+1);
                            strNewCardSetFileName = strFileName.Substring(intPos+1);
                        }
                        strNewCardSetPath = strFileName;
                        m_intFilterIndex = dlgOpenCardSet.FilterIndex;
                    }
                }           
                
                if (this.CardGameControl.CardSet != null)
                {
                    if (strNewCardSetPath.ToLower() != m_strCardSetPath.ToLower() || 
                        this.UseDefaultCardSet)
                    {
                        if (this.CardGameControl.CardSet.Load(strNewCardSetPath))
                        {
                            // resize
                            Size CardSetSize = 
                                this.CardGameControl.CardSetSizeForClient;
                            this.CardGameControl.CardSet.Resize(
                                CardSetSize.Width, CardSetSize.Height, true);
                            
                            this.UseDefaultCardSet = false;
                            
                            this.CardGameControl.RefreshDisplay();
                            
                            // update card set path
                            this.CardSetPath          = strNewCardSetPath;
                            this.CardSetDirectoryPath = strNewCardSetDirectoryPath;
                            this.CardSetFileName      = strNewCardSetFileName;
                        }   
                    }
                }
                    
                dlgOpenCardSet.Dispose();
            }
            
            protected virtual void SaveSettingsMenuItem_Click(
                    object sender, System.EventArgs e)
            {
            }
            
            protected virtual void ExitMenuItem_Click(
                    object sender, System.EventArgs e)
            {
                this.Close();
            }
            
            protected virtual void RestartMenuItem_Click(
                    object sender, System.EventArgs e)
            {
                m_GameControl.Restart();
            }
            
            protected virtual void AutoFinishMenuItem_Click(
                    object sender, System.EventArgs e)
            {
                this.CardGameControl.AutoFinish();
            }
            
            protected virtual void BackgroundMenuItem_Click(
                object sender, System.EventArgs e)
            {
                BackgroundDialog dlgBackground = new BackgroundDialog();
                
                dlgBackground.CustomColors      = 
                    this.CustomColors;
                dlgBackground.Colour            = 
                    m_GameControl.Background.Colour;
                dlgBackground.GradientColour1   = 
                    m_GameControl.Background.Colours[0];
                dlgBackground.GradientColour2   = 
                    m_GameControl.Background.Colours[1];
                dlgBackground.GradientDirection = 
                    m_GameControl.Background.GradientDirection;
                dlgBackground.BitmapPath = m_strBackgroundBitmapPath;
                dlgBackground.Tile = 
                    m_GameControl.Background.Tile;
                dlgBackground.Stretch = 
                    m_GameControl.Background.Stretch;
                dlgBackground.Centre = 
                    m_GameControl.Background.Centre;
                dlgBackground.KeepAspectRatio = 
                    m_GameControl.Background.KeepAspectRatio;
                dlgBackground.FillWithGradient = 
                    m_GameControl.Background.FillWithGradient;
                dlgBackground.BackgroundStyle = 
                    m_GameControl.Background.Style;
                    
                if (dlgBackground.ShowDialog() == DialogResult.OK)
                {
                    this.CustomColors = dlgBackground.CustomColors;
                    m_GameControl.Background.Colour = dlgBackground.Colour;
                    m_GameControl.Background.Colours = 
                        new Color[]{dlgBackground.GradientColour1,
                                    dlgBackground.GradientColour2};
                    m_GameControl.Background.GradientDirection = 
                        dlgBackground.GradientDirection;
                    string strBitmapPath = dlgBackground.BitmapPath;
                    if (strBitmapPath != "" && 
                        strBitmapPath != m_strBackgroundBitmapPath)
                    {
                        try
                        {
                            Bitmap bmpBG = new Bitmap(strBitmapPath);
                            if (bmpBG != null)
                            {
                                m_strBackgroundBitmapPath = strBitmapPath;
                                m_GameControl.Background.Bitmap = bmpBG;
                                bmpBG = null;
                            }
                        }
                        catch{}
                    }
                    m_GameControl.Background.Tile = 
                        dlgBackground.Tile;
                    m_GameControl.Background.Stretch = 
                        dlgBackground.Stretch;
                    m_GameControl.Background.Centre = 
                        dlgBackground.Centre;
                    m_GameControl.Background.KeepAspectRatio = 
                        dlgBackground.KeepAspectRatio;
                    m_GameControl.Background.FillWithGradient = 
                        dlgBackground.FillWithGradient;
                    m_GameControl.Background.Style =
                        dlgBackground.BackgroundStyle;
                    m_GameControl.RefreshDisplay();
                }
                
                dlgBackground.Dispose();
            }
            
            protected virtual void LockCurrentSizeMenuItem_Click(
                    object sender, System.EventArgs e)
            {
            }
            
            protected virtual void ResizeStandardMenuItem_Click(
                    object sender, System.EventArgs e)
            {
            }
            
            protected virtual void ResizeSmallMenuItem_Click(
                    object sender, System.EventArgs e)
            {
            }
            
            protected virtual void ResizeMinimumMenuItem_Click(
                    object sender, System.EventArgs e)
            {
            }
            
            protected virtual void ResizeMaximumMenuItem_Click(
                    object sender, System.EventArgs e)
            {
            }
            
            protected virtual void HelpContentsMenuItem_Click(
                    object sender, System.EventArgs e)
            {
            }
            
            protected virtual void AboutMenuItem_Click(
                    object sender, System.EventArgs e)
            {
            }
            
            public virtual void AboutGame()
            {
            }
            
            protected virtual void UseDefaultCardSetMenuItem_Click(
                object sender, System.EventArgs e)
            {
                if (UseDefaultCardSet)
                {
                    if (m_strCardSetPath != "")
                    {
                        if (this.CardGameControl.CardSet.Load(m_strCardSetPath))
                        {
                            UseDefaultCardSet = false;
                            
                            Size CardSetSize = 
                                this.CardGameControl.CardSetSizeForClient;
                            this.CardGameControl.CardSet.Resize(
                                CardSetSize.Width, CardSetSize.Height, true);
                                
                            this.CardGameControl.RefreshDisplay();
                        }
                    }
                }
                else
                {
                    if (this.CardGameControl.CardSet.Load(m_strDefaultCardSetPath))
                    {
                        UseDefaultCardSet = true;
                        
                        Size CardSetSize = 
                            this.CardGameControl.CardSetSizeForClient;
                        this.CardGameControl.CardSet.Resize(
                            CardSetSize.Width, CardSetSize.Height, true);
                        
                        this.CardGameControl.RefreshDisplay();
                    }
                }
            }
            
            protected bool UseDefaultCardSet
            {
                get
                {
                    return this.m_menuItemUseDefaultCardSet.Checked;
                }
                set
                {
                    this.m_menuItemUseDefaultCardSet.Checked = value;
                }
            }
            
            protected virtual void SaveSettingsOnExitMenuItem_Click(
                    object sender, System.EventArgs e)
            {
            }
            
            protected override void OnResize(EventArgs ev)
            {
                SetControlCoords();
                base.OnResize(ev);
            } 
            
            protected override void OnClosed(EventArgs e)
            {
                base.OnClosed(e);
                  
                Application.Exit();
            }
            
            //-----------------------------------------------------------------//
            
            // method to display message when game has been won
            protected virtual bool GameWonMessage()
            {
                bool blnAnotherGame = true;
                
                CongratulationsDialog dlgCongrats = new CongratulationsDialog();
                DialogResult dr = dlgCongrats.ShowDialog();
                
                if (dr == DialogResult.No)
                {
                    blnAnotherGame =  false;
                }
                
                return blnAnotherGame;
            }
            
            //-----------------------------------------------------------------//
            
            // method to execute when game has been won
            public virtual void GameWon()
            {
                bool blnAnotherGame = GameWonMessage();
                if (blnAnotherGame)
                {
                    // restart new game
                    this.CardGameControl.Restart();
                }
                else
                {
                    // exit
                    this.Close();
                }
            }
            
            //-----------------------------------------------------------------//
            
            //-----------------------------------------------------------------//
            
            protected MenuItem FileMenuItem
            {
                get
                {
                    return m_menuItemFile;
                }
                set
                {
                    m_menuItemFile = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem OpenCardSetMenuItem
            {
                get
                {
                    return m_menuItemOpen;
                }
                set
                {
                    m_menuItemOpen = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem SaveSettingsMenuItem
            {
                get
                {
                    return m_menuItemSave;
                }
                set
                {
                    m_menuItemSave = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem ExitMenuItem
            {
                get
                {
                    return m_menuItemExit;
                }
                set
                {
                    m_menuItemExit = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem GameMenuItem
            {
                get
                {
                    return m_menuItemGame;
                }
                set
                {
                    m_menuItemGame = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem RestartMenuItem
            {
                get
                {
                    return m_menuItemRestart;
                }
                set
                {
                    m_menuItemRestart = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem AutoFinishMenuItem
            {
                get
                {
                    return m_menuItemAutoFinish;
                }
                set
                {
                    m_menuItemAutoFinish = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem OptionsMenuItem
            {
                get
                {
                    return m_menuItemOptions;
                }
                set
                {
                    m_menuItemOptions = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem UseDefaultCardSetMenuItem
            {
                get
                {
                    return m_menuItemUseDefaultCardSet;
                }
                set
                {
                    m_menuItemUseDefaultCardSet = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem SaveSettingsOnExitMenuItem
            {
                get
                {
                    return m_menuItemSaveSettingsOnExit;
                }
                set
                {
                    m_menuItemSaveSettingsOnExit = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem LockCurrentSizeMenuItem
            {
                get
                {
                    return m_menuItemLockCurrentSize;
                }
                set
                {
                    m_menuItemLockCurrentSize = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem ResizeMenuItem
            {
                get
                {
                    return m_menuItemResize;
                }
                set
                {
                    m_menuItemResize = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem ResizeStandardMenuItem
            {
                get
                {
                    return m_menuItemStandard;
                }
                set
                {
                    m_menuItemStandard = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem ResizeSmallMenuItem
            {
                get
                {
                    return m_menuItemSmall;
                }
                set
                {
                    m_menuItemSmall = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem ResizeMinimumMenuItem
            {
                get
                {
                    return m_menuItemMinimum;
                }
                set
                {
                    m_menuItemMinimum = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem ResizeMaximumMenuItem
            {
                get
                {
                    return m_menuItemMaximum;
                }
                set
                {
                    m_menuItemMaximum = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem HelpMenuItem
            {
                get
                {
                    return m_menuItemHelp;
                }
                set
                {
                    m_menuItemHelp = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem HelpContentsMenuItem
            {
                get
                {
                    return m_menuItemHelpContents;
                }
                set
                {
                    m_menuItemHelpContents = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            protected MenuItem AboutMenuItem
            {
                get
                {
                    return m_menuItemAbout;
                }
                set
                {
                    m_menuItemAbout = value;
                }
            }
            
        }
        
        public class CongratulationsDialog : Form
        {
            public CongratulationsDialog()
            {
                Button btnPlayAgain = new Button();
                Button btnQuit      = new Button();
                Label  labCongrats  = new Label();
              
                this.Text = "Congratulations!";
                
                double c_dblWindowWHRatio = 1.5625;
                
                this.Size = new Size(
                    Math.Min(400, 
                        (int)Math.Min(SystemInformation.WorkingArea.Width/2,
                            SystemInformation.WorkingArea.Height/2*c_dblWindowWHRatio)),
                    Math.Min(256, 
                        (int)Math.Min(SystemInformation.WorkingArea.Height/2,
                            SystemInformation.WorkingArea.Width/2/c_dblWindowWHRatio)));
                // Define the border style of the form to a dialog box.
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.ControlBox = false;
                
                
                // Set the text of button1 to "OK".
                btnPlayAgain.Text = "Play Again";
                // Set the position of the button on the form.
                btnPlayAgain.Location = new Point (10, ClientSize.Height-btnPlayAgain.Height-10);
                btnPlayAgain.Click += new System.EventHandler(PlayAgain_Click);
                // Set the text of button2 to "Cancel".
                btnQuit.Text = "Quit";
                // Set the position of the button based on the location of button1.
                btnQuit.Location
                   = new Point (this.ClientSize.Width-btnQuit.Width-10, 
                                this.ClientSize.Height-btnQuit.Height-10);
                btnQuit.Click += new System.EventHandler(Quit_Click);
                
                
                // Set the accept button of the form to button1.
                this.AcceptButton = btnPlayAgain;
                // Set the cancel button of the form to button2.
                this.CancelButton = btnQuit;
                // Set the start position of the form to the center of the screen.
                this.StartPosition = FormStartPosition.CenterScreen;
       
                // draw congrats message
                Bitmap bmpCongrats = new Bitmap(this.ClientSize.Width-20,
                                                this.ClientSize.Height-
                                                Math.Max(btnPlayAgain.Height, btnQuit.Height)-30);
                Graphics objGraphics = Graphics.FromImage(bmpCongrats);
                objGraphics.FillRectangle(new SolidBrush(Color.White), 
                                          0, 0, bmpCongrats.Width-1, bmpCongrats.Height-1);
                int intStep = 5;
                Pen blackPen = new Pen(Color.Black);
                Pen redPen = new Pen(Color.FromArgb(192,0,0));
                for (int intOffset=0; intOffset<5*intStep; intOffset+=intStep)
                {
                    Pen linePen;
                    if (intOffset % (2*intStep) == 0)
                    {
                        linePen = blackPen;
                    }
                    else
                    {
                        linePen = redPen;
                    }
                    objGraphics.DrawRectangle(linePen, 
                                              intOffset, intOffset,
                                              bmpCongrats.Width-1-2*intOffset, 
                                              bmpCongrats.Height-1-2*intOffset);
                    objGraphics.DrawRectangle(linePen, 
                                              intOffset+1, intOffset+1,
                                              bmpCongrats.Width-3-2*intOffset, 
                                              bmpCongrats.Height-3-2*intOffset);
                }
                      
                Font fntBig;
                Font fntMedium;
                           
                SizeF line1PixelSize;
                SizeF line2PixelSize;
                           
                SolidBrush bshRed = new SolidBrush(Color.FromArgb(192,0,0));
                SolidBrush bshBlack = new SolidBrush(Color.Black);
                
                string strCongratsLine1 = "Congratulations!";
                string strCongratsLine2 = "You Win!";            
                
                const double c_dblSmallToBigFontSizeRation = 0.833334;
                int intBigFontSize = 24;
                int intSmallFontSize = 0;
                           
                do
                {        
                    intSmallFontSize = (int)(intBigFontSize*c_dblSmallToBigFontSizeRation);
                    fntBig    = new Font("Verdana", intBigFontSize);
                    fntMedium = new Font("Verdana", intSmallFontSize);
                  
                    line1PixelSize = objGraphics.MeasureString(strCongratsLine1, fntBig);
                    line2PixelSize = objGraphics.MeasureString(strCongratsLine2, fntMedium);
                     
                    if (bmpCongrats.Width < 
                            60 + Math.Max(line1PixelSize.Width, line2PixelSize.Width) ||
                        bmpCongrats.Height < 
                            60 + line1PixelSize.Height + 2*line2PixelSize.Height)
                    {
                        // font too big, move a size smaller
                        intBigFontSize --;
                        intSmallFontSize = 0;
                    }        
                }
                while (intSmallFontSize == 0 && intBigFontSize>=10);
                
                if (intBigFontSize < 10 || intSmallFontSize < 
                    (int)(10*c_dblSmallToBigFontSizeRation))
                {
                    intBigFontSize = 10;
                    intSmallFontSize = (int)(intBigFontSize * c_dblSmallToBigFontSizeRation);
                
                    fntBig    = new Font("Verdana", intBigFontSize);
                    fntMedium = new Font("Verdana", intSmallFontSize);
                      
                    line1PixelSize = objGraphics.MeasureString(strCongratsLine1, fntBig);
                    line2PixelSize = objGraphics.MeasureString(strCongratsLine2, fntMedium);
                
                    this.ClientSize = new Size(
                        60+(int)Math.Max(line1PixelSize.Width, line2PixelSize.Width),
                        (int)(60+line1PixelSize.Height + 2*line2PixelSize.Height));
                }             
                
                objGraphics.DrawString(strCongratsLine1, 
                                       fntBig, 
                                       bshRed, (bmpCongrats.Width-line1PixelSize.Width)/2, 30);
                objGraphics.DrawString(strCongratsLine2, 
                                       fntMedium, 
                                       bshBlack, (bmpCongrats.Width-line2PixelSize.Width)/2, 
                                                bmpCongrats.Height-30-line2PixelSize.Height);                                      
                objGraphics.Dispose();
                
                labCongrats.Image = bmpCongrats;
                labCongrats.Location = new Point(10,10);            
                labCongrats.Size = bmpCongrats.Size;
                
                // Add button1 to the form.
                this.Controls.Add(btnPlayAgain);
                // Add button2 to the form.
                this.Controls.Add(btnQuit);
                this.Controls.Add(labCongrats);
            }
            
            public void PlayAgain_Click(object sender, System.EventArgs e)
            {
                this.DialogResult = DialogResult.Yes;
            }
            
            public void Quit_Click(object sender, System.EventArgs e)
            {
                this.DialogResult = DialogResult.No;
            }
        }   
    }
}