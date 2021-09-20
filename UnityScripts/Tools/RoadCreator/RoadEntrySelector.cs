using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hakoniwa.Tools.RoadMap
{
    class RoadEntrySelector
    {
        private static int selected_index = -1;


        public static int GetSelectedIndex()
        {
            return selected_index;
        }
        public static bool is_select_key_down(out KeyCode code)
        {
            bool is_select = true;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                code = KeyCode.LeftArrow;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                code = KeyCode.RightArrow;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                code = KeyCode.DownArrow;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                code = KeyCode.UpArrow;
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                code = KeyCode.H;
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                code = KeyCode.LeftArrow;
            }
            else
            {
                is_select = false;
                code = KeyCode.None;
            }
            return is_select;
        }
        public static void DoSelectLastObj()
        {
            if (RoadEntryInstance.GetLastObj() != null)
            {
                selected_index = RoadEntryInstance.GetLastObjectIndex();
            }
        }
        public static void DoSelect(KeyCode code, RoadEntryInstance obj)
        {
            if (selected_index < 0)
            {
                if (RoadEntryInstance.GetInstanceFromObj(obj, out selected_index) == null)
                {
                    selected_index = RoadEntryInstance.GetLastObjectIndex();
                }
            }
            if ((code == KeyCode.UpArrow) || (code == KeyCode.RightArrow) || (code == KeyCode.H))
            {
                if (selected_index < RoadEntryInstance.GetLastObjectIndex())
                {
                    selected_index = selected_index + 1;
                }
                else
                {
                    selected_index = 0;
                }
            }
            else if ((code == KeyCode.DownArrow) || (code == KeyCode.LeftArrow) || (code == KeyCode.L))
            {
                if (selected_index == 0)
                {
                    selected_index = RoadEntryInstance.GetLastObjectIndex();
                }
                else
                {
                    selected_index = selected_index - 1;
                }
            }
        }
        public static void DoDeselect()
        {
            selected_index = -1;
        }
    }
}
