using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using Hakoniwa.Tools.RoadMap;
using Hakoniwa.Core.Utils.Logger;

public class RoadCreator : MonoBehaviour
{
    private static ISimpleLogger logger;
    private static int index = 0;
    private static GameObject parent;
    private static RoadMap map;
    private static Dictionary<string, RoadEntryInstance> hash = new Dictionary<string, RoadEntryInstance>();

    public static void Initialize()
    {
        parent = GameObject.Find("Roads");
        if (logger == null)
        {
            logger = new SimpleLogger("./hakoniwa_road.log", false);
        }
        logger.Log(Level.DEBUG, "parent=" + parent.name);
    }
    private static int GetInstanceAngleIndex(RoadEntryInstance e)
    {
        float rotation = 0;
        if (e.cfg_entry.rotation < 0)
        {
            rotation = 360.0f + e.cfg_entry.rotation;
        }
        else
        {
            rotation = e.cfg_entry.rotation;
        }
        //logger.Log(Level.Debug,"rotation=" + rotation + " index=" +(int)rotation/90) ;
        return ((int)rotation / 90);
    }

    private static int GetRelativeAngleIndex(RoadEntryInstance e, int cinx)
    {
        int instance_angle_index = GetInstanceAngleIndex(e);

        int inx = cinx - instance_angle_index;
        if (inx < 0)
        {
            inx += 4;
        }
        //logger.Log(Level.Debug,"index=" + inx);
        return inx;
    }

    private static float GetShiftSizeZ(RoadEntryInstance e, int cinx)
    {
        //logger.Log(Level.Debug,"GetShiftSizeZ: cinx=" + cinx + "GetAngleIndex(e, cinx)=" + GetAngleIndex(e, cinx));
        int instance_angle_index = GetInstanceAngleIndex(e);
        int rinx = GetRelativeAngleIndex(e, cinx);
        //logger.Log(Level.Debug,"GetShiftSizeZ: inx=" + inx);
        switch (instance_angle_index)
        {
            case 0:
                return +1.0f * e.parts_type.shift_size[rinx].z;
            case 1:
                return -1.0f * e.parts_type.shift_size[rinx].x;
            case 2:
                return -1.0f * e.parts_type.shift_size[rinx].z;
            case 3:
                return +1.0f * e.parts_type.shift_size[rinx].x;
            default:
                break;
        }
        return 0;
    }
    private static float GetShiftSizeX(RoadEntryInstance e, int cinx)
    {
        int rinx = GetRelativeAngleIndex(e, cinx);
        int instance_angle_index = GetInstanceAngleIndex(e);
        //logger.Log(Level.Debug,"GetShiftSizeZ: inx=" + inx);
        switch (instance_angle_index)
        {
            case 0:
                return +1.0f * e.parts_type.shift_size[rinx].x;
            case 1:
                return +1.0f * e.parts_type.shift_size[rinx].z;
            case 2:
                return -1.0f * e.parts_type.shift_size[rinx].x;
            case 3:
                return -1.0f * e.parts_type.shift_size[rinx].z;
            default:
                break;
        }
        return 0;
    }

