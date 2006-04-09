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
 * File:          BeleagueredCastleControl.cs                            *
 * Namespace:     SwSt.CardGames                                         *
 * Last modified: 13 January 2005                                        *
 * Class:         BeleagueredCastleControl                               *
 * Description:   Beleaguered Castle card game control.                  *
 *************************************************************************
 */

using System;
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
        public class BeleagueredCastleControl : CardGameControl
        {
            Rectangle m_recBorder = new Rectangle(0,0,0,0);
            
            // layout values
            
            private const int       mc_intNumberOfRows               = 8;
            private       int       m_intRowWidth                    = 0;
            private       int       m_intHorizontalEdgeSpace         = 5;
            private       int       m_intHorizontalSpace             = 5;
            private const int       mc_intStandardVerticalSpaceLTE800x600 = 1;
            private const int       mc_intMinVerticalSpace           = 1;
            private const int       mc_intStandardVerticalSpace      = 5;
            private const int       mc_intMinHorizontalSpace         = 1;
            private const int       mc_intStandardHorizontalSpace    = 5;
            private       int       m_intVerticalEdgeSpace           = 5;//5;
            private       int       m_intVerticalSpace               = 5;//5;
            private       double[]  ma_dblWidthVisibleOverlappedCard = new double[8];
            private const int       mc_intMinVisible                 = 8;
            private const int       mc_intStandardVisible            = 12;
            private       int       m_intWindowCentreX;
            private       int       m_intWindowCentreY;
            
            private       Size      m_MaxCardSize = new Size(0,0);
            private       Size      m_DefaultStandardClientSize = new Size(698, 545);
            
            private       bool[]    ma_blnRowChanged
                = new bool[8]{false,false,false,false,false,false,false,false};
            private       bool[]    ma_blnFoundationChanged 
                = new bool[4]{false,false,false,false};
            private       bool[]    ma_blnRowExcludeLast
                = new bool[8]{false,false,false,false,false,false,false,false};
            
            private       Size      m_WorkingAreaSize800x600 = new Size(800, 572);
            
            
            public BeleagueredCastleControl() : base()
            {
                this.CardSet = new SwSt.CardGames.CardSet();
                this.Game = new SwSt.CardGames.BeleagueredCastle();
                this.Game.Initialise(this.CardSet.PlayingCards, -1);
                this.Game.Restart();
            }
            
            //-----------------------------------------------------------------//        
            
            private Size CalculateClientSize(
                int intCardSetWidth, int intCardSetHeight)
            {
                int intHorizontalSpace = mc_intMinHorizontalSpace;
                int intVerticalSpace   = mc_intMinVerticalSpace;
                    
                int intClientWidth = (int)Math.Ceiling(
                    (double)intCardSetWidth * 3.0 +
                    (double)mc_intMinVisible * 32.0 +
                    (double)intHorizontalSpace * 10.0);
                      
                int intClientHeight = (int)Math.Ceiling(
                    (double)intCardSetHeight * 4.0 +
                    (double)intVerticalSpace * 5.0);
                    
                return new Size(intClientWidth, intClientHeight);
            }
            
            public override Size MinimumClientSizeForCardSet
            {
                get
                {
                    int intHorizontalSpace = mc_intMinHorizontalSpace;
                    int intVerticalSpace   = mc_intMinVerticalSpace;
                    
                    int intClientWidth = (int)Math.Ceiling(
                        (double)this.CardSet.MinWidth * 3.0 +
                        (double)mc_intMinVisible * 32.0 +
                        (double)intHorizontalSpace * 10.0);
                        
                    int intClientHeight = (int)Math.Ceiling(
                        (double)this.CardSet.MinHeight * 4.0 +
                        (double)intVerticalSpace * 5.0);
                    
                    return new Size(intClientWidth, intClientHeight);
                }
            }
            
            public override Size ClientSizeForCardSet
            {
                get
                {
                    int intHorizontalSpace = mc_intStandardHorizontalSpace;
                    int intVerticalSpace   = mc_intStandardVerticalSpace;
                    
                    int intClientWidth = (int)Math.Ceiling(
                        (double)this.CardSet.UnscaledWidth * 3.0 +
                        (double)mc_intStandardVisible * 32.0 +
                        (double)intHorizontalSpace * 10.0);
                        
                    int intClientHeight = (int)Math.Ceiling(
                        (double)this.CardSet.UnscaledHeight * 4.0 +
                        (double)intVerticalSpace * 5.0);
                    
                    return new Size(intClientWidth, intClientHeight);
                }
            }
            
            public override Size ClientSizeForScaledCardSet
            {
                get
                {
                    int intHorizontalSpace = mc_intStandardHorizontalSpace;
                    int intVerticalSpace   = mc_intStandardVerticalSpace;
                    
                    int intClientWidth = (int)Math.Ceiling(
                        (double)this.CardSet.Width * 3.0 +
                        (double)mc_intStandardVisible * 32.0 +
                        (double)intHorizontalSpace * 10.0);
                        
                    int intClientHeight = (int)Math.Ceiling(
                        (double)this.CardSet.Height * 4.0 +
                        (double)intVerticalSpace * 5.0);
                    
                    return new Size(intClientWidth, intClientHeight);
                }
            }
            
            public override Size CardSetSizeForClient
            {
                get
                {
                    int intMaxWidth = (int)((double)(this.ClientSize.Width - 
                                                     10*HorizontalSpace() - 
                                                     32*MinVisible())/3.0);
                                                     
                    int intMaxHeight = (int)((double)(this.ClientSize.Height - 
                                                     5*VerticalSpace())/4.0);
                    
                    return new Size(intMaxWidth, intMaxHeight);
                }
            }
            
            public override string GameTitle
		    {
		        get
		        {
		            return "Beleaguered Castle";
		        }
		    }
            
            public override bool AutoFinishAvailable
            {
                get
                {
                    return true;
                }
            }
            
            //-----------------------------------------------------------------//
            
            public override void AutoFinish()
            {
                bool blnCanAutoFinish = true;
                for (int intRow=0; intRow<8 && blnCanAutoFinish; intRow++)
                {
                    bool blnRowDescending = true;
                    if (this.Game.Row(intRow) != null && this.Game.Row(intRow).Count>0)
                    {
                        Rank lastRank = Rank.None;
                        for (int intIndex=0; intIndex<this.Game.Row(intRow).Count && blnRowDescending; intIndex++)
                        {
                            if (lastRank != Rank.None)
                            {
                                if (lastRank < this.Game.Row(intRow)[intIndex].Rank)
                                {
                                    // not descending
                                    blnRowDescending = false;
                                }
                            }
                            lastRank = this.Game.Row(intRow)[intIndex].Rank;
                        }
                    }
                    blnCanAutoFinish &= blnRowDescending;                    
                }
                
                if (blnCanAutoFinish)
                {
                    int intNextRow = 0;
                    while (!this.Game.IsGameWon())
                    {
                        int intFoundationTo = -1;
                        for (int intFoundation=0; intFoundation<4 && intFoundationTo<0; intFoundation++)
                        {
                            if (((BeleagueredCastle)this.Game).IsLegalMoveRowToFoundation(intNextRow, intFoundation))
                            {
                                intFoundationTo = intFoundation;
                            }
                        }
                        if (intFoundationTo < 0)
                        {
                            intNextRow ++;
                            if (intNextRow >= 8)
                            {
                                intNextRow = 0;
                            }
                        }
                        else
                        {
                            CardTableRegion regionFrom = 
                                new CardTableRegion(CardTableRegionType.Row, intNextRow);
                            CardTableRegion regionTo = 
                                new CardTableRegion(CardTableRegionType.Foundation, intFoundationTo);
                                
                            CardTableZone zoneFrom = 
                                this.Zones.GetZoneByTypeAndRegion(CardTableZoneType.PickUpZone, regionFrom);
                            CardTableZone zoneTo = 
                                this.Zones.GetZoneByTypeAndRegion(CardTableZoneType.DropZone, regionTo);
                            
                            this.Game.Move(regionFrom, regionTo);
                            MoveCard(zoneFrom, zoneTo);
                            Invalidate();
                            this.Update();
                        }
                    }
                    this.GameWon();                
                }
            }
            
            //-----------------------------------------------------------------//
            
            public override string Copyright
            {
                get
                {
                    return "Copyright (C) 2005 Trevor Barnett";
                }
            }
            
            //-----------------------------------------------------------------//
            
            public override string Website
            {
                get
                {
                    return "http://www.e381.net";
                }
            }
            
            //-----------------------------------------------------------------//
            
            public override string ContactEmail
            {
                get
                {
                    return "swst@e381.net";
                }
            }
            
            //-----------------------------------------------------------------//
            
            public override string AdditionalInfo
            {
                get
                {
                    return "";
                }
            }
            
            protected override void OnLayout(LayoutEventArgs lev)
            {
                for (int intIndex=0; intIndex<mc_intNumberOfRows; intIndex++)
                {
                    ma_blnRowChanged[intIndex] = true;
                }
                for (int intIndex=0; intIndex<4; intIndex++)
                {
                    ma_blnFoundationChanged[intIndex] = true;
                }
                
                base.OnLayout(lev);
            }
            
            protected override void InitialiseZones()
            {
                for (int intRow=0; intRow<8; intRow++)
                {
                    // row pick up zone
                    CardTableZone objZone = new CardTableZone(
                        CardTableZoneType.PickUpZone, 
                        new CardTableRegion(CardTableRegionType.Row, intRow), 
                        0, 0, 0, 0);
                    this.Zones.Add(objZone);
                    
                    // row drop zone
                    objZone = new CardTableZone(
                        CardTableZoneType.DropZone, 
                        new CardTableRegion(CardTableRegionType.Row, intRow), 
                        0, 0, 0, 0);
                    this.Zones.Add(objZone);
                }
                
                // foundation drop zones
                for (int intFoundation=0; intFoundation<4; intFoundation++)
                {
                    CardTableZone objZone = new CardTableZone(
                        CardTableZoneType.DropZone, 
                        new CardTableRegion(CardTableRegionType.Foundation, intFoundation), 
                        0, 0, 0, 0);
                    this.Zones.Add(objZone);
                }
            }
            
            protected override void SetZones()
            {
                SetFoundationDropZones();
                SetPickUpZones();
                SetDropZones();
            }
            
            //-----------------------------------------------------------------//
            
            private void SetPickUpZone(int intRow)
            {
                if (this.CardSet == null || this.Game == null || this.Game.Row(0) == null)
                {
                    return;
                }
                
                int intX; int intY;
                int intLastIndex = this.Game.Row(intRow).Count-1;
                    
                if (intRow < 4)
                {
                    intX = m_intWindowCentreX-4*m_intHorizontalSpace-this.CardSet.Width-
                           (int)(ma_dblWidthVisibleOverlappedCard[intRow]*intLastIndex);
                    intY = m_intVerticalEdgeSpace+
                           (m_intVerticalSpace+this.CardSet.Height)*intRow;
                }
                else
                {
                    intX = m_intWindowCentreX+4*m_intHorizontalSpace+this.CardSet.Width+
                           (int)(ma_dblWidthVisibleOverlappedCard[intRow]*intLastIndex);
                    intY = m_intVerticalEdgeSpace+
                           (m_intVerticalSpace+this.CardSet.Height)*(intRow-4);
                }
                  
                SetZone(CardTableZoneType.PickUpZone, new CardTableRegion(CardTableRegionType.Row, intRow), 
                        intX, intY, this.CardSet.Width, this.CardSet.Height);
            }
            
            //-----------------------------------------------------------------//
            
            private void SetPickUpZones(int[] a_intRows)
            {
                foreach (int intRow in a_intRows)
                {
                    SetPickUpZone(intRow);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void SetPickUpZones()
            {
                int[] a_intZones = {0,1,2,3,4,5,6,7};
                SetPickUpZones(a_intZones);
            }
            
            //-----------------------------------------------------------------//
            
            private void SetDropZone(int intRow)
            {
                if (this.CardSet == null || this.Game == null || this.Game.Row(0) == null)
                {
                    return;
                }
                
                int intX; int intY;
                int intLastIndex = this.Game.Row(intRow).Count-1;
                    
                if (intRow < 4)
                {
                    intX = m_intWindowCentreX-4*m_intHorizontalSpace-this.CardSet.Width-
                           (int)(ma_dblWidthVisibleOverlappedCard[intRow]*intLastIndex);
                    intY = m_intVerticalEdgeSpace+
                           (m_intVerticalSpace+this.CardSet.Height)*intRow;
                           
                    SetZone(CardTableZoneType.DropZone, new CardTableRegion(CardTableRegionType.Row, intRow), 
                            m_intHorizontalEdgeSpace, 
                            intY,
                            intX-m_intHorizontalEdgeSpace+this.CardSet.Width, 
                            this.CardSet.Height);
                }
                else
                {
                    intX = m_intWindowCentreX+4*m_intHorizontalSpace+this.CardSet.Width+
                           (int)(ma_dblWidthVisibleOverlappedCard[intRow]*intLastIndex);
                    intY = m_intVerticalEdgeSpace+
                           (m_intVerticalSpace+this.CardSet.Height)*(intRow-4);
                           
                    SetZone(CardTableZoneType.DropZone, new CardTableRegion(CardTableRegionType.Row, intRow),
                            intX, 
                            intY,
                            ClientSize.Width-m_intHorizontalEdgeSpace-intX, 
                            this.CardSet.Height);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void SetDropZones(int[] a_intRows)
            {
                foreach (int intRow in a_intRows)
                {
                    SetDropZone(intRow);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void SetDropZones()
            {
                int[] a_intZones = {0,1,2,3,4,5,6,7};
                SetDropZones(a_intZones);
            }
            
            //-----------------------------------------------------------------//
            
            private void SetFoundationDropZone(int intFoundation)
            {
                if (this.CardSet == null)
                {
                    return;
                }
                
                int intX = m_intWindowCentreX;
                int intY = m_intVerticalEdgeSpace+
                           (m_intVerticalSpace + this.CardSet.Height) * intFoundation;
                    
                SetZone(CardTableZoneType.DropZone, 
                        new CardTableRegion(CardTableRegionType.Foundation, intFoundation),
                        intX, 
                        intY,
                        this.CardSet.Width, 
                        this.CardSet.Height);
            }
            
            //-----------------------------------------------------------------//
            
            private void SetFoundationDropZones(int[] a_intFoundations)
            {
                foreach (int intFoundation in a_intFoundations)
                {
                    SetFoundationDropZone(intFoundation);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void SetFoundationDropZones()
            {
                int[] a_intZones = {0,1,2,3};
                SetFoundationDropZones(a_intZones);
            }
            
            //-----------------------------------------------------------------//
            
            protected override void CalculateMaxCardSize()
            {
                int intMaxWidth = (int)((double)(this.ClientSize.Width - 
                                                 10*HorizontalSpace() - 
                                                 32*MinVisible())/3.0);
                                                 
                int intMaxHeight = (int)((double)(this.ClientSize.Height - 
                                                 5*VerticalSpace())/4.0);
                
                this.MaxCardSize = new Size(intMaxWidth, intMaxHeight);
                
            }
                
            // Pick up card from row
            protected override Bitmap PickUpCards(CardTableZone zone)
            {
                Card pickedUpCard = this.Game.Row(zone.Region.Index).Last;
                Bitmap bmpPickedUpCards = null;
                
                if (pickedUpCard == null || pickedUpCard.Bitmap == null)
                {
                    // empty row (or card is missing a bitmap!)
                    pickedUpCard = null;
                }      
                else
                {
                    // re-draw row from, less top card
                    bmpPickedUpCards = pickedUpCard.Bitmap;
                    ma_blnRowChanged[zone.Region.Index] = true;
                    ma_blnRowExcludeLast[zone.Region.Index] = true;
                        
                }
                
                return bmpPickedUpCards;
            }
            
            // Redraw the rows and foundation involved in the move
            protected override void MoveCard(
                CardTableZone fromZone, CardTableZone toZone)
            {
                // need to redraw the affected rows/foundations
                ma_blnRowChanged[fromZone.Region.Index] = true;
                ma_blnRowExcludeLast[fromZone.Region.Index] = false;
                CalculateOverlap(fromZone.Region.Index);
                if (toZone.Region.Type == CardTableRegionType.Foundation)
                {
                    ma_blnFoundationChanged[toZone.Region.Index] = true;
                }
                else
                {
                    ma_blnRowChanged[toZone.Region.Index] = true;
                    CalculateOverlap(toZone.Region.Index);
                }
                
                SetZones();
            }
            
            // Redraw row card was moved from, 
            // hence returning card to original position
            protected override void ReplaceCard(CardTableZone fromZone)
            {
                // need to redraw row card was picked up from
                ma_blnRowChanged[fromZone.Region.Index] = true;
                ma_blnRowExcludeLast[fromZone.Region.Index] = false;
                    
            }
            
            protected override bool BackgroundChanged
            {
                get
                {
                    bool blnChanged = false;
                    for (int intIndex=0; 
                         intIndex<mc_intNumberOfRows && !blnChanged; 
                         intIndex++)
                    {
                        blnChanged |= ma_blnRowChanged[intIndex];
                    }
                    for (int intIndex=0; intIndex<4 && !blnChanged; intIndex++)
                    {
                        blnChanged |= ma_blnFoundationChanged[intIndex];
                    }
                    return blnChanged;
                }
            }
            
            protected override bool BackgroundAllChanged
            {
                get
                {
                    bool blnChanged = true;
                    for (int intIndex=0; 
                         intIndex<mc_intNumberOfRows && blnChanged; 
                         intIndex++)
                    {
                        blnChanged &= ma_blnRowChanged[intIndex];
                    }
                    for (int intIndex=0; intIndex<4 && blnChanged; intIndex++)
                    {
                        blnChanged &= ma_blnFoundationChanged[intIndex];
                    }
                    return blnChanged;
                }
                set
                {
                    for (int intIndex=0; intIndex<mc_intNumberOfRows; intIndex++)
                    {
                        ma_blnRowChanged[intIndex] = value;
                    }
                    for (int intIndex=0; intIndex<4; intIndex++)
                    {
                        ma_blnFoundationChanged[intIndex] = value;
                    }
                }                
            }
            
            protected override void DrawDisplay(Graphics objGraphics)
            {    
                if (BackgroundAllChanged)
                {
                    // draw background
                    objGraphics.FillRectangle(this.Background.Brush, 
                                              0,0,this.Width,this.Height);
                }
                if (this.Game == null ||
                    this.Game.Foundations == null || this.Game.Rows == null ||
                    this.CardSet == null || this.Game.Foundation(0) == null ||
                    this.Game.Row(0) == null)
                {
                    // cards not loaded, exit and don't draw anything
                    return;
                }
                
                // display playing cards
                
                // display foundations
                DrawFoundations(objGraphics);
                
                // display rows
                DrawRows(objGraphics);
                
        	} // end method
            
            //-----------------------------------------------------------------//
            
            private int HorizontalSpace()
            {
                int intSpace = mc_intMinHorizontalSpace;
                if (ClientSize.Width < m_DefaultStandardClientSize.Width)
                {
                    int intHSpaceDiff = 
                        mc_intStandardHorizontalSpace - 
                        mc_intMinHorizontalSpace;
                    int intMinClientWidth = 
                        MinimumClientSizeForCardSet.Width;
                    int intHSizeDiff =
                        m_DefaultStandardClientSize.Width -
                        intMinClientWidth;
                        
                    intSpace = Math.Max(mc_intMinHorizontalSpace,
                        (int)Math.Floor(
                          (double)(intHSpaceDiff *
                           ClientSize.Width +
                          ((m_DefaultStandardClientSize.Width *
                            mc_intMinHorizontalSpace) -
                           (intMinClientWidth *
                            mc_intStandardHorizontalSpace))) /
                          (double)intHSizeDiff));
                }
                else
                {
                    intSpace = Math.Max(mc_intMinHorizontalSpace, 
                        (int)Math.Floor(
                          (double)mc_intStandardHorizontalSpace * 
                          (double)ClientSize.Width /
                          (double)m_DefaultStandardClientSize.Width));
                }
                return intSpace;
            }
            
            //-----------------------------------------------------------------//
            
            private int VerticalSpace()
            {
                int intStandardVerticalSpace = mc_intStandardVerticalSpace;
                
                if (SystemInformation.WorkingArea.Width <= 
                    WorkingAreaSize800x600.Width ||
                    SystemInformation.WorkingArea.Height <= 
                    WorkingAreaSize800x600.Height)
                {
                    intStandardVerticalSpace = mc_intStandardVerticalSpaceLTE800x600;
                }
                
                int intSpace = mc_intMinVerticalSpace;
                if (ClientSize.Height < m_DefaultStandardClientSize.Height)
                {
                    int intVSpaceDiff = 
                        intStandardVerticalSpace - 
                        mc_intMinVerticalSpace;
                    int intMinClientHeight = 
                        MinimumClientSizeForCardSet.Height;
                    int intVSizeDiff =
                        m_DefaultStandardClientSize.Height -
                        intMinClientHeight;
                        
                    intSpace = Math.Max(mc_intMinVerticalSpace,
                        (int)Math.Floor(
                          (double)(intVSpaceDiff *
                           ClientSize.Height +
                          ((m_DefaultStandardClientSize.Height *
                            mc_intMinVerticalSpace) -
                           (intMinClientHeight *
                            intStandardVerticalSpace))) /
                          (double)intVSizeDiff));
                }
                else
                {
                    intSpace = Math.Max(mc_intMinVerticalSpace, 
                        (int)Math.Floor(
                          (double)intStandardVerticalSpace * 
                          (double)ClientSize.Height /
                          (double)m_DefaultStandardClientSize.Height));
                }
                return intSpace;
            }
            
            //-----------------------------------------------------------------//
            
            protected Size WorkingAreaSize800x600
            {
                get
                {
                    return m_WorkingAreaSize800x600;
                }
            }
            
            //-----------------------------------------------------------------//
            
            private int HorizontalEdgeSpace()
            {
                int intValue = HorizontalSpace();
                             
                return intValue;
            }
                
            //-----------------------------------------------------------------//
            
            private int VerticalEdgeSpace()
            {
                int intValue = 
                    (int)((double)(ClientSize.Height - 4*this.CardSet.Height -
                             3*VerticalSpace())/2.0);
                             
                return intValue;
            }
            
            //-----------------------------------------------------------------//
            
            private int RowWidth()
            {
                int intValue = 
                    (int)((double)(ClientSize.Width - this.CardSet.Width -
                             2*HorizontalEdgeSpace() - 8*HorizontalSpace())/2.0);
                             
                return intValue;
            }
            
            //-----------------------------------------------------------------//
            
            private double MinVisible()
            {
                double dblVisible = 
                    Math.Max((double)mc_intMinVisible,
                             (double)mc_intStandardVisible * 
                             (double)ClientSize.Width /
                             (double)m_DefaultStandardClientSize.Width);
                
                return dblVisible;
            }
            
            //-----------------------------------------------------------------//
            
            protected override void CalculateLayoutValues()
            {            
                // recalculate layout values
                if (this.CardSet == null || this.Game == null || 
                    this.Game.Row(0) == null)
                {
                    // not yet initialised so have to wait
                    return;
                }
                
                // centre coordinates
                m_intWindowCentreX = (ClientSize.Width  - this.CardSet.Width)/2;
                m_intWindowCentreY = (ClientSize.Height - this.CardSet.Height)/2;
                
                m_intHorizontalSpace     = HorizontalSpace();
                m_intVerticalSpace       = VerticalSpace();
                m_intRowWidth            = RowWidth();
                m_intHorizontalEdgeSpace = HorizontalEdgeSpace();
                m_intVerticalEdgeSpace   = VerticalEdgeSpace();
                CalculateOverlaps();
            }
            
            //-----------------------------------------------------------------//
            
            private void CalculateOverlaps()
            {
                for (int intIndex=0; intIndex<mc_intNumberOfRows; intIndex++)
                {
                    CalculateOverlap(intIndex);
                }
            }
            
            //-----------------------------------------------------------------//
               
            private void CalculateOverlap(int intRow)
            {
                double dblMinVisible = (double)MinVisible();
                double dblHalfCardWidth = dblMinVisible;
                if (this.CardSet != null)
                {
                    dblHalfCardWidth = Math.Max((double)this.CardSet.Width/2.0,
                                                dblMinVisible);
                }
                if (this.Game.Row(intRow) == null || this.CardSet == null)
                {
                    ma_dblWidthVisibleOverlappedCard[intRow] = dblMinVisible;
                }
                else
                {            
                    int intRowCount = this.Game.Row(intRow).Count;
                    
                    if (intRowCount <= 1)
                    {
                        ma_dblWidthVisibleOverlappedCard[intRow] = dblHalfCardWidth;
                    }
                    else
                    {   
                        ma_dblWidthVisibleOverlappedCard[intRow] = 
                            Math.Max(dblMinVisible, 
                                     Math.Min(dblHalfCardWidth,
                                              (RowWidth()-this.CardSet.Width) / 
                                              (intRowCount-1)));
                    }
                }
            }
    
            private void DrawRow(int intRow, Graphics objGraphics)
            {
                DrawRow(intRow, objGraphics, ma_blnRowExcludeLast[intRow]);
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawRow(int intRow, Graphics objGraphics,
                                 bool blnExcludeLast)
            {
                if (!ma_blnRowChanged[intRow])
                {
                    return;
                }
                
                // draw row
                int intCardX; int intCardY;
                        
                // draw background rectangle to clear row
                if (intRow < 4)
                {
                    intCardY = m_intVerticalEdgeSpace+
                               (m_intVerticalSpace+this.CardSet.Height)*intRow;
                
                    objGraphics.FillRectangle(this.Background.Brush, 
                                              m_intHorizontalEdgeSpace, 
                                              intCardY,
                                              m_intRowWidth,
                                              this.CardSet.Height);
                }
                else
                {
                    intCardY = m_intVerticalEdgeSpace+
                               (m_intVerticalSpace+this.CardSet.Height)*(intRow-4);
                                       
                    objGraphics.FillRectangle(this.Background.Brush, 
                                              m_intWindowCentreX+this.CardSet.Width+
                                              4*m_intHorizontalSpace, intCardY, 
                                              m_intRowWidth, this.CardSet.Height);
                }                    
                            
                int intPosition = 0;                   
                foreach (Card card in this.Game.Row(intRow))
                {
                    if (card != this.Game.Row(intRow).Last || !blnExcludeLast)
                    {
                        if (intRow < 4)
                        {
                            intCardX = m_intWindowCentreX-4*m_intHorizontalSpace-
                                       this.CardSet.Width-
                                       (int)(ma_dblWidthVisibleOverlappedCard[intRow]*
                                             intPosition);
                        }
                        else
                        {
                            intCardX = m_intWindowCentreX+4*m_intHorizontalSpace+
                                       this.CardSet.Width+
                                       (int)(ma_dblWidthVisibleOverlappedCard[intRow]*
                                             intPosition);
                        }
                        
                        try
                        {
                            objGraphics.DrawImage(card.Bitmap, intCardX, intCardY);
                        }
                        catch
                        {
                        }
                    }
                    
                    intPosition ++;
                } // next card
                
                ma_blnRowChanged[intRow] = false;
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawFoundation(int intFoundation, Graphics objGraphics)
            {
                if (!ma_blnFoundationChanged[intFoundation])
                {
                    return;
                }
                
                int intX = m_intWindowCentreX;
                int intY = m_intVerticalEdgeSpace+
                       (m_intVerticalSpace + this.CardSet.Height) * 
                       intFoundation;
                                   
                // fill rectangle with background colour to avoid any
                // of card below showing through if card paritally transparent
                objGraphics.FillRectangle(this.Background.Brush, intX, intY,
                                          this.CardSet.Width,
                                          this.CardSet.Height);
                
                // draw stack card
                StackCard stackCard = this.CardSet.StackCard(intFoundation);
                if (stackCard != null && stackCard.Bitmap != null)
                {
                    objGraphics.DrawImage(stackCard.Bitmap, intX, intY);
                }       
                
                foreach (Card card in this.Game.Foundation(intFoundation))
                {
                    Card TopCard = card;
                    
                    if (TopCard != null && TopCard.Bitmap != null)
                    {
                        try
                        {
                            // draw card               
                            objGraphics.DrawImage(TopCard.Bitmap, intX, intY);
                        }
                        catch
                        {
                        }
                    }
                }
                
                ma_blnFoundationChanged[intFoundation] = false;
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawRows(Graphics objGraphics)
            {
                int[] a_intRows = {0,1,2,3,4,5,6,7};
                DrawRows(a_intRows, objGraphics);
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawRows(int[] a_intRows, Graphics objGraphics)
            {
                foreach (int intRow in a_intRows)
                {
                    DrawRow(intRow, objGraphics);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawFoundations(Graphics objGraphics)
            {
                int[] a_intFoundations = {0,1,2,3};
                DrawFoundations(a_intFoundations, objGraphics);
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawFoundations(int[] a_intFoundations, 
                                         Graphics objGraphics)
            {
                foreach (int intFoundations in a_intFoundations)
                {
                    DrawFoundation(intFoundations, objGraphics);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawRowsAndFoundations(int[] a_intRows, 
                                                int[] a_intFoundations,
                                                Graphics objGraphics)
            {
                foreach (int intRow in a_intRows)
                {
                    DrawRow(intRow, objGraphics);
                }
                foreach (int intFoundations in a_intFoundations)
                {
                    DrawFoundation(intFoundations, objGraphics);
                }
            }
        }
    }
}