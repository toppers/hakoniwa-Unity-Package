using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMotion : MonoBehaviour
{
    private bool doStart = false;
    public GameObject targetObject;
    private Transform startMarker;
    private Vector3 targetDropPoint;
    private float journeyTime = 2.0f;
    private float startTime;
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        this.targetObject = GameObject.Find("TouchSensor");
    }
    public void DoStart()
    {
        this.startMarker = this.transform;
#if false
        this.targetDropPoint = new Vector3(targetObject.transform.position.x,
                        targetObject.transform.position.y + 3f,
                        targetObject.transform.position.z);
        startTime = Time.time;
        this.doStart = true;
#else
        this.targetDropPoint = new Vector3(targetObject.transform.position.x -0.1f,
                        targetObject.transform.position.y+0.5f,
                        targetObject.transform.position.z);
        transform.position = this.targetDropPoint;
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if false
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        if (doStart == false)
        {
            return;
        }
        else
        {
            float fracComplete = (Time.time - startTime) / journeyTime;
            //float present_Location = (Time.time * speed) / distance_two;
            distance = Vector3.Distance(startMarker.position, this.targetDropPoint);
            Debug.Log("distance=" + distance);
            if (distance <= 0.1f)
            {
                this.doStart = false;
            }
            else
            {
                transform.position = Vector3.Slerp(startMarker.position, this.targetDropPoint, fracComplete);
            }
        }
#endif
    }
}
