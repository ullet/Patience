Patience: An extensible graphical card game system.
Version 0.1.0 (13 January 2005)
Copyright (C) 2005 Trevor Barnett
 
http://www.e381.net/patience/
 
Please send comments or questions about this program to swst@e381.net
 
---------------------------------------------------------------------------

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public
License as published by the Free Software Foundation; either
version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
General Public License for more details.

You should have received a copy of the GNU General Public License along 
with this program (in a file called LICENSE.txt); if not, go to
http://www.gnu.org/copyleft/gpl.html or write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA

---------------------------------------------------------------------------

Source code to this program is always available; for more
information visit the application website at:
http://www.e381.net/patience/

---------------------------------------------------------------------------

The complete binary archive for this program consists of the following
executables and libraries:
  Patience.exe
  Cards.dll
  CardGames.dll
  GraphicCardGames.dll
  BeleagueredCastle.dll
  Klondike.dll
  ScalableCards.dll
  
Each of above libraries is individually released under the terms of the
GNU General Public License as described above.

Compiling and Running the Program
---------------------------------
This program is written in C# .NET.  
To compile this program:
1) Open a command prompt.
2) Change the current directory to the source folder.
3) Type "make" to run the compile batch script.  
A subfolder called "bin" will be created inside the source folder containing
the compiled application.
To install the application simply copy everything inside the bin folder to
the desired location.
The compile script requires that the C# compiler, csc, is in your executable
path.  The C# compiler is available as part of the free 
Microsoft(R) .NET Framework SDK Version 1.1, which can be downloaded from 
the Microsoft(R) Download Center:
http://www.microsoft.com/downloads/details.aspx?familyid=9B3A2CA6-3647-4070-9F41-A333C6B9181D
There is not currently a solution file for Microsoft(R) Visual 
Studio(R) .NET 2003.  The application may not work as expected or at all 
if built using Visual Studio.

To run this program, version 1.1 of the .NET Framework
must be installed.  The .NET Framework 1.1 for Windows can be downloaded 
using Windows Update or by following the instructions at
http://msdn.microsoft.com/netframework/downloads/howtoget.aspx