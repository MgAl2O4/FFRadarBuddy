using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRadarBuddy
{
    public class ActorFilter
    {
        public string Description;
        public bool HasDescriptionOverride;
        public Pen Pen;
        public GameData.OverlaySettings.DisplayMode Mode;

        public bool UseMatchType;
        public MemoryLayout.ActorType MatchType;

        public bool UseMatchNpcId;
        public uint MatchNpcId;

        public ActorFilter()
        {
            HasDescriptionOverride = false;
            Pen = Pens.Red;
            Mode = GameData.OverlaySettings.DisplayMode.WhenClose;
        }

        public bool Apply(GameData.ActorItem actor)
        {
            bool hasMatch = true;
            if (UseMatchType && actor.Type != MatchType)
            {
                hasMatch = false;
            }

            if (UseMatchNpcId && actor.NpcId != MatchNpcId)
            {
                hasMatch = false;
            }

            if (hasMatch)
            {
                actor.OverlaySettings.Mode = Mode;
                actor.OverlaySettings.DrawPen = Pen;
                actor.OverlaySettings.Description = HasDescriptionOverride ? Description : actor.ShowName;
                actor.OverlaySettings.IsMatchingFilters = true;
            }

            return hasMatch;
        }

        public bool LoadFromJson(JsonParser.ObjectValue jsonOb)
        {
            bool hasLoaded = false;
            try
            {
                HasDescriptionOverride = (JsonParser.BoolValue)jsonOb["hasName"];
                Description = jsonOb["name"];

                string colorHex = "0x" + jsonOb["color"];
                Color color = Color.FromArgb(Convert.ToInt32(colorHex, 16));
                Pen = new Pen(color);

                Mode = (GameData.OverlaySettings.DisplayMode)((JsonParser.IntValue)jsonOb["mode"]).IntNumber;

                UseMatchType = (JsonParser.BoolValue)jsonOb["hasType"];
                MatchType = UseMatchType ? (MemoryLayout.ActorType)((JsonParser.IntValue)jsonOb["matchType"]).IntNumber : 0;

                UseMatchNpcId = (JsonParser.BoolValue)jsonOb["hasNpcId"];
                MatchNpcId = UseMatchNpcId ? (uint)((JsonParser.IntValue)jsonOb["matchNpcId"]).IntNumber : 0;

                hasLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to load filter '" + jsonOb + "', exception:" + ex);
            }

            return hasLoaded;
        }

        public void SaveToJson(JsonWriter writer)
        {
            writer.WriteObjectStart();

            writer.WriteBool(HasDescriptionOverride, "hasName");
            writer.WriteString(Description, "name");

            string colorHex = Pen.Color.ToArgb().ToString("x8");
            writer.WriteString(colorHex, "color");
            writer.WriteInt((int)Mode, "mode");

            writer.WriteBool(UseMatchType, "hasType");
            if (UseMatchType) { writer.WriteInt((int)MatchType, "matchType"); }

            writer.WriteBool(UseMatchNpcId, "hasNpcId");
            if (UseMatchNpcId) { writer.WriteInt((int)MatchNpcId, "matchNpcId"); }

            writer.WriteObjectEnd();
        }
    }

    public class ActorFilterPreset
    {
        public string Name;
        public bool ShowOnlyMatching;
        public List<ActorFilter> Filters;
        public int version;

        public ActorFilterPreset()
        {
            Name = "Preset";
            ShowOnlyMatching = false;
            Filters = new List<ActorFilter>();
            version = 1;
        }

        public void Apply(GameData.ActorItem actor)
        {
            bool hasMatch = false;
            foreach (ActorFilter filter in Filters)
            {
                if (filter.Apply(actor))
                {
                    hasMatch = true;
                    break;
                }
            }

            if (!hasMatch)
            {
                actor.OverlaySettings.Description = actor.ShowName;
                actor.OverlaySettings.Mode = ShowOnlyMatching ? GameData.OverlaySettings.DisplayMode.Never : GameData.OverlaySettings.DisplayMode.WhenClose;
                actor.OverlaySettings.DrawPen = Pens.Gray;
                actor.OverlaySettings.IsMatchingFilters = false;
            }
        }

        public bool LoadFromJson(JsonParser.ObjectValue jsonOb)
        {
            bool hasLoaded = false;
            try
            {
                Name = jsonOb["name"];
                ShowOnlyMatching = (JsonParser.BoolValue)jsonOb["onlyMatching"];

                version = jsonOb.entries.ContainsKey("ver") ? (JsonParser.IntValue)jsonOb["ver"] : 1;

                JsonParser.ArrayValue arrFilters = (JsonParser.ArrayValue)jsonOb["filters"];
                foreach (JsonParser.Value v in arrFilters.entries)
                {
                    JsonParser.ObjectValue filterJsonOb = (JsonParser.ObjectValue)v;
                    ActorFilter filterOb = new ActorFilter();

                    bool loadedFilter = filterOb.LoadFromJson(filterJsonOb);
                    if (loadedFilter)
                    {
                        Filters.Add(filterOb);
                    }
                }

                hasLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to load preset '" + jsonOb + "', exception:" + ex);
            }

            return hasLoaded;
        }

        public void SaveToJson(JsonWriter writer)
        {
            writer.WriteObjectStart();

            writer.WriteString(Name, "name");
            writer.WriteBool(ShowOnlyMatching, "onlyMatching");

            if (version > 1)
            {
                writer.WriteInt(version, "ver");
            }

            writer.WriteArrayStart("filters");
            foreach (ActorFilter filter in Filters)
            {
                filter.SaveToJson(writer);
            }
            writer.WriteArrayEnd();

            writer.WriteObjectEnd();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
