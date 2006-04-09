/*
 *************************************************************************
 * ScalableCards.dll: Library to create and manipulate scalable cards.   *
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
 * Last modified: 30 August 2003                                         *
 * Description:   Create resources file for ScalableCards.dll            *
 *************************************************************************
 */

using System;
using System.Resources;
using System.Drawing;

class DefaultCards
{
    static void Main(string[] args)
    {
        string[] a_strSuits = {"Clubs", "Spades", "Diamonds", "Hearts"};
        string[] a_strRankSizes = {"11x13", "15x17", "18x20", "22x25", "29x33"};
        string[] a_strSuitSizes = {"9x9", "13x13", "15x15", "17x17", "19x19", "22x22", 
                                   "27x27", "30x30", "35x35"};
        string[] a_strRoyalSizes = {"30x30", "44x44", "58x58", "76x76", "86x86", 
                                    "97x97", "124x124"};
        
        ResourceWriter rw = new ResourceWriter("cardimages.resources");
        Bitmap bmp;
        // rank numbers
        for (int intRank=1; intRank<=13; intRank++)
        {
            for (int intRankSize=0; intRankSize < a_strRankSizes.Length; intRankSize++)
            {
                bmp = new Bitmap(
                    "cardimages\\multi\\black"+intRank+"_"+a_strRankSizes[intRankSize]+".png");
                rw.AddResource("bmpBlack"+intRank+"_"+intRankSize, bmp);
                bmp = new Bitmap(
                    "cardimages\\multi\\red"+intRank+"_"+a_strRankSizes[intRankSize]+".png");
                rw.AddResource("bmpRed"+intRank+"_"+intRankSize, bmp);
            }
        }
        // kings, queens and jacks, and large, medium and small suit symbols
        for (int intIndex=0; intIndex<4; intIndex++)
        {
            for (int intSize=0; intSize < a_strRoyalSizes.Length; intSize++)
            {
                bmp = new Bitmap(
                  "cardimages\\multi\\king"+a_strSuits[intIndex]+"_"+
                  a_strRoyalSizes[intSize]+".png");
                rw.AddResource("bmpKing"+a_strSuits[intIndex]+"_"+intSize, bmp);
                bmp = new Bitmap(
                  "cardimages\\multi\\queen"+a_strSuits[intIndex]+"_"+
                  a_strRoyalSizes[intSize]+".png");
                rw.AddResource("bmpQueen"+a_strSuits[intIndex]+"_"+intSize, bmp);
                bmp = new Bitmap(
                  "cardimages\\multi\\jack"+a_strSuits[intIndex]+"_"+
                  a_strRoyalSizes[intSize]+".png");
                rw.AddResource("bmpJack"+a_strSuits[intIndex]+"_"+intSize, bmp);
            }
        }
        
        // 'pips'
        for (int intIndex=0; intIndex<4; intIndex++)
        {
            for (int intSize=0; intSize < a_strSuitSizes.Length; intSize++)
            {
                bmp = new Bitmap(
                  "cardimages\\multi\\"+a_strSuits[intIndex]+"_"+
                  a_strSuitSizes[intSize]+".png");
                rw.AddResource("bmp"+a_strSuits[intIndex]+"_"+intSize, bmp);
            }
        }
       
        rw.Close();
    }
}