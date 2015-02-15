/*
 *************************************************************************
 * Patience: An extensible graphical card game system.                   *
 * Copyright (C) 2003, 2005, 2015 Trevor Barnett <mr.ullet@gmail.com>    *
 *                                                                       * 
 * Released under the terms of the GNU General Public License, version 2.*
 * See file LICENSE for full details                                     *
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
       public class BeleagueredCastle : CardGame
        {
            private CardCollection m_Aces; 
            private CardCollection m_NotAces; 
            
            
            public BeleagueredCastle()
            {
                this.Rows        = new CardCollection[8];
                this.Foundations = new CardCollection[4];
                
                // new deck of cards
                this.Deck = new Deck();
            }            
            
            public BeleagueredCastle(CardCollection playingCards) : 
                this()
            {
                this.Initialise(playingCards);
                
                // start new game
                Restart();
            }
            
            public BeleagueredCastle(CardCollection playingCards, int intRandomSeed) : 
                this()
            {
                this.Initialise(playingCards, intRandomSeed);
                
                // start new game
                Restart();
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
                    
                    // seperate Aces from rest of deck
                    m_Aces    = new CardCollection(); 
                    m_NotAces = new CardCollection();
                    
                    Deck deck = new Deck(playingCards);
                    for (int intIndex=0; intIndex<4; intIndex++)
                    {
                        Card card = deck.Card((Suit)intIndex, Rank.Ace);
                        if (card != null)
                        {
                            m_Aces.Add(card);
                        }
                        for (int intRank=2; intRank<=13; intRank++)
                        {
                            card = deck.Card((Suit)intIndex, (Rank)intRank);
                            if (card != null)
                            {
                                m_NotAces.Add(card);
                            }
                        }
                    }
                    
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
                    
                    // set rows
                                    
                    for (int intRow=0; intRow<8; intRow++)
                    {
                        if (this.Row(intRow) != null)
                        {
                            CardCollection row = new CardCollection();
                            foreach (Cards.Card card in this.Row(intRow))
                            {
                                row.Add(deck.Card(card.Suit, card.Rank));
                            }
                            this.Rows[intRow] = row;
                            row = null;
                        }
                    }
                }
            }                
            
            public override void GatherCards()
            {
                this.Deck.Cards = m_NotAces;
            }
            
            public override void Deal()
            {
                // initialise foundations
                for (int intIndex=0; intIndex<4; intIndex++)
                {
                    this.Foundations[intIndex] = new CardCollection();
                    this.Foundations[intIndex].Add(m_Aces[intIndex]);
                }
                
                // deal remaining cards to rows of tableau
                for (int intRow=0; intRow<8; intRow++)
                {
                    this.Rows[intRow] = new CardCollection();
                    
                    for (int intCount=0; intCount<6; intCount++)
                    {
                        Card topCard = this.Deck.RemoveTopCard();
                        this.Rows[intRow].Add(topCard);
                    }
                }      
            }
            
            public override CardTableRegion Move(CardTableRegion regionFrom)
            {
                return null;
            }
            
            public override bool Move(CardTableRegion regionFrom, CardTableRegion regionTo)
            {
                bool blnLegalMove = false;
                switch (regionFrom.Type)
                {
                    case (CardTableRegionType.Row):
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Row):
                                blnLegalMove = MoveRowToRow(regionFrom.Index, regionTo.Index);
                                break;
                            case (CardTableRegionType.Foundation):
                                blnLegalMove = MoveRowToFoundation(regionFrom.Index, regionTo.Index);
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
            
            public override bool IsLegalMove(CardTableRegion regionFrom, CardTableRegion regionTo)
            {
                bool blnLegalMove = false;
                switch (regionFrom.Type)
                {
                    case (CardTableRegionType.Row):
                        switch (regionTo.Type)
                        {
                            case (CardTableRegionType.Row):
                                blnLegalMove = 
                                    IsLegalMoveRowToRow(regionFrom.Index, regionTo.Index);
                                break;
                            case (CardTableRegionType.Foundation):
                                blnLegalMove = 
                                    IsLegalMoveRowToFoundation(regionFrom.Index, regionTo.Index);
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
            
            public bool IsLegalMoveRowToRow(int intRowFrom, int intRowTo)
            {
                bool blnLegalMove = false;
                if (intRowFrom >=0 && intRowFrom < 8 && intRowTo >=0 && intRowTo < 8)
                {
                    Card cardMoved = this.Row(intRowFrom).Last;
                    if (cardMoved != null)
                    {
                        if (this.Row(intRowTo).Last == null ||
                                cardMoved.Rank+1 == this.Row(intRowTo).Last.Rank)
                        {
                            // Legal move:
                            // moving to empty row or to row with end card one rank greater
                            // than card being moved (suit not important)
                               
                            blnLegalMove = true;
                        }
                    }
                }
                
                return blnLegalMove;
            }
            
            private bool MoveRowToRow(int intRowFrom, int intRowTo)
            {
                bool blnMoved = false;
                Card cardMoved = this.Row(intRowFrom).Last;
                
                if (cardMoved != null)
                {            
                    if (IsLegalMoveRowToRow(intRowFrom, intRowTo))
                    {
                        // Legal move:
                        // moving to empty row or to row with end card one rank greater
                        // than card being moved (suit not important)
                        
                        this.Row(intRowFrom).Remove(cardMoved);
                        this.Row(intRowTo).Add(cardMoved);
                    
                        blnMoved = true;
                    }
                }
                    
                return blnMoved;
            }
            
            public bool IsLegalMoveRowToFoundation(int intRowFrom, int intFoundationTo)
            {
                bool blnLegalMove = false;
                if (intRowFrom >=0 && intRowFrom < 8 && intFoundationTo >=0 && intFoundationTo < 4)
                {
                    Card cardMoved = this.Row(intRowFrom).Last;
                    if (cardMoved != null)
                    {
                        if (cardMoved.Rank == this.Foundation(intFoundationTo).Last.Rank+1 &&
                            cardMoved.Suit == this.Foundation(intFoundationTo).Last.Suit)
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
            
            private bool MoveRowToFoundation(int intRowFrom, int intFoundationTo)
            {
                bool blnMoved = false;
                Card cardMoved = this.Row(intRowFrom).Last;
                
                if (cardMoved != null)
                {            
                    if (IsLegalMoveRowToFoundation(intRowFrom, intFoundationTo))
                    {
                        // Legal move:
                        // moving to foundation of same suit with top card one rank less
                        // than card being moved
                        
                        this.Row(intRowFrom).Remove(cardMoved);
                        this.Foundation(intFoundationTo).Add(cardMoved);
                    
                        blnMoved = true;
                    }
                }
                    
                return blnMoved;
            }
            
            public override bool IsGameWon()
            {
                bool blnGameWon = true;
                
                for (int intIndex = 0; intIndex<4; intIndex++)
                {
                    blnGameWon &= this.Foundation(intIndex).Last.Rank == Rank.King;
                }
                
                return blnGameWon;
            }                
        }
    }
}