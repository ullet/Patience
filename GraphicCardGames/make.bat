@echo off

csc /t:library /lib:..\bin\ /r:CardGames.dll;Cards.dll /out:..\bin\GraphicCardGames.dll CardSet.cs CardGameWindow.cs CardGameControl.cs Background.cs BackgroundDialog.cs Zones.cs CardTableZones.cs AssemblyInfo.cs PluginManager.cs