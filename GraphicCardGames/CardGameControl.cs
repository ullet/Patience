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
 * File:          CardGameControl.cs                                     *
 * Namespace:     SwSt.CardGames                                         *
 * Last modified: 13 January 2005                                        *
 * Description:   Card Game Control Interface and base class.            *
 *************************************************************************
 */
 
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SwSt;
using SwSt.Cards;
using SwSt.CardGames;

namespace SwSt
{
    namespace CardGames
    {   
        public interface ICardGameControl : IComparable
        {
            // cardset used by game
            ICardSet CardSet
            {
                get;
                set;
            }
            
            ICardGameHost Host
            {
                get;
                set;
            }
            
            // name of game e.g. "Klondike" or "Beleaguered Castle"
            string GameTitle
            {
                get;
            }
            
            // Game specific options to add to Options menu under
            // 'Game' sub menu
            MenuItem Options
            {
                get;
            }
            
            // XML node list of game specific settings
            System.Xml.XmlNodeList Settings
            {
                get;
                set;
            }
            
            // path of game help file
            string HelpFile
            {
                get;
            }
            
            // called to start new game
            void Restart();
            
            // return true if Auto Finish feature is implemented
            bool AutoFinishAvailable
            {
                get;
            }
            
            // Execute game Auto Finish
            void AutoFinish();
            
            void LoadCardSet(string strPath);
            
            // Version string, including "Version" part
            string Version
            {
                get;
            }
            
            // Copyright string, including "Copyright (c)" part
            string Copyright
            {
                get;
            }
            
            // URL of author's website
            string Website
            {
                get;
            }
            
            // Author's contact email address
            string ContactEmail
            {
                get;
            }
            
            // Any additional information to display in about window,
            // such as a list of contributors etc.
            string AdditionalInfo
            {
                get;
            }
            
            // return size required to display cardset at its minimum size
            Size MinimumClientSizeForCardSet
            {
                get;
            }
            
            // return size required to display cardset at its current size
            Size ClientSizeForScaledCardSet
            {
                get;
            }
            
            // return size required to display cardset at its default size
            Size ClientSizeForCardSet
            {
                get;
            }
            
            // return CardSet size to display at current client size
            Size CardSetSizeForClient
            {
                get;
            }
            
            // get or set background colour
            Color BackgroundColour
            {
                get;
                set;
            }
            
            // get or set background (for image or gradient etc.)
            Background Background
            {
                get;
                set;
            }
            
            // force refresh of display
            void RefreshDisplay();
            
            // Display about message/window
            void About();
        }
        
        public abstract class CardGameControl : Label, ICardGameControl
        {
            private Bitmap             m_bmpBuffer  = null;
            private Bitmap             m_bmpPickedUpCards      = null;
            private Rectangle          m_FGRectangle = new Rectangle(-1,-1,0,0);
            private Point              m_MousePointerOffset = new Point(-1,-1);
            
            private Form               m_frmParent  = null;
            private CardGame           m_Game       = null;
            private ICardSet           m_CardSet    = null;
            private Background         m_Background = new Background();
            private Size               m_MaxCardSize = new Size(0,0);
        
            private CardTableZone      m_FromZone = null;
            private CardTableZone      m_ToZone = null;
            
            private CardTableZones     m_Zones = new CardTableZones();
            
            private ICardGameHost      m_Host = null;
            
            public CardGameControl() : base()
            {
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                InitialiseZones();
            }
            
            public CardGameControl(ICardGameHost host) : this()
            {
                this.Host = host;
            }
            
            public void Initialise(ICardGameHost host, CardGame game, 
                ICardSet cardSet, Color bgColour)
            {
                this.Host = host;
                this.Game = game;
                this.CardSet = cardSet;
                this.BackgroundColour = bgColour;
            }
            
            public int CompareTo(object obj)
            {
                int intCompareValue = -1;
                try
                {
                    intCompareValue = 
                        this.GameTitle.CompareTo(((ICardGameControl)obj).GameTitle);
                }
                catch
                {
                    intCompareValue = -1;
                }
                
                return intCompareValue;
                
            }
            
            public ICardGameHost Host
            {
                get
                {
                    return m_Host;
                }
                set
                {
                    m_Host = value;
                }
            }
            
            protected Size MaxCardSize
            {
                get
                {
                    return this.m_MaxCardSize;
                }
                set
                {
                    this.m_MaxCardSize.Width  = value.Width;
                    this.m_MaxCardSize.Height = value.Height;
                }
            }
            
