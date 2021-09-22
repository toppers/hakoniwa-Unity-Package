using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.GUI
{
    public class ButtonDrop : MonoBehaviour
    {
        private GameObject passenger;

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
            Debug.Log("Drop Clicked");
            passenger.GetComponent<DropMotion>().DoStart();
        }
    }
}
