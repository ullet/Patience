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
 * File:          KlondikeCardGame.cs                                    *
 * Namespace:     SwSt.CardGames                                         *
 * Last modified: 13 January 20005                                       *
 * Class:         Klondike                                               *
 * Description:   Implement the rules of the card game Klondike.         *
 *************************************************************************
 */

using System;
using SwSt;
using SwSt.Cards;
using SwSt.CardGames;

namespace SwSt
{
    namespace CardGames
    {
       public class Klondike : CardGame
        {
            protected CardCollection[] m_FaceDownColumns = new CardCollection[7];
            protected Suit[] a_FoundationSuitMapping = new Suit[4];
            
            public Klondike()
            {
                this.Columns     = new CardCollection[7];
                this.Foundations = new CardCollection[4];
                
                // new deck of cards
                this.Deck = new Deck();
            }            
            
            public Klondike(CardCollection playingCards) : 
                this()
            {
                this.Initialise(playingCards);
                
                // start new game
                Restart();
            }
            
            public Klondike(CardCollection playingCards, int intRandomSeed) : 
                this()
            {
                this.Initialise(playingCards, intRandomSeed);
                
                // start new game
                Restart();
            }
            
            public override void Restart()
            {
                a_FoundationSuitMapping[0] = Suit.None;
                a_FoundationSuitMapping[1] = Suit.None;
                a_FoundationSuitMapping[2] = Suit.None;
                a_FoundationSuitMapping[3] = Suit.None;
                base.Restart();
            }
            
            public override CardCollection PlayingCards
            {
                get
                {
                    return base.PlayingCards;
                }
                set
                {
                    CardCollection playingCards = value;
                    base.PlayingCards = playingCards;
                    
                    Deck deck = new Deck(playingCards);
                    
                    // update foundations and rows with new cards
                    // keeping the existing order
                    
                    // set foundations
                    for (int intIndex=0; intIndex<4; intIndex++)
                    {
                        if (this.Foundation(intIndex) != null)
                        {
                            CardCollection foundation = new CardCollection();
                            foreach (Cards.Card card in this.Foundation(intIndex))
                            {
                                foundation.Add(deck.Card(card.Suit, card.Rank));
                            }
                            this.Foundations[intIndex] = foundation;
                            foundation = null;
                        }
                    }
                    
                    // set columns
                                    
                    for (int intColumn=0; intColumn<7; intColumn++)
                    {
                        if (this.Column(intColumn) != null)
                        {
                            CardCollection column = new CardCollection();
                            foreach (Cards.Card card in this.Column(intColumn))
                            {
                                column.Add(deck.Card(card.Suit, card.Rank));
                            }
                            this.Columns[intColumn] = column;
                            column = null;
                            column = new CardCollection();
                            foreach (Cards.Card card in this.FaceDownColumn(intColumn))
                            {
                                column.Add(deck.Card(card.Suit, card.Rank));
                            }
                            this.FaceDownColumns[intColumn] = column;
                            column = null;
                        }
                    }
                    
                    // set reserve
                    
                    if (this.TheReserve != null)
                    {
                        CardCollection reserve = new CardCollection();
                        foreach (Cards.Card card in this.TheReserve)
                        {
                            reserve.Add(deck.Card(card.Suit, card.Rank));
                        }
                        this.Reserves[0] = reserve;
                        reserve = null;
                    }
                    
                    // set discard
                    
                    if (this.TheDiscard != null)
                    {
                        CardCollection discard = new CardCollection();
                        foreach (Cards.Card card in this.TheDiscard)
                        {
                            discard.Add(deck.Card(card.Suit, card.Rank));
                        }
                        this.Discards[0] = discard;
                        discard = null;
                    }
                }
            }                
            
            public override void GatherCards()
            {
                this.Deck.Cards = this.PlayingCards;
            }
            
            public override void Deal()
            {
                // initialise foundations
                for (int intIndex=0; intIndex<4; intIndex++)
                {
                    this.Foundations[intIndex] = new CardCollection();
                }
                
                // deal cards to columns
                for (int intColumn=0; intColumn<7; intColumn++)
                {
                    this.Columns[intColumn] = new CardCollection();
                    this.FaceDownColumns[intColumn] = new CardCollection();
                    
                    // deal one card less than number of column face down
                    for (int intCount=0; intCount<intColumn; intCount++)
                    {
                        this.FaceDownColumns[intColumn].Add(this.Deck.RemoveTopCard());
                    }
                    // deal one card face up on each row
                    this.Columns[intColumn].Add(this.Deck.RemoveTopCard());
                }
                
                // remaining cards go into reserve
                this.TheReserve = new CardCollection();
                this.TheDiscard = new CardCollection();
                Card card = null;
                do
                {
                    card = this.Deck.RemoveTopCard();
                    if (card != null)
                    {
                        this.TheReserve.Add(card);
                    }
                }
                while (card != null);
                          
            }
            
