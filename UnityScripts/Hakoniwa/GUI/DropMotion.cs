using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

public class DropMotion : MonoBehaviour
{
    private bool doStart = false;
    public GameObject [] stopStations;
    private Transform StartMarker;
    private float distance_two;
    private float speed = 0.01f;
    private float pos = 0.0f;
    private GameObject dropTarget;

    // Start is called before the first frame update
    void Start()
    {
        this.stopStations = GameObject.FindGameObjectsWithTag("StopStation");
    }
    public void DoStart()
    {
        this.SelectDropTarget();
        distance_two = Vector3.Distance(StartMarker.position, dropTarget.transform.position);
        this.doStart = true;
    }
    private void SelectDropTarget()
    {
        float diff_min = float.MaxValue;
        this.StartMarker = this.transform;
        this.dropTarget = null;
        for (int i = 0; i < this.stopStations.Length; i++)
        {
            float diff = Vector3.Distance(StartMarker.position, stopStations[i].transform.position);
            if (diff < diff_min)
            {
                diff_min = diff;
                this.dropTarget = this.stopStations[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        if (doStart == false)
        {
            return;
        }
        else
        {
            //float present_Location = (Time.time * speed) / distance_two;
            float present_Location = pos;
            if (pos >= 1.0f)
            {
                this.doStart = false;
                this.pos = 0.0f;
            }
            else
            {
                transform.position = Vector3.Slerp(StartMarker.position, dropTarget.transform.position, present_Location);
            }
            pos += speed;
        }
    }
}
