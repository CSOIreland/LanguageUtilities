using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordnetSynonymExtractor
{
    internal class Extractor
    {
        string? _outputFileName;
        bool isFirst;
        List<string> matchWords;
        internal int lemmaCount { get; set; }
        internal int matchCount { get; set; }
        internal Extractor()
        {
            isFirst = true;
            matchWords = new();
        }

        /// <summary>
        /// Extracts the synonyms from the index and data files and writes them to a json file
        /// 
        /// </summary>
        /// <param name="outputFileName"></param>
        /// <param name="dataFileName"></param>
        /// <param name="indexFileName"></param>
        /// <param name="wordType"></param>
        /// <returns></returns>
        internal bool ExtractWords(string outputFileName,string dataFileName, string indexFileName,char wordType)
        {
            _outputFileName = outputFileName;   
            //Get the index file as a string
            string wordIndex = GetWordIndex(indexFileName);
            //Get the data file as a string
            string wordData = GetWordData(dataFileName);
            //Get the valid entries in the the index file as a list of strings
            List<string> candidateWords=GetWordCandidateStrings(wordIndex);

            //Prepare the stream writer
            StreamWriter sw = new StreamWriter(_outputFileName,true);

            //For each valid entry in the index file list of strings
            foreach (string item in candidateWords)
            {
                //Get the word..start of line then a complete word then a space break
                string lemma = Regex.Match(item, "^[a-z]+\\b").Value;
                List<string> matches = new();
                //8 digit codes from the extract
                matches = Regex.Matches(item, "\\d{8}", RegexOptions.Multiline).Select(x => x.Value).ToList();
                //We now have a list of 8 digit codes that we will use to lookup the data file
                foreach (var match in matches)
                {
                    this.lemmaCount++;
                    //the matched code then all characters up to the pipe character
                    string regexSearch = "^" + match + "[^|]+";
                    string dataItem = Regex.Match(wordData, regexSearch, RegexOptions.Multiline).Value;

                    //dataItem is a line in the data file
                    //2 or more characters with a following space plus one or two digits plus a space
                    List<string> dataItemMatches = Regex.Matches(dataItem, "[A-Za-z_]{2,}(?=\\s\\d{1,2}\\s)", RegexOptions.Multiline).Select(x => x.Value).ToList();
                    //We now have a list of matches for the original word - write them to the file if suitable
                    foreach(string m in dataItemMatches)
                    {
                        //single words
                        if (m.Length > 0 && lemma.Length > 0 && Regex.Match(m, "^[a-zA-Z]+$").Success && !matchWords.Contains(lemma + m))
                        {
                            if (!isFirst) sw.Write(',');
                            sw.WriteLine("{");
                            sw.WriteLine("\"match\": " + "\"" + m + "\",");
                            sw.WriteLine("\"lemma\": " + "\"" + lemma + "\"");
                            sw.WriteLine("}");
                            isFirst = false;
                            matchWords.Add(lemma + m);
                            this.matchCount++;
                        }
                    }

                }
                Console.WriteLine(lemma);
            }
            sw.Close();
            sw.Dispose();
            return true;
        }


        /// <summary>
        /// Close down the output
        /// </summary>
        /// <param name="outputFileName"></param>
        internal static void TerminateOutputFile(string outputFileName)
        {
            StreamWriter sw = new StreamWriter(outputFileName, true);
            sw.WriteLine("]");
            sw.Close();
            sw.Dispose();
        }

        /// <summary>
        /// Initiate the output
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string InitiateOutputFile(string fileName )
        {
            File.WriteAllText(fileName, "[");
            return fileName;
        }

        /// <summary>
        /// Get the data file as a string
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetWordData(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        /// <summary>
        /// Get the index file as a string
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetWordIndex(string fileName)
        {
            return File.ReadAllText(fileName);
        }
        /// <summary>
        /// Get the valid entries from the index file (ignores the preamble)
        /// </summary>
        /// <param name="wordIndex"></param>
        /// <returns></returns>
        private static List<string> GetWordCandidateStrings(string wordIndex)
        {
            List<string> candidateStrings = new List<string>();
            //lines starting with lowercase regular letters followed by a space
            candidateStrings = Regex.Matches(wordIndex, "^[a-z]+\\s.*",RegexOptions.Multiline ).Select(x => x.Value).ToList();
            return candidateStrings;
        }
    }
}
