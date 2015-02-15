/*
 *************************************************************************
 * Patience: An extensible graphical card game system.                   *
 * Copyright (C) 2003, 2005, 2015 Trevor Barnett <mr.ullet@gmail.com>    *
 *                                                                       * 
 * Released under the terms of the GNU General Public License, version 2.*
 * See file LICENSE for full details                                     *
 *************************************************************************
 */

using System.Drawing;
using System.Collections;

namespace SwSt
{
    // ------------------------------------------------------ //
    
    public class Zone
    {
        private string m_strName;
        private string m_strType;
        private int    m_intValue;
        private int    m_intX;
        private int    m_intY;
        private int    m_intWidth;
        private int    m_intHeight;
        
        public Zone(int intValue, string strName, string strType,
                    Point location, Size size) :
            this(intValue, strName, strType, 
                 location.X, location.Y, size.Width, size.Height)
        {
        }
        
        public Zone(int intValue, string strName, string strType,
                    int intX, int intY, int intWidth, int intHeight)
        {
            this.Value  = intValue;
            this.Name   = strName;
            this.Type   = strType;
            this.X      = intX;
            this.Y      = intY;
            this.Width  = intWidth;
            this.Height = intHeight;
        }
        
        public int Value
        {
            get
            {
                return m_intValue;
            }
            set
            {
                m_intValue = value;
            }
        }
        
        public string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
            }
        }
        
        public string Type
        {
            get
            {
                return m_strType;
            }
            set
            {
                m_strType = value;
            }
        }
        
        public int X
        {
            get
            {
                return m_intX;
            }
            set
            {
                m_intX = value;
            }
        }
        
        public int Y
        {
            get
            {
                return m_intY;
            }
            set
            {
                m_intY = value;
            }
        }
        
        public int Width
        {
            get
            {
                return m_intWidth;
            }
            set
            {
                m_intWidth = value;
            }
        }
        
        public int Height
        {
            get
            {
                return m_intHeight;
            }
            set
            {
                m_intHeight = value;
            }
        }
        
        public Point Location
        {
            get
            {
                return new Point(this.X, this.Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        
        public Size Size
        {
            get
            {
                return new Size(this.Width, this.Height);
            }
            set
            {
                this.Width  = value.Width;
                this.Height = value.Height;
            }
        }
        
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(this.X, this.Y, this.Width, this.Height);
            }
            set
            {
                this.X      = value.X;
                this.Y      = value.Y;
                this.Width  = value.Width;
                this.Height = value.Height;
            }
        }
    }
    
    // ------------------------------------------------------ //
    
    public class Zones : CollectionBase
    {
        public Zones()
        {
        }
        
        public void Add(Zone objZone)
        {
            List.Add(objZone);
        }
        
        public void Remove(Zone objZone)
        {
            List.Remove(objZone);
        }
        
        public Zone this[int intIndex]
        {
            get
            {
                return (Zone)List[intIndex];
            }
            set
            {
                List[intIndex] = value;
            }
        }
        
        // get collection of zones which contain the specified point
        // (collection allows for overlapping zones)
        public Zones WhichZone(int intX, int intY)
        {
            Zones colZones = new Zones();
            foreach (Zone objZone in this)
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
        
        // get first zone that matches value
        public Zone GetZoneByValue(int intValue)
        {
            Zone objZone = null;
            int intZone = 0;
            while(objZone == null && intZone < this.Count)
            {
                if (this[intZone].Value == intValue)
                {
                    objZone = this[intZone];
                }
                intZone++;
            }
            
            return objZone;
        }
        
        // get first zone that matches name
        public Zone GetZoneByName(string strName)
        {
            Zone objZone = null;
            int intZone = 0;
            while(objZone == null && intZone < this.Count)
            {
                if (this[intZone].Name == strName)
                {
                    objZone = this[intZone];
                }
                intZone++;
            }
            
            return objZone;
        }
    }
}