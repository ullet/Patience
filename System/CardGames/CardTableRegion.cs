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
 * File:          CardTableRegion.cs                                     *
 * Namespace:     SwSt.CardGames                                         *
 * Last modified: 12 September 2003                                      *
 * Class:         CardTableRegion                                        *
 * Description:   Class used to indentify special areas on a card table. *
 *************************************************************************
 */

namespace SwSt
{
    namespace CardGames
    {
        public enum CardTableRegionType
        {
            Row        = 1,
            Foundation = 2,
            Reserve    = 3,
            Discard    = 4,
            Column     = 5,
            RowEnd     = 6,
            ColumnEnd  = 7,
        }
    
        public class CardTableRegion
        {
            private CardTableRegionType m_Type;
            private int                 m_intIndex;
            
            public CardTableRegion()
            {
            }
            
            public CardTableRegion(CardTableRegionType type, int intIndex)
            {
                this.Type  = type;
                this.Index = intIndex;
            }        
            
            public CardTableRegionType Type
            {
                get
                {
                    return m_Type;
                }
                set
                {
                    m_Type = value;
                }
            }
            
            public int Index
            {
                get
                {
                    return m_intIndex;
                }
                set
                {
                    m_intIndex = value;
                }
            } 
        }
    }
}