    private static int GetLocateIndex(RoadEntryInstance e)
    {
        int index = 0;
        if (e.cfg_entry.connect_direction.Contains("z"))
        {
            if (e.cfg_entry.connect_direction.Contains("-"))
            {
                index = 2;
            }
            else
            {
                index = 0;
            }
        }
        else
        {
            if (e.cfg_entry.connect_direction.Contains("-"))
            {
                index = 3;
            }
            else
            {
                index = 1;
            }
        }
        return index;
    }
    private static int GetReverseLocateIndex(RoadEntryInstance e)
    {
        int index = 0;
        if (e.cfg_entry.connect_direction.Contains("z"))
        {
            if (e.cfg_entry.connect_direction.Contains("-"))
            {
                index = 0;
            }
            else
            {
                index = 2;
            }
        }
        else
        {
            if (e.cfg_entry.connect_direction.Contains("-"))
            {
                index = 1;
            }
            else
            {
                index = 3;
            }
        }
        return index;
    }
    private static void CalculatePos(RoadEntryInstance prev_e, RoadEntryInstance e)
    {
        //prev prefab name     instance_angle    x, z, can_locate
        //current prefab name  instance_angle    locate_angle
        float pos_z = prev_e.pos.z;
        float pos_x = prev_e.pos.x;
        float scale = 1.0f;
        int locate_index = GetLocateIndex(e);
        int r_locate_index = GetReverseLocateIndex(e);
        int rinx = GetRelativeAngleIndex(prev_e, locate_index);

        logger.Log(Level.DEBUG, "LOCATION-INDEX: " + locate_index);

        if (prev_e.cfg_entry.scale > 0.0f)
        {
            scale = prev_e.cfg_entry.scale;
        }
        float prev_size_z = GetShiftSizeZ(prev_e, locate_index) * scale;
        float prev_size_x = GetShiftSizeX(prev_e, locate_index) * scale;
        logger.Log(Level.DEBUG, "PREV: " + prev_e.prefab_fname + "angle: " + GetInstanceAngleIndex(prev_e)
            + " Z=" + prev_size_z + " X=" + prev_size_x + " RINX=" + rinx);

        if (!prev_e.parts_type.shift_size[rinx].can_locate)
        {
            throw new InvalidDataException("Invalid Location:CURR: " + e.prefab_fname + "angle: " + GetInstanceAngleIndex(e));
        }

        //current entry size
        scale = 1.0f;
        if (e.cfg_entry.scale > 0.0f)
        {
            scale = e.cfg_entry.scale;
        }
        int crinx = GetRelativeAngleIndex(e, r_locate_index);
        float c_size_z = -1.0f * GetShiftSizeZ(e, r_locate_index) * scale;
        float c_size_x = -1.0f * GetShiftSizeX(e, r_locate_index) * scale;
        logger.Log(Level.DEBUG, "CURR: " + e.prefab_fname + "angle: " + GetInstanceAngleIndex(e)
            + " Z=" + c_size_z + " X=" + c_size_x + " RINX=" + crinx);
        pos_z += (prev_size_z + c_size_z);
        pos_x += (prev_size_x + c_size_x);

        e.pos.z = pos_z;
        e.pos.x = pos_x;
        return;
    }
    private static void CreateEntry(ref RoadEntryInstance e, ref RoadEntryInstance prev_e, RoadMapEntry entry)
    {
        var e_type = RoadEntryInstance.Get(entry.prefab_name);
        e = new RoadEntryInstance(entry);
        e.parts_type = e_type;
        e.cfg_entry = entry;
        if (prev_e != null)
        {
            CalculatePos(prev_e, e);
        }
        LoadPrefab(e);
        prev_e = e;
        hash[entry.prefab_name] = e;
        RoadEntryInstance.AddInstance(e);
    }
    private static int GetRepeatNum(RoadMapEntry entry)
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
    public static void Create()
    {
        if (parent == null)
        {
            return;
        }
        index = 0;
        RoadEntryInstance e = null;
        RoadEntryInstance prev_e = null;

        string jsonString = File.ReadAllText("./road_map.json");
        map = JsonConvert.DeserializeObject<RoadMap>(jsonString);

        //jsonString = File.ReadAllText("./road_parts_type.json");
        jsonString = Resources.Load<TextAsset>("road_parts_type").ToString();
        RoadEntryInstance.parts = JsonConvert.DeserializeObject<RoadParts>(jsonString);

        foreach (var entry in map.entries)
        {
            if (prev_e != null)
            {
                if (entry.connect_direction.Contains("/"))
                {
                    string[] values = entry.connect_direction.Split('/');
                    prev_e = null;
                    if (hash.ContainsKey(values[1]))
                    {
                        prev_e = hash[values[1]];
                    }
                    else
                    {
                        prev_e = RoadEntryInstance.GetInstance(values[1]);
                    }
                    if (prev_e == null)
                    {
                        throw new InvalidDataException("Can not found entry name:" + values[1]);
                    }
                }
            }
            for (int i = 0; i < GetRepeatNum(entry); i++)
            {
                CreateEntry(ref e, ref prev_e, entry);
            }
        }
    }
    private static void LoadPrefab(RoadEntryInstance road_entry)
    {
        string path = road_entry.parts_type.prefab_path + "/" + road_entry.prefab_fname;
        path = path.Split('.')[0];
        //var p = AssetDatabase.LoadAssetAtPath<GameObject>(road_entry.parts_type.prefab_path + "/" + road_entry.prefab_fname);
        var p = Resources.Load< GameObject>(path);
        if (p == null)
        {
            Debug.LogError("path is not found:" + path);
        }
        road_entry.instance = Instantiate(p, road_entry.pos, Quaternion.identity) as GameObject;

        if (road_entry.cfg_entry.name != null)
        {
            road_entry.instance.name = road_entry.cfg_entry.name;
        }
        else
        {
            road_entry.instance.name = p.name + "_" + index;
        }
        if (parent)
        {
            road_entry.instance.transform.parent = parent.transform;
            road_entry.instance.transform.parent = parent.transform;
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
    public static void ClearParts()
    {
        if (parent == null)
        {
            return;
        }
        Transform[] childrens = parent.GetComponentsInChildren<Transform>();
        logger.Log(Level.DEBUG, "ClearParts:" + childrens.Length);
        for (int i = 0; i < childrens.Length; i++)
        {
            if (i > 0 && childrens[i] != null)
            {
                Destroy(childrens[i].gameObject);
            }
        }
    }
}
