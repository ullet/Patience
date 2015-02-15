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
 * File:          Background.cs                                          *
 * Namespace:     SwSt                                                   *
 * Last modified: 21 September 2003                                      *
 * Class:         Background                                             *
 * Description:   Class to hold background image, pattern or colour      *
 *                preferences.                                           *
 *************************************************************************
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SwSt;

namespace SwSt
{    
    public enum BackgroundStyle
    {
        SolidColour = 1,
        Gradient    = 2,
        Bitmap      = 3,
    }
    
    public enum GradientDirection
    {
        LeftToRight          =  1,
        RightToLeft          =  2,
        TopToBottom          =  3,
        BottomToTop          =  4,
        TopLeftToBottomRight =  5,
        BottomLeftToTopRight =  6,
        TopRightToBottomLeft =  7,
        BottomRightToTopLeft =  8,
        Inwards              =  9,
        Outwards             = 10,
    }
    
    public class Background
    {
        private Color               m_BGColour           = Color.White;
        private GradientDirection   m_GradientDirection = 
            GradientDirection.LeftToRight;
        private Color[]             m_GradientColours    = 
            {Color.Black, Color.White};
        private Bitmap              m_Bitmap             = null;
        private Brush               m_BGBrush            = null;
        private bool                m_blnTile            = true;
        private bool                m_blnStretch         = false;
        private bool                m_blnCentre          = false;
        private bool                m_blnKeepAspectRatio = true;
        private bool                m_blnFillWithGradient= false;
        private BackgroundStyle     m_Style              = BackgroundStyle.SolidColour;
        private Size                m_Size               = new Size(100,100);
    
        public Background()
        {
        }
        
        public Background(Size size) : base()
        {
            this.Size = size;
        }
        
        public Background(Color colour) : base()
        {
            this.Colour = colour;
            this.Style = BackgroundStyle.SolidColour;
        }
        
        public Background(Color colour, Size size) : this(size)
        {
            this.Colour = colour;
            this.Style = BackgroundStyle.SolidColour;
        }
        
        public Background(GradientDirection direction) : base()
        {
            this.GradientDirection = direction;
            this.Style = BackgroundStyle.Gradient;
        }
        
        public Background(GradientDirection direction, Size size) : this(size)
        {
            this.GradientDirection = direction;
            this.Style = BackgroundStyle.Gradient;
        }
        
        public Background(Bitmap bmp) : base()
        {
            this.Bitmap = bmp;
            this.Style = BackgroundStyle.Bitmap;
        }
        
        public Background(Bitmap bmp, Size size) : this(size)
        {
            this.Bitmap = bmp;
            this.Style = BackgroundStyle.Bitmap;
        }
        
        public Color Colour
        {
            get
            {
                return m_BGColour;
            }
            set
            {
                m_BGColour = value;
                m_BGBrush = null;
            }
        }
        
        public Color[] Colours
        {
            get
            {
                return m_GradientColours;
            }
            set
            {
                m_GradientColours[0] = value[0];
                m_GradientColours[1] = value[1];
                m_BGBrush = null;
            }
        }
        
        public GradientDirection GradientDirection
        {
            get
            {
                return m_GradientDirection;
            }
            set
            {
                m_GradientDirection = value;
                m_BGBrush = null;
            }
        }
        
        public Bitmap Bitmap
        {
            get
            {
                return m_Bitmap;
            }
            set
            {
                m_Bitmap = value;
                m_BGBrush = null;
            }
        }
        
        public BackgroundStyle Style
        {
            get
            {
                return m_Style;
            }
            set
            {
                m_Style = value;
                m_BGBrush = null;
            }
        }
        
        public Size Size
        {
            get
            {
                return m_Size;
            }
            set
            {
                m_Size.Width  = value.Width;
                m_Size.Height = value.Height;
                m_BGBrush = null;
            }
        }
        
        public bool Tile
        {
            get
            {
                return m_blnTile;
            }
            set
            {
                m_blnTile = value;
                m_BGBrush = null;
            }
        }
        
