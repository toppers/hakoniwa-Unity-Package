using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hakoniwa.Tools.RoadMap
{
    public class RoadLoader
    {
        private static RoadMap map;

        public static void Load()
        {
            RoadEntryInstance prev_e = null;

            RoadEntryComposer.Initialize();

            string jsonString = File.ReadAllText("./road_map.json");
            map = JsonConvert.DeserializeObject<RoadMap>(jsonString);

            jsonString = File.ReadAllText("./road_parts_type.json");
            RoadEntryInstance.parts = JsonConvert.DeserializeObject<RoadParts>(jsonString);

            foreach (var entry in map.entries)
            {
                if (prev_e != null)
                {
                    var tmp = RoadEntryComposer.GetBranchEntry(entry);
                    if (tmp != null)
                    {
                        prev_e = tmp;
                    }
                }
                for (int i = 0; i < RoadEntryComposer.GetRepeatNum(entry); i++)
                {
                    var e = RoadEntryComposer.CreateEntry(ref prev_e, entry);
                }
            }
        }

    }
}
