using Hakoniwa.PluggableAsset.Assets.Environment;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public class HakoEnvObstacle : MonoBehaviour, IHakoEnvObstacle
    {
        private GameObject obstacle;
        public bool isTouched = false;

        public void Initialize(object root)
        {
            obstacle = (GameObject)root;
            this.isTouched = false;
        }

        public bool IsTouched()
        {
            return isTouched;
        }

        void OnTriggerEnter(Collider t)
        {
            this.isTouched = true;
            //Debug.Log("ENTER:" + t.gameObject.name);
        }
        void OnTriggerStay(Collider t)
        {
            this.isTouched = true;
            //Debug.Log("STAY:" + t.gameObject.name);
        }

        private void OnTriggerExit(Collider t)
        {
            this.isTouched = false;
            //Debug.Log("EXIT:" + t.gameObject.name);
        }
    }

}
