using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hakoniwa.Tools.RoadMap
{
    public class RoadEntryInstance
    {
        internal static RoadParts parts;
        private static List<RoadEntryInstance> instances = new List<RoadEntryInstance>();

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
            if (!instances.Contains(instance))
            {
                instances.Add(instance);
            }
        }
        public static RoadEntryInstance GetLastObj()
        {
            if (instances.Count > 0)
            {
                return instances[instances.Count - 1];
            }
            else
            {
                return null;
            }
        }
        public static void RemoveInstance(RoadEntryInstance victim)
        {
            if (instances.Contains(victim))
            {
                instances.Remove(victim);
            }
        }
        public static RoadEntryInstance GetInstanceFromIndex(int inx)
        {
            int index = 0;
            foreach (var e in instances)
            {
                if (index == inx)
                {
                    //Debug.Log("name=" + e.instance.name);
                    return e;
                }
                index++;
            }
            return null;
        }
        public static RoadEntryInstance GetInstanceFromObj(RoadEntryInstance obj, out int obj_index)
        {
            int inx = 0;
            obj_index = -1;
            foreach (var e in instances)
            {
                if (e == obj)
                {
                    //Debug.Log("name=" + e.instance.name);
                    obj_index = inx;
                    return e;
                }
                inx++;
            }
            return null;
        }
        public static int GetLastObjectIndex()
        {
            return instances.Count - 1;
        }

        public static RoadEntryInstance GetInstance(string name)
        {
            foreach (var e in instances)
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
            //this.prefab_fname = entry.prefab_name + ".prefab";
            this.prefab_fname = entry.prefab_name;
            this.cfg_entry = entry;
            this.pos = new Vector3(0, 0, 0);
            if (e_type.patch_pos != null)
            {
                this.pos.x = this.pos.x + e_type.patch_pos.x;
                this.pos.y = this.pos.y + e_type.patch_pos.y;
                this.pos.z = this.pos.z + e_type.patch_pos.z;
            }

        }

        public static RoadMap CreateRoadMap()
        {
            var map = new RoadMap();
            map.entries = new RoadMapEntry[instances.Count];
            int i = 0;
            foreach (var e in instances)
            {
                e.cfg_entry.name = e.instance.name;
                map.entries[i] = e.cfg_entry;
                i++;
            }
            return map;
        }
    }
}
