using Hakoniwa.Tools.RoadMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventButton : MonoBehaviour
{
    private RoadCreateUserOperation m_handler;

    void Start()
    {
        RoadEntryComposer.Initialize();
        RoadEntryComposer.InitForExternalComponent();

        var root = GameObject.Find("Roads");
        this.m_handler = root.GetComponentInChildren<RoadCreateUserOperation>();

        Debug.Log("m_handler=" + this.m_handler);

    }
    public void OnCreateButtonClick()
    {
        RoadLoader.Load();
    }
    public void OnClearButtonClick()
    {

        RoadEntryComposer.Initialize();
        RoadEntryComposer.ClearParts();
    }
}
