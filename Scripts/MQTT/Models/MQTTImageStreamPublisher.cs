using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using OCR.TextureEnhancement;
using MRModels;

namespace MQTT.Models 
{
    public class MQTTImageStreamPublisher: MQTTPublisher
    {
        private static string[] TOPICS = new string[] { "device/holo/image" };

        [Header("Source")]
        [SerializeField]
        private Camera cam;
        private WebCamDevice wcam;
        private RenderTexture _render;

        [Header("Quality")]
        [SerializeField]
        private int xResolution;
        private RescaleEnhancement _scaler;

        [Header("[deprecated] Stream")]
        [SerializeField]
        private bool listen;
        [SerializeField]
        private RawImage display;

        public override string[] Topics => TOPICS;

        public override void PublishMessage(string topic, string message)
        {
            base.PublishMessage(topic, message);
        }

        protected override void Awake()
        {
            base.Awake();

            if (this.cam == null || this.display == null)
                Debug.Log("Error raise on Inspector - object(s) reference(s) needed.");

            this._render = null;

            this._scaler = new RescaleEnhancement();
            this._scaler.TargedWidth = this.xResolution;
        }

        private void UpdateRender()
        {
            int width = this.cam.pixelWidth,
                height = this.cam.pixelHeight;

            //this.StartCoroutine(this.UpdateRender(width, height));

            this._render = new RenderTexture(width, height, 32);
            this.cam.targetTexture = this._render;
            this.cam.Render();

            this.cam.targetTexture = null;
        }

        private void LateUpdate()
        {
            if (this._render != null)
                return;

            this.UpdateRender();

            int width = this._render.width,
                height = this._render.height;

            Texture2D cTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

            RenderTexture.active = this._render;
            cTexture.ReadPixels(new Rect(0, 0, this._render.width, this._render.height), 0, 0);
            RenderTexture.active = null;
            this._render = null;

            // ajusta escala
            this._scaler.Enhance(ref cTexture);
            width = cTexture.width;
            height = cTexture.height;

            //cTexture.Apply();
            //this.display.texture = cTexture;

            if (base.MQTTCC.IsConnected) {
                byte[] byteColors;
                Utilities.TextureUtilities.ColorsToBytes(width, height, cTexture.GetPixels32(), out byteColors);
                MRModels.Image img = new MRModels.Image(byteColors, width, height);
                this.PublishMessage(this.Topics[0], UnityEngine.JsonUtility.ToJson(img, true));
            }

        }
    }
}
