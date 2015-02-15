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
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace SwSt.WindowsForms.Dialogs
{
    public class AboutDialog : Form
    {
        private struct AboutText
        {
            internal string Text;
            internal string URL;
            internal int    LinkStart;
            internal int    LinkLength;
            internal int    FontSize;
            internal bool   Centre;
                
            public AboutText(string strText, string strURL, int intLinkStart,
                int intLinkLength, int intFontSize, bool blnCentre)
            {
                this.Text       = strText;
                this.URL        = strURL;
                this.LinkStart  = intLinkStart;
                this.LinkLength = intLinkLength;
                this.FontSize   = intFontSize;
                this.Centre     = blnCentre;
            }
        }
            
        private ArrayList  ma_AboutText   = new ArrayList();
        private Font m_fntBig    = new Font("Verdana", 16);
        private Font m_fntMedium = new Font("Verdana", 12);
        private Font m_fntSmall  = new Font("Verdana",  8);
            
        private bool m_blnLaidOut = false;
            
        public AboutDialog()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        } 
               
        public void ShowAbout()
        {
            LayoutWindow();
            ShowDialog();
        }
            
        public string WindowTitle
        {
            set
            {
                this.Text = value;
            }
        }
            
        public void AddLine()
        {
            this.AddLine("", "", -1, -1, 1, false);
        }
            
        public void AddLine(string strText)
        {
            this.AddLine(strText, "", -1, -1, 1, false);
        }
            
        public void AddLine(string strText, bool blnCentre)
        {
            this.AddLine(strText, "", -1, -1, 1, blnCentre);
        }
            
        public void AddLine(string strText, string strURL, int intLinkStart,
            int intLinkLength, int intFontSize, bool blnCentre)
        {
                
            ma_AboutText.Add(new AboutText(strText, strURL, intLinkStart,
                intLinkLength, intFontSize, blnCentre));
        }
            
        private void LayoutWindow()
        {
            Panel panAbout = new Panel();
            panAbout.AutoScroll = true;
            panAbout.BackColor = Color.White;
            panAbout.BorderStyle = BorderStyle.FixedSingle;
            panAbout.Top = 5;
            panAbout.Left = 5;
                        
            if (m_blnLaidOut)
            {
                return;
            }
                
            int intTop = 10;
            int intMaxWidth = 0;
            foreach (object objLine in ma_AboutText)
            {
                AboutText line = (AboutText)objLine;
                    
                if (line.Text == "-")
                {
                    // full width line
                    LineLabel labLine = new LineLabel();
                    intTop += 5;
                    labLine.Top = intTop;
                    intTop += labLine.Height+5;
                    labLine.Left = 10;
                    panAbout.Controls.Add(labLine);
                }
                else if (line.Text.Length >= 8&& 
                    line.Text.Substring(0,6).ToLower() == "[line " &&
                    line.Text.Substring(line.Text.Length-1,1) == "]")
                {
                    // % width                        
                    float fltPCWidth = 100f;
                    try
                    {
                        fltPCWidth = Single.Parse(
                            line.Text.Substring(6, line.Text.Length-7));
                    }
                    catch
                    {
                        fltPCWidth = 100f;
                    }
                        
                    LineLabel labLine = new LineLabel();
                    labLine.PercentWidth = fltPCWidth;
                    intTop += 5;
                    labLine.Top = intTop;
                    intTop += labLine.Height+5;
                    labLine.Left = 10;
                    panAbout.Controls.Add(labLine);
                }
                else if (line.URL == "" || line.LinkStart < 0 ||
                    line.LinkLength < 1)
                {
                    Label labLine = new Label();
                    labLine.Text = line.Text;
                    labLine.AutoSize = true;
                    labLine.Left = 10;
                    switch (line.FontSize)
                    {
                        case 1:
                            labLine.Font = m_fntSmall;
                            break;
                        case 2:
                            labLine.Font = m_fntMedium;
                            break;
                        case 3:
                            labLine.Font = m_fntBig;
                            break;
                        default:
                            labLine.Font = m_fntSmall;
                            break;
                    }
                            
                    if (line.Centre)
                    {
                        labLine.TextAlign = ContentAlignment.MiddleCenter;
                    }
                    else
                    {
                        labLine.TextAlign = ContentAlignment.MiddleLeft;
                    }
                    
                    labLine.Top = intTop;
                    intTop += labLine.Height;
                    intMaxWidth = Math.Max(intMaxWidth, labLine.Width);
                    panAbout.Controls.Add(labLine);
                }
                else
                {
                    // link
                    LinkLabel labLine = new LinkLabel();
                    labLine.Text = line.Text;
                    labLine.Links.Add(line.LinkStart, line.LinkLength, line.URL);
                    labLine.AutoSize = true;
                    labLine.Left = 10;
                        
                    labLine.LinkClicked += 
                        new LinkLabelLinkClickedEventHandler(
                        LinkLabel_LinkClicked);
                        
                    switch (line.FontSize)
                    {
                        case 1:
                            labLine.Font = m_fntSmall;
                            break;
                        case 2:
                            labLine.Font = m_fntMedium;
                            break;
                        case 3:
                            labLine.Font = m_fntBig;
                            break;
                        default:
                            labLine.Font = m_fntSmall;
                            break;
                    }
                            
                    if (line.Centre)
                    {
                        labLine.TextAlign = ContentAlignment.MiddleCenter;
                    }
                    else
                    {
                        labLine.TextAlign = ContentAlignment.MiddleLeft;
                    }
                    
                    labLine.Top = intTop;
                    intTop += labLine.Height;
                    intMaxWidth = Math.Max(intMaxWidth, labLine.Width);
                    panAbout.Controls.Add(labLine);
                }
            }
                
            panAbout.Width = intMaxWidth+28;
            foreach (Label label in panAbout.Controls)
            {
                int intAutoHeight = label.Height;
                label.AutoSize = false;
                label.Width = intMaxWidth+6;
                label.Height = intAutoHeight;
            }
                
            Button btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Click += new System.EventHandler(OK_Click);
            this.AcceptButton = btnOK;
                 
            int intMaxHeight = SystemInformation.WorkingArea.Height/3;
            int intHeightForPanel = intTop+10 + 12 + btnOK.Height + 20;
            int intDesiredHeight = Math.Min(intMaxHeight,
                intHeightForPanel);
            int intDesiredWidth = panAbout.Size.Width+10;
                 
            this.ClientSize = new Size(
                intDesiredWidth,
                intDesiredHeight);
                
            btnOK.Location = new Point ((int)((ClientSize.Width-btnOK.Width)/2), 
                ClientSize.Height-btnOK.Height-10);
                
            panAbout.Size = new Size(ClientSize.Width-10,
                ClientSize.Height - 10 - btnOK.Height - 20);
                
            if (intMaxHeight < intHeightForPanel)
            {
                // adjust width to account for scroll bar
                this.ClientSize = new Size(
                    2*intDesiredWidth-panAbout.Width+10,
                    ClientSize.Height);
                
                panAbout.Width = ClientSize.Width-10;
            }
                
            this.Controls.Add(panAbout);
            this.Controls.Add(btnOK);
                
            m_blnLaidOut = true;
        }
            
        public Font BigFont 
        {
            get
            {
                return m_fntBig;
            }
            set
            {
                m_fntBig = value;
            }
        }
            
        public Font MediumFont 
        {
            get
            {
                return m_fntMedium;
            }
            set
            {
                m_fntMedium = value;
            }
        }
            
        public Font SmallFont 
        {
            get
            {
                return m_fntSmall;
            }
            set
            {
                m_fntSmall = value;
            }
        }
            
        public void OK_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
            
        public void LinkLabel_LinkClicked(
            object sender, LinkLabelLinkClickedEventArgs ev)
        {
            ((LinkLabel)sender).Links[0].Visited = true;
            System.Diagnostics.Process.Start(ev.Link.LinkData.ToString());
        }
    }
        
    public class LineLabel : Label
    {
        private Color m_Color = Color.Black;
        private float m_fltPCWidth = 100f;
            
        public LineLabel()
        {
            this.Height = 1;
            DrawLine();
        }
            
        public float PercentWidth
        {
            get
            {
                return m_fltPCWidth;
            }
            set
            {
                m_fltPCWidth = Math.Max(0f, Math.Min(100f, value));
            }
        }
            
        public Color Color
        {
            get
            {
                return m_Color;
            }
            set
            {
                m_Color = value;
            }
        }
            
        protected override void OnResize(EventArgs ev)
        {
            DrawLine();
        }
            
        private void DrawLine()
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(bmp);
                
            int intWidth = (int)(m_fltPCWidth*(float)bmp.Width/100f);
            int intX = (bmp.Width - intWidth)/2;
                
            g.FillRectangle(new SolidBrush(m_Color), 
                intX, 0, intWidth, bmp.Height);
            g.Dispose();
                
            this.Image = bmp;
        }
    }
}