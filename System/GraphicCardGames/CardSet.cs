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
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using SwSt;
using SwSt.Cards;

// ------------------------------------------------------ //

namespace SwSt
{
    namespace CardGames
    {
        // ------------------------------------------------------ //
        
        public class CardSet : Cards.CardSet
        {      
            private enum CardSetType
            {
                Unspecified     = 0,
                ScalableCardSet = 1,
                PluginCardSet   = 2
            };
            
            private const string          mc_strPluginCardSetDirPath = 
                "CardsetFormats\\";
            private static PluginList     m_Plugins = new PluginList();
            private PluginCardSetList     m_pluginCardSets = 
                new PluginCardSetList();
            private Cards.ICardSet        m_PluginCardSet   = null;
            private Cards.ICardSet        m_DefaultCardSet = null;
            private Cards.ICardSet        m_ActiveCardSet = null;
            private System.Drawing.Size   m_MinSize        = 
                new System.Drawing.Size(0,0);
            private System.Drawing.Size   m_MinPluginCardSetSize    = 
                new System.Drawing.Size(0,0);
            
            private static bool           m_blnPluginsChecked = false;
            
            
            public CardSet() : this("")
            {
            }
            
            public CardSet(string strDefaultCardSetID) : base()
            {
                // set up card collections without bitmaps
                
                // check for plugins
                try
                {
                    if (PluginCardSetAvailable)
                    {
                        //Create class instance
                        foreach (Plugin plugin in CardSet.m_Plugins)
                        {
                            ICardSet cardSet = (ICardSet)
                                plugin.Assembly.CreateInstance
                                (plugin.Type.FullName);
                                
                            // add instance to cardsets collection
                            m_pluginCardSets.Add((ICardSet)
                                plugin.Assembly.CreateInstance
                                    (plugin.Type.FullName));
                                    
                            // check if this card set is the default
                            if (strDefaultCardSetID != null && 
                                strDefaultCardSetID != "" &&
                                cardSet.CardSetID == strDefaultCardSetID &&
                                cardSet.IsSelfContained)
                            {
                                m_DefaultCardSet = cardSet;;
                            }
                            
                        }
                        if (m_DefaultCardSet == null)
                        {
                            // look fot first self-contained card set
                            // and use that as default
                            foreach (ICardSet cardSet in m_pluginCardSets)
                            {
                                if (cardSet.IsSelfContained)
                                {
                                    m_DefaultCardSet = cardSet;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    m_PluginCardSet  = null;
                    m_DefaultCardSet = null;
                }
                
                // start with active card set set to default
                m_ActiveCardSet = m_DefaultCardSet;
            }
            
            // ------------------------------------------------------ //

            public override int UnscaledWidth
            {
                get
                {
                    if (m_ActiveCardSet == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return m_ActiveCardSet.UnscaledWidth;
                    }
                }
            }
                
            // ------------------------------------------------------ //
                
            public override int UnscaledHeight
            {
                get
                {
                    if (m_ActiveCardSet == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return m_ActiveCardSet.UnscaledHeight;
                    }
                }
            }
            
            private static string PluginCardSetDirPath
            {
                get
                {
                    string strExePath = Assembly.GetEntryAssembly().Location;
                    
                    string strEntryAssemblyDirPath = "";
                    int intPos = strExePath.LastIndexOf("\\");
                    if (intPos > 0)
                    {
                        strEntryAssemblyDirPath = 
                            strExePath.Substring(0, intPos+1);
                    }
                    
                    return strEntryAssemblyDirPath + mc_strPluginCardSetDirPath;
                }
            }
            
            // -------------------------------- //
            
            public StackCard DefaultStackCard(int intStackNumber)
            {
                return null;
            }
            
            // -------------------------------- //
            
            public CardBack DefaultCardBack
            {
                get
                {
                    return null;
                }
            }
            
            // -------------------------------- //
            
            public CardSpace DefaultCardSpace
            {
                get
                {
                    return null;
                }
            }
            
            // -------------------------------- //
            
            public static bool PluginCardSetAvailable
            {
                get
                {
                    if (!m_blnPluginsChecked)
                    {
                        // check for plugins
                        m_Plugins = 
                            PluginManager.GetPlugins(
                                PluginCardSetDirPath,
                                typeof(SwSt.Cards.ICardSet));
                          
                        m_blnPluginsChecked = true;
                    }
                        
                    if (m_Plugins == null || m_Plugins.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }       
            
            public bool IsDefault
            {
                get
                {
                    if (m_ActiveCardSet == m_DefaultCardSet)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
            public override int MinWidth
            {
                get
                {
                    int intMin = (int)Math.Pow(2,16);
                    if (m_DefaultCardSet != null)
                    {
                        intMin = Math.Min(intMin, m_DefaultCardSet.MinWidth);
                    }
                    if (m_PluginCardSet != null)
                    {
                        intMin = Math.Min(intMin, m_PluginCardSet.MinWidth);
                    }
                    return intMin;
                }
            }
            
            public override int MinHeight
            {
                get
                {
                    int intMin = (int)Math.Pow(2,16);
                    if (m_DefaultCardSet != null)
                    {
                        intMin = Math.Min(intMin, m_DefaultCardSet.MinHeight);
                    }
                    if (m_PluginCardSet != null)
                    {
                        intMin = Math.Min(intMin, m_PluginCardSet.MinHeight);
                    }
                    return intMin;
                }
            }
            
            public System.Drawing.Size MinSize
            {
                get
                {
                    return m_MinSize;
                }
                set
                {
                    m_MinSize.Width  = value.Width;
                    m_MinSize.Height = value.Height; 
                }
            }
            
            public override bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional)
            {
                return Resize(intNewWidth, intNewHeight, blnProportional, true, true);
            }
            
            public override bool Resize(int intNewWidth, int intNewHeight, 
                bool blnProportional, bool blnScaleUp)
            {
                return Resize(intNewWidth, intNewHeight, blnProportional, blnScaleUp, true);
            }
            
            public override bool Resize(int intNewWidth, int intNewHeight, bool blnProportional,
                                   bool blnScaleUp, bool blnScaleDown)
            {
                bool blnResized = false;
                                
                if (m_ActiveCardSet != m_DefaultCardSet)
                {
                    if (intNewWidth  < m_ActiveCardSet.MinWidth || 
                        intNewHeight < m_ActiveCardSet.MinHeight)
                    {
                        // size too small if less than minimum
                        // use default instead
                        m_ActiveCardSet = m_DefaultCardSet;
                        m_ActiveCardSet.UseInternal();
                    }
                    // else keep using plugin cardset
                }
                else
                {
                    // currently default
                    if (m_PluginCardSet != null &&
                        intNewWidth  >= m_PluginCardSet.MinWidth && 
                        intNewHeight >= m_PluginCardSet.MinHeight)
                    {
                        // plugin not null and not below minimum
                        // so switch back to plugin
                        m_ActiveCardSet = m_PluginCardSet;
                    }
                    else if (intNewWidth  < m_ActiveCardSet.MinWidth || 
                             intNewHeight < m_ActiveCardSet.MinHeight)
                    {
                        // using default so limit to minimum size
                        intNewWidth  = m_ActiveCardSet.MinWidth;
                        intNewHeight = m_ActiveCardSet.MinHeight;
                    }
                    // else keep using default cardset
                }
                    
                    
                // resize card set
                blnResized = m_ActiveCardSet.Resize
                    (intNewWidth, intNewHeight, blnProportional,
                     blnScaleUp, blnScaleDown);
                        
                m_intWidth  = m_ActiveCardSet.Width;
                m_intHeight = m_ActiveCardSet.Height;
                CopyCardsetBitmaps(m_ActiveCardSet);
                
                return blnResized;
            }                
            
            public override bool Load(string strCardSetPath)
            {
                return Load(strCardSetPath, "", true);
            }
            
            public bool Load(string strCardSetPath, 
                string strDefaultPluginCardSetPath)
            {
                return Load(strCardSetPath, strDefaultPluginCardSetPath, true);
            }
            
            public bool Load(string strCardSetPath, bool blnLoadDefault)
            {
                return Load(strCardSetPath, "", blnLoadDefault);
            }
            
            ///<summary>
            ///Copy cardset bitmap references
            ///</summary>
            public void CopyCardsetBitmaps(ICardSet cardSet)
            {
                if (cardSet == null)
                {
                    return;
                }
                
                Card myCard = null;
                foreach (Card card in cardSet.PlayingCards)
                {
                    myCard = this.Card(card.Suit, card.Rank);
                    if (myCard != null)
                    {
                        if (card.Bitmap != null)
                        {
                            myCard.Bitmap = card.Bitmap;
                        }
                        else
                        {
                            myCard.Bitmap = Cards.Card.BorderBitmap(
                                cardSet.Width,
                                cardSet.Height,
                                Color.Black);
                        }
                    }
                }
                foreach (StackCard card in cardSet.StackCards)
                {
                    myCard = this.StackCard(card.StackNumber);
                    if (myCard != null)
                    {
                        if (card.Bitmap != null)
                        {
                            myCard.Bitmap = card.Bitmap;
                        }
                        else
                        {
                            myCard.Bitmap = Cards.Card.BorderBitmap(
                                cardSet.Width,
                                cardSet.Height,
                                Color.Black);
                        }
                    }
                }
                if (m_CardBack != null)
                {
                    if (cardSet.CardBack != null && 
                        cardSet.CardBack.Bitmap != null)
                    {
                        m_CardBack.Bitmap = cardSet.CardBack.Bitmap;
                    }
                    else
                    {
                        m_CardBack.Bitmap = Cards.Card.BorderBitmap(
                            cardSet.Width,
                            cardSet.Height,
                            Color.Black);
                    }
                }
                if (m_CardSpace != null)
                {
                    if (cardSet.CardSpace != null && 
                        cardSet.CardSpace.Bitmap != null)
                    {
                        m_CardSpace.Bitmap = cardSet.CardSpace.Bitmap;
                    }
                    else
                    {
                        m_CardSpace.Bitmap = Cards.Card.BorderBitmap(
                            cardSet.Width,
                            cardSet.Height,
                            Color.Black);
                    }
                }
            }
            
            
            public bool Load(string strCardSetPath, 
                             string strDefaultPluginCardSetPath, 
                             bool blnLoadDefault)
            {
                bool blnFoundPlugin = false;
                m_PluginCardSet = null;
                for (int intCounter=0; 
                     intCounter < m_pluginCardSets.Count && !blnFoundPlugin; 
                     intCounter++)
                {
                    ICardSet pluginCardSet = m_pluginCardSets[intCounter];
                    if (pluginCardSet.IsValid(strCardSetPath))
                    {
                        m_PluginCardSet = pluginCardSet;
                        blnFoundPlugin = true;
                    }
                    pluginCardSet = null;
                }
                
                if (m_PluginCardSet != null && m_PluginCardSet.Load(strCardSetPath))
                {
                    CopyCardsetBitmaps(m_PluginCardSet);
                    m_intWidth           = m_PluginCardSet.Width;
                    m_intHeight          = m_PluginCardSet.Height;
                    m_ActiveCardSet = m_PluginCardSet;
                    
                    return true;
                }
                
                // plugin card set didn't load
                
                if (strDefaultPluginCardSetPath != "" && 
                         strCardSetPath != strDefaultPluginCardSetPath)
                {
                    // try to load default plugin card set if not just tried
                    blnFoundPlugin = false;
                    m_PluginCardSet = null;
                    for (int intCounter=0; 
                         intCounter < m_pluginCardSets.Count && !blnFoundPlugin; 
                         intCounter++)
                    {
                        ICardSet pluginCardSet = m_pluginCardSets[intCounter];
                        if (pluginCardSet.IsValid(strCardSetPath))
                        {
                            m_PluginCardSet = pluginCardSet;
                            blnFoundPlugin = true;
                        }
                        pluginCardSet = null;
                    }
                    
                    if (m_PluginCardSet != null &&
                        m_PluginCardSet.Load(strDefaultPluginCardSetPath))
                    {
                        CopyCardsetBitmaps(m_PluginCardSet);
                        m_intWidth           = m_PluginCardSet.Width;
                        m_intHeight          = m_PluginCardSet.Height;
                        
                        m_ActiveCardSet = m_PluginCardSet;
                        
                        return true;
                    }
                }
                
                // default plugin card set didn't load (or didn't try)
                   
                if(blnLoadDefault && m_DefaultCardSet != null)
                {
                    // try to use default card set
                    if(m_DefaultCardSet.UseInternal())
                    {
                        m_ActiveCardSet = m_DefaultCardSet;
                        CopyCardsetBitmaps(m_DefaultCardSet);
                        m_intWidth           = m_ActiveCardSet.Width;
                        m_intHeight          = m_ActiveCardSet.Height;
                           
                        return true;
                    }
                }
                
                // Not loaded
                m_ActiveCardSet = null;
                return false;
            }
            
            public bool Load(string strCardSetPath, int intMaxWidth, int intMaxHeight)
            {
                return Load(strCardSetPath, false, intMaxWidth, intMaxHeight, false);
            }
            
            public bool Load(string strCardSetPath, string strDefaultPluginCardSetPath,
                             int intMaxWidth, int intMaxHeight)
            {
                return Load(strCardSetPath, strDefaultPluginCardSetPath, false,
                            intMaxWidth, intMaxHeight, false);
            }
            
            public bool Load(string strCardSetPath, bool blnLoadDefault, int intMaxWidth, 
                             int intMaxHeight)
            {
                return Load(strCardSetPath, "", blnLoadDefault, intMaxWidth, intMaxHeight, 
                            false);
            }
            
            public bool Load(string strCardSetPath, string strDefaultPluginCardSetPath, 
                             bool blnLoadDefault, int intMaxWidth, int intMaxHeight)
            {
                return Load(strCardSetPath, strDefaultPluginCardSetPath, blnLoadDefault, 
                            intMaxWidth, intMaxHeight, false);
            }
            
            public bool Load(string strCardSetPath, string strDefaultPluginCardSetPath, 
                             int intMaxWidth, int intMaxHeight, bool blnScaleUp)
            {
                return Load(strCardSetPath, strDefaultPluginCardSetPath, false, 
                            intMaxWidth, intMaxHeight, blnScaleUp);
            }
            
            public bool Load(string strCardSetPath, bool blnLoadDefault, int intMaxWidth, 
                             int intMaxHeight, bool blnScaleUp)
            {
                return Load(strCardSetPath, "", blnLoadDefault, intMaxWidth, intMaxHeight, 
                            blnScaleUp);
            }
            
            public bool Load(string strCardSetPath, int intMaxWidth, int intMaxHeight,
                             bool blnScaleUp)
            {
                return Load(strCardSetPath, "", false, intMaxWidth, 
                            intMaxHeight, blnScaleUp);
            }
                  
            public bool Load(string strCardSetPath, string strDefaultPluginCardSetPath,
                             bool blnLoadDefault, int intMaxWidth, int intMaxHeight, 
                             bool blnScaleUp)
            {
                return Load(strCardSetPath, "", false, intMaxWidth, intMaxHeight, 
                            blnScaleUp, false);
            }
                             
            public bool Load(string strCardSetPath, string strDefaultPluginCardSetPath,
                             bool blnLoadDefault, int intMaxWidth, int intMaxHeight, 
                             bool blnScaleUp, bool blnScaleDown)
            {
                if (Load(strCardSetPath, strDefaultPluginCardSetPath, blnLoadDefault))
                {
                    if (intMaxWidth < this.Width || intMaxHeight < this.Height ||
                        (blnScaleUp && 
                         (intMaxWidth > this.Width || intMaxHeight > this.Height)))
                    {
                        // scale proportionally
                        this.Resize(intMaxWidth, intMaxHeight, true, 
                                    blnScaleUp, blnScaleDown);
                    }
                    
                    return true;
                }
                    
                return false;
            } 
            
            public override string[] FileFilter
            {
                get
                {
                    string[] a_strFilter;
                    ArrayList a_Filter = new ArrayList();
                    foreach (ICardSet cardset in m_pluginCardSets)
                    {
                        string[] a_strCardSetFilter = cardset.FileFilter;
                        for (int intIndex=0; 
                             intIndex<a_strCardSetFilter.Length; intIndex++)
                        {
                            string strCardSetFilter = a_strCardSetFilter[intIndex];
                                
                            if (strCardSetFilter != "")
                            {
                                string strPattern = "";
                                int intPos = strCardSetFilter.LastIndexOf("|");
                                if (intPos >=0 && intPos < strCardSetFilter.Length)
                                {
                                    strPattern = 
                                        strCardSetFilter.Substring(intPos+1).Trim();
                                }
                                if (strPattern != "*.*")
                                {
                                    a_Filter.Add(strCardSetFilter);
                                }
                            }
                        }
                    }
                    // add "All Files" filter
                    a_Filter.Add("All Files|*.*");
                    a_strFilter = new string[a_Filter.Count];
                    a_Filter.CopyTo(a_strFilter);
                    
                    return a_strFilter;
                }
            }    
            
            // Module copyright  
            public override string Copyright
            {
                get
                {
                    return "";
                }
            }
            
            // Module author's contact email address
            public override string ContactEmail
            {
                get
                {
                    return "";
                }
                    
            }
            
            // Module author's website
            public override string Website
            {
                get
                {
                    return "";
                }
            }
                        
            // 
            public override string Version
            {
                get
                {
                    return "";
                }
            }
            
            // Cardset Format name 
            public override string ModuleName
            {
                get
                {
                    return "";
                }
            }
            
            // Cardset Format copyright  
            public override string FormatCopyright
            {
                get
                {
                    return "";
                }
            }
            
            // Cardset Format copyright holders contact email address
            public override string FormatContactEmail
            {
                get
                {
                    return "";
                }
            }
            
            // Cardset Format copyright holders website
            public override string FormatWebsite
            {
                get
                {
                    return "";
                }
            }     
            
            public override ICardSet[] ChildCardSets
            {
                get
                {
                    ICardSet[] cardsets = null;
                    
                    if (m_pluginCardSets != null)
                    {
                        if (m_pluginCardSets.Count > 0)
                        {
                            cardsets = new ICardSet[m_pluginCardSets.Count];
                            
                            for (int intIndex=0; 
                                 intIndex<m_pluginCardSets.Count; 
                                 intIndex++)
                            {
                                cardsets[intIndex] =
                                    m_pluginCardSets[intIndex];
                            }
                            
                        }
                    }
                    return cardsets;
                }
            }
        }
        
        // ------------------------------------------------------ //
    }
    
    // ------------------------------------------------------ //
    
    public class PluginCardSetList : CollectionBase
    {
        public PluginCardSetList() : base()
        {
        }
        
        public void Add(ICardSet pluginCardSet)
        {
            this.List.Add(pluginCardSet);
        }
        
        public void Remove(ICardSet pluginCardSet)
        {
            this.List.Remove(pluginCardSet);
        }
        
        public ICardSet this[int intCardIndex]
        {
            get
            {
                return (ICardSet)List[intCardIndex];
            }
            set
            {
                List[intCardIndex] = value;
            }
        }
    }
}