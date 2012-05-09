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
using System.IO;
using System.Text;
using CaesarCipher.Properties;

namespace CaesarCipher
{
    
    public class Core
    {
        private HunspellChecker _checker;
        private DictionaryService _criptService;
        private readonly LanguageService _configuration = new LanguageService();
        private readonly Settings _settings = new Settings();

        /// <summary>
        /// Entry point
        /// </summary>
        public void Run()
        {
            try
            {
                string inputData;
                bool startForEncrypt;
                int? shift;

                Input(out startForEncrypt, out inputData, out shift);

                string resultStr;
                if (startForEncrypt)
                {
                    if (shift != null)
                        resultStr = Encrypt(inputData, (int) shift);
                    else
                        throw new NullReferenceException();
                }
                else
                    resultStr = Decrypt(inputData);
                

                WriteToOutput(resultStr);
            }
            catch (Exception e)
            {
                ShowToUser(e.Message);
                BlockUserActions();
            }
        }

        private void Input(out bool forEncrypt, out string data, out int? shift)
        {

            using (var reader = new StreamReader("input.txt", Encoding.UTF8))
            {
                string firstLine = reader.ReadLine();
                bool isEncrypt;
                CheckFirstLine(firstLine, out isEncrypt);

                int? currentShift = (isEncrypt) ? GetShift(firstLine) : null;

                var firstWords = firstLine.Split();
                if (firstWords.Length >1 && firstWords[1].Length == 2)
                {
                    InstallLanguage(firstWords[1]);
                }
                else
                {
                    InstallLanguage(_settings.DefaultLanguageShortname);
                }

                string content = reader.ReadLine();
                CheckSecondLine(content);

                forEncrypt = isEncrypt;
                data = content;
                shift = currentShift;

            }
        }

        private void InstallLanguage(string language)
        {
            LanguageModel model = _configuration.GetLanguageByName(language);
            _checker = new HunspellChecker(model.AffFilename, model.DicFilename);
            _criptService = new DictionaryService(model.Alphabet);
        }

        private void CheckFirstLine(string line, out bool isEncrypt)
        {
            bool encrypt = false;
            if (!string.IsNullOrEmpty(line))
            {
                int? currentShift = GetShift(line);
                encrypt = line.IndexOf("encrypt_", StringComparison.Ordinal) != -1
                         && currentShift != null;
            }

            if (!encrypt && (string.IsNullOrEmpty(line) ||
                line.IndexOf("decrypt", StringComparison.Ordinal) == -1))
            {
                ShowToUser("write \"encrypt_<SHIFTCOUNT>\" or \"decrypt\" in first line input.txt");
                BlockUserActions();
                throw new MissingFieldException();
            }
            isEncrypt = encrypt;
        }
		
        private void CheckSecondLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                throw new MissingFieldException("write content for encrypt or decrypt in second line input.txt");
        }

        private int? GetShift(string inputLine)
        {//get input value of shift when start for encoding
            string firstWord = inputLine.Split()[0];
            int prefixLength = "encrypt_".Length;
            if (prefixLength >= firstWord.Length)
            {
                return null;
            }
            string value = firstWord.Substring(prefixLength);
            int? result;
            int tryResult;

            if (int.TryParse(value, out tryResult))
                result = tryResult;
            else result = null;

            return result;
        }

        private string Encrypt(string encodingData, int shift)
        {
            return _criptService.Shift(encodingData, shift);
        }

        private string Decrypt(string decodingStr)
        {
            int abcLength = _criptService.AlphabetList.Count;
            //dictionary for defined right shift. 
            //Keys - value of shift, Values - identified the number of words for current shift
            var shiftScoresDict = new Dictionary<int, int>(abcLength);

            for (int i = 0; i < abcLength; i++)
            {
                string shiftedStr = _criptService.Shift(decodingStr, i);
                string[] words = shiftedStr.Split();

                //first N words, for fast work
                int length = words.Length > _settings.WordsProcessing ? _settings.WordsProcessing : words.Length;
                int scores = ScoresForCombination(words, length);
                var percentReady = (int) ((double) i/abcLength*100);
                ShowToUser(String.Format("{0}%", percentReady));
                shiftScoresDict.Add(i, scores);
            }
            int trueShift = KeyForMax(shiftScoresDict);

            return _criptService.Shift(decodingStr, trueShift);
        }

        private void ShowToUser(string message)
        {
            Console.WriteLine(message);
        }

        private void BlockUserActions()
        {
            Console.ReadKey();
        }

        private int KeyForMax(Dictionary<int, int> dict)
        {
            int max = 0;
            int maxKey = 0;
            foreach (var pair in dict)
            {
                if (pair.Value > max)
                {
                    max = pair.Value;
                    maxKey = pair.Key;
                }
            }
            return maxKey;
        }

        private int ScoresForCombination(string[] words, int length)
        {
            if (words.Length > length)
                throw new ArgumentException();

            var pool = new List<Func<string, bool>>(length);
            var results = new List<IAsyncResult>(length);
            for (int i = 0; i < length; i++)
            {
                Func<string, bool> caller = WordIsCorrect;
                //the method WordIsCorrect will be run for each word in separate thread for fast work
                IAsyncResult result = caller.BeginInvoke(words[i], null, null);

                pool.Add(caller);
                results.Add(result);
            }

            int trueCharCount = 0;
            for (int i = 0; i < length; i++)
            {//synchronization with general thread
                bool endInvoke = pool[i].EndInvoke(results[i]);
                if (endInvoke)
                {
                    if (words[i].Length > 1)
                        trueCharCount += words[i].Length;
                    //if word is correct add scores (equivalent word length) for this shift position
                }
            }
            return trueCharCount;
        }

        private bool WordIsCorrect(string word)
        {
            return _checker.WordIsCorrect(word);
        }


        private void WriteToOutput(string finalData)
        {
            using (var writer = new StreamWriter("output.txt", false, Encoding.UTF8))
            {
                writer.WriteLine(finalData);
            }
        }


    }
}