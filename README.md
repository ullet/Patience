# Patience: An extensible graphical card game system.
Copyright (C) 2003, 2005, 2015 Trevor Barnett

Released under the terms of the GNU General Public License, version 2.
See file LICENSE for full details.

## About
Patience is "an extensible graphical card game system".  It is a framework for
solitaire/patience single player card games.  A "plug-in" system allows it to be
extended with new games and new card set formats, providing potential to use
"pretty" picture cards designed for other card game applications.
Out of the box two games are available: Beleagured Castle and the "standard"
patience game Klondike.

## Compiling
Open the Visual Studio 2013 solution and build it.

A bin folder will be created in the root with the "deployed" application
containing the following files and folders:

 * Patience.exe
 * Cards.dll
 * CardGames.dll
 * GraphicCardGames.dll
 * CardSetFormats/
   * ScalableCards.dll
 * Games/
   * BeleaguredCastle.dll
   * Klondike.dll

The `CardSetFormats` folder must contain at least one card set format dll, and
the `Games` folder must contain at least one game dll.

## History
Way back in 2003, when the dot-com bubble burst for my then employer, I found
myself suddenly between jobs and in need of updating my skills. .NET and C# were
the next big thing back then, so I decided to master those.

As I couldn't afford to buy Visual Studio, I made do with the free SDK and a
text editor ([UltraEdit](http://www.ultraedit.com/) 8), but, as many a seasoned developer will tell you, it
was also the best way to learn.

Since there's nothing like jumping in with both feet into the deep end, I set
out to create not just a card game, but a whole extensisble card game system,
with dynamically loaded plugins, scalable graphics, and font-sensitive dialog
windows.  I don't think I did too bad for a first try!

I chose the probably lesser known game of Beleagured Castle as it was one of my
favourite patience games, and the only electronic version I knew of was the
version I'd written for the Amiga some years earlier.

What you see in this repository is mostly the original code from 2003, tweaked
a bit in 2005, and just recently "modernised" to use Visual Studio, but
otherwise unchanged in 10 years.

Looking back at this code now, I'm not completely disgusted by it, but it is far
from what I'd consider now to be good code.
Amongst the low-lights are the hungarian notation, including the scope prefixes,
the very long classes and files, and the complete absence of unit tests (which I
hadn't even heard of 10 years ago).

## How to play

### General

 * Drag and drop cards to make moves.  Card(s) will return to original position if attempt to make an illegal move.
 * Use menu to start new game or switch between games.
 * Use menu to "auto complete" if possible to win and near the end of the game with all cards face up.

### Rules of Beleaguered Castle

#### Deck
The game is played with a standard deck of 52 cards.

#### Layout
The aces are separated from the rest of the deck 
and laid face up in a single column down the centre of the table to form 
the foundations.  The remaining 48 cards are shuffled thoroughly and then
laid out face up in 8 overlapping rows, of 6 cards each, at each side of 
the foundation piles.

#### Objective
To move all the cards from the rows to the foundations.

#### Game Play
At each turn a single card is moved from one of the rows to either the 
foundation or the end of another row.  Rows are built in ascending 
numerical order, regardless of suit or suit colour.  That is, a Queen may 
only be placed on a King, a Jack only on a Queen and so on.  Any card may 
be moved from a row to an empty row.
The foundations are built up from Ace to King by suit, that is the Two of 
Spades may only go on the foundation with the top card of Ace of Spades, 
the Seven of Hearts may only go on the Six of Hearts and so on.  Once 
placed on a foundation pile a card may not be moved off again.  The game 
ends when the objective is complete, i.e. all cards have been correctly 
placed on the foundations, in which case the game is won, or if there are 
still cards on the rows and there are no more legal moves, in which case 
the game is lost.

### Rules of Klondike

See [http://en.wikipedia.org/wiki/Klondike_(solitaire)](http://en.wikipedia.org/wiki/Klondike_%28solitaire%29)

Uses following variation of the rules:

 * Turning three cards at once, reversing the order of each group of three as the cards are dealt, placing no limit on passes through the deck.
 * Once placed on a foundation pile a card may not be moved off again.
