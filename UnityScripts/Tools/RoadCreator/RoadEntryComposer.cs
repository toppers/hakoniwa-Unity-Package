using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using Hakoniwa.Tools.RoadMap;
using Hakoniwa.Core.Utils.Logger;

public class RoadEntryComposer : MonoBehaviour
{
    private static ISimpleLogger logger;
    private static int index = 0;
    private static GameObject parent;
    private static Dictionary<string, RoadEntryInstance> hash = new Dictionary<string, RoadEntryInstance>();
    //public static List<RoadEntryInstance> road_objs = new List<RoadEntryInstance>();

    public static void Initialize()
    {
        parent = GameObject.Find("Roads");
        if (logger == null)
        {
            logger = new SimpleLogger("./hakoniwa_road.log", false);
        }
        logger.Log(Level.DEBUG, "parent=" + parent.name);
    }
 
    public static RoadEntryInstance CreateEntry(ref RoadEntryInstance prev_e, RoadMapEntry entry)
    {
        var e_type = RoadEntryInstance.Get(entry.prefab_name);
        if (e_type == null)
        {
            throw new InvalidDataException("ERROR: Not found prefab=" + entry.prefab_name);
        }
        var e = new RoadEntryInstance(entry, e_type);
        e.parts_type = e_type;
        Debug.Log("parts_type=" + e_type);
        e.cfg_entry = entry;
        if (prev_e != null)
        {
            RoadEntryPositionCalculator.CalculatePos(prev_e, e);
        }
        LoadPrefab(e);
        prev_e = e;
        hash[entry.prefab_name] = e;
        RoadEntryInstance.AddInstance(e);
        return e;
    }

    public static RoadEntryInstance GetBranchEntry(RoadMapEntry entry)
    {
        RoadEntryInstance prev_e = null;
        if (entry.connect_direction.Contains("/"))
        {
            string[] values = entry.connect_direction.Split('/');
            if (RoadEntryComposer.hash.ContainsKey(values[1]))
            {
                prev_e = RoadEntryComposer.hash[values[1]];
            }
            else
            {
                prev_e = RoadEntryInstance.GetInstance(values[1]);
            }
            if (prev_e == null)
            {
                throw new InvalidDataException("Can not found entry name:" + values[1]);
            }
            return prev_e;
        }
        else
        {
            return null;
        }

    }

    public static int GetRepeatNum(RoadMapEntry entry)
    {
        if (entry.repeat_num > 0)
        {
            return entry.repeat_num;
        }
        else
        {
            return 1;
        }
    }

    public static void InitForExternalComponent()
    {
        string jsonString = File.ReadAllText("./road_parts_type.json");
        RoadEntryInstance.parts = JsonConvert.DeserializeObject<RoadParts>(jsonString);
    }

    private static void LoadPrefab(RoadEntryInstance road_entry)
    {
        Debug.Log("road_entry pos=" + road_entry.pos);
        //Debug.Log("prefab_fname=" + road_entry.prefab_fname);
        //Debug.Log("prefab_path=" + road_entry.parts_type.prefab_path);
        string path = road_entry.parts_type.prefab_path + "/" + road_entry.prefab_fname;
        //var p = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var p = Resources.Load<GameObject>(path);
        if (p == null)
        {
            throw new InvalidDataException("ERROR: path is not found:" + path);
        }
        road_entry.instance = Instantiate(p, road_entry.pos, Quaternion.identity) as GameObject;


        //string patch_path = "Assets/HakoniwaPatch/" + road_entry.prefab_fname;
        //var patch = AssetDatabase.LoadAssetAtPath<GameObject>(patch_path);
        string patch_path = "HakoniwaPatch/" + road_entry.prefab_fname;
        var patch = Resources.Load<GameObject>(patch_path);
        if (patch == null)
        {
            throw new InvalidDataException("ERROR: patch path is not found:" + patch_path);
        }
        road_entry.patch_instance = Instantiate(patch, road_entry.pos, Quaternion.identity) as GameObject;

        if (road_entry.cfg_entry.name != null)
        {
            road_entry.instance.name = road_entry.cfg_entry.name;
        }
        else
        {
            road_entry.instance.name = p.name + "_" + index;
            road_entry.patch_instance.name = p.name + "_collider_" + index;
        }
        if (parent)
        {
            road_entry.instance.transform.parent = parent.transform;
            road_entry.patch_instance.transform.parent = road_entry.instance.transform;
        }
        else
        {
            throw new InvalidDataException("ERROR: can not found parent");
        }
        //Undo.RegisterCreatedObjectUndo(road_entry.instance, "Create Roads");

        road_entry.instance.transform.Rotate(0, road_entry.cfg_entry.rotation, 0);
        if (road_entry.cfg_entry.scale > 0.0f)
        {
            road_entry.instance.transform.localScale = new Vector3(road_entry.instance.transform.localScale.x, road_entry.instance.transform.localScale.y, road_entry.cfg_entry.scale);
        }
        var bounds = road_entry.instance.GetComponentInChildren<MeshRenderer>().bounds;
        logger.Log(Level.DEBUG, road_entry.prefab_fname + " : road_pos.instance scale=" + bounds.size);
        //logger.Log(Level.Debug,"road_pos.rotation_angle=" + road_entry.cfg_entry.rotation);

        index++;
    }

    public static void DestroyOne(RoadEntryInstance entry)
    {
        //hash[entry.prefab_fname] = null;
        foreach (var e in hash)
        {
            if (e.Value == entry)
            {
                hash.Remove(e.Key);
                break;
            }
        }
        RoadEntryInstance.RemoveInstance(entry);
        Destroy(entry.instance);
        entry.instance = null;
    }
}
