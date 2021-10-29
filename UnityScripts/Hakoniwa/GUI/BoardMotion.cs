using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMotion : MonoBehaviour
{
    private bool doStart = false;
    public GameObject TargetObject;
    private Transform StartMarker;
    private float distance_two;
    private float speed = 0.01f;
    private float pos = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.TargetObject = GameObject.Find("TouchSensor");
    }
    public void DoStart()
    {
        this.StartMarker = this.transform;
        distance_two = Vector3.Distance(StartMarker.position, TargetObject.transform.position);
        this.doStart = true;
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
                transform.position = Vector3.Slerp(StartMarker.position, TargetObject.transform.position, present_Location);
            }
            pos += speed;
        }
    }
}