        public bool Centre
        {
            get
            {
                return m_blnCentre;
            }
            set
            {
                m_blnCentre = value;
                m_BGBrush = null;
            }
        }
        
        public bool Stretch
        {
            get
            {
                return m_blnStretch;
            }
            set
            {
                m_blnStretch = value;
                m_BGBrush = null;
            }
        }
        
        public bool KeepAspectRatio
        {
            get
            {
                return m_blnKeepAspectRatio;
            }
            set
            {
                m_blnKeepAspectRatio = value;
                m_BGBrush = null;
            }
        }
        
        public bool FillWithGradient
        {
            get
            {
                return m_blnFillWithGradient;
            }
            set
            {
                m_blnFillWithGradient = value;
                m_BGBrush = null;
            }
        }
        
        public Brush SolidBrush
        {
            get
            {
                return new SolidBrush(m_BGColour);
            }
        }
        
        public Brush GradientBrush
        {
            get
            {
                Brush ReturnBrush = null;
                Rectangle lgrec = new Rectangle(
                    0, 0, Size.Width, Size.Height);
                PointF[]  path = 
                    new PointF[]
                        {new PointF(0,0),
                         new PointF(lgrec.Width, 0),
                         new PointF(lgrec.Width, lgrec.Height),
                         new PointF(0, lgrec.Height)};
                PathGradientBrush pgb = 
                    new PathGradientBrush(path);
                pgb.CenterPoint = new Point(
                    Size.Width/2, Size.Height/2);
                        
                switch (m_GradientDirection)
                {
                    case GradientDirection.LeftToRight:
                        ReturnBrush = new LinearGradientBrush(
                            lgrec, m_GradientColours[0],
                            m_GradientColours[1], 
                            LinearGradientMode.Horizontal);
                        break;
                    case GradientDirection.RightToLeft:
                        ReturnBrush = new LinearGradientBrush(
                            lgrec, m_GradientColours[1],
                            m_GradientColours[0], 
                            LinearGradientMode.Horizontal);
                        break;
                    case GradientDirection.TopToBottom:
                        ReturnBrush = new LinearGradientBrush(
                            lgrec, m_GradientColours[0],
                            m_GradientColours[1], 
                            LinearGradientMode.Vertical);
                        break;
                    case GradientDirection.BottomToTop:
                        ReturnBrush = new LinearGradientBrush(
                            lgrec, m_GradientColours[1],
                            m_GradientColours[0], 
                            LinearGradientMode.Vertical);
                        break;
                    case GradientDirection.TopLeftToBottomRight:
                        ReturnBrush = new LinearGradientBrush(
                            lgrec, m_GradientColours[0],
                            m_GradientColours[1], 
                            LinearGradientMode.ForwardDiagonal);
                        break;
                    case GradientDirection.TopRightToBottomLeft:
                        ReturnBrush = new LinearGradientBrush(
                            lgrec, m_GradientColours[0],
                            m_GradientColours[1], 
                            LinearGradientMode.BackwardDiagonal);
                        break;
                    case GradientDirection.BottomLeftToTopRight:
                        ReturnBrush = new LinearGradientBrush(
                            lgrec, m_GradientColours[1],
                            m_GradientColours[0], 
                            LinearGradientMode.BackwardDiagonal);
                        break;
                    case GradientDirection.BottomRightToTopLeft:
                        ReturnBrush = new LinearGradientBrush(
                            lgrec, m_GradientColours[1],
                            m_GradientColours[0], 
                            LinearGradientMode.ForwardDiagonal);
                        break;
                    case GradientDirection.Inwards:
                        pgb.CenterColor = m_GradientColours[1];
                        pgb.SurroundColors = new Color[]
                          {m_GradientColours[0], m_GradientColours[0], 
                           m_GradientColours[0], m_GradientColours[0]};
                        ReturnBrush = pgb;
                        break;
                    case GradientDirection.Outwards:
                        pgb.CenterColor = m_GradientColours[0];
                        pgb.SurroundColors = new Color[]
                          {m_GradientColours[1], m_GradientColours[1], 
                           m_GradientColours[1], m_GradientColours[1]};
                        ReturnBrush = pgb;
                        break;
                    default:
                        ReturnBrush = this.SolidBrush;
                        break;
                }
                
                return ReturnBrush;
            }
        }
        
