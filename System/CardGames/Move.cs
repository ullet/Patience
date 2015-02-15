/*
 *************************************************************************
 * Copyright (C) 2003 Trevor Barnett                                     *
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
 * File:          Move.cs                                                *
 * Namespace:     SwSt.CardGames                                         *
 * Last modified: 12 September 2003                                      *
 * Class:         Move                                                   *
 * Description:   Class to represent a move in a patience card game.     *
 *************************************************************************
 */

namespace SwSt
{
    namespace CardGames
    {    
        public class Move
        {
            private CardTableRegion m_RegionFrom;
            private CardTableRegion m_RegionTo;
            
            public Move()
            {
            }
            
            public Move(CardTableRegion regionFrom, CardTableRegion regionTo)
            {
                this.From = regionFrom;
                this.To   = regionTo;
            }
            
            public Move(CardTableRegionType fromType, int intFromIndex, 
                        CardTableRegionType toType,   int intToIndex)
            {
                this.From = new CardTableRegion(fromType, intFromIndex);
                this.To   = new CardTableRegion(toType,   intToIndex);
            }
            
            public CardTableRegion From
            {
                get
                {
                    return m_RegionFrom;
                }
                set
                {
                    m_RegionFrom = value;
                }
            }
            
            public CardTableRegion To
            {
                get
                {
                    return m_RegionTo;
                }
                set
                {
                    m_RegionTo = value;
                }
            }
        }
    }
}