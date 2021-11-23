using Hakoniwa.PluggableAsset.Assets;
using Hakoniwa.PluggableAsset.Assets.Environment;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Hakoniwa.PluggableAsset.Assets.Environment
{
    class HakoEnv : MonoBehaviour, IInsideAssetController
    {
        private GameObject root;
        private IHakoEnvObstacle[] obstacles;
        private IPduWriter pdu_obstacles;
        private PduIoConnector pdu_io;
        private string my_name = "HakoEnv";
        private bool[] is_touch;

        public void Initialize()
        {
            this.root = GameObject.Find("HakoEnv");
            Debug.Log("HakoEnv Enter");
            var tmp = this.root.GetComponentsInChildren<IHakoEnvObstacle>();
            obstacles = new IHakoEnvObstacle[tmp.Length];
            int i = 0;
            foreach (var e in tmp)
            {
                obstacles[i] = e;
                Debug.Log("Obstacle:" + e);
                i++;
            }
            this.pdu_io = PduIoConnector.Get(my_name);

            this.pdu_obstacles = this.pdu_io.GetWriter(my_name + "_obstaclesPdu");
            if (this.pdu_obstacles == null)
            {
                throw new ArgumentException("can not found HakoEnv pdu:" + my_name + "_obstaclesPdu");
            }
            this.is_touch = new bool[tmp.Length];
            this.CopySensingDataToPdu();
        }
        public void CopySensingDataToPdu()
        {
            int i = 0;
            foreach (var e in obstacles)
            {
                this.is_touch[i] = e.IsTouched();
                i++;
                //Debug.Log("is_touch:" + this.is_touch[i]);
            }
            this.pdu_obstacles.GetWriteOps().SetData("is_touch", is_touch);
        }

        public void DoActuation()
        {
            /* nothing to do */
        }

        public string GetName()
        {
            return "HakoEnv";
        }


    }
}
