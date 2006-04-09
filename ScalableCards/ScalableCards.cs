/*
 *************************************************************************
 * ScalableCards.dll: Library to create and manipulate scalable cards.   *
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
 * File:          ScalableCards.cs                                       *
 * Namespace:     SwSt.Cards.Scalable                                    *
 * Last modified: 13 January 2005                                        *
 * Description:   Classes to create and manipulate scalable cards.       *
 *************************************************************************
 */

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Resources;
using System.Reflection;
using SwSt;
using SwSt.Cards;

// ------------------------------------------------------ //

namespace SwSt
{
    namespace Cards
    {
        namespace Scalable
        {    
            // ------------------------------------------------------ //
            
            public class ScalableCard : Card
            {
                private const double m_dblRankWidthProportion = 0.125;
                private const double m_dblRankHeightProportion = 0.100;
                private const double m_dblSuitWidthProportion = 0.180;
                private const double m_dblSuitHeightProportion = 0.120;
                private const double m_dblSmallSuitWidthProportion = 0.103;
                private const double m_dblSmallSuitHeightProportion = 0.070;
                private const double m_dblRoyalWidthProportion = 0.660;
                private const double m_dblRoyalHeightProportion = 0.450;
                private const double m_dblAceWidthProportion = 0.270;
                private const double m_dblAceHeightProportion = 0.180;
                private const double m_dblPipTopProportion = 0.091;
                private const double m_dblEdgeSpaceWidthProportion = 0.023;
                private const double m_dblEdgeSpaceHeightProportion = 0.016;
                
                // ------------------------------------------------------ //
                
                public ScalableCard() : base()
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCard(Bitmap bmp) : base(bmp)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCard(Suit suit, Rank rank) : base(suit, rank)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCard(Suit suit, Rank rank, Bitmap bmp) :
                       base(suit, rank, bmp)
                {
                }
                
                // ------------------------------------------------------ //
                
                ~ScalableCard()
                {
                    this.DisposeBitmap();
                }
                
                // ------------------------------------------------------ //
                
                public override bool Resize(int intNewWidth, int intNewHeight, 
                    bool blnProportional)
                {
                    bool blnResized = false;
                    
                    if (m_bmpUnscaled != null)
                    {
                        if (intNewWidth > 0 && intNewHeight > 0 && 
                            m_bmpUnscaled.Width > 0 && m_bmpUnscaled.Height > 0)
                        {
                            // calculate scale factor
                            Size ScaledSize = CalculateNewSize(intNewWidth, intNewHeight,
                                                               blnProportional);
                            CreateBitmap(ScaledSize);
                            blnResized = true;
                        }
                    }            
                    
                    return blnResized;
                }
                
                // ------------------------------------------------------ //
                
