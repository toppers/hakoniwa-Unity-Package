using Assets.Scripts.Hakoniwa.PluggableAsset.Assets.Robot;
using Assets.Scripts.Hakoniwa.PluggableAsset.Assets.Robot.TB3;
using Hakoniwa.Core.Utils;
using Hakoniwa.PluggableAsset.Assets.Robot;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.TB3
{
    public class CameraSensor : MonoBehaviour, ICameraSensor
    {
        private GameObject sensor;
        public RenderTexture RenderTextureRef;
        //public string saveFilePath = "./SavedScreen.jpeg";
        private Texture2D tex;
        private byte[] bytes;

        public void Initialize(object root)
        {
            this.sensor = (GameObject)root;
        }

        public void UpdateSensorValues()
        {
            tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.RGB24, false);
            RenderTexture.active = RenderTextureRef;
            tex.ReadPixels(new Rect(0, 0, RenderTextureRef.width, RenderTextureRef.height), 0, 0);
            tex.Apply();
            // Encode texture into JPG
            bytes = tex.EncodeToJPG();
            Object.Destroy(tex);
            //File.WriteAllBytes(saveFilePath, bytes);
        }
        public void UpdateSensorData(Pdu pdu)
        {
            TimeStamp.Set(pdu);
            pdu.Ref("header").SetData("frame_id", "camera"); //TODO
            pdu.SetData("format", "jpeg");
            pdu.SetData("data", bytes);
        }
    }
}

