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

    public class RoadEntryInstance
    {
        internal static RoadParts parts;
        internal static List<RoadEntryInstance> instances = new List<RoadEntryInstance>();

        public static RoadPartsEntry Get(string name)
        {
            foreach (var e in RoadEntryInstance.parts.entries)
            {
                if (e.name.Equals(name))
                {
                    return e;
                }
            }
            return null;
        }
        public static void AddInstance(RoadEntryInstance instance)
        {
            instances.Add(instance);
        }
        public static RoadEntryInstance GetInstance(string name)
        {
            foreach(var e in instances)
            {
                if (e.instance != null && e.instance.name != null)
                {
                    if (e.instance.name.Equals(name))
                    {
                        return e;
                    }
                }
            }
            return null;
        }

        public RoadPartsEntry parts_type;
        public string prefab_fname;
        public Vector3 pos;
        public GameObject instance;
        public GameObject patch_instance;
        public RoadPartsEntry type;
        public RoadMapEntry cfg_entry { get; internal set; }

        public RoadEntryInstance(RoadMapEntry entry, RoadPartsEntry e_type)
        {
            this.type = e_type;
            this.prefab_fname = entry.prefab_name + ".prefab";
            this.cfg_entry = entry;
            this.pos = new Vector3(0, 0, 0);
            if (e_type.patch_pos != null)
            {
                this.pos.x = this.pos.x + e_type.patch_pos.x;
                this.pos.y = this.pos.y + e_type.patch_pos.y;
                this.pos.z = this.pos.z + e_type.patch_pos.z;
            }

        }
    }
}
