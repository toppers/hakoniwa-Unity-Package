using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventButton : MonoBehaviour
{

    public void OnCreateButtonClick()
    {

        RoadCreator.Initialize();
        RoadCreator.Create();
    }
    public void OnClearButtonClick()
    {

        RoadCreator.Initialize();
        RoadCreator.ClearParts();
    }
}
