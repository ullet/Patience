/*
 *************************************************************************
 * Cards.dll: Generic card handling class library.                       *
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
 * File:          Cards.cs                                               *
 * Namespace:     SwSt.Cards                                             *
 * Last modified: 13 January 2005                                        *
 * Description:   Generic card handling classes.                         *
 *************************************************************************
 */

using System;
using System.Reflection;
using System.Collections;
using System.Drawing;

// ------------------------------------------------------ //

namespace SwSt
{
    namespace Cards
    {
        // -------------------------------- //
        
        public enum Colour
        {
            None  =  0,
            Red   =  1,
            Black =  2,
        };
        
        // -------------------------------- //
        
        public enum Suit
        {
            Clubs      =  0,
            Diamonds   =  1,
            Hearts     =  2,
            Spades     =  3,
            RedJoker   =  4,
            BlackJoker =  5,
            None       = 99
        };
        
        // -------------------------------- //
    
        public enum Rank
        {
            None       =  0,
            Ace        =  1,
            Deuce      =  2,
            Three      =  3,
            Four       =  4,
            Five       =  5,
            Six        =  6,
            Seven      =  7,
            Eight      =  8,
            Nine       =  9,
            Ten        = 10,
            Jack       = 11,
            Queen      = 12,
            King       = 13
        };
        
        // -------------------------------- //
        
        public interface ICardSet
        {
            // -------------------------------- //
            
            // for "wrapper classes" supporting multiple cardsets
            ICardSet[] ChildCardSets
            {
                get;
            }
            
            // -------------------------------- //
            
            string CardSetID
            {
                get;
            }
            
            // -------------------------------- //
            
            int Width
            {
                get;
            } 
            
            // -------------------------------- //
            
            int Height
            {
                get;
            }
            
            // -------------------------------- //
            
            int UnscaledWidth
            {
                get;
            } 
            
            // -------------------------------- //
            
            int UnscaledHeight
            {
                get;
            }
            
            // -------------------------------- //
            
            int MinWidth
            {
                get;
            } 
            
            // -------------------------------- //
            
            int MinHeight
            {
                get;
            }
            
            // -------------------------------- //
            
            int Count
            {
                get;
            }
            
            // -------------------------------- //
                    
            CardCollection AllCards
            {
                get;
            }
            
            // -------------------------------- //
                    
            Card Card(int intCardNumber);
            
            // -------------------------------- //
            
            Card Card(Suit suit, Rank rank);
            
            // -------------------------------- //
            
            StackCard StackCard(int intStackNumber);
            
            // -------------------------------- //
            
            CardCollection PlayingCards
            {
                get;
            }
            
            // -------------------------------- //
            
            CardCollection StackCards
            {
                get;
            }
            
            // -------------------------------- //
            
            CardBack CardBack
            {
                get;
            }
            
            // -------------------------------- //
            
            CardSpace CardSpace
            {
                get;
            }
            
            // -------------------------------- //
            
            bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional);
            
            // -------------------------------- //
            
            bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional, bool blnResizeUp);
            
            // -------------------------------- //
            
            bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional, bool blnResizeUp, bool blnResizeDown);
            
            // -------------------------------- //
        
            bool Load(string strCardSetPath);
            
            // -------------------------------- //
            
            ///<summary>
            ///Save card set to a file.  Intended for use in card set
            ///editing or convertion programs, for example.
            ///</summary>
            bool Save(string strCardSetPath);
            
            // -------------------------------- //
            
            ///<summary>
            ///Use self-contained card set if IsSelfContained is true.
            ///</summary>
            bool UseInternal();
            
            // -------------------------------- //
            
            ///<summary>
            ///Set to true if realizing class contains all data required 
            ///to render card set without needing an external file.
            ///UseInternal method must be implemented.
            ///</summary>
            bool IsSelfContained
            {
                get;
            }
            
            // -------------------------------- //  
            
            ///<summary>
            ///Set to true if Load method implemented in realizing class
            ///</summary>
            bool IsLoadable
            {
                get;
            }
            
            // -------------------------------- //            
            
            ///<summary>
            ///Set to true if Save method implemented in realizing class
            ///</summary>
            bool IsSavable
            {
                get;
            }
            
            // -------------------------------- //
            
            void CreateFrom(ICardSet cardSet);
            
            // -------------------------------- //
            
            ///<summary>
            ///Filter to use for this cardset in open dialog
            ///</summary>
            string[] FileFilter
            {
                get;
            }
            
            // -------------------------------- //
            
            bool IsValid(string strFile);
            
            // -------------------------------- //
            
            // Module copyright  
            string Copyright
            {
                get;
            }
            
            // Module author's contact email address
            string ContactEmail
            {
                get;
            }
            
            // Module author's website
            string Website
            {
                get;
            }
                        
            // Module version
            string Version
            {
                get;
            }
            
            // Cardset Format name 
            string ModuleName
            {
                get;
            }
            
            // Cardset Format copyright  
            string FormatCopyright
            {
                get;
            }
            
            // Cardset Format copyright holders contact email address
            string FormatContactEmail
            {
                get;
            }
            
            // Cardset Format copyright holders  website
            string FormatWebsite
            {
                get;
            }
            
            // -------------------------------- //
            
        }
        
        // -------------------------------- //
        
        public class Card
        {
            protected Suit   m_suit        = Suit.None;
            protected Rank   m_rank        = Rank.None;
            protected Bitmap m_bmp         = null;
            protected Bitmap m_bmpUnscaled = null;
            
            // -------------------------------- //
            
            public Colour Colour
            {
                get
                {
                    Colour colour;
                    switch (m_suit)
                    {
                        case Suit.Hearts:
                        case Suit.Diamonds:
                        case Suit.RedJoker:
                            colour = Colour.Red;
                            break;
                        case Suit.Clubs:
                        case Suit.Spades:
                            colour = Colour.Black;
                            break;
                        default:
                            colour = Colour.None;
                            break;
                    }
                    return colour;
                }
            }
            
            // -------------------------------- //
            
            public Suit Suit
            {
                get
                {
                    return m_suit;
                }
                set
                {
                    m_suit = value;
                }
            }
            
            // -------------------------------- //
            
            public Rank Rank
            {
                get
                {
                    return m_rank;
                }
                set
                {
                    m_rank = value;
                }
            }
            
            // -------------------------------- //
            
            public string ShortSuitString
            {
                get
                {
                    return ((string)(""+m_suit)).Substring(0,1);
                }
            }
            
            // -------------------------------- //
            
            public string ShortRankString
            {
                get
                {
                    string strRank;
                    switch (m_rank)
                    {
                        case (Rank.Ace):
                            strRank = "A";
                            break;
                        case (Rank.King):
                            strRank = "K";
                            break;
                        case (Rank.Queen):
                            strRank = "Q";
                            break;
                        case (Rank.Jack):
                            strRank = "J";
                            break;
                        case (Rank.Ten):
                            strRank = "T";
                            break;
                        default:
                            strRank = "" + (int)m_rank;
                            break;
                    }
                    return strRank;
                }
            }
            
            // -------------------------------- //
            
            public Bitmap Bitmap
            {
                get
                {
                    return m_bmp;
                }
                set
                {
                    m_bmp         = value;
                }
            }
            
            // -------------------------------- //
            
            public void CreateFrom(Card card)
            {
                if (card != null && card.Bitmap != null)
                {
                    this.Bitmap = new Bitmap(card.Bitmap);
                    this.Suit   = card.Suit;
                    this.Rank   = card.Rank;
                    this.FixSize();
                }
            }
            
            // -------------------------------- //
            
            public Card()
            {
            }
            
            // -------------------------------- //
            
            public Card(Card card)
            {
                this.CreateFrom(card);
            }
            
            // -------------------------------- //
            
            public Card(Bitmap bmp)
            {
                if (bmp != null)
                {
                    m_bmp         = bmp;
                    FixSize();
                }
                else
                {
                    m_bmp         = null;
                    m_bmpUnscaled = null;
                }
            }
            
            // -------------------------------- //
            
            public Card(Suit suit, Rank rank)
            {
                m_suit  = suit;
                m_rank  = rank;
            }
            
            // -------------------------------- //
            
            public Card(Suit suit, Rank rank, Bitmap bmp) :
                   this(suit, rank)
            {
                m_bmp = bmp;
                FixSize();
            }
            
            // -------------------------------- //
            
            ~Card()
            {
                this.DisposeBitmap();
            }
            
            // -------------------------------- //
            
            protected virtual void DisposeBitmap()
            {
                if (m_bmp != null)
                {
                    try
                    {
                        m_bmp.Dispose();
                    }
                    catch{}
                    finally
                    {
                        m_bmp = null;
                    }
                }
                if (m_bmpUnscaled != null)
                {
                    try
                    {
                        m_bmpUnscaled.Dispose();
                    }
                    catch{}
                    finally
                    {
                        m_bmpUnscaled = null;
                    }
                }
            }
            
            // -------------------------------- //
            
            public virtual int Width
            {
                get
                {
                    if (m_bmp != null)
                    {
                        return m_bmp.Width;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            
            // -------------------------------- //
            
            public virtual int Height
            {
                get
                {
                    if (m_bmp != null)
                    {
                        return m_bmp.Height;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            
            // -------------------------------- //
            
            public virtual int UnscaledWidth
            {
                get
                {
                    if (m_bmpUnscaled != null)
                    {
                        return m_bmpUnscaled.Width;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            
            // -------------------------------- //
            
            public virtual int UnscaledHeight
            {
                get
                {
                    if (m_bmpUnscaled != null)
                    {
                        return m_bmpUnscaled.Height;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            
            // -------------------------------- //
            
            public override string ToString()
            {
                return m_rank + " of " + m_suit;
            }     
            
            // -------------------------------- //
            
            public string ToShortString()
            {
                return this.ShortRankString + this.ShortSuitString;
            }     
            
            // -------------------------------- //
            
            public void CopyTo(Card dest)
            {
                dest.Suit          = this.Suit;
                dest.Rank          = this.Rank;
                dest.m_bmp         = new Bitmap(this.m_bmp);
                dest.m_bmpUnscaled = new Bitmap(this.m_bmp); 
            }
            
            // -------------------------------- //
            
            protected Size CalculateNewSize(int intNewWidth, int intNewHeight, 
                                            bool blnProportional)
            {
                Size ScaledSize = new Size(intNewWidth, intNewHeight);
                
                if (blnProportional)
                {
                    double dblWidthScaleFactor  = 
                        (double)intNewWidth/(double)m_bmpUnscaled.Width;
                    double dblHeightScaleFactor = 
                        (double)intNewHeight/(double)m_bmpUnscaled.Height; 
                    double dblScaleFactor = Math.Min(dblWidthScaleFactor,
                                                     dblHeightScaleFactor);
                                   
                    ScaledSize.Width  = 
                        (int)(dblScaleFactor*m_bmpUnscaled.Width);
                    ScaledSize.Height = 
                        (int)(dblScaleFactor*m_bmpUnscaled.Height);
                }
                 
                return ScaledSize;
            }
            
            // -------------------------------- //
            
            public virtual bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional)
            {
                // allow resize both up and down
                return Resize(intNewWidth, intNewHeight, blnProportional, true, true);
            }
            
            // -------------------------------- //
            
            public virtual bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional, bool blnResizeUp)
            {
                // allow resize down
                return Resize(intNewWidth, intNewHeight, 
                              blnProportional, blnResizeUp, true);
            }
            
            // -------------------------------- //
            
            public virtual bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional, bool blnResizeUp, bool blnResizeDown)
            {
                bool blnResized = false;
                
                if (m_bmpUnscaled != null)
                {
                    if (intNewWidth > 0 && intNewHeight > 0)
                    {
                        // calculate scale factor
                        Size ScaledSize = CalculateNewSize(intNewWidth, intNewHeight, 
                                                           blnProportional);
                        // only scale if correct flags are set and size has changed                    
                        if (m_bmp == null || 
                           ScaledSize.Width  != m_bmp.Width ||
                           ScaledSize.Height != m_bmp.Height)
                        {
                            // new size different to old size so need to check if
                            // need to scale
                            
                            if (ScaledSize.Width  == m_bmpUnscaled.Width &&
                                ScaledSize.Height == m_bmpUnscaled.Height)
                            {
                                // new size same as unscaled size so just copy
                                m_bmp = new Bitmap(m_bmpUnscaled);
                                blnResized = true;
                            }
                            else if ((blnResizeUp && blnResizeDown) ||
                                     (blnResizeUp && 
                                      (ScaledSize.Width>m_bmpUnscaled.Width &&
                                       ScaledSize.Height>m_bmpUnscaled.Height)) ||
                                     (blnResizeDown && 
                                      (ScaledSize.Width<m_bmpUnscaled.Width &&
                                       ScaledSize.Height<m_bmpUnscaled.Height)))
                            {
                                // scale as correct flags are set
                                m_bmp = new Bitmap(m_bmpUnscaled, ScaledSize);
                                blnResized = true;
                            }
                        } // end if size changed
                    }
                    // else don't resize
                }
                // else don't resize
                
                return blnResized;
            }
            
            // -------------------------------- //
            
            // replaced unscaled bitmap with 'scaled' one, for whatever purpose
            public void FixSize()
            {
                if (m_bmp == null)
                {
                    // null bitmap so clear unscaled
                    m_bmpUnscaled = null;
                }
                else
                {
                    if (m_bmp != m_bmpUnscaled)
                    {
                        if (m_bmpUnscaled != null)
                        {
                            try
                            {
                                m_bmpUnscaled.Dispose();
                            }
                            catch{}
                        }
                        m_bmpUnscaled = new Bitmap(m_bmp);
                    }
                }
            }
            
            // -------------------------------- //
            
            // Restores bitmap to unscaled bitmap
            public void Restore()
            {
                if (m_bmpUnscaled != null)
                {
                    if (m_bmp != m_bmpUnscaled)
                    {
                        if (m_bmp != null)
                        {
                            try
                            {
                                m_bmp.Dispose();
                            }
                            catch{}
                        }
                        m_bmp = new Bitmap(m_bmpUnscaled);
                    }
                }
            }       
            
            public static Bitmap BorderBitmap(int intWidth, int intHeight,
                                              Color color)
            {
                Bitmap bmpBorder = new Bitmap(intWidth, intHeight);
                
                Graphics graphics = Graphics.FromImage(bmpBorder);
                
                Pen pen = new Pen(color);
                graphics.DrawRectangle(pen, 0, 0, intWidth-1, intHeight-1);
                graphics.Dispose();
                
                bmpBorder.SetPixel(1,1,color);
                bmpBorder.SetPixel(1,intHeight-2,color);
                bmpBorder.SetPixel(intWidth-2,1,color);
                bmpBorder.SetPixel(intWidth-2,intHeight-2,color);
                bmpBorder.SetPixel(0,0,Color.Transparent);
                bmpBorder.SetPixel(0,intHeight-1,Color.Transparent);
                bmpBorder.SetPixel(intWidth-1,0,Color.Transparent);
                bmpBorder.SetPixel(intWidth-1,intHeight-1,Color.Transparent);
                
                return bmpBorder;
            }  
            
            public void SetAlpha(int intAlphaValue)
            {
                if (this.Bitmap == null)
                {
                    return;
                }
                
                for (int intX=0; intX<this.Bitmap.Width; intX++)
                {
                    for (int intY=0; intY<this.Bitmap.Height; intY++)
                    {
                        Color cp = this.Bitmap.GetPixel(intX, intY);
                        this.Bitmap.SetPixel(intX, intY, 
                            Color.FromArgb(intAlphaValue, cp.R, cp.G, cp.B));
                    }
                }
            }
            
            public void SetAlpha(Bitmap bmpMask)
            {
                if (bmpMask == null || this.Bitmap == null)
                {
                    return;
                }
                
                for (int intX=0; intX<this.Bitmap.Width; intX++)
                {
                    for (int intY=0; intY<this.Bitmap.Height; intY++)
                    {
                        Color cp = this.Bitmap.GetPixel(intX, intY);
                        int intAlphaValue = 
                            bmpMask.GetPixel(
                                intX % bmpMask.Width,
                                intY % bmpMask.Height).A;
                        this.Bitmap.SetPixel(intX, intY, 
                            Color.FromArgb(intAlphaValue, cp.R, cp.G, cp.B));
                    }
                }
            }
            
            public void ApplyAlphaMask(int intAlphaValue)
            {
                if (this.Bitmap == null)
                {
                    return;
                }
                
                for (int intX=0; intX<this.Bitmap.Width; intX++)
                {
                    for (int intY=0; intY<this.Bitmap.Height; intY++)
                    {
                        Color cp = this.Bitmap.GetPixel(intX, intY);
                        int intAlpha = cp.A;
                        intAlpha &= intAlphaValue;
                        this.Bitmap.SetPixel(intX, intY, 
                            Color.FromArgb(intAlpha, cp.R, cp.G, cp.B));
                    }
                }
            } 
            
            public void ApplyAlphaMask(Bitmap bmpMask)
            {
                if (bmpMask == null || this.Bitmap == null)
                {
                    return;
                }
                
                for (int intX=0; intX<this.Bitmap.Width; intX++)
                {
                    for (int intY=0; intY<this.Bitmap.Height; intY++)
                    {
                        Color cp = this.Bitmap.GetPixel(intX, intY);
                        int intAlpha = cp.A;
                        int intAlphaValue = 
                            bmpMask.GetPixel(
                                intX % bmpMask.Width,
                                intY % bmpMask.Height).A;
                        intAlpha &= intAlphaValue;
                        this.Bitmap.SetPixel(intX, intY, 
                            Color.FromArgb(intAlpha, cp.R, cp.G, cp.B));
                    }
                }
            }
        }
        
        // -------------------------------- //
        
        public class CardBack : Card
        {
            // -------------------------------- //
            
            public void CreateFrom(CardBack card)
            {
                if (card != null && card.Bitmap != null)
                {
                    this.Bitmap = new Bitmap(card.Bitmap);
                    this.FixSize();
                }
            }
            
            // -------------------------------- //
            
            public CardBack() : base()
            {
            }
            
            // -------------------------------- //
            
            public CardBack(Bitmap bmp) : base(bmp)
            {
            }
            
            // -------------------------------- //
            
            public CardBack(Card card) : base(card)
            {
            }
            
            // -------------------------------- //
            
            public CardBack(CardBack card)
            {
                this.CreateFrom(card);
            }
            
            // -------------------------------- //
            
            ~CardBack()
            {
                this.DisposeBitmap();
            }
        }
        
        // -------------------------------- //
        
        public class CardSpace : Card
        {
            // -------------------------------- //
            
            public void CreateFrom(CardSpace card)
            {
                if (card != null && card.Bitmap != null)
                {
                    this.Bitmap = new Bitmap(card.Bitmap);
                    this.FixSize();
                }
            }
            
            // -------------------------------- //
            
            public CardSpace() : base()
            {
            }
            
            // -------------------------------- //
            
            public CardSpace(Bitmap bmp) : base(bmp)
            {
            }
            
            // -------------------------------- //
            
            public CardSpace(Card card) : base(card)
            {
            }
            
            // -------------------------------- //
            
            public CardSpace(CardSpace card)
            {
                this.CreateFrom(card);
            }
            
            // -------------------------------- //
            
            ~CardSpace()
            {
                this.DisposeBitmap();
            }
        }
        
        // -------------------------------- //
        
        public class StackCard : Card
        {
            protected int m_intStackNumber = 0;
            
            // -------------------------------- //
            
            public void CreateFrom(StackCard card)
            {
                if (card != null && card.Bitmap != null)
                {
                    this.Bitmap      = new Bitmap(card.Bitmap);
                    this.StackNumber = card.StackNumber; 
                    this.FixSize();
                }
            }
            
            // -------------------------------- //
            
            public StackCard() : base()
            {
            }
            
            // -------------------------------- //
            
            public StackCard(Bitmap bmp) : base(bmp)
            {
            }
            
            // -------------------------------- //
            
            public StackCard(int intStackNumber)
            {
                m_intStackNumber = intStackNumber;
            }
            
            // -------------------------------- //
            
            public StackCard(Bitmap bmp, int intStackNumber) : this(bmp)
            {
                m_intStackNumber = intStackNumber;
            }
            
            // -------------------------------- //
            
            public StackCard(Card card) : base(card)
            {
            }
            
            // -------------------------------- //
            
            public StackCard(StackCard card)
            {
                this.CreateFrom(card);
            }
            
            // -------------------------------- //
            
            ~StackCard()
            {
                this.DisposeBitmap();
            }
            
            // -------------------------------- //
            
            public int StackNumber
            {
                get
                {
                    return m_intStackNumber;
                }
                set
                {
                    m_intStackNumber = value;
                }
            }
        }
        
        // -------------------------------- //
        
        public class CardCollection : CollectionBase
        {
            // -------------------------------- //
            
            public CardCollection()
            {
            }
            
            // -------------------------------- //
            
            public void Add(Card card)
            {
                List.Add(card);
            }
            
            // -------------------------------- //
            
            public void Remove(Card card)
            {
                List.Remove(card);
            }
            
            // -------------------------------- //
            
            public Card this[int intCardIndex]
            {
                get
                {
                    return (Card)List[intCardIndex];
                }
                set
                {
                    List[intCardIndex] = value;
                }
            }
            
            // -------------------------------- //
            
            public Card Last
            {
                get
                {
                    if (this.Count > 0)
                    {
                        return this[this.Count-1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            
            // -------------------------------- //
            
            public Card First
            {
                get
                {
                    if (this.Count > 0)
                    {
                        return this[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            
            // -------------------------------- //
            
            public void CopyTo(CardCollection dest)
            {
                CopyTo(dest, 0, this.Count);
            }
            
            // -------------------------------- //
            
            public void CopyTo(CardCollection dest, int intStartIndex)
            {
                CopyTo(dest, intStartIndex, this.Count-intStartIndex);
            }
            
            // -------------------------------- //
            
            public void CopyTo(CardCollection dest, int intStartIndex, int intLength)
            {
                if (intStartIndex >=0 && intStartIndex <= this.Count &&
                    intStartIndex+intLength <= this.Count)
                {
                    // shallow copy collection
                    for (int intIndex=intStartIndex; 
                         intIndex<intStartIndex+intLength; 
                         intIndex++)
                    {
                        dest.Add(this[intIndex]);
                    }
                }
            }
        }
        
        // -------------------------------- //
        
        public class Deck
        {
            // -------------------------------- //
            
            protected enum CardOrder
            {
                Forward = 1,
                Reverse = 2,
                Random  = 3
            }; 
            
            // -------------------------------- //
            private CardCollection m_Cards;
            private CardOrder      m_CardOrder = CardOrder.Random; 
                                   // assume initial deck is random
            private Random         m_RandomNumberGenerator;
            
            // -------------------------------- //
            
            public Deck()
            {
                // start with no seed for random number generator i.e. 'really' random
                m_RandomNumberGenerator = new Random();
            }
            
            // -------------------------------- //
            
            public Deck(CardCollection CardDeck) : this()
            {
                m_Cards = new CardCollection();
                CardDeck.CopyTo(m_Cards);
            }
            
            // -------------------------------- //
            
            public CardCollection Cards
            {
                get
                {
                    return m_Cards;
                }
                set
                {
                    m_Cards = new CardCollection();
                    value.CopyTo(m_Cards);
                }
            }
            
            // -------------------------------- //
            
            public Card TopCard
            {
                get
                {
                    return Card(0);
                }
            }
            
            // -------------------------------- //
            
            public Card BottomCard
            {
                get
                {
                    return Card(m_Cards.Count-1);
                }            
            }
            
            // -------------------------------- //
            
            public int Count
            {
                get
                {
                    return m_Cards.Count;
                }
            }
            
            // -------------------------------- //
            
            public Card Card(int intCardNumber)
            {
                if (m_Cards.Count > intCardNumber && intCardNumber >= 0)
                {
                    return m_Cards[intCardNumber];
                }
                else
                {
                    return null;
                }
            }
            
            // -------------------------------- //
            
            public Card Card(Cards.Suit suit, Cards.Rank rank)
            {
                Card ReturnCard = null;
                foreach (Card card in m_Cards)
                {
                    if (card.Suit == suit && card.Rank == rank)
                    {
                        ReturnCard = card;
                        break;
                    }
                }
                
                return ReturnCard;
            }
            
            // -------------------------------- //
            
            public void Sort()
            {
                Sort(CardOrder.Forward);
            }
            
            // -------------------------------- //
            
            public void ReverseSort()
            {
                Sort(CardOrder.Reverse);
            }
            
            // -------------------------------- //
            
            private void Sort(CardOrder order)
            {
                Suit[]         suitOrder   = new Suit[4];
                Rank[]         rankOrder   = new Rank[13];
                CardCollection SortedCards = new CardCollection();
                
                if (order != CardOrder.Random && order == m_CardOrder)
                {
                    // already sorted so don't do it again
                    return;
                }
                
                if (order == CardOrder.Random)
                {
                    Shuffle();
                }
                else if (order == CardOrder.Forward)
                {
                    suitOrder[0] = Suit.Clubs;
                    suitOrder[1] = Suit.Diamonds;
                    suitOrder[2] = Suit.Hearts;
                    suitOrder[3] = Suit.Spades;
                    
                    for (int intRank = 1; intRank <= 13; intRank ++)
                    {
                        rankOrder[intRank-1] = (Rank)intRank;
                    }
                }
                else
                {
                    suitOrder[3] = Suit.Clubs;
                    suitOrder[2] = Suit.Diamonds;
                    suitOrder[1] = Suit.Hearts;
                    suitOrder[0] = Suit.Spades;
                    
                    for (int intRank = 13; intRank > 0; intRank --)
                    {
                        rankOrder[13-intRank] = (Rank)intRank;
                    }                
                }
                
                int intSuitNumber = 0;
                int intRankNumber = 0;
                while (intSuitNumber < 4)
                {
                    bool blnFoundCard = false;
                    foreach (Card card in m_Cards)
                    {
                        if (card.Suit == suitOrder[intSuitNumber] &&
                            card.Rank == rankOrder[intRankNumber])
                        {
                            SortedCards.Add(card);
                            blnFoundCard = true;
                            intRankNumber ++;
                            if (intRankNumber >= 13)
                            {
                                intSuitNumber ++;
                                intRankNumber = 0;
                            }
                            if (intSuitNumber >= 4)
                            {
                                break;
                            }
                        }
                    }
                    
                    if (intSuitNumber < 4 && !blnFoundCard)
                    {
                        // card not in deck so skip it
                        intRankNumber ++;
                        if (intRankNumber >= 13)
                        {
                            intSuitNumber ++;
                            intRankNumber = 0;
                        }
                    }
                }        
                
                m_Cards = SortedCards;   
            }
            
            // -------------------------------- //
            
            // Reverse order of cards, last becomes first, first becomes last and so on
            // Does NOT sort cards in to suit and rank order
            public void ReverseOrder()
            {
                CardCollection ReversedDeck = new CardCollection();
                for (int intIndex=m_Cards.Count-1; intIndex >= 0; intIndex --)
                {
                    ReversedDeck.Add(m_Cards[intIndex]);
                }
                
                m_Cards = ReversedDeck;
                if (m_CardOrder == CardOrder.Forward)
                {
                    m_CardOrder = CardOrder.Reverse;
                }
                else if (m_CardOrder == CardOrder.Reverse)
                {
                    m_CardOrder = CardOrder.Forward;
                }
                // else was random so still is
            }
            
            // -------------------------------- //
            
            public void Shuffle()
            {
                Shuffle(-1);
            }
            
            // -------------------------------- //
            
            public void Shuffle(int intSeed)
            {
                CardCollection ShuffledCards = new CardCollection();
                if (intSeed > 0)
                {
                    // new seed value given
                    m_RandomNumberGenerator = new Random(intSeed);
                }
                else if (intSeed == 0)
                {
                    // zero seed value given - 'randomize'
                    m_RandomNumberGenerator = new Random();
                }
                // else use existing random number generator
                
                while (m_Cards.Count > 0)
                {
                    int intIndex = m_RandomNumberGenerator.Next(m_Cards.Count);
                    ShuffledCards.Add(m_Cards[intIndex]);
                    m_Cards.Remove(m_Cards[intIndex]);
                }
                
                m_Cards = ShuffledCards;
                m_CardOrder = CardOrder.Random;
            }
            
            // -------------------------------- //
            
            public void Remove(Suit suit, Rank rank)
            {
                for (int intIndex = 0; intIndex<m_Cards.Count; intIndex++)
                {
                    if (m_Cards[intIndex].Rank == rank && m_Cards[intIndex].Suit == suit)
                    {
                        m_Cards.Remove(m_Cards[intIndex]);
                        break;
                    }
                }
            }
            
            // -------------------------------- //
            
            public void Remove(Suit suit)
            {
                int intIndex = 0;
                while (intIndex<m_Cards.Count)
                {
                    if (m_Cards[intIndex].Suit == suit)
                    {
                        m_Cards.Remove(m_Cards[intIndex]);
                        // don't increment index as next card is now where this one was
                    }
                    else
                    {
                        intIndex ++;
                    }
                }
            }
            
            // -------------------------------- //
            
            public void Remove(Rank rank)
            {
                int intIndex = 0;
                while (intIndex<m_Cards.Count)
                {
                    if (m_Cards[intIndex].Rank == rank)
                    {
                        m_Cards.Remove(m_Cards[intIndex]);
                        // don't increment index as next card is now where this one was
                    }
                    else
                    {
                        intIndex ++;
                    }
                }
            }
            
            // -------------------------------- //
            
            public Card RemoveTopCard()
            {
                return RemoveCard(0);
            }
            
            // -------------------------------- //
            
            public Card RemoveBottomCard()
            {
                return RemoveCard(m_Cards.Count-1);
            }
            
            // -------------------------------- //
            
            public Card RemoveCard(int intIndex)
            {
                Card card = null;
                if (intIndex < m_Cards.Count)
                {
                    card = m_Cards[intIndex];
                    m_Cards.Remove(card);
                }
                
                return card;
            }
            
            // -------------------------------- //
            
            public Card RemoveCard(Suit suit, Rank rank)
            {
                Card card = null;
                
                for (int intIndex = 0; intIndex<m_Cards.Count; intIndex++)
                {
                    if (m_Cards[intIndex].Rank == rank && m_Cards[intIndex].Suit == suit)
                    {
                        card = m_Cards[intIndex];
                        m_Cards.Remove(card);
                        break;
                    }
                }
                
                return card;
            }
            
            // -------------------------------- //
            
            public CardCollection RemoveCards(Suit suit)
            {
                CardCollection cards = new CardCollection();
                
                int intIndex = 0;
                while (intIndex<m_Cards.Count)
                {
                    if (m_Cards[intIndex].Suit == suit)
                    {
                        cards.Add(m_Cards[intIndex]);
                        m_Cards.Remove(m_Cards[intIndex]);
                        // don't increment index as next card is now where this one was
                    }
                    else
                    {
                        intIndex ++;
                    }
                }
                
                return cards;
            }
            
            // -------------------------------- //
            
            public CardCollection RemoveCards(Rank rank)
            {
                CardCollection cards = new CardCollection();
                
                int intIndex = 0;
                while (intIndex<m_Cards.Count)
                {
                    if (m_Cards[intIndex].Rank == rank)
                    {
                        cards.Add(m_Cards[intIndex]);
                        m_Cards.Remove(m_Cards[intIndex]);
                        // don't increment index as next card is now where this one was
                    }
                    else
                    {
                        intIndex ++;
                    }
                }
                
                return cards;
            }
            
            // -------------------------------- //
            
            public override string ToString()
            {
                string strDeck = "";
                
                foreach (Card card in m_Cards)
                {
                    if (strDeck != "")
                    {
                        strDeck += ", ";
                    }
                    strDeck += card;
                }    
                
                return strDeck;
            }          
        }
        
        // -------------------------------- //
        
        // basic graphic CardSet class
        public class CardSet : ICardSet
        {
            protected CardCollection m_AllCards     = null;
            protected CardCollection m_PlayingCards = null;
            protected CardCollection m_StackCards   = null;
            protected CardBack       m_CardBack     = null;
            protected CardSpace      m_CardSpace    = null;
            protected int            m_intWidth     = 0;
            protected int            m_intHeight    = 0;
                  
            // -------------------------------- //      
              
            public CardSet()
            {
                // collections of cards without bitmaps
                
                m_AllCards     = new CardCollection();
                m_PlayingCards = new CardCollection(); 
                m_StackCards   = new CardCollection();
                m_CardBack     = new CardBack();
                m_CardSpace    = new CardSpace();
                
                for (int intSuit=0; intSuit<=4; intSuit++)
                {
                    for (int intRank=1; intRank<=13; intRank++)
                    {
                        Card card = new Card((Suit)intSuit, (Rank)intRank);
                        m_AllCards.Add(card);
                        m_PlayingCards.Add(card);
                    }
                }
                // stack/foundation cards, one per suit
                for (int intStackNumber=0; intStackNumber<4; intStackNumber++)
                {
                    StackCard card = new StackCard(intStackNumber);
                    m_StackCards.Add(card);
                    m_AllCards.Add(card);
                }
                
                m_AllCards.Add(m_CardBack);
                m_AllCards.Add(m_CardSpace);
            }
            
            // -------------------------------- //
            
            public virtual ICardSet[] ChildCardSets
            {
                get
                {
                    return null;
                }
            }
            
            // -------------------------------- //
            
            public virtual string CardSetID
            {
                get
                {
                    return "";
                }
            }
            
            // -------------------------------- //
            
            public int Width
            {
                get
                {
                    return m_intWidth;
                }
            } 
            
            // -------------------------------- //
            
            public int Height
            {
                get
                {
                    return m_intHeight;
                }
            }
            
            // -------------------------------- //
            
            public virtual int UnscaledWidth
            {
                get
                {
                    if (m_AllCards.Count >= 0)
                    {
                        return m_AllCards[0].UnscaledWidth;
                    }
                    else
                    {
                        return m_intWidth;
                    }
                }
            } 
            
            // -------------------------------- //
            
            public virtual int UnscaledHeight
            {
                get
                {
                    if (m_AllCards.Count >= 0)
                    {
                        return m_AllCards[0].UnscaledHeight;
                    }
                    else
                    {
                        return m_intHeight;
                    }
                }
            }
            
            // -------------------------------- //
            
            public virtual int MinWidth
            {
                get
                {
                    return 0;
                }
            }
            
            // -------------------------------- //
            
            public virtual int MinHeight
            {
                get
                {
                    return 0;
                }
            }
            
            // -------------------------------- //
            
            public int Count
            {
                get
                {
                    return m_AllCards.Count;
                }
            }
            
            // -------------------------------- //
                    
            public CardCollection AllCards
            {
                get
                {
                    return m_AllCards;
                }
            }
            
            // -------------------------------- //
                    
            public Card Card(int intCardNumber)
            {
                return m_AllCards[intCardNumber];
            }
            
            // -------------------------------- //
            
            public Card Card(Suit suit, Rank rank)
            {
                Card ReturnCard = null;
                foreach (Card card in m_AllCards)
                {
                    if (card.Suit == suit && card.Rank == rank)
                    {
                        ReturnCard = card;
                        break;
                    }
                }
                
                return ReturnCard;
            }
            
            // -------------------------------- //
            
            public StackCard StackCard(int intStackNumber)
            {
                StackCard ReturnCard = null;
                
                foreach (StackCard card in m_StackCards)
                {
                    if (card.StackNumber == intStackNumber)
                    {
                        ReturnCard = card;
                        break;
                    }
                }
                
                return ReturnCard;
            }
            
            // -------------------------------- //
            
            public CardCollection PlayingCards
            {
                get
                {
                    return m_PlayingCards;
                }
            }
            
            // -------------------------------- //
            
            public CardCollection StackCards
            {
                get
                {
                    return m_StackCards;
                }
            }
            
            // -------------------------------- //
            
            public CardBack CardBack
            {
                get
                {
                    return m_CardBack;
                }
            }
            
            // -------------------------------- //
            
            public CardSpace CardSpace
            {
                get
                {
                    return m_CardSpace;
                }
            }
            
            // -------------------------------- //
            
            public virtual bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional)
            {
                // allow resize both up and down
                return Resize(intNewWidth, intNewHeight, blnProportional, true, true);
            }
            
            // -------------------------------- //
            
            public virtual bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional, bool blnResizeUp)
            {
                // allow resize down
                return Resize(intNewWidth, intNewHeight, blnProportional, blnResizeUp, true);
            }
            
            // -------------------------------- //
            
            public virtual bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional, bool blnResizeUp, bool blnResizeDown)
            {
                bool blnResized = false;
                
                foreach (Card card in m_PlayingCards)
                {
                    blnResized |= card.Resize(intNewWidth, intNewHeight, blnProportional,
                                              blnResizeUp, blnResizeDown);
                }
                
                foreach (StackCard card in m_StackCards)
                {
                    blnResized |= card.Resize(intNewWidth, intNewHeight, blnProportional,
                                              blnResizeUp, blnResizeDown);
                }
                
                blnResized |= m_CardBack.Resize(intNewWidth, intNewHeight, blnProportional,
                                                blnResizeUp, blnResizeDown);
                
                blnResized |= m_CardSpace.Resize(intNewWidth, intNewHeight, blnProportional,
                                                blnResizeUp, blnResizeDown);
                
                if (m_AllCards[0].Bitmap != null)
                {
                    m_intWidth  = m_AllCards[0].Bitmap.Width;
                    m_intHeight = m_AllCards[0].Bitmap.Height;
                }
                
                return blnResized;
            }
            
            
            // -------------------------------- //
        
            public virtual bool UseInternal()
            {
                return false;
            }
            
            // -------------------------------- //
        
            public virtual bool Load(string strCardSetPath)
            {
                return false;
            }
            
            // -------------------------------- //
            
            public virtual bool Save(string strCardSetPath)
            {
                return false;
            }
            
            // -------------------------------- //

            public virtual bool IsSelfContained
            {
                get
                {
                    return false;
                }
            }
            
            // -------------------------------- //
                        
            public virtual bool IsLoadable
            {
                get
                {
                    return false;
                }
            }
            
            // -------------------------------- //
            
            public virtual bool IsSavable
            {
                get
                {
                    return false;
                }
            }
            
            // -------------------------------- //
            
            public void CreateFrom(ICardSet cardSet)
            {
                if (cardSet == null)
                {
                    return;
                }
                
                Card myCard = null;
                foreach (Card card in cardSet.PlayingCards)
                {
                    myCard = this.Card(card.Suit, card.Rank);
                    if (myCard != null)
                    {
                        myCard.CreateFrom(card);
                    }
                }
                
                foreach (StackCard card in cardSet.StackCards)
                {
                    myCard = this.StackCard(card.StackNumber);
                    if (myCard != null)
                    {
                        myCard.CreateFrom(card);
                    }
                }
                
                myCard = this.CardBack;
                if (myCard != null)
                {
                    myCard.CreateFrom(cardSet.CardBack);
                }
                
                myCard = this.CardSpace;
                if (myCard != null)
                {
                    myCard.CreateFrom(cardSet.CardSpace);
                }
            }
            
            // -------------------------------- //
            
            public virtual string[] FileFilter
            {
                get
                {
                    return new string[]{"All Files|*.*"};
                }
            }
            
            // -------------------------------- //
            
            public virtual bool IsValid(string strCardSetPath)
            {
                return false;
            }
            
            // Module copyright  
            public virtual string Copyright
            {
                get
                {
                    return "";
                }
            }
            
            // Module author's contact email address
            public virtual string ContactEmail
            {
                get
                {
                    return "";
                }
                    
            }
            
            // Module author's website
            public virtual string Website
            {
                get
                {
                    return "";
                }
            }
                        
            // 
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
            
            // Cardset Module name 
            public virtual string ModuleName
            {
                get
                {
                    return "";
                }
            }
            
            // Cardset Format copyright  
            public virtual string FormatCopyright
            {
                get
                {
                    return "";
                }
            }
            
            // Cardset Format copyright holders contact email address
            public virtual string FormatContactEmail
            {
                get
                {
                    return "";
                }
            }
            
            // Cardset Format copyright holders website
            public virtual string FormatWebsite
            {
                get
                {
                    return "";
                }
            }
        } 
        
        // -------------------------------- //
    }
}