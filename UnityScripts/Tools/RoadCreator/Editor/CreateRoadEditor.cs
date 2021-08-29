using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class CreateRoadEditor : EditorWindow
{
    private GameObject parent;

    [MenuItem("Window/HAKONIWA/CreateRoads")]
    static void Init()
    {
        EditorWindow.GetWindow<CreateRoadEditor>(true, "HAKONIWA CreateRoads");
        RoadCreator.Initialize();
    }
    void OnGUI()
    {
        try
        {

            parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true) as GameObject;

            GUILayout.Label("", EditorStyles.boldLabel);
            if (GUILayout.Button("CREATE"))
            {
                RoadCreator.Create();
            }
        }
        catch (System.FormatException) { }
    }
}
