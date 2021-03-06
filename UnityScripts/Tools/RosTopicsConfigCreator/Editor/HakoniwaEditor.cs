using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using Hakoniwa.PluggableAsset.Assets.Environment;
using Hakoniwa.PluggableAsset;
using System.Runtime.Serialization.Json;
using System.Text;
using Hakoniwa.PluggableAsset.Assets.Robot;

public class HakoniwaEditor : EditorWindow
{
    private static int asset_num;
    private static RosTopicMessageConfigContainer ros_topic_container;
    private static GameObject[] hako_asset_roots;
    private static GameObject[] hako_assets;

    static string ConvertToJson(RosTopicMessageConfigContainer cfg)
    {
        return JsonConvert.SerializeObject(cfg, Formatting.Indented);
    }
    static private int GetHakoAssetRoots()
    {
        hako_asset_roots = GameObject.FindGameObjectsWithTag("HakoAssetRoot");
        int ret = 0;
        foreach (var root in hako_asset_roots)
        {
            var hako_env_root = root.GetComponent<IHakoEnv>();
            if ((hako_env_root != null) && hako_env_root.getRosConfig() != null)
            {
                ret += hako_env_root.getRosConfig().Length;
            }
            var hako_robo_root = root.GetComponent<IRobotComponent>();
            if ((hako_robo_root != null) && hako_robo_root.getRosConfig() != null)
            {
                ret += hako_robo_root.getRosConfig().Length;
            }
        }
        return ret;
    }
    static private void GetHakoAssets(int root_num)
    {
        hako_assets = GameObject.FindGameObjectsWithTag("HakoAsset");
        ros_topic_container = new RosTopicMessageConfigContainer();
        int len = 0;
        foreach(var e in hako_assets)
        {
            if (e.GetComponent<IHakoEnv>() != null)
            {
                len += e.GetComponent<IHakoEnv>().getRosConfig().Length;
            }
            if (e.GetComponent<IRobotComponent>() != null)
            {
                len += e.GetComponent<IRobotComponent>().getRosConfig().Length;
            }
        }
        ros_topic_container.fields = new RosTopicMessageConfig[len + root_num];
    }

    static private void GetHakoAssetConfigs(GameObject root)
    {
        IHakoEnv[] hako_assets = root.GetComponentsInChildren<IHakoEnv>();
        foreach(var asset in hako_assets)
        {
            foreach (var e in asset.getRosConfig())
            {
                e.topic_message_name = root.name + "_" + asset.GetAssetName();
                e.robot_name = root.name;
                ros_topic_container.fields[asset_num] = e;
                asset_num++;
            }
        }
    }
    static private void GetRobotAssetConfig(GameObject root)
    {
        IRobotComponent[] robot_assets = root.GetComponentsInChildren<IRobotComponent>();
        if (robot_assets == null)
        {
            return;
        }
        foreach (var asset in robot_assets)
        {
            var configs = asset.getRosConfig();
            if (configs == null)
            {
                continue;
            }
            foreach ( var e in configs)
            {
                e.topic_message_name = root.name + "_" + e.topic_message_name;
                //e.topic_message_name = e.topic_message_name;
                e.robot_name = root.name;
                ros_topic_container.fields[asset_num] = e;
                asset_num++;
            }
        }
    }


    [MenuItem("Window/Hakoniwa/Assets")]
    static void AssetsUpdate()
    {
        Debug.Log("assets");
        int root_num = GetHakoAssetRoots();
        GetHakoAssets(root_num);
        asset_num = 0;

        foreach(var root in hako_asset_roots)
        {
            GetRobotAssetConfig(root);
            GetHakoAssetConfigs(root);
        }
        Debug.Log("json:" + ConvertToJson(ros_topic_container));
        File.WriteAllText("../../../settings/tb3/RosTopics.json", ConvertToJson(ros_topic_container));
    }


    void OnGUI()
    {
        try
        {

        }
        catch (System.FormatException) { }
    }
}
