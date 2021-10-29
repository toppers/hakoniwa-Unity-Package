using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hakoniwa.Tools.RoadMap
{
    [System.Serializable]
    public class RoadMapEntry
    {
        public string name;
        public string prefab_name;
        public int repeat_num;
        public float rotation;
        public float scale;
        public string connect_direction;
    }
    [System.Serializable]
    public class RoadMap
    {
        public RoadMapEntry[] entries;
    }

    [System.Serializable]
    public class RoadPartsShiftSize
    {
        public bool can_locate;
        public float x;
        public float z;
    }
    [System.Serializable]
    public class PatchShiftPos
    {
        public float x;
        public float y;
        public float z;
    }
    [System.Serializable]
    public class RoadPartsEntry
    {
        public string prefab_path;
        public string name;
        public float size_x;
        public float size_z;
        public PatchShiftPos patch_pos;
        public RoadPartsShiftSize[] shift_size;
    }
    [System.Serializable]
    public class RoadParts
    {
        public RoadPartsEntry[] entries;
    }


}
