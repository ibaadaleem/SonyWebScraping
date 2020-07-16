using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SonyWebScraper
{
    class Program
    {       
        static async Task Main(string[] args)
        {
            List<List<string>> parameters = readFromFile();
            List<List<string>> outputParameters = new List<List<string>>();

            foreach (List<string> parameter in parameters)
            {
                string languageCode = parameter[0];
                string countryCode = parameter[1];
                string pmtProductId = parameter[2];
                
                string url = CreateUrl(languageCode, countryCode, pmtProductId);
                
                string redirectedUrl = await GetRedirectedUrl(url);

                string newLanguageCountryCode;
                string newPmtProductId;

                (newLanguageCountryCode, newPmtProductId) = BreakUrl(redirectedUrl);

                outputParameters.Add(new List<string>{newLanguageCountryCode, pmtProductId, newPmtProductId});

            }

            writeToFile(outputParameters);

        }

        public static List<List<string>> readFromFile()
        {
            string[] lines = System.IO.File.ReadAllLines(@"files/Input.txt");

            List<List<string>> outputList = new List<List<string>>();

            foreach (string line in lines)
            {
                string[] parameters = line.Split(",");

                List<string> parameterList = new List<string>{parameters[0], parameters[1], parameters[2]};

                outputList.Add(parameterList);
            }

            return outputList;
        }

        public static void writeToFile (List<List<string>> outputParameters)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"files/Output.txt"))
            {
                foreach (List<string> outputParameter in outputParameters)
                {
                    string languageCountryCode = outputParameter[0];
                    string PmtProductId = outputParameter[1];
                    string newPmtProductId = outputParameter[2];

                    string line = languageCountryCode + "," + PmtProductId + "," + newPmtProductId;
                    file.WriteLine(line);      
                }
            }
        }

        public static void writeToFile (string languageCountryCode, string PmtProductId, string newPmtProductId)
        {
            string line = languageCountryCode + "," + PmtProductId + "," + newPmtProductId;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"files/Output.txt"))
            {
                file.WriteLine(line);      
            }
        }


        public static (string,string) BreakUrl(string url)
        {
            string newUrl = url.Replace("https://store.playstation.com/", "");
            newUrl = newUrl.Replace("product/","");

            string[] parameters = newUrl.Split("/");
            string languageCountryCode = parameters[0];
            string pmtProductId = parameters[1];

            return (languageCountryCode, pmtProductId);
        }

        public static string CreateUrl(string languageCode, string countryCode, string pmtProductId)
        {
            string languageCountryCode = languageCode + "-" + countryCode;
            string url = CreateUrl(languageCountryCode, pmtProductId);

            return url;
        }

        public static string CreateUrl(string languageCountryCode, string pmtProductId)
        {
            string prefixUrl = "https://store.playstation.com";
            string product = "product";
            string url = prefixUrl + "/" + languageCountryCode + "/" + product + "/" + pmtProductId;

            return url;
        }


        public static async Task<string> GetRedirectedUrl(string url)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();
            string responseUri = response.RequestMessage.RequestUri.ToString();

            return responseUri;
        }
    }
}
