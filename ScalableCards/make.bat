@echo off

csc /t:library /lib:..\bin /out:..\bin\CardSetFormats\ScalableCards.dll /reference:Cards.dll /res:cardimages.resources ScalableCards.cs AssemblyInfo.cs