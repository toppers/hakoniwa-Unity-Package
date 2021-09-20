using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hakoniwa.Tools.RoadMap
{
    class RoadSaver
    {
        public static void Save()
        {
            string destPath = "./road_map_saved.json";
            var map = RoadEntryInstance.CreateRoadMap();
            string jsonstring = JsonConvert.SerializeObject(map, Formatting.Indented);
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }
            File.WriteAllText(destPath, jsonstring);
        }
    }
}
