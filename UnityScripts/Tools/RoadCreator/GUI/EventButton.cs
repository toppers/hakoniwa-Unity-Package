using Hakoniwa.Tools.RoadMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventButton : MonoBehaviour
{
    private RoadCreateUserOperation user_operation;
    void Start()
    {
        RoadEntryComposer.Initialize();
        RoadEntryComposer.InitForExternalComponent();

        var root = GameObject.Find("Roads");
        this.user_operation = root.GetComponentInChildren<RoadCreateUserOperation>();

        Debug.Log("m_handler=" + this.user_operation);

    }
    public void OnCreateButtonClick()
    {
        RoadLoader.Load();
    }
    public void OnClearButtonClick()
    {
        user_operation.DestroyAll();
    }
}