        public Brush Brush
        {
            get
            {
                if (m_BGBrush == null)
                {
                    switch (m_Style)
                    {
                        case BackgroundStyle.SolidColour:
                            m_BGBrush = this.SolidBrush;
                            break;
                        case BackgroundStyle.Gradient:
                            m_BGBrush = this.GradientBrush;
                            break;
                        case BackgroundStyle.Bitmap:
                            if (m_Bitmap == null)
                            {
                                if (m_blnFillWithGradient)
                                {
                                    m_BGBrush = this.GradientBrush;
                                }
                                else
                                {
                                    m_BGBrush = this.SolidBrush;
                                }
                            }
                            else
                            {
                                Bitmap bmpBG = new Bitmap(m_Bitmap);
                                
                                if (m_blnStretch)
                                {
                                    int intWidth;
                                    int intHeight;
                                    
                                    if (m_blnKeepAspectRatio)
                                    {
                                        double dblRatio = (double)m_Bitmap.Width/
                                            (double)m_Bitmap.Height;
                                        double dblBGRatio = (double)m_Size.Width/
                                            (double)m_Size.Height;
                                        if (dblBGRatio>dblRatio)
                                        {
                                            intWidth = (int)(m_Size.Height*dblRatio);
                                            intHeight = m_Size.Height;
                                        }
                                        else
                                        {
                                            intWidth = m_Size.Width;
                                            intHeight = (int)(m_Size.Width/dblRatio);
                                        }
                                    }
                                    else
                                    {
                                        intWidth = m_Size.Width;
                                        intHeight = m_Size.Height;
                                    }
                                    Bitmap bmp = new Bitmap(m_Bitmap, 
                                        intWidth+
                                            Math.Max(0,
                                                (intWidth/m_Bitmap.Width-1)), 
                                        intHeight+
                                            Math.Max(0,
                                                (intHeight/m_Bitmap.Height-1)));
                                    bmpBG = new Bitmap(
                                        intWidth,
                                        intHeight);
                                    Graphics g = Graphics.FromImage(bmpBG);
                                    g.DrawImage(bmp, 0, 0);
                                    g.Dispose();
                                    bmp.Dispose();
                                }
                                if ((m_blnCentre || 
                                     (m_blnStretch && m_blnKeepAspectRatio)) && 
                                    (m_Size.Width != bmpBG.Width ||
                                     m_Size.Height != bmpBG.Height))
                                {
                                    Bitmap bmpTmp = new Bitmap(m_Size.Width, 
                                                               m_Size.Height);
                                    Graphics g = Graphics.FromImage(bmpTmp);
                                    if (m_blnFillWithGradient)
                                    {
                                        g.FillRectangle(this.GradientBrush,
                                            0, 0, m_Size.Width, m_Size.Height);
                                    }
                                    else
                                    {
                                        g.FillRectangle(this.SolidBrush,
                                            0, 0, m_Size.Width, m_Size.Height);
                                    }
                                    g.DrawImage(bmpBG, 
                                        (m_Size.Width-bmpBG.Width)/2, 
                                        (m_Size.Height-bmpBG.Height)/2);
                                    g.Dispose();
                                    bmpBG.Dispose();
                                    bmpBG = bmpTmp;
                                }   
                                
                                m_BGBrush = new TextureBrush(bmpBG);
                            }
                            break;
                        default:
                            m_BGBrush = new SolidBrush(m_BGColour);
                            break;
                    }
                }
                
                return m_BGBrush;
            }
        }
    }
}