            public virtual Form ParentForm
            {
                get
                {
                    return m_frmParent;
                }
                set
                {
                    m_frmParent = value;
                }
            }
                                    
            protected override void OnResize(EventArgs ev)
		    {
		        if (this.Size.Width > 0 && this.Size.Height > 0)
		        {
		            if (m_bmpBuffer != null)
		            {
		                try
		                {
		                    m_bmpBuffer.Dispose();
		                }
		                catch{}
		            }
		            m_bmpBuffer = new Bitmap(this.Size.Width, this.Size.Height);
		            if (this.Image != null)
		            {
		                try
		                {
		                    this.Image.Dispose();
		                }
		                catch{}
		            }
		            this.Image = null;
		            
		            this.Background.Size = this.Size;
		           
		        }
		        base.OnResize(ev);
		    }
		    
		    protected abstract void InitialiseZones();
		    
		    protected abstract void SetZones();
		    
		    protected void SetZone(CardTableZoneType type, CardTableRegion region, 
                             int intX, int intY, int intWidth, int intHeight)
            {
                CardTableZone objZone = this.Zones.GetZoneByTypeAndRegion(type, region);
                if (objZone != null)
                {
                    objZone.X      = intX;
                    objZone.Y      = intY;
                    objZone.Width  = intWidth;
                    objZone.Height = intHeight;
                }
            }
		    
		    protected abstract void CalculateMaxCardSize();
		    
		    public virtual void LoadCardSet(string strPath)
		    {
		        this.CardSet.Load(strPath);
		        
		        // reset max card size
                CalculateMaxCardSize();
                    
                // resize cards
                try
                {
                    this.CardSet.Resize(this.MaxCardSize.Width, 
                                        this.MaxCardSize.Height, 
                                        true, true, true);
                }
                catch
                {
                }
		        
		        CalculateLayoutValues();
		        
		        SetZones();
		    }		        
		    
		    protected override void OnLayout(LayoutEventArgs lev)
		    {
		        // reset max card size
                CalculateMaxCardSize();
                    
                // resize cards
                try
                {
                    this.CardSet.Resize(this.MaxCardSize.Width, 
                                        this.MaxCardSize.Height, 
                                        true, true, true);
                }
                catch
                {
                }
		        
		        CalculateLayoutValues();
		        
		        SetZones();
		        
		        base.OnLayout(lev);
		    }
		    
		    public abstract string GameTitle
		    {
		        get;
		    }
		        
		    protected abstract bool BackgroundChanged
		    {
		        get;
		    }
		        
		    protected override void OnPaintBackground(PaintEventArgs pev)
		    {
		        this.SuspendLayout();
    		    Graphics objGraphics = null;
		        if (this.Image == null || BackgroundChanged)
		        {
		            objGraphics = Graphics.FromImage(m_bmpBuffer);
                    DrawDisplay(objGraphics);
                    objGraphics.Dispose();
                    this.Image = (Image)m_bmpBuffer.Clone();
                }
                
                if (this.Image != null)
		        {
		            objGraphics = Graphics.FromImage(this.Image);
                    if (m_blnMoving)
                    {
                        RestoreBackground(objGraphics, pev.ClipRectangle);
                        if (m_blnStopMoving)
                        {
                            m_blnMoving = false;
                            m_blnStopMoving = false;
                        }
                    }
                    DrawObjects(objGraphics);
                    objGraphics.Dispose();
                }
    		    this.ResumeLayout();
    		    base.OnPaintBackground(pev);
		    }
		    
		    protected abstract void CalculateLayoutValues();
		    protected abstract void DrawDisplay(Graphics objGraphics);
		    
		    protected virtual void DrawObjects(Graphics objGraphics)
		    {
		        if (m_bmpPickedUpCards != null)
		        {
		            objGraphics.DrawImage(
		                m_bmpPickedUpCards, m_FGRectangle.X, m_FGRectangle.Y);
		        }
		    }
		    
		    protected Bitmap PickedUpCards
		    {
		        get
		        {
		            return m_bmpPickedUpCards;
		        }
		        set
		        {
		            m_bmpPickedUpCards = value;
		        }
		    }
		    
