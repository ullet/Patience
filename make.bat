@echo off

mkdir bin >NUL
mkdir bin\CardsetFormats >NUL
mkdir bin\Games >NUL

:build
cd Cards
call make

cd ..\CardGames
call make

cd ..\GraphicCardGames
call make

cd ..\ScalableCards
call make

cd ..\BeleagueredCastle
call make

cd ..\Klondike
call make

cd ..\Patience
call make
cd ..\

copy /Y LICENSE.txt bin\
copy /Y README.txt bin\
copy /Y Rules.txt bin\
