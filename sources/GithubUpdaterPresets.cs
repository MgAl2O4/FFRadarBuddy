using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFRadarBuddy
{
    public class GithubUpdaterPresets
    {
        const string repoLink = "https://github.com/MgAl2O4/FFRadarBuddy/";

        public static List<string> FindAndDownloadPresets(out string statusMsg)
        {
            List<string> presetJsons = new List<string>();
            try
            {
                List<string> fileNames = FindPresetFiles();
                foreach (string fileName in fileNames)
                {
                    string jsonContent = DownloadPresetFile(fileName);
                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        presetJsons.Add(jsonContent);
                    }
                }

                statusMsg = "downloaded preset files: " + presetJsons.Count + "/" + fileNames.Count;
            }
            catch (Exception ex)
            {
                statusMsg = "failed! " + ex;
            }

            return presetJsons;
        }

        private static List<string> FindPresetFiles()
        {
            List<string> fileNames = new List<string>();

            WebRequest ReqTree = WebRequest.Create(repoLink + "tree/master/presets");
            ReqTree.Timeout = -1;

            WebResponse RespTree = ReqTree.GetResponse();
            using (Stream dataStream = RespTree.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                string pattern = "href=\\\".+\\/blob\\/master\\/presets\\/(.+\\.json)\\\"";
                Regex regexPattern = new Regex(pattern);

                string prefix = "href=\"";
                int readPos = responseFromServer.IndexOf(prefix);
                while (readPos > 0)
                {
                    int linkEndPos = responseFromServer.IndexOf('\"', readPos + prefix.Length);
                    if (linkEndPos > 0)
                    {
                        string linkDesc = responseFromServer.Substring(readPos, linkEndPos - readPos + 1);
                        Match match = regexPattern.Match(linkDesc);
                        if (match.Success)
                        {
                            fileNames.Add(match.Groups[1].Value);
                        }                        

                        readPos = responseFromServer.IndexOf(prefix, linkEndPos);
                    }
                    else
                    {
                        readPos = -1;
                    }
                }
            }

            return fileNames;
        }

        private static string DownloadPresetFile(string fileName)
        {
            string filePath = repoLink.Replace("github.com", "raw.githubusercontent.com") + "master/presets/" + fileName;
            string fileContent = null;

            WebRequest ReqTree = WebRequest.Create(filePath);
            ReqTree.Timeout = -1;

            WebResponse RespTree = ReqTree.GetResponse();
            using (Stream dataStream = RespTree.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                fileContent = reader.ReadToEnd();
            }

            return fileContent;
        }
    }
}
