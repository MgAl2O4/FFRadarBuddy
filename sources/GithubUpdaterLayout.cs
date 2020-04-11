using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace FFRadarBuddy
{
    public class GithubUpdaterLayout
    {
        const string repoLink = "https://github.com/MgAl2O4/FFRadarBuddy/";

        public static bool DownloadAndUpdateLayout(out string statusMsg)
        {
            bool result = false;
            try
            {
                string jsonContent = DownloadLayoutFile("signatures.json");
                statusMsg = "downloaded memory layout";

                JsonParser.ObjectValue jsonOb = JsonParser.ParseJson(jsonContent);
                JsonParser.ArrayValue entriesArr = (JsonParser.ArrayValue)jsonOb["layout"];
                for (int idx = 0; idx < entriesArr.entries.Count; idx++)
                {
                    JsonParser.ObjectValue entryOb = (JsonParser.ObjectValue)entriesArr.entries[idx];
                    string typeStr = entryOb["id"];
                    if (typeStr == "actors")
                    {
                        MemoryLayout.memPathActors = CreateMemoryPath(entryOb);
                        UpdateMemoryLayout((JsonParser.ArrayValue)entryOb["fields"], typeof(MemoryLayout.ActorConsts));
                    }
                    else if (typeStr == "target")
                    {
                        MemoryLayout.memPathTarget = CreateMemoryPath(entryOb);
                        UpdateMemoryLayout((JsonParser.ArrayValue)entryOb["fields"], typeof(MemoryLayout.TargetConsts));
                    }
                    else if (typeStr == "camera")
                    {
                        MemoryLayout.memPathCamera = CreateMemoryPath(entryOb);
                        UpdateMemoryLayout((JsonParser.ArrayValue)entryOb["fields"], typeof(MemoryLayout.CameraConsts));
                    }
                    else
                    {
                        throw new Exception("Unexpected type: " + typeStr + " in entry " + idx);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                statusMsg = "failed! " + ex;
            }

            return result;
        }

        private static string DownloadLayoutFile(string fileName)
        {
            string filePath = repoLink.Replace("github.com", "raw.githubusercontent.com") + "master/" + fileName;
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

        private static MemoryPath CreateMemoryPath(JsonParser.ObjectValue jsonOb)
        {
            JsonParser.ArrayValue offsetsArr = (JsonParser.ArrayValue)jsonOb["offsets"];
            long[] offsetValues = new long[offsetsArr.entries.Count];
            for (int idx = 0; idx < offsetValues.Length; idx++)
            {
                JsonParser.IntValue intV = (JsonParser.IntValue)offsetsArr.entries[idx];
                offsetValues[idx] = intV;
            }

            string pathType = jsonOb["type"];
            if (pathType == "sig")
            {
                string pattern = jsonOb["sig"];
                return new MemoryPathSignature(pattern, offsetValues);
            }
            else if (pathType == "fixed")
            {
                string pattern = jsonOb["fixed"];
                long[] offsetValuesFixed = new long[offsetValues.Length + 1];
                offsetValuesFixed[0] = Convert.ToInt64(pattern, 16);
                
                for (int idx = 0; idx < offsetValues.Length; idx++)
                {
                    offsetValuesFixed[idx + 1] = offsetValues[idx];
                }

                return new MemoryPath(offsetValuesFixed);
            }

            return null;
        }

        private static void UpdateMemoryLayout(JsonParser.ArrayValue jsonArr, Type structType)
        {
            List<Tuple<string, int>> layoutArr = new List<Tuple<string, int>>();
            for (int idx = 0; idx < jsonArr.entries.Count; idx++)
            {
                JsonParser.ObjectValue fieldOb = (JsonParser.ObjectValue)jsonArr.entries[idx];
                JsonParser.StringValue idV = (JsonParser.StringValue)fieldOb["id"];
                JsonParser.IntValue valueV = (JsonParser.IntValue)fieldOb["v"];

                layoutArr.Add(new Tuple<string, int>(idV, valueV));
            }

            FieldInfo[] fields = structType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo prop in fields)
            {
                for (int idx = 0; idx < layoutArr.Count; idx++)
                {
                    if (prop.Name.Equals(layoutArr[idx].Item1, StringComparison.OrdinalIgnoreCase))
                    {
                        prop.SetValue(null, layoutArr[idx].Item2);
                        break;
                    }
                }
            }
        }
    }
}
