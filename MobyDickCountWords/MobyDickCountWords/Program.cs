using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
namespace MobyDickCountWords
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            string url = "http://www.gutenberg.org/files/15/15-text.zip";
            string myLocalFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "MobyDickText.zip"));
            string extractedFiles = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "MobyDickText"));
            string xmlWordsFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "MobyDickText.xml"));

            bool resultDownloading = true;

            //Control File Exist or not 
            Console.WriteLine("Controlling MobyDick.zip exist or not.. ");
            bool resultExist = isFileDownloaded(myLocalFilePath);
            Console.WriteLine(resultExist ? "File exists." : "File does not exist.");

            //Downloadinf mobydick text if not exist
            if (!resultExist)
            {
                Console.WriteLine("Downloading MobyDick.zip File..");
                resultDownloading = downloadFile(url, myLocalFilePath);

            }

            //Control file downloded and doesnt Extracted before
            if (resultDownloading & !Directory.Exists(extractedFiles))
            {
                Console.WriteLine("Extracting Zip file..");
                ZipFile.ExtractToDirectory(myLocalFilePath, extractedFiles);
            }

            //Count words in text files..
            Console.WriteLine("Count words in text files..");
            Dictionary<string, int> words = readTextFiles(extractedFiles);


            Console.WriteLine("Writing words to xml file..");
            XMLWrite(words,xmlWordsFile);


            Console.WriteLine("Process FINISHED..");
            Environment.Exit(1);

        }

        private static bool downloadFile(string url,string filePathName)
        {
           

            try
            {
                using (WebClient client = new WebClient())
                {

                    client.DownloadFile(new Uri(url), filePathName);
                    return true;

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0}", ex.Message));
                return false;
            }



        }

        private static bool isFileDownloaded(string filePathName)
        {
            return File.Exists(filePathName);

        }

        private static Dictionary<string, int> readTextFiles(string textFilesPath)
        {
            char[] separators = { ' ' };
            var wordCount = new Dictionary<string, int>();

            var txtFiles = Directory.EnumerateFiles(textFilesPath, "*.txt");
            foreach (string currentFile in txtFiles)
            {

                foreach (var line in File.ReadLines(currentFile, Encoding.UTF8))
                {
                    var words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        string wordX =word.Replace(",","").Replace("(","").Replace(")","").Replace(";","").Replace("!","").Replace(".","").ToLower();
                        int count;
                        wordCount.TryGetValue(wordX ,out count);
                        wordCount[wordX] = count + 1;
                    }
                }
            }

            return wordCount;
        }

        private static void XMLWrite(Dictionary<string, int> dict,string pathName)
        {
            //LINQ to XML
            
            XDocument doc = new XDocument(new XElement("words"));

            foreach (KeyValuePair<string, int> entry in dict)
                doc.Root.Add(new XElement("word", new XAttribute("text", entry.Key.ToString()), new XAttribute("count", entry.Value.ToString())));

            doc.Save(pathName);
        }
    }
}
