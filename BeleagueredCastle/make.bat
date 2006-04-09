@echo off

csc /target:library /lib:..\bin\ /r:Cards.dll;CardGames.dll;GraphicCardGames.dll /out:..\bin\Games\BeleagueredCastle.dll BeleagueredCastleCardGame.cs BeleagueredCastleControl.cs AssemblyInfo.cs