using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hakoniwa.Tools.RoadMap
{
    class RoadEntryPositionCalculator
    {
        private static int GetInstanceAngleIndex(RoadEntryInstance e)
        {
            float rotation = 0;
            if (e.cfg_entry.rotation < 0)
            {
                rotation = 360.0f + e.cfg_entry.rotation;
            }
            else
            {
                rotation = e.cfg_entry.rotation;
            }
            //logger.Log(Level.Debug,"rotation=" + rotation + " index=" +(int)rotation/90) ;
            return ((int)rotation / 90);
        }

        private static int GetRelativeAngleIndex(RoadEntryInstance e, int cinx)
        {
            int instance_angle_index = GetInstanceAngleIndex(e);

            int inx = cinx - instance_angle_index;
            if (inx < 0)
            {
                inx += 4;
            }
            //logger.Log(Level.Debug,"index=" + inx);
            return inx;
        }

        private static float GetShiftSizeZ(RoadEntryInstance e, int cinx)
        {
            //logger.Log(Level.Debug,"GetShiftSizeZ: cinx=" + cinx + "GetAngleIndex(e, cinx)=" + GetAngleIndex(e, cinx));
            int instance_angle_index = GetInstanceAngleIndex(e);
            int rinx = GetRelativeAngleIndex(e, cinx);
            //logger.Log(Level.Debug,"GetShiftSizeZ: inx=" + inx);
            switch (instance_angle_index)
            {
                case 0:
                    return +1.0f * e.parts_type.shift_size[rinx].z;
                case 1:
                    return -1.0f * e.parts_type.shift_size[rinx].x;
                case 2:
                    return -1.0f * e.parts_type.shift_size[rinx].z;
                case 3:
                    return +1.0f * e.parts_type.shift_size[rinx].x;
                default:
                    break;
            }
            return 0;
        }
        private static float GetShiftSizeX(RoadEntryInstance e, int cinx)
        {
            int rinx = GetRelativeAngleIndex(e, cinx);
            int instance_angle_index = GetInstanceAngleIndex(e);
            //logger.Log(Level.Debug,"GetShiftSizeZ: inx=" + inx);
            switch (instance_angle_index)
            {
                case 0:
                    return +1.0f * e.parts_type.shift_size[rinx].x;
                case 1:
                    return +1.0f * e.parts_type.shift_size[rinx].z;
                case 2:
                    return -1.0f * e.parts_type.shift_size[rinx].x;
                case 3:
                    return -1.0f * e.parts_type.shift_size[rinx].z;
                default:
                    break;
            }
            return 0;
        }

        private static int GetLocateIndex(RoadEntryInstance e)
        {
            int index = 0;
            if (e.cfg_entry.connect_direction.Contains("z"))
            {
                if (e.cfg_entry.connect_direction.Contains("-"))
                {
                    index = 2;
                }
                else
                {
                    index = 0;
                }
            }
            else
            {
                if (e.cfg_entry.connect_direction.Contains("-"))
                {
                    index = 3;
                }
                else
                {
                    index = 1;
                }
            }
            return index;
        }
        private static int GetReverseLocateIndex(RoadEntryInstance e)
        {
            int index = 0;
            if (e.cfg_entry.connect_direction.Contains("z"))
            {
                if (e.cfg_entry.connect_direction.Contains("-"))
                {
                    index = 0;
                }
                else
                {
                    index = 2;
                }
            }
            else
            {
                if (e.cfg_entry.connect_direction.Contains("-"))
                {
                    index = 1;
                }
                else
                {
                    index = 3;
                }
            }
            return index;
        }
        public static void CalculatePos(RoadEntryInstance prev_e, RoadEntryInstance e)
        {
            //prev prefab name     instance_angle    x, z, can_locate
            //current prefab name  instance_angle    locate_angle
            Debug.Log("CalculatePos: prev_pos=" + prev_e.pos);
            float pos_z = prev_e.pos.z;
            float pos_x = prev_e.pos.x;
            float scale = 1.0f;
            int locate_index = GetLocateIndex(e);
            int r_locate_index = GetReverseLocateIndex(e);
            int rinx = GetRelativeAngleIndex(prev_e, locate_index);


            if (prev_e.cfg_entry.scale > 0.0f)
            {
                scale = prev_e.cfg_entry.scale;
            }
            float prev_size_z = GetShiftSizeZ(prev_e, locate_index) * scale;
            float prev_size_x = GetShiftSizeX(prev_e, locate_index) * scale;

            if (!prev_e.parts_type.shift_size[rinx].can_locate)
            {
                //throw new InvalidDataException("Invalid Location:CURR: " + e.prefab_fname + "angle: " + GetInstanceAngleIndex(e));
            }

            //current entry size
            scale = 1.0f;
            if (e.cfg_entry.scale > 0.0f)
            {
                scale = e.cfg_entry.scale;
            }
            int crinx = GetRelativeAngleIndex(e, r_locate_index);
            float c_size_z = -1.0f * GetShiftSizeZ(e, r_locate_index) * scale;
            float c_size_x = -1.0f * GetShiftSizeX(e, r_locate_index) * scale;
            pos_z += (prev_size_z + c_size_z);
            pos_x += (prev_size_x + c_size_x);

            e.pos.z = pos_z;
            e.pos.x = pos_x;
            return;
        }
    }
}
