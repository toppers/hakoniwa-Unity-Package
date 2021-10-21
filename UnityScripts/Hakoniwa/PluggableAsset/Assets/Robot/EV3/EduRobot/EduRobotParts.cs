using UnityEngine;
using Hakoniwa.PluggableAsset.Assets.Robot.EV3;

namespace Hakoniwa.Assets.EduRobot
{
    public class EduRobotParts : MonoBehaviour, IEV3Parts
    {
        private EV3PartsLoader loader = new EV3PartsLoader();
        private string motor_a = "RoboModel_Axis/EV3_LeftMotor/HackEV_L8_Wheel2/L8_Tire_Bk";
        private string motor_c = "RoboModel_Axis/EV3_RightMotor/HackEV_L8_Wheel/L8_Tire_Bk 1";
        private string color_sensor0 = "RoboModel_Axis/EV3_ColorSensor0/EV3_ColorSensor_Wh06/ColorSensor";
        private string color_sensor1 = "RoboModel_Axis/EV3_ColorSensor1/EV3_ColorSensor_Wh06/ColorSensor";
        private string ultra_sonic_sensor = "RoboModel_Axis/EV3_Sidewinder_Head/EV3_UltrasonicSensor/EV3_UltrasonicSensor_Bk10";
        private string touch_sensor1 = "RoboModel_Axis/EV3_TouchSensor/BoxHolder";
        private string touch_sensor0 = "RoboModel_Axis/EV3_Bumper/Bumper_Front";

        public void Load()
        {
            this.loader.Load();
            return;
        }


        public string GetColorSensor0()
        {
            if (this.loader.GetColorSensor0() != null)
            {
                return this.loader.GetColorSensor0();
            }
            return color_sensor0;
        }

        public string GetMotorA()
        {
            if (this.loader.GetMotorA() != null)
            {
                return this.loader.GetMotorA();
            }
            return motor_a;
        }

        public string GetMotorB()
        {
            if (this.loader.GetMotorB() != null)
            {
                return this.loader.GetMotorB();
            }
            return null;
        }

        public string getUltraSonicSensor()
        {
            if (this.loader.getUltraSonicSensor() != null)
            {
                return this.loader.getUltraSonicSensor();
            }
            return ultra_sonic_sensor;
        }
        public string getTouchSensor0()
        {
            if (this.loader.getTouchSensor0() != null)
            {
                return this.loader.getTouchSensor0();
            }
            return touch_sensor0;
        }

        public string getGyroSensor()
        {
            if (this.loader.getGyroSensor() != null)
            {
                return this.loader.getGyroSensor();
            }
            return null;
        }

        public string GetMotorC()
        {
            if (this.loader.GetMotorC() != null)
            {
                return this.loader.GetMotorC();
            }
            return motor_c;
        }

        string IEV3Parts.getTouchSensor1()
        {
            if (this.loader.getTouchSensor1() != null)
            {
                return this.loader.getTouchSensor1();
            }
            return touch_sensor1;
        }

        public string GetLed()
        {
            if (this.loader.GetLed() != null)
            {
                return this.loader.GetLed();
            }
            return null;
        }


        public string GetColorSensor1()
        {
            if (this.loader.GetColorSensor1() != null)
            {
                return this.loader.GetColorSensor1();
            }
            return color_sensor1;
        }

        public string getGpsSensor()
        {
            if (this.loader.getGpsSensor() != null)
            {
                return this.loader.getGpsSensor();
            }
            return null;
        }

        public string getButtonSensor(ButtonSensorType type)
        {
            if (this.loader.getButtonSensor(type) != null)
            {
                return this.loader.getButtonSensor(type);
            }
            switch (type)
            {
                case ButtonSensorType.BUTTON_SENSOR_LEFT:
                    return "RoboModel_Axis/EV3_Body/EV3_IntelligentBlock_Root/EV3_IntelligentBlock_Ash05/TouchSensor";
                case ButtonSensorType.BUTTON_SENSOR_RIGHT:
                    return "RoboModel_Axis/EV3_Body/EV3_IntelligentBlock_Root/EV3_IntelligentBlock_Ash06/TouchSensor";
                case ButtonSensorType.BUTTON_SENSOR_ENTER:
                    return "RoboModel_Axis/EV3_Body/EV3_IntelligentBlock_Root/EV3_IntelligentBlock_DAsh07/TouchSensor";
                case ButtonSensorType.BUTTON_SENSOR_DOWN:
                    return "RoboModel_Axis/EV3_Body/EV3_IntelligentBlock_Root/EV3_IntelligentBlock_Ash08/TouchSensor";
                case ButtonSensorType.BUTTON_SENSOR_UP:
                    return "RoboModel_Axis/EV3_Body/EV3_IntelligentBlock_Root/EV3_IntelligentBlock_Ash09/TouchSensor";
                case ButtonSensorType.BUTTON_SENSOR_BACK:
                    return "RoboModel_Axis/EV3_Body/EV3_IntelligentBlock_Root/EV3_IntelligentBlock_Ash04/TouchSensor";
            }
            throw new System.NotImplementedException();
        }
    }
}
