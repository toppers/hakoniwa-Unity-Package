using Hakoniwa.PluggableAsset;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.TB3
{
    public class MotorController
    {
        private Motor[] motors = new Motor[2];      // 0: R, 1: L
        private float[] prev_angle = new float[2];  // 0: R, 1: L
        private float[] delta_angle = new float[2];  // 0: R, 1: L
        private float[] moving_distance = new float[2];  // 0: R, 1: L
        private int motor_power = 500;
        private float motor_interval_distance = 0.160f; // 16cm
        private IPduReader pdu_reader;
        private ParamScale scale;
        private Dictionary<string, UpdateDeviceCycle> device_update_cycle = new Dictionary<string, UpdateDeviceCycle>();

        private float steering_sensitivity = 1.5f;                // 経験値

        internal Motor GetRightMotor()
        {
            return motors[0];
        }
        internal Motor GetLeftMotor()
        {
            return motors[1];
        }

        public void Initialize(GameObject root, Transform transform, ITB3Parts parts, IPduReader pdu_reader)
        {
            GameObject obj;
            this.pdu_reader = pdu_reader;

            this.scale = AssetConfigLoader.GetScale();

            for (int i = 0; i < 2; i++)
            {
                int update_cycle = 1;
                string subParts = parts.GetMotor(i, out update_cycle);
                if (subParts != null)
                {
                    string devname_actuator = "motor_actuator" + i.ToString();
                    string devname_sensor = "motor_sensor" + i.ToString();
                    this.device_update_cycle[devname_actuator] = new UpdateDeviceCycle(update_cycle);
                    this.device_update_cycle[devname_sensor] = new UpdateDeviceCycle(update_cycle);
                    obj = root.transform.Find(transform.name + "/" + subParts).gameObject;
                    Debug.Log("path=" + transform.name + "/" + subParts);
                    motors[i] = obj.GetComponentInChildren<Motor>();
                    motors[i].Initialize(obj);
                    motors[i].SetForce(this.motor_power);
                }
            }
        }

        public void CopySensingDataToPdu()
        {
            for (int i = 0; i < 2; i++)
            {
                string devname_sensor = "motor_sensor" + i.ToString();
                device_update_cycle[devname_sensor].count++;
                if (device_update_cycle[devname_sensor].count >= device_update_cycle[devname_sensor].cycle)
                {
                    this.motors[i].UpdateSensorValues();
                    var angle = motors[i].GetDegree();
                    this.delta_angle[i] = angle - this.prev_angle[i];
                    this.prev_angle[i] = angle;

                    this.moving_distance[i] = ((Mathf.Deg2Rad * this.delta_angle[i]) / Mathf.PI) * motors[i].GetRadius();
                    //Debug.Log("d[" + i + "]=" + this.moving_distance[i]);
                    //Debug.Log("delta_angle[" + i + "]=" + this.delta_angle[i]);
                    device_update_cycle[devname_sensor].count = 0;
                }
            }
        }
        public void DoActuation()
        {
            double target_velocity;
            double target_rotation_angle_rate;

            target_velocity = this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("x") * this.scale.cmdvel;
            target_rotation_angle_rate = this.pdu_reader.GetReadOps().Ref("angular").GetDataFloat64("z");

            //Debug.Log("scale.cmdvel=" + this.scale.cmdvel);
            //Debug.Log("target_velocity=" + target_velocity);
            //Debug.Log("target_rotation_angle_rate=" + target_rotation_angle_rate);
            // V_R(右車輪の目標速度) = V(目標速度) + d × ω(目標角速度)
            // V_L(左車輪の目標速度) = V(目標速度) - d × ω(目標角速度)

            for (int i = 0; i < 2; i++)
            {
                string devname_actuator = "motor_actuator" + i.ToString();
                device_update_cycle[devname_actuator].count++;
                if (device_update_cycle[devname_actuator].count >= device_update_cycle[devname_actuator].cycle)
                {
                    if (i == 0)
                    {
                        motors[i].SetTargetVelicty((float)(target_velocity + (steering_sensitivity * target_rotation_angle_rate * motor_interval_distance / 2)));
                    }
                    else
                    {
                        motors[i].SetTargetVelicty((float)(target_velocity - (steering_sensitivity * target_rotation_angle_rate * motor_interval_distance / 2)));
                    }
                    device_update_cycle[devname_actuator].count = 0;
                }
            }
        }

        internal float GetDeltaMovingDistance()
        {
            return ((this.moving_distance[0] + this.moving_distance[1]) / 2.0f);
        }
    }
}
