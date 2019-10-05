using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FFRadarBuddy
{
    public class PlayerSettings
    {
        public float FontSize;
        public float MaxDistanceFromCenter;
        public float MaxDistanceFromCamera;

        public List<ActorFilterPreset> Presets;

        private static PlayerSettings Instance = new PlayerSettings();
        private string DBPath;

        public PlayerSettings()
        {
            FontSize = 8.0f;
            MaxDistanceFromCenter = 0.5f;
            MaxDistanceFromCamera = 100.0f;
            Presets = new List<ActorFilterPreset>();
            DBPath = "FFRadarBuddy-settings.json";
        }

        public static PlayerSettings Get() { return Instance; }

        public void Load()
        {
            string FilePath = CreateFilePath(DBPath);
            if (File.Exists(FilePath))
            {
                try
                {
                    using (StreamReader file = new StreamReader(FilePath))
                    {
                        string fileContent = file.ReadToEnd();
                        file.Close();

                        JsonParser.ObjectValue rootOb = JsonParser.ParseJson(fileContent);
                        LoadFromJson(rootOb);
                    }
                }
                catch (Exception) { }
            }

            if (Presets.Count == 0)
            {
                CreateDefaultPreset();
            }
        }

        public void Save()
        {
            JsonWriter writer = new JsonWriter();
            SaveToJson(writer);

            string FilePath = CreateFilePath(DBPath);
            using (StreamWriter file = new StreamWriter(FilePath))
            {
                string jsonString = writer.ToString();
                file.Write(jsonString);
                file.Close();
            }
        }

        public void CreateDefaultPreset()
        {
            Presets.Add(new ActorFilterPreset() { Name = "Default" });
        }

        private string CreateFilePath(string relativeFilePath)
        {
            string currentDirName = Environment.CurrentDirectory;
            string[] devIgnorePatterns = new string[] { @"sources\bin\Debug", @"sources\bin\Release" };
            foreach (string pattern in devIgnorePatterns)
            {
                if (currentDirName.EndsWith(pattern))
                {
                    currentDirName = currentDirName.Remove(currentDirName.Length - pattern.Length);
                    break;
                }
            }

            return Path.Combine(currentDirName, relativeFilePath);
        }

        private bool LoadFromJson(JsonParser.ObjectValue jsonOb)
        {
            bool hasLoaded = false;
            try
            {
                FontSize = (JsonParser.FloatValue)jsonOb["fontSize"];
                MaxDistanceFromCenter = (JsonParser.FloatValue)jsonOb["maxCenterDist"];
                MaxDistanceFromCamera = (JsonParser.FloatValue)jsonOb["maxCameraDist"];

                JsonParser.ArrayValue arrPresets = (JsonParser.ArrayValue)jsonOb["presets"];
                foreach (JsonParser.Value v in arrPresets.entries)
                {
                    JsonParser.ObjectValue presetJsonOb = (JsonParser.ObjectValue)v;
                    ActorFilterPreset presetOb = new ActorFilterPreset();

                    bool loadedPreset = presetOb.LoadFromJson(presetJsonOb);
                    if (loadedPreset)
                    {
                        Presets.Add(presetOb);
                    }
                }

                hasLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to load settings, exception:" + ex);
            }

            return hasLoaded;
        }

        private void SaveToJson(JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WriteFloat(FontSize, "fontSize");
            writer.WriteFloat(MaxDistanceFromCenter, "maxCenterDist");
            writer.WriteFloat(MaxDistanceFromCamera, "maxCameraDist");

            writer.WriteArrayStart("presets");
            foreach (ActorFilterPreset preset in Presets)
            {
                preset.SaveToJson(writer);
            }
            writer.WriteArrayEnd();

            writer.WriteObjectEnd();
        }

    }
}
