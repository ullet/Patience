/*
 *************************************************************************
 * Patience: An extensible graphical card game system.                   *
 * Version 0.1.0 (12 January 2005)                                       *
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
 * File:          CreateResources.cs                                     *
 * Last modified: 16 September 2003                                      *
 * Description:   Create resources file for Patience.exe                 *
 *************************************************************************
 */

using System;
using System.Resources;
using System.Drawing;

class CR
{
    static void Main(string[] args)
    {
       ResourceWriter rw = new ResourceWriter("patience.resources");
       Icon icoWindow = new Icon("cardgames_small.ico");
       rw.AddResource("icoWindow", icoWindow);
       rw.Close();
    }
}