            public override CardTableRegion Move(CardTableRegion regionFrom)
            {
                CardTableRegion regionTo = null;
                
                if (regionFrom.Type == CardTableRegionType.Reserve)
                {
                    regionTo = new CardTableRegion(CardTableRegionType.Discard, 0);
                    if (!Move(regionFrom, regionTo))
                    {
                        regionTo = null;
                    }
                }
                
                return regionTo;
            }
            
            public override bool Move(CardTableRegion regionFrom, 
                                      CardTableRegion regionTo)
            {
                bool blnMoved = false;
                switch (regionFrom.Type)
                {
                    case (CardTableRegionType.Column):
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Column):
                                blnMoved = 
                                    MoveColumnToColumn(regionFrom.Index, 
                                                       regionTo.Index);
                                break;
                            case (CardTableRegionType.Foundation):
                                blnMoved = 
                                    MoveColumnToFoundation(regionFrom.Index, 
                                                           regionTo.Index);
                                break;
                            default:
                                // illegal move
                                blnMoved = false;
                                break;
                        }
                        break;
                    case (CardTableRegionType.ColumnEnd) :
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Column):
                                blnMoved = 
                                    MoveColumnEndToColumn(regionFrom.Index, 
                                                          regionTo.Index);
                                break;
                            case (CardTableRegionType.Foundation):
                                blnMoved = 
                                    MoveColumnEndToFoundation(regionFrom.Index, 
                                                              regionTo.Index);
                                break;
                            default:
                                // illegal move
                                blnMoved = false;
                                break;
                        }
                        break;                        
                    case (CardTableRegionType.Discard):
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Column):
                                blnMoved = 
                                    MoveDiscardToColumn(regionTo.Index);
                                break;
                            case (CardTableRegionType.Foundation):
                                blnMoved = 
                                    MoveDiscardToFoundation(regionTo.Index);
                                break;
                            default:
                                // illegal move
                                blnMoved = false;
                                break;
                        }
                        break;
                    case (CardTableRegionType.Reserve):
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Discard):
                                blnMoved = 
                                    MoveReserveToDiscard();
                                break;
                            default:
                                // illegal move
                                blnMoved = false;
                                break;
                        }
                        break;
                    default:
                        // illegal move
                        blnMoved = false;
                        break;
                }
                return blnMoved;
            }     
            
            public override bool IsLegalMove(CardTableRegion regionFrom, 
                                             CardTableRegion regionTo)
            {
                bool blnLegalMove = false;
                switch (regionFrom.Type)
                {
                    case (CardTableRegionType.Column):
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Column):
                                blnLegalMove = 
                                    IsLegalMoveColumnToColumn(regionFrom.Index, 
                                                              regionTo.Index);
                                break;
                            case (CardTableRegionType.Foundation):
                                blnLegalMove = 
                                    IsLegalMoveColumnToFoundation(regionFrom.Index, 
                                                                  regionTo.Index);
                                break;
                            default:
                                // illegal move
                                blnLegalMove = false;
                                break;
                        }
                        break;
                    case (CardTableRegionType.ColumnEnd):
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Column):
                                blnLegalMove = 
                                    IsLegalMoveColumnEndToColumn(regionFrom.Index, 
                                                                 regionTo.Index);
                                break;
                            case (CardTableRegionType.Foundation):
                                blnLegalMove = 
                                    IsLegalMoveColumnEndToFoundation(regionFrom.Index, 
                                                                     regionTo.Index);
                                break;
                            default:
                                // illegal move
                                blnLegalMove = false;
                                break;
                        }
                        break;                        
                    case (CardTableRegionType.Discard):
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Column):
                                blnLegalMove = 
                                    IsLegalMoveDiscardToColumn(regionTo.Index);
                                break;
                            case (CardTableRegionType.Foundation):
                                blnLegalMove = 
                                    IsLegalMoveDiscardToFoundation(regionTo.Index);
                                break;
                            default:
                                // illegal move
                                blnLegalMove = false;
                                break;
                        }
                        break;
                    case (CardTableRegionType.Reserve):
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Discard):
                                blnLegalMove = 
                                    IsLegalMoveReserveToDiscard();
                                break;
                            default:
                                // illegal move
                                blnLegalMove = false;
                                break;
                        }
                        break;
                    default:
                        // illegal move
                        blnLegalMove = false;
                        break;
                }
                return blnLegalMove;
            }    
            
            public bool IsLegalMoveColumnToColumn(int intColumnFrom, int intColumnTo)
            {
                bool blnLegalMove = false;
                if (intColumnFrom >=0 && intColumnFrom < 7 && 
                    intColumnTo >=0 && intColumnTo < 7)
                {
                    Card topCardMoved = this.Column(intColumnFrom).First;
                    if (topCardMoved != null)
                    {
                        if ((this.Column(intColumnTo).Count == 0 && 
                             this.FaceDownColumn(intColumnTo).Count == 0 &&
                             topCardMoved.Rank == Rank.King) ||
                            (this.Column(intColumnTo).Count > 0 &&
                             topCardMoved.Rank+1 == 
                                this.Column(intColumnTo).Last.Rank &&
                             topCardMoved.Colour != 
                                this.Column(intColumnTo).Last.Colour))
                        {
                            // Legal move:
                            // moving column starting with a King to empty column 
                            // or to column with end card one rank greater then 
                            // and suit opposite colour to card being moved
                               
                            blnLegalMove = true;
                        }
                    }
                }
                
                return blnLegalMove;
            }
            
            private bool MoveColumnToColumn(int intColumnFrom, int intColumnTo)
            {
                bool blnMoved = false;
                
                if (IsLegalMoveColumnToColumn(intColumnFrom, intColumnTo))
                {
                    CardCollection movedCards = new CardCollection();
                    foreach (Card cardMoved in this.Column(intColumnFrom))
                    {
                        movedCards.Add(cardMoved);
                    }
                    
                    foreach (Card cardMoved in movedCards)
                    {
                        this.Column(intColumnFrom).Remove(cardMoved);
                        this.Column(intColumnTo).Add(cardMoved);
                    }
                    Card revealedCard = this.FaceDownColumn(intColumnFrom).Last;
                    if (revealedCard != null)
                    {
                        this.Column(intColumnFrom).Add(revealedCard);
                        this.FaceDownColumn(intColumnFrom).Remove(revealedCard);
                    }
                    blnMoved = true;
                }
                    
                return blnMoved;
            }
            
            public bool IsLegalMoveColumnEndToColumn(int intColumnFrom, int intColumnTo)
            {
                bool blnLegalMove = false;
                if (intColumnFrom >=0 && intColumnFrom < 7 && 
                    intColumnTo >=0 && intColumnTo < 7)
                {
                    if (this.Column(intColumnFrom).Count == 1)
                    {
                        // can only move column end if its only (face up) card in column
                        
                        blnLegalMove = 
                            IsLegalMoveColumnToColumn(intColumnFrom, intColumnTo);
                        
                    }
                }
                
                return blnLegalMove;
            }
            
            private bool MoveColumnEndToColumn(int intColumnFrom, int intColumnTo)
            {
                bool blnMoved = false;
                
                if (IsLegalMoveColumnEndToColumn(intColumnFrom, intColumnTo))
                {
                    blnMoved = MoveColumnToColumn(intColumnFrom, intColumnTo);
                }
                    
                return blnMoved;
            }
            
            public bool IsLegalMoveColumnToFoundation(int intColumnFrom,  
                                                      int intFoundationTo)
            {
                bool blnLegalMove = false;
                if (intColumnFrom >=0 && intColumnFrom < 7 && 
                    intFoundationTo >=0 && intFoundationTo < 4)
                {
                    // can only move single card columns to foundation
                    // (for multi-card need to move "column end")
                    if (this.Column(intColumnFrom).Count == 1)
                    {
                        blnLegalMove = 
                            IsLegalMoveColumnEndToFoundation(intColumnFrom, 
                                                             intFoundationTo);
                    }
                }
                
                return blnLegalMove;
            }
            
            private bool MoveColumnToFoundation(int intColumnFrom, 
                                                int intFoundationTo)
            {
                bool blnMoved = false;
                
                if (IsLegalMoveColumnToFoundation(intColumnFrom, 
                                                  intFoundationTo))
                {
                    blnMoved = MoveColumnEndToFoundation(intColumnFrom, 
                                                         intFoundationTo);
                }
                    
                return blnMoved;
            }
            
            public bool IsLegalMoveColumnEndToFoundation(int intColumnFrom, 
                                                         int intFoundationTo)
            {
                bool blnLegalMove = false;
                if (intColumnFrom >=0 && intColumnFrom < 7 && 
                    intFoundationTo >=0 && intFoundationTo < 4)
                {
                    Card cardMoved = this.Column(intColumnFrom).Last;
                    if (cardMoved != null)
                    {
                        if (((a_FoundationSuitMapping[intFoundationTo] == 
                              Suit.None ||
                              cardMoved.Suit == 
                              a_FoundationSuitMapping[intFoundationTo]) &&
                             cardMoved.Rank == Rank.Ace && 
                             this.Foundation(intFoundationTo).Count < 1) ||
                            (cardMoved.Suit == 
                              a_FoundationSuitMapping[intFoundationTo] &&
                              this.Foundation(intFoundationTo).Count > 0 &&
                              cardMoved.Rank == 
                                this.Foundation(intFoundationTo).Last.Rank+1))
                        
                        {
                            // Legal move:
                            // moving to foundation of same suit with top card one rank less
                            // than card being moved
                               
                            blnLegalMove = true;
                        }
                    }
                }
                
                return blnLegalMove;
            }
            
            private bool MoveColumnEndToFoundation(int intColumnFrom, int intFoundationTo)
            {
                bool blnMoved = false;
                
                if (IsLegalMoveColumnEndToFoundation(intColumnFrom, intFoundationTo))
                {
                    Card cardMoved = this.Column(intColumnFrom).Last;
                
                    if (cardMoved != null)
                    {            
                        // Legal move:
                        // moving to foundation of same suit with top card one rank less
                        // than card being moved
                        
                        this.Column(intColumnFrom).Remove(cardMoved);
                        this.Foundation(intFoundationTo).Add(cardMoved);
                        if (this.Column(intColumnFrom).Count < 1)
                        {
                            // reveal card below
                            Card revealedCard = this.FaceDownColumn(intColumnFrom).Last;
                            if (revealedCard != null)
                            {
                                this.Column(intColumnFrom).Add(revealedCard);
                                this.FaceDownColumn(intColumnFrom).Remove(revealedCard);
                            }
                        }
                        if (cardMoved.Rank == Rank.Ace)
                        {
                            // first card on foundation column so set suit
                            a_FoundationSuitMapping[intFoundationTo] = 
                                cardMoved.Suit;
                        }
                    
                        blnMoved = true;
                    }
                }
                    
                return blnMoved;
            }
            
            public bool IsLegalMoveDiscardToColumn(int intColumnTo)
            {
                bool blnLegalMove = false;
                
                if (intColumnTo >=0 && intColumnTo < 7)
                {
                    Card cardMoved = this.TheDiscard.Last;
                    if (cardMoved != null)
                    {
                        if ((this.Column(intColumnTo).Count == 0 && 
                             this.FaceDownColumn(intColumnTo).Count == 0 &&
                             cardMoved.Rank == Rank.King) ||
                            (this.Column(intColumnTo).Count > 0 &&
                             cardMoved.Rank+1 == this.Column(intColumnTo).Last.Rank &&
                             cardMoved.Colour != this.Column(intColumnTo).Last.Colour))
                        {
                            // Legal move:
                            // moving a King to empty column 
                            // or other rank to column with end card one rank greater then 
                            // and suit opposite colour to card being moved
                               
                            blnLegalMove = true;
                        }
                    }
                }
                
                return blnLegalMove;
            }
            
            private bool MoveDiscardToColumn(int intColumnTo)
            {
                bool blnMoved = false;
                
                if (IsLegalMoveDiscardToColumn(intColumnTo))
                {
                    Card cardMoved = this.TheDiscard.Last;
                    if (cardMoved != null)
                    {
                        this.TheDiscard.Remove(cardMoved);
                        this.Column(intColumnTo).Add(cardMoved);
                    }
                    blnMoved = true;
                }
                
                return blnMoved;
            }
            
            public bool IsLegalMoveDiscardToFoundation(int intFoundationTo)
            {
                bool blnLegalMove = false;
                if (intFoundationTo >=0 && intFoundationTo < 4)
                {
                    Card cardMoved = this.TheDiscard.Last;
                    if (cardMoved != null)
                    {
                        if (((a_FoundationSuitMapping[intFoundationTo] ==
                              Suit.None ||
                              cardMoved.Suit == 
                              a_FoundationSuitMapping[intFoundationTo]) &&
                             cardMoved.Rank == Rank.Ace && 
                             this.Foundation(intFoundationTo).Count < 1) ||
                             (cardMoved.Suit == 
                              a_FoundationSuitMapping[intFoundationTo] &&
                              this.Foundation(intFoundationTo).Count > 0 &&
                              cardMoved.Rank == 
                                this.Foundation(intFoundationTo).Last.Rank+1))
                        {
                            // Legal move:
                            // moving to foundation of same suit with top card one rank less
                            // than card being moved
                               
                            blnLegalMove = true;
                        }
                    }
                }
                
                return blnLegalMove;
            }
            
            private bool MoveDiscardToFoundation(int intFoundationTo)
            {
                bool blnMoved = false;
                
                if (IsLegalMoveDiscardToFoundation(intFoundationTo))
                {
                    Card cardMoved = this.TheDiscard.Last;
                
                    if (cardMoved != null)
                    {            
                        // Legal move:
                        // moving to foundation of same suit with top card one rank less
                        // than card being moved
                        
                        this.TheDiscard.Remove(cardMoved);
                        this.Foundation(intFoundationTo).Add(cardMoved);
                    
                        if (cardMoved.Rank == Rank.Ace)
                        {
                            // first card on foundation column so set suit
                            a_FoundationSuitMapping[intFoundationTo] = 
                                cardMoved.Suit;
                        }
                    
                        blnMoved = true;
                    }
                }
                    
                return blnMoved;
            }
            
            public bool IsLegalMoveReserveToDiscard()
            {
                bool blnLegalMove = false;
                if (this.TheReserve.Count > 0 || this.TheDiscard.Count > 0)
                {
                    blnLegalMove = true;
                }
                
                return blnLegalMove;
            }
            
            private bool MoveReserveToDiscard()
            {
                bool blnMoved = false;
                
                if (IsLegalMoveReserveToDiscard())
                {
                    if (this.TheReserve.Count == 0)
                    {
                        // move discard back to reserve
                        for (int intIndex=this.TheDiscard.Count-1; intIndex>=0; intIndex--)
                        {
                            Card card = this.TheDiscard[intIndex];
                            this.TheReserve.Add(card);
                            this.TheDiscard.Remove(card);
                        }
                    }
                    // move top three cards of reserve to discard
                    // first card is placed at top of discard
                    CardCollection discarded = new CardCollection();
                    for (int intCount=2; intCount>=0; intCount--)
                    {
                        int intIndex = this.TheReserve.Count-1-intCount;
                        if (intIndex >= 0 && intIndex < this.TheReserve.Count)
                        {
                            Card card = this.TheReserve[intIndex];
                            if (card != null)
                            {
                                discarded.Add(card);
                                //this.TheReserve.Remove(card);
                            }
                        }
                    }
                    foreach (Card card in discarded)
                    {
                        this.TheDiscard.Add(card);
                        this.TheReserve.Remove(card);
                    }
                    blnMoved = true;
                }
                
                return blnMoved;
            }
                        
            public override bool IsGameWon()
            {
                bool blnGameWon = true;
                
                for (int intIndex = 0; intIndex<4 && blnGameWon; intIndex++)
                {
                    if (this.Foundation(intIndex) == null || 
                        this.Foundation(intIndex).Last == null ||
                        this.Foundation(intIndex).Last.Rank != Rank.King)
                    {
                        blnGameWon = false;
                    }
                }
                
                return blnGameWon;
            } 
            
            public CardCollection[] FaceDownColumns
            {
                get
                {
                    return m_FaceDownColumns;
                }
                set
                {
                    m_FaceDownColumns = value;
                }
            }     
            
            public CardCollection FaceDownColumn(int intColumn)
            {
                if (intColumn >=0 && intColumn < m_FaceDownColumns.Length)
                {
                    return m_FaceDownColumns[intColumn];
                }
                return null;
            }
        }
    }
}