		    protected virtual void RestoreBackground(
		        Graphics objGraphics, Rectangle rec)
		    {
		        if (rec.X < 0 || rec.Y < 0)
		        {
		            // nothing to restore
		            return;
		        }
		        
		        // NB. more than twice as fast creating 'clone' of buffer bitmap
		        // and then drawing to background instead of drawing buffer directly
		        // to background using DrawImage(bmpBG, rec, rec, GraphicsUnit.Pixel)
		        Bitmap bmpBG = m_bmpBuffer.Clone(rec, m_bmpBuffer.PixelFormat);
		        objGraphics.DrawImage(bmpBG, rec.X, rec.Y);
		    }
		    
		    protected virtual CardGame Game
            {
                get
                {
                    return this.m_Game;
                }
                set
                {
                    this.m_Game = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            public virtual ICardSet CardSet
            {
                get
                {
                    return this.m_CardSet;
                }
                set
                {
                    this.m_CardSet = value;
                }
            }
            
            //-----------------------------------------------------------------//
            
            public virtual MenuItem Options
            {
                get
                {
                    return null;
                }
            }
            
            //-----------------------------------------------------------------//
            
            public virtual System.Xml.XmlNodeList Settings
            {
                get
                {
                    return null;
                }
                set
                {
                }
            }
            
            //-----------------------------------------------------------------//
            
            public virtual string HelpFile
            {
                get
                {
                    return "";
                }
            }
            
            //-----------------------------------------------------------------//
            
            public virtual bool AutoFinishAvailable
            {
                get
                {
                    return false;
                }
            }
            
            //-----------------------------------------------------------------//
            
            public abstract void AutoFinish();
            
            //-----------------------------------------------------------------//
            
            public virtual string Version
            {
                get
                {
                    string strVersion = "Version ";
                    
                    string strVersionNumber = "";
                    try
                    {
                        strVersionNumber = 
                            Assembly.GetAssembly(this.GetType()).
                                GetName().Version.ToString();
                    }
                    catch
                    {
                        strVersionNumber = "";
                    }
                        
                    if (strVersionNumber == "")
                    {
                        strVersion = "";
                    }
                    else
                    {
                        strVersion += strVersionNumber;
                    }
                    
                    return strVersion;
                }
            }
            
            //-----------------------------------------------------------------//
            
            public abstract string Copyright
            {
                get;
            }
            
            //-----------------------------------------------------------------//
            
            public abstract string Website
            {
                get;
            }
            
            //-----------------------------------------------------------------//
            
            public abstract string ContactEmail
            {
                get;
            }
            
            //-----------------------------------------------------------------//
            
            public abstract string AdditionalInfo
            {
                get;
            }
            
            //-----------------------------------------------------------------//
            
            public virtual Background Background
            {
                get
                {
                    return this.m_Background;
                }
                set
                {
                    this.m_Background = value;
                    // complete redraw required
                    BackgroundAllChanged = true;
                    Invalidate();
                }
            }
            
            //-----------------------------------------------------------------//
            
            public virtual Color BackgroundColour
            {
                get
                {
                    return this.Background.Colour;
                }
                set
                {
                    this.Background.Colour = value;
                    this.Background.Style = BackgroundStyle.SolidColour;
                    // complete redraw required
                    BackgroundAllChanged = true;
                    Invalidate();
                }
            }
            
            public virtual void RefreshDisplay()
            {
                BackgroundAllChanged = true;
                Invalidate();
            }
            
            //-----------------------------------------------------------------//
            
            protected abstract bool BackgroundAllChanged
            {
                get;
                set;
            }
            
            //-----------------------------------------------------------------//
            
            public virtual void Restart()
            {
                if (this.Game != null)
                {
                    this.Game.Restart();
                }
                this.CalculateLayoutValues();
                this.SetZones();
                BackgroundAllChanged = true;
                Invalidate();
            }
            
            //-----------------------------------------------------------------//
            
            public abstract Size ClientSizeForCardSet
            {
                get;
            }
            
            public abstract Size ClientSizeForScaledCardSet
            {
                get;
            }
            
            public abstract Size CardSetSizeForClient
            {
                get;
            }
            
            public abstract Size MinimumClientSizeForCardSet
            {
                get;
            }
            
            public void About()
            {
                // call generic method of Host
                this.Host.AboutGame();
            }
            
            //-----------------------------------------------------------------//
		    
		    protected Bitmap BufferBitmap
            {
                get
                {
                    return this.m_bmpBuffer;
                }
            }
            
            protected CardTableZones Zones
            {
                get
                {
                    return m_Zones;
                }
            }
            
            //-----------------------------------------------------------------//
		    
		    protected abstract Bitmap PickUpCards(CardTableZone zoneFrom);
		    
		    protected abstract void MoveCard(
		        CardTableZone zoneFrom, CardTableZone zoneTo);
		    
		    protected abstract void ReplaceCard(CardTableZone zoneFrom);
		    
		    protected virtual void GameWon()
		    {
		        m_Host.GameWon();
		    }
		    
		    //-----------------------------------------------------------------//
            // Mouse Event Handlers                                             //
            //-----------------------------------------------------------------//
            
            private bool m_blnMoving     = false;
            private bool m_blnStopMoving = false;
            
            protected Rectangle FGRectangle
            {
                get
                {
                    return m_FGRectangle;
                }
                set
                {
                    m_FGRectangle.X = value.X;
                    m_FGRectangle.Y = value.Y;
                    m_FGRectangle.Width = value.Width;
                    m_FGRectangle.Height = value.Height;
                }
            }
            
            protected int FGRectangleX
            {
                get
                {
                    return m_FGRectangle.X;
                }
                set
                {
                    m_FGRectangle.X = value;
                }
            }
            
            protected int FGRectangleY
            {
                get
                {
                    return m_FGRectangle.Y;
                }
                set
                {
                    m_FGRectangle.Y = value;
                }
            }
            
            protected override void OnMouseDown(MouseEventArgs mev)
            {
                if (mev.Button == MouseButtons.Left)
                { 
                    CardTableZones colZones = m_Zones.WhichZone(mev.X, mev.Y);
                    m_FromZone = null;
                    if (colZones.Count > 0)
                    {
                        foreach (CardTableZone objZone in colZones)
                        {
                            if (objZone.Type == CardTableZoneType.PickUpZone ||
                                objZone.Type == CardTableZoneType.ClickZone)
                            {
                                // NB. possibly overlapping zones, but
                                // assume only one of any given type
                                m_FromZone = objZone;
                            }
                        }
                        
                        if (m_FromZone != null &&
                            m_FromZone.Type == CardTableZoneType.PickUpZone)
                        {
                            m_bmpPickedUpCards = PickUpCards(m_FromZone);
                            
                            m_FGRectangle.Width = m_bmpPickedUpCards.Width;
                            m_FGRectangle.Height = m_bmpPickedUpCards.Height;
                            m_FGRectangle.X = m_FromZone.X;
                            m_FGRectangle.Y = m_FromZone.Y;
                            m_MousePointerOffset = new Point(
                                    Math.Min(this.CardSet.Width-1,
                                             Math.Max(0, mev.X-m_FromZone.X)),
                                    Math.Min(this.CardSet.Height-1,
                                             Math.Max(0, mev.Y-m_FromZone.Y)));
                            
                            m_blnMoving = true;
                            
                            Invalidate(m_FGRectangle, false);
                        }
                    }
                }
                
                base.OnMouseDown(mev);
                
            }
            
            // NB. not a real event, called from OnMouseUp
            protected virtual void OnMouseClick(MouseEventArgs mev)
            {
                if (mev.Button == MouseButtons.Left && m_FromZone != null)
                {
                    CardTableRegion regionTo = null;
                    
                    CardTableZones colZones = m_Zones.WhichZone(mev.X, mev.Y);
                    m_ToZone   = null;
                    CardTableZone releaseZone = null;
                    
                    if (colZones.Count > 0)
                    {
                        foreach (CardTableZone objZone in colZones)
                        {
                            if (objZone.Type == CardTableZoneType.ClickZone)
                            {
                                // NB. possibly overlapping zones, but
                                // assume only one of any given type
                                releaseZone = objZone;
                            }
                        }
                    }
                    
                    if (releaseZone != null && releaseZone.IsEqualTo(m_FromZone))
                    {
                        // mouse up and down on same zone, 
                        // i.e. clicked on zone
                        regionTo =
                            this.Game.Move(m_FromZone.Region);
                    } 
                     
                    if (regionTo != null)
                    {
                        m_ToZone = new CardTableZone(
                            CardTableZoneType.ClickZone,
                            regionTo, 0, 0, 0, 0);
                            
                        MoveCard(m_FromZone, m_ToZone);
                        
                        // check if won
                        if (this.Game.IsGameWon())
                        {
                            GameWon();
                        }        
                    }
                    else
                    {
                        ReplaceCard(m_FromZone);
                    }
                    
                    Invalidate();
                                    
                    m_FromZone = null;
                    m_ToZone = null;
                } // end if left button
            }
            
            protected override void OnMouseUp(MouseEventArgs mev)
            {
                if (m_blnMoving || m_FromZone != null &&
                    this.m_FromZone.Type == CardTableZoneType.ClickZone)
                {
                    if (mev.Button == MouseButtons.Left && m_FromZone != null)
                    { 
                        if (this.m_FromZone.Type == CardTableZoneType.ClickZone)
                        {
                            OnMouseClick(mev);
                        }
                        else
                        {
                            bool blnMoved = false;
                            
                            CardTableZones colZones = 
                                m_Zones.WhichZone(mev.X, mev.Y);
                            m_ToZone = null;
                            
                            if (colZones.Count > 0)
                            {
                                foreach (CardTableZone objZone in colZones)
                                {
                                    if (objZone.Type == CardTableZoneType.DropZone)
                                    {
                                        // NB. possibly overlapping zones, but
                                        // assume only one of any given type
                                        m_ToZone = objZone;
                                    }
                                }
                            }
                            
                            if (m_ToZone != null && m_FromZone != null)
                            {
                                blnMoved = this.Game.Move(
                                    m_FromZone.Region, m_ToZone.Region);
                                
                            } 
                             
                            if (blnMoved)
                            {
                                MoveCard(m_FromZone, m_ToZone);
                                
                                // check if won
                                if (this.Game.IsGameWon())
                                {
                                    GameWon();
                                }        
                            }
                            else
                            {
                                ReplaceCard(m_FromZone);
                            }
                                            
                            
                        }
                    }
                    
                    m_bmpPickedUpCards = null;
                    m_FromZone = null;
                    m_ToZone = null;
                    
                    if (m_blnMoving)
                    {
                        Invalidate(m_FGRectangle, false);
                            
                        m_blnStopMoving = true;
                    }
                    
                    m_FGRectangle.X = -1;
                    m_FGRectangle.Y = -1;
                    m_FGRectangle.Width = 0;
                    m_FGRectangle.Height = 0;
                    
                }
                                
                base.OnMouseUp(mev);
            }
            
            protected override void OnMouseMove(MouseEventArgs mev)
            {
                if (m_blnMoving && !m_blnStopMoving)
                {
                    Invalidate(m_FGRectangle, false);
                    
                    m_FGRectangle.X = mev.X - m_MousePointerOffset.X;
                    m_FGRectangle.Y = mev.Y - m_MousePointerOffset.Y;
                    
                    Invalidate(m_FGRectangle, false);
                    
                    CardTableZones colZones = this.Zones.WhichZone(mev.X, mev.Y);
                    CardTableZone zone = null;
                    if (colZones.Count > 0)
                    {
                        foreach (CardTableZone objZone in colZones)
                        {
                            if (objZone.Type == CardTableZoneType.DropZone)
                            {
                                // NB. possibly overlapping zones, but
                                // assume only one of any given type
                                zone = objZone;
                            }
                        }
                            
                        if (zone != null)
                        {
                            if (this.Game.IsLegalMove(
                                this.m_FromZone.Region, zone.Region))
                            {
                                // change mouse pointer to a hand
                                Cursor.Current = Cursors.Hand;
                            }
                            else
                            {
                                Cursor.Current = Cursors.Arrow;
                            }
                        }
                        else
                        {
                            Cursor.Current = Cursors.Arrow;
                        }
                    }
                    else
                    {
                        Cursor.Current = Cursors.Arrow;
                    }
                }
                else
                {
                    CardTableZones colZones = this.Zones.WhichZone(mev.X, mev.Y);
                    CardTableZone zone = null;
                    if (colZones.Count > 0)
                    {
                        foreach (CardTableZone objZone in colZones)
                        {
                            if (objZone.Type == CardTableZoneType.PickUpZone ||
                                objZone.Type == CardTableZoneType.ClickZone)
                            {
                                // NB. possibly overlapping zones, but
                                // assume only one of any given type
                                zone = objZone;
                            }
                        }
                            
                        if (zone != null)
                        {
                            // change mouse pointer to a hand
                            Cursor.Current = Cursors.Hand;
                        }
                        else
                        {
                            Cursor.Current = Cursors.Arrow;
                        }
                    }
                }
                
                base.OnMouseMove(mev);
            }
        }
    }
}