                public void CreateBitmap(Size size)
                {
                    Suit suit = this.Suit;
                    Rank rank = this.Rank;
                    if (rank == Rank.None || suit == Suit.None)
                    {
                        return;
                    }
                    
                    // base card white with black border
                        
                    // create blank bitmap to draw on
                    Bitmap bmpCard = new Bitmap(size.Width, size.Height);
                    // draw white background
                    Graphics objGraphics = Graphics.FromImage(bmpCard);
                    objGraphics.FillRectangle(Brushes.White, 0, 0, size.Width, size.Height);
                    // draw black border                
                    objGraphics.DrawRectangle(new Pen(Color.Black), 0, 0, 
                                              size.Width-1, size.Height-1);
                    bmpCard.SetPixel(1, 1, Color.Black);
                    bmpCard.SetPixel(size.Width-2, 1, Color.Black);
                    bmpCard.SetPixel(1, size.Height-2, Color.Black);
                    bmpCard.SetPixel(size.Width-2, size.Height-2, Color.Black);
                    // set corners transparent
                    bmpCard.SetPixel(0, 0, Color.Transparent);
                    bmpCard.SetPixel(size.Width-1, 0, Color.Transparent);
                    bmpCard.SetPixel(0, size.Height-1, Color.Transparent);
                    bmpCard.SetPixel(size.Width-1, size.Height-1, Color.Transparent);
                    
                    int intEdgeSpaceWidth  = Math.Max(2, 
                        (int)(m_dblEdgeSpaceWidthProportion * (double)size.Width));
                    int intEdgeSpaceHeight = Math.Max(2, 
                        (int)(m_dblEdgeSpaceHeightProportion * (double)size.Height));
                    int intPipTop = Math.Max(2,
                        (int)(m_dblPipTopProportion * (double)size.Height));
                    
                    int intRankWidth  = 
                        (int)(m_dblRankWidthProportion * (double)(size.Width));
                    int intRankHeight = 
                        (int)(m_dblRankHeightProportion * (double)(size.Height));
                    int intSuitWidth  = 
                        (int)(m_dblSuitWidthProportion * (double)(size.Width));
                    int intSuitHeight = 
                        (int)(m_dblSuitHeightProportion * (double)(size.Height));
                    int intSmallSuitWidth  = 
                        (int)(m_dblSmallSuitWidthProportion * (double)(size.Width));
                    int intSmallSuitHeight = 
                        (int)(m_dblSmallSuitHeightProportion * (double)(size.Height));
                    int intAceWidth  = 
                        (int)(m_dblAceWidthProportion * (double)(size.Width));
                    int intAceHeight = 
                        (int)(m_dblAceHeightProportion * (double)(size.Height));
                    
                    Bitmap bmpSmallSuit = CardGraphics.SuitBitmap(
                        suit, intSmallSuitWidth, intSmallSuitHeight);
                    if (bmpSmallSuit == null)
                    {
                        bmpSmallSuit = CardGraphics.SmallestSuitBitmap(suit);
                    }
                    Bitmap bmpSuit = CardGraphics.SuitBitmap(
                        suit, intSuitWidth, intSuitHeight);
                    if (bmpSuit == null)
                    {
                        bmpSuit = CardGraphics.SmallestSuitBitmap(suit);
                    }
                    Bitmap bmpRank = CardGraphics.RankBitmap(
                        rank, suit, intRankWidth, intRankHeight);
                    if (bmpRank == null)
                    {
                        bmpRank = CardGraphics.SmallestRankBitmap(rank, suit);
                    }
                    
                    int intRoyalWidth  = 
                        Math.Min(bmpCard.Width - 2*(intEdgeSpaceWidth +
                            Math.Max(bmpRank.Width, bmpSmallSuit.Width))- 2*intEdgeSpaceWidth,
                            (int)(m_dblRoyalWidthProportion * (double)(size.Width)));
                    int intRoyalHeight = 
                        Math.Min(bmpCard.Height - 2*intEdgeSpaceHeight,
                                 (int)(m_dblRoyalHeightProportion * (double)(size.Height)));
                    
                    // draw suit and rank symbols
                    if (rank == Rank.King)
                    {
                        // draw King centre images
                        Bitmap bmpKing = CardGraphics.KingBitmap(
                            suit, intRoyalWidth, intRoyalHeight);
                        if (bmpKing == null ||
                            bmpCard.Width < 2*(intEdgeSpaceWidth +
                                                Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             2*intEdgeSpaceWidth + bmpKing.Width ||
                            bmpCard.Height < 2*intEdgeSpaceHeight + 
                                               Math.Max(bmpKing.Height,
                                                        2*(1+bmpRank.Height+bmpSmallSuit.Height)+
                                                        intEdgeSpaceHeight))
                        { 
                            bmpKing = CardGraphics.SmallestSuitBitmap(suit);
                        }
                        if (bmpKing != null)
                        {
                            if (bmpCard.Width >= 2*(intEdgeSpaceWidth +
                                                Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             2*intEdgeSpaceWidth + bmpKing.Width &&
                                bmpCard.Height >= 2*intEdgeSpaceHeight + 
                                               Math.Max(bmpKing.Height,
                                                        2*(1+bmpRank.Height+bmpSmallSuit.Height)+
                                                        intEdgeSpaceHeight))
                            {
                                int intXOffset = 
                                    (int)((double)bmpCard.Width/2-(double)bmpKing.Width/2);
                                int intYOffset = 
                                    (int)((double)bmpCard.Height/2-(double)bmpKing.Height/2);
                                objGraphics.DrawImage(bmpKing,
                                    intXOffset,
                                    intYOffset);
                            }
                        }
                    }
                    else if (rank == Rank.Queen)
                    {
                        // draw Queen centre images
                        Bitmap bmpQueen = CardGraphics.QueenBitmap(
                            suit, intRoyalWidth, intRoyalHeight);
                        if (bmpQueen == null ||
                            bmpCard.Width < 2*(intEdgeSpaceWidth +
                                                Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             2*intEdgeSpaceWidth + bmpQueen.Width ||
                            bmpCard.Height < 2*intEdgeSpaceHeight + 
                                               Math.Max(bmpQueen.Height,
                                                        2*(1+bmpRank.Height+bmpSmallSuit.Height)+
                                                        intEdgeSpaceHeight))
                        {
                            bmpQueen = CardGraphics.SmallestSuitBitmap(suit);
                        }
                        if (bmpQueen != null)
                        {
                            if (bmpCard.Width >= 2*(intEdgeSpaceWidth +
                                                Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             2*intEdgeSpaceWidth + bmpQueen.Width &&
                                bmpCard.Height >= 2*intEdgeSpaceHeight + 
                                               Math.Max(bmpQueen.Height,
                                                        2*(1+bmpRank.Height+bmpSmallSuit.Height)+
                                                        intEdgeSpaceHeight))
                            {
                                int intXOffset = 
                                    (int)((double)bmpCard.Width/2-(double)bmpQueen.Width/2);
                                int intYOffset = 
                                    (int)((double)bmpCard.Height/2-(double)bmpQueen.Height/2);
                                objGraphics.DrawImage(bmpQueen,
                                    intXOffset,
                                    intYOffset);
                            }
                        }
                    }
                    else if (rank == Rank.Jack)
                    {
                        // draw Jack centre images
                        Bitmap bmpJack = CardGraphics.JackBitmap(
                            suit, intRoyalWidth, intRoyalHeight);
                        if (bmpJack == null ||
                            bmpCard.Width < 2*(intEdgeSpaceWidth +
                                                Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             2*intEdgeSpaceWidth + bmpJack.Width ||
                            bmpCard.Height < 2*intEdgeSpaceHeight + 
                                               Math.Max(bmpJack.Height,
                                                        2*(1+bmpRank.Height+bmpSmallSuit.Height)+
                                                        intEdgeSpaceHeight))
                        {
                            bmpJack = CardGraphics.SmallestSuitBitmap(suit);
                        }
                        if (bmpJack != null)
                        {
                            if (bmpCard.Width >= 2*(intEdgeSpaceWidth +
                                                Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             2*intEdgeSpaceWidth + bmpJack.Width &&
                                bmpCard.Height >= 2*intEdgeSpaceHeight + 
                                               Math.Max(bmpJack.Height,
                                                        2*(1+bmpRank.Height+bmpSmallSuit.Height)+
                                                        intEdgeSpaceHeight))
                            {
                                int intXOffset = 
                                    (int)((double)bmpCard.Width/2-(double)bmpJack.Width/2);
                                int intYOffset = 
                                    (int)((double)bmpCard.Height/2-(double)bmpJack.Height/2);
                                objGraphics.DrawImage(bmpJack,
                                    intXOffset,
                                    intYOffset);
                            }
                        }
                    }
                    else if (rank == Rank.Ace)
                    {
                        // draw Ace centre images
                        
                        Bitmap bmpAce = CardGraphics.SuitBitmap(
                            suit, intAceWidth, intAceHeight);
                        if (bmpAce == null ||
                            bmpCard.Width < 2*(intEdgeSpaceWidth +
                                                Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             2*intEdgeSpaceWidth + bmpAce.Width ||
                            bmpCard.Height < 2*intEdgeSpaceHeight + 
                                               Math.Max(bmpAce.Height,
                                                        2*(1+bmpRank.Height+bmpSmallSuit.Height)+
                                                        intEdgeSpaceHeight))
                        {
                            bmpAce = CardGraphics.SmallestSuitBitmap(suit);
                        }
                    
                        if (bmpAce != null)
                        {
                            if (bmpCard.Width >= 2*(intEdgeSpaceWidth +
                                                Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             2*intEdgeSpaceWidth + bmpAce.Width &&
                                bmpCard.Height >= 2*intEdgeSpaceHeight + 
                                               Math.Max(bmpAce.Height,
                                                        2*(1+bmpRank.Height+bmpSmallSuit.Height)+
                                                        intEdgeSpaceHeight))
                            {
                                int intXOffset = 
                                    (int)((double)bmpCard.Width/2-(double)bmpAce.Width/2);
                                int intYOffset = 
                                    (int)((double)bmpCard.Height/2-(double)bmpAce.Height/2);
                                
                                objGraphics.DrawImage(bmpAce,
                                    intXOffset, intYOffset);
                            }
                        }
                    }    
                    else
                    {
                        if (bmpSmallSuit != null && bmpSuit != null && bmpRank != null)
                        {
                            float fltHSpacing;
                            float fltVSpacing;
                        
                            int intNumberOfRows = CardGraphics.Rows(rank);
                        
                            if (size.Width-2*(intEdgeSpaceWidth+
                                Math.Max(bmpSmallSuit.Width, bmpRank.Width)) <
                                bmpSuit.Width*3+2 ||
                                size.Height-2*intEdgeSpaceHeight <
                                bmpSuit.Width*7+6)
                            {
                                bmpSuit = CardGraphics.SmallestSuitBitmap(suit);
                            }
                        
                            int intHSpace = size.Width - 
                                            2*(intEdgeSpaceWidth +
                                            Math.Max(bmpSmallSuit.Width, bmpRank.Width))
                                            - 2*intEdgeSpaceWidth - 3*bmpSuit.Width;
                            fltHSpacing = bmpSuit.Width + (float)intHSpace / 2;
                            
                            int intVSpace = size.Height - 4*intEdgeSpaceHeight - 2*intPipTop -
                                            intNumberOfRows*bmpSuit.Height;
                            fltVSpacing = bmpSuit.Height + (float)intVSpace / (intNumberOfRows-1);
                        
                            if (bmpCard.Width < 2*(intEdgeSpaceWidth +
                                                   Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                                   2*intEdgeSpaceWidth + bmpSuit.Width ||
                                bmpCard.Height < 2*intEdgeSpaceHeight + 
                                                   Math.Max(bmpSuit.Height,
                                                            2*(1+bmpRank.Height+bmpSmallSuit.Height)+
                                                             intEdgeSpaceHeight))
                            {
                                // too small to show any pips
                            }                
                            else if (fltHSpacing < 1 || fltVSpacing < 1 ||
                                size.Width-2*intEdgeSpaceWidth-2*(intEdgeSpaceWidth+
                                Math.Max(bmpSmallSuit.Width, bmpRank.Width)) <
                                bmpSuit.Width*3+2 ||
                                size.Height-2*intEdgeSpaceHeight <
                                bmpSuit.Height*7+6 )
                            {
                                // can only draw centre pip
                            
                                int intXOffset = 
                                    (int)((double)bmpCard.Width/2-(double)bmpSuit.Width/2);
                                int intYOffset = 
                                    (int)((double)bmpCard.Height/2-(double)bmpSuit.Height/2);
                                
                                objGraphics.DrawImage(bmpSuit,
                                    intXOffset, intYOffset);
                            }
                            else
                            {
                                // draw all pips
                                
                                bool blnFlipped = false;
                        
                                for (int intPointNumber=0; 
                                     intPointNumber<CardGraphics.Points(rank); 
                                     intPointNumber++)
                                {
                                    int intXCoord;
                                    int intYCoord;
                                    if (intPointNumber >= CardGraphics.FlipPoint(rank) && !blnFlipped)
                                    {
                                        // rotate 180 degrees
                                        objGraphics.TranslateTransform(bmpCard.Width/2, bmpCard.Height/2);
                                        objGraphics.RotateTransform(180);
                                        objGraphics.TranslateTransform(-bmpCard.Width/2, -bmpCard.Height/2);
                        
                                        blnFlipped = true;
                                    }
                                    
                                    // calculate coordinates of point
                                    
                                    int intRow = CardGraphics.RankPointRow(rank, intPointNumber);
                                    if (blnFlipped)
                                    {
                                        intRow = intNumberOfRows-intRow+1;
                                    }
                                    
                                    intYCoord = (int)(intEdgeSpaceHeight + intPipTop + fltVSpacing * 
                                    ((double)intRow-1));
                                            
                                    if (rank == Rank.Deuce || 
                                        rank == Rank.Three)
                                    {
                                        intXCoord = (bmpCard.Width-bmpSuit.Width)/2;
                                    }
                                    else
                                    {
                                        if ((CardGraphics.RankPointPosition(rank, intPointNumber) % 2) 
                                             == 0)
                                        {
                                            if (intPointNumber==12)
                                            {
                                                // third column
                                                intXCoord = 
                                                    Math.Max(bmpSmallSuit.Width, bmpRank.Width) +
                                                    (int)(2*intEdgeSpaceWidth + 2*fltHSpacing);
                                            }
                                            else if (
                                              CardGraphics.RankPointPosition(rank, intPointNumber) !=
                                              CardGraphics.RankPointPosition(rank, intPointNumber+1))
                                            {
                                                // third column
                                                intXCoord = 
                                                    Math.Max(bmpSmallSuit.Width, bmpRank.Width) + 
                                                    (int)(2*intEdgeSpaceWidth + 2*fltHSpacing);
                                            }
                                            else
                                            {
                                                // first column
                                                intXCoord = 
                                                    Math.Max(bmpSmallSuit.Width, bmpRank.Width) +
                                                    2*intEdgeSpaceWidth;
                                            }
                                        }
                                        else
                                        {
                                            // second column
                                            intXCoord = 
                                                Math.Max(bmpSmallSuit.Width, bmpRank.Width) +
                                                (int)(2*intEdgeSpaceWidth + fltHSpacing);
                                        }
                                    }
                                    
                                    
                                    objGraphics.DrawImage(bmpSuit, intXCoord, intYCoord);
                                } // next point*/
                                
                                if (blnFlipped)
                                {
                                    // rotate back
                                    objGraphics.TranslateTransform(bmpCard.Width/2, bmpCard.Height/2);
                                    objGraphics.RotateTransform(180);
                                    objGraphics.TranslateTransform(-bmpCard.Width/2, -bmpCard.Height/2);
                                    blnFlipped = false;
                                }
                                    
                                
                            } // end if size too small for 'pips'
                        }
                    }
                    
                    if (bmpSmallSuit != null && bmpRank != null)
                    {
                        int intRankOffset = Math.Max((bmpSmallSuit.Width-bmpRank.Width)/2,0);
                        int intSuitOffset = Math.Max((bmpRank.Width-bmpSmallSuit.Width)/2,0);
                        
                        // top left
                        objGraphics.DrawImage(bmpRank,
                                              intEdgeSpaceWidth+intRankOffset,
                                              intEdgeSpaceHeight);
                        objGraphics.DrawImage(bmpSmallSuit,
                                              intEdgeSpaceWidth+intSuitOffset,
                                              1+intEdgeSpaceHeight+bmpRank.Height);
                                   
                        if (bmpCard.Width >= 2*(intEdgeSpaceWidth+
                                             Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             intEdgeSpaceWidth &&
                            bmpCard.Height >= 2*(intEdgeSpaceHeight+1+bmpRank.Height+bmpSuit.Height)+
                                              intEdgeSpaceWidth)
                            
                        {                    
                            // top right           
                            objGraphics.DrawImage(bmpRank,
                                                  bmpCard.Width -
                                                  Math.Max(bmpRank.Width, bmpSmallSuit.Width) -
                                                  intEdgeSpaceWidth+intRankOffset,
                                                  intEdgeSpaceHeight);
                            objGraphics.DrawImage(bmpSmallSuit,
                                                  bmpCard.Width -
                                                  Math.Max(bmpRank.Width, bmpSmallSuit.Width) -
                                                  intEdgeSpaceWidth+intSuitOffset,
                                                  1+intEdgeSpaceHeight+bmpRank.Height);
                        }
                        
                        // rotate 180 degrees for bottom symbols
                        objGraphics.TranslateTransform(bmpCard.Width/2, bmpCard.Height/2);
                        objGraphics.RotateTransform(180);
                        objGraphics.TranslateTransform(-bmpCard.Width/2, -bmpCard.Height/2);
                        
                        // bottom right
                        objGraphics.DrawImage(bmpRank,
                                              intEdgeSpaceWidth+intRankOffset,
                                              intEdgeSpaceHeight);
                        objGraphics.DrawImage(bmpSmallSuit,
                                              intEdgeSpaceWidth+intSuitOffset,
                                              1+intEdgeSpaceHeight+bmpRank.Height);
                                   
                        if (bmpCard.Width >= 2*(intEdgeSpaceWidth+
                                             Math.Max(bmpRank.Width, bmpSmallSuit.Width))+
                                             intEdgeSpaceWidth &&
                            bmpCard.Height >= 2*(intEdgeSpaceHeight+1+bmpRank.Height+bmpSuit.Height)+
                                              intEdgeSpaceWidth)
                            
                        {                    
                            // bottom left           
                            objGraphics.DrawImage(bmpRank,
                                                  bmpCard.Width -
                                                  Math.Max(bmpRank.Width, bmpSmallSuit.Width) -
                                                  intEdgeSpaceWidth+intRankOffset,
                                                  intEdgeSpaceHeight);
                            objGraphics.DrawImage(bmpSmallSuit,
                                                  bmpCard.Width -
                                                  Math.Max(bmpRank.Width, bmpSmallSuit.Width) -
                                                  intEdgeSpaceWidth+intSuitOffset,
                                                  1+intEdgeSpaceHeight+bmpRank.Height);
                           
                        }
                        // restore orientation
                        objGraphics.TranslateTransform(bmpCard.Width/2, bmpCard.Height/2);
                        objGraphics.RotateTransform(180);
                        objGraphics.TranslateTransform(-bmpCard.Width/2, -bmpCard.Height/2);
                    }
                    
                    objGraphics.Dispose();
                    
                    m_bmp = bmpCard;
                    bmpCard = null;        
                }
            }
            
            // ------------------------------------------------------ //
            
            public class ScalableStackCard : StackCard
            {
                private const double mc_dblHEdgeProp    = 0.136137;
                private const double mc_dblVEdgeProp    = 0.184616;
                private const double mc_dblHSpacingProp = 0.090910;
                private const double mc_dblVSpacingProp = 0.061539;
                
                // ------------------------------------------------------ //
                
                public ScalableStackCard() : base()
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableStackCard(Bitmap bmp) : base(bmp)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableStackCard(int intStackNumber)
                {
                    m_intStackNumber = intStackNumber;
                }
                
                // ------------------------------------------------------ //
                
                public ScalableStackCard(Bitmap bmp, int intStackNumber) : this(bmp)
                {
                    m_intStackNumber = intStackNumber;
                }
                
                // ------------------------------------------------------ //
                
                public ScalableStackCard(Card card) : base(card)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableStackCard(StackCard card) : this((Card)card)
                {
                    m_intStackNumber = card.StackNumber;
                }
                
                // ------------------------------------------------------ //
                
                public ScalableStackCard(ScalableStackCard card) : this((StackCard)card)
                {
                }
                
                // ------------------------------------------------------ //
                
                ~ScalableStackCard()
                {
                    this.DisposeBitmap();
                }
                
                // ------------------------------------------------------ //
                
                public override bool Resize(int intNewWidth, int intNewHeight, bool blnProportional)
                {
                    bool blnResized = false;
                    
                    if (m_bmpUnscaled != null)
                    {
                        if (intNewWidth > 0 && intNewHeight > 0 && 
                            m_bmpUnscaled.Width > 0 && m_bmpUnscaled.Height > 0)
                        {
                            // calculate scale factor
                            Size ScaledSize = CalculateNewSize(intNewWidth, intNewHeight,
                                                               blnProportional);
                            CreateBitmap(ScaledSize);
                            blnResized = true;
                        }
                    }            
                    
                    return blnResized;
                }
                
                // ------------------------------------------------------ //
                
                public void CreateBitmap(Size size)
                {
                    // white with black border
                    // create blank bitmap to draw on
                    // NB. setting PixelFormat to PixelFormat.Format16bppArgb1555
                    // causes an "Out of Memory" exception, no exception if don't
                    // set PixelFormat.  Bug in C#/.NET?
                    Bitmap bmpCard = new Bitmap(size.Width, size.Height);
                                                
                    // draw transparent background
                    Graphics objGraphics = Graphics.FromImage(bmpCard);
                    objGraphics.FillRectangle(Brushes.Transparent, 
                                              0, 0, size.Width, size.Height);
                    
                    
                    // draw black border                
                    objGraphics.DrawRectangle(new Pen(Color.Black), 0, 0, 
                                              size.Width-1, size.Height-1);
                    bmpCard.SetPixel(1, 1, Color.Black);
                    bmpCard.SetPixel(size.Width-2, 1, Color.Black);
                    bmpCard.SetPixel(1, size.Height-2, Color.Black);
                    bmpCard.SetPixel(size.Width-2, size.Height-2, Color.Black);
                    // set corners transparent
                    bmpCard.SetPixel(0, 0, Color.Transparent);
                    bmpCard.SetPixel(size.Width-1, 0, Color.Transparent);
                    bmpCard.SetPixel(0, size.Height-1, Color.Transparent);
                    bmpCard.SetPixel(size.Width-1, size.Height-1, Color.Transparent);
                    
                    objGraphics.Dispose();
                    
                    m_bmp = bmpCard;
                    bmpCard = null;
                }
            }
            
            // ------------------------------------------------------ //
            
            public class ScalableCardBack : CardBack
            {
                // ------------------------------------------------------ //
                
                public ScalableCardBack() : base()
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCardBack(Bitmap bmp) : base(bmp)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCardBack(Card card) : base(card)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCardBack(CardBack card) : base(card)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCardBack(ScalableCardBack card) : base((CardBack)card)
                {
                }
                
                // ------------------------------------------------------ //
                
                ~ScalableCardBack()
                {
                    this.DisposeBitmap();
                }
                
                // ------------------------------------------------------ //
                
                public override bool Resize(int intNewWidth, int intNewHeight, bool blnProportional)
                {
                    bool blnResized = false;
                    
                    if (m_bmpUnscaled != null)
                    {
                        if (intNewWidth > 0 && intNewHeight > 0 && 
                            m_bmpUnscaled.Width > 0 && m_bmpUnscaled.Height > 0)
                        {
                            // calculate scale factor
                            Size ScaledSize = CalculateNewSize(intNewWidth, intNewHeight,
                                                               blnProportional);
                            CreateBitmap(ScaledSize);
                            blnResized = true;
                        }
                    }            
                    
                    return blnResized;
                }
                
                // ------------------------------------------------------ //
                
                public void CreateBitmap(Size size)
                {
                    // all black
                    // create blank bitmap to draw on
                    // NB. setting PixelFormat to PixelFormat.Format16bppArgb1555
                    // causes an "Out of Memory" exception, no exception if don't
                    // set PixelFormat.  Bug in C#/.NET?
                    Bitmap bmpCard = new Bitmap(size.Width, size.Height);//,
                                                //PixelFormat.Format16bppArgb1555);
                                                
                    // draw white background
                    Graphics objGraphics = Graphics.FromImage(bmpCard);
                    objGraphics.FillRectangle(Brushes.White, 0, 0, size.Width, size.Height);
                    
                    
                    // draw line pattern
                    int intLength = size.Width + size.Height;
                                        
                    int intStep = Math.Max(1, 
                                           (int)Math.Floor(4.0*size.Height/130.0));
                    
                    int intLineWidth = 
                        Math.Max(1, 
                                 (int)Math.Floor(1.5*size.Height/130.0));
                    
                    Pen redPen = new Pen(Color.FromArgb(192,0,0));
                    for (int intWidth = 0; intWidth<intLineWidth; intWidth++)
                    {
                        for (int intPos=-intLength; intPos<=intLength; intPos+=intStep)
                        {
                            objGraphics.DrawLine(redPen, 
                                                 intPos+intWidth, 0, 
                                                 intPos+intWidth-intLength, intLength);
                            objGraphics.DrawLine(redPen, 
                                                 intPos+intWidth, 0, 
                                                 intPos+intWidth+intLength, intLength);
                        }
                    }
                    
                    // draw black border                
                    objGraphics.DrawRectangle(new Pen(Color.Black), 0, 0, 
                                              size.Width-1, size.Height-1);
                    bmpCard.SetPixel(1, 1, Color.Black);
                    bmpCard.SetPixel(size.Width-2, 1, Color.Black);
                    bmpCard.SetPixel(1, size.Height-2, Color.Black);
                    bmpCard.SetPixel(size.Width-2, size.Height-2, Color.Black);
                    // set corners transparent
                    bmpCard.SetPixel(0, 0, Color.Transparent);
                    bmpCard.SetPixel(size.Width-1, 0, Color.Transparent);
                    bmpCard.SetPixel(0, size.Height-1, Color.Transparent);
                    bmpCard.SetPixel(size.Width-1, size.Height-1, Color.Transparent);
                    
                    this.Bitmap = bmpCard;
                    bmpCard = null;
                }
            }
            
            // ------------------------------------------------------ //
            
            public class ScalableCardSpace : CardSpace
            {
                // ------------------------------------------------------ //
                
                public ScalableCardSpace() : base()
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCardSpace(Bitmap bmp) : base(bmp)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCardSpace(Card card) : base(card)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCardSpace(CardSpace card) : base(card)
                {
                }
                
                // ------------------------------------------------------ //
                
                public ScalableCardSpace(ScalableCardSpace card) : base((CardSpace)card)
                {
                }
                
                // ------------------------------------------------------ //
                
                ~ScalableCardSpace()
                {
                    this.DisposeBitmap();
                }
                
                // ------------------------------------------------------ //
                
                public override bool Resize(int intNewWidth, int intNewHeight, 
                                            bool blnProportional)
                {
                    bool blnResized = false;
                    
                    if (m_bmpUnscaled != null)
                    {
                        if (intNewWidth > 0 && intNewHeight > 0 && 
                            m_bmpUnscaled.Width > 0 && m_bmpUnscaled.Height > 0)
                        {
                            // calculate scale factor
                            Size ScaledSize = CalculateNewSize(intNewWidth, intNewHeight,
                                                               blnProportional);
                            CreateBitmap(ScaledSize);
                            blnResized = true;
                        }
                    }            
                    
                    return blnResized;
                }
                
                // ------------------------------------------------------ //
                
                public void CreateBitmap(Size size)
                {
                    // all black
                    // create blank bitmap to draw on
                    // NB. setting PixelFormat to PixelFormat.Format16bppArgb1555
                    // causes an "Out of Memory" exception, no exception if don't
                    // set PixelFormat.  Bug in C#/.NET?
                    Bitmap bmpCard = new Bitmap(size.Width, size.Height);//,
                                                //PixelFormat.Format16bppArgb1555);
                                                
                    // draw transparent background
                    Graphics objGraphics = Graphics.FromImage(bmpCard);
                    objGraphics.FillRectangle(Brushes.Transparent, 0, 0, 
                                              size.Width, size.Height);
                    
                    // draw black border                
                    objGraphics.DrawRectangle(new Pen(Color.Black), 0, 0, 
                                              size.Width-1, size.Height-1);
                    bmpCard.SetPixel(1, 1, Color.Black);
                    bmpCard.SetPixel(size.Width-2, 1, Color.Black);
                    bmpCard.SetPixel(1, size.Height-2, Color.Black);
                    bmpCard.SetPixel(size.Width-2, size.Height-2, Color.Black);
                    // set corners transparent
                    bmpCard.SetPixel(0, 0, Color.Transparent);
                    bmpCard.SetPixel(size.Width-1, 0, Color.Transparent);
                    bmpCard.SetPixel(0, size.Height-1, Color.Transparent);
                    bmpCard.SetPixel(size.Width-1, size.Height-1, Color.Transparent);
                    
                    m_bmp = bmpCard;
                    bmpCard = null;
                }
            }
            
            // ------------------------------------------------------ //
            
            public class ScalableCardSet : Cards.CardSet
            { 
                private const int                   mc_intStandardWidth     =  88;
                private const int                   mc_intStandardHeight    = 130;
                private const int                   mc_intStandardCardCount =  57;
                // absolute minimum width
                private const int                   mc_intMinWidth          =  28;
                // absolute minimum height
                private const int                   mc_intMinHeight         = 42;
                // min width for large suit symbols
                private const int                   mc_intMinWidthLarge     =  76;
                // min height for large suit symbols
                private const int                   mc_intMinHeightLarge    =  84;
                // min width for small suit symbols
                private const int                   mc_intMinWidthSmall     =  66;
                // min height for small suit symbols
                private const int                   mc_intMinHeightSmall    =  64; 
              
                // ------------------------------------------------------ //
              
                public ScalableCardSet()
                { 
                    m_AllCards     = new CardCollection();
                    m_PlayingCards = new CardCollection(); 
                    m_StackCards   = new CardCollection();
                    m_CardBack     = new ScalableCardBack();
                    m_CardSpace    = new ScalableCardSpace();
                    
                    for (int intSuit=0; intSuit<=4; intSuit++)
                    {
                        for (int intRank=1; intRank<=13; intRank++)
                        {
                            ScalableCard card = new ScalableCard((Suit)intSuit, (Rank)intRank);
                            m_AllCards.Add(card);
                            m_PlayingCards.Add(card);
                        }
                    }
                    // stack/foundation cards, one per suit
                    for (int intStackNumber=0; intStackNumber<4; intStackNumber++)
                    {
                        ScalableStackCard card = new ScalableStackCard(intStackNumber);
                        m_StackCards.Add(card);
                        m_AllCards.Add(card);
                    }
                    
                    m_AllCards.Add(m_CardBack);
                    m_AllCards.Add(m_CardSpace);       
                }
                
                // ------------------------------------------------------ //

                public override string CardSetID
                {
                    get
                    {
                        return "SwSt.Cards.Scalable.ScalableCardSet";
                    }
                }
            
                // ------------------------------------------------------ //

                public override int UnscaledWidth
                {
                    get
                    {
                        return mc_intStandardWidth;
                    }
                }
                
                // ------------------------------------------------------ //
                
                public override int UnscaledHeight
                {
                    get
                    {
                        return mc_intStandardHeight;
                    }
                }
                
                // ------------------------------------------------------ //

                public override int MinWidth
                {
                    get
                    {
                        return mc_intMinWidth;
                    }
                }
                
                // ------------------------------------------------------ //
                
                public override int MinHeight
                {
                    get
                    {
                        return mc_intMinHeight;
                    }
                }
                
                // ------------------------------------------------------ //
                
                public override bool IsSelfContained
                {
                    get
                    {
                        return true;
                    }
                }
                
                // ------------------------------------------------------ //
                      
                public override bool UseInternal()
                {
                    return Create(mc_intStandardWidth, mc_intStandardHeight);
                }
                
                // ------------------------------------------------------ //
                        
                public bool Create()
                {
                    return Create(mc_intStandardWidth, mc_intStandardHeight);
                }
                
                // ------------------------------------------------------ //
                
                public bool Create(int intMaxWidth, int intMaxHeight)
                {
                    m_intWidth  = intMaxWidth > mc_intMinWidth 
                                  ? intMaxWidth : mc_intMinWidth;
                    // width must be even
                    m_intWidth += m_intWidth % 2 == 0 ? 0 : 1;
                    m_intHeight = intMaxHeight > mc_intMinHeight 
                                  ? intMaxHeight : mc_intMinHeight;
                                  
                    // create card back
                    ((ScalableCardBack)this.CardBack).CreateBitmap(
                        new Size(m_intWidth, m_intHeight));
                    this.CardBack.FixSize();
                                     
                    // create card space
                    ((ScalableCardSpace)this.CardSpace).CreateBitmap(
                        new Size(m_intWidth, m_intHeight));
                    this.CardSpace.FixSize();
                                     
                    // playing cards
                    for (int intSuit = 0; intSuit<4; intSuit++)
                    {
                        for (int intRank = 1; intRank<=13; intRank++)
                        {                
                            Suit suit = (Suit)intSuit;
                            Rank rank = (Rank)intRank;
                            
                            ((ScalableCard)this.Card(suit, rank)).CreateBitmap(
                                new Size(m_intWidth, m_intHeight));
                            this.Card(suit, rank).FixSize();
                             
                        }  // next rank
                        
                    }  // next suit
                    
                    // stack cards
                    for (int intCardNumber=0; intCardNumber<4; intCardNumber++)
                    {
                        // create card
                        ((ScalableStackCard)this.StackCard(intCardNumber)).
                            CreateBitmap(
                            new Size(m_intWidth, m_intHeight));
                        this.StackCard(intCardNumber).FixSize();             
                    }
                    
                    return true;
                }
                
                // ------------------------------------------------------ //
                
                public override bool Resize
                    (int intNewWidth, int intNewHeight, bool blnProportional,
                     bool blnScaleUp, bool blnScaleDown)
                {
                    return Resize(intNewWidth, intNewHeight, blnProportional);
                }
                
                public override bool Resize(int intNewWidth, int intNewHeight, bool blnProportional)
                {
                    bool blnResized = false;
                    
                    foreach (ScalableCard card in m_PlayingCards)
                    {
                        blnResized |= card.Resize(intNewWidth, intNewHeight, blnProportional);
                    }
                    foreach (ScalableStackCard card in m_StackCards)
                    {
                        blnResized |= card.Resize(intNewWidth, intNewHeight, blnProportional);
                    }
                    blnResized |= m_CardBack.Resize(intNewWidth, intNewHeight, blnProportional);
                    blnResized |= m_CardSpace.Resize(intNewWidth, intNewHeight, blnProportional);
                    
                    m_intWidth  = m_AllCards[0].Bitmap.Width;
                    m_intHeight = m_AllCards[0].Bitmap.Height;
                    
                    return blnResized;
                }
                
                // Module copyright  
                public override string Copyright
                {
                    get
                    {
                        return "Copyright (C) 2005 Trevor Barnett";
                    }
                }
                
                // Module author's contact email address
                public override string ContactEmail
                {
                    get
                    {
                        return "swst@e381.net";
                    }
                        
                }
                
                // Module author's website
                public override string Website
                {
                    get
                    {
                        return "www.e381.net";
                    }
                }
                            
                // Cardset Module name 
                public override string ModuleName
                {
                    get
                    {
                        return "Scalable Cardset Module for Patience";
                    }
                }
            }
            
            // ------------------------------------------------------ //
            
            public struct DrawingInstructions
            {
                public int  SmallPipSize;
                public int  PipSize;
                public int  RankSize;
                public int  RoyalSize;
                public bool NoPips;
                public bool OnePip;
                public bool TLLabel;
                public bool TRLabel;
                public bool BLLabel;
                public bool BRLabel;
            }
            
            // ------------------------------------------------------ //
            
            public struct LayoutValues
            {
                public int   BorderWidth;
                public int   MinSmallPipSpacing;
                public Point RankOffset;
                public Point SmallPipOffset;
                public Point PipAreaOffset;
                public Point MinPipOffset;
            }        
            
            // ------------------------------------------------------ //
                    
            public class CardGraphics
            {   
                public struct SizeSet
                {
                    public int SmallPip;
                    public int Pip;
                    public int Rank;
                }
                
                private static bool      m_blnResourcesLoaded   = false;
                private static ArrayList[]  ma_bmpKingImages       = new ArrayList[4];
                private static ArrayList[]  ma_bmpQueenImages      = new ArrayList[4];
                private static ArrayList[]  ma_bmpJackImages       = new ArrayList[4];
                private static ArrayList[]  ma_bmpSuitImages       = new ArrayList[4];
                private static ArrayList[]  ma_bmpRedRankImages    = new ArrayList[14];
                private static ArrayList[]  ma_bmpBlackRankImages  = new ArrayList[14];
                
                // ------------------------------------------------------ //
                
                private static Bitmap GetBitmapFromResource(ResourceManager rm, string strName)
                {
                    Bitmap bmpTemp = (Bitmap)rm.GetObject(strName);
                    Bitmap bmpRes = new Bitmap(bmpTemp, bmpTemp.Width, bmpTemp.Height);
                    return bmpRes;
                }
                
                private static void LoadResources()
                {
                    // get images from embedded .resources
                    
                    if (m_blnResourcesLoaded)
                    {
                        // resources already loaded
                        return;
                    }
                        
                    string[] a_strSuits = {"Clubs", "Diamonds", "Hearts", "Spades"};
                    
                    try
                    {
                        ResourceManager rmCardImages = new ResourceManager(
                            "cardimages", Assembly.GetExecutingAssembly());
                        
                        for (int intSuit=0; intSuit<4; intSuit++)
                        {
                            ma_bmpKingImages[intSuit]  = new ArrayList();
                            ma_bmpQueenImages[intSuit] = new ArrayList();
                            ma_bmpJackImages[intSuit]  = new ArrayList();
                            ma_bmpSuitImages[intSuit]  = new ArrayList();
                            for (int intSize=0; intSize<7; intSize++)
                            {
                                ma_bmpKingImages[intSuit].Add(
                                    GetBitmapFromResource(rmCardImages, 
                                        "bmpKing"+a_strSuits[intSuit]+"_"+intSize));
                        
                                ma_bmpQueenImages[intSuit].Add(
                                    GetBitmapFromResource(rmCardImages, 
                                        "bmpQueen"+a_strSuits[intSuit]+"_"+intSize));
                                       
                                ma_bmpJackImages[intSuit].Add(
                                    GetBitmapFromResource(rmCardImages, 
                                        "bmpJack"+a_strSuits[intSuit]+"_"+intSize));
                            }
                            
                            for (int intSize=0; intSize<9; intSize++)
                            {
                                ma_bmpSuitImages[intSuit].Add(
                                    GetBitmapFromResource(rmCardImages, 
                                        "bmp"+a_strSuits[intSuit]+"_"+intSize));
                            }
                        }
                        
                        for (int intIndex=1; intIndex<=13; intIndex++)
                        {
                            ma_bmpBlackRankImages[intIndex] = new ArrayList();
                            ma_bmpRedRankImages[intIndex] = new ArrayList();
                            
                            for (int intSize=0; intSize<5; intSize++)
                            {
                                ma_bmpBlackRankImages[intIndex].Add(
                                    GetBitmapFromResource(rmCardImages, 
                                        "bmpBlack"+intIndex+"_"+intSize));
                                ma_bmpRedRankImages[intIndex].Add(
                                    GetBitmapFromResource(rmCardImages, 
                                        "bmpRed"+intIndex+"_"+intSize));
                            }
                        }
                        
                        m_blnResourcesLoaded = true;
                    }
                    catch
                    {
                    } 
                }
                
                // ------------------------------------------------------ //*/
                        
                public static Bitmap KingBitmap(Suit suit, int intWidth, int intHeight)
                {
                    LoadResources();
                    
                    Bitmap bmpKing = null;
                    for (int intSize = ma_bmpKingImages[(int)suit].Count-1;
                         intSize >= 0 && bmpKing == null; intSize--)
                    {
                        Bitmap bmpCurrentKing = (Bitmap)ma_bmpKingImages[(int)suit][intSize];
                        
                        if (bmpCurrentKing != null)
                        {
                            if (bmpCurrentKing.Width <= intWidth &&
                                bmpCurrentKing.Height <= intHeight)
                            {
                                bmpKing = bmpCurrentKing;
                            }
                        }
                    }
                    
                    return bmpKing;
                }
                
                // ------------------------------------------------------ //
                
                public static Bitmap QueenBitmap(Suit suit, int intWidth, int intHeight)
                {
                    LoadResources();
                    
                    Bitmap bmpQueen = null;
                    for (int intSize = ma_bmpQueenImages[(int)suit].Count-1;
                         intSize >= 0 && bmpQueen == null; intSize--)
                    {
                        Bitmap bmpCurrentQueen = (Bitmap)ma_bmpQueenImages[(int)suit][intSize];
                        
                        if (bmpCurrentQueen.Width <= intWidth &&
                            bmpCurrentQueen.Height <= intHeight)
                        {
                            bmpQueen = bmpCurrentQueen;
                        }
                    }
                    
                    return bmpQueen;
                }
                
                // ------------------------------------------------------ //
                
                public static Bitmap JackBitmap(Suit suit, int intWidth, int intHeight)
                {
                    LoadResources();
                    
                    Bitmap bmpJack = null;
                    for (int intSize = ma_bmpJackImages[(int)suit].Count-1;
                         intSize >= 0 && bmpJack == null; intSize--)
                    {
                        Bitmap bmpCurrentJack = (Bitmap)ma_bmpJackImages[(int)suit][intSize];
                        
                        if (bmpCurrentJack.Width <= intWidth &&
                            bmpCurrentJack.Height <= intHeight)
                        {
                            bmpJack = bmpCurrentJack;
                        }
                    }
                    
                    return bmpJack;
                }
                
                // ------------------------------------------------------ //
                
                public static Bitmap SuitBitmap(Suit suit, int intWidth, int intHeight)
                {
                    LoadResources();
                    
                    Bitmap bmpSuit = null;
                    for (int intSize = ma_bmpSuitImages[(int)suit].Count-1;
                         intSize >= 0 && bmpSuit == null; intSize--)
                    {
                        Bitmap bmpCurrentSuit = (Bitmap)ma_bmpSuitImages[(int)suit][intSize];
                        
                        if (bmpCurrentSuit.Width <= intWidth &&
                            bmpCurrentSuit.Height <= intHeight)
                        {
                            bmpSuit = bmpCurrentSuit;
                        }
                    }
                    
                    return bmpSuit;
                }
                
                // ------------------------------------------------------ //
                
                public static Bitmap RankBitmap(Rank rank, Suit suit, int intWidth, int intHeight)
                {
                    LoadResources();
                    
                    Bitmap bmpRank = null;
                    if (suit == Suit.Spades || suit == Suit.Clubs)
                    {
                        for (int intSize = ma_bmpBlackRankImages[(int)rank].Count-1;
                             intSize >= 0 && bmpRank == null; intSize--)
                        {
                            Bitmap bmpCurrentRank = (Bitmap)ma_bmpBlackRankImages[(int)rank][intSize];
                        
                            if (bmpCurrentRank.Width <= intWidth &&
                                bmpCurrentRank.Height <= intHeight)
                            {
                                bmpRank = bmpCurrentRank;
                            }
                        }
                    }
                    else
                    {
                        for (int intSize = ma_bmpRedRankImages[(int)rank].Count-1;
                             intSize >= 0 && bmpRank == null; intSize--)
                        {
                            Bitmap bmpCurrentRank = (Bitmap)ma_bmpRedRankImages[(int)rank][intSize];
                        
                            if (bmpCurrentRank.Width <= intWidth &&
                                bmpCurrentRank.Height <= intHeight)
                            {
                                bmpRank = bmpCurrentRank;
                            }
                        }
                    }
                    
                    return bmpRank;
                }
                
                public static Bitmap SmallestSuitBitmap(Suit suit)
                {
                    return (Bitmap)ma_bmpSuitImages[(int)suit][0];
                }
                
                public static Bitmap SmallestRankBitmap(Rank rank, Suit suit)
                {
                    if (suit == Suit.Spades || suit == Suit.Clubs)
                    {
                        return (Bitmap)ma_bmpBlackRankImages[(int)rank][0];
                    }
                    else
                    {
                        return (Bitmap)ma_bmpRedRankImages[(int)rank][0];
                    }
                }
                // ------------------------------------------------------ //
                
                public static int Rows(Rank rank)
                {
                    int intReturn;
                    switch(rank)
                    {
                        case Rank.Ace:
                            intReturn = 1;
                            break;
                        case Rank.Deuce:
                            intReturn = 2;
                            break;
                        case Rank.Three:
                            intReturn = 3;
                            break;
                        case Rank.Four:
                            intReturn = 2;
                            break;
                        case Rank.Five:
                            intReturn = 3;
                            break;
                        case Rank.Six:
                            intReturn = 3;
                            break;
                        case Rank.Seven:
                            intReturn = 5;
                            break;
                        case Rank.Eight:
                            intReturn = 5;
                            break;
                        case Rank.Nine:
                            intReturn = 7;
                            break;
                        case Rank.Ten:
                            intReturn = 7;
                            break;
                        case Rank.Jack:
                            intReturn = 0;
                            break;
                        case Rank.Queen:
                            intReturn = 0;
                            break;
                        case Rank.King:
                            intReturn = 0;
                            break;
                        default:
                            intReturn = 0;
                            break;
                    }
                    return intReturn;
                }
                
                // ------------------------------------------------------ //
                
                public static Color SuitColour(Suit suit)
                {
                    Color suitColour;
                    switch (suit)
                    {
                        case Suit.Spades: 
                        case Suit.Clubs:
                            suitColour = Color.Black;
                            break;
                        case Suit.Hearts: 
                        case Suit.Diamonds:
                            suitColour = Color.Red;
                            break;
                        default:
                            suitColour = Color.Transparent;
                            break;
                    }
                    
                    return suitColour;
                }
                
                
                // ------------------------------------------------------ //
                
                public static int Points(Rank rank)
                {
                    int intReturn;
                    switch(rank)
                    {
                        case Rank.Ace:
                            intReturn = 1;
                            break;
                        case Rank.Deuce:
                            intReturn = 2;
                            break;
                        case Rank.Three:
                            intReturn = 3;
                            break;
                        case Rank.Four:
                            intReturn = 4;
                            break;
                        case Rank.Five:
                            intReturn = 5;
                            break;
                        case Rank.Six:
                            intReturn = 6;
                            break;
                        case Rank.Seven:
                            intReturn = 7;
                            break;
                        case Rank.Eight:
                            intReturn = 8;
                            break;
                        case Rank.Nine:
                            intReturn = 9;
                            break;
                        case Rank.Ten:
                            intReturn = 10;
                            break;
                        case Rank.Jack:
                            intReturn = 11;
                            break;
                        case Rank.Queen:
                            intReturn = 12;
                            break;
                        case Rank.King:
                            intReturn = 13;
                            break;
                        default:
                            intReturn = 0;
                            break;
                    }
                    return intReturn;
                }
                
                // ------------------------------------------------------ //
                
                public static int RankPointPosition(Rank rank, int intPoint)
                {
                    int intReturn;
                    switch(rank)
                    {
                        case Rank.Ace:
                            intReturn = mca_intPointPos[0,intPoint];
                            break;
                        case Rank.Deuce:
                            intReturn = mca_intPointPos[1,intPoint];
                            break;
                        case Rank.Three:
                            intReturn =  mca_intPointPos[2,intPoint];
                            break;
                        case Rank.Four:
                            intReturn = mca_intPointPos[3,intPoint];
                            break;
                        case Rank.Five:
                            intReturn = mca_intPointPos[4,intPoint];
                            break;
                        case Rank.Six:
                            intReturn = mca_intPointPos[5,intPoint];
                            break;
                        case Rank.Seven:
                            intReturn = mca_intPointPos[6,intPoint];
                            break;
                        case Rank.Eight:
                            intReturn = mca_intPointPos[7,intPoint];
                            break;
                        case Rank.Nine:
                            intReturn = mca_intPointPos[8,intPoint];
                            break;
                        case Rank.Ten:
                            intReturn = mca_intPointPos[9,intPoint];
                            break;
                        case Rank.Jack:
                            intReturn = mca_intPointPos[10,intPoint];
                            break;
                        case Rank.Queen:
                            intReturn = mca_intPointPos[11,intPoint];
                            break;
                        case Rank.King:
                            intReturn = mca_intPointPos[12,intPoint];
                            break;
                        default:
                            intReturn = 0;
                            break;
                    }
                    return intReturn;
                }
                
                public static int RankPointRow(Rank rank, int intPoint)
                {
                    int intReturn;
                    switch(rank)
                    {
                        case Rank.Ace:
                            intReturn = mca_intPointRow[0,intPoint];
                            break;
                        case Rank.Deuce:
                            intReturn = mca_intPointRow[1,intPoint];
                            break;
                        case Rank.Three:
                            intReturn =  mca_intPointRow[2,intPoint];
                            break;
                        case Rank.Four:
                            intReturn = mca_intPointRow[3,intPoint];
                            break;
                        case Rank.Five:
                            intReturn = mca_intPointRow[4,intPoint];
                            break;
                        case Rank.Six:
                            intReturn = mca_intPointRow[5,intPoint];
                            break;
                        case Rank.Seven:
                            intReturn = mca_intPointRow[6,intPoint];
                            break;
                        case Rank.Eight:
                            intReturn = mca_intPointRow[7,intPoint];
                            break;
                        case Rank.Nine:
                            intReturn = mca_intPointRow[8,intPoint];
                            break;
                        case Rank.Ten:
                            intReturn = mca_intPointRow[9,intPoint];
                            break;
                        case Rank.Jack:
                            intReturn = mca_intPointRow[10,intPoint];
                            break;
                        case Rank.Queen:
                            intReturn = mca_intPointRow[11,intPoint];
                            break;
                        case Rank.King:
                            intReturn = mca_intPointRow[12,intPoint];
                            break;
                        default:
                            intReturn = 0;
                            break;
                    }
                    return intReturn;
                }
                
                // ------------------------------------------------------ //
                
                public static int FlipPoint(Rank rank)
                {
                    int intFlipPoint = 13;
                    switch(rank)
                    {
                        case Rank.Deuce:
                            intFlipPoint = 1;
                            break;
                        case Rank.Three:
                        case Rank.Four:
                            intFlipPoint = 2;
                            break;
                        case Rank.Five:
                            intFlipPoint = 3;
                            break;
                        case Rank.Six:
                            intFlipPoint = 4;
                            break;
                        case Rank.Seven:
                        case Rank.Eight:
                        case Rank.Nine:
                        case Rank.Ten:
                            intFlipPoint = 5;
                            break;
                        default:
                            intFlipPoint = 13;
                            break;
                    }
                    
                    return intFlipPoint;
                }           
                
                // ------------------------------------------------------ //
                
                // positions of suit symbols to display rank
                private static int[,] mca_intPointRow = 
                    {
                        {1,0,0,0,0,0,0,0,0,0,0,0,0},{1,2,0,0,0,0,0,0,0,0,0,0,0},
                        {1,2,3,0,0,0,0,0,0,0,0,0,0},{1,1,2,2,0,0,0,0,0,0,0,0,0},
                        {1,1,2,3,3,0,0,0,0,0,0,0,0},{1,1,2,2,3,3,0,0,0,0,0,0,0},
                        {1,1,2,3,3,5,5,0,0,0,0,0,0},{1,1,2,3,3,4,5,5,0,0,0,0,0},
                        {1,1,3,3,4,5,5,7,7,0,0,0,0},{1,1,2,3,3,5,5,6,7,7,0,0,0},
                        {0,0,0,0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {0,0,0,0,0,0,0,0,0,0,0,0,0}
                    };
                
                // positions of suit symbols to display rank
                private static int[,] mca_intPointPos = 
                    {
                        {2,0,0,0,0,0,0,0,0,0,0,0,0},{2,4,0,0,0,0,0,0,0,0,0,0,0},
                        {2,4,6,0,0,0,0,0,0,0,0,0,0},{2,2,4,4,0,0,0,0,0,0,0,0,0},
                        {2,2,3,4,4,0,0,0,0,0,0,0,0},{2,2,4,4,6,6,0,0,0,0,0,0,0},
                        {2,2,3,4,4,6,6,0,0,0,0,0,0},{2,2,3,4,4,5,6,6,0,0,0,0,0},
                        {2,2,4,4,5,6,6,8,8,0,0,0,0},{2,2,3,4,4,6,6,7,8,8,0,0,0},
                        {2,2,3,4,4,5,6,6,7,8,8,0,0},{2,2,3,4,4,6,6,8,8,9,10,10,0},
                        {2,2,3,4,4,5,6,6,8,8,9,10,10}
                    };        
            }
            
            // ------------------------------------------------------ //
        }
    }
}