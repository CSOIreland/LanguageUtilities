// This application will extract all nouns, verbs and adjectives from the English language Open Wordnet
// You will need to download the required files from https://github.com/globalwordnet/english-wordnet
// The application will create a json file of synonyms based on the wordnet data

using System.Configuration;
using WordnetSynonymExtractor;
string _outputFileName;

Console.WriteLine("Extracting...");
Extractor ex = new Extractor();
//Get the output filename and initiate it
_outputFileName= ConfigurationManager.AppSettings["outputSynonymJsonFile"] ?? "";
Extractor.InitiateOutputFile(_outputFileName);

//Extract the nouns
ex.ExtractWords(ConfigurationManager.AppSettings["outputSynonymJsonFile"] ?? "", ConfigurationManager.AppSettings["inputNounFile"] ?? "", ConfigurationManager.AppSettings["inputNounIndexFile"] ?? "",'n');

//Extract the verbs
ex.ExtractWords(ConfigurationManager.AppSettings["outputSynonymJsonFile"] ?? "", ConfigurationManager.AppSettings["inputVerbFile"] ?? "", ConfigurationManager.AppSettings["inputVerbIndexFile"] ?? "", 'v');

//Extract the adjectives
ex.ExtractWords(ConfigurationManager.AppSettings["outputSynonymJsonFile"] ?? "", ConfigurationManager.AppSettings["inputAdjectiveFile"] ?? "", ConfigurationManager.AppSettings["inputAdjectiveIndexFile"] ?? "", 'a');

//Summarise and terminate
Console.WriteLine("Words found: " + ex.lemmaCount + " , Matches found: " + ex.matchCount);

Extractor.TerminateOutputFile(_outputFileName);

Console.WriteLine("Finished - press any key to end");
Console.ReadLine();