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
using System.IO;
using System.Reflection;
using System.Collections;

namespace SwSt
{
    public class Plugin
    {
        private Assembly m_Assembly;
        private Type     m_Type;
        
        public Plugin()
        {
        }
        
        public Plugin(Assembly assembly, Type type)
        {
            this.Assembly = assembly;
            this.Type     = type;
        }
        
        public Assembly Assembly
        {
            get
            {
                return m_Assembly;
            }
            set
            {
                m_Assembly = value;
            }
        }
        
        public Type Type
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
    }
    
    public class PluginList : CollectionBase
    {
        public PluginList() : base()
        {
        }
        
        public void Add(Plugin newPlugin)
        {
            this.List.Add(newPlugin);
        }
        
        public void Remove(Plugin newPlugin)
        {
            this.List.Add(newPlugin);
        }
        
        public Plugin this[int intCardIndex]
        {
            get
            {
                return (Plugin)List[intCardIndex];
            }
            set
            {
                List[intCardIndex] = value;
            }
        }
    }        
    
    public class PluginManager
    {
        public static PluginList GetPlugins(string strPath, Type interfaceType)
        {
            return GetPlugins(strPath, new Type[]{interfaceType}, new Type[]{});
        }
        
        public static PluginList GetPlugins(
            string strPath, 
            Type[] a_InterfaceTypes)
        {
            return GetPlugins(strPath, a_InterfaceTypes, new Type[]{});
        }
        
        public static PluginList GetPlugins(
            string strPath, 
            Type[] a_InterfaceTypes,
            Type[] a_ParentClassTypes)
        {
            PluginList plugins = new PluginList();
            
            string[] a_strAssemblies;
            try
            {
                a_strAssemblies = 
                    Directory.GetFileSystemEntries(strPath, "*.dll");
            }
            catch
            {
                return plugins;
            }
            
            for (int intIndex=0; intIndex<a_strAssemblies.Length; intIndex++)
            {
                try
                {
                    Assembly testAssembly = 
                        Assembly.LoadFrom(a_strAssemblies[intIndex]);
                    
                    // loop through each type in the Assembly
                    foreach (Type type in testAssembly.GetTypes())
                    {
                        // only look at public classes 
                        if (type.IsPublic)
                        {
                            // skip abstract classes
                            if (!((type.Attributes & TypeAttributes.Abstract) ==
                                  TypeAttributes.Abstract))
                            {
                                bool blnValidType = true;
                                // check type implements all interfaces
                                for (int intCounter=0; 
                                     intCounter<a_ParentClassTypes.Length && 
                                     blnValidType;
                                     intCounter++)
                                {
                                    bool blnIsSubClass = false;
                                    try
                                    { 
                                        Type typParent = 
                                            a_ParentClassTypes[intCounter];
                                        if (typParent != null)
                                        {
                                            blnIsSubClass = type.IsSubclassOf(
                                                typParent);
                                            
                                        }
                                        
                                    }
                                    catch
                                    {
                                        blnIsSubClass = false;
                                    }
                                    if (!blnIsSubClass)
                                    {
                                        // type not a subclass
                                        blnValidType = false;
                                    }
                                }
                                
                                // check type implements all interface
                                for (int intCounter=0; 
                                     intCounter<a_InterfaceTypes.Length && 
                                     blnValidType;
                                     intCounter++)
                                {
                                    Type objInterface = 
                                        type.GetInterface(
                                            a_InterfaceTypes[intCounter].FullName);
                
                                    if (objInterface == null)
                                    {
                                        // type does not implement interface
                                        blnValidType = false;
                                    }
                                }
                                
                                if (blnValidType)
                                {
                                    // type implements all interfaces
                                    plugins.Add(new Plugin(testAssembly, type));
                                }
                            } 
                        } 
                    } // next type
                }
                catch
                {
                }
            } // next assembly
            
            return plugins;
        }
    }
}