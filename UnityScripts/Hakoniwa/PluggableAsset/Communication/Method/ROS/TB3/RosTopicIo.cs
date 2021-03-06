using Hakoniwa.PluggableAsset.Communication.Pdu.ROS;
using Hakoniwa.PluggableAsset.Communication.Method.ROS;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using System;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using Hakoniwa.PluggableAsset.Communication.Pdu.ROS.TB3;
using System.IO;
using Newtonsoft.Json;
using Hakoniwa.Core.Utils.Logger;

using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using RosMessageTypes.Nav;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.Tf2;

namespace Hakoniwa.PluggableAsset.Communication.Method.ROS.TB3
{
    [System.Serializable]
    public class UnityRosParameter
    {
        public bool connection_startup;
        public string ip_address;
        public int portno;
        public bool show_hud;
        public int keep_alive_time;
        public int network_timeout;
        public float sleep_time;
    }
    public class TopicCycle
    {
        public int count;
        public int cycle;
        public TopicCycle(int c)
        {
            this.count = 0;
            this.cycle = c;
        }
    }
    public class RosTopicIo : IRosTopicIo
    {
        private ROSConnection ros;
        private Dictionary<string, Message> topic_data_table = new Dictionary<string, Message>();
        private Dictionary<string, TopicCycle> topic_send_timing = new Dictionary<string, TopicCycle>();
        private UnityRosParameter parameters;
        private void LoadParameters(string filepath)
        {
            ros = ROSConnection.GetOrCreateInstance();
            if (filepath == null)
            {
                return;
            }
            string jsonString = File.ReadAllText(filepath);
            parameters = JsonConvert.DeserializeObject<UnityRosParameter>(jsonString);
            ros.ShowHud = parameters.show_hud;
            ros.RosIPAddress = parameters.ip_address;
            ros.RosPort = parameters.portno;
            ros.ConnectOnStart = parameters.connection_startup;
            ros.KeepaliveTime = parameters.keep_alive_time;
            ros.NetworkTimeoutSeconds = parameters.network_timeout;
            ros.SleepTimeSeconds = parameters.sleep_time;
            SimpleLogger.Get().Log(Level.INFO, "ShowHud=" + ros.ShowHud);
            SimpleLogger.Get().Log(Level.INFO, "RosIPAddress=" + ros.RosIPAddress);
            SimpleLogger.Get().Log(Level.INFO, "RosPort=" + ros.RosPort);
            SimpleLogger.Get().Log(Level.INFO, "ConnectOnStart=" + ros.ConnectOnStart);
            SimpleLogger.Get().Log(Level.INFO, "KeepaliveTime=" + ros.KeepaliveTime);
            SimpleLogger.Get().Log(Level.INFO, "NetworkTimeoutSeconds=" + ros.NetworkTimeoutSeconds);
            SimpleLogger.Get().Log(Level.INFO, "SleepTimeSeconds=" + ros.SleepTimeSeconds);
        }
        private RostopicPublisherOption GetPubOption(string topic_name)
        {
            foreach (var e in AssetConfigLoader.core_config.ros_topics)
            {
                if (e.topic_message_name == topic_name)
                {
                    if (e.sub == false)
                    {
                        if (e.pub_option != null)
                        {
                            return e.pub_option;
                        }
                    }
                }
            }
            return null;
        }
        public RosTopicIo()
        {
            LoadParameters(AssetConfigLoader.core_config.ros_topic_method.parameters);

            foreach (var e in AssetConfigLoader.core_config.ros_topics)
            {
                topic_data_table[e.topic_message_name] = null;
                if (e.sub == false)
                {
                    if (e.pub_option != null)
                    {
                        topic_send_timing[e.topic_message_name] = new TopicCycle(e.pub_option.cycle_scale);
                    }
                    else
                    {
                        topic_send_timing[e.topic_message_name] = new TopicCycle(1);
                    }
                }
            }
			RostopicPublisherOption option = null;

			option = GetPubOption("scan");
			if (option != null) {
				ros.RegisterPublisher<LaserScanMsg>("scan", option.queue_size, option.latch);
			}
			else {
				ros.RegisterPublisher<LaserScanMsg>("scan");
			}
			option = GetPubOption("image");
			if (option != null) {
				ros.RegisterPublisher<ImageMsg>("image", option.queue_size, option.latch);
			}
			else {
				ros.RegisterPublisher<ImageMsg>("image");
			}
			option = GetPubOption("image/compressed");
			if (option != null) {
				ros.RegisterPublisher<CompressedImageMsg>("image/compressed", option.queue_size, option.latch);
			}
			else {
				ros.RegisterPublisher<CompressedImageMsg>("image/compressed");
			}
			option = GetPubOption("camera_info");
			if (option != null) {
				ros.RegisterPublisher<CameraInfoMsg>("camera_info", option.queue_size, option.latch);
			}
			else {
				ros.RegisterPublisher<CameraInfoMsg>("camera_info");
			}
			option = GetPubOption("imu");
			if (option != null) {
				ros.RegisterPublisher<ImuMsg>("imu", option.queue_size, option.latch);
			}
			else {
				ros.RegisterPublisher<ImuMsg>("imu");
			}
			option = GetPubOption("odom");
			if (option != null) {
				ros.RegisterPublisher<OdometryMsg>("odom", option.queue_size, option.latch);
			}
			else {
				ros.RegisterPublisher<OdometryMsg>("odom");
			}
			option = GetPubOption("tf");
			if (option != null) {
				ros.RegisterPublisher<TFMessageMsg>("tf", option.queue_size, option.latch);
			}
			else {
				ros.RegisterPublisher<TFMessageMsg>("tf");
			}
			option = GetPubOption("joint_states");
			if (option != null) {
				ros.RegisterPublisher<JointStateMsg>("joint_states", option.queue_size, option.latch);
			}
			else {
				ros.RegisterPublisher<JointStateMsg>("joint_states");
			}
            ros.Subscribe<TwistMsg>("cmd_vel", TwistMsgChange);

        }


        private void TwistMsgChange(TwistMsg obj)
        {
            this.topic_data_table["cmd_vel"] = obj;
        }

        public void Publish(IPduCommTypedData data)
        {
            RosTopicPduCommTypedData typed_data = data as RosTopicPduCommTypedData;
            topic_send_timing[typed_data.GetDataName()].count++;
            if (topic_send_timing[typed_data.GetDataName()].count >= topic_send_timing[typed_data.GetDataName()].cycle)
            {
                ros.Publish(typed_data.GetDataName(), typed_data.GetTopicData());
                topic_send_timing[typed_data.GetDataName()].count = 0;
            }
        }
        
        private void Reset()
        {

            this.topic_data_table["cmd_vel"] = null;
        }

        public IPduCommTypedData Recv(string topic_name)
        {
        	if (topic_name == null) {
        		this.Reset();
        		return null;
        	}
            var cfg = AssetConfigLoader.GetRosTopic(topic_name);
            if (cfg == null)
            {
                throw new System.NotImplementedException();
            }
            else if (this.topic_data_table[topic_name] == null)
            {
                return null;
            }
            return new RosTopicPduCommTypedData(topic_name, cfg.topic_type_name, this.topic_data_table[topic_name]);
        }
    }

}
