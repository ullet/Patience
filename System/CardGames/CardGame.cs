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

namespace SwSt
{
    namespace CardGames
    {
        public abstract class CardGame
        {
            protected CardCollection   m_PlayingCards;
            protected CardCollection[] m_Rows;
            protected CardCollection[] m_Columns;
            protected CardCollection[] m_Foundations;
            protected CardCollection[] m_Reserves;
            protected CardCollection[] m_Discards;
            protected Deck             m_Deck;
            
            // Random seed for shuffling deck. 
            // If -1 continue using existing generator.
            // If 0 then new randomized generator is used.  
            // If positive value then seed is used to initialise new generator.
            // The same seed value will always produce the same sequence.
            protected int              m_intRandomSeed = -1;        
            
            public void Initialize(CardCollection playingCards)
            {
                this.Initialise(playingCards, m_intRandomSeed);
            }
            
            public void Initialize(CardCollection playingCards, int intRandomSeed)
            {
                this.Initialise(playingCards, intRandomSeed);
            }
            
            public void Initialise(CardCollection playingCards)
            {
                this.Initialise(playingCards, m_intRandomSeed);
            }
            
            public virtual void Initialise(CardCollection playingCards, int intRandomSeed)
            {
                this.PlayingCards = playingCards;
                this.RandomSeed   = intRandomSeed;
            }
            
            public virtual CardCollection PlayingCards
            {
                get
                {
                    return m_PlayingCards;
                }
                set
                {
                    m_PlayingCards = value;
                }
            }
            
            public virtual void Restart()
            {
                GatherCards();
                Shuffle();
                Deal();
            }
            
            public abstract void GatherCards();
            
            public virtual void Shuffle()
            {
                // sort deck first so can be certain order is same everytime,
                // ensuring a non-zero seed value will always give same order
                m_Deck.Sort();
                m_Deck.Shuffle(m_intRandomSeed);
            }
            
            public abstract void Deal();
            
            public virtual bool Move(Move move)
            {
                return Move(move.From, move.To);
            }
            
            public abstract bool IsLegalMove(CardTableRegion regionFrom, CardTableRegion regionTo);
            
            public abstract CardTableRegion Move(CardTableRegion regionFrom);
            
            public abstract bool Move(CardTableRegion regionFrom, CardTableRegion regionTo);
            
            public abstract bool IsGameWon();
                
            public virtual CardCollection[] Rows
            {
                get
                {
                    return m_Rows;
                }
                set
                {
                    m_Rows = value;
                }
            }
            
            public virtual CardCollection[] Columns
            {
                get
                {
                    return m_Columns;
                }
                set
                {
                    m_Columns = value;
                }
            }
            
            public virtual CardCollection[] Foundations
            {
                get
                {
                    return m_Foundations;
                }
                set
                {
                    m_Foundations = value;
                }
            }
            
            public virtual CardCollection[] Reserves
            {
                get
                {
                    return m_Reserves;
                }
                set
                {
                    m_Reserves = value;
                }
            }
            
            public virtual CardCollection[] Discards
            {
                get
                {
                    return m_Discards;
                }
                set
                {
                    m_Discards = value;
                }
            }
            
            public virtual CardCollection TheReserve
            {
                get
                {
                    if (m_Reserves == null)
                    {
                        m_Reserves = new CardCollection[1];
                    }
                    return m_Reserves[0];
                }
                set
                {
                    if (m_Reserves == null)
                    {
                        m_Reserves = new CardCollection[1];
                    }
                    m_Reserves[0] = value;
                }
            }
            
            public virtual CardCollection TheDiscard
            {
                get
                {
                    if (m_Discards == null)
                    {
                        m_Discards = new CardCollection[1];
                    }
                    return m_Discards[0];
                }
                set
                {
                    if (m_Discards == null)
                    {
                        m_Discards = new CardCollection[1];
                    }
                    m_Discards[0] = value;
                }
            }
            
            public virtual Deck Deck
            {
                get
                {
                    return m_Deck;
                }
                set
                {
                    m_Deck = value;
                }
            }
            
            public virtual int RandomSeed
            {
                get
                {
                    return m_intRandomSeed;
                }
                set 
                {
                    m_intRandomSeed = value;
                }
            }
            
            public virtual CardCollection Row(int intRow)
            {
                if (m_Rows != null && intRow >=0 && intRow < m_Rows.Length)
                {
                    return m_Rows[intRow];
                }
                return null;
            }
            
            public virtual CardCollection Column(int intColumn)
            {
                if (m_Columns != null && intColumn >=0 && intColumn < m_Columns.Length)
                {
                    return m_Columns[intColumn];
                }
                return null;
            }
            
            public virtual CardCollection Foundation(int intFoundation)
            {
                if (m_Foundations != null && intFoundation >=0 && 
                    intFoundation < m_Foundations.Length)
                {
                    return m_Foundations[intFoundation];
                }
                return null;
            }
            
            public virtual CardCollection Reserve(int intReserve)
            {
                if (m_Reserves != null && intReserve >=0 && 
                    intReserve < m_Reserves.Length)
                {
                    return m_Reserves[intReserve];
                }
                return null;
            }
            
            public virtual CardCollection Discard(int intDiscard)
            {
                if (m_Discards != null && intDiscard >=0 && 
                    intDiscard < m_Discards.Length)
                {
                    return m_Discards[intDiscard];
                }
                return null;
            }
        }        
    }
}