@echo off

csc /target:library /lib:..\bin\ /r:Cards.dll;CardGames.dll;GraphicCardGames.dll /out:..\bin\Games\Klondike.dll KlondikeCardGame.cs KlondikeControl.cs AssemblyInfo.cs