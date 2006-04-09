/*
 *************************************************************************
 * Copyright (C) 2005 Trevor Barnett                                     *
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
 * File:          BackgroundDialog.cs                                    *
 * Namespace:     SwSt                                                   *
 * Last modified: 13 January 2005                                        *
 * Class:         BackgroundDialog                                       *
 * Description:   Dialog to select background image, pattern or colour.  *
 *************************************************************************
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SwSt;

//-----------------------------------------------------------------//

namespace SwSt
{   
    public class BackgroundDialog : Form
    {
        Panel m_panStyle = new Panel();
        Label m_labStyle = new Label();
        ComboBox m_cboStyle = new ComboBox();
        GroupBox m_gbxSolidColour = new GroupBox();
        Panel m_panSolidColour = new Panel();
        Label m_labColour = new Label();
        Button m_btnColour = new Button();
        GroupBox m_gbxGradient = new GroupBox();
        Panel m_panGradientColours = new Panel();
        Label m_labGradient = new Label();
        Label m_labColour1 = new Label();
        Button m_btnColour1 = new Button();
        Label m_labColour2 = new Label();
        Button m_btnColour2 = new Button();
        Panel m_panGradientDir = new Panel();
        Label m_labGradientDir = new Label();
        RadioButton m_radGradientLR = new RadioButton();
        RadioButton m_radGradientTB = new RadioButton();
        RadioButton m_radGradientTLBR = new RadioButton();
        RadioButton m_radGradientTRBL = new RadioButton();
        RadioButton m_radGradientIn = new RadioButton();
        GroupBox m_gbxImage = new GroupBox();
        Panel m_panSelectImage = new Panel();
        Label m_labImage = new Label();
        TextBox m_txtImage = new TextBox();
        Button m_btnImage = new Button();
        Panel m_panSizing = new Panel();
        RadioButton m_radTile = new RadioButton();
        RadioButton m_radStretch = new RadioButton();
        RadioButton m_radCentre = new RadioButton();
        Panel m_panSizingOptions = new Panel();
        CheckBox m_chkKeepAspectRatio = new CheckBox();
        CheckBox m_chkFillWithGradient = new CheckBox();
        Panel m_panButtons = new Panel();
        Button m_btnOK = new Button();
        Button m_btnCancel = new Button();
        
        int m_intBitmapSize = 16;
         
        // Experiments changing display font size (the small font, large font
        // settings) show the size of the m_radio button "circle" and the space
        // between the circle and attached image, both with MiddleLeft alignment,
        // are constant.
        private const int mc_intRADIO_BUTTON_CIRCLE_WIDTH = 12;
        private const int mc_intRADIO_BUTTON_CIRCLE_TEXT_SPACE = 6;
        private const int mc_intRADIO_BUTTON_CIRCLE_IMAGE_SPACE = 5;
        private const int mc_intCHECK_BOX_CHECK_WIDTH = 13;
        private const int mc_intCHECK_BOX_CHECK_TEXT_SPACE = 6;
        
        private Color m_SelectedColour = Color.White;
        private Color m_SelectedGradientColour1 = Color.Black;
        private Color m_SelectedGradientColour2 = Color.White;
        private GradientDirection m_GradientDirection = GradientDirection.LeftToRight;
        private bool m_blnTile = true;
        private bool m_blnStretch = false;
        private bool m_blnCentre = false;
        private bool m_blnKeepAspectRatio = true;
        private bool m_blnFillWithGradient = true;
        private string m_strImageDir = "";
        private string m_strImageFile = "";
        private string m_strImageFileFullPath = "";
        private BackgroundStyle m_BackgroundStyle = BackgroundStyle.SolidColour;
        private string m_strFileFilter = 
            "All Picture Files (*.bmp;*.gif;*.jpg;*.jpeg;*.dib;*.png)|" +
            "*.bmp;*.gif;*.jpg;*.jpeg;*.dib;*.png|"+
            "All Files|*.*";
        
        private int[] ma_intCustomColours = new int[16];
        
        public BackgroundDialog()
        {
            this.Text = "Background";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(100,100); //new Size(365,400);
            
            this.StartPosition = FormStartPosition.CenterScreen;
            
            CreateControls();
            SetFont();
            RenderControls();
            
            // initialise custom colours to white
            // NB. for custom colours used by dialog box, byte order is B G R
            // no alpha value is used
            int intWhite = (Color.White.B<<16)+(Color.White.G<<8)+Color.White.R;
            for (int intIndex=0; intIndex<16; intIndex++)
            {
                ma_intCustomColours[intIndex] = intWhite;
            } 
        }
        
        protected override void OnFontChanged(EventArgs ev)
        {
            SetFont();
            RenderControls();
        }
        
        private void SetFont()
        {
            m_panStyle.Font = this.Font;
            m_labStyle.Font = this.Font;
            m_cboStyle.Font = this.Font;
            m_gbxSolidColour.Font = this.Font;
            m_panSolidColour.Font = this.Font;    
            m_labColour.Font = this.Font;    
            m_btnColour.Font = this.Font;    
            m_gbxGradient.Font = this.Font;    
            m_labGradient.Font = this.Font;    
            m_labColour1.Font = this.Font;    
            m_btnColour1.Font = this.Font;    
            m_labColour2.Font = this.Font;    
            m_btnColour2.Font = this.Font;    
            m_panGradientDir.Font = this.Font;    
            m_labGradientDir.Font = this.Font;
            m_radGradientLR.Font = this.Font;    
            m_radGradientTB.Font = this.Font;    
            m_radGradientTLBR.Font = this.Font;    
            m_radGradientTRBL.Font = this.Font;    
            m_radGradientIn.Font = this.Font;    
            m_gbxImage.Font = this.Font;    
            m_labImage.Font = this.Font;    
            m_panStyle.Font = this.Font;    
            m_btnImage.Font = this.Font;    
            m_panSizing.Font = this.Font;    
            m_radTile.Font = this.Font;    
            m_radStretch.Font = this.Font;    
            m_radCentre.Font = this.Font;    
            m_chkKeepAspectRatio.Font = this.Font;    
            m_chkFillWithGradient.Font = this.Font;    
            m_panButtons.Font = this.Font;    
            m_btnOK.Font = this.Font;    
            m_btnCancel.Font = this.Font;    
        }
        
        public void CreateControls()
        {
            ToolTip toolTip = new ToolTip();
            
            // SET CONTENT OF CONTROLS
            
            // Background Style
            
            m_labStyle.Text = "Background Style";
            
            m_cboStyle.Items.Add("Solid Colour");
            m_cboStyle.Items.Add("Gradient");
            m_cboStyle.Items.Add("Image");
            // NB. assuming first style has value 1 and then increase by 1
            m_cboStyle.SelectedIndex = (int)m_BackgroundStyle-1;
            m_cboStyle.SelectedIndexChanged += 
                new EventHandler(Style_SelectedIndexChanged);
            
            // Solid Colour
            m_gbxSolidColour.Text = "Solid Colour";
            
            m_labColour.Text  = "Selected Colour";
            
            m_btnColour.FlatStyle = FlatStyle.Flat;
            m_btnColour.Click += 
                new EventHandler(Colour_Click);
            toolTip.SetToolTip(m_btnColour, 
                "Selected background colour.  Click to change.");
            
            // Gradient
            m_gbxGradient.Text = "Gradient";
                        
            // Gradient Colours
                    
            m_labGradient.Text  = "Colours:";
            
            m_labColour1.Text = "Start";
            m_btnColour1.FlatStyle = FlatStyle.Flat;
            m_btnColour1.Click += 
                new EventHandler(Colour1_Click);
            toolTip.SetToolTip(m_btnColour1, 
                "Selected gm_radient start colour.  Click to change.");
            
            m_labColour2.Text = "End";
            m_btnColour2.FlatStyle = FlatStyle.Flat;
            m_btnColour2.Click += 
                new EventHandler(Colour2_Click);
            toolTip.SetToolTip(m_btnColour2, 
                "Selected gm_radient end colour.  Click to change.");
            
            // Gradient Direction
                    
            m_labGradientDir.Text  = "Direction:";
            
            toolTip.SetToolTip(m_radGradientLR, 
                "Horizontal gm_radient from left to right");
            m_radGradientLR.CheckedChanged +=
                new EventHandler(GradientLR_CheckedChanged);
            if (m_GradientDirection == GradientDirection.LeftToRight)
            {
                m_radGradientLR.Checked = true;
            }
            
            toolTip.SetToolTip(m_radGradientTB, 
                "Vertical gm_radient from top to bottom");
            m_radGradientTB.CheckedChanged +=
                new EventHandler(GradientTB_CheckedChanged);
            if (m_GradientDirection == GradientDirection.TopToBottom)
            {
                m_radGradientTB.Checked = true;
            }
            
            toolTip.SetToolTip(m_radGradientTLBR, 
                "Diagonal gm_radient from top left to bottom right");
            m_radGradientTLBR.CheckedChanged +=
                new EventHandler(GradientTLBR_CheckedChanged);
            if (m_GradientDirection == GradientDirection.TopLeftToBottomRight)
            {
                m_radGradientTLBR.Checked = true;
            }
            
            toolTip.SetToolTip(m_radGradientTRBL, 
                "Diagonal gm_radient from top right to bottom left");
            m_radGradientTRBL.CheckedChanged +=
                new EventHandler(GradientTRBL_CheckedChanged);
            if (m_GradientDirection == GradientDirection.TopRightToBottomLeft)
            {
                m_radGradientTRBL.Checked = true;
            }
            
            toolTip.SetToolTip(m_radGradientIn, 
                "Gradient from edge to centre");
            m_radGradientIn.CheckedChanged +=
                new EventHandler(GradientIn_CheckedChanged);
            if (m_GradientDirection == GradientDirection.Inwards)
            {
                m_radGradientIn.Checked = true;
            }
            
            // Background Image
            m_gbxImage.Text = "Image";
                
            // Selected Image    
            m_panSelectImage.Font = this.Font;    
            
            m_labImage.Text = "Selected Image";
                  
            m_btnImage.Text = "Browse...";
            m_btnImage.Click += new EventHandler(Browse_Click);
            toolTip.SetToolTip(m_btnImage, "Browse directories for background image");
            
            // Image Sizing
            
            m_radTile.Text = "Tile";
            m_radTile.CheckedChanged += new EventHandler(Tile_CheckedChanged);
            if (m_blnTile)
            {
                m_radTile.Checked = true;
            }
            
            m_radStretch.Text = "Stretch";
            m_radStretch.CheckedChanged += new EventHandler(Stretch_CheckedChanged);
            if (m_blnStretch)
            {
                m_radStretch.Checked = true;
            }
            
            m_radCentre.Text = "Centre";
            m_radCentre.CheckedChanged += new EventHandler(Centre_CheckedChanged);
            if (m_blnCentre)
            {
                m_radCentre.Checked = true;
            }
             
            // Image Sizing Options
            m_panSizingOptions.Font = this.Font;    
            
            m_chkKeepAspectRatio.Text = "Keep Aspect Ratio";
            m_chkKeepAspectRatio.CheckedChanged += 
                new EventHandler(KeepAspectRatio_CheckedChanged);
            if (m_blnKeepAspectRatio)
            {
                m_chkKeepAspectRatio.Checked = true;
            }
            else
            {
                m_chkKeepAspectRatio.Checked = false;
            }
            
            m_chkFillWithGradient.Text = "Fill With Gradient";
            m_chkFillWithGradient.CheckedChanged += 
                new EventHandler(FillWithGradient_CheckedChanged);
            if (m_blnFillWithGradient)
            {
                m_chkFillWithGradient.Checked = true;
            }
            else
            {
                m_chkFillWithGradient.Checked = false;
            }
            
            
            // Accept and Cancel Buttons
            
            m_btnOK.Text = "OK";
            m_btnOK.Click += new EventHandler(OK_Click);
            
            m_btnCancel.Text = "Cancel";
            
            this.AcceptButton = m_btnOK;
            this.CancelButton = m_btnCancel;
        }   
            
        private void RenderControls()
        {  
            Graphics cg = this.CreateGraphics();
            
            SizeF fontSizeF = cg.MeasureString("AKbdhklgjpqyf", this.Font);
            int intFontHeight = (int)Math.Ceiling(fontSizeF.Height);
            
            Bitmap bmpButton = null;
            Graphics g = null;
            
            // SET SIZES AND POSITIONS OF CONTROLS
            
            // Calculate m_label and button text lengths
            // Sizing m_radio buttons
            SizeF textSizeTile = 
                cg.MeasureString(m_radTile.Text, m_radTile.Font);
            SizeF textSizeStretch = 
                cg.MeasureString(m_radStretch.Text, m_radStretch.Font);
            SizeF textSizeCentre = 
                cg.MeasureString(m_radCentre.Text, m_radCentre.Font);
            int intRadWidth = (int)Math.Ceiling(
                Math.Max(textSizeTile.Width,
                         Math.Max(textSizeStretch.Width,
                                  textSizeCentre.Width))) + 
                mc_intRADIO_BUTTON_CIRCLE_WIDTH + 
                mc_intRADIO_BUTTON_CIRCLE_TEXT_SPACE + 20;
            
            // Sizing options check boxes
            SizeF textSizeKAR = 
                cg.MeasureString(m_chkKeepAspectRatio.Text, m_chkKeepAspectRatio.Font);
            SizeF textSizeFWG = 
                cg.MeasureString(m_chkFillWithGradient.Text, m_chkFillWithGradient.Font);
            
            // Browse button
            SizeF textSizeBrowse = 
                cg.MeasureString(m_btnImage.Text, m_btnImage.Font);
            
            // OK button
            SizeF textSizeOK = 
                cg.MeasureString(m_btnOK.Text, m_btnOK.Font);
                
            // Cancel button
            SizeF textSizeCancel = 
                cg.MeasureString(m_btnCancel.Text, m_btnCancel.Font);
            
            cg.Dispose();
            
            m_panStyle.Left = 10;
            m_panStyle.Top = 10;
            
            m_labStyle.Left = 0;
            m_labStyle.TextAlign = ContentAlignment.MiddleLeft;
            m_labStyle.AutoSize = true;
            
            m_cboStyle.Left = m_labStyle.Left + m_labStyle.Width + 5;
            
            m_cboStyle.Height = Math.Max(m_cboStyle.Height,
                                       intFontHeight + 5);
            
            m_labStyle.Top = Math.Max(0, (m_cboStyle.Height-m_labStyle.Height)/2);
            m_cboStyle.Top = Math.Max(0, (m_labStyle.Height-m_cboStyle.Height)/2);
            
            m_panStyle.Width = m_cboStyle.Left + m_cboStyle.Width;
            m_panStyle.Height = Math.Max(m_labStyle.Top + m_labStyle.Height,
                                       m_cboStyle.Top + m_cboStyle.Height);
            
            m_gbxSolidColour.Left = 10;
                        
            m_panSolidColour.Left = m_gbxSolidColour.DisplayRectangle.X + 5;
            m_panSolidColour.Top  = m_gbxSolidColour.DisplayRectangle.Y + 5;
                        
            m_labColour.Left  = 0;
            m_labColour.AutoSize = true;
            m_labColour.TextAlign = ContentAlignment.MiddleLeft;
            
            m_btnColour.Left   = m_labColour.Left + m_labColour.Width + 5;
            m_btnColour.Width  = m_intBitmapSize;
            m_btnColour.Height = m_intBitmapSize;
            
            int intBottomLastRow = m_panStyle.Top + m_panStyle.Height;
            
            m_panSolidColour.Height = Math.Max(m_labColour.Height, 
                                             m_btnColour.Height);
            
            m_labColour.Top = (m_panSolidColour.Height - m_labColour.Height)/2;
            m_btnColour.Top = (m_panSolidColour.Height - m_btnColour.Height)/2;
                         
            m_gbxSolidColour.Top = intBottomLastRow+10;
                         
            m_gbxSolidColour.Height += m_panSolidColour.Height + 10 -
                         m_gbxSolidColour.DisplayRectangle.Height;
            
            intBottomLastRow = m_gbxSolidColour.Top + m_gbxSolidColour.Height;
            
            m_gbxGradient.Top   = intBottomLastRow + 10;
            m_gbxGradient.Left  = 10;
            
            m_panGradientColours.Left = m_gbxGradient.DisplayRectangle.X + 5;
            m_panGradientColours.Top  = m_gbxGradient.DisplayRectangle.Y + 5;
            
            m_labGradient.Left  = 0;
            m_labGradient.TextAlign = ContentAlignment.MiddleLeft;
            m_labGradient.AutoSize = true;
            
            m_labColour1.Left  = m_labGradient.Left+m_labGradient.Width + 10;
            m_labColour1.TextAlign = ContentAlignment.MiddleLeft;
            m_labColour1.AutoSize = true;
            
            m_btnColour1.Left   = m_labColour1.Left+m_labColour1.Width + 5;
            m_btnColour1.Width  = m_intBitmapSize;
            m_btnColour1.Height = m_intBitmapSize;
            
            m_gbxSolidColour.Width += m_btnColour.Left + m_btnColour.Width + 5 - 
                m_gbxSolidColour.DisplayRectangle.Width;            
            
            m_labColour2.Left = m_btnColour1.Left+m_btnColour1.Width + 10;
            m_labColour2.TextAlign = ContentAlignment.MiddleLeft;
            m_labColour2.AutoSize = true;
            
            m_btnColour2.Left   = m_labColour2.Left+m_labColour2.Width + 5;
            m_btnColour2.Width  = m_intBitmapSize;
            m_btnColour2.Height = m_intBitmapSize;
            
            m_panGradientColours.Height = 
                Math.Max(m_labGradient.Height,
                         Math.Max(m_labColour1.Height,
                                  Math.Max(m_btnColour1.Height,
                                           Math.Max(m_labColour2.Height,
                                                    m_btnColour2.Height))));
            m_panGradientColours.Width = m_btnColour2.Left + m_btnColour2.Width;
            
            m_labGradient.Top = (m_panGradientColours.Height - m_labGradient.Height)/2;
            m_labColour1.Top  = (m_panGradientColours.Height - m_labColour1.Height)/2;
            m_btnColour1.Top  = (m_panGradientColours.Height - m_btnColour1.Height)/2;
            m_labColour2.Top  = (m_panGradientColours.Height - m_labColour2.Height)/2;
            m_btnColour2.Top  = (m_panGradientColours.Height - m_btnColour2.Height)/2;
            
            intBottomLastRow = m_panGradientColours.Top + m_panGradientColours.Height;
            
            m_panGradientDir.Left = m_panGradientColours.Left;
            m_panGradientDir.Top = intBottomLastRow + 10;
            
            m_labGradientDir.Left  = 0;
            m_labGradientDir.TextAlign = ContentAlignment.MiddleLeft;
            m_labGradientDir.AutoSize = true;
            
            m_radGradientLR.Width = mc_intRADIO_BUTTON_CIRCLE_WIDTH +
                                  mc_intRADIO_BUTTON_CIRCLE_IMAGE_SPACE + 
                                  m_intBitmapSize;
            m_radGradientLR.Height = Math.Max(m_intBitmapSize+1,
                                       intFontHeight);
            
            m_radGradientLR.CheckAlign = ContentAlignment.MiddleLeft;
            m_radGradientLR.ImageAlign = ContentAlignment.MiddleLeft;
            
            m_radGradientLR.Left = m_labGradientDir.Left + m_labGradientDir.Width + 10;
            
            // line up 'colour controls'
            int intColourLeft = Math.Max(m_btnColour.Left,
                                    Math.Max(m_btnColour1.Left,
                                        m_radGradientLR.Left-m_btnColour1.Width + 
                                        m_radGradientLR.Width));
                                        
            m_btnColour.Left = intColourLeft;
            m_btnColour1.Left = intColourLeft;
            m_radGradientLR.Left = 
                intColourLeft+m_btnColour1.Width-m_radGradientLR.Width;
            
            // reset positions of controls defined relative to m_btnColour1
            m_labColour2.Left = m_btnColour1.Left+m_btnColour1.Width + 10;
            m_btnColour2.Left   = m_labColour2.Left+m_labColour2.Width + 5;
             
            
            m_radGradientTB.Width = mc_intRADIO_BUTTON_CIRCLE_WIDTH +
                                  mc_intRADIO_BUTTON_CIRCLE_IMAGE_SPACE + 
                                  m_intBitmapSize;
            m_radGradientTB.Height = Math.Max(m_intBitmapSize+1,
                                       intFontHeight);
            m_radGradientTB.CheckAlign = ContentAlignment.MiddleLeft;
            m_radGradientTB.ImageAlign = ContentAlignment.MiddleLeft;
            m_radGradientTB.Left = 
                m_btnColour2.Left+m_btnColour2.Width-m_radGradientLR.Width;
            
            int intRadioButtonSpacing = m_radGradientTB.Left - m_radGradientLR.Left;
            
            m_radGradientTLBR.Width = mc_intRADIO_BUTTON_CIRCLE_WIDTH +
                                  mc_intRADIO_BUTTON_CIRCLE_IMAGE_SPACE + 
                                  m_intBitmapSize;
            m_radGradientTLBR.Height = Math.Max(m_intBitmapSize+1,
                                       intFontHeight);
            m_radGradientTLBR.CheckAlign = ContentAlignment.MiddleLeft;
            m_radGradientTLBR.ImageAlign = ContentAlignment.MiddleLeft;
            m_radGradientTLBR.Left = m_radGradientTB.Left + intRadioButtonSpacing;
            
            
            m_radGradientTRBL.Width = mc_intRADIO_BUTTON_CIRCLE_WIDTH +
                                  mc_intRADIO_BUTTON_CIRCLE_IMAGE_SPACE + 
                                  m_intBitmapSize;
            m_radGradientTRBL.Height = Math.Max(m_intBitmapSize+1,
                                       intFontHeight);
            m_radGradientTRBL.CheckAlign = ContentAlignment.MiddleLeft;
            m_radGradientTRBL.ImageAlign = ContentAlignment.MiddleLeft;
            m_radGradientTRBL.Left = m_radGradientTLBR.Left + intRadioButtonSpacing;
            
            
            m_radGradientIn.Width = mc_intRADIO_BUTTON_CIRCLE_WIDTH +
                                  mc_intRADIO_BUTTON_CIRCLE_IMAGE_SPACE + 
                                  m_intBitmapSize;
            m_radGradientIn.Height = Math.Max(m_intBitmapSize+1,
                                       intFontHeight);
            m_radGradientIn.CheckAlign = ContentAlignment.MiddleLeft;
            m_radGradientIn.ImageAlign = ContentAlignment.MiddleLeft;
            m_radGradientIn.Left = m_radGradientTRBL.Left + intRadioButtonSpacing;
            
            m_panGradientDir.Height = 
                Math.Max(m_labGradientDir.Height,
                         Math.Max(m_radGradientLR.Height,
                                  Math.Max(m_radGradientTB.Height,
                                           Math.Max(m_radGradientTLBR.Height,
                                                    Math.Max(m_radGradientTRBL.Height,
                                                             m_radGradientIn.Height)))));
            m_panGradientDir.Width = m_radGradientIn.Left + m_radGradientIn.Width;
            
            m_labGradientDir.Top   = 
                (m_panGradientDir.Height - m_labGradientDir.Height)/2;
            m_radGradientLR.Top    = 
                (m_panGradientColours.Height - m_radGradientLR.Height)/2;
            m_radGradientTB.Top    = 
                (m_panGradientColours.Height - m_radGradientTB.Height)/2;
            m_radGradientTLBR.Top  = 
                (m_panGradientColours.Height - m_radGradientTLBR.Height)/2;
            m_radGradientTRBL.Top  = 
                (m_panGradientColours.Height - m_radGradientTRBL.Height)/2;
            m_radGradientIn.Top    = 
                (m_panGradientColours.Height - m_radGradientIn.Height)/2;
            
            intBottomLastRow = m_panGradientDir.Top + m_panGradientDir.Height;
            
            m_gbxGradient.Height += m_panGradientDir.Top + m_panGradientDir.Height + 5 -
                m_gbxGradient.DisplayRectangle.Y - m_gbxGradient.DisplayRectangle.Height;
            
            m_gbxGradient.Width += 
                Math.Max(m_panGradientColours.Left + m_panGradientColours.Width,
                         m_panGradientDir.Left + m_panGradientDir.Width) + 5 -
                m_gbxGradient.DisplayRectangle.Width;
                
            m_gbxImage.Top = m_gbxGradient.Top + m_gbxGradient.Height + 10;
            m_gbxImage.Left = 10;
            
            m_panSelectImage.Left = m_gbxImage.DisplayRectangle.X + 5;
            m_panSelectImage.Top  = m_gbxImage.DisplayRectangle.Y + 5;
            
            m_labImage.Left = 0;
            m_labImage.AutoSize = true;
            
            m_txtImage.Left = m_labImage.Left + m_labImage.Width + 5;
            m_txtImage.Height = Math.Max(m_txtImage.Height,
                                      intFontHeight+5);
            m_txtImage.AutoSize = false;
            
            m_btnImage.Height = Math.Max(m_txtImage.Height,
                                      intFontHeight+5);
            
            m_btnImage.Left = m_txtImage.Left + m_txtImage.Width + 5;
            m_btnImage.Width = Math.Max(m_btnImage.Width,
                                      (int)Math.Ceiling(textSizeBrowse.Width) + 10); 
            
            m_panSelectImage.Width = m_btnImage.Left + m_btnImage.Width;
            
            m_panSelectImage.Height = Math.Max(m_labImage.Height,
                                             Math.Max(m_txtImage.Height,
                                                      m_btnImage.Height));
            
            m_labImage.Top = (m_panSelectImage.Height - m_labImage.Height)/2;
            m_txtImage.Top = (m_panSelectImage.Height - m_txtImage.Height)/2;
            m_btnImage.Top = (m_panSelectImage.Height - m_btnImage.Height)/2;
            
            
            
            intBottomLastRow = m_panSelectImage.Top + m_panSelectImage.Height;
            
            m_panSizing.Top = intBottomLastRow + 5;
            m_panSizing.Left = m_gbxImage.DisplayRectangle.X + 5;
            
            m_radTile.TextAlign = ContentAlignment.MiddleLeft;
            m_radTile.CheckAlign = ContentAlignment.MiddleLeft;
            m_radTile.Left = 0;
            m_radTile.Checked = true;
            
            m_radStretch.TextAlign = ContentAlignment.MiddleLeft;
            m_radStretch.CheckAlign = ContentAlignment.MiddleLeft;
            
            m_radCentre.TextAlign = ContentAlignment.MiddleLeft;
            m_radCentre.CheckAlign = ContentAlignment.MiddleLeft;
            
            m_radTile.Height = Math.Max(m_radTile.Height,
                                      intFontHeight);
            m_radStretch.Height = Math.Max(m_radStretch.Height,
                                      intFontHeight);
            m_radCentre.Height = Math.Max(m_radCentre.Height,
                                      intFontHeight);
            
            m_panSizing.Height = Math.Max(m_radTile.Height, 
                                        Math.Max(m_radStretch.Height,
                                                 m_radCentre.Height));
            
            m_radTile.Top    = (m_panSizing.Height - m_radTile.Height)/2;
            m_radStretch.Top = (m_panSizing.Height - m_radStretch.Height)/2;
            m_radCentre.Top  = (m_panSizing.Height - m_radCentre.Height)/2;
            
            
            m_panSizingOptions.Top = m_panSizing.Top + m_panSizing.Height;
            m_panSizingOptions.Left = m_gbxImage.DisplayRectangle.X + 5;
            
            m_chkKeepAspectRatio.TextAlign = ContentAlignment.MiddleLeft;
            m_chkKeepAspectRatio.CheckAlign = ContentAlignment.MiddleLeft;
            m_chkKeepAspectRatio.Left = 0;
            m_chkKeepAspectRatio.Enabled = false;
            m_chkKeepAspectRatio.Checked = true;
                
            m_chkFillWithGradient.TextAlign = ContentAlignment.MiddleLeft;
            m_chkFillWithGradient.CheckAlign = ContentAlignment.MiddleLeft;
            m_chkFillWithGradient.Enabled = false;
            m_chkFillWithGradient.Checked = true;
            
            int intChkWidth = (int)Math.Ceiling(
                Math.Max(textSizeKAR.Width, textSizeFWG.Width)) + 
                mc_intCHECK_BOX_CHECK_WIDTH + 
                mc_intCHECK_BOX_CHECK_TEXT_SPACE;
            
            m_radTile.Width = intRadWidth;
            m_radStretch.Left = m_radTile.Left + m_radTile.Width;
            m_radStretch.Width = intRadWidth;
            m_radCentre.Left = m_radStretch.Left + m_radStretch.Width;
            m_radCentre.Width = intRadWidth;
            
            m_panSizing.Width = m_radCentre.Left + m_radCentre.Width;
            
            m_chkKeepAspectRatio.Width = intChkWidth;
            m_chkKeepAspectRatio.Height = Math.Max(m_chkKeepAspectRatio.Height,
                                      intFontHeight);
            m_chkFillWithGradient.Left = m_chkKeepAspectRatio.Left + 
                                       m_chkKeepAspectRatio.Width + 5;
            m_chkFillWithGradient.Width = intChkWidth;
            m_chkFillWithGradient.Height = Math.Max(m_chkFillWithGradient.Height,
                                      intFontHeight);
            
            m_panSizingOptions.Width = m_chkFillWithGradient.Left + 
                                     m_chkFillWithGradient.Width;
            
            m_panSizingOptions.Height = Math.Max(m_chkKeepAspectRatio.Height, 
                                               m_chkFillWithGradient.Height);
            
            m_chkKeepAspectRatio.Top  = 
                (m_panSizingOptions.Height - m_chkKeepAspectRatio.Height)/2;
            m_chkFillWithGradient.Top = 
                (m_panSizingOptions.Height - m_chkFillWithGradient.Height)/2;
            
            
            m_gbxImage.Height += 
                m_panSizingOptions.Top + m_panSizingOptions.Height + 5 -
                m_gbxImage.DisplayRectangle.Y - 
                m_gbxImage.DisplayRectangle.Height;
            
            m_gbxImage.Width +=
                Math.Max(m_panSelectImage.Left + m_panSelectImage.Width,
                         Math.Max(m_panSizing.Left + m_panSizing.Width,
                                  m_panSizingOptions.Left + 
                                  m_panSizingOptions.Width)) + 5 -
                m_gbxImage.DisplayRectangle.Width;
            
            m_panButtons.Top = m_gbxImage.Top + m_gbxImage.Height + 10;
            m_panButtons.Left = 10;
            
            m_btnOK.Left = 0;
            m_btnOK.Height = Math.Max(m_btnOK.Height,
                                      intFontHeight+5);
            
            m_btnCancel.Left = m_btnOK.Left + m_btnOK.Width + 5;
            m_btnCancel.Height = Math.Max(m_btnCancel.Height,
                                      intFontHeight+5);
                                      
            int intButtonWidth = 
                Math.Max(m_btnOK.Width, 
                    Math.Max(m_btnCancel.Width,
                        Math.Max((int)Math.Ceiling(textSizeOK.Width) + 10,
                                 (int)Math.Ceiling(textSizeCancel.Width) + 10)));
                  
            m_btnOK.Width = intButtonWidth;                    
            m_btnCancel.Width = intButtonWidth;
            
            m_panButtons.Width = m_btnCancel.Left + m_btnCancel.Width;
            
            m_panButtons.Height = Math.Max(m_btnOK.Height, m_btnCancel.Height);
            
            m_btnOK.Top     = (m_panButtons.Height - m_btnOK.Height)/2;
            m_btnCancel.Top = (m_panButtons.Height - m_btnCancel.Height)/2;
            
            ClientSize = new Size(
                Math.Max(m_panStyle.Width,
                    Math.Max(m_gbxSolidColour.Width,
                        Math.Max(m_gbxGradient.Width,
                            Math.Max(m_gbxImage.Width,
                                     m_panButtons.Width)))) + 20,
                m_panButtons.Top + m_panButtons.Height + 10);
            
            m_panStyle.Width = ClientSize.Width - 20;
            m_cboStyle.Width = m_panStyle.Width - m_cboStyle.Left;
            m_gbxSolidColour.Width = ClientSize.Width - 20;
            m_panSolidColour.Width = m_gbxSolidColour.DisplayRectangle.Width-10;
            m_gbxGradient.Width = ClientSize.Width - 20;
            m_panGradientColours.Width = m_gbxGradient.DisplayRectangle.Width-10;
            m_panGradientDir.Width = m_gbxGradient.DisplayRectangle.Width-10;
            m_gbxImage.Width = ClientSize.Width - 20;
            m_panSelectImage.Width = m_gbxImage.DisplayRectangle.Width-10;
            m_panSizing.Width = m_gbxImage.DisplayRectangle.Width-10;
            m_panSizingOptions.Width = m_gbxImage.DisplayRectangle.Width-10;
            m_btnImage.Left = m_panSelectImage.Width - m_btnImage.Width;
            m_panSelectImage.Width += 3;
            m_txtImage.Width = m_btnImage.Left - m_txtImage.Left - 5;
            m_panButtons.Width = ClientSize.Width - 20;
            m_btnCancel.Left = m_panButtons.Width - m_btnCancel.Width;
            
            
            // SET SIZE DEPENDENT BUTTON IMAGES
            
            m_intBitmapSize = Math.Min(32, Math.Max(16, m_labColour.Height));
            
            bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
            g = Graphics.FromImage(bmpButton);
            g.FillRectangle(new SolidBrush(this.Colour), 0, 0,
                m_intBitmapSize, m_intBitmapSize);
            g.Dispose();
            m_btnColour.Image = bmpButton;
            
            bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
            g = Graphics.FromImage(bmpButton);
            g.FillRectangle(new SolidBrush(this.GradientColour1), 0, 0,
                m_intBitmapSize, m_intBitmapSize);
            g.Dispose();
            m_btnColour1.Image = bmpButton;
            
            bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
            g = Graphics.FromImage(bmpButton);
            g.FillRectangle(new SolidBrush(this.GradientColour2), 0, 0,
                m_intBitmapSize, m_intBitmapSize);
            g.Dispose();
            m_btnColour2.Image = bmpButton;
            
            LinearGradientBrush lgb = new LinearGradientBrush(
                new Rectangle(0, 0, m_intBitmapSize, m_intBitmapSize),
                Color.Black, Color.White,
                LinearGradientMode.Horizontal);
            bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
            g = Graphics.FromImage(bmpButton);
            g.FillRectangle(lgb, 0, 0,
                m_intBitmapSize, m_intBitmapSize);
            g.Dispose();
            m_radGradientLR.Image = bmpButton;
            
            lgb = new LinearGradientBrush(
                new Rectangle(0, 0, m_intBitmapSize, m_intBitmapSize),
                Color.Black, Color.White,
                LinearGradientMode.Vertical);
            bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
            g = Graphics.FromImage(bmpButton);
            g.FillRectangle(lgb, 0, 0,
                m_intBitmapSize, m_intBitmapSize);
            g.Dispose();
            m_radGradientTB.Image = bmpButton;
            
            lgb = new LinearGradientBrush(
                new Rectangle(0, 0, m_intBitmapSize, m_intBitmapSize),
                Color.Black, Color.White,
                LinearGradientMode.ForwardDiagonal);
            bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
            g = Graphics.FromImage(bmpButton);
            g.FillRectangle(lgb, 0, 0,
                m_intBitmapSize, m_intBitmapSize);
            g.Dispose();
            m_radGradientTLBR.Image = bmpButton;
            
            lgb = new LinearGradientBrush(
                new Rectangle(0, 0, m_intBitmapSize, m_intBitmapSize),
                Color.Black, Color.White,
                LinearGradientMode.BackwardDiagonal);
            bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
            g = Graphics.FromImage(bmpButton);
            g.FillRectangle(lgb, 0, 0,
                m_intBitmapSize, m_intBitmapSize);
            g.Dispose();
            m_radGradientTRBL.Image = bmpButton;
            
            PointF[]  path = 
                new PointF[]
                    {new PointF(-1,-1),
                     new PointF(m_intBitmapSize, -1),
                     new PointF(m_intBitmapSize, m_intBitmapSize),
                     new PointF(-1, m_intBitmapSize)};
            PathGradientBrush pgb = 
                new PathGradientBrush(path);
            pgb.CenterColor = Color.White;
            pgb.SurroundColors = new Color[]
                {Color.Black, Color.Black, Color.Black, Color.Black};
            bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
            g = Graphics.FromImage(bmpButton);
            g.FillRectangle(pgb, 0, 0,
                m_intBitmapSize, m_intBitmapSize);
            g.Dispose();
            m_radGradientIn.Image = bmpButton;
            
            
            // ADD CONTROLS
            
            m_panStyle.Controls.Add(m_labStyle);
            m_panStyle.Controls.Add(m_cboStyle);
            
            this.Controls.Add(m_panStyle);
            
            m_panSolidColour.Controls.Add(m_labColour);
            m_panSolidColour.Controls.Add(m_btnColour);
            
            m_gbxSolidColour.Controls.Add(m_panSolidColour);
            
            this.Controls.Add(m_gbxSolidColour);
            
            m_panGradientColours.Controls.Add(m_labGradient);
            m_panGradientColours.Controls.Add(m_labColour1);
            m_panGradientColours.Controls.Add(m_btnColour1);
            m_panGradientColours.Controls.Add(m_labColour2);
            m_panGradientColours.Controls.Add(m_btnColour2);
            
            m_gbxGradient.Controls.Add(m_panGradientColours);
            
            m_panGradientDir.Controls.Add(m_labGradientDir);
            m_panGradientDir.Controls.Add(m_radGradientLR);
            m_panGradientDir.Controls.Add(m_radGradientTB);
            m_panGradientDir.Controls.Add(m_radGradientTLBR);
            m_panGradientDir.Controls.Add(m_radGradientTRBL);
            m_panGradientDir.Controls.Add(m_radGradientIn);
            
            m_gbxGradient.Controls.Add(m_panGradientDir);
            
            this.Controls.Add(m_gbxGradient);
            
            m_panSelectImage.Controls.Add(m_labImage);
            m_panSelectImage.Controls.Add(m_txtImage);
            m_panSelectImage.Controls.Add(m_btnImage);
            
            m_gbxImage.Controls.Add(m_panSelectImage);
            
            m_panSizing.Controls.Add(m_radTile);
            m_panSizing.Controls.Add(m_radStretch);
            m_panSizing.Controls.Add(m_radCentre);
            
            m_gbxImage.Controls.Add(m_panSizing);
            
            m_panSizingOptions.Controls.Add(m_chkKeepAspectRatio);
            m_panSizingOptions.Controls.Add(m_chkFillWithGradient);
            
            m_gbxImage.Controls.Add(m_panSizingOptions);
            
            this.Controls.Add(m_gbxImage);
            
            m_panButtons.Controls.Add(m_btnOK);
            m_panButtons.Controls.Add(m_btnCancel);
            
            this.Controls.Add(m_panButtons);
        }
        
        public BackgroundStyle BackgroundStyle
        {
            get
            {
                return m_BackgroundStyle;
            }
            set
            {
                if ((int)value < 1)
                {
                    m_BackgroundStyle = BackgroundStyle.SolidColour;
                }
                else
                {
                    m_BackgroundStyle = value;
                    // NB. assuming first style has value 1 and then increase by 1
                    m_cboStyle.SelectedIndex = (int)m_BackgroundStyle-1;
                    
                }
            }
        }
        
        public string FileFilter
        {
            get
            {
                return m_strFileFilter;
            }
            set
            {
                m_strFileFilter = value;
            }
        }
        
        public string BitmapPath
        {
            get
            {
                return m_strImageFileFullPath;
            }
            set
            {
                string strFullPath = "";
                try
                {
                    strFullPath = System.IO.Path.GetFullPath(value);
                }
                catch{}
                if (strFullPath != m_strImageFileFullPath)
                {
                    if (System.IO.File.Exists(strFullPath))
                    {
                        m_strImageFileFullPath = strFullPath;
                        try
                        {
                            m_strImageDir = 
                                System.IO.Path.GetDirectoryName(strFullPath);
                        }
                        catch
                        {
                            m_strImageDir = "";
                        }
                        try
                        {
                            m_strImageFile = 
                                System.IO.Path.GetFileName(strFullPath);
                        }
                        catch
                        {
                            m_strImageFile = "";
                        }
                        m_txtImage.Text = m_strImageFileFullPath;
                    }
                }
            }
        }
        
        public Color Colour
        {
            get
            {
                return m_SelectedColour;
            }
            set
            {
                m_SelectedColour = value;
                // redraw colour control
                Bitmap bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
                Graphics g = Graphics.FromImage(bmpButton);
                g.FillRectangle(new SolidBrush(value), 0, 0,
                    m_intBitmapSize, m_intBitmapSize);
                g.Dispose();
                m_btnColour.Image = bmpButton;
            }
        }
        
        public int[] CustomColors
        {
            get
            {
                return ma_intCustomColours;
            }
            set
            {
                for (int intIndex = 0; 
                     intIndex < Math.Min(ma_intCustomColours.Length,
                                          value.Length); intIndex++)
                {
                    ma_intCustomColours[intIndex] = value[intIndex];
                }
            }
        }
                
        public Color GradientColour1
        {
            get
            {
                return m_SelectedGradientColour1;
            }
            set
            {
                m_SelectedGradientColour1 = value;
                // redraw colour1 control
                Bitmap bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
                Graphics g = Graphics.FromImage(bmpButton);
                g.FillRectangle(new SolidBrush(value), 0, 0,
                    m_intBitmapSize, m_intBitmapSize);
                g.Dispose();
                m_btnColour1.Image = bmpButton;
            }
        }
        
        public Color GradientColour2
        {
            get
            {
                return m_SelectedGradientColour2;
            }
            set
            {
                m_SelectedGradientColour2 = value;
                // redraw colour2 control
                Bitmap bmpButton = new Bitmap(m_intBitmapSize, m_intBitmapSize);
                Graphics g = Graphics.FromImage(bmpButton);
                g.FillRectangle(new SolidBrush(value), 0, 0,
                    m_intBitmapSize, m_intBitmapSize);
                g.Dispose();
                m_btnColour2.Image = bmpButton;
            }
        }
        
        private void SwapGradientColours()
        {
            Color temp = this.GradientColour1;
            this.GradientColour1 = this.GradientColour2;
            this.GradientColour2 = temp;
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
                switch (value)
                {
                    case GradientDirection.LeftToRight:
                        m_radGradientLR.Checked = true;
                        break;
                    case GradientDirection.RightToLeft:
                        m_radGradientLR.Checked = true;
                        m_GradientDirection = GradientDirection.LeftToRight;
                        SwapGradientColours();
                        break;
                    case GradientDirection.TopToBottom:
                        m_radGradientTB.Checked = true;
                        break;
                    case GradientDirection.BottomToTop:
                        m_radGradientTB.Checked = true;
                        m_GradientDirection = GradientDirection.TopToBottom;
                        SwapGradientColours();
                        break;
                    case GradientDirection.TopLeftToBottomRight:
                        m_radGradientTLBR.Checked = true;
                        break;
                    case GradientDirection.BottomRightToTopLeft:
                        m_radGradientTLBR.Checked = true;
                        m_GradientDirection = GradientDirection.BottomRightToTopLeft;
                        SwapGradientColours();
                        break;
                    case GradientDirection.TopRightToBottomLeft:
                        m_radGradientTRBL.Checked = true;
                        break;
                    case GradientDirection.BottomLeftToTopRight:
                        m_radGradientTRBL.Checked = true;
                        m_GradientDirection = GradientDirection.TopRightToBottomLeft;
                        SwapGradientColours();
                        break;
                    case GradientDirection.Inwards:
                        m_radGradientIn.Checked = true;
                        break;
                    case GradientDirection.Outwards:
                        m_radGradientIn.Checked = true;
                        m_GradientDirection = GradientDirection.Inwards;
                        SwapGradientColours();
                        break;
                    default:
                        m_GradientDirection = GradientDirection.LeftToRight;
                        m_radGradientLR.Checked = true;
                        break;
                }
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
                if (m_radTile.Checked && !value)
                {
                    // check next option
                    this.Stretch = true;
                }
                else
                {
                    m_radTile.Checked = value;
                }
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
                if (m_radStretch.Checked && !value)
                {
                    // check next option
                    this.Centre = true;
                }
                else
                {
                    m_radStretch.Checked = value;
                }
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
                if (m_radCentre.Checked && !value)
                {
                    // check next option
                    this.Tile = true;
                }
                else
                {
                    m_radCentre.Checked = value;
                }
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
                m_chkKeepAspectRatio.Checked = value;
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
                m_chkFillWithGradient.Checked = value;
            }
        }
        
        private void Tile_CheckedChanged(Object sender, EventArgs ev)
        {
            this.Tile = m_radTile.Checked;
            if (m_radTile.Checked)
            {
                m_chkKeepAspectRatio.Enabled = false;
                m_chkFillWithGradient.Enabled = false;
            }
            // change style to image
            this.BackgroundStyle = BackgroundStyle.Bitmap;
        }
        
        private void Stretch_CheckedChanged(Object sender, EventArgs ev)
        {
            this.Stretch = m_radStretch.Checked;
            if (m_radStretch.Checked)
            {
                m_chkKeepAspectRatio.Enabled = true;
                if (m_chkKeepAspectRatio.Checked)
                {
                    m_chkFillWithGradient.Enabled = true;
                }
                else
                {
                    m_chkFillWithGradient.Enabled = false;
                }
            }
            // change style to image
            this.BackgroundStyle = BackgroundStyle.Bitmap;
        }
        
        private void Centre_CheckedChanged(Object sender, EventArgs ev)
        {
            this.Centre = m_radCentre.Checked;
            if (m_radCentre.Checked)
            {
                m_chkKeepAspectRatio.Enabled = false;
                m_chkFillWithGradient.Enabled = true;
            }
            // change style to image
            this.BackgroundStyle = BackgroundStyle.Bitmap;
        }
        
        private void KeepAspectRatio_CheckedChanged(Object sender, EventArgs ev)
        {
            this.KeepAspectRatio = m_chkKeepAspectRatio.Checked;
            if (m_chkKeepAspectRatio.Checked)
            {
                m_chkFillWithGradient.Enabled = true;
            }
            else
            {
                m_chkFillWithGradient.Enabled = false;
            }
            // change style to image
            this.BackgroundStyle = BackgroundStyle.Bitmap;
        }
        
        private void FillWithGradient_CheckedChanged(Object sender, EventArgs ev)
        {
            this.FillWithGradient = m_chkFillWithGradient.Checked;
            
            // change style to image
            this.BackgroundStyle = BackgroundStyle.Bitmap;
        }
        
        private void GradientLR_CheckedChanged(Object sender, EventArgs ev)
        {
            if (m_radGradientLR.Checked)
            {
                this.GradientDirection = GradientDirection.LeftToRight;
            }
            // change style to gradient
            this.BackgroundStyle = BackgroundStyle.Gradient;
        }
        
        private void GradientTB_CheckedChanged(Object sender, EventArgs ev)
        {
            if (m_radGradientTB.Checked)
            {
                this.GradientDirection = GradientDirection.TopToBottom;
            }
            // change style to gradient
            this.BackgroundStyle = BackgroundStyle.Gradient;
        }
        
        private void GradientTLBR_CheckedChanged(Object sender, EventArgs ev)
        {
            if (m_radGradientTLBR.Checked)
            {
                this.GradientDirection = GradientDirection.TopLeftToBottomRight;
            }
            // change style to gradient
            this.BackgroundStyle = BackgroundStyle.Gradient;
        }
        
        private void GradientTRBL_CheckedChanged(Object sender, EventArgs ev)
        {
            if (m_radGradientTRBL.Checked)
            {
                this.GradientDirection = GradientDirection.TopRightToBottomLeft;
            }
            // change style to gradient
            this.BackgroundStyle = BackgroundStyle.Gradient;
        }
        
        private void GradientIn_CheckedChanged(Object sender, EventArgs ev)
        {
            if (m_radGradientIn.Checked)
            {
                this.GradientDirection = GradientDirection.Inwards;
            }
            // change style to gradient
            this.BackgroundStyle = BackgroundStyle.Gradient;
        }
        
        private void Colour_Click(Object sender, EventArgs ev)
        {
            ColorDialog dlgColor = new ColorDialog();
            dlgColor.FullOpen = true;
            dlgColor.Color = this.Colour;
            dlgColor.CustomColors = this.CustomColors;
            
            if (dlgColor.ShowDialog() == DialogResult.OK)
            {
                this.CustomColors = dlgColor.CustomColors;
                
                Color newColor = dlgColor.Color;
                if (newColor.R != this.Colour.R ||
                    newColor.G != this.Colour.G ||
                    newColor.B != this.Colour.B)
                {
                    // new colour
                    // set background, overwriting any alpha value so that
                    // it is opaque
                    this.Colour = 
                        Color.FromArgb(255, newColor.R, 
                                            newColor.G,
                                            newColor.B);
                }
                // change style to solid colour
                this.BackgroundStyle = BackgroundStyle.SolidColour;
            }
        }
        
        private void Colour1_Click(Object sender, EventArgs ev)
        {
            ColorDialog dlgColor = new ColorDialog();
            dlgColor.FullOpen = true;
            dlgColor.Color = this.GradientColour1;
            dlgColor.CustomColors = this.CustomColors;
            
            if (dlgColor.ShowDialog() == DialogResult.OK)
            {
                this.CustomColors = dlgColor.CustomColors;
                
                Color newColor = dlgColor.Color;
                if (newColor.R != this.GradientColour1.R ||
                    newColor.G != this.GradientColour1.G ||
                    newColor.B != this.GradientColour1.B)
                {
                    // new colour
                    // set background, overwriting any alpha value so that
                    // it is opaque
                    this.GradientColour1 = 
                        Color.FromArgb(255, newColor.R, 
                                            newColor.G,
                                            newColor.B);
                }
                // change style to gradient
                this.BackgroundStyle = BackgroundStyle.Gradient;
            }
        }
        
        private void Colour2_Click(Object sender, EventArgs ev)
        {
            ColorDialog dlgColor = new ColorDialog();
            dlgColor.FullOpen = true;
            dlgColor.Color = this.GradientColour2;
            dlgColor.CustomColors = this.CustomColors;
            
            if (dlgColor.ShowDialog() == DialogResult.OK)
            {
                this.CustomColors = dlgColor.CustomColors;
                
                Color newColor = dlgColor.Color;
                if (newColor.R != this.GradientColour2.R ||
                    newColor.G != this.GradientColour2.G ||
                    newColor.B != this.GradientColour2.B)
                {
                    // new colour
                    // set background, overwriting any alpha value so that
                    // it is opaque
                    this.GradientColour2 = 
                        Color.FromArgb(255, newColor.R, 
                                            newColor.G,
                                            newColor.B);
                }
                // change style to gradient
                this.BackgroundStyle = BackgroundStyle.Gradient;
            }
        }

        private void Browse_Click(Object sender, EventArgs ev)
        {
            OpenFileDialog dlgFileOpen = new OpenFileDialog();
            dlgFileOpen.Title = "Select Background Image";
            dlgFileOpen.InitialDirectory = m_strImageDir;
            dlgFileOpen.FileName         = this.BitmapPath;
            dlgFileOpen.Filter = m_strFileFilter;
                ;
            dlgFileOpen.FilterIndex = 0;
            
            if (dlgFileOpen.ShowDialog() == DialogResult.OK)
            {
                if (dlgFileOpen.FileName != m_strImageFileFullPath)
                {
                    string strImageFileFullPath = m_strImageFileFullPath;
                    string strImageDir = m_strImageDir;
                    string strImageFile = m_strImageFile;
                    m_strImageFileFullPath = dlgFileOpen.FileName;
                    try
                    {
                        m_strImageDir = 
                            System.IO.Path.GetDirectoryName(m_strImageFileFullPath);
                        m_strImageFile = 
                            System.IO.Path.GetFileName(m_strImageFileFullPath);
                        m_txtImage.Text = m_strImageFileFullPath;
                        // change style to image
                        this.BackgroundStyle = BackgroundStyle.Bitmap;                        
                    }
                    catch
                    {
                        // reset to previous
                        m_strImageFileFullPath = strImageFileFullPath;
                        m_strImageDir = strImageDir;
                        m_strImageFile = strImageFile;
                    }
                }       
            }
        }

        private void OK_Click(Object sender, EventArgs ev)
        {
            this.DialogResult = DialogResult.OK;
        }
 
        private void Style_SelectedIndexChanged(Object sender, EventArgs ev)
        {
            m_BackgroundStyle = (BackgroundStyle)(m_cboStyle.SelectedIndex+1);
        }
        
    }
}