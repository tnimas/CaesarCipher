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

using NHunspell;

namespace CaesarCipher
{
    public class HunspellChecker
    {
        private readonly Hunspell _hunspell;


        public HunspellChecker(string affFile,string dictFile)
        {
            _hunspell = new Hunspell(affFile, dictFile);
        }

        public bool WordIsCorrect(string word)
        {
            return _hunspell.Spell(word);
        }
        
    }
}
