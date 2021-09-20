using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Hakoniwa.Tools.RoadMap
{
    class RoadPartsSelector : MonoBehaviour
    {
        private int parts_index = 0;
        public Sprite[] images;
        private string[] prefab_names;
        private Image myImage;
        private Dropdown road_type_obj;

        void Start()
        {
            road_type_obj = GameObject.Find("RoadTypes").GetComponent<Dropdown>();
            //Debug.Log("road_type_obj:" + road_type_obj);

            this.myImage = GameObject.Find("Image").GetComponent<Image>();
            //Debug.Log("myImage:" + myImage);

            this.prefab_names = new string[17];
            this.prefab_names[0] = "road1"; //straight
            this.prefab_names[1] = "road2"; //corner
            this.prefab_names[2] = "road3"; // junction1
            this.prefab_names[3] = "road3bis"; //junction2
            this.prefab_names[4] = "road4"; //junction
            this.prefab_names[5] = "road5"; //cross_road
            this.prefab_names[6] = "road10"; //d_junction
            this.prefab_names[7] = "road14bis"; //d_junction1
            this.prefab_names[8] = "road13"; //d_junction2
            this.prefab_names[9] = "road13bis"; //d_junction3
            this.prefab_names[10] = "road14"; //d_junction4
            this.prefab_names[11] = "road14bisbis"; //d_junction5
            this.prefab_names[12] = "road11"; //d_straight
            this.prefab_names[13] = "road12"; //d_corner
            this.prefab_names[14] = "road12bis"; //d_corner_junction1
            this.prefab_names[15] = "road12bisbis"; //d_corner_junction2
            this.prefab_names[16] = "road15"; //d_cross_road
        }
        void Update()
        {
            this.myImage.sprite = images[road_type_obj.value];
            //Debug.Log("parts_value=" + road_type_obj.value);
        }

        public string GetCurrentSelectedPartsName()
        {
            return this.prefab_names[road_type_obj.value];
        }
        public void DoUpdateParts(bool way)
        {
            if (way)
            {
                parts_index++;
                if (parts_index >= prefab_names.Length)
                {
                    parts_index = 0;
                }

            }
            else
            {
                if (parts_index == 0)
                {
                    parts_index = prefab_names.Length - 1;
                }
                else
                {
                    parts_index--;
                }
            }
            road_type_obj.value = parts_index;
        }
    }
}
