using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.GUI
{
    public class ButtonBoard : MonoBehaviour
    {
        public GameObject passenger;
        // Start is called before the first frame update
        void Start()
        {
            this.passenger = GameObject.FindGameObjectWithTag("Passenger");
            if (this.passenger == null)
            {
                Debug.LogError("ERROR: can not found Passenger on env");
            }
        }

        public void OnButtonClick()
        {
            Debug.Log("Board Clicked");
            passenger.GetComponent<BoardMotion>().DoStart();
        }
    }

}
