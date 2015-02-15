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
 * File:          KlondikeControl.cs                                     *
 * Namespace:     SwSt.CardGames                                         *
 * Last modified: 13 January 2005                                        *
 * Class:         KlondikeControl                                        *
 * Description:   Klondike card game control.                            *
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
        public class KlondikeControl : CardGameControl
        {
            Rectangle m_recBorder = new Rectangle(0,0,0,0);
            
            // layout values
            private const int       mc_intNumberOfColumns            = 7;
            private const int       mc_intMaxOverlappedCards         = 18;
            private       int       m_intColumnHeight                = 0;
            private const int       mc_intMinHorizontalSpace         = 1;
            private const int       mc_intStandardHorizontalSpace    = 5;
            private       int       m_intHorizontalEdgeSpace         = 5;
            private       int       m_intHorizontalSpace             = 5;
            private const int       mc_intMinVerticalSpaceGT800x600  = 5;//5;
            private const int       mc_intMinVerticalSpaceLTE800x600 = 1;
            private const int       mc_intMinVerticalSpace           = 1;
            private const int       mc_intStandardVerticalSpace      = 5;
            private       int       m_intVerticalEdgeSpace           = 5;//5;
            private       double[]  ma_dblVerticalVisible 
                = new double[mc_intNumberOfColumns];
            private const int       mc_intMinVerticalVisible         = 8;
            private const int       mc_intStandardVerticalVisible    = 12;
            private       double    m_dblVerticalVisible             = 12.0;
            private const int       mc_intMinHorizontalVisible       = 8;
            private const int       mc_intStandardHorizontalVisible  = 12;
            private       double    m_dblHorizontalVisible           = 12.0;
            private       Size      m_MaxCardSize = new Size(0,0);
            private       Size      m_DefaultStandardClientSize = new Size(656, 501);
            
            private       bool[]    ma_blnColumnChanged
                = new bool[7]{false,false,false,false,false,false,false};
            private       bool[]    ma_blnFoundationChanged 
                = new bool[4]{false,false,false,false};
            private       bool      m_blnReserveChanged = false;
            private       bool      m_blnDiscardChanged = false;
            private       bool[]    ma_blnColumnExcludeLast
                = new bool[7]{false,false,false,false,false,false,false};
            private       bool[]    ma_blnColumnExcludeFaceUp
                = new bool[7]{false,false,false,false,false,false,false};
            private       bool      m_blnDiscardExcludeLast = false;
            
            
            public KlondikeControl() : base()
            {
                this.CardSet = new SwSt.CardGames.CardSet();
                this.Game = new SwSt.CardGames.Klondike();
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
                    (double)intCardSetWidth *
                    (double)mc_intNumberOfColumns + 
                    (double)(mc_intNumberOfColumns+1) *
                    (double)intHorizontalSpace);
                        
                int intClientHeight = (int)Math.Ceiling(
                    (double)intCardSetHeight * 2 +
                    (double)mc_intMaxOverlappedCards *
                    (double)mc_intMinVerticalVisible + 
                    5.0 * (double)intVerticalSpace);
                    
                return new Size(intClientWidth,intClientHeight);                
            }
            
            public override Size MinimumClientSizeForCardSet
            {
                get
                {
                    int intHorizontalSpace = mc_intMinHorizontalSpace;
                    int intVerticalSpace   = mc_intMinVerticalSpace;
                    
                    int intClientWidth = (int)Math.Ceiling(
                        (double)this.CardSet.MinWidth *
                        (double)mc_intNumberOfColumns + 
                        (double)(mc_intNumberOfColumns+1) *
                        (double)intHorizontalSpace);
                        
                    int intClientHeight = (int)Math.Ceiling(
                        (double)this.CardSet.MinHeight * 2 +
                        (double)mc_intMaxOverlappedCards *
                        (double)mc_intMinVerticalVisible + 
                        5.0 * (double)intVerticalSpace);
                    
                    return new Size(intClientWidth,intClientHeight);
                }
            }
            
            public override Size ClientSizeForCardSet
            {
                get
                {
                    int intHorizontalSpace = mc_intStandardHorizontalSpace;
                    int intVerticalSpace   = mc_intStandardVerticalSpace;
                    
                    int intClientWidth = (int)Math.Ceiling(
                        (double)this.CardSet.UnscaledWidth *
                        (double)mc_intNumberOfColumns + 
                        (double)(mc_intNumberOfColumns+1) *
                        (double)intHorizontalSpace);
                        
                    int intClientHeight = (int)Math.Ceiling(
                        (double)this.CardSet.UnscaledHeight * 2 +
                        (double)mc_intMaxOverlappedCards *
                        (double)mc_intStandardVerticalVisible + 
                        5.0 * (double)intVerticalSpace);
                    
                    return new Size(intClientWidth,intClientHeight);
                }
            }
            
            public override Size ClientSizeForScaledCardSet
            {
                get
                {
                    int intHorizontalSpace = mc_intStandardHorizontalSpace;
                    int intVerticalSpace   = mc_intStandardVerticalSpace;
                    
                    int intClientWidth = (int)Math.Ceiling(
                        (double)this.CardSet.Width *
                        (double)mc_intNumberOfColumns + 
                        (double)(mc_intNumberOfColumns+1) *
                        (double)intHorizontalSpace);
                        
                    int intClientHeight = (int)Math.Ceiling(
                        (double)this.CardSet.Height * 2 +
                        (double)mc_intMaxOverlappedCards *
                        (double)mc_intStandardVerticalVisible + 
                        5.0 * (double)intVerticalSpace);
                    
                    return new Size(intClientWidth,intClientHeight);
                }
            }            
            
            public override Size CardSetSizeForClient
            {
                get
                {
                    int intHorizontalSpace = HorizontalSpace();
                    int intVerticalSpace   = VerticalSpace();
                    
                    int intMaxWidth  = (int)Math.Floor(
                        ((double)ClientSize.Width -
                         (double)(mc_intNumberOfColumns+1) * 
                         (double)intHorizontalSpace)/(double)mc_intNumberOfColumns);
                    int intMaxHeight = (int)Math.Floor(
                        ((double)ClientSize.Height-(double)mc_intMaxOverlappedCards*
                         (double)MinVerticalVisible()-
                         5.0*(double)intVerticalSpace)/2);
                         
                    return new Size(intMaxWidth, intMaxHeight);
                }
            }
            
            public override string GameTitle
		    {
		        get
		        {
		            return "Klondike";
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
                if ((this.Game.TheReserve != null && 
                     this.Game.TheReserve.Count > 0) ||
                    (this.Game.TheDiscard != null &&
                     this.Game.TheDiscard.Count > 0))
                {
                    blnCanAutoFinish = false;
                }
                for (int intColumn=0; 
                     intColumn<mc_intNumberOfColumns && blnCanAutoFinish; intColumn++)
                {
                    if (((CardGames.Klondike)(this.Game)).
                          FaceDownColumn(intColumn) != null && 
                        ((CardGames.Klondike)(this.Game)).
                          FaceDownColumn(intColumn).Count > 0)
                    {
                        blnCanAutoFinish = false;
                    }                    
                }
                
                if (blnCanAutoFinish)
                {
                    int intNextColumn = 0;
                    
                    while (!this.Game.IsGameWon())
                    {
                        int intFoundationTo = -1;
                        for (int intFoundation=0; 
                             intFoundation<4 && intFoundationTo<0; intFoundation++)
                        {
                            if (((CardGames.Klondike)this.Game).
                                IsLegalMoveColumnEndToFoundation(
                                    intNextColumn, intFoundation))
                            {
                                intFoundationTo = intFoundation;
                            }
                        }
                        if (intFoundationTo < 0)
                        {
                            intNextColumn ++;
                            if (intNextColumn >= mc_intNumberOfColumns)
                            {
                                intNextColumn = 0;
                            }
                        }
                        else
                        {
                            CardTableRegion regionFrom = 
                                new CardTableRegion(CardTableRegionType.ColumnEnd, 
                                                    intNextColumn);
                            CardTableRegion regionTo = 
                                new CardTableRegion(CardTableRegionType.Foundation, 
                                                    intFoundationTo);
                                
                            // animate movement of card
                            CardTableZone zoneFrom = 
                                this.Zones.GetZoneByTypeAndRegion(
                                    CardTableZoneType.PickUpZone, regionFrom);
                            CardTableZone zoneTo = 
                                this.Zones.GetZoneByTypeAndRegion(
                                    CardTableZoneType.DropZone, regionTo);
                                
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
                for (int intIndex=0; intIndex<mc_intNumberOfColumns; intIndex++)
                {
                    ma_blnColumnChanged[intIndex] = true;
                }
                for (int intIndex=0; intIndex<4; intIndex++)
                {
                    ma_blnFoundationChanged[intIndex] = true;
                }
                m_blnReserveChanged = true;
                m_blnDiscardChanged = true;
                
                base.OnLayout(lev);
            }
            
            protected override void InitialiseZones()
            {
                CardTableZone objZone;
                
                for (int intColumn=0; intColumn<mc_intNumberOfColumns; intColumn++)
                {
                    // column pick up zone
                    objZone = new CardTableZone(
                        CardTableZoneType.PickUpZone, 
                        new CardTableRegion(CardTableRegionType.Column, intColumn), 
                        0, 0, 0, 0);
                    this.Zones.Add(objZone);
                    
                    // column end pick up zone
                    objZone = new CardTableZone(
                        CardTableZoneType.PickUpZone, 
                        new CardTableRegion(CardTableRegionType.ColumnEnd, intColumn), 
                        0, 0, 0, 0);
                    this.Zones.Add(objZone);
                    
                    // column drop zone
                    objZone = new CardTableZone(
                        CardTableZoneType.DropZone, 
                        new CardTableRegion(CardTableRegionType.Column, intColumn), 
                        0, 0, 0, 0);
                    this.Zones.Add(objZone);
                }
                
                // foundation drop zones
                for (int intFoundation=0; intFoundation<4; intFoundation++)
                {
                    objZone = new CardTableZone(
                        CardTableZoneType.DropZone, 
                        new CardTableRegion(CardTableRegionType.Foundation, intFoundation), 
                        0, 0, 0, 0);
                    this.Zones.Add(objZone);
                }
                
                // reserve pick up zone
                objZone = new CardTableZone(
                    CardTableZoneType.ClickZone,
                    new CardTableRegion(CardTableRegionType.Reserve, 0), 0, 0, 0, 0);
                this.Zones.Add(objZone);
                
                // discard pick up zone
                objZone = new CardTableZone(
                    CardTableZoneType.PickUpZone,
                    new CardTableRegion(CardTableRegionType.Discard, 0), 0, 0, 0, 0);
                this.Zones.Add(objZone);
            }
            
            protected override void SetZones()
            {
                SetFoundationDropZones();
                SetPickUpZones();
                SetDropZones();
                SetReserveZone();
                SetDiscardZone();
            }
            
            //-----------------------------------------------------------------//
            
            private void SetPickUpZone(int intColumn)
            {
                if (this.CardSet == null || this.Game == null || 
                    this.Game.Column(0) == null)
                {
                    return;
                }
                
                int intX; int intY; int intWidth; int intHeight;
                int intLastIndex = this.Game.Column(intColumn).Count-1;
                int intFaceDownCards = 
                    ((SwSt.CardGames.Klondike)(this.Game)).FaceDownColumn(intColumn).Count;
                    
                // column top
                    
                if (intLastIndex < 1)
                {
                    intX = 0; intY=0; intWidth=0; intHeight=0;
                }
                else
                {
                        
                    intX = m_intHorizontalEdgeSpace+
                           (m_intHorizontalSpace+this.CardSet.Width)*intColumn;
                    intY = m_intVerticalEdgeSpace + 
                           (int)(m_dblVerticalVisible*
                                 intFaceDownCards);
                           
                    intWidth  = this.CardSet.Width;
                    intHeight = (int)ma_dblVerticalVisible[intColumn];
                }    
                
                SetZone(CardTableZoneType.PickUpZone, 
                        new CardTableRegion(CardTableRegionType.Column, intColumn), 
                        intX, intY, intWidth, intHeight);
                        
                if (intLastIndex >= 0)
                {
                    intX = m_intHorizontalEdgeSpace+
                           (m_intHorizontalSpace+this.CardSet.Width)*intColumn;
                    intY = m_intVerticalEdgeSpace + 
                           (int)(m_dblVerticalVisible * intFaceDownCards +
                                 ma_dblVerticalVisible[intColumn] * intLastIndex);
                    intWidth  = this.CardSet.Width;
                    intHeight = this.CardSet.Height;
                }
                           
                SetZone(CardTableZoneType.PickUpZone, 
                        new CardTableRegion(CardTableRegionType.ColumnEnd, intColumn), 
                        intX, intY, intWidth, intHeight);
            }
            
            //-----------------------------------------------------------------//
            
            private void SetPickUpZones(int[] a_intColumns)
            {
                foreach (int intColumn in a_intColumns)
                {
                    SetPickUpZone(intColumn);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void SetPickUpZones()
            {
                int[] a_intZones = {0,1,2,3,4,5,6};
                SetPickUpZones(a_intZones);
            }
            
            //-----------------------------------------------------------------//
            
            private void SetDropZone(int intColumn)
            {
                if (this.CardSet == null || this.Game == null || 
                    this.Game.Column(0) == null)
                {
                    return;
                }
                
                int intX; int intY; int intWidth; int intHeight;
                int intLastIndex = this.Game.Column(intColumn).Count-1;
                if (intLastIndex < 0)
                {
                    intLastIndex = 0;
                }
                int intFaceDownCards = 
                    ((SwSt.CardGames.Klondike)(this.Game)).FaceDownColumn(intColumn).Count;
                    
                intX = m_intHorizontalEdgeSpace+
                       (m_intHorizontalSpace+this.CardSet.Width)*intColumn;
                intY = m_intVerticalEdgeSpace + 
                       (int)(m_dblVerticalVisible * intFaceDownCards + 
                             ma_dblVerticalVisible[intColumn] * intLastIndex);
                intWidth  = this.CardSet.Width;
                intHeight = this.CardSet.Height;
                              
                SetZone(CardTableZoneType.DropZone, 
                        new CardTableRegion(CardTableRegionType.Column, intColumn), 
                        intX, intY, intWidth, intHeight);
            }
            
            //-----------------------------------------------------------------//
            
            private void SetDropZones(int[] a_intColumns)
            {
                foreach (int intColumn in a_intColumns)
                {
                    SetDropZone(intColumn);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void SetDropZones()
            {
                int[] a_intZones = {0,1,2,3,4,5,6};
                SetDropZones(a_intZones);
            }
            
            //-----------------------------------------------------------------//
            
            private void SetFoundationDropZone(int intFoundation)
            {
                if (this.CardSet == null)
                {
                    return;
                }
                
                int intX = m_intHorizontalEdgeSpace+
                           (this.CardSet.Width+m_intHorizontalSpace) *
                           (3+intFoundation);
                int intY = ClientSize.Height - this.CardSet.Height - 
                           m_intVerticalEdgeSpace;
                    
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
            
            private void SetReserveZone()
            {
                int intX; int intY; int intWidth; int intHeight;
                
                if (this.Game.TheReserve.Count < 1 && this.Game.TheDiscard.Count < 1)
                {
                    intX = 0; intY = 0; intWidth = 0; intHeight = 0;
                }
                else
                {
                    intX = m_intHorizontalEdgeSpace;
                    intY = ClientSize.Height - this.CardSet.Height - 
                           m_intVerticalEdgeSpace;
                    intWidth = this.CardSet.Width;
                    intHeight = this.CardSet.Height;
                }
                    
                SetZone(CardTableZoneType.ClickZone, 
                        new CardTableRegion(CardTableRegionType.Reserve, 0),
                        intX, 
                        intY,
                        intWidth, 
                        intHeight);
            }
            
            //-----------------------------------------------------------------//
            
            private void SetDiscardZone()
            {
                int intX; int intY; int intWidth; int intHeight;
                
                if (this.Game.TheDiscard.Count < 1)
                {
                    intX = 0; intY = 0; intWidth = 0; intHeight = 0;
                }
                else
                {
                    intY = ClientSize.Height - this.CardSet.Height - 
                           m_intVerticalEdgeSpace;
                    intWidth = this.CardSet.Width;
                    intHeight = this.CardSet.Height;
                    if (this.Game.TheDiscard.Count >= 3)
                    {
                        intX = m_intHorizontalEdgeSpace + this.CardSet.Width +
                               m_intHorizontalSpace + (int)(2 * m_dblHorizontalVisible);
                    }
                    else
                    {
                        intX = m_intHorizontalEdgeSpace + this.CardSet.Width +
                               m_intHorizontalSpace + 
                               (int)
                               ((this.Game.TheDiscard.Count-1) * m_dblHorizontalVisible);
                    }
                }
                
                SetZone(CardTableZoneType.PickUpZone, 
                        new CardTableRegion(CardTableRegionType.Discard, 0),
                        intX, 
                        intY,
                        intWidth, 
                        intHeight);
            }
            
            protected override void CalculateMaxCardSize()
            {
                int intHorizontalSpace = HorizontalSpace();
                int intVerticalSpace   = VerticalSpace();
                    
                int intMaxWidth  = (int)Math.Floor(
                    ((double)ClientSize.Width -
                    (double)(mc_intNumberOfColumns+1) * 
                    (double)intHorizontalSpace)/(double)mc_intNumberOfColumns);
                int intMaxHeight = (int)Math.Floor(
                    ((double)ClientSize.Height-(double)mc_intMaxOverlappedCards*
                    (double)MinVerticalVisible()-
                    5.0*(double)intVerticalSpace)/2);
                                       
                this.MaxCardSize = new Size(intMaxWidth, intMaxHeight);
            }
            
            // Pick up bitmap for card or cards
            protected override Bitmap PickUpCards(CardTableZone zone)
            {
                Bitmap bmpPickedUpCards = null;
                
                if (zone.Region.Type == CardTableRegionType.ColumnEnd)
                {
                    Card pickedUpCard = this.Game.Column(zone.Region.Index).Last;
                
                    if (pickedUpCard == null || pickedUpCard.Bitmap == null)
                    {
                        // empty column (or card is missing a bitmap!)
                        bmpPickedUpCards = null;
                    }      
                    else
                    {
                        // re-draw column from, less top card
                        ma_blnColumnChanged[zone.Region.Index] = true;
                        ma_blnColumnExcludeLast[zone.Region.Index] = true;
                        ma_blnColumnExcludeFaceUp[zone.Region.Index] = false;
                        
                        bmpPickedUpCards = pickedUpCard.Bitmap;
                    }
                }
                else if (zone.Region.Type == CardTableRegionType.Column)
                {
                    // multiple cards picked up
                    int intCardCount = this.Game.Column(zone.Region.Index).Count;
                    int intFaceDownCardCount = 
                        ((SwSt.CardGames.Klondike)this.Game).FaceDownColumn(zone.Region.Index).Count;
                    if (intCardCount > 0)
                    {
                        int intTop = (int)(intFaceDownCardCount*MinVerticalVisible());
                        
                        int intHeight = (int)(
                            intFaceDownCardCount*MinVerticalVisible() +
                            (intCardCount-1)*
                            ma_dblVerticalVisible[zone.Region.Index]) + 
                            this.CardSet.Height - intTop;
                            
                        bmpPickedUpCards = new Bitmap(this.CardSet.Width, intHeight);
                        Graphics graphics = Graphics.FromImage(bmpPickedUpCards);
                        for (int intCounter=0; intCounter < intCardCount; intCounter++)
                        {
                            graphics.DrawImage(
                                this.Game.Column(zone.Region.Index)[intCounter].Bitmap,
                                0, (int)(
                                    intFaceDownCardCount*MinVerticalVisible() + 
                                    intCounter * 
                                    ma_dblVerticalVisible[zone.Region.Index]) - 
                                    intTop);
                        }
                        graphics.Dispose();
                        // re-draw column, less face up cards
                        ma_blnColumnChanged[zone.Region.Index] = true;
                        ma_blnColumnExcludeLast[zone.Region.Index] = false;
                        ma_blnColumnExcludeFaceUp[zone.Region.Index] = true;
                    }
                }                            
                else if (zone.Region.Type == CardTableRegionType.Discard)
                {
                    Card pickedUpCard = this.Game.TheDiscard.Last;
                    
                    if (pickedUpCard == null || pickedUpCard.Bitmap == null)
                    {
                        // empty column (or card is missing a bitmap!)
                        pickedUpCard = null;
                    }      
                    else
                    {
                        // re-draw discard pile, less top card
                        m_blnDiscardChanged = true;
                        m_blnDiscardExcludeLast = true;
                        bmpPickedUpCards = pickedUpCard.Bitmap;
                    }
                }
                
                return bmpPickedUpCards;
            }
            
            protected override void MoveCard(
    		        CardTableZone fromZone, CardTableZone toZone)
    		{
    		    // need to redraw the affected columns/foundations
                if (fromZone.Region.Type == CardTableRegionType.Column ||
                    fromZone.Region.Type == CardTableRegionType.ColumnEnd)
                {
                    ma_blnColumnChanged[fromZone.Region.Index] = true;
                    ma_blnColumnExcludeLast[fromZone.Region.Index] = false;
                    ma_blnColumnExcludeFaceUp[fromZone.Region.Index] = false;
                    CalculateOverlap(fromZone.Region.Index);
                    
                    if (toZone.Region.Type == CardTableRegionType.Foundation)
                    {
                        ma_blnFoundationChanged[toZone.Region.Index] = true;
                    }
                    else
                    {
                        ma_blnColumnChanged[toZone.Region.Index] = true;
                        ma_blnColumnExcludeLast[toZone.Region.Index] = false;
                        ma_blnColumnExcludeFaceUp[toZone.Region.Index] = false;
                        CalculateOverlap(toZone.Region.Index);
                    }
                }
                else if (fromZone.Region.Type == CardTableRegionType.Reserve)
                {
                    m_blnReserveChanged = true;
                    m_blnDiscardChanged = true;
                    m_blnDiscardExcludeLast = false;
                }
                else if (fromZone.Region.Type == CardTableRegionType.Discard)
                {
                    m_blnDiscardChanged = true;
                    m_blnDiscardExcludeLast = false;
                    if (toZone.Region.Type == CardTableRegionType.Foundation)
                    {
                        ma_blnFoundationChanged[toZone.Region.Index] = true;
                    }
                    else
                    {
                        ma_blnColumnChanged[toZone.Region.Index] = true;
                        ma_blnColumnExcludeLast[toZone.Region.Index] = false;
                        ma_blnColumnExcludeFaceUp[toZone.Region.Index] = false;
                        CalculateOverlap(toZone.Region.Index);
                    }
                }
                
                SetZones();
    		}
    		    
    		protected override void ReplaceCard(CardTableZone fromZone)
    		{
    		    // need to redraw region card was picked up from
                if (fromZone.Region.Type == CardTableRegionType.Column ||
                    fromZone.Region.Type == CardTableRegionType.ColumnEnd)
                {
                    ma_blnColumnChanged[fromZone.Region.Index] = true;
                    ma_blnColumnExcludeLast[fromZone.Region.Index] = false;
                    ma_blnColumnExcludeFaceUp[fromZone.Region.Index] = false;
                }
                else if (fromZone.Region.Type == CardTableRegionType.Discard)
                {
                    m_blnDiscardChanged = true;
                    m_blnDiscardExcludeLast = false;
                }
    		}
    		
    		protected override bool BackgroundChanged
            {
                get
                {
                    bool blnChanged = false;
                    blnChanged |= m_blnReserveChanged;
                    blnChanged |= m_blnDiscardChanged;
                    for (int intIndex=0; intIndex<7 && !blnChanged; intIndex++)
                    {
                        blnChanged |= ma_blnColumnChanged[intIndex];
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
                    blnChanged &= m_blnReserveChanged;
                    blnChanged &= m_blnDiscardChanged;
                    for (int intIndex=0; intIndex<7 && blnChanged; intIndex++)
                    {
                        blnChanged &= ma_blnColumnChanged[intIndex];
                    }
                    for (int intIndex=0; intIndex<4 && blnChanged; intIndex++)
                    {
                        blnChanged &= ma_blnFoundationChanged[intIndex];
                    }
                    return blnChanged;
                }
                set
                {
                    m_blnReserveChanged = value;
                    m_blnDiscardChanged = value;
                    for (int intIndex=0; intIndex<7; intIndex++)
                    {
                        ma_blnColumnChanged[intIndex] = value;
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
                    this.Game.Foundations == null || this.Game.Columns == null ||
                    this.CardSet == null || this.Game.Foundation(0) == null ||
                    this.Game.Column(0) == null || this.Game.TheReserve == null ||
                    this.Game.TheDiscard == null)
                {
                    // cards not loaded, exit and don't draw anything
                    return;
                }
                              	    	   	
                // display playing cards
                
                // display foundations
                DrawFoundations(objGraphics);
                
                // display columns
                DrawColumns(objGraphics);
                
                // display reserve
                DrawReserve(objGraphics);
                
                // display discard
                DrawDiscard(objGraphics);
            }
    
            
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
                int intSpace = mc_intMinVerticalSpace;
                if (ClientSize.Height < m_DefaultStandardClientSize.Height)
                {                          
                    int intVSpaceDiff = 
                        mc_intStandardVerticalSpace - 
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
                            mc_intStandardVerticalSpace))) /
                          (double)intVSizeDiff));
                }
                else
                {
                    intSpace = Math.Max(mc_intMinVerticalSpace, 
                        (int)Math.Floor(
                          (double)mc_intStandardVerticalSpace * 
                          (double)ClientSize.Height /
                          (double)m_DefaultStandardClientSize.Height));
                }
                return intSpace;
            }
            
            //-----------------------------------------------------------------//
            
            private int HorizontalEdgeSpace()
            {
                int intSpace = 
                    (int)Math.Floor(
                          ((double)ClientSize.Width - 
                           (double)mc_intNumberOfColumns * (double)this.CardSet.Width - 
                           6.0 * (double)HorizontalSpace())/2.0);
                
                return intSpace;
            }
                
            //-----------------------------------------------------------------//
            
            private int VerticalEdgeSpace()
            {
                int intSpace = VerticalSpace();
                 
                return intSpace;
            }
            
            //-----------------------------------------------------------------//
            
            private int ColumnHeight()
            {
                int intColumnHeight = this.ClientSize.Height - 2 * VerticalEdgeSpace() -
                                      this.CardSet.Height - 3*VerticalSpace();
                
                return intColumnHeight;
            }
            
            //-----------------------------------------------------------------//
            
            private double MinVerticalVisible()
            {
                double dblVisible = 
                    Math.Max((double)mc_intMinVerticalVisible,
                             (double)mc_intStandardVerticalVisible * 
                             (double)ClientSize.Height /
                             (double)m_DefaultStandardClientSize.Height);
                
                return dblVisible;
            }
            
            //-----------------------------------------------------------------//
            
            private double MinHorizontalVisible()
            {
                double dblVisible = 
                    Math.Max((double)mc_intMinHorizontalVisible,
                             (double)mc_intStandardHorizontalVisible * 
                             (double)ClientSize.Width /
                             (double)m_DefaultStandardClientSize.Width);
                
                return dblVisible;
            }
            
            //-----------------------------------------------------------------//
            
            protected override void CalculateLayoutValues()
            {            
                // recalculate layout values
                
                if (this.CardSet == null || this.Game == null || 
                    this.Game.Column(0) == null)
                {
                    // not yet initialised so have to wait to calculate values
                    return;
                }
                
                m_intHorizontalSpace     = HorizontalSpace();
                m_intColumnHeight        = ColumnHeight();
                m_intHorizontalEdgeSpace = HorizontalEdgeSpace();
                m_intVerticalEdgeSpace   = VerticalEdgeSpace();
                CalculateOverlaps();
            }
            
            //-----------------------------------------------------------------//
            
            private void CalculateOverlaps()
            {
                // calculate column overlaps
                
                // face down
                m_dblVerticalVisible = MinVerticalVisible();
                
                // face up
                for (int intIndex=0; intIndex<mc_intNumberOfColumns; intIndex++)
                {
                    CalculateOverlap(intIndex);
                }
                
                // calculate waste overlap
                m_dblHorizontalVisible = MinHorizontalVisible();
            }
            
            //-----------------------------------------------------------------//
               
            private void CalculateOverlap(int intColumn)
            {
                double dblQuarterCardSetHeight = (double)MinVerticalVisible();
                if (this.CardSet != null)
                {
                    dblQuarterCardSetHeight = Math.Max(MinVerticalVisible(),
                                               (double)this.CardSet.Height/4);
                }
                
                if (this.Game.Column(intColumn) == null ||
                   ((SwSt.CardGames.Klondike)this.Game).FaceDownColumn(intColumn) == null)
                {
                    // null column so just set visible to maximum
                    ma_dblVerticalVisible[intColumn] = dblQuarterCardSetHeight;
                    return;
                }
                
                int intFaceDownCount = 
                    ((SwSt.CardGames.Klondike)this.Game).FaceDownColumn(intColumn).Count;
                int intFaceUpCount = 
                    ((SwSt.CardGames.Klondike)this.Game).Column(intColumn).Count;
                
                if (intFaceUpCount < 2 || this.CardSet == null)
                {
                    // no cards to overlap (or unable to determine card height)
                    // so just set visible to maximum
                    ma_dblVerticalVisible[intColumn] = dblQuarterCardSetHeight;
                }
                else
                {
                    // limit visible area to minimum of MinVerticalVisible()
                    // and maximum of quarter card height
                    ma_dblVerticalVisible[intColumn] = 
                        Math.Max((double)MinVerticalVisible(),
                            Math.Min(dblQuarterCardSetHeight,
                                (double)(ColumnHeight() - this.CardSet.Height -
                                MinVerticalVisible() * intFaceDownCount) /
                                (double)(intFaceUpCount - 1))); 
                }
            }
    
            private void DrawColumn(int intColumn, Graphics objGraphics)
            {
                DrawColumn(intColumn, objGraphics, ma_blnColumnExcludeLast[intColumn], 
                    ma_blnColumnExcludeFaceUp[intColumn]);
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawColumn(int intColumn, Graphics objGraphics,
                                    bool blnExcludeLast)
            {
                DrawColumn(intColumn, objGraphics, blnExcludeLast, false);
            }
            
            //-----------------------------------------------------------------//
            
            
            private void DrawColumn(int intColumn, Graphics objGraphics, 
                                    bool blnExcludeLast, bool blnExcludeFaceUp)
            {
                if (!ma_blnColumnChanged[intColumn])
                {
                    return;
                }
                
                // draw column
                int intCardX; int intCardY;
                        
                intCardX = m_intHorizontalEdgeSpace+
                               (m_intHorizontalSpace+this.CardSet.Width)*intColumn;
                 
                // draw background rectangle to clear column
                objGraphics.FillRectangle(this.Background.Brush, intCardX, 
                                          m_intVerticalEdgeSpace,
                                          this.CardSet.Width,
                                          m_intColumnHeight);
                
                int intFaceDownCards =
                    ((SwSt.CardGames.Klondike)this.Game).FaceDownColumn(intColumn).Count;
                
                // draw face down cards
                for (int intCounter=0; intCounter < intFaceDownCards; intCounter++)
                {
                    intCardY = m_intVerticalEdgeSpace + 
                               (int)Math.Floor(m_dblVerticalVisible*
                               intCounter);
                               
                    try
                    {
                        objGraphics.DrawImage(this.CardSet.CardBack.Bitmap, 
                                              intCardX, intCardY);
                    }
                    catch
                    {
                    }
                }
                
                // draw face up cards
                
                int intPosition = 0;        
                
                if (!blnExcludeFaceUp)
                {           
                    foreach (Card card in this.Game.Column(intColumn))
                    {
                        if (card != this.Game.Column(intColumn).Last || !blnExcludeLast)
                        {
                            intCardY = m_intVerticalEdgeSpace + 
                                   (int)(m_dblVerticalVisible * intFaceDownCards +
                                         ma_dblVerticalVisible[intColumn] * intPosition);
                    
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
                }
                
                ma_blnColumnChanged[intColumn] = false;
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawFoundation(int intFoundation, Graphics objGraphics)
            {
                if (!ma_blnFoundationChanged[intFoundation])
                {
                    return;
                }
                
                int intX = m_intHorizontalEdgeSpace+
                           (this.CardSet.Width+m_intHorizontalSpace) *
                           (3+intFoundation);
                int intY = ClientSize.Height - this.CardSet.Height - 
                           m_intVerticalEdgeSpace;
                                   
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
            
            private void DrawReserve(Graphics objGraphics)
            {
                if (!m_blnReserveChanged)
                {
                    return;
                }
                
                Card card = null;
                Bitmap bmpCard = null;
                if (this.Game.TheReserve != null && this.Game.TheReserve.Count > 0)
                {
                    card = this.CardSet.CardBack;
                    if (card != null && card.Bitmap != null)
                    {
                        bmpCard = card.Bitmap;
                    }
                }
                else
                {
                    card = this.CardSet.CardSpace;
                    if (card != null && card.Bitmap != null)
                    {
                        bmpCard = card.Bitmap;
                    }
                }
                if (bmpCard != null)
                {
                    int intX = m_intHorizontalEdgeSpace;
                    int intY = ClientSize.Height - this.CardSet.Height - 
                               m_intVerticalEdgeSpace;
                               
                    try
                    {
                        // draw background to clear
                        objGraphics.FillRectangle(this.Background.Brush, intX, intY,
                                                  this.CardSet.Width,
                                                  this.CardSet.Height);
                        // draw card
                        objGraphics.DrawImage(bmpCard, intX, intY);
                    }
                    catch{}
                }    
                
                m_blnReserveChanged = false;
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawDiscard(Graphics objGraphics)
            {
                DrawDiscard(objGraphics, m_blnDiscardExcludeLast);
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawDiscard(Graphics objGraphics, bool blnExcludeLast)
            {
                if (!m_blnDiscardChanged)
                {
                    return;
                }
                
                Card card = null;
                int intDiscardCount = this.Game.TheDiscard.Count;
    
                if (this.Game.TheDiscard != null && intDiscardCount > 0)
                {
                    int intX = m_intHorizontalEdgeSpace + this.CardSet.Width + 
                               m_intHorizontalSpace;
                                   
                    int intY = ClientSize.Height - this.CardSet.Height - 
                               m_intVerticalEdgeSpace;
    
                    // draw background to clear
                    objGraphics.FillRectangle(this.Background.Brush, intX, intY,
                                              (int)(2*m_dblHorizontalVisible) + 
                                              this.CardSet.Width,
                                              this.CardSet.Height);
                    
                    for (int intCounter=Math.Min(intDiscardCount, 3);
                         intCounter> (blnExcludeLast ? 1 : 0);
                         intCounter --)
                    {
                        card = this.Game.TheDiscard[intDiscardCount-intCounter];
                        
                        if (card != null && card.Bitmap != null)
                        {
                            intX = m_intHorizontalEdgeSpace + this.CardSet.Width + 
                                   m_intHorizontalSpace +
                                   (int)(m_dblHorizontalVisible * 
                                   (Math.Min(intDiscardCount, 3)-intCounter));
                               
                            try
                            {
                                // draw card
                                objGraphics.DrawImage(card.Bitmap, intX, intY);
                            }
                            catch{}
                        }
                    }
                }
                
                m_blnDiscardChanged = false;
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawColumns(Graphics objGraphics)
            {
                int[] a_intColumns = {0,1,2,3,4,5,6};
                DrawColumns(a_intColumns, objGraphics);
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawColumns(int[] a_intColumns, Graphics objGraphics)
            {
                foreach (int intColumn in a_intColumns)
                {
                    DrawColumn(intColumn, objGraphics);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawFoundations(Graphics objGraphics)
            {
                int[] a_intFoundations = {0,1,2,3};
                DrawFoundations(a_intFoundations, objGraphics);
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawFoundations(int[] a_intFoundations, Graphics objGraphics)
            {
                foreach (int intFoundations in a_intFoundations)
                {
                    DrawFoundation(intFoundations, objGraphics);
                }
            }
            
            //-----------------------------------------------------------------//
            
            private void DrawColumnsAndFoundations(int[] a_intColumns, 
                                                int[] a_intFoundations,
                                                Graphics objGraphics)
            {
                foreach (int intColumn in a_intColumns)
                {
                    DrawColumn(intColumn, objGraphics);
                }
                foreach (int intFoundations in a_intFoundations)
                {
                    DrawFoundation(intFoundations, objGraphics);
                }
            }
        }
    }        
}