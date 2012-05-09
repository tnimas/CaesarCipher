//    This program implements encryption and hacking Caesar cipher.
//    Copyright (C) 2012  Maslov Nikolay
//
//    Caesar cipher is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Caesar cipher is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.Text;

namespace CaesarCipher
{
    public class DictionaryService
    {
        public DictionaryService(string alphabet)
        {
            AlphabetList = new List<char>(alphabet.Length);
            AlphabetList.AddRange(alphabet.ToCharArray());
        }

        /// <summary>
        /// Current Alphabet in char list
        /// </summary>
        public List<char> AlphabetList { get; private set; }

        /// <summary>
        /// shift in relative positions of symbols in the alphabet
        /// support negative shiftValue
        /// </summary>
        /// <returns>shifted string in lowercase and only russian symbols except for a space</returns>
        public string Shift(string source, int shiftValue)
        {
            var newStr = new StringBuilder(source.Length);
            foreach (char symbol in source.ToLower())
            {
                if (AlphabetList.Contains(symbol))
                {
                    int newIndex = GetIndexByShift(symbol, shiftValue);
                    newStr.Append(AlphabetList[newIndex]);
                }
                else
                {
                    if (symbol == ' ')
                        newStr.Append(symbol);
                }
            }
            return newStr.ToString();
        }

        private int GetIndexByShift(char symbol, int shiftValue)
        {
            int newIndex = GetCharIndex(symbol) + shiftValue;
            while (newIndex >= AlphabetList.Count)
            {
                newIndex -= AlphabetList.Count;
            }
            while (newIndex < 0)
            {
                newIndex = AlphabetList.Count + newIndex;
            }
            return newIndex;
        }

        private int GetCharIndex(char posedSymbol)
        {
            for (int i = 0; i < AlphabetList.Count; i++)
            {
                if (AlphabetList[i] == posedSymbol)
                    return i;
            }
            throw new ArgumentException();
        }
    }
}