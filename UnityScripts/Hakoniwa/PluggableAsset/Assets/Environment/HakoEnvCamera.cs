using Assets.Scripts.Hakoniwa.PluggableAsset.Assets.Robot;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public class HakoEnvCamera : MonoBehaviour, IHakoEnvCamera
    {
        private Camera my_camera;
        public RenderTexture RenderTextureRef;
        private Texture2D tex;
        private byte[] raw_bytes;
        private byte[] jpg_bytes;
        private string frame_id = "camera_link";
        private int width = 640;
        private int height = 480;

        public void Initialize(object root)
        {
            Debug.Log("my name=" + this.name);
            
            this.my_camera = this.GetComponent<Camera>();
            var texture = new Texture2D(this.width, this.height, TextureFormat.RGB24, false);
            this.RenderTextureRef = new RenderTexture(texture.width, texture.height, 32);
            this.my_camera.targetTexture = this.RenderTextureRef;
        }

        public string GetAssetName()
        {
            return this.name;
        }


        public void UpdateSensorValues(Pdu pdu)
        {
            this.UpdateSensorValues();
            if (pdu.GetName() == "sensor_msgs/CompressedImage")
            {
                TimeStamp.Set(pdu);
                pdu.Ref("header").SetData("frame_id", frame_id);
                pdu.SetData("format", "jpeg");
                pdu.SetData("data", jpg_bytes);
            }
            else
            {
                Debug.LogError("MSG Type is Not Found: " + pdu.GetName());
            }
        }

        private void UpdateSensorValues()
        {
            tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.RGB24, false);
            RenderTexture.active = RenderTextureRef;
            int width = RenderTextureRef.width;
            int height = RenderTextureRef.height;
            int step = width * 3;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            // raw_bytes = tex.GetRawTextureData();

            // Raw Image RGB24=(ROS)rgb8
            byte[] _byte = tex.GetRawTextureData();
            raw_bytes = new byte[_byte.Length];
            for (int i = 0; i < height; i++)
            {
                System.Array.Copy(_byte, i * step, raw_bytes, (height - i - 1) * step, step);
            }

            // Encode texture into JPG
            jpg_bytes = tex.EncodeToJPG();
            Object.Destroy(tex);
        }
    }
}