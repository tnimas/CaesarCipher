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

using System.Collections.Generic;
using System.Linq;


namespace CaesarCipher
{
	///<summary>
	///The model with all the necessary data about a language
	///</summary>
    public class LanguageModel
    {
		///<summary>
        ///two-letter abbreviation of language
        ///</summary>
        public string Shortname { get; set; }

		///<summary>
        ///language alphabet
        ///</summary>
        public string Alphabet { get; set; }

		///<summary>
        ///.aff file name with the data dictionary for the language
        ///</summary>
        public string AffFilename { get; set; }

		///<summary>
        ///.dic file name with the data dictionary of the language
        ///</summary>
        public string DicFilename { get; set; } 
    }
	
	///<summary>
	///Provides access to data on languages
	///</summary>
    public class LanguageService
    {
        private readonly List<LanguageModel> _data; 
        public LanguageService()
        {
            var reader = new System.Xml.Serialization.XmlSerializer(typeof(List<LanguageModel>));
            var file = new System.IO.StreamReader("langconfig.xml");
            _data = (List<LanguageModel>)reader.Deserialize(file);
        }

        public LanguageModel GetLanguageByName(string shortname)
        {
            string upperCaseName = shortname.ToUpper();
            return _data.Single(t => t.Shortname.ToUpper() == upperCaseName);
        }
    }
}
