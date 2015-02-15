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
 * File:          CardTableZones.cs                                      *
 * Namespace:     SwSt.CardGames                                         *
 * Last modified: 25 August 2003                                         *
 * Description:   Simple classes to check overlap etc. of regions of 2D  *
 *                space.                                                 *
 *************************************************************************
 */

using System;
using System.Drawing;
using System.Collections;
using SwSt;
using SwSt.CardGames;

namespace SwSt
{
    namespace CardGames
    {
        // ------------------------------------------------------ //
        
        public enum CardTableZoneType
        {
            PickUpZone = 1,
            DropZone   = 2,
            ClickZone  = 3,
        }
        
        public class CardTableZone : Zone
        {
            private CardTableRegion   m_Region;
            private CardTableZoneType m_Type;
            
            public CardTableZone(CardTableZoneType type, CardTableRegion region, 
                                 int intX, int intY, int intWidth, int intHeight)
                : base(region.Index, "", "", intX, intY, intWidth, intHeight)
            {
                this.Type   = type;
                this.Region = region;
            }
            
            public CardTableZone(CardTableZoneType type, CardTableRegion region,
                                 Point location, Size size) :
                this(type, region, 
                     location.X, location.Y, size.Width, size.Height)
            {
            }
            
            public CardTableRegion Region
            {
                get
                {
                    return m_Region;
                }
                set
                {
                    m_Region = value;
                }
            }
            
            public new CardTableZoneType Type
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
            
            public bool IsEqualTo(CardTableZone zone)
            {
                bool blnEqual = false;
                if (zone.Type == this.Type && zone.X == this.X &&
                    zone.Y == this.Y && zone.Width == this.Width &&
                    zone.Height == this.Height)
                {
                    blnEqual = true;
                }
                return blnEqual;
            }
        }
        
        public class CardTableZones : Zones
        {
            public CardTableZones()
            {
            }
            
            public new CardTableZone this[int intIndex]
            {
                get
                {
                    return (CardTableZone)List[intIndex];
                }
                set
                {
                    List[intIndex] = value;
                }
            }
            
            // get collection of zones which contain the specified point
            // (collection allows for overlapping zones)
            public new CardTableZones WhichZone(int intX, int intY)
            {
                CardTableZones colZones = new CardTableZones();
                foreach (CardTableZone objZone in this)
                {
                    if (intX >= objZone.X && intY >= objZone.Y &&
                        intX < objZone.X+objZone.Width &&
                        intY < objZone.Y+objZone.Height)
                    {
                        colZones.Add(objZone);
                    }
                } 
                 
                return colZones;
            }
            
            public CardTableZone GetZoneByTypeAndRegion(CardTableZoneType type, CardTableRegion region)
            {
                Zone objZone = null;
                int intZone = 0;
                while(objZone == null && intZone < this.Count)
                {
                    if (this[intZone].Type == type &&
                        this[intZone].Region.Type  == region.Type && 
                        this[intZone].Region.Index == region.Index)
                    {
                        objZone = this[intZone];
                    }
                    intZone++;
                }
                
                return (CardTableZone)objZone;
            }
        }
    }
}