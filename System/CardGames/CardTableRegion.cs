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