/*
 *************************************************************************
 * Patience: An extensible graphical card game system.                   *
 * Copyright (C) 2003, 2005, 2015 Trevor Barnett <mr.ullet@gmail.com>    *
 *                                                                       * 
 * Released under the terms of the GNU General Public License, version 2.*
 * See file LICENSE for full details                                     *
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