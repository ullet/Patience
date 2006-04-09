@echo off

csc /t:library /r:..\bin\Cards.dll /out:..\bin\CardGames.dll CardGame.cs CardTableRegion.cs Move.cs AssemblyInfo.cs